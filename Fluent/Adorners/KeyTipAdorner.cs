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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    using System.Diagnostics;

    /// <summary>
    /// Represents adorner for KeyTips. 
    /// KeyTipAdorners is chained to produce one from another. 
    /// Detaching root adorner couses detaching all adorners in the chain
    /// </summary>
    internal class KeyTipAdorner : Adorner
    {
        #region Events

        /// <summary>
        /// This event is occured when adorner is 
        /// detached and is not able to be attached again
        /// </summary>
        public event EventHandler Terminated;

        #endregion

        #region Fields

        // KeyTips that have been
        // found on this element
        private readonly List<KeyTip> keyTips = new List<KeyTip>();
        private readonly List<UIElement> associatedElements = new List<UIElement>();
        private readonly FrameworkElement oneOfAssociatedElements;
        private readonly Point[] keyTipPositions;

        // Parent adorner
        private readonly KeyTipAdorner parentAdorner;
        KeyTipAdorner childAdorner;

        // Focused element
        private IInputElement focusedElement;

        private readonly Visibility[] backupedVisibilities;
        private readonly UIElement keyTipElementContainer;

        // Is this adorner attached to the adorned element?
        private bool attached;
        private HwndSource attachedHwndSource;

        // Current entered chars
        string enteredKeys = "";

        // Designate that this adorner is terminated
        private bool terminated;

        private DispatcherTimer timerFocusTracking;

        private AdornerLayer adornerLayer;

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether at least one on the adorners in the chain is alive
        /// </summary>
        public bool IsAdornerChainAlive
        {
            get { return attached || (childAdorner != null && childAdorner.IsAdornerChainAlive); }
        }

        public bool AreAnyKeyTipsVisible
        {
            get { return this.keyTips.Any(x => x.IsVisible) || (childAdorner != null && childAdorner.AreAnyKeyTipsVisible); }
        }

        public KeyTipAdorner ActiveKeyTipAdorner
        {
            get
            {
                return childAdorner != null && childAdorner.IsAdornerChainAlive
                           ? childAdorner.ActiveKeyTipAdorner
                           : this;
            }
        }

        #endregion

        #region Intialization

        /// <summary>
        /// Construcotor
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="parentAdorner">Parent adorner or null</param>
        /// <param name="keyTipElementContainer">The element which is container for elements</param>
        public KeyTipAdorner(UIElement adornedElement, UIElement keyTipElementContainer, KeyTipAdorner parentAdorner)
            : base(adornedElement)
        {
            this.parentAdorner = parentAdorner;

            this.keyTipElementContainer = keyTipElementContainer;

            // Try to find supported elements
            this.FindKeyTips(this.keyTipElementContainer, false);
            this.oneOfAssociatedElements = (FrameworkElement)(
                associatedElements.Count != 0
                    ? associatedElements[0]
                    : adornedElement // Maybe here is bug, coz we need keytipped item here...
                );

            this.keyTipPositions = new Point[this.keyTips.Count];
            this.backupedVisibilities = new Visibility[this.keyTips.Count];
        }

        // Find key tips on the given element
        private void FindKeyTips(UIElement element, bool hide)
        {
            this.Log("FindKeyTips");

            var children = LogicalTreeHelper.GetChildren(element);
            foreach (var item in children)
            {
                var child = item as UIElement;

                if (child == null
                    || child.Visibility != Visibility.Visible)
                {
                    continue;
                }

                var groupBox = child as RibbonGroupBox;

                var keys = KeyTip.GetKeys(child);
                if (keys != null)
                {
                    // Gotcha!
                    var keyTip = new KeyTip
                    {
                        Content = keys,
                        Visibility = hide
                            ? Visibility.Collapsed
                            : Visibility.Visible
                    };

                    // Add to list & visual 
                    // children collections
                    this.keyTips.Add(keyTip);
                    this.AddVisualChild(keyTip);
                    this.associatedElements.Add(child);

                    if (groupBox != null)
                    {
                        if (groupBox.State == RibbonGroupBoxState.Collapsed)
                        {
                            keyTip.Visibility = Visibility.Visible;
                            this.FindKeyTips(child, true);
                            continue;
                        }
                        else
                        {
                            keyTip.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        // Bind IsEnabled property
                        var binding = new Binding("IsEnabled")
                        {
                            Source = child,
                            Mode = BindingMode.OneWay
                        };
                        keyTip.SetBinding(IsEnabledProperty, binding);
                        continue;
                    }
                }

                if (groupBox != null &&
                    groupBox.State == RibbonGroupBoxState.Collapsed)
                {
                    this.FindKeyTips(child, true);
                }
                else
                {
                    this.FindKeyTips(child, hide);
                }
            }
        }

        #endregion

        #region Attach & Detach

        /// <summary>
        /// Attaches this adorner to the adorned element
        /// </summary>
        public void Attach()
        {
            if (this.attached)
            {
                return;
            }

            this.oneOfAssociatedElements.UpdateLayout();

            this.Log("Attach begin {0}", this.Visibility);

            if (!this.oneOfAssociatedElements.IsLoaded)
            {
                // Delay attaching
                this.Log("Delaying attach");
                this.oneOfAssociatedElements.Loaded += this.OnDelayAttach;
                return;
            }

            this.adornerLayer = GetAdornerLayer(this.oneOfAssociatedElements);

            if (this.adornerLayer == null)
            {
                this.Log("No adorner layer found");
                return;
            }

            // Focus current adorned element
            // Keyboard.Focus(adornedElement);
            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.Log("Focus Attached to {0}", this.focusedElement);
                this.focusedElement.LostKeyboardFocus += this.OnFocusLost;
                this.focusedElement.PreviewKeyDown += this.OnPreviewKeyDown;
                this.focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
            }
            else
            {
                this.Log("[!] Focus Setup Failed");
            }

            GetTopLevelElement(this.oneOfAssociatedElements).PreviewMouseDown += this.OnInputActionOccured;

            // Clears previous user input
            this.enteredKeys = "";
            this.FilterKeyTips();

            // Show this adorner
            this.adornerLayer.Add(this);

            // Hookup window activation
            this.attachedHwndSource = ((HwndSource)PresentationSource.FromVisual(this.oneOfAssociatedElements));
            if (this.attachedHwndSource != null)
            {
                this.attachedHwndSource.AddHook(this.WindowProc);
            }

            // Start timer to track focus changing
            if (this.timerFocusTracking == null)
            {
                this.timerFocusTracking = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Dispatcher.CurrentDispatcher)
                {
                    Interval = TimeSpan.FromMilliseconds(50)
                };
                this.timerFocusTracking.Tick += this.OnTimerFocusTrackingTick;
            }

            this.timerFocusTracking.Start();

            this.attached = true;

            this.Log("Attach end");
        }

        private void OnTimerFocusTrackingTick(object sender, EventArgs e)
        {
            if (this.focusedElement == Keyboard.FocusedElement)
            {
                return;
            }

            this.Log("Focus is changed, but focus lost is not occured");

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusLost;
                this.focusedElement.PreviewKeyDown -= this.OnPreviewKeyDown;
                this.focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
            }

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusLost;
                this.focusedElement.PreviewKeyDown += this.OnPreviewKeyDown;
                this.focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
            }
        }

        // Window's messages hook up
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check whether window is deactivated (wParam == 0)
            if ((msg == 6) && (wParam == IntPtr.Zero) && (attached))
            {
                this.Log("The host window is deactivated, keytips will be terminated");
                this.Terminate();
            }

            return IntPtr.Zero;
        }

        private void OnDelayAttach(object sender, EventArgs args)
        {
            this.Log("Delay attach (control loaded)");
            this.oneOfAssociatedElements.Loaded -= this.OnDelayAttach;
            this.Attach();
        }

        /// <summary>
        /// Detaches this adorner from the adorned element
        /// </summary> 
        public void Detach()
        {
            if (this.childAdorner != null)
            {
                this.childAdorner.Detach();
            }

            if (!this.attached)
            {
                return;
            }

            this.Log("Detach Begin");

            // Remove window hookup
            if ((this.attachedHwndSource != null) && (!this.attachedHwndSource.IsDisposed))
            {
                // Crashes in a few time if invoke immediately ???
                this.AdornedElement.Dispatcher.BeginInvoke((System.Threading.ThreadStart)(() => this.attachedHwndSource.RemoveHook(this.WindowProc)));
            }

            // Maybe adorner awaiting attaching, cancel it
            this.oneOfAssociatedElements.Loaded -= this.OnDelayAttach;

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusLost;
                this.focusedElement.PreviewKeyDown -= this.OnPreviewKeyDown;
                this.focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
                this.focusedElement = null;
            }

            GetTopLevelElement(oneOfAssociatedElements).PreviewMouseDown -= this.OnInputActionOccured;

            // Show this adorner
            this.adornerLayer.Remove(this);
            // Clears previous user input
            this.enteredKeys = "";
            this.attached = false;

            // Stop timer to track focus changing
            this.timerFocusTracking.Stop();

            this.Log("Detach End");
        }

        #endregion

        #region Termination

        /// <summary>
        /// Terminate whole key tip's adorner chain
        /// </summary>
        public void Terminate()
        {
            if (this.terminated)
            {
                return;
            }

            this.terminated = true;

            this.Detach();

            if (this.parentAdorner != null)
            {
                this.parentAdorner.Terminate();
            }

            if (this.childAdorner != null)
            {
                this.childAdorner.Terminate();
            }

            if (this.Terminated != null)
            {
                this.Terminated(this, EventArgs.Empty);
            }

            this.Log("Termination");
        }

        #endregion

        #region Event Handlers

        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.Log("Key Down {0} ({1})", e.Key, e.OriginalSource);

            if (e.IsRepeat
                || this.Visibility == Visibility.Hidden)
            {
                return;
            }

            if ((!(AdornedElement is ContextMenu)) &&
                ((e.Key == Key.Left) || (e.Key == Key.Right) || (e.Key == Key.Up) || (e.Key == Key.Down) ||
                (e.Key == Key.Enter) || (e.Key == Key.Tab)))
            {
                this.Visibility = Visibility.Hidden;
            }
            else if (e.Key == Key.Escape)
            {
                this.Back();
                e.Handled = true;
            }
        }

        private void OnFocusedElementPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var keyToSearch = this.enteredKeys + e.Text;

            if (this.IsElementsStartWith(keyToSearch))
            {
                this.enteredKeys += e.Text;

                var element = TryGetElement(enteredKeys);

                if (element != null)
                {
                    Forward(element);
                }
                else
                {
                    FilterKeyTips();
                }

                e.Handled = true;
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void OnInputActionOccured(object sender, RoutedEventArgs e)
        {
            if (!this.attached)
            {
                return;
            }

            this.Log("Input Action, Keystips will be terminated");
            this.Terminate();
        }

        private void OnFocusLost(object sender, RoutedEventArgs e)
        {
            if (!this.attached)
            {
                return;
            }

            this.Log("Focus Lost");

            var previousFocusedElementElement = this.focusedElement;
            this.focusedElement.LostKeyboardFocus -= this.OnFocusLost;
            this.focusedElement.PreviewKeyDown -= this.OnPreviewKeyDown;
            this.focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.Log("Focus Changed from {0} to {1}", previousFocusedElementElement, this.focusedElement);
                this.focusedElement.LostKeyboardFocus += this.OnFocusLost;
                this.focusedElement.PreviewKeyDown += this.OnPreviewKeyDown;
                this.focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
            }
            else
            {
                this.Log("Focus Not Restored");
            }
        }

        #endregion

        #region Static Methods

        private static AdornerLayer GetAdornerLayer(UIElement element)
        {
            var current = element;

            while (true)
            {
                if (current == null) return null;

                var parent = (UIElement)VisualTreeHelper.GetParent(current)
                    ?? (UIElement)LogicalTreeHelper.GetParent(current);

                current = parent;

                if (current is AdornerDecorator)
                {
                    return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
                }
            }
        }

        private static UIElement GetTopLevelElement(UIElement element)
        {
            var current = element;

            while (true)
            {
                current = (VisualTreeHelper.GetParent(element)) as UIElement;

                if (current == null)
                {
                    return element;
                }

                element = current;
            }
        }

        #endregion

        #region Methods

        // Back to the previous adorner
        public void Back()
        {
            var control = this.keyTipElementContainer as IKeyTipedControl;
            if (control != null)
            {
                control.OnKeyTipBack();
            }

            if (this.parentAdorner != null)
            {
                this.Log("Back");
                this.Detach();
                this.parentAdorner.Attach();
            }
            else
            {
                this.Terminate();
            }
        }

        /// <summary>
        /// Forwards to the elements with the given keys
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="click">If true the element will be clicked</param>
        /// <returns>If the element will be found the function will return true</returns>
        public bool Forward(string keys, bool click)
        {
            this.Log("Trying to forward {0}", keys);

            var element = TryGetElement(keys);
            if (element == null)
            {
                return false;
            }

            Forward(element, click);
            return true;
        }

        // Forward to the next element
        private void Forward(UIElement element) { Forward(element, true); }

        // Forward to the next element
        private void Forward(UIElement element, bool click)
        {
            this.Log("Forwarding");

            this.Detach();

            if (click)
            {
                //element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, null));
                var control = element as IKeyTipedControl;
                if (control != null)
                {
                    control.OnKeyTipPressed();
                }
            }

            var children = LogicalTreeHelper.GetChildren(element)
                .OfType<UIElement>()
                .ToArray();

            if (children.Length == 0)
            {
                this.Terminate();
                return;
            }

            this.childAdorner = ReferenceEquals(GetTopLevelElement(children[0]), GetTopLevelElement(element)) == false
                ? new KeyTipAdorner(children[0], element, this)
                : new KeyTipAdorner(element, element, this);

            // Stop if no further KeyTips can be displayed.
            if (!this.childAdorner.keyTips.Any())
            {
                this.Terminate();
                return;
            }

            this.childAdorner.Attach();
        }

        /// <summary>
        /// Gets element keytipped by keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>Element</returns>
        private UIElement TryGetElement(string keys)
        {
            for (var i = 0; i < keyTips.Count; i++)
            {
                if (!keyTips[i].IsEnabled
                    || keyTips[i].Visibility != Visibility.Visible)
                {
                    continue;
                }

                var content = (string)keyTips[i].Content;

                if (keys.Equals(content, StringComparison.CurrentCultureIgnoreCase))
                {
                    return associatedElements[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Is one of the elements starts with the given chars
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private bool IsElementsStartWith(string keys)
        {
            foreach (var keyTip in this.keyTips.Where(x => x.IsEnabled))
            {
                var content = (string)keyTip.Content;

                if (content.StartsWith(keys, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Hide / unhide keytips relative matching to entered keys
        private void FilterKeyTips()
        {
            this.Log("FilterKeyTips");

            // Backup current visibility of key tips
            for (var i = 0; i < this.backupedVisibilities.Length; i++)
            {
                this.backupedVisibilities[i] = keyTips[i].Visibility;
            }

            // Hide / unhide keytips relative matching to entered keys
            for (var i = 0; i < this.keyTips.Count; i++)
            {
                var content = (string)this.keyTips[i].Content;

                if (string.IsNullOrEmpty(this.enteredKeys))
                {
                    this.keyTips[i].Visibility = this.backupedVisibilities[i];
                }
                else
                {
                    this.keyTips[i].Visibility = content.StartsWith(enteredKeys, StringComparison.CurrentCultureIgnoreCase)
                        ? this.backupedVisibilities[i]
                        : Visibility.Collapsed;
                }
            }

            this.Log("Filtered key tips: {0}", this.keyTips.Count(x => x.Visibility == Visibility.Visible));
        }

        #endregion

        #region Layout & Visual Children

        /// <summary>
        /// Positions child elements and determines
        /// a size for the control
        /// </summary>
        /// <param name="finalSize">The final area within the parent 
        /// that this element should use to arrange 
        /// itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Log("ArrangeOverride");

            for (var i = 0; i < this.keyTips.Count; i++)
            {
                this.keyTips[i].Arrange(new Rect(this.keyTipPositions[i], this.keyTips[i].DesiredSize));
            }

            return finalSize;
        }

        /// <summary>
        /// Measures KeyTips
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.Log("MeasureOverride");

            var infinitySize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (var tip in keyTips)
            {
                tip.Measure(infinitySize);
            }

            this.UpdateKeyTipPositions();

            var result = new Size(0, 0);
            for (var i = 0; i < this.keyTips.Count; i++)
            {
                var cornerX = this.keyTips[i].DesiredSize.Width + this.keyTipPositions[i].X;
                var cornerY = this.keyTips[i].DesiredSize.Height + this.keyTipPositions[i].Y;

                if (cornerX > result.Width)
                {
                    result.Width = cornerX;
                }

                if (cornerY > result.Height)
                {
                    result.Height = cornerY;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets parent RibbonGroupBox or null
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static RibbonGroupBox GetGroupBox(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            var groupBox = element as RibbonGroupBox;
            if (groupBox != null)
            {
                return groupBox;
            }

            var parent = VisualTreeHelper.GetParent(element);
            return GetGroupBox(parent);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        private void UpdateKeyTipPositions()
        {
            this.Log("UpdateKeyTipPositions");

            if (this.keyTips.Count == 0)
            {
                return;
            }

            double[] rows = null;
            var groupBox = GetGroupBox(this.oneOfAssociatedElements);
            if (groupBox != null)
            {
                var panel = groupBox.GetPanel();
                if (panel != null)
                {
                    var height = groupBox.GetLayoutRoot().DesiredSize.Height;
                    rows = new[]
                        {
                            groupBox.GetLayoutRoot().TranslatePoint(new Point(0, 0), AdornedElement).Y,
                            groupBox.GetLayoutRoot().TranslatePoint(new Point(0, panel.DesiredSize.Height / 2.0), AdornedElement).Y,
                            groupBox.GetLayoutRoot().TranslatePoint(new Point(0, panel.DesiredSize.Height), AdornedElement).Y,
                            groupBox.GetLayoutRoot().TranslatePoint(new Point(0, height + 1), AdornedElement).Y                            
                        };
                }
            }

            for (var i = 0; i < this.keyTips.Count; i++)
            {
                // Skip invisible keytips
                if (this.keyTips[i].Visibility != Visibility.Visible)
                {
                    continue;
                }

                // Update KeyTip Visibility
                var associatedElementIsVisible = associatedElements[i].IsVisible;
                var associatedElementInVisualTree = VisualTreeHelper.GetParent(associatedElements[i]) != null;
                this.keyTips[i].Visibility = associatedElementIsVisible && associatedElementInVisualTree ? Visibility.Visible : Visibility.Collapsed;

                if (!KeyTip.GetAutoPlacement(associatedElements[i]))
                {
                    #region Custom Placement

                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;

                    double x = 0, y = 0;
                    var margin = KeyTip.GetMargin(associatedElements[i]);

                    switch (KeyTip.GetHorizontalAlignment(associatedElements[i]))
                    {
                        case HorizontalAlignment.Left:
                            x = margin.Left;
                            break;
                        case HorizontalAlignment.Right:
                            x = elementSize.Width - keyTipSize.Width - margin.Right;
                            break;
                        case HorizontalAlignment.Center:
                        case HorizontalAlignment.Stretch:
                            x = elementSize.Width / 2.0 - keyTipSize.Width / 2.0 + margin.Left;
                            break;
                    }

                    switch (KeyTip.GetVerticalAlignment(associatedElements[i]))
                    {
                        case VerticalAlignment.Top:
                            y = margin.Top;
                            break;
                        case VerticalAlignment.Bottom:
                            y = elementSize.Height - keyTipSize.Height - margin.Bottom;
                            break;
                        case VerticalAlignment.Center:
                        case VerticalAlignment.Stretch:
                            y = elementSize.Height / 2.0 - keyTipSize.Height / 2.0 + margin.Top;
                            break;
                    }

                    keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(x, y), AdornedElement);

                    #endregion
                }
                else if (((FrameworkElement)associatedElements[i]).Name == "PART_DialogLauncherButton")
                {
                    // Dialog Launcher Button Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        0), AdornedElement);
                    keyTipPositions[i].Y = rows[3];
                }
                else if ((associatedElements[i] is InRibbonGallery && !((InRibbonGallery)associatedElements[i]).IsCollapsed))
                {
                    // InRibbonGallery Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width - keyTipSize.Width / 2.0,
                        0), AdornedElement);
                    keyTipPositions[i].Y = rows[2] - keyTipSize.Height / 2;
                }
                else if ((associatedElements[i] is RibbonTabItem) || (associatedElements[i] is Backstage))
                {
                    // Ribbon Tab Item Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        elementSize.Height - keyTipSize.Height / 2.0), AdornedElement);
                }
                else if (associatedElements[i] is RibbonGroupBox)
                {
                    // Ribbon Group Box Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        elementSize.Height + 1), AdornedElement);
                }
                else if (IsWithinQuickAccessToolbar(associatedElements[i]))
                {
                    var translatedPoint = associatedElements[i].TranslatePoint(new Point(
                        associatedElements[i].DesiredSize.Width / 2.0 - keyTips[i].DesiredSize.Width / 2.0,
                        associatedElements[i].DesiredSize.Height - keyTips[i].DesiredSize.Height / 2.0), AdornedElement);
                    keyTipPositions[i] = translatedPoint;
                }
                else if (associatedElements[i] is MenuItem)
                {
                    // MenuItem Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(
                            elementSize.Height / 2.0 + 2,
                            elementSize.Height / 2.0 + 2), AdornedElement);
                }
                else if (associatedElements[i] is BackstageTabItem)
                {
                    // BackstageButton Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(
                            5,
                            elementSize.Height / 2.0 - keyTipSize.Height), AdornedElement);
                }
                else if (((FrameworkElement)associatedElements[i]).Parent is BackstageTabControl)
                {
                    // Backstage Items Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(
                            20,
                            elementSize.Height / 2.0 - keyTipSize.Height), AdornedElement);
                }
                else
                {
                    if ((RibbonProperties.GetSize(associatedElements[i]) != RibbonControlSize.Large)
                        || (associatedElements[i] is Spinner)
                        || (associatedElements[i] is ComboBox)
                        || (associatedElements[i] is TextBox)
                        || (associatedElements[i] is CheckBox))
                    {
                        var withinRibbonToolbar = IsWithinRibbonToolbarInTwoLine(associatedElements[i]);
                        var translatedPoint = associatedElements[i].TranslatePoint(new Point(keyTips[i].DesiredSize.Width / 2.0, keyTips[i].DesiredSize.Height / 2.0), AdornedElement);

                        // Snapping to rows if it present
                        if (rows != null)
                        {
                            var index = 0;
                            var mindistance = Math.Abs(rows[0] - translatedPoint.Y);
                            for (var j = 1; j < rows.Length; j++)
                            {
                                if (withinRibbonToolbar && j == 1)
                                {
                                    continue;
                                }

                                var distance = Math.Abs(rows[j] - translatedPoint.Y);
                                if (distance < mindistance)
                                {
                                    mindistance = distance;
                                    index = j;
                                }
                            }

                            translatedPoint.Y = rows[index] - keyTips[i].DesiredSize.Height / 2.0;
                        }

                        keyTipPositions[i] = translatedPoint;
                    }
                    else
                    {
                        var translatedPoint = associatedElements[i].TranslatePoint(new Point(
                            associatedElements[i].DesiredSize.Width / 2.0 - keyTips[i].DesiredSize.Width / 2.0,
                            associatedElements[i].DesiredSize.Height - 8), AdornedElement);
                        if (rows != null)
                        {
                            translatedPoint.Y = rows[2] - keyTips[i].DesiredSize.Height / 2.0;
                        }

                        keyTipPositions[i] = translatedPoint;
                    }
                }
            }
        }

        // Determines whether the element is children to RibbonToolBar
        private static bool IsWithinRibbonToolbarInTwoLine(DependencyObject element)
        {
            var parent = LogicalTreeHelper.GetParent(element) as UIElement;
            var ribbonToolBar = parent as RibbonToolBar;
            if (ribbonToolBar != null)
            {
                var definition = ribbonToolBar.GetCurrentLayoutDefinition();
                if (definition == null)
                {
                    return false;
                }

                if (definition.RowCount == 2 || definition.Rows.Count == 2)
                {
                    return true;
                }

                return false;
            }

            if (parent == null)
            {
                return false;
            }

            return IsWithinRibbonToolbarInTwoLine(parent);
        }

        // Determines whether the element is children to quick access toolbar
        private static bool IsWithinQuickAccessToolbar(DependencyObject element)
        {
            var parent = LogicalTreeHelper.GetParent(element) as UIElement;
            if (parent is QuickAccessToolBar)
            {
                return true;
            }

            if (parent == null)
            {
                return false;
            }

            return IsWithinQuickAccessToolbar(parent);
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.keyTips.Count;
            }
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.keyTips[index];
        }

        #endregion

        #region Logging

        [SuppressMessage("Microsoft.Performance", "CA1822")]
        [SuppressMessage("Microsoft.Performance", "CA1801")]
        [Conditional("DEBUG")]
        private void Log(string format, params object[] args)
        {
            Debug.WriteLine("[" + this.AdornedElement.GetType().Name + "] " + string.Format(format, args));
        }

        #endregion
    }
}