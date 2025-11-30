using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cobalt.WpfUi;
using System.Collections.ObjectModel;
using System.Linq;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TesterApp.Pages.ViewModels;

public class SettingsPageViewModel : ObservableObject, INavigationPageViewModel
{
    public SettingsPageViewModel()
    {
        SetThemeCommand = new RelayCommand(SetTheme);
        SelectedApplicationTheme = ApplicationThemes.First();
        SelectedWindowBackdropType = WindowBackdropTypes.First();
    }
    
    public ObservableCollection<ApplicationThemeItem> ApplicationThemes { get; } = 
    [
        new("Dark", ApplicationTheme.Dark),
        new("Light", ApplicationTheme.Light),
        new("High Contrast", ApplicationTheme.HighContrast)   
    ];
    
    public ApplicationThemeItem? SelectedApplicationTheme
    {
        get => _selectedApplicationTheme;
        set => SetProperty(ref _selectedApplicationTheme, value);
    }
    private ApplicationThemeItem? _selectedApplicationTheme;
    
    public ObservableCollection<WindowBackdropTypeItem> WindowBackdropTypes { get; } = 
    [
        new("None", WindowBackdropType.None),
        new("Auto", WindowBackdropType.Auto),
        new("Mica", WindowBackdropType.Mica),
        new("Acrylic", WindowBackdropType.Acrylic),
        new("Tabbed", WindowBackdropType.Tabbed)
    ];
    
    public WindowBackdropTypeItem? SelectedWindowBackdropType
    {
        get => _selectedWindowBackdropType;
        set => SetProperty(ref _selectedWindowBackdropType, value);
    }
    private WindowBackdropTypeItem? _selectedWindowBackdropType;
    
    public RelayCommand SetThemeCommand { get; }

    private void SetTheme()
    {
        if (SelectedApplicationTheme is not null && SelectedWindowBackdropType is not null)
            ApplicationThemeManager.Apply(SelectedApplicationTheme.Theme, SelectedWindowBackdropType.Type);
    }
    
    public void OnAppeared()
    {
        
    }

    public void OnDisappearing(NavigatingCancelEventArgs args)
    {
        
    }
}

public class ApplicationThemeItem
{
    public ApplicationThemeItem(string name, ApplicationTheme theme)
    {
        Name = name;
        Theme = theme;
    }
    
    public string Name { get; }
    public ApplicationTheme Theme { get; }
}

public class WindowBackdropTypeItem
{
    public WindowBackdropTypeItem(string name, WindowBackdropType type)
    {
        Name = name;
        Type = type;
    }
    
    public string Name { get; }
    public WindowBackdropType Type { get; }
}