using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls.Editors;

public class HorizontalAlignmentPropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ComboBox
    {
       // Style = ResourceHelper.GetResourceInternal<Style>("ComboBoxCapsule"),
        ItemsSource = Enum.GetValues(propertyItem.PropertyType),
     //   ItemTemplateSelector = ResourceHelper.GetResourceInternal<DataTemplateSelector>("HorizontalAlignmentPathTemplateSelector"),
        HorizontalAlignment = HorizontalAlignment.Left
    };

    public override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;
}

public class HorizontalAlignmentPathTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is HorizontalAlignment horizontalAlignment)
        {
            var dataTemplate = new DataTemplate
            {
                DataType = typeof(ComboBox)
            };

            var factory = new FrameworkElementFactory(typeof(Path));
            factory.SetValue(FrameworkElement.WidthProperty, 12.0);
            factory.SetValue(FrameworkElement.HeightProperty, UsedBoxes.Double10Box);
            factory.SetBinding(Shape.FillProperty, new Binding(Control.ForegroundProperty.Name)
            {
                RelativeSource = new RelativeSource
                {
                    AncestorType = typeof(ComboBoxItem)
                }
            });

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                 //   factory.SetValue(Path.DataProperty, ResourceHelper.GetResourceInternal<Geometry>("AlignLeftGeometry"));
                    break;
                case HorizontalAlignment.Center:
                 //   factory.SetValue(Path.DataProperty, ResourceHelper.GetResourceInternal<Geometry>("AlignHCenterGeometry"));
                    break;
                case HorizontalAlignment.Right:
                 //   factory.SetValue(Path.DataProperty, ResourceHelper.GetResourceInternal<Geometry>("AlignRightGeometry"));
                    break;
                case HorizontalAlignment.Stretch:
                 //   factory.SetValue(Path.DataProperty, ResourceHelper.GetResourceInternal<Geometry>("AlignHStretchGeometry"));
                    break;
            }

            dataTemplate.VisualTree = factory;
            return dataTemplate;
        }

        return null!;
    }
}
