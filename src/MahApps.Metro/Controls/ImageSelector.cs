using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.ValueBoxes;
using Microsoft.Win32;
#nullable disable
namespace MahApps.Metro.Controls;

public class ImageSelector : Control
{
    public static readonly RoutedEvent ImageSelectedEvent =
        EventManager.RegisterRoutedEvent(nameof(ImageSelected), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ImageSelector));

    public event RoutedEventHandler ImageSelected
    {
        add => AddHandler(ImageSelectedEvent, value);
        remove => RemoveHandler(ImageSelectedEvent, value);
    }

    public static readonly RoutedEvent ImageUnselectedEvent =
        EventManager.RegisterRoutedEvent(nameof(ImageUnselected), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ImageSelector));

    public event RoutedEventHandler ImageUnselected
    {
        add => AddHandler(ImageUnselectedEvent, value);
        remove => RemoveHandler(ImageUnselectedEvent, value);
    }
    //新增Switch
    public static RoutedCommand Switch { get; } = new(nameof(Switch), typeof(ImageSelector));
    public ImageSelector() => CommandBindings.Add(new CommandBinding(ImageSelector.Switch, SwitchImage));

    private void SwitchImage(object sender, ExecutedRoutedEventArgs e)
    {
        if (!HasValue)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = Filter,
                DefaultExt = DefaultExt
            };

            if (dialog.ShowDialog() == true)
            {
                SetValue(UriPropertyKey, new Uri(dialog.FileName, UriKind.RelativeOrAbsolute));
                SetValue(PreviewBrushPropertyKey, new ImageBrush(BitmapFrame.Create(Uri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None))
                {
                    Stretch = Stretch
                });
                SetValue(HasValuePropertyKey, BooleanBoxes.TrueBox);
                SetCurrentValue(ToolTipProperty, dialog.FileName);
                RaiseEvent(new RoutedEventArgs(ImageSelectedEvent, this));
            }
        }
        else
        {
            SetValue(UriPropertyKey, default(Uri));
            SetValue(PreviewBrushPropertyKey, default(Brush));
            SetValue(HasValuePropertyKey, BooleanBoxes.FalseBox);
            SetCurrentValue(ToolTipProperty, default);
            RaiseEvent(new RoutedEventArgs(ImageUnselectedEvent, this));
        }
    }

    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        nameof(Stretch), typeof(Stretch), typeof(ImageSelector), new PropertyMetadata(default(Stretch)));

    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public static readonly DependencyPropertyKey UriPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Uri), typeof(Uri), typeof(ImageSelector), new PropertyMetadata(default(Uri)));

    public static readonly DependencyProperty UriProperty = UriPropertyKey.DependencyProperty;

    public Uri Uri
    {
        get => (Uri)GetValue(UriProperty);
        set => SetValue(UriPropertyKey, value);
    }

    public static readonly DependencyPropertyKey PreviewBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(PreviewBrush), typeof(Brush), typeof(ImageSelector), new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty PreviewBrushProperty = PreviewBrushPropertyKey.DependencyProperty;

    public Brush PreviewBrush
    {
        get => (Brush)GetValue(PreviewBrushProperty);
        set => SetValue(PreviewBrushPropertyKey, value);
    }

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
        nameof(StrokeThickness), typeof(double), typeof(ImageSelector), new FrameworkPropertyMetadata(UsedBoxes.Double1Box, FrameworkPropertyMetadataOptions.AffectsRender));

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(
        nameof(StrokeDashArray), typeof(DoubleCollection), typeof(ImageSelector), new FrameworkPropertyMetadata(default(DoubleCollection), FrameworkPropertyMetadataOptions.AffectsRender));

    public DoubleCollection StrokeDashArray
    {
        get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
        set => SetValue(StrokeDashArrayProperty, value);
    }

    public static readonly DependencyProperty DefaultExtProperty = DependencyProperty.Register(
        nameof(DefaultExt), typeof(string), typeof(ImageSelector), new PropertyMetadata(".jpg"));

    public string DefaultExt
    {
        get => (string)GetValue(DefaultExtProperty);
        set => SetValue(DefaultExtProperty, value);
    }

    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
        nameof(Filter), typeof(string), typeof(ImageSelector), new PropertyMetadata("(.jpg)|*.jpg|(.png)|*.png"));

    public string Filter
    {
        get => (string)GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly DependencyPropertyKey HasValuePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasValue), typeof(bool), typeof(ImageSelector), new PropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty HasValueProperty = HasValuePropertyKey.DependencyProperty;

    public bool HasValue
    {
        get => (bool)GetValue(HasValueProperty);
        set => SetValue(HasValuePropertyKey, BooleanBoxes.Box(value));
    }
}
