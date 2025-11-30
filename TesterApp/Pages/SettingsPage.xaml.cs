using Cobalt.WpfUi;
using TesterApp.Pages.ViewModels;

namespace TesterApp.Pages;

public partial class SettingsPage : INavigationPage<SettingsPageViewModel>
{
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    public SettingsPageViewModel ViewModel { get; }
    INavigationPageViewModel INavigationPage.ViewModel => ViewModel;
}