 // ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Fluent.Internal;

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
        private KeyTipAdorner childAdorner;

        private readonly Visibility[] backupedVisibilities;
        private readonly UIElement keyTipElementContainer;

        // Is this adorner attached to the adorned element?
        private bool attached;

        // Designate that this adorner is terminated
        private bool terminated;

        private AdornerLayer adornerLayer;

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether at least one on the adorners in the chain is alive
        /// </summary>
        public bool IsAdornerChainAlive
        {
            get { return this.attached || (this.childAdorner != null && this.childAdorner.IsAdornerChainAlive); }
        }

        public bool AreAnyKeyTipsVisible
        {
            get { return this.keyTips.Any(x => x.IsVisible) || (this.childAdorner != null && this.childAdorner.AreAnyKeyTipsVisible); }
        }

        public KeyTipAdorner ActiveKeyTipAdorner
        {
            get
            {
                return this.childAdorner != null && this.childAdorner.IsAdornerChainAlive
                           ? this.childAdorner.ActiveKeyTipAdorner
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
            this.oneOfAssociatedElements = (FrameworkElement)(this.associatedElements.Count != 0
                    ? this.associatedElements[0]
                    : adornedElement // Maybe here is bug, coz we need keytipped item here...
                );

            this.keyTipPositions = new Point[this.keyTips.Count];
            this.backupedVisibilities = new Visibility[this.keyTips.Count];
        }

        // Find key tips on the given element
        private void FindKeyTips(UIElement element, bool hide)
        {
            this.Log("FindKeyTips");

            var children = GetVisibleChildren(element);

            foreach (var child in children)
            {
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

                    this.Log("Found KeyTipped element \"{0}\" with keys \"{1}\".", child, keys);

                    if (groupBox != null)
                    {
                        if (groupBox.State == RibbonGroupBoxState.Collapsed || groupBox.State== RibbonGroupBoxState.QuickAccess)
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

        public static IList<UIElement> GetVisibleChildren(UIElement element)
        {
            var logicalChildren = LogicalTreeHelper.GetChildren(element)                
                .OfType<UIElement>();

            var children = logicalChildren;

            // Always using the visual tree is very expensive, so we only search through it when you got specific control types.
            // Using the visual tree here, in addition to the logical, partially fixes #244.
            if (element is ContentPresenter
                || element is ContentControl)
                
            {
                children = children
                    .Concat(UIHelper.GetVisualChildren(element))
                    .OfType<UIElement>();
            }

            if(element is ItemsControl)
            {
              ItemsControl db = (ItemsControl)element;
              List<UIElement> items = new List<UIElement>();
              foreach(var item in db.Items)
              {
                if (item != null)
                {
                  UIElement ui = db.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                  if (ui != null)
                    items.Add(ui);
                }
              }
              
              children = items;
          }

            return children
                .Where(x => x.Visibility == Visibility.Visible)
                .Distinct()
                .ToList();
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

            if (this.oneOfAssociatedElements.IsLoaded == false)
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

            this.FilterKeyTips(string.Empty);

            // Show this adorner
            this.adornerLayer.Add(this);

            this.attached = true;

            this.Log("Attach end");
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
            this.childAdorner?.Detach();

            if (!this.attached)
            {
                return;
            }

            this.Log("Detach Begin");

            // Maybe adorner awaiting attaching, cancel it
            this.oneOfAssociatedElements.Loaded -= this.OnDelayAttach;

            // Show this adorner
            this.adornerLayer.Remove(this);

            this.attached = false;

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

            this.parentAdorner?.Terminate();

            this.childAdorner?.Terminate();

            this.Terminated?.Invoke(this, EventArgs.Empty);

            this.Log("Termination");
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
            while (true)
            {
                var current = VisualTreeHelper.GetParent(element) as UIElement;

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
            control?.OnKeyTipBack();

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

            var element = this.TryGetElement(keys);
            if (element == null)
            {
                return false;
            }

            this.Forward(element, click);
            return true;
        }

        // Forward to the next element
        private void Forward(UIElement element, bool click)
        {
            this.Log("Forwarding to {0}", element);

            this.Detach();

            if (click)
            {
                //element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, null));
                var control = element as IKeyTipedControl;
                control?.OnKeyTipPressed();
            }

            var children = GetVisibleChildren(element);

            if (children.Count == 0)
            {
                this.Terminate();
                return;
            }

            this.childAdorner = ReferenceEquals(GetTopLevelElement(children[0]), GetTopLevelElement(element)) == false
                ? new KeyTipAdorner(children[0], element, this)
                : new KeyTipAdorner(element, element, this);

            // Stop if no further KeyTips can be displayed.
            if (this.childAdorner.keyTips.Any() == false)
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
            for (var i = 0; i < this.keyTips.Count; i++)
            {
                if (!this.keyTips[i].IsEnabled
                    ||
                    this.keyTips[i].Visibility != Visibility.Visible)
                {
                    continue;
                }

                var content = (string)this.keyTips[i].Content;

                if (keys.Equals(content, StringComparison.CurrentCultureIgnoreCase))
                {
                    return this.associatedElements[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if an of the keytips contained in this adorner start with <paramref name="keys"/>
        /// </summary>
        /// <returns><c>true</c> if any keytip start with <paramref name="keys"/>. Otherwise <c>false</c>.</returns>
        public bool ContainsKeyTipStartingWith(string keys)
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
        internal void FilterKeyTips(string keys)
        {
            this.Log("FilterKeyTips with {0}", keys);

            // Backup current visibility of key tips
            for (var i = 0; i < this.backupedVisibilities.Length; i++)
            {
                this.backupedVisibilities[i] = this.keyTips[i].Visibility;
            }

            // Hide / unhide keytips relative matching to entered keys
            for (var i = 0; i < this.keyTips.Count; i++)
            {
                var content = (string)this.keyTips[i].Content;

                if (string.IsNullOrEmpty(keys))
                {
                    this.keyTips[i].Visibility = this.backupedVisibilities[i];
                }
                else
                {
                    this.keyTips[i].Visibility = content.StartsWith(keys, StringComparison.CurrentCultureIgnoreCase)
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

            var infinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (var tip in this.keyTips)
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
            var panel = groupBox?.GetPanel();

            if (panel != null)
            {
                var height = groupBox.GetLayoutRoot().DesiredSize.Height;
                rows = new[]
                       {
                           groupBox.GetLayoutRoot().TranslatePoint(new Point(0, 0), this.AdornedElement).Y,
                           groupBox.GetLayoutRoot().TranslatePoint(new Point(0, panel.DesiredSize.Height / 2.0), this.AdornedElement).Y,
                           groupBox.GetLayoutRoot().TranslatePoint(new Point(0, panel.DesiredSize.Height), this.AdornedElement).Y,
                           groupBox.GetLayoutRoot().TranslatePoint(new Point(0, height + 1), this.AdornedElement).Y
                       };
            }

            for (var i = 0; i < this.keyTips.Count; i++)
            {
                // Skip invisible keytips
                if (this.keyTips[i].Visibility != Visibility.Visible)
                {
                    continue;
                }

                // Update KeyTip Visibility
                var associatedElementIsVisible = this.associatedElements[i].IsVisible;
                var associatedElementInVisualTree = VisualTreeHelper.GetParent(this.associatedElements[i]) != null;
                this.keyTips[i].Visibility = associatedElementIsVisible && associatedElementInVisualTree ? Visibility.Visible : Visibility.Collapsed;

                if (KeyTip.GetAutoPlacement(this.associatedElements[i]) == false)
                {
                    #region Custom Placement

                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].RenderSize;

                    double x = 0, y = 0;
                    var margin = KeyTip.GetMargin(this.associatedElements[i]);

                    switch (KeyTip.GetHorizontalAlignment(this.associatedElements[i]))
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

                    switch (KeyTip.GetVerticalAlignment(this.associatedElements[i]))
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

                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(new Point(x, y), this.AdornedElement);

                    #endregion
                }
                else if (((FrameworkElement)this.associatedElements[i]).Name == "PART_DialogLauncherButton")
                {
                    // Dialog Launcher Button Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        0), this.AdornedElement);
                    this.keyTipPositions[i].Y = rows[3];
                }
                else if (this.associatedElements[i] is InRibbonGallery && !((InRibbonGallery)this.associatedElements[i]).IsCollapsed)
                {
                    // InRibbonGallery Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width - keyTipSize.Width / 2.0,
                        0), this.AdornedElement);
                    this.keyTipPositions[i].Y = rows[2] - keyTipSize.Height / 2;
                }
                else if (this.associatedElements[i] is RibbonTabItem || this.associatedElements[i] is Backstage)
                {
                    // Ribbon Tab Item Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].RenderSize;
                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        elementSize.Height - keyTipSize.Height / 2.0), this.AdornedElement);
                }
                else if (IsWithinQuickAccessToolbar(this.associatedElements[i]))
                {
                  var translatedPoint = this.associatedElements[i].TranslatePoint(new Point(this.associatedElements[i].DesiredSize.Width / 2.0 - this.keyTips[i].DesiredSize.Width / 2.0, this.associatedElements[i].DesiredSize.Height - this.keyTips[i].DesiredSize.Height / 2.0), this.AdornedElement);
                  this.keyTipPositions[i] = translatedPoint;
                }
                else if (this.associatedElements[i] is RibbonGroupBox)
                {
                    // Ribbon Group Box Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].DesiredSize;
                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(new Point(
                        elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                        elementSize.Height + 1), this.AdornedElement);
                }
                
                else if (this.associatedElements[i] is MenuItem)
                {
                    // MenuItem Exclusive Placement                    
                    var elementSize = this.associatedElements[i].DesiredSize;
                    this.keyTipPositions[i] = this.associatedElements[i].TranslatePoint(
                        new Point(
                            elementSize.Height / 2.0 + 2,
                            elementSize.Height / 2.0 + 2), this.AdornedElement);
                }
                else if (((FrameworkElement)this.associatedElements[i]).Parent is BackstageTabControl)
                {
                    // Backstage Items Exclusive Placement
                    var keyTipSize = this.keyTips[i].DesiredSize;
                    var elementSize = this.associatedElements[i].DesiredSize;
                    var parent = (UIElement)((FrameworkElement)this.associatedElements[i]).Parent;
                    var positionInParent = this.associatedElements[i].TranslatePoint(default(Point), parent);
                    this.keyTipPositions[i] = parent.TranslatePoint(
                        new Point(
                            5,
                            positionInParent.Y + (elementSize.Height / 2.0 - keyTipSize.Height)), this.AdornedElement);
                }
                else
                {
                    if ((RibbonProperties.GetSize(this.associatedElements[i]) != RibbonControlSize.Large)
                        || this.associatedElements[i] is Spinner
                        || this.associatedElements[i] is ComboBox
                        || this.associatedElements[i] is TextBox
                        || this.associatedElements[i] is CheckBox)
                    {
                        var withinRibbonToolbar = IsWithinRibbonToolbarInTwoLine(this.associatedElements[i]);
                        var translatedPoint = this.associatedElements[i].TranslatePoint(new Point(this.keyTips[i].DesiredSize.Width / 2.0, this.keyTips[i].DesiredSize.Height / 2.0), this.AdornedElement);

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

                            translatedPoint.Y = rows[index] - this.keyTips[i].DesiredSize.Height / 2.0;
                        }

                        this.keyTipPositions[i] = translatedPoint;
                    }
                    else
                    {
                        var translatedPoint = this.associatedElements[i].TranslatePoint(new Point(this.associatedElements[i].DesiredSize.Width / 2.0 - this.keyTips[i].DesiredSize.Width / 2.0, this.associatedElements[i].DesiredSize.Height - 8), this.AdornedElement);
                        if (rows != null)
                        {
                            translatedPoint.Y = rows[2] - this.keyTips[i].DesiredSize.Height / 2.0;
                        }

                        this.keyTipPositions[i] = translatedPoint;
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
            var name = this.AdornedElement.GetType().Name;

            var headeredControl = this.AdornedElement as IHeaderedControl;
            if (headeredControl != null)
            {
                name += $" ({headeredControl.Header})";
            }

            var formatted = string.Format(format, args);
            Debug.WriteLine($"[{name}] {formatted}", "KeyTipAdorner");
        }

        #endregion
    }
}