using CommunityToolkit.Mvvm.ComponentModel;
using Cobalt.WpfUi.Core;

namespace TesterApp.ViewModels;

public class SplashScreenViewModel : ObservableObject, IProgress
{
    public double Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
    private double _value;

    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
    private string? _message;

    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set => SetProperty(ref _isIndeterminate, value);
    }
    private bool _isIndeterminate;
    
    public void UpdateProgress(double? value = null, string? message = null, bool? isIndeterminate = null)
    {
        if (value is not null)
            Value = value.Value;
        if (message is not null)
            Message = message;
        if (isIndeterminate is not null)
            IsIndeterminate = isIndeterminate.Value;
    }
}