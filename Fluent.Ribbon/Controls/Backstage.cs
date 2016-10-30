// ReSharper disable once CheckNamespace
namespace Fluent
{
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
    using System.Windows.Threading;
    using System.Threading;
    using System.Threading.Tasks;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents backstage button
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class Backstage : RibbonControl
    {
        private static readonly object syncIsOpen = new object();

        /// <summary>
        /// Occurs when IsOpen has been changed
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsOpenChanged;

        // Adorner for backstage
        private BackstageAdorner adorner;

        #region Properties

        /// <summary>
        /// Gets or sets whether backstage is shown
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)this.GetValue(IsOpenProperty); }
            set { this.SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged, OnCoerceIsOpen));

        /// <summary>
        /// Gets or sets whether backstage can be openend or closed.
        /// </summary>
        public bool CanChangeIsOpen
        {
            get { return (bool)this.GetValue(CanChangeIsOpenProperty); }
            set { this.SetValue(CanChangeIsOpenProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="CanChangeIsOpen"/>
        /// </summary>
        public static readonly DependencyProperty CanChangeIsOpenProperty =
            DependencyProperty.Register(nameof(CanChangeIsOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets the duration for the hide animation
        /// </summary>
        public Duration HideAnimationDuration
        {
            get { return (Duration)this.GetValue(HideAnimationDurationProperty); }
            set { this.SetValue(HideAnimationDurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideAnimationDuration.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideAnimationDurationProperty =
            DependencyProperty.Register(nameof(HideAnimationDuration), typeof(Duration), typeof(Backstage), new PropertyMetadata());

        /// <summary>
        /// Gets or sets whether context tabs on the titlebar should be hidden when backstage is open
        /// </summary>
        public bool HideContextTabsOnOpen
        {
            get { return (bool)this.GetValue(HideContextTabsOnOpenProperty); }
            set { this.SetValue(HideContextTabsOnOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets wether opening or closing should be animated.
        /// </summary>
        public bool IsOpenAnimationEnabled
        {
            get { return (bool)this.GetValue(IsOpenAnimationEnabledProperty); }
            set { this.SetValue(IsOpenAnimationEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpenAnimationEnabled.  This enables animation, styling, binding, etc...
        /// </summary>        
        public static readonly DependencyProperty IsOpenAnimationEnabledProperty =
            DependencyProperty.Register(nameof(IsOpenAnimationEnabled), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideContextTabsOnOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideContextTabsOnOpenProperty =
            DependencyProperty.Register(nameof(HideContextTabsOnOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether to close the backstage when Esc is pressed
        /// </summary>
        public bool CloseOnEsc
        {
            get { return (bool)this.GetValue(CloseOnEscProperty); }
            set { this.SetValue(CloseOnEscProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CloseOnEsc.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseOnEscProperty =
            DependencyProperty.Register(nameof(CloseOnEsc), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

        private static object OnCoerceIsOpen(DependencyObject d, object baseValue)
        {
            var backstage = (Backstage)d;

            if (backstage.CanChangeIsOpen == false)
            {
                return backstage.IsOpen;
            }

            return baseValue;
        }

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
                backstage.IsOpenChanged?.Invoke(backstage, e);
            }
        }

        #region Content

        /// <summary>
        /// Gets or sets content of the backstage
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(Backstage), new PropertyMetadata(OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Backstage()
        {
            this.Loaded += this.OnBackstageLoaded;
            this.Unloaded += this.OnBackstageUnloaded;
        }

        private void OnPopupDismiss(object sender, DismissPopupEventArgs e)
        {
            // Only close backstage when popups should always be closed.
            // "Always" applies to controls marked with IsDefinitive for example.
            if (e.DismissMode != DismissPopupMode.Always)
            {
                return;
            }

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

        /// <summary>
        /// Shows the <see cref="Backstage"/>
        /// </summary>
        protected virtual bool Show()
        {
            // don't open the backstage while in design mode
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return false;
            }

            if (this.IsLoaded == false)
            {
                this.Loaded += this.OnDelayedShow;
                return false;
            }

            if (this.Content == null)
            {
                return false;
            }

            this.CreateAndAttachBackstageAdorner();

            this.ShowAdorner();

            var parentRibbon = GetParentRibbon(this);
            if (parentRibbon != null)
            {
                if (parentRibbon.TabControl != null)
                {
                    parentRibbon.TabControl.IsDropDownOpen = false;
                    parentRibbon.TabControl.HighlightSelectedItem = false;
                    parentRibbon.TabControl.RequestBackstageClose += this.OnTabControlRequestBackstageClose;
                }

                // Disable QAT & title bar
                if (parentRibbon.QuickAccessToolBar != null)
                {
                    parentRibbon.QuickAccessToolBar.IsEnabled = false;
                }

                if (parentRibbon.TitleBar != null)
                {
                    parentRibbon.TitleBar.IsEnabled = false;
                    parentRibbon.TitleBar.HideContextTabs = this.HideContextTabsOnOpen;
                }
            }

            var window = Window.GetWindow(this);

            if (window == null
                && this.Parent != null)
            {
                window = Window.GetWindow(this.Parent);
            }

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
            content?.Focus();

            return true;
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
            // It's possible that we created an adorner but it's parent AdornerLayer got destroyed.
            // If that's the case we have to destroy our adorner.
            // This fixes #228 Backstage disappears when changing DontUseDwm
            if (this.adorner?.Parent == null)
            {
                this.DestroyAdorner();
            }

            if (this.adorner != null)
            {
                return;
            }

            FrameworkElement elementToAdorn;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // TODO: in design mode it is required to use design time adorner
                elementToAdorn = (FrameworkElement)VisualTreeHelper.GetParent(this);
            }
            else
            {
                elementToAdorn = UIHelper.GetParent<AdornerDecorator>(this) 
                    ?? UIHelper.GetParent<AdornerDecorator>(LogicalTreeHelper.GetParent(this));
            }

            if (elementToAdorn == null)
            {
                return;
            }

            var layer = UIHelper.GetAdornerLayer(elementToAdorn);

            if (layer == null)
            {
                throw new Exception($"AdornerLayer could not be found for {this}.");
            }

            this.adorner = new BackstageAdorner(elementToAdorn, this);
            layer.Add(this.adorner);

            layer.CommandBindings.Add(new CommandBinding(RibbonCommands.OpenBackstage, HandleOpenBackstageCommandExecuted, HandleOpenBackstageCommandCanExecute));
        }

        private static void HandleOpenBackstageCommandCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var target = ((BackstageAdorner)args.Source).Backstage;
            args.CanExecute = target.CanChangeIsOpen;
        }

        private static void HandleOpenBackstageCommandExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            var target = ((BackstageAdorner)args.Source).Backstage;
            target.IsOpen = !target.IsOpen;
        }

        private void DestroyAdorner()
        {
            if (this.adorner == null)
            {
                return;
            }

            var layer = AdornerLayer.GetAdornerLayer(this.adorner);
            layer?.CommandBindings.Clear();
            layer?.Remove(this.adorner);

            this.adorner.Clear();
            this.adorner = null;
        }

        private void OnDelayedShow(object sender, EventArgs args)
        {
            this.Loaded -= this.OnDelayedShow;

            // Delaying show so everthing can load properly.
            // If we don't run this in the background setting IsOpen=true on application start we don't have access to the Bastage from the BackstageTabControl.
            this.RunInDispatcherAsync(() => this.Show(), DispatcherPriority.Background);
        }

        /// <summary>
        /// Hides the <see cref="Backstage"/>
        /// </summary>
        protected virtual void Hide()
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

            var parentRibbon = GetParentRibbon(this);
            if (parentRibbon != null)
            {
                if (parentRibbon.TabControl != null)
                {
                    parentRibbon.TabControl.HighlightSelectedItem = true;
                    parentRibbon.TabControl.RequestBackstageClose -= this.OnTabControlRequestBackstageClose;
                }

                // Restore enable under QAT & title bar
                if (parentRibbon.QuickAccessToolBar != null)
                {
                    parentRibbon.QuickAccessToolBar.IsEnabled = true;
                    parentRibbon.QuickAccessToolBar.Refresh();
                }

                if (parentRibbon.TitleBar != null)
                {
                    parentRibbon.TitleBar.IsEnabled = true;
                    parentRibbon.TitleBar.HideContextTabs = false;
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
            if (window == null
                || window.WindowState == WindowState.Maximized)
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
            if (parent is BackstageAdorner)
            {
                return;
            }

            if (frameworkElement != null)
            {
                if (parent is HwndHost
                    && frameworkElement.Visibility != Visibility.Collapsed)
                {
                    this.collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                    frameworkElement.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            // Traverse visual tree
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                this.CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
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

            if (e.Key == Key.Enter
                || e.Key == Key.Space)
            {
                if (this.IsFocused)
                {
                    this.IsOpen = !this.IsOpen;
                    e.Handled = true;
                }
            }

            base.OnKeyDown(e);
        }

        // Handles backstage Esc key keydown
        private void HandleWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (this.CloseOnEsc == false
                || e.Key != Key.Escape)
            {
                return;
            }

            // only handle ESC when the backstage is open
            e.Handled = this.IsOpen;

            this.IsOpen = false;
        }

        private void OnBackstageLoaded(object sender, RoutedEventArgs e)
        {
            this.AddHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);
        }

        private void OnBackstageUnloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);

            this.DestroyAdorner();
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
            base.OnMouseLeftButtonDown(e);

            if (ReferenceEquals(e.Source, this) == false)
            {
                return;
            }

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