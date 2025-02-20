// Copyright (c)  2023  Xiaomi Corporation
//
// This file shows how to use a streaming model for real-time speech
// recognition from a microphone.
// Please refer to
// https://k2-fsa.github.io/sherpa/onnx/pretrained_models/online-transducer/zipformer-transducer-models.html
// to download streaming models

using Godot;
using CommandLine;
using CommandLine.Text;
using PortAudioSharp;
using SherpaOnnx;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

class OfflineRecordingAndTranscription
{
    // move Options to be a private inner class since it's only used internally
    private class Options
    {
        [Option(Required = false, Default = "cpu", HelpText = "Provider, e.g., cpu, coreml")]
        public string? Provider { get; set; }

        [Option(Required = false, HelpText = "Path to transducer encoder.onnx")]
        public string? Encoder { get; set; }

        [Option(Required = false, HelpText = "Path to transducer decoder.onnx")]
        public string? Decoder { get; set; }

        [Option(Required = false, HelpText = "Path to transducer joiner.onnx")]
        public string? Joiner { get; set; }

        [Option("paraformer-encoder", Required = false, HelpText = "Path to paraformer encoder.onnx")]
        public string? ParaformerEncoder { get; set; }

        [Option("paraformer-decoder", Required = false, HelpText = "Path to paraformer decoder.onnx")]
        public string? ParaformerDecoder { get; set; }

        [Option("num-threads", Required = false, Default = 1, HelpText = "Number of threads for computation")]
        public int NumThreads { get; set; }

        [Option("decoding-method", Required = false, Default = "greedy_search",
                HelpText = "Valid decoding methods are: greedy_search, modified_beam_search")]
        public string? DecodingMethod { get; set; }

        [Option(Required = false, Default = false, HelpText = "True to show model info during loading")]
        public bool Debug { get; set; }

        [Option("sample-rate", Required = false, Default = 16000, HelpText = "Sample rate of the data used to train the model")]
        public int SampleRate { get; set; }

