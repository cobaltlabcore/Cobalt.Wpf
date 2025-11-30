using System.Windows;
using Cobalt.WpfUi;
using TesterApp.Pages.ViewModels;

namespace TesterApp.Pages;

public partial class HomePage : INavigationPage<HomePageViewModel>
{
    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Validate();
    }

    public HomePageViewModel ViewModel { get; }
    INavigationPageViewModel INavigationPage.ViewModel => ViewModel;
}