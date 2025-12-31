using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using MahApps.Metro.Converters;
#nullable disable
namespace MahApps.Metro.Controls;

public sealed class ZoomboxViewStack : Collection<ZoomboxView>, IWeakEventListener
{
	private sealed class SourceAccess : IDisposable
	{
		private ZoomboxViewStack _viewStack;

		public SourceAccess(ZoomboxViewStack viewStack)
		{
			_viewStack = viewStack;
			_viewStack.IsChangeFromSource = true;
		}

		~SourceAccess()
		{
			Dispose();
		}

		public void Dispose()
		{
			_viewStack.IsChangeFromSource = false;
			_viewStack = null;
			GC.SuppressFinalize(this);
		}
	}

	private enum CacheBits
	{
		AreViewsFromSource = 1,
		IsChangeFromSource = 2,
		IsResettingViews = 4,
		IsMovingViews = 8,
		IsSettingInitialViewAfterClear = 0x10
	}

	private IEnumerable _source;

	private WeakReference _zoomboxRef;

	private BitVector32 _cacheBits = new BitVector32(0);

	public ZoomboxView SelectedView
	{
		get
		{
			int viewStackIndex = Zoombox.ViewStackIndex;
			if (viewStackIndex >= 0 && viewStackIndex <= base.Count - 1)
			{
				return base[viewStackIndex];
			}
			return ZoomboxView.Empty;
		}
	}

	internal bool AreViewsFromSource
	{
		get
		{
			return _cacheBits[1];
		}
		set
		{
			_cacheBits[1] = value;
		}
	}

	internal IEnumerable Source => _source;

	private bool IsChangeFromSource
	{
		get
		{
			return _cacheBits[2];
		}
		set
		{
			_cacheBits[2] = value;
		}
	}

	private bool IsMovingViews
	{
		get
		{
			return _cacheBits[8];
		}
		set
		{
			_cacheBits[8] = value;
		}
	}

	private bool IsResettingViews
	{
		get
		{
			return _cacheBits[4];
		}
		set
		{
			_cacheBits[4] = value;
		}
	}

	private bool IsSettingInitialViewAfterClear
	{
		get
		{
			return _cacheBits[16];
		}
		set
		{
			_cacheBits[16] = value;
		}
	}

	private ZoomBox Zoombox => _zoomboxRef.Target as ZoomBox;

	public ZoomboxViewStack(ZoomBox zoombox)
	{
		_zoomboxRef = new WeakReference(zoombox);
	}

	internal void ClearViewStackSource()
	{
		if (AreViewsFromSource)
		{
			AreViewsFromSource = false;
			MonitorSource(monitor: false);
			_source = null;
			using (new SourceAccess(this))
			{
				Clear();
			}
			Zoombox.CoerceValue(ZoomBox.ViewStackModeProperty);
		}
	}

	internal void PushView(ZoomboxView view)
	{
		int viewStackIndex = Zoombox.ViewStackIndex;
		while (base.Count - 1 > viewStackIndex)
		{
			RemoveAt(base.Count - 1);
		}
		Add(view);
	}

	internal void SetViewStackSource(IEnumerable source)
	{
		if (_source != source)
		{
			MonitorSource(monitor: false);
			_source = source;
			MonitorSource(monitor: true);
			AreViewsFromSource = true;
			Zoombox.CoerceValue(ZoomBox.ViewStackModeProperty);
			ResetViews();
		}
	}

	protected override void ClearItems()
	{
		VerifyStackModification();
		bool flag = Zoombox.CurrentViewIndex >= 0;
		base.ClearItems();
		Zoombox.SetViewStackCount(base.Count);
		if (IsResettingViews)
		{
			return;
		}
		if (Zoombox.EffectiveViewStackMode == ZoomboxViewStackMode.Auto && Zoombox.CurrentView != ZoomboxView.Empty)
		{
			IsSettingInitialViewAfterClear = true;
			try
			{
				Add(Zoombox.CurrentView);
			}
			finally
			{
				IsSettingInitialViewAfterClear = false;
			}
			Zoombox.ViewStackIndex = 0;
			if (flag)
			{
				Zoombox.SetCurrentViewIndex(0);
			}
		}
		else
		{
			Zoombox.ViewStackIndex = -1;
			Zoombox.SetCurrentViewIndex(-1);
		}
	}

	protected override void InsertItem(int index, ZoomboxView view)
	{
		VerifyStackModification();
		if (Zoombox.HasArrangedContentPresenter && Zoombox.ViewStackIndex >= index && !IsSettingInitialViewAfterClear && !IsResettingViews && !IsMovingViews)
		{
			bool isUpdatingView = Zoombox.IsUpdatingView;
			Zoombox.IsUpdatingView = true;
			try
			{
				Zoombox.ViewStackIndex++;
				if (Zoombox.CurrentViewIndex != -1)
				{
					Zoombox.SetCurrentViewIndex(Zoombox.CurrentViewIndex + 1);
				}
			}
			finally
			{
				Zoombox.IsUpdatingView = isUpdatingView;
			}
		}
		base.InsertItem(index, view);
		Zoombox.SetViewStackCount(base.Count);
	}

