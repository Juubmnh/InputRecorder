using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
using SharpHook.Providers;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace InputRecorder;

internal partial class MainForm : Form
{
    private readonly InputRecorderOptions _options;
    private const string SAMPLE_FILENAME = "Sample.ass";

    private EventLoopGlobalHook? _hook;
    private readonly Dictionary<KeyCode, bool> KeyStates = [];
    private readonly List<string> _pressed = [];

    private readonly Filter _filterSwitch, _filter;
    private readonly Stopwatch _stopwatch = new();
    private bool _recording;
    private readonly List<(TimeSpan, string)> _record = [];

    private FileStream? _stream;
    private int _messageBoxShowedCount = 0;
    private int _indexWritten = 0;

    public MainForm(IOptions<InputRecorderOptions> options)
    {
        InitializeComponent();
        _filter = new(btnFilter, txtFilter, lstFilter);
        _filterSwitch = new(btnFilter, txtFilterSwitch, lstFilterSwitch);

        _options = options.Value;
        ckKeyTyped.Checked = _options.KeyTypedEnabled;
        ckKeyTyped_CheckedChanged(this, EventArgs.Empty);
        lstFilterSwitch.Items.AddRange([.. _options.GetFilterSwitch(txtLog)]);
        lstFilter.Items.AddRange([.. _options.GetFilter(txtLog)]);
        txtFile.Text = _options.OutputFile;
        CreateNewStream();
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

            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                _stopwatch.Reset();
                _recording = false;
                _record.Clear();
                _indexWritten = 0;
                txtLog.AppendText($"[-] Stopwatch Stopped\r\n");
            }
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
                if (_messageBoxShowedCount == 0)
                {
                    _messageBoxShowedCount++;
                    MessageBox.Show("Please select an output file");
                    _messageBoxShowedCount--;
                }
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
                    writer.WriteLine();
                    writer.WriteLine($"; {DateTime.Now}");

                    for (int i = _indexWritten; i < _record.Count - 1; i++)
                    {
                        (var time, var str) = _record[i + 1];
                        writer.WriteLine($"Dialogue: 0,{ToAssTime(_record[i].Item1)},{ToAssTime(time)},Default,,0,0,0,,[Subtitle-{i + 1} ({str})]");
                    }
                    _indexWritten = _record.Count - 1;

                    txtLog.AppendText($"[+] Writing Finished\r\n");

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

    private void CreateNewStream()
    {
        _stream?.Dispose();
        try
        {
            var dir = Path.GetDirectoryName(txtFile.Text);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _stream = new(txtFile.Text, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        catch (Exception)
        {
            _stream = null;
        }
    }

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
            CreateNewStream();
        }
    }

    private void txtFile_Leave(object sender, EventArgs e) => CreateNewStream();

    private async void MainForm_FormClosing(object sender, EventArgs e)
    {
        _options.KeyTypedEnabled = ckKeyTyped.Checked;
        _options.FilterSwitch = string.Join('/', lstFilterSwitch.Items.Cast<Regex>().Select(regex => regex.ToString()));
        _options.Filter = string.Join('/', lstFilter.Items.Cast<Regex>().Select(regex => regex.ToString()));
        _options.OutputFile = txtFile.Text;
        _options.WriteToFile();

        _hook?.Dispose();
        _stream?.Dispose();
    }
}
