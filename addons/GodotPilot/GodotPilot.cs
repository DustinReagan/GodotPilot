#if TOOLS
using Godot;
using Ollama;
using GodotPilotPlugin;
using GodotPilotPlugin.Tools;
using GodotPilotPlugin.Grpc;
using System.Text;
using System.Text.Json;

namespace GodotPilotPlugin;
[Tool]
public partial class GodotPilot : EditorPlugin
{
	private Control _control;

	private Node _outputText;
	private OfflineRecordingAndTranscription _offlineTranscription;
	private GrpcServer? _grpcServer;
	private Task _recordingTask;
	private Agent _agent = new Agent();
	private System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri("http://localhost:8082") };

	private Orchestrator _orchestrator;
	private CancellationTokenSource? _currentCancellation;
	private GodotPilotConfig _config;

	private int _chatIndex = 0;
	public override void _EnterTree()
	{
		GD.Print("GodotPilot is initializing!");
		ConsoleRedirect.RedirectToGodot();
		_control = GD.Load<PackedScene>("res://addons/GodotPilot/GodotPilot.tscn").Instantiate() as Control ?? throw new Exception("Failed to load GodotPilot.tscn!");

		_outputText = _control.GetNode("TabContainer/Output");
		try{
			_offlineTranscription = new OfflineRecordingAndTranscription();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Failed to initialize offline transcription: {e.Message}");
		}

		InitializeSignals();

		AddControlToDock(DockSlot.LeftUl, _control);

		StartGrpcServerDeferred();
	}

	public override async void _ExitTree()
	{
		RemoveControlFromDocks(_control);
		_control.QueueFree();

		_currentCancellation?.Cancel();
		_currentCancellation?.Dispose();
		if(_grpcServer != null) {
			await _grpcServer.StopAsync();
		}

		GD.PrintErr("Plugin is disabled!");
	}

	private async Task PopulateOptionButton(OptionButton modelMenu){
		var models = await _agent.GetModels();
		modelMenu.CallDeferred("clear");
		foreach (var model in models)
		{
			modelMenu.CallDeferred("add_item", model);
		}
	}

	private async Task StartGrpcServerDeferred(CancellationToken cancellationToken=default) {
		try {
			// initialize and start the grpc server asynchronously
			_grpcServer = new GrpcServer();
			await _grpcServer.StartAsync(cancellationToken);
		}
		catch (Exception e) {
			GD.PrintErr($"failed to start grpc server: {e.Message}");
		}
	}

	private void InitializeSignals()
	{
			// get references to controls
		var listenButton = _control.GetNode<Button>("ListenButton");
		var submitButton = _control.GetNode<Button>("SubmitButton");
		var textEdit = _control.GetNode<TextEdit>("TextEdit");

		var statusText = _control.GetNode<TextEdit>("StatusText");
		var cancelButton = _control.GetNode<Button>("CancelButton");
		var clearButton = _control.GetNode<Button>("ClearButton");

		var modelMenu = _control.GetNode<OptionButton>("ModelMenu");

		_outputText.Set("markdown_text", "## sample markdown content\n- item1\n- item2");
		var toolUsageText = _control.GetNode<TextEdit>("TabContainer/ToolUse");
		Task.Run(async () => await PopulateOptionButton(modelMenu));
		// connect button signals
		listenButton.ButtonDown += () =>
		{
			try
			{
				// start the recording in a background thread
				_recordingTask = Task.Run(() =>
				{
					_offlineTranscription.StartRecording();
				});
			}
			catch (Exception e)
			{
				GD.PrintErr($"Failed to start recording: {e.Message}");
			}
		};

		listenButton.ButtonUp += () =>
		{
			try
			{
				// continue in the same task
				_recordingTask = _recordingTask.ContinueWith(task =>
				{
					string result = _offlineTranscription.StopAndProcess();
					textEdit.CallDeferred("set_text", result);
				});
			}
			catch (Exception e)
			{
				GD.Print($"Failed to process recording: {e.Message}");
			}
		};

		clearButton.ButtonDown += () =>
		{
			_chatIndex++;
		};

		submitButton.ButtonDown += () =>
		{
			try
			{
				// cancel any existing operation and create a new cancellation token source
				_currentCancellation?.Cancel();
				_currentCancellation?.Dispose();
				_currentCancellation = new CancellationTokenSource();

				Task.Run(async () =>
				{
					try
					{
						// update status on main thread
						statusText.CallDeferred("set_text", "thinking...");
						// await the chat messages
						var messages = await SendActRequest(
							textEdit.Text,
							modelMenu.GetItemText(modelMenu.Selected),
							_chatIndex,
							_currentCancellation.Token
						);

						// move ui update here; update the output node for any ai messages
						foreach (var message in messages)
						{
							if (message.Type == "ai")
							{
								updateOutput(message.Content ?? "");
							}
						}

						if (!_currentCancellation.Token.IsCancellationRequested)
						{
							statusText.CallDeferred("set_text", "done!");
							GD.Print($"response: {messages}");
						}
						else
						{
							statusText.CallDeferred("set_text", "Cancelled!");
						}
					}
					catch (OperationCanceledException)
					{
						statusText.CallDeferred("set_text", "Cancelled!");
					}
					catch (Exception e)
					{
						statusText.CallDeferred("set_text", $"Error: {e.Message}");
						GD.PrintErr($"Failed to process: {e.Message}");
					}
				});
			}
			catch (Exception e)
			{
				GD.PrintErr($"Failed to process recording: {e.Message}");
			}
		};

		cancelButton.ButtonDown += () =>
		{
			if (_currentCancellation?.Token.IsCancellationRequested == false)
			{
				_currentCancellation?.Cancel();
				statusText.CallDeferred("set_text", "Cancelling...");
			}
		};
	}

	private async Task<List<ChatMessage>> SendActRequest(string prompt, string model, int chatId, CancellationToken cancellationToken)
	{
		try
		{
			// create request body
			var requestBody = new
			{
				prompt = prompt,
				id = chatId,
				model = model
			};

			var content = new StringContent(
				JsonSerializer.Serialize(requestBody),
				Encoding.UTF8,
				"application/json"
			);

			// send request
			var response = await _httpClient.PostAsync("/act", content, cancellationToken);
			response.EnsureSuccessStatusCode();

			// parse response
			var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
			GD.Print($"Response: {responseString}");


			try
			{
				var messages = JsonSerializer.Deserialize<List<ChatMessage>>(responseString);
				if (messages == null)
					throw new Exception("failed to deserialize chat messages");

				return messages;
			}
			catch (JsonException je)
			{
				// log detailed error information
				GD.PrintErr($"JSON parsing error: {je.Message}");
				GD.PrintErr($"Response content: {responseString}");
				GD.PrintErr($"Path: {je.Path}, Line: {je.LineNumber}, Position: {je.BytePositionInLine}");
				throw new Exception($"Failed to parse response: {je.Message}\nResponse: {responseString}");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Request failed: {ex.Message}");
			GD.PrintErr($"Stack trace: {ex.StackTrace}");
			throw new Exception($"Request failed: {ex.Message}", ex);
		}
	}

	// updateOutput method in GodotPilot class; ensures the UI update happens on the main thread
	private void updateOutput(string content)
	{
		MainThreadHelper.RunOnMainThread(() => {
			// using call_deferred on the output node to ensure the function is executed on the main thread
			_outputText.Set("markdown_text", content);
		});
	}
}

#endif
