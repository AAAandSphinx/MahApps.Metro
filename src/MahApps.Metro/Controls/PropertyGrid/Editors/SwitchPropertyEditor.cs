using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives; 
namespace MahApps.Metro.Controls.Editors;

public class SwitchPropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new CheckBox
    {
       // Style = ResourceHelper.GetResourceInternal<Style>("ToggleButtonSwitch"),
        HorizontalAlignment = HorizontalAlignment.Left,
        IsEnabled = !propertyItem.IsReadOnly
    };

    public override DependencyProperty GetDependencyProperty() => CheckBox.IsCheckedProperty;
}
