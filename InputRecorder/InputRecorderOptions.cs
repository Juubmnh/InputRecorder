using System.Text.RegularExpressions;

namespace InputRecorder;

internal sealed class InputRecorderOptions
{
    public const string CONFIG_FILENAME = "Config.ini";
    public bool KeyTypedEnabled { get; set; } = true;
    public string FilterSwitch { get; set; } = "^VcF12$";
    public string Filter { get; set; } = "^Button1$";
    public string OutputFile { get; set; } = $"Subtitles\\Subtitle {DateTime.Now.Year}.{DateTime.Now.Month:D2}.{DateTime.Now.Day:D2} {DateTime.Now.Hour:D2}.{DateTime.Now.Minute:D2}.{DateTime.Now.Second:D2}.ass";

    public List<Regex> GetFilterSwitch(TextBox txtLog)
    {
        List<Regex> list = [];
        string[] patterns = FilterSwitch.Split('/');
        foreach (var pattern in patterns)
        {
            if (pattern.IsWhiteSpace())
            {
                continue;
            }
            try
            {
                list.Add(new(pattern));
            }
            catch (Exception)
            {
                txtLog.AppendText($"[!] Invalid Regex: {pattern}\r\n");
            }
        }
        return list;
    }

    public List<Regex> GetFilter(TextBox txtLog)
    {
        List<Regex> list = [];
        string[] patterns = Filter.Split('/');
        foreach (var pattern in patterns)
        {
            if (pattern.IsWhiteSpace())
            {
                continue;
            }
            try
            {
                list.Add(new(pattern));
            }
            catch (Exception)
            {
                txtLog.AppendText($"[!] Invalid Regex: {pattern}\r\n");
            }
        }
        return list;
    }

    public void WriteToFile()
    {
        using StreamWriter writer = new(CONFIG_FILENAME);
        writer.WriteLine($"[{nameof(InputRecorderOptions)}]");
        writer.WriteLine($"KeyTypedEnabled={KeyTypedEnabled}");
        writer.WriteLine($"FilterSwitch={FilterSwitch}");
        writer.WriteLine($"Filter={Filter}");
        writer.WriteLine($"OutputFile={OutputFile}");
    }
}
