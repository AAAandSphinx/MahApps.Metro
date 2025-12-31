 //< !--***********************************************************************************

 //  Toolkit for WPF

 //  Copyright(C) 2007 - 2025 Xceed Software Inc.

 //  This program is provided to you under the terms of the XCEED SOFTWARE, INC.
 //  COMMUNITY LICENSE AGREEMENT(for non - commercial use) as published at
 //  https://github.com/xceedsoftware/wpftoolkit/blob/master/license.md 

 //  For more features, controls, and fast professional support,
 //  pick up the Plus Edition at https://xceed.com/xceed-toolkit-plus-for-wpf/

 //  Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

 // **********************************************************************************-->

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Converters;
using MahApps.Metro.Utilities;
#nullable disable 
namespace MahApps.Metro.Controls;

[TemplatePart(Name = "PART_VerticalScrollBar", Type = typeof(ScrollBar))]
[TemplatePart(Name = "PART_HorizontalScrollBar", Type = typeof(ScrollBar))]
public sealed class ZoomBox : ContentControl
{
    private const string PART_VerticalScrollBar = "PART_VerticalScrollBar";
    private const string PART_HorizontalScrollBar = "PART_HorizontalScrollBar";
    private bool _isUpdatingVisualTree = false;
    private bool _isUsingDefaultViewFinder = false;
    public static readonly DependencyProperty AnimationAccelerationRatioProperty = DependencyProperty.Register(nameof(AnimationAccelerationRatio), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0), new ValidateValueCallback(ZoomBox.ValidateAccelerationRatio));
    public static readonly DependencyProperty AnimationDecelerationRatioProperty = DependencyProperty.Register(nameof(AnimationDecelerationRatio), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0), new ValidateValueCallback(ZoomBox.ValidateDecelerationRatio));
    public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Duration(TimeSpan.FromMilliseconds(300.0))));
    private static readonly DependencyPropertyKey AreDragModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof(AreDragModifiersActive), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty AreDragModifiersActiveProperty = ZoomBox.AreDragModifiersActivePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey AreRelativeZoomModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof(AreRelativeZoomModifiersActive), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty AreRelativeZoomModifiersActiveProperty = ZoomBox.AreRelativeZoomModifiersActivePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey AreZoomModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof(AreZoomModifiersActive), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty AreZoomModifiersActiveProperty = ZoomBox.AreZoomModifiersActivePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey AreZoomToSelectionModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof(AreZoomToSelectionModifiersActive), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty AreZoomToSelectionModifiersActiveProperty = ZoomBox.AreZoomToSelectionModifiersActivePropertyKey.DependencyProperty;
    public static readonly DependencyProperty AutoWrapContentWithViewboxProperty = DependencyProperty.Register(nameof(AutoWrapContentWithViewbox), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, new PropertyChangedCallback(ZoomBox.OnAutoWrapContentWithViewboxChanged)));
    private UIElement _trueContent;
    private static readonly DependencyPropertyKey CurrentViewPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CurrentView), typeof(ZoomboxView), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomboxView.Empty, new PropertyChangedCallback(ZoomBox.OnCurrentViewChanged)));
    public static readonly DependencyProperty CurrentViewProperty = ZoomBox.CurrentViewPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey CurrentViewIndexPropertyKey = DependencyProperty.RegisterReadOnly(nameof(CurrentViewIndex), typeof(int), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)-1));
    public static readonly DependencyProperty CurrentViewIndexProperty = ZoomBox.CurrentViewIndexPropertyKey.DependencyProperty;
    public static readonly DependencyProperty DragModifiersProperty = DependencyProperty.Register(nameof(DragModifiers), typeof(KeyModifierCollection), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomBox.GetDefaultDragModifiers()));
    public static readonly DependencyProperty DragOnPreviewProperty = DependencyProperty.Register(nameof(DragOnPreview), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    private static readonly DependencyPropertyKey EffectiveViewStackModePropertyKey = DependencyProperty.RegisterReadOnly(nameof(EffectiveViewStackMode), typeof(ZoomboxViewStackMode), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomboxViewStackMode.Auto));
    public static readonly DependencyProperty EffectiveViewStackModeProperty = ZoomBox.EffectiveViewStackModePropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey HasBackStackPropertyKey = DependencyProperty.RegisterReadOnly(nameof(HasBackStack), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty HasBackStackProperty = ZoomBox.HasBackStackPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey HasForwardStackPropertyKey = DependencyProperty.RegisterReadOnly(nameof(HasForwardStack), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty HasForwardStackProperty = ZoomBox.HasForwardStackPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, (PropertyChangedCallback)null, new CoerceValueCallback(ZoomBox.CoerceIsAnimatedValue)));
    private static readonly DependencyPropertyKey IsDraggingContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDraggingContent), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty IsDraggingContentProperty = ZoomBox.IsDraggingContentPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IsSelectingRegionPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelectingRegion), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty IsSelectingRegionProperty = ZoomBox.IsSelectingRegionPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsUsingScrollBarsProperty = DependencyProperty.Register(nameof(IsUsingScrollBars), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (PropertyChangedCallback)null));
    public static readonly DependencyProperty MaxScaleProperty = DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)100.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(ZoomBox.OnMaxScaleChanged), new CoerceValueCallback(ZoomBox.CoerceMaxScaleValue)));
    public static readonly DependencyProperty MinScaleProperty = DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.01, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(ZoomBox.OnMinScaleChanged), new CoerceValueCallback(ZoomBox.CoerceMinScaleValue)));
    public static readonly DependencyProperty NavigateOnPreviewProperty = DependencyProperty.Register(nameof(NavigateOnPreview), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
    public static readonly DependencyProperty PanDistanceProperty = DependencyProperty.Register(nameof(PanDistance), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)5.0));
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Point), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)PointHelper.Empty, new PropertyChangedCallback(ZoomBox.OnPositionChanged)));
    public static readonly DependencyProperty RelativeZoomModifiersProperty = DependencyProperty.Register(nameof(RelativeZoomModifiers), typeof(KeyModifierCollection), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomBox.GetDefaultRelativeZoomModifiers()));
    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.NaN, new PropertyChangedCallback(ZoomBox.OnScaleChanged), new CoerceValueCallback(ZoomBox.CoerceScaleValue)));
    private static readonly DependencyPropertyKey ViewFinderPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewFinder), typeof(FrameworkElement), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(ZoomBox.OnViewFinderChanged)));
    public static readonly DependencyProperty ViewFinderProperty = ZoomBox.ViewFinderPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ViewFinderVisibilityProperty = DependencyProperty.RegisterAttached("ViewFinderVisibility", typeof(Visibility), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)Visibility.Visible));
    private static readonly DependencyPropertyKey ViewportPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Viewport), typeof(Rect), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)Rect.Empty, new PropertyChangedCallback(ZoomBox.OnViewportChanged)));
    public static readonly DependencyProperty ViewportProperty = ZoomBox.ViewportPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey ViewStackCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewStackCount), typeof(int), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)-1, new PropertyChangedCallback(ZoomBox.OnViewStackCountChanged)));
    public static readonly DependencyProperty ViewStackCountProperty = ZoomBox.ViewStackCountPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ViewStackIndexProperty = DependencyProperty.Register(nameof(ViewStackIndex), typeof(int), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)-1, new PropertyChangedCallback(ZoomBox.OnViewStackIndexChanged), new CoerceValueCallback(ZoomBox.CoerceViewStackIndexValue)));
    public static readonly DependencyProperty ViewStackModeProperty = DependencyProperty.Register(nameof(ViewStackMode), typeof(ZoomboxViewStackMode), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomboxViewStackMode.Default, new PropertyChangedCallback(ZoomBox.OnViewStackModeChanged), new CoerceValueCallback(ZoomBox.CoerceViewStackModeValue)));
    public static readonly DependencyProperty ViewStackSourceProperty = DependencyProperty.Register(nameof(ViewStackSource), typeof(IEnumerable), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(ZoomBox.OnViewStackSourceChanged)));
    public static readonly DependencyProperty ZoomModifiersProperty = DependencyProperty.Register(nameof(ZoomModifiers), typeof(KeyModifierCollection), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomBox.GetDefaultZoomModifiers()));
    public static readonly DependencyProperty ZoomOnPreviewProperty = DependencyProperty.Register(nameof(ZoomOnPreview), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)true));
    public static readonly DependencyProperty ZoomOriginProperty = DependencyProperty.Register(nameof(ZoomOrigin), typeof(Point), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Point(0.5, 0.5)));
    public static readonly DependencyProperty ZoomPercentageProperty = DependencyProperty.Register(nameof(ZoomPercentage), typeof(double), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)5.0));
    public static readonly DependencyProperty ZoomOnProperty = DependencyProperty.Register(nameof(ZoomOn), typeof(ZoomboxZoomOn), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomboxZoomOn.Content));
    public static readonly DependencyProperty ZoomToSelectionModifiersProperty = DependencyProperty.Register(nameof(ZoomToSelectionModifiers), typeof(KeyModifierCollection), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)ZoomBox.GetDefaultZoomToSelectionModifiers()));
    public static readonly DependencyProperty KeepContentInBoundsProperty = DependencyProperty.Register(nameof(KeepContentInBounds), typeof(bool), typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(ZoomBox.OnKeepContentInBoundsChanged)));
    public static readonly RoutedEvent AnimationBeginningEvent = EventManager.RegisterRoutedEvent(nameof(AnimationBeginning), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ZoomBox));
    public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent(nameof(AnimationCompleted), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ZoomBox));
    public static readonly RoutedEvent CurrentViewChangedEvent = EventManager.RegisterRoutedEvent(nameof(CurrentViewChanged), RoutingStrategy.Bubble, typeof(ZoomboxViewChangedEventHandler), typeof(ZoomBox));
    public static readonly RoutedEvent ViewStackIndexChangedEvent = EventManager.RegisterRoutedEvent(nameof(ViewStackIndexChanged), RoutingStrategy.Bubble, typeof(IndexChangedEventHandler), typeof(ZoomBox));
    public static RoutedUICommand Back = new RoutedUICommand("Go Back", nameof(Back), typeof(ZoomBox));
    public static RoutedUICommand Center = new RoutedUICommand("Center Content", nameof(Center), typeof(ZoomBox));
    public static RoutedUICommand Fill = new RoutedUICommand("Fill Bounds with Content", nameof(Fill), typeof(ZoomBox));
    public static RoutedUICommand Fit = new RoutedUICommand("Fit Content within Bounds", nameof(Fit), typeof(ZoomBox));
    public static RoutedUICommand Forward = new RoutedUICommand("Go Forward", nameof(Forward), typeof(ZoomBox));
    public static RoutedUICommand Home = new RoutedUICommand("Go Home", nameof(Home), typeof(ZoomBox));
    public static RoutedUICommand PanDown = new RoutedUICommand("Pan Down", nameof(PanDown), typeof(ZoomBox));
    public static RoutedUICommand PanLeft = new RoutedUICommand("Pan Left", nameof(PanLeft), typeof(ZoomBox));
    public static RoutedUICommand PanRight = new RoutedUICommand("Pan Right", nameof(PanRight), typeof(ZoomBox));
    public static RoutedUICommand PanUp = new RoutedUICommand("Pan Up", nameof(PanUp), typeof(ZoomBox));
    public static RoutedUICommand Refocus = new RoutedUICommand("Refocus View", nameof(Refocus), typeof(ZoomBox));
    public static RoutedUICommand ZoomIn = new RoutedUICommand("Zoom In", nameof(ZoomIn), typeof(ZoomBox));
    public static RoutedUICommand ZoomOut = new RoutedUICommand("Zoom Out", nameof(ZoomOut), typeof(ZoomBox));
    private static int MOUSE_WHEEL_DELTA = 28;
    private ContentPresenter _contentPresenter = (ContentPresenter)null;
    private ScrollBar _verticalScrollBar = (ScrollBar)null;
    private ScrollBar _horizontalScrollBar = (ScrollBar)null;
    private UIElement _content = (UIElement)null;
    private DragAdorner _dragAdorner = (DragAdorner)null;
    private ZoomboxViewStack _viewStack = (ZoomboxViewStack)null;
    private ZoomboxViewFinderDisplay _viewFinderDisplay = (ZoomboxViewFinderDisplay)null;
    private Rect _resizeViewportBounds = Rect.Empty;
    private Point _resizeAnchorPoint = new Point(0.0, 0.0);
    private Point _resizeDraggingPoint = new Point(0.0, 0.0);
    private Point _originPoint = new Point(0.0, 0.0);
    private double _viewboxFactor = 1.0;
    private double _relativeScale = 1.0;
    private Point _relativePosition = new Point();
    private Point _basePosition = new Point();
    private DateTime _lastStackAddition;
    private int _lastViewIndex = -1;
    private BitVector32 _cacheBits = new BitVector32(0);

    static ZoomBox()
    {
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ZoomBox)));
        UIElement.ClipToBoundsProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)true));
        UIElement.FocusableProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)true));
        Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)HorizontalAlignment.Center, new PropertyChangedCallback(ZoomBox.RefocusView)));
        Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)VerticalAlignment.Center, new PropertyChangedCallback(ZoomBox.RefocusView)));
        ContentControl.ContentProperty.OverrideMetadata(typeof(ZoomBox), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null, new CoerceValueCallback(ZoomBox.CoerceContentValue)));
    }

    public ZoomBox()
    {
        try
        {
            // new UIPermission(PermissionState.Unrestricted).Demand();
            this._cacheBits[512 /*0x0200*/] = true;
        }
        catch (SecurityException)
        {
        }
        this.InitCommands();
        this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
        this.AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnSizeChanged), true);
        this.CoerceValue(ZoomBox.ViewStackModeProperty);
        this.Loaded += new RoutedEventHandler(this.Zoombox_Loaded);
    }

    public double AnimationAccelerationRatio
    {
        get => (double)this.GetValue(ZoomBox.AnimationAccelerationRatioProperty);
        set => this.SetValue(ZoomBox.AnimationAccelerationRatioProperty, (object)value);
    }

    private static bool ValidateAccelerationRatio(object value)
    {
        double num = (double)value;
        if (num < 0.0 || num > 1.0 || DoubleHelper.IsNaN(num))
            throw new ArgumentException("AnimationAccelerationRatioOOR");
        return true;
    }

    public double AnimationDecelerationRatio
    {
        get => (double)this.GetValue(ZoomBox.AnimationDecelerationRatioProperty);
        set => this.SetValue(ZoomBox.AnimationDecelerationRatioProperty, (object)value);
    }

    private static bool ValidateDecelerationRatio(object value)
    {
        double num = (double)value;
        if (num < 0.0 || num > 1.0 || DoubleHelper.IsNaN(num))
            throw new ArgumentException("AnimationDecelerationRatioOOR");
        return true;
    }

    public Duration AnimationDuration
    {
        get => (Duration)this.GetValue(ZoomBox.AnimationDurationProperty);
        set => this.SetValue(ZoomBox.AnimationDurationProperty, (object)value);
    }

    public bool AreDragModifiersActive
    {
        get => (bool)this.GetValue(ZoomBox.AreDragModifiersActiveProperty);
    }

    private void SetAreDragModifiersActive(bool value)
    {
        this.SetValue(ZoomBox.AreDragModifiersActivePropertyKey, (object)value);
    }

    public bool AreRelativeZoomModifiersActive
    {
        get => (bool)this.GetValue(ZoomBox.AreRelativeZoomModifiersActiveProperty);
    }

    private void SetAreRelativeZoomModifiersActive(bool value)
    {
        this.SetValue(ZoomBox.AreRelativeZoomModifiersActivePropertyKey, (object)value);
    }

    public bool AreZoomModifiersActive
    {
        get => (bool)this.GetValue(ZoomBox.AreZoomModifiersActiveProperty);
    }

    private void SetAreZoomModifiersActive(bool value)
    {
        this.SetValue(ZoomBox.AreZoomModifiersActivePropertyKey, (object)value);
    }

    public bool AreZoomToSelectionModifiersActive
    {
        get => (bool)this.GetValue(ZoomBox.AreZoomToSelectionModifiersActiveProperty);
    }

    private void SetAreZoomToSelectionModifiersActive(bool value)
    {
        this.SetValue(ZoomBox.AreZoomToSelectionModifiersActivePropertyKey, (object)value);
    }

    public bool AutoWrapContentWithViewbox
    {
        get => (bool)this.GetValue(ZoomBox.AutoWrapContentWithViewboxProperty);
        set => this.SetValue(ZoomBox.AutoWrapContentWithViewboxProperty, (object)value);
    }

    private static void OnAutoWrapContentWithViewboxChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
        o.CoerceValue(ContentControl.ContentProperty);
    }

    private static object CoerceContentValue(DependencyObject d, object value)
    {
        return ((ZoomBox)d).CoerceContentValue(value);
    }

    private object CoerceContentValue(object value)
    {
        int num;
        switch (value)
        {
            case null:
            case UIElement _:
                num = 0;
                break;
            default:
                num = !(bool)this.GetValue(DesignerProperties.IsInDesignModeProperty) ? 1 : 0;
                break;
        }
        if (num != 0)
            throw new InvalidContentException("ZoomboxContentMustBeUIElement");
        object content1 = (object)this._content;
        if (value != this._trueContent || this.IsContentWrapped != this.AutoWrapContentWithViewbox)
        {
            if (this.IsContentWrapped && this._content is Viewbox && this._content != this._trueContent)
            {
                Viewbox content2 = (Viewbox)this._content;
                BindingOperations.ClearAllBindings((DependencyObject)content2);
                if (content2.Child is FrameworkElement)
                    (content2.Child as FrameworkElement).RemoveHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged));
                content2.Child = (UIElement)null;
                this.RemoveLogicalChild((object)content2);
            }
            if (this._viewFinderDisplay != null && this._viewFinderDisplay.VisualBrush != null)
            {
                this._viewFinderDisplay.VisualBrush.Visual = (Visual)null;
                this._viewFinderDisplay.VisualBrush = (VisualBrush)null;
            }
            this._content = value as UIElement;
            this._trueContent = value as UIElement;
            if (this._contentPresenter != null && this._contentPresenter.Content != null)
                this._contentPresenter.Content = (object)null;
            this.IsContentWrapped = false;
            if (this.AutoWrapContentWithViewbox)
            {
                Viewbox child = new Viewbox();
                this.AddLogicalChild((object)child);
                child.Child = value as UIElement;
                this._content = (UIElement)child;
                child.HorizontalAlignment = HorizontalAlignment.Left;
                child.VerticalAlignment = VerticalAlignment.Top;
                this.IsContentWrapped = true;
            }
            if (this._content is Viewbox && this.IsContentWrapped && this._trueContent is FrameworkElement)
                (this._trueContent as FrameworkElement).AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged), true);
            if (this._contentPresenter != null)
                this._contentPresenter.Content = (object)this._content;
            if (this._viewFinderDisplay != null)
                this.CreateVisualBrushForViewFinder((Visual)this._content);
            this.UpdateViewFinderDisplayContentBounds();
        }
        if (content1 != this._content && this.HasArrangedContentPresenter && this.HasRenderedFirstView)
        {
            this.HasArrangedContentPresenter = false;
            this.HasRenderedFirstView = false;
            this.RefocusViewOnFirstRender = true;
            this._contentPresenter.LayoutUpdated += new EventHandler(this.ContentPresenterFirstArranged);
        }
        return (object)this._content;
    }

    public ZoomboxView CurrentView => (ZoomboxView)this.GetValue(ZoomBox.CurrentViewProperty);

    private void SetCurrentView(ZoomboxView value)
    {
        this.SetValue(ZoomBox.CurrentViewPropertyKey, (object)value);
    }

    private static void OnCurrentViewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)o;
        if (!zoombox.IsUpdatingView)
            zoombox.ZoomTo(zoombox.CurrentView);
        zoombox.RaiseEvent((RoutedEventArgs)new ZoomboxViewChangedEventArgs(e.OldValue as ZoomboxView, e.NewValue as ZoomboxView, zoombox._lastViewIndex, zoombox.CurrentViewIndex));
    }

    public int CurrentViewIndex => (int)this.GetValue(ZoomBox.CurrentViewIndexProperty);

    internal void SetCurrentViewIndex(int value)
    {
        this.SetValue(ZoomBox.CurrentViewIndexPropertyKey, (object)value);
    }

    [TypeConverter(typeof(KeyModifierCollectionConverter))]
    public KeyModifierCollection DragModifiers
    {
        get => (KeyModifierCollection)this.GetValue(ZoomBox.DragModifiersProperty);
        set => this.SetValue(ZoomBox.DragModifiersProperty, (object)value);
    }

    private static KeyModifierCollection GetDefaultDragModifiers()
    {
        KeyModifierCollection defaultDragModifiers = new KeyModifierCollection();
        defaultDragModifiers.Add(KeyModifier.Ctrl);
        defaultDragModifiers.Add(KeyModifier.Exact);
        return defaultDragModifiers;
    }

    public bool DragOnPreview
    {
        get => (bool)this.GetValue(ZoomBox.DragOnPreviewProperty);
        set => this.SetValue(ZoomBox.DragOnPreviewProperty, (object)value);
    }

    public ZoomboxViewStackMode EffectiveViewStackMode
    {
        get => (ZoomboxViewStackMode)this.GetValue(ZoomBox.EffectiveViewStackModeProperty);
    }

    private void SetEffectiveViewStackMode(ZoomboxViewStackMode value)
    {
        this.SetValue(ZoomBox.EffectiveViewStackModePropertyKey, (object)value);
    }

    public bool HasBackStack => (bool)this.GetValue(ZoomBox.HasBackStackProperty);

    public bool HasForwardStack => (bool)this.GetValue(ZoomBox.HasForwardStackProperty);

    public bool IsAnimated
    {
        get => (bool)this.GetValue(ZoomBox.IsAnimatedProperty);
        set => this.SetValue(ZoomBox.IsAnimatedProperty, (object)value);
    }

    private static object CoerceIsAnimatedValue(DependencyObject d, object value)
    {
        ZoomBox zoombox = (ZoomBox)d;
        bool flag = (bool)value;
        if (!zoombox.IsInitialized)
            flag = false;
        return (object)flag;
    }

    public bool IsDraggingContent => (bool)this.GetValue(ZoomBox.IsDraggingContentProperty);

    private void SetIsDraggingContent(bool value)
    {
        this.SetValue(ZoomBox.IsDraggingContentPropertyKey, (object)value);
    }

    public bool IsSelectingRegion => (bool)this.GetValue(ZoomBox.IsSelectingRegionProperty);

    private void SetIsSelectingRegion(bool value)
    {
        this.SetValue(ZoomBox.IsSelectingRegionPropertyKey, (object)value);
    }

    public bool IsUsingScrollBars
    {
        get => (bool)this.GetValue(ZoomBox.IsUsingScrollBarsProperty);
        set => this.SetValue(ZoomBox.IsUsingScrollBarsProperty, (object)value);
    }

    public double MaxScale
    {
        get => (double)this.GetValue(ZoomBox.MaxScaleProperty);
        set => this.SetValue(ZoomBox.MaxScaleProperty, (object)value);
    }

    private static void OnMaxScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)o;
        zoombox.CoerceValue(ZoomBox.MinScaleProperty);
        zoombox.CoerceValue(ZoomBox.ScaleProperty);
    }

    private static object CoerceMaxScaleValue(DependencyObject d, object value)
    {
        ZoomBox zoombox = (ZoomBox)d;
        double num = (double)value;
        if (num < zoombox.MinScale)
            num = zoombox.MinScale;
        return (object)num;
    }

    public double MinScale
    {
        get => (double)this.GetValue(ZoomBox.MinScaleProperty);
        set => this.SetValue(ZoomBox.MinScaleProperty, (object)value);
    }

    private static void OnMinScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)o;
        zoombox.CoerceValue(ZoomBox.MinScaleProperty);
        zoombox.CoerceValue(ZoomBox.ScaleProperty);
    }

    private static object CoerceMinScaleValue(DependencyObject d, object value)
    {
        ZoomBox zoombox = (ZoomBox)d;
        double num = (double)value;
        if (num > zoombox.MaxScale)
            num = zoombox.MaxScale;
        return (object)num;
    }

    public bool NavigateOnPreview
    {
        get => (bool)this.GetValue(ZoomBox.NavigateOnPreviewProperty);
        set => this.SetValue(ZoomBox.NavigateOnPreviewProperty, (object)value);
    }

    public double PanDistance
    {
        get => (double)this.GetValue(ZoomBox.PanDistanceProperty);
        set => this.SetValue(ZoomBox.PanDistanceProperty, (object)value);
    }

    public Point Position
    {
        get => (Point)this.GetValue(ZoomBox.PositionProperty);
        set => this.SetValue(ZoomBox.PositionProperty, (object)value);
    }

    private static void OnPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)o;
        if (zoombox.IsUpdatingViewport)
            return;
        Point newValue = (Point)e.NewValue;
        if (zoombox.Scale > 0.0)
            zoombox.ZoomTo(new Point(-newValue.X, -newValue.Y));
    }

    [TypeConverter(typeof(KeyModifierCollectionConverter))]
    public KeyModifierCollection RelativeZoomModifiers
    {
        get => (KeyModifierCollection)this.GetValue(ZoomBox.RelativeZoomModifiersProperty);
        set => this.SetValue(ZoomBox.RelativeZoomModifiersProperty, (object)value);
    }

    private static KeyModifierCollection GetDefaultRelativeZoomModifiers()
    {
        KeyModifierCollection relativeZoomModifiers = new KeyModifierCollection();
        relativeZoomModifiers.Add(KeyModifier.Ctrl);
        relativeZoomModifiers.Add(KeyModifier.Alt);
        relativeZoomModifiers.Add(KeyModifier.Exact);
        return relativeZoomModifiers;
    }

    public double Scale
    {
        get => (double)this.GetValue(ZoomBox.ScaleProperty);
        set => this.SetValue(ZoomBox.ScaleProperty, (object)value);
    }

    private static void OnScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)o;
        if (zoombox.IsUpdatingView)
            return;
        double newValue = (double)e.NewValue;
        zoombox.ZoomTo(newValue);
    }

    private static object CoerceScaleValue(DependencyObject d, object value)
    {
        ZoomBox zoombox = (ZoomBox)d;
        double num = (double)value;
        if (num < zoombox.MinScale)
            num = zoombox.MinScale;
        if (num > zoombox.MaxScale)
            num = zoombox.MaxScale;
        return (object)num;
    }

    public FrameworkElement ViewFinder
    {
        get => (FrameworkElement)this.GetValue(ZoomBox.ViewFinderProperty);
        set => this.SetValue(ZoomBox.ViewFinderPropertyKey, (object)value);
    }

    private static void OnViewFinderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ZoomBox)d).OnViewFinderChanged(e);
    }

    private void OnViewFinderChanged(DependencyPropertyChangedEventArgs e)
    {
        this.AttachToVisualTree();
        this._isUsingDefaultViewFinder = false;
    }

    public static Visibility GetViewFinderVisibility(DependencyObject d)
    {
        return (Visibility)d.GetValue(ZoomBox.ViewFinderVisibilityProperty);
    }

    public static void SetViewFinderVisibility(DependencyObject d, Visibility value)
    {
        d.SetValue(ZoomBox.ViewFinderVisibilityProperty, (object)value);
    }

    public Rect Viewport => (Rect)this.GetValue(ZoomBox.ViewportProperty);

    private static void OnViewportChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox1 = (ZoomBox)o;
        ZoomBox zoombox2 = zoombox1;
        Rect viewport = zoombox1.Viewport;
        double x = -viewport.Left * zoombox1.Scale / zoombox1._viewboxFactor;
        viewport = zoombox1.Viewport;
        double y = -viewport.Top * zoombox1.Scale / zoombox1._viewboxFactor;
        Point point = new Point(x, y);
        zoombox2.Position = point;
    }

    public int ViewStackCount => (int)this.GetValue(ZoomBox.ViewStackCountProperty);

    internal void SetViewStackCount(int value)
    {
        this.SetValue(ZoomBox.ViewStackCountPropertyKey, (object)value);
    }

    private static void OnViewStackCountChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
        ((ZoomBox)d).OnViewStackCountChanged(e);
    }

    private void OnViewStackCountChanged(DependencyPropertyChangedEventArgs e)
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
            return;
        this.UpdateStackProperties();
    }

    public int ViewStackIndex
    {
        get => (int)this.GetValue(ZoomBox.ViewStackIndexProperty);
        set => this.SetValue(ZoomBox.ViewStackIndexProperty, (object)value);
    }

    private static void OnViewStackIndexChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
        ((ZoomBox)d).OnViewStackIndexChanged(e);
    }

    private void OnViewStackIndexChanged(DependencyPropertyChangedEventArgs e)
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
            return;
        if (!this.IsUpdatingView)
        {
            int viewStackIndex = this.ViewStackIndex;
            if (viewStackIndex >= 0 && viewStackIndex < this.ViewStack.Count)
                this.UpdateView(this.ViewStack[viewStackIndex], true, false, viewStackIndex);
        }
        this.UpdateStackProperties();
        this.RaiseEvent((RoutedEventArgs)new IndexChangedEventArgs(ZoomBox.ViewStackIndexChangedEvent, (int)e.OldValue, (int)e.NewValue));
    }

    private static object CoerceViewStackIndexValue(DependencyObject d, object value)
    {
        return (((((d as ZoomBox))))).EffectiveViewStackMode == ZoomboxViewStackMode.Disabled ? (object)-1 : value;
    }

    public ZoomboxViewStackMode ViewStackMode
    {
        get => (ZoomboxViewStackMode)this.GetValue(ZoomBox.ViewStackModeProperty);
        set => this.SetValue(ZoomBox.ViewStackModeProperty, (object)value);
    }

    private static void OnViewStackModeChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
        ((ZoomBox)d).OnViewStackModeChanged(e);
    }

    private void OnViewStackModeChanged(DependencyPropertyChangedEventArgs e)
    {
        if ((ZoomboxViewStackMode)e.NewValue != ZoomboxViewStackMode.Disabled || this._viewStack == null)
            return;
        this._viewStack.ClearViewStackSource();
        this._viewStack = (ZoomboxViewStack)null;
    }

    private static object CoerceViewStackModeValue(DependencyObject d, object value)
    {
        ZoomBox zoombox = ((((d as ZoomBox))));
        ZoomboxViewStackMode zoomboxViewStackMode = (ZoomboxViewStackMode)value;
        if (zoombox.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
            zoombox.SetEffectiveViewStackMode(zoomboxViewStackMode);
        switch (zoomboxViewStackMode)
        {
            case ZoomboxViewStackMode.Default:
                zoomboxViewStackMode = zoombox.ViewStack.AreViewsFromSource ? ZoomboxViewStackMode.Manual : ZoomboxViewStackMode.Auto;
                goto default;
            case ZoomboxViewStackMode.Disabled:
                zoombox.SetEffectiveViewStackMode(zoomboxViewStackMode);
                return value;
            default:
                if (zoombox.ViewStack.AreViewsFromSource && zoomboxViewStackMode != ZoomboxViewStackMode.Manual)
                    throw new InvalidOperationException("ViewModeInvalidForSource");
                goto case ZoomboxViewStackMode.Disabled;
        }
    }

    [Bindable(true)]
    public IEnumerable ViewStackSource
    {
        get => this._viewStack == null ? (IEnumerable)null : this.ViewStack.Source;
        set
        {
            if (value == null)
                this.ClearValue(ZoomBox.ViewStackSourceProperty);
            else
                this.SetValue(ZoomBox.ViewStackSourceProperty, (object)value);
        }
    }

    private static void OnViewStackSourceChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = (ZoomBox)d;
        IEnumerable oldValue = (IEnumerable)e.OldValue;
        IEnumerable newValue = (IEnumerable)e.NewValue;
        if (e.NewValue == null && !BindingOperations.IsDataBound(d, ZoomBox.ViewStackSourceProperty))
        {
            if (zoombox.ViewStack != null)
                zoombox.ViewStack.ClearViewStackSource();
        }
        else
            zoombox.ViewStack.SetViewStackSource(newValue);
        zoombox.CoerceValue(ZoomBox.ViewStackModeProperty);
    }

    [TypeConverter(typeof(KeyModifierCollectionConverter))]
    public KeyModifierCollection ZoomModifiers
    {
        get => (KeyModifierCollection)this.GetValue(ZoomBox.ZoomModifiersProperty);
        set => this.SetValue(ZoomBox.ZoomModifiersProperty, (object)value);
    }

    private static KeyModifierCollection GetDefaultZoomModifiers()
    {
        KeyModifierCollection defaultZoomModifiers = new KeyModifierCollection();
        defaultZoomModifiers.Add(KeyModifier.Shift);
        defaultZoomModifiers.Add(KeyModifier.Exact);
        return defaultZoomModifiers;
    }

    public bool ZoomOnPreview
    {
        get => (bool)this.GetValue(ZoomBox.ZoomOnPreviewProperty);
        set => this.SetValue(ZoomBox.ZoomOnPreviewProperty, (object)value);
    }

    public Point ZoomOrigin
    {
        get => (Point)this.GetValue(ZoomBox.ZoomOriginProperty);
        set => this.SetValue(ZoomBox.ZoomOriginProperty, (object)value);
    }

    public double ZoomPercentage
    {
        get => (double)this.GetValue(ZoomBox.ZoomPercentageProperty);
        set => this.SetValue(ZoomBox.ZoomPercentageProperty, (object)value);
    }

    public ZoomboxZoomOn ZoomOn
    {
        get => (ZoomboxZoomOn)this.GetValue(ZoomBox.ZoomOnProperty);
        set => this.SetValue(ZoomBox.ZoomOnProperty, (object)value);
    }

    [TypeConverter(typeof(KeyModifierCollectionConverter))]
    public KeyModifierCollection ZoomToSelectionModifiers
    {
        get => (KeyModifierCollection)this.GetValue(ZoomBox.ZoomToSelectionModifiersProperty);
        set => this.SetValue(ZoomBox.ZoomToSelectionModifiersProperty, (object)value);
    }

    private static KeyModifierCollection GetDefaultZoomToSelectionModifiers()
    {
        KeyModifierCollection selectionModifiers = new KeyModifierCollection();
        selectionModifiers.Add(KeyModifier.Alt);
        selectionModifiers.Add(KeyModifier.Exact);
        return selectionModifiers;
    }

    public bool KeepContentInBounds
    {
        get => (bool)this.GetValue(ZoomBox.KeepContentInBoundsProperty);
        set => this.SetValue(ZoomBox.KeepContentInBoundsProperty, (object)value);
    }

    private static void OnKeepContentInBoundsChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
        ((ZoomBox)d).OnKeepContentInBoundsChanged(e);
    }

    private void OnKeepContentInBoundsChanged(DependencyPropertyChangedEventArgs e)
    {
        bool isAnimated = this.IsAnimated;
        this.IsAnimated = false;
        try
        {
            this.UpdateView(this.CurrentView, false, false, this.ViewStackIndex);
        }
        finally
        {
            this.IsAnimated = isAnimated;
        }
    }

    public ZoomboxViewStack ViewStack
    {
        get
        {
            if (this._viewStack == null && this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled)
                this._viewStack = new ZoomboxViewStack(this);
            return this._viewStack;
        }
    }

    internal bool HasArrangedContentPresenter
    {
        get => this._cacheBits[64 /*0x40*/];
        set => this._cacheBits[64 /*0x40*/] = value;
    }

    internal bool IsUpdatingView
    {
        get => this._cacheBits[1];
        set => this._cacheBits[1] = value;
    }

    private Vector ContentOffset
    {
        get
        {
            if (this.IsContentWrapped || this._content == null || !(this._content is FrameworkElement))
                return new Vector(0.0, 0.0);
            double x = 0.0;
            double y = 0.0;
            Size size = this.ContentRect.Size;
            Size renderSize;
            switch ((this._content as FrameworkElement).HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    renderSize = this.RenderSize;
                    x = (renderSize.Width - size.Width) / 2.0;
                    break;
                case HorizontalAlignment.Right:
                    renderSize = this.RenderSize;
                    x = renderSize.Width - size.Width;
                    break;
            }
            switch ((this._content as FrameworkElement).VerticalAlignment)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    renderSize = this.RenderSize;
                    y = (renderSize.Height - size.Height) / 2.0;
                    break;
                case VerticalAlignment.Bottom:
                    renderSize = this.RenderSize;
                    y = renderSize.Height - size.Height;
                    break;
            }
            return new Vector(x, y);
        }
    }

    private Rect ContentRect
    {
        get
        {
            return this._content == null ? Rect.Empty : new Rect(new Size(this._content.RenderSize.Width / this._viewboxFactor, this._content.RenderSize.Height / this._viewboxFactor));
        }
    }

    private bool HasRenderedFirstView
    {
        get => this._cacheBits[128 /*0x80*/];
        set => this._cacheBits[128 /*0x80*/] = value;
    }

    private bool HasUIPermission => this._cacheBits[512 /*0x0200*/];

    private bool IsContentWrapped
    {
        get => this._cacheBits[32 /*0x20*/];
        set => this._cacheBits[32 /*0x20*/] = value;
    }

    private bool IsDraggingViewport
    {
        get => this._cacheBits[4];
        set => this._cacheBits[4] = value;
    }

    private bool IsMonitoringInput
    {
        get => this._cacheBits[16 /*0x10*/];
        set => this._cacheBits[16 /*0x10*/] = value;
    }

    private bool IsResizingViewport
    {
        get => this._cacheBits[8];
        set => this._cacheBits[8] = value;
    }

    private bool IsUpdatingViewport
    {
        get => this._cacheBits[2];
        set => this._cacheBits[2] = value;
    }

    private bool RefocusViewOnFirstRender
    {
        get => this._cacheBits[256 /*0x0100*/];
        set => this._cacheBits[256 /*0x0100*/] = value;
    }

    private Rect ViewFinderDisplayRect
    {
        get
        {
            return this._viewFinderDisplay == null ? Rect.Empty : new Rect(new Point(0.0, 0.0), new Point(this._viewFinderDisplay.RenderSize.Width, this._viewFinderDisplay.RenderSize.Height));
        }
    }

    public event RoutedEventHandler AnimationBeginning
    {
        add => this.AddHandler(ZoomBox.AnimationBeginningEvent, value);
        remove => this.RemoveHandler(ZoomBox.AnimationBeginningEvent, value);
    }

    public event RoutedEventHandler AnimationCompleted
    {
        add => this.AddHandler(ZoomBox.AnimationCompletedEvent, value);
        remove => this.RemoveHandler(ZoomBox.AnimationCompletedEvent, value);
    }

    public event ZoomboxViewChangedEventHandler CurrentViewChanged
    {
        add => this.AddHandler(ZoomBox.CurrentViewChangedEvent, value);
        remove => this.RemoveHandler(ZoomBox.CurrentViewChangedEvent, value);
    }

    public event EventHandler<ScrollEventArgs> Scroll;

    public event IndexChangedEventHandler ViewStackIndexChanged
    {
        add => this.AddHandler(ZoomBox.ViewStackIndexChangedEvent, value);
        remove => this.RemoveHandler(ZoomBox.ViewStackIndexChangedEvent, value);
    }

    private void CanGoBack(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStackIndex > 0;
    }

    private void GoBack(object sender, ExecutedRoutedEventArgs e) => this.GoBack();

    private void CenterContent(object sender, ExecutedRoutedEventArgs e) => this.CenterContent();

    private void FillToBounds(object sender, ExecutedRoutedEventArgs e) => this.FillToBounds();

    private void FitToBounds(object sender, ExecutedRoutedEventArgs e) => this.FitToBounds();

    private void CanGoForward(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStackIndex < this.ViewStack.Count - 1;
    }

    private void GoForward(object sender, ExecutedRoutedEventArgs e) => this.GoForward();

    private void CanGoHome(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStack.Count > 0 && this.ViewStackIndex != 0;
    }

    private void GoHome(object sender, ExecutedRoutedEventArgs e) => this.GoHome();

    private void PanDownExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Position = new Point(this._basePosition.X, this._basePosition.Y + this.PanDistance);
    }

    private void PanLeftExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Position = new Point(this._basePosition.X - this.PanDistance, this._basePosition.Y);
    }

    private void PanRightExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Position = new Point(this._basePosition.X + this.PanDistance, this._basePosition.Y);
    }

    private void PanUpExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Position = new Point(this._basePosition.X, this._basePosition.Y - this.PanDistance);
    }

    private void CanRefocusView(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = this.EffectiveViewStackMode == ZoomboxViewStackMode.Manual && this.ViewStackIndex >= 0 && this.ViewStackIndex < this.ViewStack.Count && this.CurrentView != this.ViewStack[this.ViewStackIndex];
    }

    private void RefocusView(object sender, ExecutedRoutedEventArgs e) => this.RefocusView();

    private void ZoomInExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Zoom(this.ZoomPercentage / 100.0);
    }

    private void ZoomOutExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        this.Zoom(-this.ZoomPercentage / 100.0);
    }

    public void CenterContent()
    {
        if (this._content == null)
            return;
        this.SetScrollBars();
        this.ZoomTo(ZoomboxView.Center);
    }

    public void FillToBounds()
    {
        if (this._content == null)
            return;
        this.SetScrollBars();
        this.ZoomTo(ZoomboxView.Fill);
    }

    public void FitToBounds()
    {
        if (this._content == null)
            return;
        this.SetScrollBars();
        this.ZoomTo(ZoomboxView.Fit);
    }

    public void GoBack()
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex <= 0)
            return;
        --this.ViewStackIndex;
    }

    public void GoForward()
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex >= this.ViewStack.Count - 1)
            return;
        ++this.ViewStackIndex;
    }

    public void GoHome()
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex <= 0)
            return;
        this.ViewStackIndex = 0;
    }

    public override void OnApplyTemplate()
    {
        this.AttachToVisualTree();
        base.OnApplyTemplate();
        this.SetCurrentView(ZoomboxView.Empty);
        this.GoHome();
    }

    public void RefocusView()
    {
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex < 0 || this.ViewStackIndex >= this.ViewStack.Count || !(this.CurrentView != this.ViewStack[this.ViewStackIndex]))
            return;
        this.UpdateView(this.ViewStack[this.ViewStackIndex], true, false, this.ViewStackIndex);
    }

    public void Zoom(double percentage)
    {
        if (this._content == null)
            return;
        this.Zoom(percentage, this.GetZoomRelativePoint());
    }

    public void Zoom(double percentage, Point relativeTo)
    {
        if (this._content == null)
            return;
        this.ZoomTo(this.Scale * (1.0 + percentage), relativeTo);
    }

    public void ZoomTo(Point position)
    {
        if (this._content == null)
            return;
        this.ZoomTo(new ZoomboxView(new Point(-position.X, -position.Y)));
    }

    public void ZoomTo(Rect region)
    {
        if (this._content == null)
            return;
        this.UpdateView(new ZoomboxView(region), true, true);
    }

    public void ZoomTo(double scale)
    {
        if (this._content == null)
            return;
        this.ZoomTo(scale, true);
    }

    public void ZoomTo(double scale, Point relativeTo) => this.ZoomTo(scale, relativeTo, true, true);

    public void ZoomTo(ZoomboxView view) => this.UpdateView(view, true, true);

    internal void UpdateStackProperties()
    {
        this.SetValue(ZoomBox.HasBackStackPropertyKey, (object)(this.ViewStackIndex > 0));
        this.SetValue(ZoomBox.HasForwardStackPropertyKey, (object)(this.ViewStack.Count > this.ViewStackIndex + 1));
        CommandManager.InvalidateRequerySuggested();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (this._content != null)
        {
            Size size = base.MeasureOverride(constraint);
            this._content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return size;
        }
        if (double.IsInfinity(constraint.Height))
            constraint.Height = 0.0;
        if (double.IsInfinity(constraint.Width))
            constraint.Width = 0.0;
        return constraint;
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        if (oldContent is FrameworkElement)
            (oldContent as FrameworkElement).RemoveHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged));
        else
            this.RemoveHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged));
        if (this._content is FrameworkElement)
            (this._content as FrameworkElement).AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged), true);
        else
            this.AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(this.OnContentSizeChanged), true);
        if (this._viewFinderDisplay == null || this._viewFinderDisplay.VisualBrush == null)
            return;
        this._viewFinderDisplay.VisualBrush.Visual = (Visual)this._content;
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        this.MonitorInput();
        base.OnGotKeyboardFocus(e);
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        this.MonitorInput();
        base.OnLostKeyboardFocus(e);
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        this.CoerceValue(ZoomBox.IsAnimatedProperty);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (this.HasArrangedContentPresenter && !this.HasRenderedFirstView)
        {
            this.HasRenderedFirstView = true;
            if (this.RefocusViewOnFirstRender)
            {
                this.RefocusViewOnFirstRender = false;
                bool isAnimated = this.IsAnimated;
                this.IsAnimated = false;
                try
                {
                    this.RefocusView();
                }
                finally
                {
                    this.IsAnimated = isAnimated;
                }
            }
        }
        base.OnRender(drawingContext);
    }

    private static void RefocusView(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox zoombox = ((((o as ZoomBox))));
        zoombox.UpdateView(zoombox.CurrentView, true, false, zoombox.ViewStackIndex);
    }



    private void CreateVisualBrushForViewFinder(Visual visual)
    {
        this._viewFinderDisplay.VisualBrush = new VisualBrush(visual);
        this._viewFinderDisplay.VisualBrush.Stretch = Stretch.Uniform;
        this._viewFinderDisplay.VisualBrush.AlignmentX = AlignmentX.Left;
        this._viewFinderDisplay.VisualBrush.AlignmentY = AlignmentY.Top;
    }

    private void ContentPresenterFirstArranged(object sender, EventArgs e)
    {
        this._contentPresenter.LayoutUpdated -= new EventHandler(this.ContentPresenterFirstArranged);
        this.HasArrangedContentPresenter = true;
        this.InvalidateVisual();
        bool isAnimated = this.IsAnimated;
        this.IsAnimated = false;
        try
        {
            double scale = this.Scale;
            Point position = this.Position;
            if (this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled)
            {
                bool flag = false;
                if (this.ViewStack.Count > 0)
                {
                    if (this.ViewStackIndex >= 0)
                    {
                        if (this.ViewStackIndex > this.ViewStack.Count - 1)
                            this.ViewStackIndex = this.ViewStack.Count - 1;
                        else
                            this.UpdateView(this.ViewStack[this.ViewStackIndex], false, false, this.ViewStackIndex);
                    }
                    else if (this.EffectiveViewStackMode != 0 && this.ViewStackIndex < 0)
                        this.ViewStackIndex = 0;
                    if (this.ViewStackIndex >= 0)
                    {
                        flag = true;
                        if (!DoubleHelper.IsNaN(scale) || !PointHelper.IsEmpty(position))
                            this.UpdateView(new ZoomboxView(scale, position), false, false);
                    }
                }
                if (!flag)
                {
                    ZoomboxView view = new ZoomboxView(DoubleHelper.IsNaN(this.Scale) ? 1.0 : this.Scale, PointHelper.IsEmpty(position) ? new Point() : position);
                    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Auto)
                    {
                        this.ViewStack.PushView(view);
                        this.ViewStackIndex = 0;
                    }
                    else
                        this.UpdateView(view, false, false);
                }
            }
            else
                this.UpdateView(new ZoomboxView(DoubleHelper.IsNaN(this.Scale) ? 1.0 : this.Scale, position), false, false);
        }
        finally
        {
            this.IsAnimated = isAnimated;
        }
        this.ZoomTo(this.Scale);
        this.UpdateViewFinderDisplayContentBounds();
    }

    private void DetachFromVisualTree()
    {
        if (this._dragAdorner != null && AdornerLayer.GetAdornerLayer((Visual)this) != null)
            AdornerLayer.GetAdornerLayer((Visual)this).Remove((Adorner)this._dragAdorner);
        this.InputBindings.Clear();
        if (this._contentPresenter != null)
            this._contentPresenter.LayoutUpdated -= new EventHandler(this.ContentPresenterFirstArranged);
        if (this._verticalScrollBar != null)
            this._verticalScrollBar.Scroll -= new ScrollEventHandler(this.VerticalScrollBar_Scroll);
        if (this._horizontalScrollBar != null)
            this._horizontalScrollBar.Scroll -= new ScrollEventHandler(this.HorizontalScrollBar_Scroll);
        if (this._viewFinderDisplay != null)
        {
            this._viewFinderDisplay.MouseMove -= new MouseEventHandler(this.ViewFinderDisplayMouseMove);
            this._viewFinderDisplay.MouseLeftButtonDown -= new MouseButtonEventHandler(this.ViewFinderDisplayBeginCapture);
            this._viewFinderDisplay.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ViewFinderDisplayEndCapture);
            BindingOperations.ClearBinding((DependencyObject)this._viewFinderDisplay, ZoomboxViewFinderDisplay.ViewportRectProperty);
            this._viewFinderDisplay = (ZoomboxViewFinderDisplay)null;
        }
        this._contentPresenter = (ContentPresenter)null;
    }

    private void Zoombox_Loaded(object sender, RoutedEventArgs e) => this.SetScrollBars();

    private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        double num = -(e.NewValue + this._relativePosition.Y);
        if (e.ScrollEventType == ScrollEventType.LargeIncrement)
            num = -this._verticalScrollBar.ViewportSize;
        else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
            num = this._verticalScrollBar.ViewportSize;
        this.OnDrag(new DragDeltaEventArgs(0.0, num / this.Scale), false);
        EventHandler<ScrollEventArgs> scroll = this.Scroll;
        if (scroll == null)
            return;
        scroll((object)this, e);
    }

    private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        double num = -(e.NewValue + this._relativePosition.X);
        if (e.ScrollEventType == ScrollEventType.LargeIncrement)
            num = -this._horizontalScrollBar.ViewportSize;
        else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
            num = this._horizontalScrollBar.ViewportSize;
        this.OnDrag(new DragDeltaEventArgs(num / this.Scale, 0.0), false);
        EventHandler<ScrollEventArgs> scroll = this.Scroll;
        if (scroll == null)
            return;
        scroll((object)this, e);
    }

    private void DragDisplayViewport(DragDeltaEventArgs e, bool end)
    {
        double scale = this._viewFinderDisplay.Scale;
        Rect viewportRect = this._viewFinderDisplay.ViewportRect;
        Rect contentBounds = this._viewFinderDisplay.ContentBounds;
        if (viewportRect.Contains(contentBounds))
            return;
        double num1 = e.HorizontalChange;
        double num2 = e.VerticalChange;
        if (viewportRect.Left < contentBounds.Left)
            num1 = Math.Max(0.0, num1);
        else if (viewportRect.Left + num1 < contentBounds.Left)
            num1 = contentBounds.Left - viewportRect.Left;
        if (viewportRect.Right > contentBounds.Right)
            num1 = Math.Min(0.0, num1);
        else if (viewportRect.Right + num1 > contentBounds.Left + contentBounds.Width)
            num1 = contentBounds.Left + contentBounds.Width - viewportRect.Right;
        if (viewportRect.Top < contentBounds.Top)
            num2 = Math.Max(0.0, num2);
        else if (viewportRect.Top + num2 < contentBounds.Top)
            num2 = contentBounds.Top - viewportRect.Top;
        if (viewportRect.Bottom > contentBounds.Bottom)
            num2 = Math.Min(0.0, num2);
        else if (viewportRect.Bottom + num2 > contentBounds.Top + contentBounds.Height)
            num2 = contentBounds.Top + contentBounds.Height - viewportRect.Bottom;
        this.OnDrag(new DragDeltaEventArgs(-num1 / scale / this._viewboxFactor, -num2 / scale / this._viewboxFactor), end);
        this._originPoint += new Vector(num1, num2);
    }

    private void InitCommands()
    {
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Back, new ExecutedRoutedEventHandler(this.GoBack), new CanExecuteRoutedEventHandler(this.CanGoBack)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Center, new ExecutedRoutedEventHandler(this.CenterContent)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Fill, new ExecutedRoutedEventHandler(this.FillToBounds)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Fit, new ExecutedRoutedEventHandler(this.FitToBounds)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Forward, new ExecutedRoutedEventHandler(this.GoForward), new CanExecuteRoutedEventHandler(this.CanGoForward)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Home, new ExecutedRoutedEventHandler(this.GoHome), new CanExecuteRoutedEventHandler(this.CanGoHome)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.PanDown, new ExecutedRoutedEventHandler(this.PanDownExecuted)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.PanLeft, new ExecutedRoutedEventHandler(this.PanLeftExecuted)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.PanRight, new ExecutedRoutedEventHandler(this.PanRightExecuted)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.PanUp, new ExecutedRoutedEventHandler(this.PanUpExecuted)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.Refocus, new ExecutedRoutedEventHandler(this.RefocusView), new CanExecuteRoutedEventHandler(this.CanRefocusView)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.ZoomIn, new ExecutedRoutedEventHandler(this.ZoomInExecuted)));
        this.CommandBindings.Add(new CommandBinding((ICommand)ZoomBox.ZoomOut, new ExecutedRoutedEventHandler(this.ZoomOutExecuted)));
    }

    private void MonitorInput()
    {
        if (!this.HasUIPermission)
            return;
        this.PreProcessInput();
    }

    private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.UpdateViewFinderDisplayContentBounds();
        if (!this.HasArrangedContentPresenter)
            return;
        if (this.HasRenderedFirstView)
        {
            this.SetScrollBars();
            this.UpdateView(this.CurrentView, true, false, this.CurrentViewIndex);
        }
        else
        {
            this.RefocusViewOnFirstRender = true;
            this.InvalidateVisual();
        }
    }

    private void OnDrag(DragDeltaEventArgs e, bool end)
    {
        Point relativePosition = this._relativePosition;
        double scale = this.Scale;
        Point position = relativePosition + this.ContentOffset * scale + new Vector(e.HorizontalChange * scale, e.VerticalChange * scale);
        if (this.IsUsingScrollBars)
        {
            position.X = Math.Max(Math.Min(position.X, 0.0), -this._horizontalScrollBar.Maximum);
            position.Y = Math.Max(Math.Min(position.Y, 0.0), -this._verticalScrollBar.Maximum);
        }
        this.UpdateView(new ZoomboxView(scale, position), false, end);
    }

    private void OnLayoutUpdated(object sender, EventArgs e) => this.UpdateViewport();

    private void OnSelectRegion(DragDeltaEventArgs e, bool end)
    {
        if (end)
        {
            _dragAdorner.Rect = Rect.Empty;
            if (_trueContent != null)
            {
                Rect region = new Rect(TranslatePoint(_dragAdorner.LastPosition, _trueContent), TranslatePoint(_dragAdorner.LastPosition + new Vector(_dragAdorner.LastSize.Width, _dragAdorner.LastSize.Height), _trueContent));
                ZoomTo(region);
            }
        }
        else
        {
            _dragAdorner.Rect = Rect.Intersect(new Rect(_originPoint, new Vector(e.HorizontalChange, e.VerticalChange)), new Rect(new Point(0.0, 0.0), new Point(base.RenderSize.Width, base.RenderSize.Height)));
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!this.HasArrangedContentPresenter)
            return;
        this.SetScrollBars();
        this.UpdateViewboxFactor();
        bool isAnimated = this.IsAnimated;
        this.IsAnimated = false;
        try
        {
            this.UpdateView(this.CurrentView, false, false, this.ViewStackIndex);
        }
        finally
        {
            this.IsAnimated = isAnimated;
        }
    }

    private void SetScrollBars()
    {
        if (this._content == null || this._verticalScrollBar == null || this._horizontalScrollBar == null)
            return;
        Size size = this._content is Viewbox ? ((Decorator)this._content).Child.DesiredSize : this.RenderSize;
        this._verticalScrollBar.SmallChange = 10.0;
        this._verticalScrollBar.LargeChange = 10.0;
        this._verticalScrollBar.Minimum = 0.0;
        ScrollBar verticalScrollBar = this._verticalScrollBar;
        Size renderSize = this.RenderSize;
        double height = renderSize.Height;
        verticalScrollBar.ViewportSize = height;
        this._verticalScrollBar.Maximum = size.Height - this._verticalScrollBar.ViewportSize;
        this._horizontalScrollBar.SmallChange = 10.0;
        this._horizontalScrollBar.LargeChange = 10.0;
        this._horizontalScrollBar.Minimum = 0.0;
        ScrollBar horizontalScrollBar = this._horizontalScrollBar;
        renderSize = this.RenderSize;
        double width = renderSize.Width;
        horizontalScrollBar.ViewportSize = width;
        this._horizontalScrollBar.Maximum = size.Width - this._horizontalScrollBar.ViewportSize;
    }

    private void PreProcessInput()
    {
        if (this.IsMouseOver || this.IsKeyboardFocusWithin)
        {
            if (this.IsMonitoringInput)
                return;
            this.IsMonitoringInput = true;
            InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(this.PreProcessInput);
            this.UpdateKeyModifierTriggerProperties();
        }
        else if (this.IsMonitoringInput)
        {
            this.IsMonitoringInput = false;
            InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.PreProcessInput);
            this.SetAreDragModifiersActive(false);
            this.SetAreRelativeZoomModifiersActive(false);
            this.SetAreZoomModifiersActive(false);
            this.SetAreZoomToSelectionModifiersActive(false);
        }
    }

    private void PreProcessInput(object sender, NotifyInputEventArgs e)
    {
        if (!(e.StagingItem.Input is KeyEventArgs))
            return;
        this.UpdateKeyModifierTriggerProperties();
    }

    private void ProcessMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (this.ZoomToSelectionModifiers.AreActive)
        {
            this.SetIsDraggingContent(false);
            this.SetIsSelectingRegion(true);
        }
        else if (this.DragModifiers.AreActive)
        {
            this.SetIsSelectingRegion(false);
            this.SetIsDraggingContent(true);
        }
        else
        {
            this.SetIsSelectingRegion(false);
            this.SetIsDraggingContent(false);
        }
        if (!this.IsSelectingRegion && !this.IsDraggingContent)
            return;
        this._originPoint = e.GetPosition((IInputElement)this);
        this._contentPresenter.CaptureMouse();
        e.Handled = true;
        if (this.IsDraggingContent)
        {
            this.OnDrag(new DragDeltaEventArgs(0.0, 0.0), false);
        }
        else
        {
            if (!this.IsSelectingRegion)
                return;
            this.OnSelectRegion(new DragDeltaEventArgs(0.0, 0.0), false);
        }
    }

    private void ProcessMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (!this.IsDraggingContent && !this.IsSelectingRegion)
            return;
        bool isDraggingContent = this.IsDraggingContent;
        this.SetIsDraggingContent(false);
        this.SetIsSelectingRegion(false);
        this._originPoint = new Point();
        this._contentPresenter.ReleaseMouseCapture();
        e.Handled = true;
        if (isDraggingContent)
            this.OnDrag(new DragDeltaEventArgs(0.0, 0.0), true);
        else
            this.OnSelectRegion(new DragDeltaEventArgs(0.0, 0.0), true);
    }

    private void ProcessMouseMove(MouseEventArgs e)
    {
        if (e.MouseDevice.LeftButton != MouseButtonState.Pressed || !this.IsDraggingContent && !this.IsSelectingRegion)
            return;
        Point position = e.GetPosition((IInputElement)this);
        e.Handled = true;
        if (this.IsDraggingContent)
        {
            Vector vector = (position - this._originPoint) / this.Scale;
            this.OnDrag(new DragDeltaEventArgs(vector.X, vector.Y), false);
            this._originPoint = position;
        }
        else
        {
            if (!this.IsSelectingRegion)
                return;
            Vector vector = position - this._originPoint;
            this.OnSelectRegion(new DragDeltaEventArgs(vector.X, vector.Y), false);
        }
    }

    private void ProcessMouseWheelZoom(MouseWheelEventArgs e)
    {
        if (this._content == null)
            return;
        bool flag = this.ZoomModifiers.AreActive;
        bool areActive = this.RelativeZoomModifiers.AreActive;
        if (flag & areActive)
            flag = false;
        if (!(flag | areActive))
            return;
        e.Handled = true;
        double percentage = (double)(e.Delta / ZoomBox.MOUSE_WHEEL_DELTA) * this.ZoomPercentage / 100.0;
        if (areActive)
            this.Zoom(percentage, Mouse.GetPosition((IInputElement)this._content));
        else
            this.Zoom(percentage);
    }

    private void ProcessNavigationButton(RoutedEventArgs e)
    {
        switch (e)
        {
            case MouseButtonEventArgs _:
                MouseButtonEventArgs mouseButtonEventArgs = e as MouseButtonEventArgs;
                if (mouseButtonEventArgs.ChangedButton != MouseButton.XButton1 && mouseButtonEventArgs.ChangedButton != MouseButton.XButton2)
                    break;
                if (mouseButtonEventArgs.ChangedButton == MouseButton.XButton2)
                    this.GoForward();
                else
                    this.GoBack();
                mouseButtonEventArgs.Handled = true;
                break;
            case KeyEventArgs _:
                KeyEventArgs keyEventArgs = e as KeyEventArgs;
                if (keyEventArgs.Key == Key.Back || keyEventArgs.Key == Key.BrowserBack || keyEventArgs.Key == Key.BrowserForward)
                {
                    if (keyEventArgs.Key == Key.BrowserForward)
                        this.GoForward();
                    else
                        this.GoBack();
                    keyEventArgs.Handled = true;
                }
                break;
        }
    }

    private void ResizeDisplayViewport(DragDeltaEventArgs e, ResizeEdge relativeTo)
    {
        Rect viewportRect = this._viewFinderDisplay.ViewportRect;
        double scale = this._viewFinderDisplay.Scale;
        double num1 = Math.Max(this._resizeViewportBounds.Left, Math.Min(this._resizeDraggingPoint.X + e.HorizontalChange, this._resizeViewportBounds.Right));
        double num2 = Math.Max(this._resizeViewportBounds.Top, Math.Min(this._resizeDraggingPoint.Y + e.VerticalChange, this._resizeViewportBounds.Bottom));
        Point point = new Point(this._resizeAnchorPoint.X / scale, this._resizeAnchorPoint.Y / scale);
        Vector vector = new Vector((num1 - this._resizeAnchorPoint.X) / scale / this._viewboxFactor, (num2 - this._resizeAnchorPoint.Y) / scale / this._viewboxFactor);
        Rect rect = new Rect(point, vector);
        rect = new Rect(this._content.TranslatePoint(rect.TopLeft, (UIElement)this._contentPresenter), this._content.TranslatePoint(rect.BottomRight, (UIElement)this._contentPresenter));
        double num3 = this.RenderSize.Width / rect.Width;
        double num4 = this.RenderSize.Height / rect.Height;
        this.ZoomTo(num3 < num4 ? num3 : num4, point, false, false);
    }

    private void UpdateKeyModifierTriggerProperties()
    {
        this.SetAreDragModifiersActive(this.DragModifiers.AreActive);
        this.SetAreRelativeZoomModifiersActive(this.RelativeZoomModifiers.AreActive);
        this.SetAreZoomModifiersActive(this.ZoomModifiers.AreActive);
        this.SetAreZoomToSelectionModifiersActive(this.ZoomToSelectionModifiers.AreActive);
    }

    private void UpdateView(ZoomboxView view, bool allowAnimation, bool allowStackAddition)
    {
        this.UpdateView(view, allowAnimation, allowStackAddition, -1);
    }
    private bool IsGreaterThanOrClose(double value1, double value2)
    {
        return value1 > value2 || DoubleHelper.AreVirtuallyEqual(value1, value2);
    }

    private Rect CalculateFillRect()
    {
        Size renderSize = this.RenderSize;
        double width1 = renderSize.Width;
        renderSize = this.RenderSize;
        double height1 = renderSize.Height;
        double num = width1 / height1;
        double x = 0.0;
        double y = 0.0;
        double width2 = this.ContentRect.Width;
        double height2 = this.ContentRect.Height;
        if (num > width2 / height2)
        {
            height2 = width2 / num;
            y = (this.ContentRect.Height - height2) / 2.0;
        }
        else
        {
            width2 = height2 * num;
            x = (this.ContentRect.Width - width2) / 2.0;
        }
        return new Rect(x, y, width2, height2);
    }

    private void CalculatePositionAndScale(
      Rect region,
      ref Point newRelativePosition,
      ref double newRelativeScale)
    {
        if (region.Width == 0.0 || region.Height == 0.0 || !this.ContentRect.IntersectsWith(region))
            return;
        region = new Rect(this._content.TranslatePoint(region.TopLeft, (UIElement)this._contentPresenter), this._content.TranslatePoint(region.BottomRight, (UIElement)this._contentPresenter));
        double num1 = this.RenderSize.Width / region.Width;
        Size renderSize = this.RenderSize;
        double num2 = renderSize.Height / region.Height;
        newRelativeScale = num1 < num2 ? num1 : num2;
        if (newRelativeScale > this.MaxScale)
            newRelativeScale = this.MaxScale;
        else if (newRelativeScale < this.MinScale)
            newRelativeScale = this.MinScale;
        double x1 = 0.0;
        double y1 = 0.0;
        switch (this.HorizontalContentAlignment)
        {
            case HorizontalAlignment.Center:
            case HorizontalAlignment.Stretch:
                renderSize = this.RenderSize;
                x1 = (renderSize.Width - region.Width * newRelativeScale) / 2.0;
                break;
            case HorizontalAlignment.Right:
                renderSize = this.RenderSize;
                x1 = renderSize.Width - region.Width * newRelativeScale;
                break;
        }
        switch (this.VerticalContentAlignment)
        {
            case VerticalAlignment.Center:
            case VerticalAlignment.Stretch:
                renderSize = this.RenderSize;
                y1 = (renderSize.Height - region.Height * newRelativeScale) / 2.0;
                break;
            case VerticalAlignment.Bottom:
                renderSize = this.RenderSize;
                y1 = renderSize.Height - region.Height * newRelativeScale;
                break;
        }
        ref Point local = ref newRelativePosition;
        Point topLeft = region.TopLeft;
        double x2 = -topLeft.X * newRelativeScale;
        topLeft = region.TopLeft;
        double y2 = -topLeft.Y * newRelativeScale;
        Point point = new Point(x2, y2) + new Vector(x1, y1);
        local = point;
    }

    private void UpdateViewFinderDisplayContentBounds()
    {
        if (this._content == null || this._trueContent == null || this._viewFinderDisplay == null || this._viewFinderDisplay.AvailableSize.IsEmpty)
            return;
        this.UpdateViewboxFactor();
        Size renderSize = this._content.RenderSize;
        Size size = this._viewFinderDisplay.AvailableSize;
        if (size.Width > 0.0 && DoubleHelper.AreVirtuallyEqual(size.Height, 0.0))
            size = new Size(size.Width, renderSize.Height * size.Width / renderSize.Width);
        else if (size.Height > 0.0 && DoubleHelper.AreVirtuallyEqual(size.Width, 0.0))
            size = new Size(renderSize.Width * size.Height / renderSize.Height, size.Width);
        double num1 = size.Width / renderSize.Width;
        double num2 = size.Height / renderSize.Height;
        double num3 = num1 < num2 ? num1 : num2;
        double width = renderSize.Width * num3;
        double height = renderSize.Height * num3;
        this._viewFinderDisplay.Scale = num3;
        this._viewFinderDisplay.ContentBounds = new Rect(new Size(width, height));
    }

    private void UpdateViewboxFactor()
    {
        if (this._content == null || this._trueContent == null)
            return;
        Size renderSize = this._content.RenderSize;
        double width1 = renderSize.Width;
        renderSize = this._trueContent.RenderSize;
        double width2 = renderSize.Width;
        if (DoubleHelper.AreVirtuallyEqual(width1, 0.0) || DoubleHelper.AreVirtuallyEqual(width2, 0.0))
            this._viewboxFactor = 1.0;
        else
            this._viewboxFactor = width1 / width2;
    }
    private void UpdateViewport(object sender, EventArgs e) => this.UpdateViewport();

    private void ViewFinderDisplayBeginCapture(object sender, MouseButtonEventArgs e)
    {
        if (!((((((this._viewFinderDisplay.Tag is ResizeEdge)))))))
            return;
        if ((ResizeEdge)this._viewFinderDisplay.Tag == ZoomBox.ResizeEdge.None)
        {
            this.IsDraggingViewport = true;
        }
        else
        {
            this.IsResizingViewport = true;
            Vector vector = new Vector();
            switch ((ResizeEdge)this._viewFinderDisplay.Tag)
            {
                case ZoomBox.ResizeEdge.TopLeft:
                    this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.TopLeft;
                    this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.BottomRight;
                    vector = new Vector(-1.0, -1.0);
                    break;
                case ZoomBox.ResizeEdge.TopRight:
                    this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.TopRight;
                    this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.BottomLeft;
                    vector = new Vector(1.0, -1.0);
                    break;
                case ZoomBox.ResizeEdge.BottomLeft:
                    this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.BottomLeft;
                    this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.TopRight;
                    vector = new Vector(-1.0, 1.0);
                    break;
                case ZoomBox.ResizeEdge.BottomRight:
                    this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.BottomRight;
                    this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.TopLeft;
                    vector = new Vector(1.0, 1.0);
                    break;
                case ZoomBox.ResizeEdge.Left:
                    double left1 = this._viewFinderDisplay.ViewportRect.Left;
                    Rect viewportRect1 = this._viewFinderDisplay.ViewportRect;
                    double top1 = viewportRect1.Top;
                    viewportRect1 = this._viewFinderDisplay.ViewportRect;
                    double num1 = viewportRect1.Height / 2.0;
                    double y1 = top1 + num1;
                    this._resizeDraggingPoint = new Point(left1, y1);
                    viewportRect1 = this._viewFinderDisplay.ViewportRect;
                    double right1 = viewportRect1.Right;
                    viewportRect1 = this._viewFinderDisplay.ViewportRect;
                    double top2 = viewportRect1.Top;
                    viewportRect1 = this._viewFinderDisplay.ViewportRect;
                    double num2 = viewportRect1.Height / 2.0;
                    double y2 = top2 + num2;
                    this._resizeAnchorPoint = new Point(right1, y2);
                    vector = new Vector(-1.0, 0.0);
                    break;
                case ZoomBox.ResizeEdge.Top:
                    double x1 = this._viewFinderDisplay.ViewportRect.Left + this._viewFinderDisplay.ViewportRect.Width / 2.0;
                    Rect viewportRect2 = this._viewFinderDisplay.ViewportRect;
                    double top3 = viewportRect2.Top;
                    this._resizeDraggingPoint = new Point(x1, top3);
                    viewportRect2 = this._viewFinderDisplay.ViewportRect;
                    double left2 = viewportRect2.Left;
                    viewportRect2 = this._viewFinderDisplay.ViewportRect;
                    double num3 = viewportRect2.Width / 2.0;
                    double x2 = left2 + num3;
                    viewportRect2 = this._viewFinderDisplay.ViewportRect;
                    double bottom1 = viewportRect2.Bottom;
                    this._resizeAnchorPoint = new Point(x2, bottom1);
                    vector = new Vector(0.0, -1.0);
                    break;
                case ZoomBox.ResizeEdge.Right:
                    double right2 = this._viewFinderDisplay.ViewportRect.Right;
                    Rect viewportRect3 = this._viewFinderDisplay.ViewportRect;
                    double top4 = viewportRect3.Top;
                    viewportRect3 = this._viewFinderDisplay.ViewportRect;
                    double num4 = viewportRect3.Height / 2.0;
                    double y3 = top4 + num4;
                    this._resizeDraggingPoint = new Point(right2, y3);
                    viewportRect3 = this._viewFinderDisplay.ViewportRect;
                    double left3 = viewportRect3.Left;
                    viewportRect3 = this._viewFinderDisplay.ViewportRect;
                    double top5 = viewportRect3.Top;
                    viewportRect3 = this._viewFinderDisplay.ViewportRect;
                    double num5 = viewportRect3.Height / 2.0;
                    double y4 = top5 + num5;
                    this._resizeAnchorPoint = new Point(left3, y4);
                    vector = new Vector(1.0, 0.0);
                    break;
                case ZoomBox.ResizeEdge.Bottom:
                    double x3 = this._viewFinderDisplay.ViewportRect.Left + this._viewFinderDisplay.ViewportRect.Width / 2.0;
                    Rect viewportRect4 = this._viewFinderDisplay.ViewportRect;
                    double bottom2 = viewportRect4.Bottom;
                    this._resizeDraggingPoint = new Point(x3, bottom2);
                    viewportRect4 = this._viewFinderDisplay.ViewportRect;
                    double left4 = viewportRect4.Left;
                    viewportRect4 = this._viewFinderDisplay.ViewportRect;
                    double num6 = viewportRect4.Width / 2.0;
                    double x4 = left4 + num6;
                    viewportRect4 = this._viewFinderDisplay.ViewportRect;
                    double top6 = viewportRect4.Top;
                    this._resizeAnchorPoint = new Point(x4, top6);
                    vector = new Vector(0.0, 1.0);
                    break;
            }
            double scale = this._viewFinderDisplay.Scale;
            Rect contentBounds = this._viewFinderDisplay.ContentBounds;
            this._resizeViewportBounds = new Rect(this._resizeAnchorPoint + new Vector(vector.X * 10000000000.0, vector.Y * 10000000000.0), this._resizeAnchorPoint + new Vector(vector.X * contentBounds.Width / this.MaxScale, vector.Y * contentBounds.Height / this.MaxScale));
        }
        this._originPoint = e.GetPosition((IInputElement)this._viewFinderDisplay);
        this._viewFinderDisplay.CaptureMouse();
        e.Handled = true;
    }

    private void ViewFinderDisplayEndCapture(object sender, MouseButtonEventArgs e)
    {
        if (!this.IsDraggingViewport && !this.IsResizingViewport)
            return;
        this.DragDisplayViewport(new DragDeltaEventArgs(0.0, 0.0), true);
        this.IsDraggingViewport = false;
        this.IsResizingViewport = false;
        this._originPoint = new Point();
        this._viewFinderDisplay.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void ViewFinderDisplayMouseMove(object sender, MouseEventArgs e)
    {
        if (e.MouseDevice.LeftButton == MouseButtonState.Pressed && (this.IsDraggingViewport || this.IsResizingViewport))
        {
            Vector vector = e.GetPosition((IInputElement)this._viewFinderDisplay) - this._originPoint;
            if (this.IsDraggingViewport)
                this.DragDisplayViewport(new DragDeltaEventArgs(vector.X, vector.Y), false);
            else
                this.ResizeDisplayViewport(new DragDeltaEventArgs(vector.X, vector.Y), (ResizeEdge)this._viewFinderDisplay.Tag);
            e.Handled = true;
        }
        else
        {
            Point position = e.GetPosition((IInputElement)this._viewFinderDisplay);
            Rect viewportRect = this._viewFinderDisplay.ViewportRect;
            double num = viewportRect.Width * viewportRect.Height > 100.0 ? 5.0 : Math.Sqrt(viewportRect.Width * viewportRect.Height) / 2.0;
            if (viewportRect.Contains(position) && !DoubleHelper.AreVirtuallyEqual(Rect.Intersect(viewportRect, this._viewFinderDisplay.ContentBounds), this._viewFinderDisplay.ContentBounds))
            {
                if (PointHelper.DistanceBetween(position, viewportRect.TopLeft) < num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.TopLeft;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNWSE;
                }
                else if (PointHelper.DistanceBetween(position, viewportRect.BottomRight) < num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.BottomRight;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNWSE;
                }
                else if (PointHelper.DistanceBetween(position, viewportRect.TopRight) < num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.TopRight;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNESW;
                }
                else if (PointHelper.DistanceBetween(position, viewportRect.BottomLeft) < num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.BottomLeft;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNESW;
                }
                else if (position.X <= viewportRect.Left + num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.Left;
                    this._viewFinderDisplay.Cursor = Cursors.SizeWE;
                }
                else if (position.Y <= viewportRect.Top + num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.Top;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNS;
                }
                else if (position.X >= viewportRect.Right - num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.Right;
                    this._viewFinderDisplay.Cursor = Cursors.SizeWE;
                }
                else if (position.Y >= viewportRect.Bottom - num)
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.Bottom;
                    this._viewFinderDisplay.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this._viewFinderDisplay.Tag = (object)ZoomBox.ResizeEdge.None;
                    this._viewFinderDisplay.Cursor = Cursors.SizeAll;
                }
            }
            else
            {
                this._viewFinderDisplay.Tag = (object)null;
                this._viewFinderDisplay.Cursor = Cursors.Arrow;
            }
        }
    }

    private void ZoomAnimationCompleted(object sender, EventArgs e)
    {
        if ((sender as AnimationClock).CurrentState == 0)
            return;
        (sender as AnimationClock).CurrentStateInvalidated -= new EventHandler(this.ZoomAnimationCompleted);
        (sender as AnimationClock).CurrentTimeInvalidated -= new EventHandler(this.UpdateViewport);
        this.RaiseEvent(new RoutedEventArgs(ZoomBox.AnimationCompletedEvent, (object)this));
    }

    private void VerticalValueAnimation_Completed(object sender, EventArgs e)
    {
        if (this._verticalScrollBar.Value != -this._relativePosition.Y && this._verticalScrollBar.Value != this._verticalScrollBar.Maximum && this._verticalScrollBar.Value != this._verticalScrollBar.Minimum)
            return;
        double num = this._verticalScrollBar.Value;
        this._verticalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline)null);
        this._verticalScrollBar.Value = num;
    }

    private void HorizontalValueAnimation_Completed(object sender, EventArgs e)
    {
        if (this._horizontalScrollBar.Value != -this._relativePosition.X && this._horizontalScrollBar.Value != this._horizontalScrollBar.Maximum && this._horizontalScrollBar.Value != this._horizontalScrollBar.Minimum)
            return;
        double num = this._horizontalScrollBar.Value;
        this._horizontalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline)null);
        this._horizontalScrollBar.Value = num;
    }

    private void ZoomTo(double scale, bool allowStackAddition)
    {
        if (this._content == null)
            return;
        this.ZoomTo(scale, this.GetZoomRelativePoint(), true, allowStackAddition);
    }

    private void ZoomTo(
      double scale,
      Point relativeTo,
      bool restrictRelativePointToContent,
      bool allowStackAddition)
    {
        if (this._content == null || double.IsNaN(scale) || restrictRelativePointToContent && !new Rect(this._content.RenderSize).Contains(relativeTo))
            return;
        if (scale > this.MaxScale)
            scale = this.MaxScale;
        else if (scale < this.MinScale)
            scale = this.MinScale;
        Point point = relativeTo;
        if (this.HasRenderedFirstView)
        {
            relativeTo = this._content.TranslatePoint(relativeTo, (UIElement)this);
            point = this.TranslatePoint(relativeTo, (UIElement)this._contentPresenter);
        }
        else if (this._contentPresenter != null)
        {
            if (this._contentPresenter.RenderTransform == Transform.Identity)
                this.UpdateView(new ZoomboxView(1.0, new Point(0.0, 0.0)), false, false);
            relativeTo = this._contentPresenter.RenderTransform.Transform(relativeTo);
        }
        Point position = new Point(relativeTo.X - point.X * scale / this._viewboxFactor, relativeTo.Y - point.Y * scale / this._viewboxFactor) + this.ContentOffset * scale / this._viewboxFactor;
        this.UpdateView(new ZoomboxView(scale, position), !this.IsResizingViewport, allowStackAddition);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (this.NavigateOnPreview && !e.Handled)
            this.ProcessNavigationButton((RoutedEventArgs)e);
        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!this.NavigateOnPreview && !e.Handled)
            this.ProcessNavigationButton((RoutedEventArgs)e);
        base.OnKeyDown(e);
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        if (this.NavigateOnPreview && !e.Handled)
            this.ProcessNavigationButton((RoutedEventArgs)e);
        base.OnPreviewMouseDown(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (!this.NavigateOnPreview && !e.Handled)
            this.ProcessNavigationButton((RoutedEventArgs)e);
        base.OnMouseDown(e);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        this.MonitorInput();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        this.MonitorInput();
        base.OnMouseLeave(e);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseLeftButtonDown(e);
        base.OnPreviewMouseLeftButtonDown(e);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseLeftButtonDown(e);
        base.OnMouseLeftButtonDown(e);
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseLeftButtonUp(e);
        base.OnPreviewMouseLeftButtonUp(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseLeftButtonUp(e);
        base.OnMouseLeftButtonUp(e);
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseMove(e);
        base.OnPreviewMouseMove(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseMove(e);
        base.OnMouseMove(e);
    }

    protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
    {
        if (this.ZoomOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseWheelZoom(e);
        base.OnPreviewMouseWheel(e);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (!this.ZoomOnPreview && !e.Handled && this._contentPresenter != null)
            this.ProcessMouseWheelZoom(e);
        base.OnMouseWheel(e);
    }

    private sealed class ViewFinderSelectionConverter : IValueConverter
    {
        private readonly ZoomBox _zoombox;

        public ViewFinderSelectionConverter(ZoomBox zoombox)
        {
            this._zoombox = zoombox;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rect rect1 = (Rect)value;
            if (rect1.IsEmpty)
                return (object)rect1;
            double num = this._zoombox._viewFinderDisplay.Scale * this._zoombox._viewboxFactor;
            Rect rect2 = new Rect(rect1.Left * num, rect1.Top * num, rect1.Width * num, rect1.Height * num);
            rect2.Offset(this._zoombox._viewFinderDisplay.ContentBounds.Left, this._zoombox._viewFinderDisplay.ContentBounds.Top);
            return (object)rect2;
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return (object)null;
        }
    }

    internal sealed class DragAdorner : Adorner
    {
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(nameof(Brush), typeof(Brush), typeof(DragAdorner), (PropertyMetadata)new FrameworkPropertyMetadata((object)Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty PenProperty = DependencyProperty.Register(nameof(Pen), typeof(Pen), typeof(DragAdorner), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Pen((Brush)new SolidColorBrush(Color.FromArgb((byte)127 /*0x7F*/, (byte)63 /*0x3F*/, (byte)63 /*0x3F*/, (byte)63 /*0x3F*/)), 2.0), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RectProperty = DependencyProperty.Register(nameof(Rect), typeof(Rect), typeof(DragAdorner), (PropertyMetadata)new FrameworkPropertyMetadata((object)Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ZoomBox.DragAdorner.OnRectChanged)));
        private Point _cachedPosition;
        private Size _cachedSize;

        public DragAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
            this.ClipToBounds = true;
        }

        public Brush Brush
        {
            get => (Brush)this.GetValue(ZoomBox.DragAdorner.BrushProperty);
            set => this.SetValue(ZoomBox.DragAdorner.BrushProperty, (object)value);
        }

        public Pen Pen
        {
            get => (Pen)this.GetValue(ZoomBox.DragAdorner.PenProperty);
            set => this.SetValue(ZoomBox.DragAdorner.PenProperty, (object)value);
        }

        public Rect Rect
        {
            get => (Rect)this.GetValue(ZoomBox.DragAdorner.RectProperty);
            set => this.SetValue(ZoomBox.DragAdorner.RectProperty, (object)value);
        }

        private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner dragAdorner = (DragAdorner)d;
            if (((Rect)e.NewValue).IsEmpty)
                return;
            dragAdorner._cachedPosition = ((Rect)e.NewValue).TopLeft;
            dragAdorner._cachedSize = ((Rect)e.NewValue).Size;
        }

        public Point LastPosition => this._cachedPosition;

        public Size LastSize => this._cachedSize;

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(this.Brush, this.Pen, this.Rect);
        }
    }

    private enum CacheBits
    {
        IsUpdatingView = 1,
        IsUpdatingViewport = 2,
        IsDraggingViewport = 4,
        IsResizingViewport = 8,
        IsMonitoringInput = 16, // 0x00000010
        IsContentWrapped = 32, // 0x00000020
        HasArrangedContentPresenter = 64, // 0x00000040
        HasRenderedFirstView = 128, // 0x00000080
        RefocusViewOnFirstRender = 256, // 0x00000100
        HasUIPermission = 512, // 0x00000200
    }

    private enum ResizeEdge
    {
        None,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Left,
        Top,
        Right,
        Bottom,
    }



    private void AttachToVisualTree()
    {
        if (_isUpdatingVisualTree)
        {
            return;
        }
        _isUpdatingVisualTree = true;
        DetachFromVisualTree();
        _dragAdorner = new DragAdorner(this);
        if (base.Template.Resources.Contains("SelectionBrush"))
        {
            _dragAdorner.Brush = base.Template.Resources["SelectionBrush"] as Brush;
        }
        if (base.Template.Resources.Contains("SelectionPen"))
        {
            _dragAdorner.Pen = base.Template.Resources["SelectionPen"] as Pen;
        }
        if (base.Template.Resources.Contains("InputBindings") && base.Template.Resources["InputBindings"] is InputBindingCollection collection)
        {
            base.InputBindings.AddRange(collection);
        }
        _contentPresenter = VisualTreeHelperEx.FindDescendantByType(this, typeof(ContentPresenter)) as ContentPresenter;
        if (_contentPresenter == null)
        {
            throw new InvalidTemplateException("ZoomboxTemplateNeedsContent");
        }
        _verticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
        if (_verticalScrollBar == null)
        {
            throw new InvalidTemplateException("Zoombox vertical scrollBar not found.");
        }
        _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
        _horizontalScrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
        if (_horizontalScrollBar == null)
        {
            throw new InvalidTemplateException("Zoombox horizontal scrollBar not found.");
        }
        _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
        AdornerLayer adornerLayer = null;
        if (VisualTreeHelperEx.FindDescendantByType(this, typeof(AdornerDecorator)) is AdornerDecorator adornerDecorator)
        {
            adornerLayer = adornerDecorator.AdornerLayer;
        }
        else
        {
            try
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(this);
            }
            catch (Exception)
            {
            }
        }
        adornerLayer?.Add(_dragAdorner);
        VisualTreeHelperEx.FindDescendantWithPropertyValue(this, ButtonBase.IsPressedProperty, true);
        if (GetValue(ViewFinderPropertyKey.DependencyProperty) == null || _isUsingDefaultViewFinder)
        {
            SetValue(ViewFinderPropertyKey, base.Template.FindName("ViewFinder", this) as FrameworkElement);
            SetViewFinderVisibility(this, Visibility.Collapsed);
            _isUsingDefaultViewFinder = true;
        }
        else
        {
            SetViewFinderVisibility(this, Visibility.Hidden);
        }
        if (ViewFinder != null)
        {
            _viewFinderDisplay = VisualTreeHelperEx.FindDescendantByType(ViewFinder, typeof(ZoomboxViewFinderDisplay)) as ZoomboxViewFinderDisplay;
        }
        if (ViewFinder != null && _viewFinderDisplay == null)
        {
            throw new InvalidTemplateException("ZoomboxHasViewFinderButNotDisplay");
        }
        if (_viewFinderDisplay != null)
        {
            CreateVisualBrushForViewFinder(_content);
            _viewFinderDisplay.MouseMove += ViewFinderDisplayMouseMove;
            _viewFinderDisplay.MouseLeftButtonDown += ViewFinderDisplayBeginCapture;
            _viewFinderDisplay.MouseLeftButtonUp += ViewFinderDisplayEndCapture;
            Binding binding = new Binding("Viewport");
            binding.Mode = BindingMode.OneWay;
            binding.Converter = new ViewFinderSelectionConverter(this);
            binding.Source = this;
            _viewFinderDisplay.SetBinding(ZoomboxViewFinderDisplay.ViewportRectProperty, binding);
        }
        UpdateViewFinderDisplayContentBounds();
        _contentPresenter.LayoutUpdated += ContentPresenterFirstArranged;
        _isUpdatingVisualTree = false;
    }


    private void UpdateView(ZoomboxView view, bool allowAnimation, bool allowStackAddition, int stackIndex)
    {
        if (_contentPresenter == null || _content == null || !HasArrangedContentPresenter)
        {
            return;
        }
        if (view.ViewKind == ZoomboxViewKind.Absolute && PointHelper.IsEmpty(view.Position))
        {
            ZoomTo(view.Scale, allowStackAddition);
        }
        else
        {
            if (IsUpdatingView)
            {
                return;
            }
            IsUpdatingView = true;
            try
            {
                double newRelativeScale = _viewboxFactor;
                Point newRelativePosition = default(Point);
                Rect region = Rect.Empty;
                switch (view.ViewKind)
                {
                    case ZoomboxViewKind.Absolute:
                        newRelativeScale = (DoubleHelper.IsNaN(view.Scale) ? _relativeScale : view.Scale);
                        newRelativePosition = (PointHelper.IsEmpty(view.Position) ? _relativePosition : (new Point(view.Position.X, view.Position.Y) - ContentOffset * newRelativeScale));
                        break;
                    case ZoomboxViewKind.Region:
                        region = view.Region;
                        break;
                    case ZoomboxViewKind.Center:
                        {
                            Rect rect = new Rect(_content.TranslatePoint(ContentRect.TopLeft, this), _content.TranslatePoint(ContentRect.BottomRight, this));
                            region = Rect.Inflate(rect, (base.RenderSize.Width / _viewboxFactor - rect.Width) / 2.0, (base.RenderSize.Height / _viewboxFactor - rect.Height) / 2.0);
                            region = new Rect(TranslatePoint(region.TopLeft, _content), TranslatePoint(region.BottomRight, _content));
                            break;
                        }
                    case ZoomboxViewKind.Fit:
                        region = ContentRect;
                        break;
                    case ZoomboxViewKind.Fill:
                        region = CalculateFillRect();
                        break;
                }
                if (view.ViewKind != ZoomboxViewKind.Empty)
                {
                    if (!region.IsEmpty)
                    {
                        CalculatePositionAndScale(region, ref newRelativePosition, ref newRelativeScale);
                    }
                    else if (view != ZoomboxView.Empty)
                    {
                        if (newRelativeScale > MaxScale)
                        {
                            newRelativeScale = MaxScale;
                        }
                        else if (newRelativeScale < MinScale)
                        {
                            newRelativeScale = MinScale;
                        }
                    }
                    double fromValue = _relativeScale;
                    double x = _relativePosition.X;
                    double y = _relativePosition.Y;
                    ScaleTransform scaleTransform = null;
                    TranslateTransform translateTransform = null;
                    TransformGroup transformGroup = null;
                    if (_contentPresenter.RenderTransform != Transform.Identity)
                    {
                        transformGroup = _contentPresenter.RenderTransform as TransformGroup;
                        scaleTransform = transformGroup.Children[0] as ScaleTransform;
                        translateTransform = transformGroup.Children[1] as TranslateTransform;
                        fromValue = scaleTransform.ScaleX;
                        x = translateTransform.X;
                        y = translateTransform.Y;
                    }
                    if (KeepContentInBounds)
                    {
                        Rect rect2 = new Rect(new Size(ContentRect.Width * newRelativeScale, ContentRect.Height * newRelativeScale));
                        Point location = new Point(0.0 - newRelativePosition.X, 0.0 - newRelativePosition.Y);
                        Rect rect3 = new Rect(location, _contentPresenter.RenderSize);
                        if (DoubleHelper.AreVirtuallyEqual(_relativeScale, newRelativeScale))
                        {
                            if (IsGreaterThanOrClose(rect2.Width, rect3.Width))
                            {
                                if (rect2.Right < rect3.Right)
                                {
                                    newRelativePosition.X = 0.0 - (rect2.Width - rect3.Width);
                                }
                                if (rect2.Left > rect3.Left)
                                {
                                    newRelativePosition.X = 0.0;
                                }
                            }
                            else if (IsGreaterThanOrClose(rect3.Width, rect2.Width))
                            {
                                if (rect3.Right < rect2.Right)
                                {
                                    newRelativePosition.X = rect3.Width - rect2.Width;
                                }
                                if (rect3.Left > rect2.Left)
                                {
                                    newRelativePosition.X = 0.0;
                                }
                            }
                            if (IsGreaterThanOrClose(rect2.Height, rect3.Height))
                            {
                                if (rect2.Bottom < rect3.Bottom)
                                {
                                    newRelativePosition.Y = 0.0 - (rect2.Height - rect3.Height);
                                }
                                if (rect2.Top > rect3.Top)
                                {
                                    newRelativePosition.Y = 0.0;
                                }
                            }
                            else if (IsGreaterThanOrClose(rect3.Height, rect2.Height))
                            {
                                if (rect3.Bottom < rect2.Bottom)
                                {
                                    newRelativePosition.Y = rect3.Height - rect2.Height;
                                }
                                if (rect3.Top > rect2.Top)
                                {
                                    newRelativePosition.Y = 0.0;
                                }
                            }
                        }
                    }
                    scaleTransform = new ScaleTransform(newRelativeScale / _viewboxFactor, newRelativeScale / _viewboxFactor);
                    translateTransform = new TranslateTransform(newRelativePosition.X, newRelativePosition.Y);
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(scaleTransform);
                    transformGroup.Children.Add(translateTransform);
                    _contentPresenter.RenderTransform = transformGroup;
                    Size size = ((_content is Viewbox) ? ((Viewbox)_content).Child.DesiredSize : base.RenderSize);
                    Size size2 = new Size(size.Width * newRelativeScale, size.Height * newRelativeScale);
                    if (allowAnimation && IsAnimated)
                    {
                        DoubleAnimation doubleAnimation = new DoubleAnimation(fromValue, newRelativeScale / _viewboxFactor, AnimationDuration);
                        doubleAnimation.AccelerationRatio = AnimationAccelerationRatio;
                        doubleAnimation.DecelerationRatio = AnimationDecelerationRatio;
                        DoubleAnimation doubleAnimation2 = new DoubleAnimation(x, newRelativePosition.X, AnimationDuration);
                        doubleAnimation2.AccelerationRatio = AnimationAccelerationRatio;
                        doubleAnimation2.DecelerationRatio = AnimationDecelerationRatio;
                        DoubleAnimation doubleAnimation3 = new DoubleAnimation(y, newRelativePosition.Y, AnimationDuration);
                        doubleAnimation3.AccelerationRatio = AnimationAccelerationRatio;
                        doubleAnimation3.DecelerationRatio = AnimationDecelerationRatio;
                        doubleAnimation3.CurrentTimeInvalidated += UpdateViewport;
                        doubleAnimation3.CurrentStateInvalidated += ZoomAnimationCompleted;
                        RaiseEvent(new RoutedEventArgs(AnimationBeginningEvent, this));
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
                        translateTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation2);
                        translateTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation3);
                        if (IsUsingScrollBars)
                        {
                            DoubleAnimation doubleAnimation4 = new DoubleAnimation();
                            doubleAnimation4.From = _verticalScrollBar.Maximum;
                            doubleAnimation4.To = size2.Height - _verticalScrollBar.ViewportSize;
                            doubleAnimation4.Duration = AnimationDuration;
                            _verticalScrollBar.BeginAnimation(RangeBase.MaximumProperty, doubleAnimation4);
                            DoubleAnimation doubleAnimation5 = new DoubleAnimation();
                            doubleAnimation5.From = _verticalScrollBar.Value;
                            doubleAnimation5.To = 0.0 - newRelativePosition.Y;
                            doubleAnimation5.Duration = AnimationDuration;
                            doubleAnimation5.Completed += VerticalValueAnimation_Completed;
                            _verticalScrollBar.BeginAnimation(RangeBase.ValueProperty, doubleAnimation5);
                            DoubleAnimation doubleAnimation6 = new DoubleAnimation();
                            doubleAnimation6.From = _horizontalScrollBar.Maximum;
                            doubleAnimation6.To = size2.Width - _horizontalScrollBar.ViewportSize;
                            doubleAnimation6.Duration = AnimationDuration;
                            _horizontalScrollBar.BeginAnimation(RangeBase.MaximumProperty, doubleAnimation6);
                            DoubleAnimation doubleAnimation7 = new DoubleAnimation();
                            doubleAnimation7.From = _horizontalScrollBar.Value;
                            doubleAnimation7.To = 0.0 - newRelativePosition.X;
                            doubleAnimation7.Duration = AnimationDuration;
                            doubleAnimation7.Completed += HorizontalValueAnimation_Completed;
                            _horizontalScrollBar.BeginAnimation(RangeBase.ValueProperty, doubleAnimation7);
                        }
                    }
                    else if (IsUsingScrollBars)
                    {
                        _verticalScrollBar.Maximum = size2.Height - _verticalScrollBar.ViewportSize;
                        _verticalScrollBar.Value = 0.0 - newRelativePosition.Y;
                        _horizontalScrollBar.Maximum = size2.Width - _horizontalScrollBar.ViewportSize;
                        _horizontalScrollBar.Value = 0.0 - newRelativePosition.X;
                    }
                    _relativePosition = newRelativePosition;
                    _relativeScale = newRelativeScale;
                    Scale = newRelativeScale;
                    _basePosition = newRelativePosition + ContentOffset * newRelativeScale;
                    UpdateViewport();
                }
                if (EffectiveViewStackMode == ZoomboxViewStackMode.Auto && allowStackAddition)
                {
                    if (ViewStack.Count > 1 && Math.Abs(DateTime.Now.Ticks - _lastStackAddition.Ticks) < TimeSpan.FromMilliseconds(300.0).Ticks)
                    {
                        ViewStack.RemoveAt(ViewStack.Count - 1);
                        _lastStackAddition = DateTime.Now - TimeSpan.FromMilliseconds(300.0);
                    }
                    if (ViewStack.Count <= 0 || !(view == ViewStack.SelectedView))
                    {
                        ViewStack.PushView(view);
                        ViewStackIndex++;
                        stackIndex = ViewStackIndex;
                        _lastStackAddition = DateTime.Now;
                    }
                }
                _lastViewIndex = CurrentViewIndex;
                SetCurrentViewIndex(stackIndex);
                SetCurrentView(view);
            }
            finally
            {
                IsUpdatingView = false;
            }
        }
    }

    private void UpdateViewport()
    {
        if (_contentPresenter == null || _trueContent == null)
        {
            return;
        }
        IsUpdatingViewport = true;
        try
        {
            Rect rect = new Rect(TranslatePoint(new Point(0.0, 0.0), _trueContent), TranslatePoint(new Point(base.RenderSize.Width, base.RenderSize.Height), _trueContent));
            if (!DoubleHelper.AreVirtuallyEqual(rect, Viewport))
            {
                SetValue(ViewportPropertyKey, rect);
            }
        }
        finally
        {
            IsUpdatingViewport = false;
        }
    }

    private Point GetZoomRelativePoint()
    {
        if (ZoomOn == ZoomboxZoomOn.View)
        {
            Point point = default(Point);
            point.X = Viewport.X + Viewport.Width * ZoomOrigin.X;
            point.Y = Viewport.Y + Viewport.Height * ZoomOrigin.Y;
            Point result = _trueContent.TranslatePoint(point, _content);
            if (result.X < 0.0)
            {
                result.X = 0.0;
            }
            else if (result.X > _content.RenderSize.Width)
            {
                result.X = _content.RenderSize.Width;
            }
            if (result.Y < 0.0)
            {
                result.Y = 0.0;
            }
            else if (result.Y > _content.RenderSize.Height)
            {
                result.Y = _content.RenderSize.Height;
            }
            return result;
        }
        Point result2 = new Point(_content.RenderSize.Width * ZoomOrigin.X, _content.RenderSize.Height * ZoomOrigin.Y);
        return result2;
    }

    public enum KeyModifier
    {
        None,
        Blocked,
        Ctrl,
        LeftCtrl,
        RightCtrl,
        Shift,
        LeftShift,
        RightShift,
        Alt,
        LeftAlt,
        RightAlt,
        Exact
    }

    [TypeConverter(typeof(KeyModifierCollectionConverter))]
    public class KeyModifierCollection : Collection<KeyModifier>
    {
        public bool AreActive
        {
            get
            {
                if (base.Count == 0)
                {
                    return true;
                }
                if (Contains(KeyModifier.Blocked))
                {
                    return false;
                }
                if (Contains(KeyModifier.Exact))
                {
                    return IsExactMatch();
                }
                return MatchAny();
            }
        }

        private static bool IsKeyPressed(KeyModifier modifier, ICollection<Key> keys)
        {
            switch (modifier)
            {
                case KeyModifier.Alt:
                    if (!keys.Contains(Key.LeftAlt))
                    {
                        return keys.Contains(Key.RightAlt);
                    }
                    return true;
                case KeyModifier.LeftAlt:
                    return keys.Contains(Key.LeftAlt);
                case KeyModifier.RightAlt:
                    return keys.Contains(Key.RightAlt);
                case KeyModifier.Ctrl:
                    if (!keys.Contains(Key.LeftCtrl))
                    {
                        return keys.Contains(Key.RightCtrl);
                    }
                    return true;
                case KeyModifier.LeftCtrl:
                    return keys.Contains(Key.LeftCtrl);
                case KeyModifier.RightCtrl:
                    return keys.Contains(Key.RightCtrl);
                case KeyModifier.Shift:
                    if (!keys.Contains(Key.LeftShift))
                    {
                        return keys.Contains(Key.RightShift);
                    }
                    return true;
                case KeyModifier.LeftShift:
                    return keys.Contains(Key.LeftShift);
                case KeyModifier.RightShift:
                    return keys.Contains(Key.RightShift);
                case KeyModifier.None:
                    return true;
                default:
                    throw new NotSupportedException("Unknown modifier");
            }
        }

        private static bool HasModifier(Key key, ICollection<KeyModifier> modifiers)
        {
            switch (key)
            {
                case Key.LeftAlt:
                    if (!modifiers.Contains(KeyModifier.Alt))
                    {
                        return modifiers.Contains(KeyModifier.LeftAlt);
                    }
                    return true;
                case Key.RightAlt:
                    if (!modifiers.Contains(KeyModifier.Alt))
                    {
                        return modifiers.Contains(KeyModifier.RightAlt);
                    }
                    return true;
                case Key.LeftCtrl:
                    if (!modifiers.Contains(KeyModifier.Ctrl))
                    {
                        return modifiers.Contains(KeyModifier.LeftCtrl);
                    }
                    return true;
                case Key.RightCtrl:
                    if (!modifiers.Contains(KeyModifier.Ctrl))
                    {
                        return modifiers.Contains(KeyModifier.RightCtrl);
                    }
                    return true;
                case Key.LeftShift:
                    if (!modifiers.Contains(KeyModifier.Shift))
                    {
                        return modifiers.Contains(KeyModifier.LeftShift);
                    }
                    return true;
                case Key.RightShift:
                    if (!modifiers.Contains(KeyModifier.Shift))
                    {
                        return modifiers.Contains(KeyModifier.RightShift);
                    }
                    return true;
                default:
                    throw new NotSupportedException("Unknown key");
            }
        }

        private bool IsExactMatch()
        {
            HashSet<KeyModifier> keyModifiers = GetKeyModifiers();
            HashSet<Key> keysPressed = GetKeysPressed();
            if (Contains(KeyModifier.None))
            {
                if (keyModifiers.Count == 0)
                {
                    return keysPressed.Count == 0;
                }
                return false;
            }
            foreach (KeyModifier item in keyModifiers)
            {
                if (!IsKeyPressed(item, keysPressed))
                {
                    return false;
                }
            }
            foreach (Key item2 in keysPressed)
            {
                if (!HasModifier(item2, keyModifiers))
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchAny()
        {
            if (Contains(KeyModifier.None))
            {
                return true;
            }
            HashSet<KeyModifier> keyModifiers = GetKeyModifiers();
            HashSet<Key> keysPressed = GetKeysPressed();
            foreach (KeyModifier item in keyModifiers)
            {
                if (IsKeyPressed(item, keysPressed))
                {
                    return true;
                }
            }
            return false;
        }

        private HashSet<KeyModifier> GetKeyModifiers()
        {
            HashSet<KeyModifier> hashSet = new HashSet<KeyModifier>();
            using IEnumerator<KeyModifier> enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyModifier current = enumerator.Current;
                if ((uint)(current - 2) <= 8u && !hashSet.Contains(current))
                {
                    hashSet.Add(current);
                }
            }
            return hashSet;
        }

        private HashSet<Key> GetKeysPressed()
        {
            HashSet<Key> hashSet = new HashSet<Key>();
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                hashSet.Add(Key.LeftAlt);
            }
            if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                hashSet.Add(Key.RightAlt);
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                hashSet.Add(Key.LeftCtrl);
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl))
            {
                hashSet.Add(Key.RightCtrl);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                hashSet.Add(Key.LeftShift);
            }
            if (Keyboard.IsKeyDown(Key.RightShift))
            {
                hashSet.Add(Key.RightShift);
            }
            return hashSet;
        }
    }
}