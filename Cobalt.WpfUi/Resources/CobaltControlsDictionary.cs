using System.Windows;
using System;

namespace Cobalt.WpfUi.Resources;

/// <summary>
/// A custom resource dictionary that provides access to Cobalt.WpfUi control styles and templates.
/// This class automatically loads the main resource dictionary containing all UI control definitions
/// from the Cobalt.WpfUi library.
/// </summary>
/// <remarks>
/// This dictionary is typically used in XAML applications to include Cobalt.WpfUi controls
/// by adding it to the application's merged dictionaries. It serves as a convenient wrapper
/// around the pack URI for the main resource dictionary.
/// </remarks>
public class CobaltControlsDictionary : ResourceDictionary
{
    private const string DictionaryUri = "pack://application:,,,/Cobalt.WpfUi;component/Resources/Cobalt.WpfUi.xaml";

    /// <summary>
    /// Initializes a new instance of the <see cref="CobaltControlsDictionary"/> class.
    /// Automatically sets the Source property to load the Cobalt.WpfUi resource dictionary.
    /// </summary>
    public CobaltControlsDictionary()
    {
        Source = new Uri(DictionaryUri, UriKind.Absolute);
    }
}