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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

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

        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Backstage backstage = (Backstage)d;
            if ((bool)e.NewValue) backstage.Show();
            else backstage.Hide();

            // Invoke the event
            if (backstage.IsOpenChanged != null) backstage.IsOpenChanged(backstage, e);
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
            StyleProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
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
            this.Unloaded += this.Backstage_Unloaded;

            CoerceValue(HeaderProperty);
            CoerceValue(KeyTip.KeysProperty);

            AddHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)OnPopupDismiss);
        }

        private void OnPopupDismiss(object sender, DismissPopupEventArgs e)
        {
            IsOpen = false;
        }

        #endregion

        #region Methods

        // Handles click event
        void Click()
        {
            IsOpen = !IsOpen;
        }

        #region Show / Hide

        // We have to collapse WindowsFormsHost while Backstate is open
        Dictionary<FrameworkElement, Visibility> collapsedElements =
            new Dictionary<FrameworkElement, Visibility>();
        // Saved when backstage opened tab item
        RibbonTabItem savedTabItem;

        // Saved window sizes
        double savedMinWidth;
        double savedMinHeight;
        int savedWidth;
        int savedHeight;

        // Opens backstage on an Adorner layer
        void Show()
        {
            if (!IsLoaded)
            {
                Loaded += OnDelayedShow;
                return;
            }

            if (Content == null) return;

            AdornerLayer layer = GetAdornerLayer(this);
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

            Ribbon ribbon = FindRibbon();
            if (ribbon != null)
            {
                savedTabItem = ribbon.SelectedTabItem;
                if (savedTabItem == null && ribbon.Tabs.Count > 0)
                    savedTabItem = (RibbonTabItem)ribbon.Tabs[0];
                ribbon.SelectedTabItem = null;
                ribbon.SelectedTabChanged += OnSelectedRibbonTabChanged;

                // Disable QAT & title bar
                if (ribbon.QuickAccessToolBar != null) ribbon.QuickAccessToolBar.IsEnabled = false;
                if (ribbon.TitleBar != null) ribbon.TitleBar.IsEnabled = false;
            }

            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown += OnBackstageEscapeKeyDown;
                savedMinWidth = window.MinWidth;
                savedMinHeight = window.MinHeight;

                SaveWindowSize(window);

                if (savedMinWidth < 500) window.MinWidth = 500;
                if (savedMinHeight < 400) window.MinHeight = 400;
                window.SizeChanged += OnWindowSizeChanged;

                // We have to collapse WindowsFormsHost while Backstage is open
                CollapseWindowsFormsHosts(window);
            }

            IInputElement content = Content as IInputElement;
            if (content != null) content.Focus();
        }

        void OnDelayedShow(object sender, EventArgs args)
        {
            Loaded -= OnDelayedShow;
            Show();
        }

        // Hide backstage
        void Hide()
        {
            Loaded -= OnDelayedShow;
            if (Content == null) return;
            if (!IsLoaded || adorner == null) return;

            AdornerLayer layer = GetAdornerLayer(this);
            layer.Remove(adorner);
            Ribbon ribbon = FindRibbon();
            if (ribbon != null)
            {
                ribbon.SelectedTabChanged -= OnSelectedRibbonTabChanged;
                if (!ribbon.IsMinimized) ribbon.SelectedTabItem = savedTabItem;
                // Restore enable under QAT & title bar
                if (ribbon.QuickAccessToolBar != null) ribbon.QuickAccessToolBar.IsEnabled = true;
                if (ribbon.TitleBar != null) ribbon.TitleBar.IsEnabled = true;
            }

            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= OnBackstageEscapeKeyDown;
                window.SizeChanged -= OnWindowSizeChanged;

                window.MinWidth = savedMinWidth;
                window.MinHeight = savedMinHeight;
                NativeMethods.SetWindowPos((new WindowInteropHelper(window)).Handle,
                                           new IntPtr(NativeMethods.HWND_NOTOPMOST),
                                           0, 0, savedWidth, savedHeight, NativeMethods.SWP_NOMOVE);
            }

            // Uncollapse elements
            foreach (var element in collapsedElements) element.Key.Visibility = element.Value;
            collapsedElements.Clear();

            if (ribbon != null && !ribbon.IsMinimized) ribbon.SelectedTabItem = savedTabItem;

        }

        // Finds underlying ribbon control
        Ribbon FindRibbon()
        {
            DependencyObject item = this;
            while (item != null && !(item is Ribbon))
                item = VisualTreeHelper.GetParent(item);
            return (Ribbon)item;
        }

        void SaveWindowSize(Window wnd)
        {
            NativeMethods.WINDOWINFO info = new NativeMethods.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            NativeMethods.GetWindowInfo((new WindowInteropHelper(wnd)).Handle, ref info);
            savedWidth = info.rcWindow.Right - info.rcWindow.Left;
            savedHeight = info.rcWindow.Bottom - info.rcWindow.Top;
        }

        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window wnd = Window.GetWindow(this);
            SaveWindowSize(wnd);
        }


        void OnSelectedRibbonTabChanged(object sender, EventArgs e)
        {
            Ribbon ribbon = FindRibbon();
            if (ribbon != null) savedTabItem = ribbon.SelectedTabItem;
            IsOpen = false;
        }

        // We have to collapse WindowsFormsHost while Backstage is open
        void CollapseWindowsFormsHosts(DependencyObject parent)
        {
            if (parent == null) return;
            FrameworkElement frameworkElement = parent as FrameworkElement;

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
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
            }

        }

        // Handles backstage Esc key keydown
        void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) IsOpen = false;
        }

        private void Backstage_Unloaded(object sender, RoutedEventArgs e)
        {
            RemoveHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)OnPopupDismiss);
        }

        /// <summary>
        /// Get adorner layer for element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Adorner layer</returns>
        static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
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
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            Click();
        }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            Click();
            base.OnKeyTipPressed();
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
