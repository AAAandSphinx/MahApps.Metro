using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace MahApps.Metro.Controls.Editors;

public class SwitchPropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ToggleSwitch
    {
        // Style = ResourceHelper.GetResourceInternal<Style>("ToggleButtonSwitch"),
        OffContent = string.Empty,
        OnContent = string.Empty,
        HorizontalAlignment = HorizontalAlignment.Left,
        IsEnabled = !propertyItem.IsReadOnly
    };

    public override DependencyProperty GetDependencyProperty() => CheckBox.IsCheckedProperty;
}
