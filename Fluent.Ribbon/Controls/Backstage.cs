// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
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

        private BackstageAdorner adorner;

        #region Properties

        /// <summary>
        /// Gets the <see cref="AdornerLayer"/> for the <see cref="Backstage"/>.
        /// </summary>
        /// <remarks>This is exposed to make it possible to show content on the same <see cref="AdornerLayer"/> as the backstage is shown on.</remarks>
        public AdornerLayer AdornerLayer { get; private set; }

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
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged, CoerceIsOpen));

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
        /// Gets or sets whether context tabs on the titlebar should be hidden when backstage is open
        /// </summary>
        public bool HideContextTabsOnOpen
        {
            get { return (bool)this.GetValue(HideContextTabsOnOpenProperty); }
            set { this.SetValue(HideContextTabsOnOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideContextTabsOnOpen.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideContextTabsOnOpenProperty =
            DependencyProperty.Register(nameof(HideContextTabsOnOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether opening or closing should be animated.
        /// </summary>
        public bool AreAnimationsEnabled
        {
            get { return (bool)this.GetValue(AreAnimationsEnabledProperty); }
            set { this.SetValue(AreAnimationsEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpenAnimationEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AreAnimationsEnabledProperty =
            DependencyProperty.Register(nameof(AreAnimationsEnabled), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

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

        private static object CoerceIsOpen(DependencyObject d, object baseValue)
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
                    backstage.Hide();
                }

                // Invoke the event
                backstage.IsOpenChanged?.Invoke(backstage, e);
            }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="UseHighestAvailableAdornerLayer"/>.
        /// </summary>
        public static readonly DependencyProperty UseHighestAvailableAdornerLayerProperty = DependencyProperty.Register(nameof(UseHighestAvailableAdornerLayer), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the highest available adorner layer should be used for the <see cref="BackstageAdorner"/>.
        /// This means that we will try to look up the visual tree till we find the highest <see cref="AdornerDecorator"/>.
        /// </summary>
        public bool UseHighestAvailableAdornerLayer
        {
            get { return (bool)this.GetValue(UseHighestAvailableAdornerLayerProperty); }
            set { this.SetValue(UseHighestAvailableAdornerLayerProperty, value); }
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
                if (e.NewValue is DependencyObject dependencyObject)
                {
                    BindingOperations.ClearBinding(dependencyObject, VisibilityProperty);
                }

                backstage.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                backstage.AddLogicalChild(e.NewValue);

                if (e.NewValue is DependencyObject dependencyObject)
                {
                    BindingOperations.SetBinding(dependencyObject, VisibilityProperty, new Binding { Path = new PropertyPath(VisibilityProperty), Source = backstage });
                }
            }
        }

        #endregion

        #region LogicalChildren

        /// <inheritdoc />
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
            this.DataContextChanged += this.Handle_DataContextChanged;
        }

        private void Handle_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.adorner != null)
            {
                this.adorner.DataContext = e.NewValue;
            }
        }

        /// <summary>
        /// Called when this control receives the <see cref="PopupService.DismissPopupEvent"/>.
        /// </summary>
        protected virtual void OnDismissPopup(object sender, DismissPopupEventArgs e)
        {
            // Don't close on dismiss popup event if application lost focus
            // or keytips should be shown.
            if (e.DismissReason == DismissPopupReason.ApplicationLostFocus
                || e.DismissReason == DismissPopupReason.ShowingKeyTips)
            {
                return;
            }

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
        private Window ownerWindow;
        private Ribbon parentRibbon;

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

            this.parentRibbon = GetParentRibbon(this);
            if (this.parentRibbon != null)
            {
                if (this.parentRibbon.TabControl != null)
                {
                    this.parentRibbon.TabControl.IsDropDownOpen = false;
                    this.parentRibbon.TabControl.HighlightSelectedItem = false;
                    this.parentRibbon.TabControl.RequestBackstageClose += this.HandleTabControlRequestBackstageClose;
                }

                if (this.parentRibbon.QuickAccessToolBar != null)
                {
                    this.parentRibbon.QuickAccessToolBar.IsEnabled = false;
                }

                if (this.parentRibbon.TitleBar != null)
                {
                    this.parentRibbon.TitleBar.HideContextTabs = this.HideContextTabsOnOpen;
                }
            }

            this.ownerWindow = Window.GetWindow(this);

            if (this.ownerWindow == null
                && this.Parent != null)
            {
                this.ownerWindow = Window.GetWindow(this.Parent);
            }

            this.SaveWindowSize(this.ownerWindow);
            this.SaveWindowMinSize(this.ownerWindow);

            if (this.ownerWindow != null)
            {
                this.ownerWindow.KeyDown += this.HandleOwnerWindowKeyDown;

                if (this.savedWindowMinWidth < 500)
                {
                    this.ownerWindow.MinWidth = 500;
                }

                if (this.savedWindowMinHeight < 400)
                {
                    this.ownerWindow.MinHeight = 400;
                }

                this.ownerWindow.SizeChanged += this.HandleOwnerWindowSizeChanged;

                // We have to collapse WindowsFormsHost while Backstage is open
                this.CollapseWindowsFormsHosts(this.ownerWindow);
            }

            var content = this.Content as IInputElement;
            content?.Focus();

            return true;
        }

        /// <summary>
        /// Hides the <see cref="Backstage"/>
        /// </summary>
        protected virtual void Hide()
        {
            // potentially fixes https://github.com/fluentribbon/Fluent.Ribbon/issues/489
            if (this.Dispatcher.HasShutdownStarted
                || this.Dispatcher.HasShutdownFinished)
            {
                return;
            }

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

            this.HideAdornerAndRestoreParentProperties();
        }

        private void ShowAdorner()
        {
            if (this.adorner == null)
            {
                return;
            }

            if (this.AreAnimationsEnabled
                && this.TryFindResource("Fluent.Ribbon.Storyboards.Backstage.IsOpenTrueStoryboard") is Storyboard storyboard)
            {
                storyboard = storyboard.Clone();

                storyboard.CurrentStateInvalidated += HanldeStoryboardCurrentStateInvalidated;
                storyboard.Completed += HandleStoryboardOnCompleted;

                storyboard.Begin(this.adorner);
            }
            else
            {
                this.adorner.Visibility = Visibility.Visible;
            }

            void HanldeStoryboardCurrentStateInvalidated(object sender, EventArgs e)
            {
                this.adorner.Visibility = Visibility.Visible;
                storyboard.CurrentStateInvalidated -= HanldeStoryboardCurrentStateInvalidated;
            }

            void HandleStoryboardOnCompleted(object sender, EventArgs args)
            {
                this.AdornerLayer?.Update();

                storyboard.Completed -= HandleStoryboardOnCompleted;
            }
        }

        private void HideAdornerAndRestoreParentProperties()
        {
            if (this.adorner == null)
            {
                return;
            }

            if (this.AreAnimationsEnabled
                && this.TryFindResource("Fluent.Ribbon.Storyboards.Backstage.IsOpenFalseStoryboard") is Storyboard storyboard)
            {
                if (this.AdornerLayer != null)
                {
                    this.AdornerLayer.Visibility = Visibility.Collapsed;
                }

                storyboard = storyboard.Clone();

                storyboard.Completed += HandleStoryboardOnCompleted;

                storyboard.Begin(this.adorner);
            }
            else
            {
                this.adorner.Visibility = Visibility.Collapsed;
                this.RestoreParentProperties();
            }

            void HandleStoryboardOnCompleted(object sender, EventArgs args)
            {
                if (this.adorner != null)
                {
                    this.adorner.Visibility = Visibility.Collapsed;
                }

                if (this.AdornerLayer != null)
                {
                    this.AdornerLayer.Visibility = Visibility.Visible;
                }

                this.RestoreParentProperties();

                storyboard.Completed -= HandleStoryboardOnCompleted;
            }
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

            FrameworkElement elementToAdorn = UIHelper.GetParent<AdornerDecorator>(this)
                                              ?? UIHelper.GetParent<AdornerDecorator>(this.Parent);

            if (elementToAdorn == null)
            {
                return;
            }

            if (this.UseHighestAvailableAdornerLayer)
            {
                AdornerDecorator currentAdornerDecorator;
                while ((currentAdornerDecorator = UIHelper.GetParent<AdornerDecorator>(elementToAdorn)) != null)
                {
                    elementToAdorn = currentAdornerDecorator;
                }
            }

            this.AdornerLayer = UIHelper.GetAdornerLayer(elementToAdorn);

            if (this.AdornerLayer == null)
            {
                throw new Exception($"AdornerLayer could not be found for {this}.");
            }

            this.adorner = new BackstageAdorner(elementToAdorn, this);

            BindingOperations.SetBinding(this.adorner, DataContextProperty, new Binding
                                                                            {
                                                                                Path = new PropertyPath(DataContextProperty),
                                                                                Source = this
                                                                            });

            this.AdornerLayer.Add(this.adorner);

            this.AdornerLayer.CommandBindings.Add(new CommandBinding(RibbonCommands.OpenBackstage, HandleOpenBackstageCommandExecuted, HandleOpenBackstageCommandCanExecute));
        }

        private void DestroyAdorner()
        {
            this.AdornerLayer?.CommandBindings.Clear();
            this.AdornerLayer?.Remove(this.adorner);

            if (this.adorner != null)
            {
                BindingOperations.ClearAllBindings(this.adorner);
            }

            this.adorner?.Clear();
            this.adorner = null;

            this.AdornerLayer = null;
        }

        private void RestoreParentProperties()
        {
            if (this.parentRibbon != null)
            {
                if (this.parentRibbon.TabControl != null)
                {
                    this.parentRibbon.TabControl.HighlightSelectedItem = true;
                    this.parentRibbon.TabControl.RequestBackstageClose -= this.HandleTabControlRequestBackstageClose;
                }

                if (this.parentRibbon.QuickAccessToolBar != null)
                {
                    this.parentRibbon.QuickAccessToolBar.IsEnabled = true;
                    this.parentRibbon.QuickAccessToolBar.Refresh();
                }

                if (this.parentRibbon.TitleBar != null)
                {
                    this.parentRibbon.TitleBar.HideContextTabs = false;
                }

                this.parentRibbon = null;
            }

            if (this.ownerWindow != null)
            {
                this.ownerWindow.PreviewKeyDown -= this.HandleOwnerWindowKeyDown;
                this.ownerWindow.SizeChanged -= this.HandleOwnerWindowSizeChanged;

                if (double.IsNaN(this.savedWindowMinWidth) == false
                    && double.IsNaN(this.savedWindowMinHeight) == false)
                {
                    this.ownerWindow.MinWidth = this.savedWindowMinWidth;
                    this.ownerWindow.MinHeight = this.savedWindowMinHeight;
                }

                if (double.IsNaN(this.savedWindowWidth) == false
                    && double.IsNaN(this.savedWindowHeight) == false)
                {
                    this.ownerWindow.Width = this.savedWindowWidth;
                    this.ownerWindow.Height = this.savedWindowHeight;
                }

                this.savedWindowMinWidth = double.NaN;
                this.savedWindowMinHeight = double.NaN;

                this.savedWindowWidth = double.NaN;
                this.savedWindowHeight = double.NaN;
                
                this.ownerWindow = null;
            }

            // Uncollapse elements
            foreach (var element in this.collapsedElements)
            {
                element.Key.Visibility = element.Value;
            }

            this.collapsedElements.Clear();
        }

        private void OnDelayedShow(object sender, EventArgs args)
        {
            this.Loaded -= this.OnDelayedShow;

            // Delaying show so everthing can load properly.
            // If we don't run this in the background setting IsOpen=true on application start we don't have access to the Bastage from the BackstageTabControl.
            this.RunInDispatcherAsync(() => this.Show(), DispatcherPriority.Background);
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

        private void HandleOwnerWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SaveWindowSize(Window.GetWindow(this));
        }

        private void HandleTabControlRequestBackstageClose(object sender, EventArgs e)
        {
            this.IsOpen = false;
        }

        // We have to collapse WindowsFormsHost while Backstage is open
        private void CollapseWindowsFormsHosts(DependencyObject parent)
        {
            switch (parent)
            {
                case null:
                case BackstageAdorner _:
                    return;

                case FrameworkElement frameworkElement when parent is HwndHost
                                                            && frameworkElement.Visibility != Visibility.Collapsed:
                    this.collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                    frameworkElement.Visibility = Visibility.Collapsed;
                    return;
            }

            // Traverse visual tree
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                this.CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
            }
        }

        /// <inheritdoc />
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
        private void HandleOwnerWindowKeyDown(object sender, KeyEventArgs e)
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
            this.AddHandler(PopupService.DismissPopupEvent, (EventHandler<DismissPopupEventArgs>)this.OnDismissPopup);
        }

        private void OnBackstageUnloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveHandler(PopupService.DismissPopupEvent, (EventHandler<DismissPopupEventArgs>)this.OnDismissPopup);

            this.DestroyAdorner();
        }

        #endregion

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (ReferenceEquals(e.Source, this) == false)
            {
                return;
            }

            this.Click();
        }

        /// <inheritdoc />
        public override KeyTipPressedResult OnKeyTipPressed()
        {
            this.IsOpen = true;
            base.OnKeyTipPressed();

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public override void OnKeyTipBack()
        {
            this.IsOpen = false;
            base.OnKeyTipBack();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override FrameworkElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}