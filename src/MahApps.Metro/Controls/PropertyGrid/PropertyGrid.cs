using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Extensions;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls;

[TemplatePart(Name = ElementItemsControl, Type = typeof(ItemsControl))]
[TemplatePart(Name = ElementSearchBar, Type = typeof(SearchBar))]
public class PropertyGrid : Control
{
    private const string ElementItemsControl = "PART_ItemsControl";

    private const string ElementSearchBar = "PART_Search";

    private ItemsControl? _itemsControl;

    private ICollectionView? _dataView;

    private TextBox? _searchBar;

    private string? _searchKey;

    public PropertyGrid()
    {
        CommandBindings.Add(new CommandBinding(PropertyGrid.SortByCategory, ExecSortByCategory, (s, e) => e.CanExecute = ShowSortButton));
        CommandBindings.Add(new CommandBinding(PropertyGrid.SortByName, ExecSortByName, (s, e) => e.CanExecute = ShowSortButton));
    }

    /// <summary>
    ///     按照类别排序
    /// </summary>
    public static RoutedCommand? SortByCategory { get; } = new(nameof(SortByCategory), typeof(PropertyGrid));

    /// <summary>
    ///     按照名称排序
    /// </summary>
    public static RoutedCommand? SortByName { get; } = new(nameof(SortByName), typeof(PropertyGrid));


    public virtual PropertyResolver PropertyResolver { get; } = new();

    public static readonly RoutedEvent SelectedObjectChangedEvent =
        EventManager.RegisterRoutedEvent(nameof(SelectedObjectChanged), RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));

    public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
    {
        add => AddHandler(SelectedObjectChangedEvent, value);
        remove => RemoveHandler(SelectedObjectChangedEvent, value);
    }

    public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
        nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(default, OnSelectedObjectChanged));

    private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PropertyGrid)d;
        ctl.OnSelectedObjectChanged(e.OldValue, e.NewValue);
    }

    public object SelectedObject
    {
        get => GetValue(SelectedObjectProperty);
        set => SetValue(SelectedObjectProperty, value);
    }

    protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
    {
        UpdateItems(newValue);
        RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectedObjectChangedEvent));
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(PropertyGrid), new PropertyMetadata(default(string)));

    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty MaxTitleWidthProperty = DependencyProperty.Register(
        nameof(MaxTitleWidth), typeof(double), typeof(PropertyGrid), new PropertyMetadata(UsedBoxes.Double0Box));

    public double MaxTitleWidth
    {
        get => (double)GetValue(MaxTitleWidthProperty);
        set => SetValue(MaxTitleWidthProperty, value);
    }

    public static readonly DependencyProperty MinTitleWidthProperty = DependencyProperty.Register(
        nameof(MinTitleWidth), typeof(double), typeof(PropertyGrid), new PropertyMetadata(UsedBoxes.Double0Box));

    public double MinTitleWidth
    {
        get => (double)GetValue(MinTitleWidthProperty);
        set => SetValue(MinTitleWidthProperty, value);
    }

    public static readonly DependencyProperty ShowSortButtonProperty = DependencyProperty.Register(
        nameof(ShowSortButton), typeof(bool), typeof(PropertyGrid), new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowSortButton
    {
        get => (bool)GetValue(ShowSortButtonProperty);
        set => SetValue(ShowSortButtonProperty, BooleanBoxes.Box(value));
    }

    public override void OnApplyTemplate()
    {
        if (_searchBar != null)
        {
            _searchBar.TextChanged -= Search_SearchStarted;
        }

        base.OnApplyTemplate();

        _itemsControl = GetTemplateChild(ElementItemsControl) as ItemsControl;
        _searchBar = GetTemplateChild(ElementSearchBar) as TextBox;

        if (_searchBar != null)
        {
            _searchBar.TextChanged += Search_SearchStarted;
        }

        UpdateItems(SelectedObject);
    }

    private void UpdateItems(object obj)
    {
        if (obj == null || _itemsControl == null) return;

        _dataView = CollectionViewSource.GetDefaultView(TypeDescriptor.GetProperties(obj.GetType()).OfType<PropertyDescriptor>()
            .Where(item => PropertyResolver.ResolveIsBrowsable(item)).Select(CreatePropertyItem)
            .Do(item => item.InitElement()));

        ExecSortByCategory(null, null);
        _itemsControl.ItemsSource = _dataView;
    }

    private void ExecSortByCategory(object? sender, ExecutedRoutedEventArgs? e)
    {
        if (_dataView == null) return;

        using (_dataView.DeferRefresh())
        {
            _dataView.GroupDescriptions.Clear();
            _dataView.SortDescriptions.Clear();
            _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.CategoryProperty.Name, ListSortDirection.Ascending));
            _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.DisplayNameProperty.Name, ListSortDirection.Ascending));
            _dataView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyItem.CategoryProperty.Name));
        }
    }

    private void ExecSortByName(object sender, ExecutedRoutedEventArgs e)
    {
        if (_dataView == null) return;

        using (_dataView.DeferRefresh())
        {
            _dataView.GroupDescriptions.Clear();
            _dataView.SortDescriptions.Clear();
            _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.PropertyNameProperty.Name, ListSortDirection.Ascending));
        }
    }

    private void Search_SearchStarted(object? sender, TextChangedEventArgs e)
    {
        if (_dataView == null) return;
        _searchKey = (sender as TextBox)?.Text?.Trim();
        if (string.IsNullOrEmpty(_searchKey))
        {
            foreach (UIElement item in _dataView)
            {
                item.Show();
            }
        }
        else
        {
            foreach (PropertyItem item in _dataView)
            {
                item.Show(item.PropertyName.ToLower().Contains(_searchKey) || item.DisplayName.ToLower().Contains(_searchKey));
            }
        }
    }

    protected virtual PropertyItem CreatePropertyItem(PropertyDescriptor propertyDescriptor)
    {
        return new()
        {
            Category = PropertyResolver.ResolveCategory(propertyDescriptor),
            DisplayName = PropertyResolver.ResolveDisplayName(propertyDescriptor),
            Description = PropertyResolver.ResolveDescription(propertyDescriptor),
            IsReadOnly = PropertyResolver.ResolveIsReadOnly(propertyDescriptor),
            DefaultValue = PropertyResolver.ResolveDefaultValue(propertyDescriptor),
            Editor = PropertyResolver.ResolveEditor(propertyDescriptor),
            Value = SelectedObject,
            PropertyName = propertyDescriptor.Name,
            PropertyType = propertyDescriptor.PropertyType,
            PropertyTypeName = $"{propertyDescriptor.PropertyType.Namespace}.{propertyDescriptor.PropertyType.Name}"
        };
    }
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        //TitleElement.SetTitleWidth(this, new GridLength(Math.Max(MinTitleWidth, Math.Min(MaxTitleWidth, ActualWidth / 3))));
    }
}
