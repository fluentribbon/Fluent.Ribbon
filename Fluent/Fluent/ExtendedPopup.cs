using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace Fluent
{
    public class ExtendedPopup:Popup
    {
        private bool canClose = false;

        public bool IngnoreFirstClose { get; set; }

        #region Constructors

        static ExtendedPopup()
        {
            //IsOpenProperty.AddOwner(typeof(ExtendedPopup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged, OnIsCoerceCallback));
        }

        private static object OnIsCoerceCallback(DependencyObject d, object basevalue)
        {
            //return !((d as ExtendedPopup).canClose)&&((bool)basevalue);
            if((bool)basevalue == false)
            {
                return !((d as ExtendedPopup).canClose);
            }
            return true;
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /*if((bool)e.NewValue==false)
            {
                Window.GetWindow(d).Activate();
            }*/
        }

        #endregion

        #region Overrides

        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            //IsOpen = false;
        }

        protected override void OnOpened(EventArgs e)
        {            
            PopupAnimation = PopupAnimation.None;
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            ((HwndSource)PresentationSource.FromVisual(this.Child)).AddHook(WindowProc);
            //int styleEx = GetWindowLong(hwnd, GWL_EXSTYLE);
            //SetWindowLong(hwnd, GWL_EXSTYLE, styleEx | WS_EX_TOPMOST | WS_EX_NOACTIVATE | WS_EX_NOPARENTNOTIFY | WS_EX_PALETTEWINDOW);
            SetActiveWindow(hwnd);
            //Keyboard.AddPreviewLostKeyboardFocusHandler(this,OnFocusChanged);
            //Keyboard.AddGotKeyboardFocusHandler(this,OnFocusChanged);
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            PopupAnimation = PopupAnimation.None;
        }

        #endregion

        #region Private methods

        // Функиця окна
        private IntPtr WindowProc(
                   IntPtr hwnd,
                   int msg,
                   IntPtr wParam,
                   IntPtr lParam,
                   ref bool handled)
        {
            switch (msg)
            {
                case 0x0006:
                    {
                        if (((short)wParam.ToInt32()) == 0)
                        {
                            if (lParam != hwnd)
                            {
                                //IsOpen = false;
                                //canClose = true;
                                if (!IngnoreFirstClose) PopupAnimation = PopupAnimation.Fade;
                                else IngnoreFirstClose = false;
                                IsOpen = false;
                                //handled = true;
                            }
                        }
                        else
                        {

                            IntPtr parentHwnd = (new WindowInteropHelper(Window.GetWindow(this))).Handle;
                            SendMessage(parentHwnd, 0x0086, 1, IntPtr.Zero);
                            handled = true;

                        }
                        break;
                    }
                case 0x0021:
                    {
                        IntPtr parentHwnd = (IntPtr)wParam;
                        SendMessage(parentHwnd, 0x0086, 1, IntPtr.Zero);
                        handled = true;
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Послать сообщение
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd, // handle to destination window 
            int Msg, // message 
            int wParam, // first message parameter 
            IntPtr lParam // second message parameter 
        );

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("User32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int GWL_STYLE = (-16);
        public const int GWL_EXSTYLE = (-20);

        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const int WS_EX_LAYERED = 0x00080000;
        // Disable inheritence of mirroring by children
        public const int WS_EX_NOINHERITLAYOUT = 0x00100000;
        // Right to left mirroring
        public const int WS_EX_LAYOUTRTL = 0x00400000;
        public const int WS_EX_COMPOSITED = 0x02000000;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        #endregion
    }
}
