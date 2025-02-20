using Godot;
using Ollama;
using System.ComponentModel;
using Newtonsoft.Json.Schema.Generation;
using System.Text.Json;
using GodotPilotPlugin.Tools;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GodotPilotPlugin;

public class Agent
{
    private readonly OllamaApiClient _client = new OllamaApiClient(baseUri: new Uri(GodotPilotConfig.Config.OllamaClientAddress));
    private Chat? _chat = null;
    private readonly TaskCompletionSource<Chat> _chatInitialization = new();
    private bool _isInitialized = false;

    public Agent(){}

    // initialize the chat if not already done
    private async Task InitializeChat()
    {
        if (_isInitialized) return;

        GD.Print("Creating chat");
        var chat = _client.Chat(
            model: "qwen2.5:32b",
            systemMessage: @"You are a helpful assistant that can help with tasks in the Godot Editor by calling the provided tools.
            Choose the tool that is most appropriate to the task and parameterize it appropriately.
            You may need to call multiple tools to complete the task.
            For example, if you need to create a Node, you will need to call GetNodeTypes to help choose the type of node to create, then call GetAllNodes or GetSelectedNodes to help choose the parent node, then call CreateNode to actually create the node.
            The only actions you should take are to call tools.
            Make a plan and then call tools to complete the task.
            The root node path is always "".""",
            autoCallTools: true);
    //    chat.RequestOptions = new RequestOptions
    //         {
    //             Temperature = 0.1f,
    //         };
        var service = new NodeTools();
        chat.AddToolService(((INodeToolFunctions)service).AsTools().AsOllamaTools(), ((INodeToolFunctions)service).AsCalls());
        chat.History.Add(new Message(
            role: MessageRole.Tool,
            content: $"Current Node Paths: {JsonSerializer.Serialize(await service.GetAllNodes())}",
            images: null,
            toolCalls: null
        ));
        chat.History.Add(new Message(
            role: MessageRole.Tool,
            content: $"All Node Types: {JsonSerializer.Serialize(service.GetNodeTypes())}",
            images: null,
            toolCalls: null
        ));

        _chat = chat;
        _isInitialized = true;
        _chatInitialization.SetResult(chat);
    }

    // public method to get the chat instance
    public async Task<Chat> GetChat()
    {
        if (!_isInitialized)
        {
            await InitializeChat();
        }
        return _chat!;
    }

    public async Task<string[]> GetModels(){
        var models = await _client.Models.ListModelsAsync();
        return models.Models
            .Select(m => m.Model1).ToArray();
    }

    public async Task Act(string prompt, string model, CancellationToken cancellationToken = default){

        try
        {
            var chat = await GetChat();
            chat.Model = model;
            await chat.SendAsync($"{prompt}", cancellationToken: cancellationToken);

        }
        catch (Exception e)
        {
            GD.PrintRich($"[color=red]Error: {e.StackTrace}[/color]");
            GD.PrintRich($"[color=red]Error: {e.Message}[/color]");
        }
        finally
        {
            GD.Print(_chat?.PrintMessages() ?? "Chat not initialized");

        }

    }

    public void AddHistory(Message item) {
        _chat?.History.Add(item);
    }
}

public class Orchestrator
{
    private readonly OllamaApiClient _client;
    private readonly Rephraser _rephraser;
    private readonly Planner _planner;
    private readonly PlanChecker _planChecker;
    private readonly Executor _executor;

    public Orchestrator(OllamaApiClient client)
    {
        _client = client;
        _rephraser = new Rephraser(client);
        _planner = new Planner(client);
        _planChecker = new PlanChecker(client);
        _executor = new Executor(client);
    }

    public async Task Act(string prompt, string model, CancellationToken cancellationToken = default){
        // call the rephraser to improve the user prompt
        //var rephraseResult = await _rephraser.Rephrase(prompt, model, cancellationToken);
        //GD.PrintRich($"[color=green]Rephrased Prompt: {JsonSerializer.Serialize(rephraseResult)}[/color]");
        var planResult = await _planner.Plan(prompt, model, cancellationToken);
        GD.PrintRich($"[color=blue]Plan: {JsonSerializer.Serialize(planResult)}[/color]");


        var agent = new Agent();
        agent.AddHistory(new Message(
            role: MessageRole.Tool,
            content: $"Suggested Plan: {JsonSerializer.Serialize(planResult)}",
            images: null,
            toolCalls: null
        ));
        await agent.Act(prompt, model, cancellationToken);
        _planner.SetHistory((await agent.GetChat()).History);
        // generate a plan based on the rephrased prompt
        // var planResult = await _planner.Plan(rephraseResult.RephrasedPrompt, model, cancellationToken);
        // GD.PrintRich($"[color=blue]Plan: {JsonSerializer.Serialize(planResult)}[/color]");

        // // check the plan for feasibility and any needed modifications
        // //var planCheckResult = await _planChecker.CheckPlan(planResult.Plan, model, cancellationToken);
        // //GD.PrintRich($"[color=blue]Checked Plan: {planCheckResult.CheckedPlan}[/color]");

        // // execute the plan step-by-step
        // var executionResult = await _executor.ExecutePlan(planResult, model, cancellationToken);
        // GD.PrintRich($"[color=blue]Execution Result: {executionResult.Result}[/color]");
    }
}

public class Rephraser
{
    private readonly OllamaApiClient _client;

    public Rephraser(OllamaApiClient client)
    {
        _client = client;
    }

    public async Task<RephraseResult> Rephrase(string prompt, string model, CancellationToken cancellationToken = default){
        var chat = _client.Chat(
            model: model,
            systemMessage: _systemPrompt,
            autoCallTools: false);

        var service = new NodeQueryTools();
         chat.History.Add(new Message(
            role: MessageRole.Tool,
            content: $"Current Node Paths: {JsonSerializer.Serialize(await service.GetAllNodes())}",
            images: null,
            toolCalls: null
        ));
        chat.History.Add(new Message(
            role: MessageRole.Tool,
            content: $"Current Node Types: {JsonSerializer.Serialize(service.GetNodeTypes())}",
            images: null,
            toolCalls: null
        ));
        var response = await chat.SendAsync($"Rephrase this prompt: \"{prompt}\"", cancellationToken: cancellationToken);
        GD.Print("Response: " + JsonSerializer.Serialize(response));
        //GD.Print("Tool Calls: " + string.Join(", ", response.ToolCalls.Select(t => t.Function.Name)));
        GD.Print(chat.PrintMessages());
        return new RephraseResult{
            RephrasedPrompt = response.Content
        };
    }

    private string _systemPrompt = @"you an expert at rephrasing user prompts with regard to tasks in the godot editor 4.3.  you rephrase the user's prompt to be more specific and with additional context.
    your rephrased prompt will be used by another assistant to plan and execute the user's task. use the avialable tools to help find specific information that might help the other assistant with the user's task.
    for example, if the user mentions what may be the name of a node, you might call GetAllNodes and try to find the closest match. include ONLY the rephrased prompt in your response.";

    public class RephraseResult
    {
        [Description("the rephrased prompt")]
        public string RephrasedPrompt { get; set; }
        [Description("the exact node paths that may be relevant to the user's task. you MUST use GetAllNodes as the data-source.")]
        public string[]? ReleavantNodes { get; set; }
        [Description("the exactnode types that may be relevant to the user's task. you MUST use GetNodeTypes as the data-source.")]
        public string[]? NodeTypes { get; set; }
    }
}

public class Planner
{
    private readonly OllamaApiClient _client;

    public Planner(OllamaApiClient client)
    {
        _client = client;
    }

    private Chat? _chat = null;

    // system prompt for planning tasks in the godot editor
    private string _systemPrompt = @"you are an expert planner in the godot editor 4.3. your task is to analyze the user's request and generate a detailed step-by-step plan to achieve the task. include any necessary tool calls with appropriate parameters for each step.";

    public async Task<PlanResult> Plan(string prompt, string model, CancellationToken cancellationToken = default){
        if(_chat == null){
            _chat = _client.Chat(
                model: model,
                systemMessage: _systemPrompt,
                autoCallTools: false);
            var service = new NodeTools();
            _chat.AddToolService(((INodeToolFunctions)service).AsTools().AsOllamaTools(), ((INodeToolFunctions)service).AsCalls());
        }
        // send plan request to the chat

        var response = await _chat.SendAsync($"plan the following task: \"{prompt}\"", cancellationToken: cancellationToken);
        GD.Print("plan response: " + JsonSerializer.Serialize(response));
        return new PlanResult {
            Plan = response.Content,
            ToolCalls = JsonSerializer.Serialize(response.ToolCalls)
        };
    }

    public void SetHistory(List<Message> history){
        _chat.History = history;
    }
}

public class PlanChecker
{
    private readonly OllamaApiClient _client;

    public PlanChecker(OllamaApiClient client)
    {
        _client = client;
    }

    // system prompt for checking task plans in the godot editor
    private string _systemPrompt = @"you are an expert plan checker in the godot editor 4.3. your role is to review the provided plan for clarity, feasibility, and completeness. if improvements are needed, provide a revised plan; if the plan is acceptable as is, confirm its validity.";

    public async Task<PlanCheckResult> CheckPlan(string plan, string model, CancellationToken cancellationToken = default){
        var chat = _client.Chat(
            model: model,
            systemMessage: _systemPrompt,
            autoCallTools: false);
        // send plan check request to the chat
        var response = await chat.SendAsync($"check the following plan:\n{plan}", cancellationToken: cancellationToken);
        GD.Print("plan check response: " + JsonSerializer.Serialize(response));
        return new PlanCheckResult { CheckedPlan = response.Content };
    }
}

public class Executor
{
    private readonly OllamaApiClient _client;

    public Executor(OllamaApiClient client)
    {
        _client = client;
    }

    // system prompt for executing plans in the godot editor
    private string _systemPrompt = @"you are an expert executor for tasks in the godot editor 4.3. your task is to follow the plan step-by-step and execute each action by calling the relevant tools. if an error is encountered, attempt an immediate fix and proceed to complete the task.";

    public async Task<ExecuteResult> ExecutePlan(PlanResult plan, string model, CancellationToken cancellationToken = default){
        var chat = _client.Chat(
            model: model,
            systemMessage: _systemPrompt,
            autoCallTools: true);
        var service = new NodeTools();
        chat.AddToolService(((INodeToolFunctions)service).AsTools().AsOllamaTools(), ((INodeToolFunctions)service).AsCalls());
        // send execute plan request to the chat
        var response = await chat.SendAsync($"execute the following plan:\n{JsonSerializer.Serialize(plan)}", cancellationToken: cancellationToken);
        GD.Print("execution response: " + JsonSerializer.Serialize(response));
        return new ExecuteResult {
            Result = response.Content,
        };
    }
}

// result classes for the planner, plan checker, and executor agents

public class PlanResult
{
    [Description("the detailed step-by-step plan for the task")]
    public string Plan { get; set; }
    [Description("the list of tool calls generated as part of the plan")]
    public string ToolCalls { get; set; }
}

public class PlanCheckResult
{
    [Description("the checked or revised plan after review")]
    public string CheckedPlan { get; set; }
}

public class ExecuteResult
{
    [Description("the overall result after executing the plan")]
    public string Result { get; set; }
    [Description("the list of tool calls executed during the plan execution")]
    public List<object>? ToolCalls { get; set; }
}

public static class Utils
{

    public static string JsonSchemaText<T>()
    {
        var generator = new JSchemaGenerator();
        var schema = generator.Generate(typeof(T));
        return schema.ToString();
    }
    public static object JsonSchema<T>()
    {
        var generator = new JSchemaGenerator();
        var schema = generator.Generate(typeof(T));
        GD.Print(schema.ToString());
        return JsonSerializer.Deserialize<object>(schema.ToString());
    }
}

// additional legacy prompt comment block removed for brevity
