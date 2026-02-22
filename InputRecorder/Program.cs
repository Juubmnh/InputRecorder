using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InputRecorder;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        if (!File.Exists(InputRecorderOptions.CONFIG_FILENAME))
        {
            File.Create(InputRecorderOptions.CONFIG_FILENAME);
        }

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddIniFile(InputRecorderOptions.CONFIG_FILENAME);
        builder.Services.Configure<InputRecorderOptions>(builder.Configuration.GetSection(nameof(InputRecorderOptions)));
        builder.Services.AddSingleton<MainForm>();

        using IHost host = builder.Build();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}