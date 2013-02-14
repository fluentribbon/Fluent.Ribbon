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
        List<KeyTip> keyTips = new List<KeyTip>();
        List<UIElement> associatedElements = new List<UIElement>();
        FrameworkElement oneOfAssociatedElements;
        Point[] keyTipPositions;

        // Parent adorner
        KeyTipAdorner parentAdorner;
        KeyTipAdorner childAdorner;

        // Focused element
        IInputElement focusedElement;

        Visibility[] backupedVisibilities;
        private UIElement keyTipElementContainer;

        // Is this adorner attached to the adorned element?
        bool attached;
        HwndSource attachedHwndSource;

        // Current entered chars
        string enteredKeys = "";

        // Designate that this adorner is terminated
        bool terminated;

        DispatcherTimer timerFocusTracking;

        AdornerLayer adornerLayer;

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
            FindKeyTips(this.keyTipElementContainer, false);
            oneOfAssociatedElements = (FrameworkElement)(associatedElements.Count != 0 ?
                associatedElements[0] :
                adornedElement // Maybe here is bug, coz we need keytipped item here...
                );

            keyTipPositions = new Point[keyTips.Count];
        }

        // Find key tips on the given element
        private void FindKeyTips(UIElement element, bool hide)
        {
            this.Log("FindKeyTips");

            IEnumerable children = LogicalTreeHelper.GetChildren(element);
            foreach (object item in children)
            {
                UIElement child = item as UIElement;
                RibbonGroupBox groupBox = (child as RibbonGroupBox);
                if (child != null)
                {
                    string keys = KeyTip.GetKeys(child);
                    if (keys != null)
                    {
                        // Gotcha!
                        KeyTip keyTip = new KeyTip();
                        keyTip.Content = keys;
                        keyTip.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;

                        // Add to list & visual 
                        // children collections
                        keyTips.Add(keyTip);
                        base.AddVisualChild(keyTip);
                        associatedElements.Add(child);

                        if (groupBox != null)
                        {
                            if (groupBox.State == RibbonGroupBoxState.Collapsed)
                            {
                                keyTip.Visibility = Visibility.Visible;
                                FindKeyTips(child, true);
                                continue;
                            }
                            else keyTip.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            // Bind IsEnabled property
                            Binding binding = new Binding("IsEnabled");
                            binding.Source = child;
                            binding.Mode = BindingMode.OneWay;
                            keyTip.SetBinding(UIElement.IsEnabledProperty, binding);
                            continue;
                        }
                    }

                    if ((groupBox != null) &&
                       (groupBox.State == RibbonGroupBoxState.Collapsed))
                        FindKeyTips(child, true);
                    else FindKeyTips(child, hide);
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
            if (attached) return;
            Log("Attach begin");

            if (!oneOfAssociatedElements.IsLoaded)
            {
                // Delay attaching
                oneOfAssociatedElements.Loaded += OnDelayAttach;
                return;
            }

            adornerLayer = GetAdornerLayer(oneOfAssociatedElements);
            if (adornerLayer == null) return;

            // Focus current adorned element
            // Keyboard.Focus(adornedElement);
            focusedElement = Keyboard.FocusedElement;
            if (focusedElement != null)
            {
                Log("Focus Attached to " + focusedElement.ToString());
                focusedElement.LostKeyboardFocus += OnFocusLost;
                focusedElement.PreviewKeyDown += OnPreviewKeyDown;
                focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
            }
            else Log("[!] Focus Setup Failed");
            GetTopLevelElement(oneOfAssociatedElements).PreviewMouseDown += OnInputActionOccured;

            // Show this adorner
            adornerLayer.Add(this);

            // Clears previous user input
            enteredKeys = "";
            FilterKeyTips();


            // Hookup window activation
            attachedHwndSource = ((HwndSource)PresentationSource.FromVisual(oneOfAssociatedElements));
            if (attachedHwndSource != null) attachedHwndSource.AddHook(WindowProc);

            // Start timer to track focus changing
            if (timerFocusTracking == null)
            {
                timerFocusTracking = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Dispatcher.CurrentDispatcher);
                timerFocusTracking.Interval = TimeSpan.FromMilliseconds(50);
                timerFocusTracking.Tick += OnTimerFocusTrackingTick;
            }
            timerFocusTracking.Start();

            attached = true;
            Log("Attach end");
        }

        private void OnTimerFocusTrackingTick(object sender, EventArgs e)
        {
            if (focusedElement != Keyboard.FocusedElement)
            {
                Log("Focus is changed, but focus lost is not occured");
                if (focusedElement != null)
                {
                    focusedElement.LostKeyboardFocus -= OnFocusLost;
                    focusedElement.PreviewKeyDown -= OnPreviewKeyDown;
                    focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
                }
                focusedElement = Keyboard.FocusedElement;
                if (focusedElement != null)
                {
                    focusedElement.LostKeyboardFocus += OnFocusLost;
                    focusedElement.PreviewKeyDown += OnPreviewKeyDown;
                    focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
                }
            }
        }

        // Window's messages hook up
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check whether window is deactivated (wParam == 0)
            if ((msg == 6) && (wParam == IntPtr.Zero) && (attached))
            {
                Log("The host window is deactivated, keytips will be terminated");
                Terminate();
            }
            return IntPtr.Zero;
        }

        private void OnDelayAttach(object sender, EventArgs args)
        {
            oneOfAssociatedElements.Loaded -= OnDelayAttach;
            Attach();
        }

        /// <summary>
        /// Detaches this adorner from the adorned element
        /// </summary> 
        public void Detach()
        {
            if (childAdorner != null) childAdorner.Detach();
            if (!attached) return;


            Log("Detach Begin");

            // Remove window hookup
            if ((attachedHwndSource != null) && (!attachedHwndSource.IsDisposed))
            {
                // Crashes in a few time if invoke immediately ???
                AdornedElement.Dispatcher.BeginInvoke((System.Threading.ThreadStart)delegate { attachedHwndSource.RemoveHook(WindowProc); });
            }

            // Maybe adorner awaiting attaching, cancel it
            oneOfAssociatedElements.Loaded -= OnDelayAttach;

            if (focusedElement != null)
            {
                focusedElement.LostKeyboardFocus -= OnFocusLost;
                focusedElement.PreviewKeyDown -= OnPreviewKeyDown;
                focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
                focusedElement = null;
            }

            GetTopLevelElement(oneOfAssociatedElements).PreviewMouseDown -= OnInputActionOccured;

            // Show this adorner
            adornerLayer.Remove(this);
            // Clears previous user input
            enteredKeys = "";
            attached = false;

            // Stop timer to track focus changing
            timerFocusTracking.Stop();

            Log("Detach End");
        }

        #endregion

        #region Termination

        /// <summary>
        /// Terminate whole key tip's adorner chain
        /// </summary>
        public void Terminate()
        {
            if (terminated)
            {
                return;
            }

            terminated = true;

            Detach();
            if (parentAdorner != null) parentAdorner.Terminate();
            if (childAdorner != null) childAdorner.Terminate();
            if (Terminated != null) Terminated(this, EventArgs.Empty);

            Log("Termination");
        }

        #endregion

        #region Event Handlers

        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Log("Key Down " + e.Key.ToString() + " (" + e.OriginalSource + ")");

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
                Back();
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
            if (attached)
            {
                Log("Focus Lost");

                var previousFocusedElementElement = focusedElement;
                focusedElement.LostKeyboardFocus -= OnFocusLost;
                focusedElement.PreviewKeyDown -= OnPreviewKeyDown;
                focusedElement.PreviewTextInput -= this.OnFocusedElementPreviewTextInput;
                focusedElement = Keyboard.FocusedElement;

                if (focusedElement != null)
                {
                    Log("Focus Changed from " + previousFocusedElementElement.ToString() + " to " + focusedElement.ToString());
                    focusedElement.LostKeyboardFocus += OnFocusLost;
                    focusedElement.PreviewKeyDown += OnPreviewKeyDown;
                    focusedElement.PreviewTextInput += this.OnFocusedElementPreviewTextInput;
                }
                else
                {
                    Log("Focus Not Restored");
                }
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
        private void Back()
        {
            var control = this.keyTipElementContainer as IKeyTipedControl;
            if (control != null)
            {
                control.OnKeyTipBack();
            }

            if (parentAdorner != null)
            {
                Log("Back");
                Detach();
                parentAdorner.Attach();
            }
            else
            {
                Terminate();
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
            Detach();
            if (click)
            {
                //element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, null));
                var control = element as IKeyTipedControl;
                if (control != null) control.OnKeyTipPressed();
                element.UpdateLayout();
            }

            var children = LogicalTreeHelper.GetChildren(element)
                .Cast<object>()
                .Where(x => x is UIElement)
                .Cast<UIElement>().ToArray();
            if (children.Length == 0) { Terminate(); return; }

            childAdorner = GetTopLevelElement(children[0]) != GetTopLevelElement(element) ?
                new KeyTipAdorner(children[0], element, this) :
                new KeyTipAdorner(element, element, this);

            Detach();
            childAdorner.Attach();
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
                if (!keyTips[i].IsEnabled)
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
            foreach (var keyTip in keyTips.Where(x => x.IsEnabled))
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

            if (backupedVisibilities == null)
            {
                // Backup current visibility of key tips
                backupedVisibilities = new Visibility[keyTips.Count];

                for (var i = 0; i < backupedVisibilities.Length; i++)
                {
                    backupedVisibilities[i] = keyTips[i].Visibility;
                }
            }

            // Hide / unhide keytips relative matching to entered keys
            for (var i = 0; i < keyTips.Count; i++)
            {
                var content = (string)keyTips[i].Content;

                if (string.IsNullOrEmpty(enteredKeys))
                {
                    keyTips[i].Visibility = backupedVisibilities[i];
                }
                else
                {
                    keyTips[i].Visibility = content.StartsWith(enteredKeys, StringComparison.CurrentCultureIgnoreCase)
                        ? backupedVisibilities[i]
                        : Visibility.Collapsed;
                }
            }
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
            Rect rect = new Rect(finalSize);

            for (int i = 0; i < keyTips.Count; i++)
            {
                keyTips[i].Arrange(new Rect(keyTipPositions[i], keyTips[i].DesiredSize));
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
            Size infinitySize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (KeyTip tip in keyTips) tip.Measure(infinitySize);
            UpdateKeyTipPositions();

            Size result = new Size(0, 0);
            for (int i = 0; i < keyTips.Count; i++)
            {
                double cornerX = keyTips[i].DesiredSize.Width + keyTipPositions[i].X;
                double cornerY = keyTips[i].DesiredSize.Height + keyTipPositions[i].Y;
                if (cornerX > result.Width) result.Width = cornerX;
                if (cornerY > result.Height) result.Height = cornerY;
            }
            return result;
        }

        /// <summary>
        /// Gets parent RibbonGroupBox or null
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static RibbonGroupBox GetGroupBox(DependencyObject element)
        {
            if (element == null) return null;
            RibbonGroupBox groupBox = element as RibbonGroupBox;
            if (groupBox != null) return groupBox;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            return GetGroupBox(parent);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        private void UpdateKeyTipPositions()
        {
            this.Log("UpdateKeyTipPositions");

            if (keyTips.Count == 0)
            {
                return;
            }

            double[] rows = null;
            var groupBox = GetGroupBox(oneOfAssociatedElements);
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

            for (var i = 0; i < keyTips.Count; i++)
            {
                // Skip invisible keytips
                if (keyTips[i].Visibility != Visibility.Visible)
                {
                    continue;
                }

                // Update KeyTip Visibility
                var associatedElementIsVisible = associatedElements[i].Visibility == Visibility.Visible;
                var associatedElementInVisualTree = VisualTreeHelper.GetParent(associatedElements[i]) != null;
                keyTips[i].Visibility = associatedElementIsVisible && associatedElementInVisualTree ? Visibility.Visible : Visibility.Collapsed;

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
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                            0), AdornedElement);
                    keyTipPositions[i].Y = rows[3];
                }
                else if ((associatedElements[i] is InRibbonGallery && !((InRibbonGallery)associatedElements[i]).IsCollapsed))
                {
                    // InRibbonGallery Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width - keyTipSize.Width / 2.0,
                            0), AdornedElement);
                    keyTipPositions[i].Y = rows[2] - keyTipSize.Height / 2;
                }
                else if ((associatedElements[i] is RibbonTabItem) || (associatedElements[i] is Backstage))
                {
                    // Ribbon Tab Item Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].RenderSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                            elementSize.Height - keyTipSize.Height / 2.0), AdornedElement);
                }
                else if (associatedElements[i] is RibbonGroupBox)
                {
                    // Ribbon Group Box Exclusive Placement
                    var keyTipSize = keyTips[i].DesiredSize;
                    var elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
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
                    if (((associatedElements[i] is IRibbonControl) && (((IRibbonControl)associatedElements[i]).Size != RibbonControlSize.Large)) ||
                        (associatedElements[i] is Spinner) || (associatedElements[i] is ComboBox) || (associatedElements[i] is TextBox) || (associatedElements[i] is CheckBox))
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
        static bool IsWithinRibbonToolbarInTwoLine(DependencyObject element)
        {
            UIElement parent = LogicalTreeHelper.GetParent(element) as UIElement;
            RibbonToolBar ribbonToolBar = parent as RibbonToolBar;
            if (ribbonToolBar != null)
            {
                RibbonToolBarLayoutDefinition definition = ribbonToolBar.GetCurrentLayoutDefinition();
                if (definition == null) return false;
                if (definition.RowCount == 2 || definition.Rows.Count == 2) return true;
                return false;
            }
            if (parent == null) return false;
            return IsWithinRibbonToolbarInTwoLine(parent);
        }

        // Determines whether the element is children to quick access toolbar
        static bool IsWithinQuickAccessToolbar(DependencyObject element)
        {
            UIElement parent = LogicalTreeHelper.GetParent(element) as UIElement;
            if (parent is QuickAccessToolBar) return true;
            if (parent == null) return false;
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
            Trace.WriteLine("[" + AdornedElement.GetType().Name + "] " + string.Format(format, args));
        }

        #endregion
    }
}