using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public class PropertyGridHelper
    {


        public static double GetTitleWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(TitleWidthProperty);
        }

        public static void SetTitleWidth(DependencyObject obj, double value)
        {
            obj.SetValue(TitleWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleWidthProperty =
            DependencyProperty.RegisterAttached("TitleWidth", typeof(double), typeof(PropertyGridHelper), new PropertyMetadata(120d));





    }
}
