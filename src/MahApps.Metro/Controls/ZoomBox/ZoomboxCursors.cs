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
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Input;

namespace MahApps.Metro.Controls;

public class ZoomBoxCursors
{
	private static readonly Cursor _zoom;

	private static readonly Cursor _zoomRelative;

	public static Cursor Zoom => _zoom;

	public static Cursor ZoomRelative => _zoomRelative;

	static ZoomBoxCursors()
	{
		_zoom = Cursors.Arrow;
		_zoomRelative = Cursors.Arrow;
		try
		{
			//new EnvironmentPermission(PermissionState.Unrestricted).Demand();
			//_zoom = new Cursor(ResourceHelper.LoadResourceStream(Assembly.GetExecutingAssembly(), "Zoombox/Resources/Zoom.cur"));
			//_zoomRelative = new Cursor(ResourceHelper.LoadResourceStream(Assembly.GetExecutingAssembly(), "Zoombox/Resources/ZoomRelative.cur"));
		}
		catch (SecurityException)
		{
		}
	}
}
