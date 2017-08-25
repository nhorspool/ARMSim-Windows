// *****************************************************************************
// 
//  (c) Crownwood Consulting Limited 2002-2003
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Crownwood Consulting 
//	Limited, Crownwood, Bracknell, Berkshire, England and are supplied subject 
//  to licence terms.
// 
//  Magic Version 1.7.4.0 	www.dotnetmagic.com
// *****************************************************************************

using System;
using System.Drawing;
using ARMSim.GUI.DockingWindows.Common;
using ARMSim.GUI.DockingWindows.Collections;

namespace ARMSim.GUI.DockingWindows.Docking
{
    public enum State
    {
        Floating,
        DockTop,
        DockBottom,
        DockLeft,
        DockRight
    }

    public interface IHotZoneSource
    {
        void AddHotZones(Redocker redock, HotZoneCollection collection);
    }

    public interface IZoneMaximizeWindow
    {
        Direction Direction { get; }
        bool IsMaximizeAvailable();
        bool IsWindowMaximized(Window w);
        void MaximizeWindow(Window w);
        void RestoreWindow();
        event EventHandler RefreshMaximize;
    }

    // Delegate signatures
    public delegate void ContextHandler(Point screenPos);
}
