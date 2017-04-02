 // ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Fluent.Extensibility;
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
        private readonly List<KeyTipInformation> keyTipInformations = new List<KeyTipInformation>();

        private readonly FrameworkElement oneOfAssociatedElements;

        // Parent adorner
        private readonly KeyTipAdorner parentAdorner;
        private KeyTipAdorner childAdorner;

        private readonly FrameworkElement keyTipElementContainer;

        // Is this adorner attached to the adorned element?
        private bool attached;
        private bool isAttaching;

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
            get { return this.isAttaching || this.attached || (this.childAdorner != null && this.childAdorner.IsAdornerChainAlive); }
        }

        public bool AreAnyKeyTipsVisible
        {
            get { return this.keyTipInformations.Any(x => x.IsVisible) || (this.childAdorner != null && this.childAdorner.AreAnyKeyTipsVisible); }
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
        public KeyTipAdorner(FrameworkElement adornedElement, FrameworkElement keyTipElementContainer, KeyTipAdorner parentAdorner)
            : base(adornedElement)
        {
            this.parentAdorner = parentAdorner;

            this.keyTipElementContainer = keyTipElementContainer;

            // Try to find supported elements
            this.FindKeyTips(this.keyTipElementContainer, false);
            this.oneOfAssociatedElements = this.keyTipInformations.Count != 0
                    ? this.keyTipInformations[0].AssociatedElement
                    : adornedElement // Maybe here is bug, coz we need keytipped item here...
                ;
        }

        // Find key tips on the given element
        private void FindKeyTips(FrameworkElement element, bool hide)
        {
            var children = GetVisibleChildren(element);

            foreach (var child in children)
            {
                var groupBox = child as RibbonGroupBox;

                var keys = KeyTip.GetKeys(child);
                if (keys != null)
                {
                    if (groupBox != null)
                    {
                        this.GenerateAndAddGroupBoxKeyTipInformation(hide, keys, child, groupBox);
                    }
                    else
                    {
                        this.GenerateAndAddRegularKeyTipInformations(keys, child, hide);

                        // Do no search deeper in the tree
                        continue;
                    }
                }

                var innerHide = hide || groupBox?.State == RibbonGroupBoxState.Collapsed;
                this.FindKeyTips(child, innerHide);
            }
        }

        private void GenerateAndAddGroupBoxKeyTipInformation(bool hide, string keys, FrameworkElement child, RibbonGroupBox groupBox)
        {
            var keyTipInformation = new KeyTipInformation(keys, child, hide);

            // Add to list & visual children collections                    
            this.AddKeyTipInformationElement(keyTipInformation);

            this.Log("Found KeyTipped RibbonGroupBox \"{0}\" with keys \"{1}\".", keyTipInformation.AssociatedElement, keyTipInformation.Keys);

            keyTipInformation.KeyTip.Visibility = groupBox.State == RibbonGroupBoxState.Collapsed
                                                      ? Visibility.Visible
                                                      : Visibility.Collapsed;
        }

        private void GenerateAndAddRegularKeyTipInformations(string keys, FrameworkElement child, bool hide)
        {
            IEnumerable<KeyTipInformation> informations;
            var keyTipInformationProvider = child as IKeyTipInformationProvider;

            if (keyTipInformationProvider != null)
            {
                informations = keyTipInformationProvider.GetKeyTipInformations(hide);
            }
            else
            {
                informations = new []{ new KeyTipInformation(keys, child, hide) };
            }

            foreach (var keyTipInformation in informations)
            {
                // Add to list & visual children collections
                this.AddKeyTipInformationElement(keyTipInformation);

                this.Log("Found KeyTipped element \"{0}\" with keys \"{1}\".", keyTipInformation.AssociatedElement, keyTipInformation.Keys);
            }
        }

        private void AddKeyTipInformationElement(KeyTipInformation keyTipInformation)
        {
            this.keyTipInformations.Add(keyTipInformation);
            this.AddVisualChild(keyTipInformation.KeyTip);
        }

        private static IList<FrameworkElement> GetVisibleChildren(FrameworkElement element)
        {
            var logicalChildren = LogicalTreeHelper.GetChildren(element)                
                .OfType<FrameworkElement>();

            var children = logicalChildren;

            // Always using the visual tree is very expensive, so we only search through it when you got specific control types.
            // Using the visual tree here, in addition to the logical, partially fixes #244.
            if (element is ContentPresenter
                || element is ContentControl)
            {
                children = children
                    .Concat(UIHelper.GetVisualChildren(element))
                    .OfType<FrameworkElement>();
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

            this.isAttaching = true;

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
                this.isAttaching = false;
                return;
            }
           
            this.FilterKeyTips(string.Empty);

            // Show this adorner
            this.adornerLayer.Add(this);

            this.isAttaching = false;
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
        private void Forward(FrameworkElement element, bool click)
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

            // Panels aren't good elements to adorn. For example, trying to display KeyTips on MenuItems in SplitButton fails if using a panel.
            var validChild = children.FirstOrDefault(x => x is Panel == false) ?? children[0];
            this.childAdorner = ReferenceEquals(GetTopLevelElement(validChild), GetTopLevelElement(element)) == false
                ? new KeyTipAdorner(validChild, element, this)
                : new KeyTipAdorner(element, element, this);

            // Stop if no further KeyTips can be displayed.
            if (this.childAdorner.keyTipInformations.Any() == false)
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
        private FrameworkElement TryGetElement(string keys)
        {
            return this.keyTipInformations.FirstOrDefault(x => x.IsEnabled && x.Visibility == Visibility.Visible && keys.Equals(x.Keys, StringComparison.CurrentCultureIgnoreCase))
                ?.AssociatedElement;
        }

        /// <summary>
        /// Determines if an of the keytips contained in this adorner start with <paramref name="keys"/>
        /// </summary>
        /// <returns><c>true</c> if any keytip start with <paramref name="keys"/>. Otherwise <c>false</c>.</returns>
        public bool ContainsKeyTipStartingWith(string keys)
        {
            foreach (var keyTipInformation in this.keyTipInformations.Where(x => x.IsEnabled))
            {
                var content = keyTipInformation.Keys;

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
            this.Log("FilterKeyTips with \"{0}\"", keys);

            // Reset visibility if filter is empty
            if (string.IsNullOrEmpty(keys))
            {
                foreach (var keyTipInformation in this.keyTipInformations)
                {
                    keyTipInformation.KeyTip.Visibility = Visibility.Visible;
                }
            }

            // Backup current visibility of key tips
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                keyTipInformation.BackupVisibility = keyTipInformation.KeyTip.Visibility;
            }

            // Hide / unhide keytips relative matching to entered keys
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                var content = keyTipInformation.Keys;

                if (string.IsNullOrEmpty(keys))
                {
                    keyTipInformation.KeyTip.Visibility = keyTipInformation.BackupVisibility;
                }
                else
                {
                    keyTipInformation.KeyTip.Visibility = content.StartsWith(keys, StringComparison.CurrentCultureIgnoreCase)
                                              ? keyTipInformation.BackupVisibility
                                              : Visibility.Collapsed;
                }
            }

            this.Log("Filtered key tips: {0}", this.keyTipInformations.Count(x => x.Visibility == Visibility.Visible));
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

            foreach (var keyTipInformation in this.keyTipInformations)
            {
                keyTipInformation.KeyTip.Arrange(new Rect(keyTipInformation.Position, keyTipInformation.KeyTip.DesiredSize));
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
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                keyTipInformation.KeyTip.Measure(infinitySize);
            }

            this.UpdateKeyTipPositions();

            var result = new Size(0, 0);
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                var cornerX = keyTipInformation.KeyTip.DesiredSize.Width + keyTipInformation.Position.X;
                var cornerY = keyTipInformation.KeyTip.DesiredSize.Height + keyTipInformation.Position.Y;

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

        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        private void UpdateKeyTipPositions()
        {
            this.Log("UpdateKeyTipPositions");

            if (this.keyTipInformations.Count == 0)
            {
                return;
            }

            double[] rows = null;
            var groupBox = UIHelper.GetParent<RibbonGroupBox>(this.oneOfAssociatedElements);
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

            foreach (var keyTipInformation in this.keyTipInformations)
            {
                // Skip invisible keytips
                if (keyTipInformation.Visibility != Visibility.Visible)
                {
                    continue;
                }

                // Update KeyTip Visibility
                var associatedElementIsVisible = keyTipInformation.AssociatedElement.IsVisible;
                var associatedElementInVisualTree = VisualTreeHelper.GetParent(keyTipInformation.AssociatedElement) != null;
                keyTipInformation.KeyTip.Visibility = associatedElementIsVisible && associatedElementInVisualTree ? Visibility.Visible : Visibility.Collapsed;

                if (KeyTip.GetAutoPlacement(keyTipInformation.AssociatedElement) == false)
                {
                    #region Custom Placement

                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    
                    var elementSize = keyTipInformation.AssociatedElement.RenderSize;

                    double x = 0, y = 0;

                    keyTipInformation.KeyTip.Margin = KeyTip.GetMargin(keyTipInformation.AssociatedElement);

                    switch (KeyTip.GetHorizontalAlignment(keyTipInformation.AssociatedElement))
                    {
                        case HorizontalAlignment.Left:
                            break;
                        case HorizontalAlignment.Right:
                            x = elementSize.Width - keyTipSize.Width;
                            break;
                        case HorizontalAlignment.Center:
                        case HorizontalAlignment.Stretch:
                            x = elementSize.Width / 2.0 - keyTipSize.Width / 2.0;
                            break;
                    }

                    switch (KeyTip.GetVerticalAlignment(keyTipInformation.AssociatedElement))
                    {
                        case VerticalAlignment.Top:
                            break;
                        case VerticalAlignment.Bottom:
                            y = elementSize.Height - keyTipSize.Height;
                            break;
                        case VerticalAlignment.Center:
                        case VerticalAlignment.Stretch:
                            y = elementSize.Height / 2.0 - keyTipSize.Height / 2.0;
                            break;
                    }

                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(new Point(x, y), this.AdornedElement);

                    #endregion
                }
                else if (keyTipInformation.AssociatedElement.Name == "PART_DialogLauncherButton")
                {
                    // Dialog Launcher Button Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.AssociatedElement.RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(new Point(
                                                                              elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                                                                              0), this.AdornedElement);
                    keyTipInformation.Position = new Point(keyTipInformation.Position.X, rows[3]);
                }
                else if (keyTipInformation.AssociatedElement is InRibbonGallery && !((InRibbonGallery)keyTipInformation.AssociatedElement).IsCollapsed)
                {
                    // InRibbonGallery Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.AssociatedElement.RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(new Point(
                                                                              elementSize.Width - keyTipSize.Width / 2.0,
                                                                              0), this.AdornedElement);
                    keyTipInformation.Position = new Point(keyTipInformation.Position.X, rows[2] - keyTipSize.Height / 2);
                }
                else if (keyTipInformation.AssociatedElement is RibbonTabItem || keyTipInformation.AssociatedElement is Backstage)
                {
                    // Ribbon Tab Item Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.AssociatedElement.RenderSize;
                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(new Point(
                                                                              elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                                                                              elementSize.Height - keyTipSize.Height / 2.0), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement is RibbonGroupBox)
                {
                    // Ribbon Group Box Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.AssociatedElement.DesiredSize;
                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(new Point(
                                                                              elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                                                                              elementSize.Height + 1), this.AdornedElement);
                }
                else if (IsWithinQuickAccessToolbar(keyTipInformation.AssociatedElement))
                {
                    var translatedPoint = keyTipInformation.AssociatedElement.TranslatePoint(new Point(keyTipInformation.AssociatedElement.DesiredSize.Width / 2.0 - keyTipInformation.KeyTip.DesiredSize.Width / 2.0, keyTipInformation.AssociatedElement.DesiredSize.Height - keyTipInformation.KeyTip.DesiredSize.Height / 2.0), this.AdornedElement);
                    keyTipInformation.Position = translatedPoint;
                }
                else if (keyTipInformation.AssociatedElement is MenuItem)
                {
                    // MenuItem Exclusive Placement                    
                    var elementSize = keyTipInformation.AssociatedElement.DesiredSize;
                    keyTipInformation.Position = keyTipInformation.AssociatedElement.TranslatePoint(
                                                                    new Point(
                                                                              elementSize.Height / 3.0 + 2,
                                                                              elementSize.Height / 4.0 + 2), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement.Parent is BackstageTabControl)
                {
                    // Backstage Items Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.AssociatedElement.DesiredSize;
                    var parent = (UIElement)keyTipInformation.AssociatedElement.Parent;
                    var positionInParent = keyTipInformation.AssociatedElement.TranslatePoint(default(Point), parent);
                    keyTipInformation.Position = parent.TranslatePoint(
                                                       new Point(
                                                                 5,
                                                                 positionInParent.Y + (elementSize.Height / 2.0 - keyTipSize.Height)), this.AdornedElement);
                }
                else
                {
                    if ((RibbonProperties.GetSize(keyTipInformation.AssociatedElement) != RibbonControlSize.Large)
                        || keyTipInformation.AssociatedElement is Spinner
                        || keyTipInformation.AssociatedElement is ComboBox
                        || keyTipInformation.AssociatedElement is TextBox
                        || keyTipInformation.AssociatedElement is CheckBox)
                    {
                        var withinRibbonToolbar = IsWithinRibbonToolbarInTwoLine(keyTipInformation.AssociatedElement);
                        var translatedPoint = keyTipInformation.AssociatedElement.TranslatePoint(new Point(keyTipInformation.KeyTip.DesiredSize.Width / 2.0, keyTipInformation.KeyTip.DesiredSize.Height / 2.0), this.AdornedElement);

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

                            translatedPoint.Y = rows[index] - keyTipInformation.KeyTip.DesiredSize.Height / 2.0;
                        }

                        keyTipInformation.Position = translatedPoint;
                    }
                    else
                    {
                        var translatedPoint = keyTipInformation.AssociatedElement.TranslatePoint(new Point(keyTipInformation.AssociatedElement.DesiredSize.Width / 2.0 - keyTipInformation.KeyTip.DesiredSize.Width / 2.0, keyTipInformation.AssociatedElement.DesiredSize.Height - 8), this.AdornedElement);
                        if (rows != null)
                        {
                            translatedPoint.Y = rows[2] - keyTipInformation.KeyTip.DesiredSize.Height / 2.0;
                        }

                        keyTipInformation.Position = translatedPoint;
                    }
                }
            }
        }

        // Determines whether the element is children to RibbonToolBar
        private static bool IsWithinRibbonToolbarInTwoLine(DependencyObject element)
        {
            while (true)
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

                    if (definition.RowCount == 2
                        || definition.Rows.Count == 2)
                    {
                        return true;
                    }

                    return false;
                }

                if (parent == null)
                {
                    return false;
                }

                element = parent;
            }
        }

        // Determines whether the element is children to quick access toolbar
        private static bool IsWithinQuickAccessToolbar(DependencyObject element)
        {
            while (true)
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

                element = parent;
            }
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount => this.keyTipInformations.Count;

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.keyTipInformations[index].KeyTip;
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

            Debug.WriteLine($"[{name}] {string.Format(format, args)}", "KeyTipAdorner");
        }

        #endregion
    }
}