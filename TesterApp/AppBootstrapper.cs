using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cobalt.WpfUi.Core;
using Cobalt.WpfUi.Services;
using Cobalt.WpfUi;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using TesterApp.Pages.ViewModels;
using TesterApp.Pages;
using TesterApp.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui;

namespace TesterApp;

public class AppBootstrapper : WpfBootstrapper
{
    //TODO: Move to Bootstrapper ????
    private IHost AppHost => Host ?? throw new InvalidOperationException("Host is not initialized.");
    
    public AppBootstrapper()
    {
        UnhandledException += (_, exception) => Console.WriteLine(exception);
    }

    protected override bool ShowSplashScreen => true;
    protected override TimeSpan SplashScreenDuration => TimeSpan.FromSeconds(2);
    protected override Func<Window> MainWindowFactory => CreateMainWindow;
    protected override Func<Window> SplashScreenWindowFactory => CreateSplashScreen;
    protected override Func<IProgress, Task> SplashScreenAction => DoSomethingAsync;
    protected override Func<IProgress> SplashScreenViewModelFactory => CreateSplashScreenViewModel;

    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        base.ConfigureServices(context, services);

        services.AddWpfUiNavigation();
        services.AddSingleton<NavigationPageService>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomePage>();
        services.AddSingleton<HomePageViewModel>();
        services.AddSingleton<SettingsPage>();
        services.AddSingleton<SettingsPageViewModel>();
        
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<IOverlayService, OverlayService>();
        services.AddSingleton<IInfoBarService, InfoBarService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    protected override void SetWpfUiTheme()
    {
        // ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None);
        ApplicationThemeManager.ApplySystemTheme();
        SystemThemeWatcher.Watch(MainWindow, WindowBackdropType.None);
    }

    private Window CreateMainWindow()
    {
        var window = AppHost.Services.GetRequiredService<MainWindow>();
        AppHost.Services.GetRequiredService<INavigationService>().SetNavigationControl(window.RootNavigation);
        AppHost.Services.GetRequiredService<NavigationPageService>().RegisterNavigationEvents(window.RootNavigation);
        AppHost.Services.GetRequiredService<IContentDialogService>().SetDialogHost(window.RootContentDialog);
        AppHost.Services.GetRequiredService<IOverlayService>().SetOverlayHost(window.RootOverlay);
        AppHost.Services.GetRequiredService<IInfoBarService>().SetInfoBarHost(window.RootInfoBar);
        AppHost.Services.GetRequiredService<ISnackbarService>().SetSnackbarPresenter(window.RootSnackbar);
        window.Loaded += OnWindowLoaded;
        window.Closing += OnWindowClosing;
        window.Closed += OnWindowClosed;
        return window;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        AppHost.Services.GetRequiredService<INavigationService>().Navigate(typeof(HomePage));
    }

    private async void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        try
        {
            // Called a 2nd time when the application is shutting down
            if (Host is null)
                return;
            
            e.Cancel = true;
            await ShowCloseMessageAsync();
        }
        catch (Exception ex)
        {
            RaiseUnhandledException(ex);
        }
    }

    private async Task ShowCloseMessageAsync()
    {
        var host = AppHost.Services.GetRequiredService<IContentDialogService>().GetDialogHost();

        var dialog = new ContentDialog(host)
        {
            Title = "Close application?",
            Content = "Are you sure you want to close the application?",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await StopAsync();
            Application.Current.Shutdown();
        }
    }

    private async void OnWindowClosed(object? sender, EventArgs e)
    {
        try
        {
            await StopAsync();
            Application.Current.Shutdown();
        }
        catch
        {
            // ignored
        }
    }

    private Window CreateSplashScreen()
        => new SplashScreen();

    private IProgress CreateSplashScreenViewModel()
        => new SplashScreenViewModel();

    private async Task DoSomethingAsync(IProgress progress)
    {
        progress.UpdateProgress(
            isIndeterminate: true,
            message: "Doing something...");
        
        await Task.Delay(500);

        progress.UpdateProgress(
            isIndeterminate: false,
            message: "Doing something else...");
        const int total = 60;
        var current = 0;
        while (current < total)
        {
            progress.UpdateProgress(value: current / (double) total * 100);
            await Task.Delay(10);
            current++;
        }
    }
}