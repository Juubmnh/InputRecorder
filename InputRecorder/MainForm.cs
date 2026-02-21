using SharpHook;
using SharpHook.Data;
using SharpHook.Providers;
using System.Diagnostics;

namespace InputRecorder;

internal partial class MainForm : Form
{
    private const string SAMPLE_FILENAME = "sample.ass";

    private EventLoopGlobalHook? _hook;
    private readonly Dictionary<KeyCode, bool> KeyStates = [];
    private readonly List<string> _pressed = [];
    private readonly Filter _filterSwitch, _filter;
    private readonly Stopwatch _stopwatch = new();
    private bool _recording;
    private readonly List<(TimeSpan, string)> _record = [];
    private int _indexWritten = 0;
    private FileStream? _stream;

    public MainForm()
    {
        InitializeComponent();
        _filter = new Filter(btnFilter, txtFilter, lstFilter, "^Button1$");
        _filterSwitch = new Filter(btnFilter, txtFilterSwitch, lstFilterSwitch, "^VcF12$");
    }

    private async void btnHook_Click(object sender, EventArgs e)
    {
        _hook?.Dispose();
        if (btnHook.Text.Equals("Run Hook"))
        {
            btnHook.Text = "Stop Hook";

            _hook = new();
            _hook.MousePressed += Hook_MousePressed;
            _hook.MouseReleased += Hook_MouseReleased;
            if (ckKeyTyped.Checked)
            {
                _hook.KeyPressed += Hook_KeyPressed;
                _hook.KeyReleased += Hook_KeyReleased;
            }
            await _hook.RunAsync();
        }
        else
        {
            btnHook.Text = "Run Hook";
        }
    }

    private void ckKeyTyped_CheckedChanged(object sender, EventArgs e)
    {
        UioHookProvider.Instance.KeyTypedEnabled = ckKeyTyped.Checked;
    }

    private static string ToAssTime(TimeSpan time)
    {
        int centiseconds = (int)((time.Ticks / TimeSpan.TicksPerMillisecond) / 10 % 100);
        return $"{time.Hours}:{time.Minutes:D2}:{time.Seconds:D2}.{centiseconds:D2}";
    }

    private void Press(string item, string log)
    {
        _pressed.Add(item);
        txtLog.AppendText($"{log}({item})\r\n");

        if (_filterSwitch.IsUnfiltered(_pressed))
        {
            if (_stream is null)
            {
                MessageBox.Show("Please select an output file");
            }
            else
            {
                //if (_stopwatch.IsRunning)
                if (_recording)
                {
                    //_stopwatch.Stop();
                    //txtLog.AppendText($"[-] Stopwatch Stopped\r\n");

                    txtLog.AppendText($"[-] Recording Stopped\r\n");

                    using StreamWriter writer = new(_stream, leaveOpen: true);
                    using StreamReader reader = new(_stream, leaveOpen: true);

                    _stream.Seek(0, SeekOrigin.Begin);
                    if ((reader.ReadLine() ?? string.Empty).Equals("[Script Info]"))
                    {
                        _stream.Seek(0, SeekOrigin.End);
                        writer.WriteLine();
                        writer.WriteLine($"; {DateTime.Now}");
                    }
                    else
                    {
                        using StreamReader sampleReader = new(SAMPLE_FILENAME);
                        string? line;
                        while ((line = sampleReader.ReadLine()) is not null)
                        {
                            writer.WriteLine(line);
                        }
                    }

                    for (int i = _indexWritten; i < _record.Count - 1; i++)
                    {
                        (var time, var str) = _record[i + 1];
                        writer.WriteLine($"Dialogue: 0,{ToAssTime(_record[i].Item1)},{ToAssTime(time)},Default,,0,0,0,,[Subtitle-{i + 1} ({str})]");
                    }
                    _indexWritten = _record.Count - 1;

                    _recording = false;
                }
                else
                {
                    //_record.Clear();
                    //_record.Add(new());
                    //_stopwatch.Restart();
                    //txtLog.AppendText($"[+] Stopwatch Restarted\r\n");

                    if (_record.Count == 0)
                    {
                        _record.Add(new());
                        _stopwatch.Start();
                        txtLog.AppendText($"[+] Stopwatch Started\r\n");
                    }
                    else
                    {
                        txtLog.AppendText($"[+] Recording Resumed\r\n");
                    }

                    _recording = true;
                }
            }
        }
        //else if (_stopwatch.IsRunning && _filter.IsUnfiltered(_pressed))
        else if (_recording && _filter.IsUnfiltered(_pressed))
                {
            _record.Add((_stopwatch.Elapsed, item));
            txtLog.AppendText($"[+] {_stopwatch.Elapsed} ({item})\r\n");
        }
    }

    private void Release(string item, string log)
    {
        _pressed.Remove(item);
        txtLog.AppendText($"{log}({item})\r\n");
    }

    private async void Hook_MousePressed(object? sender, MouseHookEventArgs e) => await txtLog.InvokeAsync(() =>
    {
        Press(e.Data.Button.ToString(), "Mouse Pressed: ");
    });

    private async void Hook_MouseReleased(object? sender, MouseHookEventArgs e) => await txtLog.InvokeAsync(() =>
    {
        Release(e.Data.Button.ToString(), "Mouse Released: ");
    });

    private async void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e) => await txtLog.InvokeAsync(() =>
    {
        if (KeyStates.TryGetValue(e.Data.KeyCode, out bool state))
        {
            KeyStates[e.Data.KeyCode] = true;
        }
        else
        {
            KeyStates.Add(e.Data.KeyCode, true);
        }

        if (!state)
        {
            Press(e.Data.KeyCode.ToString(), "Key Pressed: ");
        }
    });

    private async void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e) => await txtLog.InvokeAsync(() =>
    {
        if (KeyStates.TryGetValue(e.Data.KeyCode, out bool state))
        {
            KeyStates[e.Data.KeyCode] = false;
        }
        else
        {
            KeyStates.Add(e.Data.KeyCode, false);
        }

        if (state)
        {
            Release(e.Data.KeyCode.ToString(), "Key Released: ");
        }
    });

    private void btnFile_Click(object sender, EventArgs e)
    {
        SaveFileDialog dialog = new()
        {
            AddExtension = true,
            Filter = "ASS|*.ass|Any|*.*",
            InitialDirectory = Application.StartupPath,
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtFile.Text = dialog.FileName;
            _stream = new(dialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }

    private async void MainForm_FormClosing(object sender, EventArgs e)
    {
        _hook?.Dispose();
        _stream?.Dispose();
    }
}
