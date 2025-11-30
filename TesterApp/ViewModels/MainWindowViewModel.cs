using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TesterApp.Pages;
using Wpf.Ui.Controls;

namespace TesterApp.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        NavigationItems.Add(new NavigationViewItem
        {
            Content = "Home",
            ToolTip = "Home",
            Icon = new SymbolIcon(SymbolRegular.Home24) { FontSize = 18 },
            TargetPageType = typeof(HomePage)
        });
        NavigationFooterItems.Add(new NavigationViewItem
        {
            Content = "Settings",
            ToolTip = "Settings",
            Icon = new SymbolIcon(SymbolRegular.Settings24) { FontSize = 18 },
            TargetPageType = typeof(SettingsPage)
        });
    }
    
    public ObservableCollection<NavigationViewItem> NavigationItems { get; } = [];
    public ObservableCollection<NavigationViewItem> NavigationFooterItems { get; } = [];
}