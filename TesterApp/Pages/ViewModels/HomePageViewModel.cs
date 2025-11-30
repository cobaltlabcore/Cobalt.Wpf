using CommunityToolkit.Mvvm.Input;
using Cobalt.WpfUi.Core;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Cobalt.WpfUi;
using TesterApp.Views;
using Wpf.Ui.Controls;
using Wpf.Ui;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace TesterApp.Pages.ViewModels;

public class HomePageViewModel : ObservableValidator, INavigationPageViewModel
{
    private readonly IContentDialogService _contentDialogService;
    private readonly IOverlayService _overlayService;
    private readonly IInfoBarService _infoBarService;
    private readonly ISnackbarService _snackbarService;

    public HomePageViewModel(
        IContentDialogService contentDialogService,
        IOverlayService overlayService,
        IInfoBarService infoBarService,
        ISnackbarService snackbarService)
    {
        _contentDialogService = contentDialogService;
        _overlayService = overlayService;
        _infoBarService = infoBarService;
        _snackbarService = snackbarService;
        ShowSimpleDialogCommand = new AsyncRelayCommand(ShowSimpleDialog);
        ShowOverlayCommand = new AsyncRelayCommand(ShowOverlay);
        ShowInfoBarCommand = new AsyncRelayCommand(ShowInfoBar);
        HideInfoBarCommand = new AsyncRelayCommand(HideInfoBar);
        ShowSnackbarCommand = new AsyncRelayCommand(ShowSnackbar);
    }

    [Required]
    public string? LastName
    {
        get => _lastName;
        set
        {
            SetProperty(ref _lastName, value);    
            ValidateProperty(LastName);
        }
    }
    private string? _lastName;
    
    [Required]
    [Range(20, 50)]
    public uint? Age
    {
        get => _age;
        set
        {
            SetProperty(ref _age, value);    
            ValidateProperty(Age);
        }
    }
    private uint? _age;

    public string? ImageFilePath
    {
        get => _imageFilePath;
        set => SetProperty(ref _imageFilePath, value);
    }
    private string? _imageFilePath;

    public string? FolderPath
    {
        get => _folderPath;
        set => SetProperty(ref _folderPath, value);
    }
    private string? _folderPath;

    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }
    private string? _address;

    public double Test
    {
        get => _test;
        set => SetProperty(ref _test, value);
    }
    private double _test;

    public byte[]? Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }
    private byte[]? _data = [0x01, 0x02, 0xFE, 0xFF];

    // [CustomValidation(typeof(HomePageViewModel), nameof(ValidatePassword))]
    // [Required]
    // [Range(0, 100, ErrorMessage = "The value must be between 0 and 100")]
    // public uint? TestValue
    // {
    //     get => _testValue;
    //     set
    //     {
    //         SetProperty(ref _testValue, value);
    //         ValidateProperty(TestValue);
    //     }
    // }
    // private uint? _testValue;
    
    // public static ValidationResult ValidatePassword(string? value, ValidationContext context)
    // {
    //     if (string.IsNullOrEmpty(value))
    //         return new("Password is empty");
    //     if (value.Length < 10)
    //         return new ValidationResult("Password length must be at least 10 chars");
    //     return ValidationResult.Success!;
    // }
    
    public AsyncRelayCommand ShowSimpleDialogCommand { get; }
    public AsyncRelayCommand ShowOverlayCommand { get; }
    public AsyncRelayCommand ShowInfoBarCommand { get; }
    public AsyncRelayCommand HideInfoBarCommand { get; }
    public AsyncRelayCommand ShowSnackbarCommand { get; }

    public void Validate()
    {
        ValidateAllProperties();
    }

    private async Task ShowSimpleDialog()
    {
        var host = _contentDialogService.GetDialogHost();
        var dialog = new ContentDialog(host)
        {
            Title = "This is the title",
            Content = "This is the content message",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        var test = new MessageBox
        {
            Title = "Dialog result",
            Content = $"The ContentDialog result is: {result}",
            Owner = App.Bootstrapper!.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        await test.ShowDialogAsync();
    }

    private async Task ShowOverlay()
    {
        var progressView = new OverlayProgressView { Width = 300 };
        var vm = new Progress();
        progressView.DataContext = vm;
        
        try
        {
            await _overlayService.ShowAsync(progressView);
            await DoSomethingWithProgress(vm);
        }
        finally
        {
            await _overlayService.HideAsync();
        }
    }

    private async Task DoSomethingWithProgress(IProgress progress)
    {
        progress.UpdateProgress(message: "Initializing", isIndeterminate: true);
        await Task.Delay(500);
        
        progress.UpdateProgress(isIndeterminate: false);
        var total = 400;
        var cpt = 0;

        while (cpt++ < total)
        {
            progress.UpdateProgress(
                value: cpt / (double)total * 100,
                message: Guid.NewGuid().ToString());
            await Task.Delay(10);
        }
    }

    private async Task ShowInfoBar()
    {
        await _infoBarService.ShowAsync(
            severity: InfoBarSeverity.Error,
            message: "This is an error message",
            isClosable: false);
    }
    
    private async Task HideInfoBar()
    {
        await _infoBarService.HideAsync();
    }

    private async Task ShowSnackbar()
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _snackbarService.Show(
                title: "Title",
                message: "This is a message",
                appearance: ControlAppearance.Danger,
                icon: new SymbolIcon(SymbolRegular.ErrorCircle24) { FontSize = 24 },
                timeout: TimeSpan.FromSeconds(5));
        });
    }
    
    public void OnAppeared()
    {
        
    }

    public void OnDisappearing(NavigatingCancelEventArgs args)
    {
        
    }
}