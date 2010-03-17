#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace Fluent
{
    /// <summary>
    /// Represents popup. This popup has Microsoft Office behavior
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    public class RibbonPopup : Popup
    {
        #region Static Methods

        // Currently opened popups
        static readonly List<RibbonPopup> openedPopups = new List<RibbonPopup>();

        /// <summary>
        /// Returns active popup
        /// </summary>
        /// <returns>Active popup or null if no popup is opened</returns>
        internal static RibbonPopup GetActivePopup()
        {
            if(openedPopups.Count==0) return null;
            return openedPopups[openedPopups.Count - 1];
        }

        /// <summary>
        /// Closes all currently opened popups
        /// </summary>
        public static void CloseAll()
        {
            ClosePopups(0);
        }

        // TODO: maybe rename CollapseCurrent -> CloseCurrent

        /// <summary>
        /// Closes the current popup
        /// </summary>
        public static void CollapseCurrent()
        {
            if (openedPopups.Count > 0) ClosePopups(openedPopups.Count - 1);
        }

        #endregion

        #region Fields

        // Current HwndSource of this Popup
        HwndSource hwndSource;
        // TODO: comment this field (isFirstMouseUp)
        bool isFirstMouseUp;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonPopup()
        {
            Focusable = false;
            ToolTip = new ToolTip();
            (ToolTip as ToolTip).Template = null;
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Responds to the condition in which the value of the Popup.IsOpen property 
        /// changes from false to true.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            
            isFirstMouseUp = true;

            PopupAnimation = PopupAnimation.None;

            hwndSource = (HwndSource)PresentationSource.FromVisual(this.Child);
            if (hwndSource != null) hwndSource.AddHook(WindowProc);

            openedPopups.Add(this);

            Activate();                                    
        }

        /// <summary>
        /// Provides class handling for the PreviewMouseLeftButtonDown event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            isFirstMouseUp = false;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.PreviewMouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
                e.Handled = true;
            }
            if (isFirstMouseUp)
            {
                e.Handled = true;
                isFirstMouseUp = false;
            }
        }

        /// <summary>
        /// Responds when the value of the Popup.IsOpen property changes from to true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (openedPopups.Contains(this)) openedPopups.Remove(this);
            PopupAnimation = PopupAnimation.None;

            base.OnClosed(e);
        }


        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyDown attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;                                
                IsOpen = false;
                return;
            }
            if ((e.Key==Key.System)&&((e.SystemKey == Key.LeftAlt)||(e.SystemKey == Key.RightAlt)||(e.SystemKey == Key.F10)))
            {
                if (e.SystemKey != Key.F10)
                {
                    ClosePopups(0);
                }
                else e.Handled = true;
                return;
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Private methods

        private static void ClosePopups(int index)
        {
            for (int i = openedPopups.Count - 1; i >= index; i--)
            {
                openedPopups[i].PopupAnimation = PopupAnimation.Fade;
                openedPopups[i].IsOpen = false;

                if (openedPopups.Contains(openedPopups[i])) openedPopups.Remove(openedPopups[i]);
            }
        }


        internal void Activate()
        {
            if (hwndSource != null)
            {
                //NativeMethods.SetActiveWindow(hwndSource.Handle);
                if (hwndSource.Handle!=IntPtr.Zero) NativeMethods.SetActiveWindow(hwndSource.Handle);
            }
        }

        /// <summary>
        /// Window function
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WindowProc(
                   IntPtr hwnd,
                   int msg,
                   IntPtr wParam,
                   IntPtr lParam,
                   ref bool handled)
        {            
            switch (msg)
            {
                case 0x0006/*WM_ACTIVATE*/:
                    {
                        if (((short)wParam.ToInt32()) == 0)
                        {
                            // BUG: Fix for MessageBox: without parent messagebox closes
                            if (NativeMethods.GetWindowLongPtr(lParam, NativeMethods.GWL_HWNDPARENT) == hwndSource.Handle)
                                break;                                
                            if(openedPopups.Count(x => x.hwndSource.Handle == lParam) == 0)
                            {                                
                                ClosePopups(0);
                            }
                        }
                        else
                        {
                            int index = openedPopups.IndexOf(this);
                            ClosePopups(index + 1);
                            Window wnd = Window.GetWindow(this);
                            if (wnd != null)
                            {
                                IntPtr parentHwnd = (new WindowInteropHelper(wnd)).Handle;
                                NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);
                            }
                        }
                        break;
                    }
                case 0x0021/*WM_MOUSEACTIVATE*/:
                    {
                        int index = openedPopups.IndexOf(this);
                        ClosePopups(index + 1);
                        Window wnd = Window.GetWindow(this);
                        if (wnd != null)
                        {
                            IntPtr parentHwnd = (new WindowInteropHelper(wnd)).Handle;
                            NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);
                        }
                        break;
                    }
                case 0x0002/*WM_DESTROY*/:
                    {
                        /*if ((hwndSource != null) && (!hwndSource.IsDisposed))
                        {
                            // Remove hook
                            hwndSource.RemoveHook(WindowProc);
                            hwndSource = null;
                        }*/
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        

        #endregion
    }
}
