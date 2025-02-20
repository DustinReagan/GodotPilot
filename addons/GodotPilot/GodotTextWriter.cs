using System.Text;
using Godot;

namespace GodotPilotPlugin;
public class GodotTextWriter : TextWriter
{
    private StringBuilder _buffer = new StringBuilder();

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value == '\n')
        {
            // flush the buffer when we hit a newline
            GD.Print(_buffer.ToString());
            _buffer.Clear();
        }
        else
        {
            _buffer.Append(value);
        }
    }

    public override void WriteLine(string? value)
    {
        GD.PrintRich($"[color=green]{value}[/color]");
    }
}

public static class ConsoleRedirect
{
    public static void RedirectToGodot()
    {
        Console.SetOut(new GodotTextWriter());
    }
}