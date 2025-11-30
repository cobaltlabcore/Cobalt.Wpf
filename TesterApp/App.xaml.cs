using System.Windows;
using System;

namespace TesterApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBootstrapper? Bootstrapper { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Bootstrapper = new AppBootstrapper();
            Bootstrapper.StartAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Bootstrapper?.StopAsync().GetAwaiter().GetResult();
            Current.Shutdown(1);
        }
    }
}