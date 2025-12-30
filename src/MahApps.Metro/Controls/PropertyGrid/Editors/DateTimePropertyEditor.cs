using System;
using System.Windows;

namespace MahApps.Metro.Controls.Editors;

public class DateTimePropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem)
    {
        var datetime = new DateTimePicker
        {
            IsEnabled = !propertyItem.IsReadOnly, 
        };
        return datetime;
    }


    public override DependencyProperty GetDependencyProperty() => DateTimePicker.SelectedDateTimeProperty;
}
