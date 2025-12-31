using System;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Converters;
using MahApps.Metro.Utilities;
#nullable disable
namespace MahApps.Metro.Controls;

[TypeConverter(typeof(ZoomboxViewConverter))]
public class ZoomboxView
{
    private static readonly ZoomboxView _empty = new ZoomboxView(ZoomboxViewKind.Empty);

    private static readonly ZoomboxView _fill = new ZoomboxView(ZoomboxViewKind.Fill);

    private static readonly ZoomboxView _fit = new ZoomboxView(ZoomboxViewKind.Fit);

    private static readonly ZoomboxView _center = new ZoomboxView(ZoomboxViewKind.Center);

    private double _kindHeight = -1.0;

    private double _x = double.NaN;

    private double _y = double.NaN;

    private double _scaleWidth = double.NaN;

    public static ZoomboxView Empty => _empty;

    public static ZoomboxView Fill => _fill;

    public static ZoomboxView Fit => _fit;

    public static ZoomboxView Center => _center;

    public ZoomboxViewKind ViewKind
    {
        get
        {
            if (_kindHeight > 0.0)
            {
                return ZoomboxViewKind.Region;
            }
            return (ZoomboxViewKind)_kindHeight;
        }
    }

    public Point Position
    {
        get
        {
            if (ViewKind != ZoomboxViewKind.Absolute)
            {
                throw new InvalidOperationException("PositionOnlyAccessibleOnAbsolute");
            }
            return new Point(_x, _y);
        }
        set
        {
            if (ViewKind != ZoomboxViewKind.Absolute && ViewKind != ZoomboxViewKind.Empty)
            {
                throw new InvalidOperationException(string.Format("ZoomboxViewAlreadyInitialized", ViewKind.ToString()));
            }
            _x = value.X;
            _y = value.Y;
            _kindHeight = -5.0;
        }
    }

    public double Scale
    {
        get
        {
            if (ViewKind != ZoomboxViewKind.Absolute)
            {
                throw new InvalidOperationException("ScaleOnlyAccessibleOnAbsolute");
            }
            return _scaleWidth;
        }
        set
        {
            if (ViewKind != ZoomboxViewKind.Absolute && ViewKind != ZoomboxViewKind.Empty)
            {
                throw new InvalidOperationException(string.Format("ZoomboxViewAlreadyInitialized", ViewKind.ToString()));
            }
            _scaleWidth = value;
            _kindHeight = -5.0;
        }
    }

    public Rect Region
    {
        get
        {
            if (_kindHeight < 0.0)
            {
                throw new InvalidOperationException("RegionOnlyAccessibleOnRegionalView");
            }
            return new Rect(_x, _y, _scaleWidth, _kindHeight);
        }
        set
        {
            if (ViewKind != 0 && ViewKind != ZoomboxViewKind.Empty)
            {
                throw new InvalidOperationException(string.Format("ZoomboxViewAlreadyInitialized", ViewKind.ToString()));
            }
            if (!value.IsEmpty)
            {
                _x = value.X;
                _y = value.Y;
                _scaleWidth = value.Width;
                _kindHeight = value.Height;
            }
        }
    }

    public ZoomboxView()
    {
    }

    public ZoomboxView(double scale)
    {
        Scale = scale;
    }

    public ZoomboxView(Point position)
    {
        Position = position;
    }

    public ZoomboxView(double scale, Point position)
    {
        Position = position;
        Scale = scale;
    }

    public ZoomboxView(Rect region)
    {
        Region = region;
    }

    public ZoomboxView(double x, double y)
        : this(new Point(x, y))
    {
    }

    public ZoomboxView(double scale, double x, double y)
        : this(scale, new Point(x, y))
    {
    }

    public ZoomboxView(double x, double y, double width, double height)
        : this(new Rect(x, y, width, height))
    {
    }

    public override int GetHashCode()
    {
        return _x.GetHashCode() ^ _y.GetHashCode() ^ _scaleWidth.GetHashCode() ^ _kindHeight.GetHashCode();
    }

    public override bool Equals(object o)
    {
        bool result = false;
        if (o is ZoomboxView)
        {
            ZoomboxView zoomboxView = (ZoomboxView)o;
            if (ViewKind == zoomboxView.ViewKind)
            {
                result = ViewKind switch
                {
                    ZoomboxViewKind.Absolute => DoubleHelper.AreVirtuallyEqual(_scaleWidth, zoomboxView._scaleWidth) && DoubleHelper.AreVirtuallyEqual(Position, zoomboxView.Position),
                    ZoomboxViewKind.Region => DoubleHelper.AreVirtuallyEqual(Region, zoomboxView.Region),
                    _ => true,
                };
            }
        }
        return result;
    }

    public override string ToString()
    {
        return ViewKind switch
        {
            ZoomboxViewKind.Empty => "ZoomboxView: Empty",
            ZoomboxViewKind.Center => "ZoomboxView: Center",
            ZoomboxViewKind.Fill => "ZoomboxView: Fill",
            ZoomboxViewKind.Fit => "ZoomboxView: Fit",
            ZoomboxViewKind.Absolute => string.Format("ZoomboxView: Scale = {0}; Position = ({1}, {2})", _scaleWidth.ToString("f"), _x.ToString("f"), _y.ToString("f")),
            ZoomboxViewKind.Region => string.Format("ZoomboxView: Region = ({0}, {1}, {2}, {3})", _x.ToString("f"), _y.ToString("f"), _scaleWidth.ToString("f"), _kindHeight.ToString("f")),
            _ => base.ToString(),
        };
    }

    private ZoomboxView(ZoomboxViewKind viewType)
    {
        _kindHeight = (double)viewType;
    }

    public static bool operator ==(ZoomboxView v1, ZoomboxView v2)
    {
        if ((object)v1 == null)
        {
            return (object)v2 == null;
        }
        if ((object)v2 == null)
        {
            return (object)v1 == null;
        }
        return v1.Equals(v2);
    }

    public static bool operator !=(ZoomboxView v1, ZoomboxView v2)
    {
        return !(v1 == v2);
    }
}
