using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Utilities;
#nullable disable
namespace MahApps.Metro.Converters;

public sealed class ZoomboxViewConverter : TypeConverter
{
	private static ZoomboxViewConverter _converter;

	internal static ZoomboxViewConverter Converter
	{
		get
		{
			if (_converter == null)
			{
				_converter = new ZoomboxViewConverter();
			}
			return _converter;
		}
	}

	public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type type)
	{
		if (!(type == typeof(string)) && !(type == typeof(double)) && !(type == typeof(Point)) && !(type == typeof(Rect)))
		{
			return base.CanConvertFrom(typeDescriptorContext, type);
		}
		return true;
	}

	public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type type)
	{
		if (!(type == typeof(string)))
		{
			return base.CanConvertTo(typeDescriptorContext, type);
		}
		return true;
	}

	public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
	{
		ZoomboxView zoomboxView = null;
		if (value is double)
		{
			zoomboxView = new ZoomboxView((double)value);
		}
		else if (value is Point)
		{
			zoomboxView = new ZoomboxView((Point)value);
		}
		else if (value is Rect)
		{
			zoomboxView = new ZoomboxView((Rect)value);
		}
		else if (value is string)
		{
			if (string.IsNullOrEmpty((value as string).Trim()))
			{
				zoomboxView = ZoomboxView.Empty;
			}
			else
			{
				switch ((value as string).Trim().ToLower())
				{
				case "center":
					zoomboxView = ZoomboxView.Center;
					break;
				case "empty":
					zoomboxView = ZoomboxView.Empty;
					break;
				case "fill":
					zoomboxView = ZoomboxView.Fill;
					break;
				case "fit":
					zoomboxView = ZoomboxView.Fit;
					break;
				default:
				{
					List<double> list = new List<double>();
					string[] array = (value as string).Split(new char[3] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						if (double.TryParse(array[i], out var result))
						{
							list.Add(result);
						}
						if (list.Count >= 4)
						{
							break;
						}
					}
					switch (list.Count)
					{
					case 1:
						zoomboxView = new ZoomboxView(list[0]);
						break;
					case 2:
						zoomboxView = new ZoomboxView(list[0], list[1]);
						break;
					case 3:
						zoomboxView = new ZoomboxView(list[0], list[1], list[2]);
						break;
					case 4:
						zoomboxView = new ZoomboxView(list[0], list[1], list[2], list[3]);
						break;
					}
					break;
				}
				}
			}
		}
		if (!(zoomboxView == null))
		{
			return zoomboxView;
		}
		return base.ConvertFrom(typeDescriptorContext, cultureInfo, value);
	}

	public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
	{
		object obj = null;
		ZoomboxView zoomboxView = value as ZoomboxView;
		if (zoomboxView != null && destinationType == typeof(string))
		{
			obj = "Empty";
			switch (zoomboxView.ViewKind)
			{
			case ZoomboxViewKind.Absolute:
				if (PointHelper.IsEmpty(zoomboxView.Position))
				{
					if (!DoubleHelper.IsNaN(zoomboxView.Scale))
					{
						obj = zoomboxView.Scale.ToString();
					}
					break;
				}
				obj = ((!DoubleHelper.IsNaN(zoomboxView.Scale)) ? (zoomboxView.Scale + "," + zoomboxView.Position.X + "," + zoomboxView.Position.Y) : (zoomboxView.Position.X + "," + zoomboxView.Position.Y));
				break;
			case ZoomboxViewKind.Center:
				obj = "Center";
				break;
			case ZoomboxViewKind.Fill:
				obj = "Fill";
				break;
			case ZoomboxViewKind.Fit:
				obj = "Fit";
				break;
			case ZoomboxViewKind.Region:
				obj = zoomboxView.Region.X + "," + zoomboxView.Region.Y + "," + zoomboxView.Region.Width + "," + zoomboxView.Region.Height;
				break;
			}
		}
		if (obj != null)
		{
			return obj;
		}
		return base.ConvertTo(typeDescriptorContext, cultureInfo, value, destinationType);
	}
}
