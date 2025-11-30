using TesterApp.ViewModels;

namespace TesterApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
    
    public MainWindowViewModel ViewModel { get; }
}