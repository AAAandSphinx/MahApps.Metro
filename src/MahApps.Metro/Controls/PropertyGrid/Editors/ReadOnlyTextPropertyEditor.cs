using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Controls.Editors;

public class ReadOnlyTextPropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
    {
        IsReadOnly = true
    };

    public override DependencyProperty GetDependencyProperty() => System.Windows.Controls.TextBox.TextProperty;

    public override BindingMode GetBindingMode(PropertyItem propertyItem) => BindingMode.OneWay;

    protected override IValueConverter GetConverter(PropertyItem propertyItem) => null!; //ResourceHelper.GetResourceInternal<IValueConverter>("Object2StringConverter");
}
