#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents backstage button
    /// </summary>
    [ContentProperty("Content")]
    public class Backstage : RibbonControl
    {
        #region Events

        /// <summary>
        /// Occurs when IsOpen has been changed
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsOpenChanged;

        #endregion

        #region Fields

        // Adorner for backstage
        BackstageAdorner adorner;

        #endregion

        #region Properties

        #region IsOpen

        /// <summary>
        /// Gets or sets whether backstage is shown
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool),
            typeof(Backstage), new UIPropertyMetadata(false, OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var backstage = (Backstage)d;

            if ((bool)e.NewValue)
            {
                backstage.Show();
            }
            else
            {
                backstage.Hide();
            }

            // Invoke the event
            if (backstage.IsOpenChanged != null)
            {
                backstage.IsOpenChanged(backstage, e);
            }
        }

        #endregion

        #region Content

        /// <summary>
        /// Gets or sets content of the backstage
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(Backstage),
            new UIPropertyMetadata(null, OnContentChanged));

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Backstage backstage = (Backstage)d;
            if (e.OldValue != null) backstage.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null) backstage.AddLogicalChild(e.NewValue);
        }

        #endregion

        #region LogicalChildren

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (Content != null) list.Add(Content);
                return list.GetEnumerator();
            }
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Backstage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(typeof(Backstage)));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(false));
            // Make default header
            HeaderProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(null, null, CoerceHeader));
            KeyTip.KeysProperty.AddOwner(typeof(Backstage), new FrameworkPropertyMetadata(null, null, CoerceKeyTipKeys));
            StyleProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(null, OnCoerceStyle));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(Backstage));
            }

            return basevalue;
        }

        static object CoerceHeader(DependencyObject d, object basevalue)
        {
            return basevalue ?? Ribbon.Localization.BackstageButtonText;
        }

        static object CoerceKeyTipKeys(DependencyObject d, object basevalue)
        {
            return basevalue ?? Ribbon.Localization.BackstageButtonKeyTip;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Backstage()
        {
            this.Loaded += this.OnBackstageLoaded;
            this.Unloaded += this.OnBackstageUnloaded;

            CoerceValue(HeaderProperty);
            CoerceValue(KeyTip.KeysProperty);

            this.CommandBindings.Add(new CommandBinding(RibbonCommands.OpenBackstage, (sender, args) => this.IsOpen = !this.IsOpen));
        }

        private void OnPopupDismiss(object sender, DismissPopupEventArgs e)
        {
            IsOpen = false;
        }

        #endregion

        #region Methods

        // Handles click event
        private void Click()
        {
            this.IsOpen = !this.IsOpen;
        }

        #region Show / Hide

        // We have to collapse WindowsFormsHost while Backstate is open
        readonly Dictionary<FrameworkElement, Visibility> collapsedElements = new Dictionary<FrameworkElement, Visibility>();

        // Saved window sizes
        double savedMinWidth;
        double savedMinHeight;
        int savedWidth;
        int savedHeight;

        // Opens backstage on an Adorner layer
        private void Show()
        {
            if (!this.IsLoaded)
            {
                this.Loaded += this.OnDelayedShow;
                return;
            }

            if (this.Content == null)
            {
                return;
            }

            var layer = GetAdornerLayer(this);
            if (adorner == null)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    // TODO: in design mode it is required to use design time adorner
                    var topLevelElement = (FrameworkElement)VisualTreeHelper.GetParent(this);
                    var topOffset = this.TranslatePoint(new Point(0, this.ActualHeight), topLevelElement).Y;
                    adorner = new BackstageAdorner(topLevelElement, Content, topOffset);
                }
                else
                {
                    var mainWindow = Window.GetWindow(this);
                    if (mainWindow == null)
                    {
                        return;
                    }

                    var topLevelElement = (FrameworkElement)mainWindow.Content;
                    if (topLevelElement == null)
                    {
                        return;
                    }

                    var topOffset = this.TranslatePoint(new Point(0, this.ActualHeight), topLevelElement).Y;
                    adorner = new BackstageAdorner(topLevelElement, this.Content, topOffset);
                }
            }

            layer.Add(adorner);

            var ribbon = this.FindRibbon();
            if (ribbon != null)
            {
                ribbon.TabControl.IsDropDownOpen = false;
                ribbon.TabControl.HighlightSelectedItem = false;
                ribbon.TabControl.RequestBackstageClose += this.OnTabControlRequestBackstageClose;

                // Disable QAT & title bar
                if (ribbon.QuickAccessToolBar != null)
                {
                    ribbon.QuickAccessToolBar.IsEnabled = false;
                }

                if (ribbon.TitleBar != null)
                {
                    ribbon.TitleBar.IsEnabled = false;
                }
            }

            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown += this.OnBackstageEscapeKeyDown;
                savedMinWidth = window.MinWidth;
                savedMinHeight = window.MinHeight;

                this.SaveWindowSize(window);

                if (savedMinWidth < 500)
                {
                    window.MinWidth = 500;
                }

                if (savedMinHeight < 400)
                {
                    window.MinHeight = 400;
                }

                window.SizeChanged += this.OnWindowSizeChanged;

                // We have to collapse WindowsFormsHost while Backstage is open
                this.CollapseWindowsFormsHosts(window);
            }

            var content = this.Content as IInputElement;
            if (content != null)
            {
                content.Focus();
            }
        }

        private void OnDelayedShow(object sender, EventArgs args)
        {
            this.Loaded -= this.OnDelayedShow;
            this.Show();
        }

        // Hide backstage
        private void Hide()
        {
            this.Loaded -= this.OnDelayedShow;
            if (this.Content == null)
            {
                return;
            }

            if (!this.IsLoaded
                || this.adorner == null)
            {
                return;
            }

            var layer = GetAdornerLayer(this);
            layer.Remove(adorner);

            var ribbon = this.FindRibbon();
            if (ribbon != null)
            {
                ribbon.TabControl.HighlightSelectedItem = true;
                ribbon.TabControl.RequestBackstageClose -= this.OnTabControlRequestBackstageClose;

                // Restore enable under QAT & title bar
                if (ribbon.QuickAccessToolBar != null)
                {
                    ribbon.QuickAccessToolBar.IsEnabled = true;
                }

                if (ribbon.TitleBar != null)
                {
                    ribbon.TitleBar.IsEnabled = true;
                }
            }

            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= this.OnBackstageEscapeKeyDown;
                window.SizeChanged -= this.OnWindowSizeChanged;

                window.MinWidth = this.savedMinWidth;
                window.MinHeight = this.savedMinHeight;
                NativeMethods.SetWindowPos((new WindowInteropHelper(window)).Handle,
                                           new IntPtr(NativeMethods.HWND_NOTOPMOST),
                                           0, 0, savedWidth, savedHeight, NativeMethods.SWP_NOMOVE);
            }

            // Uncollapse elements
            foreach (var element in this.collapsedElements)
            {
                element.Key.Visibility = element.Value;
            }

            this.collapsedElements.Clear();
        }

        // Finds underlying ribbon control
        private Ribbon FindRibbon()
        {
            DependencyObject item = this;

            while (item != null
                && !(item is Ribbon))
            {
                item = VisualTreeHelper.GetParent(item);
            }

            return (Ribbon)item;
        }

        private void SaveWindowSize(Window wnd)
        {
            var info = new NativeMethods.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            NativeMethods.GetWindowInfo((new WindowInteropHelper(wnd)).Handle, ref info);
            savedWidth = info.rcWindow.Right - info.rcWindow.Left;
            savedHeight = info.rcWindow.Bottom - info.rcWindow.Top;
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var wnd = Window.GetWindow(this);
            SaveWindowSize(wnd);
        }

        private void OnTabControlRequestBackstageClose(object sender, EventArgs e)
        {
            this.IsOpen = false;
        }

        // We have to collapse WindowsFormsHost while Backstage is open
        private void CollapseWindowsFormsHosts(DependencyObject parent)
        {
            if (parent == null)
            {
                return;
            }

            var frameworkElement = parent as FrameworkElement;

            if (frameworkElement != null)
            {
                if ((parent is WindowsFormsHost || parent is WebBrowser) &&
                    frameworkElement.Visibility != Visibility.Collapsed)
                {
                    collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                    frameworkElement.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            // Traverse visual tree
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
            }
        }

        // Handles backstage Esc key keydown
        private void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // only handle ESC when the backstage is open
                e.Handled = this.IsOpen;

                this.IsOpen = false;
            }
        }

        private void OnBackstageLoaded(object sender, RoutedEventArgs e)
        {
            this.AddHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);
        }

        private void OnBackstageUnloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);
        }

        /// <summary>
        /// Get adorner layer for element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Adorner layer</returns>
        private static AdornerLayer GetAdornerLayer(UIElement element)
        {
            var current = element;

            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator)
                {
                    return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
                }
            }
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data.
        ///  The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Click();
        }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            this.IsOpen = true;
            base.OnKeyTipPressed();
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public override void OnKeyTipBack()
        {
            this.IsOpen = false;
            base.OnKeyTipBack();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // OnAplyTemplate is executed then theme is changed            
            if (adorner != null)
            {
                /*bool isOpened = IsOpen;
                if (isOpened)
                {
                    Hide();
                    IsOpen = false;
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)(() => {                
                adorner.Clear();
                adorner = null;
                if (isOpened)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() => { IsOpen = true; }));    
                }
                }));    */
                if (IsOpen)
                {
                    Hide();
                    IsOpen = false;
                    // Clear adorner
                    adorner.Clear();
                    adorner = null;
                    /*// Remove adorner
                    AdornerLayer layer = GetAdornerLayer(this);
                    layer.Remove(adorner);
                    // Clear adorner
                    adorner.Clear();
                    adorner = null;
                    // Create new adorner
                    Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)(() =>
                    {
                        if (adorner == null)
                        {
                            if (DesignerProperties.GetIsInDesignMode(this))
                            {
                                // TODO: in design mode it is required to use design time adorner
                                FrameworkElement topLevelElement = (FrameworkElement)VisualTreeHelper.GetParent(this);
                                double topOffset = this.TranslatePoint(new Point(0, this.ActualHeight), topLevelElement).Y;
                                adorner = new BackstageAdorner(topLevelElement, Content, topOffset);
                            }
                            else
                            {
                                Window mainWindow = Window.GetWindow(this);
                                if (mainWindow == null) return;
                                FrameworkElement topLevelElement = (FrameworkElement)mainWindow.Content;
                                if (topLevelElement == null) return;
                                double topOffset = this.TranslatePoint(new Point(0, this.ActualHeight), topLevelElement).Y;
                                adorner = new BackstageAdorner(topLevelElement, this.Content, topOffset);
                            }
                        }
                        layer.Add(adorner);
                    }));*/
                }
                else
                {
                    // Clear adorner
                    adorner.Clear();
                    adorner = null;
                }
            }
        }

        #endregion

        #region Quick Access Toolbar

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}