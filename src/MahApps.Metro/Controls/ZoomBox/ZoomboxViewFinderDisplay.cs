using System;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Utilities;

namespace MahApps.Metro.Controls;

 
public class ZoomboxViewFinderDisplay : FrameworkElement
{
	public static readonly DependencyProperty BackgroundProperty;

	private static readonly DependencyPropertyKey ContentBoundsPropertyKey;

	public static readonly DependencyProperty ContentBoundsProperty;

	public static readonly DependencyProperty ShadowBrushProperty;

	public static readonly DependencyProperty ViewportBrushProperty;

	public static readonly DependencyProperty ViewportPenProperty;

	public static readonly DependencyProperty ViewportRectProperty;

	private static readonly DependencyPropertyKey VisualBrushPropertyKey;

	public static readonly DependencyProperty VisualBrushProperty;

	private Size _availableSize = Size.Empty;

	private double _scale = 1.0;

	public Brush Background
	{
		get
		{
			return (Brush)GetValue(BackgroundProperty);
		}
		set
		{
			SetValue(BackgroundProperty, value);
		}
	}

	internal Rect ContentBounds
	{
		get
		{
			return (Rect)GetValue(ContentBoundsProperty);
		}
		set
		{
			SetValue(ContentBoundsPropertyKey, value);
		}
	}

	public Brush ShadowBrush
	{
		get
		{
			return (Brush)GetValue(ShadowBrushProperty);
		}
		set
		{
			SetValue(ShadowBrushProperty, value);
		}
	}

	public Brush ViewportBrush
	{
		get
		{
			return (Brush)GetValue(ViewportBrushProperty);
		}
		set
		{
			SetValue(ViewportBrushProperty, value);
		}
	}

	public Pen ViewportPen
	{
		get
		{
			return (Pen)GetValue(ViewportPenProperty);
		}
		set
		{
			SetValue(ViewportPenProperty, value);
		}
	}

	public Rect ViewportRect
	{
		get
		{
			return (Rect)GetValue(ViewportRectProperty);
		}
		set
		{
			SetValue(ViewportRectProperty, value);
		}
	}

	internal VisualBrush VisualBrush
	{
		get
		{
			return (VisualBrush)GetValue(VisualBrushProperty);
		}
		set
		{
			SetValue(VisualBrushPropertyKey, value);
		}
	}

	internal Size AvailableSize => _availableSize;

	internal double Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
		}
	}

	static ZoomboxViewFinderDisplay()
	{
		BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(192, byte.MaxValue, byte.MaxValue, byte.MaxValue)), FrameworkPropertyMetadataOptions.AffectsRender));
		ContentBoundsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ContentBounds), typeof(Rect), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		ContentBoundsProperty = ContentBoundsPropertyKey.DependencyProperty;
		ShadowBrushProperty = DependencyProperty.Register(nameof(ShadowBrush), typeof(Brush), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(128, byte.MaxValue, byte.MaxValue, byte.MaxValue)), FrameworkPropertyMetadataOptions.AffectsRender));
		ViewportBrushProperty = DependencyProperty.Register(nameof(ViewportBrush), typeof(Brush), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
		ViewportPenProperty = DependencyProperty.Register(nameof(ViewportPen), typeof(Pen), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)), 1.0), FrameworkPropertyMetadataOptions.AffectsRender));
		ViewportRectProperty = DependencyProperty.Register(nameof(ViewportRect), typeof(Rect), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
		VisualBrushPropertyKey = DependencyProperty.RegisterReadOnly(nameof(VisualBrush), typeof(VisualBrush), typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(null));
		VisualBrushProperty = VisualBrushPropertyKey.DependencyProperty;
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomboxViewFinderDisplay), new FrameworkPropertyMetadata(typeof(ZoomboxViewFinderDisplay)));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return base.DesiredSize;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		_availableSize = availableSize;
		double width = (DoubleHelper.IsNaN(ContentBounds.Width) ? 0.0 : Math.Max(0.0, ContentBounds.Width));
		double height = (DoubleHelper.IsNaN(ContentBounds.Height) ? 0.0 : Math.Max(0.0, ContentBounds.Height));
		Size result = new Size(width, height);
		if (result.Width > availableSize.Width || result.Height > availableSize.Height)
		{
			double num = availableSize.Width / result.Width;
			double num2 = availableSize.Height / result.Height;
			double num3 = ((num < num2) ? num : num2);
			result = new Size(result.Width * num3, result.Height * num3);
		}
		return result;
	}

	protected override void OnRender(DrawingContext dc)
	{
		base.OnRender(dc);
		dc.DrawRectangle(Background, null, ContentBounds);
		dc.DrawRectangle(VisualBrush, null, ContentBounds);
		if (ViewportRect.IntersectsWith(new Rect(base.RenderSize)))
		{
			Rect rectangle = new Rect(new Point(0.0, 0.0), new Size(base.RenderSize.Width, Math.Max(0.0, ViewportRect.Top)));
			Rect rectangle2 = new Rect(new Point(0.0, ViewportRect.Top), new Size(Math.Max(0.0, ViewportRect.Left), ViewportRect.Height));
			Rect rectangle3 = new Rect(new Point(ViewportRect.Right, ViewportRect.Top), new Size(Math.Max(0.0, base.RenderSize.Width - ViewportRect.Right), ViewportRect.Height));
			Rect rectangle4 = new Rect(new Point(0.0, ViewportRect.Bottom), new Size(base.RenderSize.Width, Math.Max(0.0, base.RenderSize.Height - ViewportRect.Bottom)));
			dc.DrawRectangle(ShadowBrush, null, rectangle);
			dc.DrawRectangle(ShadowBrush, null, rectangle2);
			dc.DrawRectangle(ShadowBrush, null, rectangle3);
			dc.DrawRectangle(ShadowBrush, null, rectangle4);
			dc.DrawRectangle(ViewportBrush, ViewportPen, ViewportRect);
		}
		else
		{
			dc.DrawRectangle(ShadowBrush, null, new Rect(base.RenderSize));
		}
	}
}
