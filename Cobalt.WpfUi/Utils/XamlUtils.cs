using System.Windows.Markup;
using System.Windows;
using System;

namespace Cobalt.WpfUi.Utils;

/// <summary>
/// Utility class providing helper methods for working with XAML resources and templates.
/// </summary>
public static class XamlUtils
{
    /// <summary>
    /// Creates a DataTemplate that binds a ViewModel type to its corresponding View type.
    /// This enables automatic view resolution based on view model types in WPF applications.
    /// </summary>
    /// <param name="viewModelType">The type of the view model that will trigger this template</param>
    /// <param name="viewType">The type of the view that should be instantiated for the view model</param>
    /// <returns>A DataTemplate that can be added to application resources</returns>
    public static DataTemplate CreateDataTemplate(Type viewModelType, Type viewType)
    {
        // Define the XAML template string with placeholders for view model and view type names
        // The double braces {{ }} are escaped braces for string.Format, resulting in single braces in the final XAML
        const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
        var xaml = string.Format(xamlTemplate, viewModelType.Name, viewType.Name);

        // Create a parser context to handle namespace mappings when parsing the XAML
        var context = new ParserContext
        {
            XamlTypeMapper = new XamlTypeMapper([])
        };

        // Map the view model namespace to the "vm" prefix if the type has valid namespace and assembly info
        if (viewModelType is { Namespace: not null, Assembly.FullName: not null })
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);

        // Map the view namespace to the "v" prefix if the type has valid namespace and assembly info
        if (viewType is { Namespace: not null, Assembly.FullName: not null })
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

        // Add standard WPF XAML namespace declarations
        context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"); // Default WPF namespace
        context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml"); // XAML namespace for x:Type, etc.
        context.XmlnsDictionary.Add("vm", "vm"); // Custom namespace for view models
        context.XmlnsDictionary.Add("v", "v"); // Custom namespace for views

        // Parse the XAML string into a DataTemplate object using the configured context
        var template = (DataTemplate)XamlReader.Parse(xaml, context);
        return template;
    } 

    /// <summary>
    /// Retrieves a strongly-typed resource from the application's resource dictionary.
    /// </summary>
    /// <typeparam name="T">The expected type of the resource</typeparam>
    /// <param name="resourceName">The key name of the resource in the application resources</param>
    /// <returns>The resource cast to type T if found and compatible, otherwise the default value of T</returns>
    public static T? GetAppResource<T>(string resourceName)
    {
        // Retrieve the resource object from the application's resource dictionary
        var obj = Application.Current.Resources[resourceName];

        // Use pattern matching to safely cast to the desired type
        if (obj is T t)
            return t;

        // Return default value if resource not found or type doesn't match
        return default;
    }
}