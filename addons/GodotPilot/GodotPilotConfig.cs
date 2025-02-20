using Godot;
using Godot.Collections;

namespace GodotPilotPlugin;

public class GodotPilotConfig
{
    public string OllamaClientAddress { get; set; } = "http://localhost:11434/api";
    private static GodotPilotConfig LoadConfig()
    {
        var editorSettings = EditorInterface.Singleton.GetEditorSettings();
        const string settingKey = "godot_pilot/ollama_client_address";

        // if the setting does not exist, set a default value
        if (!editorSettings.HasSetting(settingKey))
        {
            editorSettings.Set(settingKey, "localhost:11434");
        }

        // add property info so that the setting appears in the Editor Settings UI
        // var propertyInfo = new Godot.Collections.Dictionary
        // {
        //     ["name"] = settingKey,
        //     ["type"] = Variant.Type.String,
        //     ["hint"] = PropertyHint.None,
        //     ["usage"] = (int)PropertyUsageFlags.Default
        // };
        // editorSettings.AddPropertyInfo(propertyInfo);

        string ollamaClientAddress = (string)editorSettings.Get(settingKey);
        GD.Print("editor settings ollama client address: " + ollamaClientAddress);

        return new GodotPilotConfig
        {
            OllamaClientAddress = $"http://{ollamaClientAddress}/api"
        };
    }

    private GodotPilotConfig()
    {
    }

    public static GodotPilotConfig Config { get; private set; } = LoadConfig();
}