	protected override void RemoveItem(int index)
	{
		VerifyStackModification();
		bool flag = Zoombox.ViewStackIndex == index;
		if (!IsMovingViews && Zoombox.HasArrangedContentPresenter && (Zoombox.ViewStackIndex > index || (flag && Zoombox.ViewStackIndex == Zoombox.ViewStack.Count - 1)))
		{
			if (flag && Zoombox.ViewStack.Count == 1)
			{
				Clear();
				return;
			}
			bool isUpdatingView = Zoombox.IsUpdatingView;
			Zoombox.IsUpdatingView = true;
			try
			{
				Zoombox.ViewStackIndex--;
				if (Zoombox.CurrentViewIndex != -1)
				{
					Zoombox.SetCurrentViewIndex(Zoombox.CurrentViewIndex - 1);
				}
			}
			finally
			{
				Zoombox.IsUpdatingView = isUpdatingView;
			}
		}
		base.RemoveItem(index);
		if (!IsMovingViews && flag && Zoombox.CurrentViewIndex != -1)
		{
			Zoombox.RefocusView();
		}
		Zoombox.SetViewStackCount(base.Count);
	}

	protected override void SetItem(int index, ZoomboxView view)
	{
		VerifyStackModification();
		base.SetItem(index, view);
		if (index == Zoombox.CurrentViewIndex)
		{
			Zoombox.RefocusView();
		}
	}

	private static ZoomboxView GetViewFromSourceItem(object item)
	{
		ZoomboxView obj = ((item is ZoomboxView) ? (item as ZoomboxView) : (ZoomboxViewConverter.Converter.ConvertFrom(item) as ZoomboxView));
		if (obj == null)
		{
			throw new InvalidCastException(string.Format("UnableToConvertToZoomboxView", item));
		}
		return obj;
	}

	private void InsertViews(int index, IList newItems)
	{
		using (new SourceAccess(this))
		{
			foreach (object newItem in newItems)
			{
				ZoomboxView viewFromSourceItem = GetViewFromSourceItem(newItem);
				if (index >= base.Count)
				{
					Add(viewFromSourceItem);
				}
				else
				{
					Insert(index, viewFromSourceItem);
				}
				index++;
			}
		}
	}

	private void MonitorSource(bool monitor)
	{
		if (_source != null && _source is INotifyCollectionChanged)
		{
			if (monitor)
			{
				CollectionChangedEventManager.AddListener(_source as INotifyCollectionChanged, this);
			}
			else
			{
				CollectionChangedEventManager.RemoveListener(_source as INotifyCollectionChanged, this);
			}
		}
	}

	private void MoveViews(int oldIndex, int newIndex, IList movedItems)
	{
		using (new SourceAccess(this))
		{
			int viewStackIndex = Zoombox.ViewStackIndex;
			int num = viewStackIndex;
			if ((oldIndex >= viewStackIndex || newIndex >= viewStackIndex) && (oldIndex <= viewStackIndex || newIndex <= viewStackIndex))
			{
				if (viewStackIndex >= oldIndex && viewStackIndex < oldIndex + movedItems.Count)
				{
					num += newIndex - oldIndex;
				}
				else if (viewStackIndex >= newIndex)
				{
					num += movedItems.Count;
				}
			}
			IsMovingViews = true;
			try
			{
				for (int i = 0; i < movedItems.Count; i++)
				{
					RemoveAt(oldIndex);
				}
				for (int j = 0; j < movedItems.Count; j++)
				{
					Insert(newIndex + j, GetViewFromSourceItem(movedItems[j]));
				}
				if (num != viewStackIndex)
				{
					Zoombox.ViewStackIndex = num;
					Zoombox.SetCurrentViewIndex(num);
				}
			}
			finally
			{
				IsMovingViews = false;
			}
		}
	}

	private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			InsertViews(e.NewStartingIndex, e.NewItems);
			break;
		case NotifyCollectionChangedAction.Move:
			MoveViews(e.OldStartingIndex, e.NewStartingIndex, e.OldItems);
			break;
		case NotifyCollectionChangedAction.Remove:
			RemoveViews(e.OldStartingIndex, e.OldItems);
			break;
		case NotifyCollectionChangedAction.Replace:
			ResetViews();
			break;
		case NotifyCollectionChangedAction.Reset:
			ResetViews();
			break;
		}
	}

	private void ResetViews()
	{
		using (new SourceAccess(this))
		{
			int viewStackIndex = Zoombox.ViewStackIndex;
			IsResettingViews = true;
			try
			{
				Clear();
				foreach (object item in _source)
				{
					ZoomboxView viewFromSourceItem = GetViewFromSourceItem(item);
					Add(viewFromSourceItem);
				}
				viewStackIndex = Math.Min(Math.Max(0, viewStackIndex), base.Count - 1);
				Zoombox.ViewStackIndex = viewStackIndex;
				Zoombox.SetCurrentViewIndex(viewStackIndex);
				Zoombox.RefocusView();
			}
			finally
			{
				IsResettingViews = false;
			}
		}
	}

	private void RemoveViews(int index, IList removedItems)
	{
		using (new SourceAccess(this))
		{
			for (int i = 0; i < removedItems.Count; i++)
			{
				RemoveAt(index);
			}
		}
	}

	private void VerifyStackModification()
	{
		if (AreViewsFromSource && !IsChangeFromSource)
		{
			throw new InvalidOperationException("ViewStackCannotBeManipulatedNow");
		}
	}

	public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(CollectionChangedEventManager))
		{
			OnSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
			return true;
		}
		return false;
	}
}
