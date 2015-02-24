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
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Fluent.Extensions;

    /// <summary>
    /// Represents backstage button
    /// </summary>
    [ContentProperty("Content")]
    public class Backstage : RibbonControl
    {
        private static readonly object syncIsOpen = new object();

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

        /// <summary>
        /// Gets or sets the duration for the hide animation
        /// </summary>
        public Duration HideAnimationDuration
        {
            get { return (Duration)GetValue(HideAnimationDurationProperty); }
            set { SetValue(HideAnimationDurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideAnimationDuration.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideAnimationDurationProperty = DependencyProperty.Register("HideAnimationDuration", typeof(Duration), typeof(Backstage), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether context tabs on the titlebar should be hidden when backstage is open
        /// </summary>
        public bool HideContextTabsOnOpen
        {
            get { return (bool)GetValue(HideContextTabsOnOpenProperty); }
            set { SetValue(HideContextTabsOnOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideContextTabsOnOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideContextTabsOnOpenProperty = DependencyProperty.Register("HideContextTabsOnOpen", typeof(bool), typeof(Backstage), new PropertyMetadata(false));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var backstage = (Backstage)d;

            lock (syncIsOpen)
            {
                if ((bool)e.NewValue)
                {
                    backstage.Show();
                }
                else
                {
                    if (backstage.HideAnimationDuration.HasTimeSpan)
                    {
                        var timespan = backstage.HideAnimationDuration.TimeSpan;

                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(timespan);

                            backstage.Dispatcher.RunInDispatcher(backstage.Hide);
                        });
                    }
                    else
                    {
                        backstage.Hide();
                    }
                }

                // Invoke the event
                if (backstage.IsOpenChanged != null)
                {
                    backstage.IsOpenChanged(backstage, e);
                }
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
            var backstage = (Backstage)d;
            if (e.OldValue != null)
            {
                backstage.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                backstage.AddLogicalChild(e.NewValue);
            }
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
                if (this.Content != null)
                {
                    yield return this.Content;
                }
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

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Backstage()
        {
            this.Loaded += this.OnBackstageLoaded;
            this.Unloaded += this.OnBackstageUnloaded;

            this.CommandBindings.Add(new CommandBinding(RibbonCommands.OpenBackstage, (sender, args) => this.IsOpen = !this.IsOpen));
        }

        private void OnPopupDismiss(object sender, DismissPopupEventArgs e)
        {
            this.IsOpen = false;
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
        private readonly Dictionary<FrameworkElement, Visibility> collapsedElements = new Dictionary<FrameworkElement, Visibility>();

        // Saved window sizes
        private double savedWindowMinWidth = double.NaN;
        private double savedWindowMinHeight = double.NaN;
        private double savedWindowWidth = double.NaN;
        private double savedWindowHeight = double.NaN;

        // Opens backstage on an Adorner layer
        private void Show()
        {
            // don't open the backstage while in design mode
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            if (this.IsLoaded == false)
            {
                this.Loaded += this.OnDelayedShow;
                return;
            }

            if (this.Content == null)
            {
                return;
            }

            this.CreateAndAttachBackstageAdorner();

            this.ShowAdorner();

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
                    ribbon.TitleBar.HideContextTabs = this.HideContextTabsOnOpen;
                }
            }

            var window = Window.GetWindow(this);

            this.SaveWindowSize(window);
            this.SaveWindowMinSize(window);

            if (window != null)
            {
                window.KeyDown += this.HandleWindowKeyDown;


                if (this.savedWindowMinWidth < 500)
                {
                    window.MinWidth = 500;
                }

                if (this.savedWindowMinHeight < 400)
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

        private void ShowAdorner()
        {
            if (this.adorner == null)
            {
                return;
            }

            this.adorner.Visibility = Visibility.Visible;
        }

        private void HideAdorner()
        {
            if (this.adorner == null)
            {
                return;
            }

            this.adorner.Visibility = Visibility.Collapsed;
        }

        private void CreateAndAttachBackstageAdorner()
        {
            if (this.adorner != null)
            {
                return;
            }

            FrameworkElement topLevelElement;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // TODO: in design mode it is required to use design time adorner
                topLevelElement = (FrameworkElement)VisualTreeHelper.GetParent(this);
            }
            else
            {
                var mainWindow = Window.GetWindow(this);
                if (mainWindow == null)
                {
                    return;
                }

                topLevelElement = (FrameworkElement)mainWindow.Content;
            }

            if (topLevelElement == null)
            {
                return;
            }

            this.adorner = new BackstageAdorner(topLevelElement, this);

            var layer = AdornerLayer.GetAdornerLayer(this);
            layer.Add(this.adorner);
        }

        private void DestroyAdorner()
        {
            if (this.adorner == null)
            {
                return;
            }

            var layer = AdornerLayer.GetAdornerLayer(this);
            layer.Remove(this.adorner);

            this.adorner.Clear();
            this.adorner = null;
        }

        private void OnDelayedShow(object sender, EventArgs args)
        {
            this.Loaded -= this.OnDelayedShow;

            // Delaying show so everthing can load properly.
            // If we don't run this in the background setting IsOpen=true on application start we don't have access to the Bastage from the BackstageTabControl.
            this.RunInDispatcherAsync(this.Show, DispatcherPriority.Background);
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

            this.HideAdorner();

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
                    ribbon.TitleBar.HideContextTabs = false;
                }
            }

            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= this.HandleWindowKeyDown;
                window.SizeChanged -= this.OnWindowSizeChanged;

                if (double.IsNaN(this.savedWindowMinWidth) == false
                    && double.IsNaN(this.savedWindowMinHeight) == false)
                {
                    window.MinWidth = this.savedWindowMinWidth;
                    window.MinHeight = this.savedWindowMinHeight;
                }

                if (double.IsNaN(this.savedWindowWidth) == false
                    && double.IsNaN(this.savedWindowHeight) == false)
                {
                    window.Width = this.savedWindowWidth;
                    window.Height = this.savedWindowHeight;
                }
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

        private void SaveWindowMinSize(Window window)
        {
            if (window == null)
            {
                this.savedWindowMinWidth = double.NaN;
                this.savedWindowMinHeight = double.NaN;
                return;
            }

            this.savedWindowMinWidth = window.MinWidth;
            this.savedWindowMinHeight = window.MinHeight;
        }

        private void SaveWindowSize(Window window)
        {
            if (window == null)
            {
                this.savedWindowWidth = double.NaN;
                this.savedWindowHeight = double.NaN;
                return;
            }

            this.savedWindowWidth = window.ActualWidth;
            this.savedWindowHeight = window.ActualHeight;
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SaveWindowSize(Window.GetWindow(this));
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

            // Do not hide contents in the backstage area
            if (parent is BackstageAdorner) return;

            if (frameworkElement != null)
            {
                if ((parent is HwndHost) &&
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

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                case Key.Space:
                    if (this.IsFocused)
                    {
                        this.IsOpen = !this.IsOpen;
                        e.Handled = true;
                    }
                    break;
            }

            base.OnKeyDown(e);
        }

        // Handles backstage Esc key keydown
        private void HandleWindowKeyDown(object sender, KeyEventArgs e)
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

            if (this.IsOpen)
            {
                this.Hide();
            }

            this.DestroyAdorner();

            if (this.IsOpen)
            {
                this.Show();
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