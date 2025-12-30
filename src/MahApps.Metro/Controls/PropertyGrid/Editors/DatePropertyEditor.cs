using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls.Editors;

public class DatePropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new DatePicker
    {
        IsEnabled = !propertyItem.IsReadOnly,  
    };

    public override DependencyProperty GetDependencyProperty() => DatePicker.SelectedDateProperty;
}
