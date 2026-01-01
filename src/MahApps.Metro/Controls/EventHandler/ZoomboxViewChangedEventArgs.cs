using System; 

namespace MahApps.Metro.Controls;

public class ZoomboxViewChangedEventArgs : PropertyChangedEventArgs<ZoomboxView>
{
	private readonly int _newViewStackIndex = -1;

	private readonly int _oldViewStackIndex = -1;

	public int NewViewStackIndex => _newViewStackIndex;

	public int OldViewStackIndex => _oldViewStackIndex;

	public bool IsNewViewFromStack => _newViewStackIndex >= 0;

	public bool IsOldViewFromStack => _oldViewStackIndex >= 0;

	public ZoomboxViewChangedEventArgs(ZoomboxView oldView, ZoomboxView newView, int oldViewStackIndex, int newViewStackIndex)
		: base(Zoombox.CurrentViewChangedEvent, oldView, newView)
	{
		_newViewStackIndex = newViewStackIndex;
		_oldViewStackIndex = oldViewStackIndex;
	}

	protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
	{
		((ZoomboxViewChangedEventHandler)genericHandler)(genericTarget, this);
	}
}