        [Option("max-active-paths", Required = false, Default = 4,
            HelpText = @"Used only when --decoding--method is modified_beam_search.
It specifies number of active paths to keep during the search")]
        public int MaxActivePaths { get; set; }

        [Option("enable-endpoint", Required = false, Default = true,
            HelpText = "True to enable endpoint detection.")]
        public bool EnableEndpoint { get; set; }

        [Option("rule1-min-trailing-silence", Required = false, Default = 2.4F,
            HelpText = @"An endpoint is detected if trailing silence in seconds is
larger than this value even if nothing has been decoded. Used only when --enable-endpoint is true.")]
        public float Rule1MinTrailingSilence { get; set; }

        [Option("rule2-min-trailing-silence", Required = false, Default = 0.8F,
            HelpText = @"An endpoint is detected if trailing silence in seconds is
larger than this value after something that is not blank has been decoded. Used
only when --enable-endpoint is true.")]
        public float Rule2MinTrailingSilence { get; set; }

        [Option("rule3-min-utterance-length", Required = false, Default = 20.0F,
            HelpText = @"An endpoint is detected if the utterance in seconds is
larger than this value. Used only when --enable-endpoint is true.")]
        public float Rule3MinUtteranceLength { get; set; }

        [Option(Required = false, HelpText = "Path to tokens.txt", Default = "res://addons/GodotPilot/models/whisper/base.en-tokens.txt")]
        public string? Tokens { get; set; }

        [Option("whisper-encoder", Required = false, HelpText = "Path to whisper encoder.onnx", Default = "res://addons/GodotPilot/models/whisper/base.en-encoder.onnx")]
        public string? WhisperEncoder { get; set; }

        [Option("whisper-decoder", Required = false, HelpText = "Path to whisper decoder.onnx", Default = "res://addons/GodotPilot/models/whisper/base.en-decoder.onnx")]
        public string? WhisperDecoder { get; set; }

        [Option("whisper-language", Required = false, HelpText = "Language for whisper model", Default = "en")]
        public string? WhisperLanguage { get; set; }

        [Option("whisper-task", Required = false, HelpText = "Task for whisper model", Default = "transcribe")]
        public string? WhisperTask { get; set; }
    }

    private readonly Options _options;
    private readonly OfflineRecognizer _recognizer;
    private PortAudioSharp.Stream? _audioStream;
    private List<float> _recordedSamples;
    private bool _isRecording;

    public OfflineRecordingAndTranscription()
    {
        // initialize with default options
        _options = new Options
        {
            Provider = "coreml",
            NumThreads = 3,
            DecodingMethod = "greedy_search",
            Debug = false,
            SampleRate = 16000,
            MaxActivePaths = 4,
            EnableEndpoint = true,
            Rule1MinTrailingSilence = 2.4F,
            Rule2MinTrailingSilence = 0.8F,
            Rule3MinUtteranceLength = 20.0F,
            Tokens = "res://addons/GodotPilot/models/whisper/base.en-tokens.txt",
            WhisperEncoder = "res://addons/GodotPilot/models/whisper/base.en-encoder.onnx",
            WhisperDecoder = "res://addons/GodotPilot/models/whisper/base.en-decoder.onnx",
            WhisperLanguage = "en",
            WhisperTask = "transcribe"
        };

        GD.PrintErr("Current Working Directory: " + System.Environment.CurrentDirectory);
        GD.PrintErr("Whisper Encoder Path: " + ProjectSettings.GlobalizePath(_options.WhisperEncoder));
        GD.PrintErr("File Exists: " + System.IO.File.Exists(ProjectSettings.GlobalizePath(_options.WhisperEncoder)));

        // initialize recognizer
        var config = new OfflineRecognizerConfig();
        config.FeatConfig.SampleRate = _options.SampleRate;
        config.ModelConfig.Provider = _options.Provider;

        // convert Godot resource paths to actual file paths
        config.ModelConfig.Tokens = ProjectSettings.GlobalizePath(_options.Tokens);
        config.ModelConfig.Whisper.Encoder = ProjectSettings.GlobalizePath(_options.WhisperEncoder);
        config.ModelConfig.Whisper.Decoder = ProjectSettings.GlobalizePath(_options.WhisperDecoder);
        config.ModelConfig.Whisper.Language = _options.WhisperLanguage;
        config.ModelConfig.Whisper.Task = _options.WhisperTask;


        // create recognizer
        _recognizer = new OfflineRecognizer(config);
        _recordedSamples = new List<float>();
        _isRecording = false;
    }





    private PortAudioSharp.Stream.Callback _audioCallback;

    public void StartRecording()
    {

        if (_isRecording)
        {
            throw new InvalidOperationException("recording is already in progress");
        }


        Console.WriteLine(PortAudio.VersionInfo.versionText);
        PortAudio.Initialize();



        int deviceIndex = PortAudio.DefaultInputDevice;
        if (deviceIndex == PortAudio.NoDevice)
        {
            throw new Exception("no default input device found");
        }

        var info = PortAudio.GetDeviceInfo(deviceIndex);
        Console.WriteLine($"Use default device {deviceIndex} ({info.name})");

        var param = new StreamParameters();
        param.device = deviceIndex;
        param.channelCount = 1;
        param.sampleFormat = SampleFormat.Float32;
        param.suggestedLatency = info.defaultLowInputLatency;

        _recordedSamples = new List<float>();

        _audioCallback = (IntPtr input, IntPtr output, uint frameCount,
                      ref StreamCallbackTimeInfo timeInfo,
                      StreamCallbackFlags statusFlags, IntPtr userData) =>
        {
            if (input == IntPtr.Zero)
                return StreamCallbackResult.Continue;

            var samples = new float[frameCount];
            Marshal.Copy(input, samples, 0, (int)frameCount);
            _recordedSamples.AddRange(samples);

            return StreamCallbackResult.Continue;
        };

        _audioStream = new PortAudioSharp.Stream(
            inParams: param,
            outParams: null,
            sampleRate: _options.SampleRate,
            framesPerBuffer: 0,
            streamFlags: StreamFlags.ClipOff,
            callback: _audioCallback,
            userData: IntPtr.Zero);

        _audioStream.Start();
        _isRecording = true;
    }

    public string StopAndProcess()
    {
        if (!_isRecording || _audioStream == null)
        {
            throw new InvalidOperationException("no recording in progress");
        }

        _audioStream.Stop();
        _audioStream.Close();
        _audioStream.Dispose();
        _audioStream = null;
        _isRecording = false;

        //process recorded audio
        var offlineStream = _recognizer.CreateStream();
        offlineStream.AcceptWaveform(_options.SampleRate, _recordedSamples.ToArray());

        _recognizer.Decode(new List<OfflineStream> { offlineStream });

        var result = offlineStream.Result;
        return result.Text;

    }

    // legacy Run method that combines both operations
    public string Run()
    {
        StartRecording();
        Console.WriteLine("Recording... Press Enter to stop");
        Console.ReadLine(); // wait for Enter key
        return StopAndProcess();
    }

    public void Dispose()
    {
        if (_audioStream != null)
        {
            try
            {
                if (_isRecording)
                {
                    StopAndProcess();
                }
                _audioStream.Dispose();
                _audioStream = null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during disposal: {e.Message}");
            }
        }

        _recognizer?.Dispose();
    }
}