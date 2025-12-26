using System.Windows;

namespace MahApps.Metro.Controls.Editors;

public class TimePropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new DateTimePicker
    {
        IsEnabled = !propertyItem.IsReadOnly
    };

    public override DependencyProperty GetDependencyProperty() => TimePicker.SelectedDateTimeProperty;
}
