 // ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Fluent.Extensibility;
    using Fluent.Internal;

    /// <summary>
    /// Represents adorner for KeyTips.
    /// KeyTipAdorners is chained to produce one from another.
    /// Detaching root adorner couses detaching all adorners in the chain
    /// </summary>
    public class KeyTipAdorner : Adorner
    {
        #region Events

        /// <summary>
        /// This event is occured when adorner is
        /// detached and is not able to be attached again
        /// </summary>
        public event EventHandler<KeyTipPressedResult> Terminated;

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

        /// <summary>
        /// Returns wether any key tips are visibile.
        /// </summary>
        public bool AreAnyKeyTipsVisible
        {
            get { return this.keyTipInformations.Any(x => x.IsVisible) || (this.childAdorner != null && this.childAdorner.AreAnyKeyTipsVisible); }
        }

        /// <summary>
        /// Gets the currently active <see cref="KeyTipAdorner"/> by following eventually present child adorners.
        /// </summary>
        public KeyTipAdorner ActiveKeyTipAdorner
        {
            get
            {
                return this.childAdorner != null && this.childAdorner.IsAdornerChainAlive
                           ? this.childAdorner.ActiveKeyTipAdorner
                           : this;
            }
        }

        /// <summary>
        /// Gets a copied list of the currently available <see cref="KeyTipInformation"/>.
        /// </summary>
        public IReadOnlyList<KeyTipInformation> KeyTipInformations
        {
            get
            {
                return this.keyTipInformations.ToList();
            }
        }

        #endregion

        #region Intialization

        /// <summary>
        /// Construcotor
        /// </summary>
        /// <param name="adornedElement">Element to adorn.</param>
        /// <param name="parentAdorner">Parent adorner or null.</param>
        /// <param name="keyTipElementContainer">The element which is container for elements.</param>
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
        private void FindKeyTips(FrameworkElement container, bool hide)
        {
            var children = GetVisibleChildren(container);

            foreach (var child in children)
            {
                var groupBox = child as RibbonGroupBox;

                var keys = KeyTip.GetKeys(child);

                if (keys != null || child is IKeyTipInformationProvider)
                {
                    if (groupBox != null)
                    {
                        this.GenerateAndAddGroupBoxKeyTipInformation(hide, keys, child, groupBox);
                    }
                    else
                    {
                        this.GenerateAndAddRegularKeyTipInformations(keys, child, hide);

                        // Do not search deeper in the tree
                        continue;
                    }
                }

                var innerHide = hide || groupBox?.State == RibbonGroupBoxState.Collapsed;
                this.FindKeyTips(child, innerHide);
            }
        }

        private void GenerateAndAddGroupBoxKeyTipInformation(bool hide, string keys, FrameworkElement child, RibbonGroupBox groupBox)
        {
            var keyTipInformation = new KeyTipInformation(keys, child, hide || groupBox.State != RibbonGroupBoxState.Collapsed);

            // Add to list & visual children collections
            this.AddKeyTipInformationElement(keyTipInformation);

            this.LogDebug("Found KeyTipped RibbonGroupBox \"{0}\" with keys \"{1}\".", keyTipInformation.AssociatedElement, keyTipInformation.Keys);
        }

        private void GenerateAndAddRegularKeyTipInformations(string keys, FrameworkElement child, bool hide)
        {
            IEnumerable<KeyTipInformation> informations;

            if (child is IKeyTipInformationProvider keyTipInformationProvider)
            {
                informations = keyTipInformationProvider.GetKeyTipInformations(hide);
            }
            else
            {
                informations = new[] { new KeyTipInformation(keys, child, hide) };
            }

            if (informations != null)
            {
                foreach (var keyTipInformation in informations)
                {
                    // Add to list & visual children collections
                    this.AddKeyTipInformationElement(keyTipInformation);

                    this.LogDebug("Found KeyTipped element \"{0}\" with keys \"{1}\".", keyTipInformation.AssociatedElement, keyTipInformation.Keys);
                }
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

            // Always using the visual tree is very expensive, so we only search through it when we got specific control types.
            // Using the visual tree here, in addition to the logical, partially fixes #244.
            if (element is ContentPresenter
                || element is ContentControl)
            {
                children = children
                    .Concat(UIHelper.GetVisualChildren(element))
                    .OfType<FrameworkElement>();
            }
            else if (element is ItemsControl itemsControl)
            {
                children = children.Concat(UIHelper.GetAllItemContainers<FrameworkElement>(itemsControl));
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

            this.LogDebug("Attach begin {0}", this.Visibility);

            if (this.oneOfAssociatedElements.IsLoaded == false)
            {
                // Delay attaching
                this.LogDebug("Delaying attach");
                this.oneOfAssociatedElements.Loaded += this.OnDelayAttach;
                return;
            }

            this.adornerLayer = GetAdornerLayer(this.oneOfAssociatedElements);

            if (this.adornerLayer == null)
            {
                this.LogDebug("No adorner layer found");
                this.isAttaching = false;
                return;
            }

            this.FilterKeyTips(string.Empty);

            // Show this adorner
            this.adornerLayer.Add(this);

            this.isAttaching = false;
            this.attached = true;

            this.LogDebug("Attach end");
        }

        private void OnDelayAttach(object sender, EventArgs args)
        {
            this.LogDebug("Delay attach (control loaded)");
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

            this.LogDebug("Detach Begin");

            // Maybe adorner awaiting attaching, cancel it
            this.oneOfAssociatedElements.Loaded -= this.OnDelayAttach;

            // Show this adorner
            this.adornerLayer.Remove(this);

            this.attached = false;

            this.LogDebug("Detach End");
        }

        #endregion

        #region Termination

        /// <summary>
        /// Terminate whole key tip's adorner chain
        /// </summary>
        public void Terminate(KeyTipPressedResult keyTipPressedResult)
        {
            if (this.terminated)
            {
                return;
            }

            this.terminated = true;

            this.Detach();

            this.parentAdorner?.Terminate(keyTipPressedResult);

            this.childAdorner?.Terminate(keyTipPressedResult);

            this.Terminated?.Invoke(this, keyTipPressedResult);

            this.LogDebug("Termination");
        }

        #endregion

        #region Static Methods

        private static AdornerLayer GetAdornerLayer(UIElement element)
        {
            var current = element;

            while (true)
            {
                if (current == null)
                {
                    return null;
                }

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

        /// <summary>
        /// Back to the previous adorner.
        /// </summary>
        public void Back()
        {
            this.LogTrace("Invoking back.");

            var control = this.keyTipElementContainer as IKeyTipedControl;
            control?.OnKeyTipBack();

            if (this.parentAdorner != null)
            {
                this.LogDebug("Back");
                this.Detach();
                this.parentAdorner.Attach();
            }
            else
            {
                this.Terminate(KeyTipPressedResult.Empty);
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
            this.LogTrace("Trying to forward keys \"{0}\"...", keys);

            var keyTipInformation = this.TryGetKeyTipInformation(keys);
            if (keyTipInformation == null)
            {
                this.LogTrace("Found no element for keys \"{0}\".", keys);
                return false;
            }

            this.Forward(keys, keyTipInformation.AssociatedElement, click);
            return true;
        }

        // Forward to the next element
        private void Forward(string keys, FrameworkElement element, bool click)
        {
            this.LogTrace("Forwarding keys \"{0}\" to element \"{1}\".", keys, GetControlLogText(element));

            this.Detach();
            var keyTipPressedResult = KeyTipPressedResult.Empty;

            if (click)
            {
                this.LogTrace("Invoking click.");

                var control = element as IKeyTipedControl;
                keyTipPressedResult = control?.OnKeyTipPressed() ?? KeyTipPressedResult.Empty;
            }

            var children = GetVisibleChildren(element);

            if (children.Count == 0)
            {
                this.Terminate(keyTipPressedResult);
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
                this.Terminate(keyTipPressedResult);
                return;
            }

            this.childAdorner.Attach();
        }

        /// <summary>
        /// Gets <see cref="KeyTipInformation"/> by keys.
        /// </summary>
        /// <param name="keys">The keys to look for.</param>
        /// <returns>The <see cref="KeyTipInformation"/> associated with <paramref name="keys"/>.</returns>
        private KeyTipInformation TryGetKeyTipInformation(string keys)
        {
            return this.keyTipInformations.FirstOrDefault(x => x.IsEnabled && x.Visibility == Visibility.Visible && keys.Equals(x.Keys, StringComparison.OrdinalIgnoreCase));
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

                if (content.StartsWith(keys, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Hide / unhide keytips relative matching to entered keys
        internal void FilterKeyTips(string keys)
        {
            this.LogDebug("FilterKeyTips with \"{0}\"", keys);

            // Reset visibility if filter is empty
            if (string.IsNullOrEmpty(keys))
            {
                foreach (var keyTipInformation in this.keyTipInformations)
                {
                    keyTipInformation.Visibility = keyTipInformation.DefaultVisibility;
                }
            }

            // Backup current visibility of key tips
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                keyTipInformation.BackupVisibility = keyTipInformation.Visibility;
            }

            // Hide / unhide keytips relative matching to entered keys
            foreach (var keyTipInformation in this.keyTipInformations)
            {
                var content = keyTipInformation.Keys;

                if (string.IsNullOrEmpty(keys))
                {
                    keyTipInformation.Visibility = keyTipInformation.BackupVisibility;
                }
                else
                {
                    keyTipInformation.Visibility = content.StartsWith(keys, StringComparison.OrdinalIgnoreCase)
                                              ? keyTipInformation.BackupVisibility
                                              : Visibility.Collapsed;
                }
            }

            this.LogDebug("Filtered key tips: {0}", this.keyTipInformations.Count(x => x.Visibility == Visibility.Visible));
        }

        #endregion

        #region Layout & Visual Children

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.LogDebug("ArrangeOverride");

            foreach (var keyTipInformation in this.keyTipInformations)
            {
                keyTipInformation.KeyTip.Arrange(new Rect(keyTipInformation.Position, keyTipInformation.KeyTip.DesiredSize));
            }

            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            this.LogDebug("MeasureOverride");

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

        private void UpdateKeyTipPositions()
        {
            this.LogDebug("UpdateKeyTipPositions");

            if (this.keyTipInformations.Count == 0)
            {
                return;
            }

            double[] rows = null;
            var groupBox = this.oneOfAssociatedElements as RibbonGroupBox ?? UIHelper.GetParent<RibbonGroupBox>(this.oneOfAssociatedElements);
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
                var visualTargetIsVisible = keyTipInformation.VisualTarget.IsVisible;
                var visualTargetInVisualTree = VisualTreeHelper.GetParent(keyTipInformation.VisualTarget) != null;
                keyTipInformation.Visibility = visualTargetIsVisible && visualTargetInVisualTree ? Visibility.Visible : Visibility.Collapsed;

                keyTipInformation.KeyTip.Margin = KeyTip.GetMargin(keyTipInformation.AssociatedElement);

                if (IsWithinQuickAccessToolbar(keyTipInformation.AssociatedElement))
                {
                    var x = (keyTipInformation.VisualTarget.DesiredSize.Width / 2.0) - (keyTipInformation.KeyTip.DesiredSize.Width / 2.0);
                    var y = keyTipInformation.VisualTarget.DesiredSize.Height - (keyTipInformation.KeyTip.DesiredSize.Height / 2.0);

                    if (KeyTip.GetAutoPlacement(keyTipInformation.AssociatedElement) == false)
                    {
                        switch (KeyTip.GetHorizontalAlignment(keyTipInformation.AssociatedElement))
                        {
                            case HorizontalAlignment.Left:
                                x = 0;
                                break;
                            case HorizontalAlignment.Right:
                                x = keyTipInformation.VisualTarget.DesiredSize.Width - keyTipInformation.KeyTip.DesiredSize.Width;
                                break;
                        }
                    }

                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(new Point(x, y), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement.Name == "PART_DialogLauncherButton")
                {
                    // Dialog Launcher Button Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.VisualTarget.RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(new Point(
                                                                                                         (elementSize.Width / 2.0) - (keyTipSize.Width / 2.0),
                                                                                                         0), this.AdornedElement);
                    keyTipInformation.Position = new Point(keyTipInformation.Position.X, rows[3]);
                }
                else if (KeyTip.GetAutoPlacement(keyTipInformation.AssociatedElement) == false)
                {
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;

                    var elementSize = keyTipInformation.VisualTarget.RenderSize;

                    double x = 0, y = 0;

                    switch (KeyTip.GetHorizontalAlignment(keyTipInformation.AssociatedElement))
                    {
                        case HorizontalAlignment.Left:
                            break;
                        case HorizontalAlignment.Right:
                            x = elementSize.Width - keyTipSize.Width;
                            break;
                        case HorizontalAlignment.Center:
                        case HorizontalAlignment.Stretch:
                            x = (elementSize.Width / 2.0) - (keyTipSize.Width / 2.0);
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
                            y = (elementSize.Height / 2.0) - (keyTipSize.Height / 2.0);
                            break;
                    }

                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(new Point(x, y), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement is InRibbonGallery gallery
                         && gallery.IsCollapsed == false)
                {
                    // InRibbonGallery Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.VisualTarget.RenderSize;
                    if (rows == null)
                    {
                        continue;
                    }

                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(new Point(
                                                                              elementSize.Width - (keyTipSize.Width / 2.0),
                                                                              0), this.AdornedElement);
                    keyTipInformation.Position = new Point(keyTipInformation.Position.X, rows[2] - (keyTipSize.Height / 2));
                }
                else if (keyTipInformation.AssociatedElement is RibbonTabItem || keyTipInformation.AssociatedElement is Backstage)
                {
                    // Ribbon Tab Item Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.VisualTarget.RenderSize;
                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(new Point(
                                                                              (elementSize.Width / 2.0) - (keyTipSize.Width / 2.0),
                                                                              elementSize.Height - (keyTipSize.Height / 2.0)), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement is MenuItem)
                {
                    // MenuItem Exclusive Placement
                    var elementSize = keyTipInformation.VisualTarget.DesiredSize;
                    keyTipInformation.Position = keyTipInformation.VisualTarget.TranslatePoint(
                                                                    new Point(
                                                                              (elementSize.Height / 3.0) + 2,
                                                                              (elementSize.Height / 4.0) + 2), this.AdornedElement);
                }
                else if (keyTipInformation.AssociatedElement.Parent is BackstageTabControl)
                {
                    // Backstage Items Exclusive Placement
                    var keyTipSize = keyTipInformation.KeyTip.DesiredSize;
                    var elementSize = keyTipInformation.VisualTarget.DesiredSize;
                    var parent = (UIElement)keyTipInformation.VisualTarget.Parent;
                    var positionInParent = keyTipInformation.VisualTarget.TranslatePoint(default, parent);
                    keyTipInformation.Position = parent.TranslatePoint(
                                                       new Point(
                                                                 5,
                                                                 positionInParent.Y + ((elementSize.Height / 2.0) - keyTipSize.Height)), this.AdornedElement);
                }
                else
                {
                    if (RibbonProperties.GetSize(keyTipInformation.AssociatedElement) != RibbonControlSize.Large
                        || IsTextBoxShapedControl(keyTipInformation.AssociatedElement))
                    {
                        var x = keyTipInformation.KeyTip.DesiredSize.Width / 2.0;
                        var y = keyTipInformation.KeyTip.DesiredSize.Height / 2.0;
                        var point = new Point(x, y);
                        var translatedPoint = keyTipInformation.VisualTarget.TranslatePoint(point, this.AdornedElement);

                        // Snapping to rows if it present
                        SnapToRowsIfPresent(rows, keyTipInformation, translatedPoint);

                        keyTipInformation.Position = translatedPoint;
                    }
                    else
                    {
                        var x = (keyTipInformation.VisualTarget.DesiredSize.Width / 2.0) - (keyTipInformation.KeyTip.DesiredSize.Width / 2.0);
                        var y = keyTipInformation.VisualTarget.DesiredSize.Height - 8;
                        var point = new Point(x, y);
                        var translatedPoint = keyTipInformation.VisualTarget.TranslatePoint(point, this.AdornedElement);

                        // Snapping to rows if it present
                        SnapToRowsIfPresent(rows, keyTipInformation, translatedPoint);

                        keyTipInformation.Position = translatedPoint;
                    }
                }
            }
        }

        private static bool IsTextBoxShapedControl(FrameworkElement element)
        {
            return element is Spinner || element is System.Windows.Controls.ComboBox || element is System.Windows.Controls.TextBox || element is System.Windows.Controls.CheckBox;
        }

        // Determines whether the element is children to RibbonToolBar
        private static bool IsWithinRibbonToolbarInTwoLine(DependencyObject element)
        {
            var ribbonToolBar = UIHelper.GetParent<RibbonToolBar>(element);

            var definition = ribbonToolBar?.GetCurrentLayoutDefinition();
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

        // Determines whether the element is children to quick access toolbar
        private static bool IsWithinQuickAccessToolbar(DependencyObject element)
        {
            return UIHelper.GetParent<QuickAccessToolBar>(element) != null;
        }

        private static void SnapToRowsIfPresent(double[] rows, KeyTipInformation keyTipInformation, Point translatedPoint)
        {
            if (rows == null)
            {
                return;
            }

            var withinRibbonToolbar = IsWithinRibbonToolbarInTwoLine(keyTipInformation.VisualTarget);

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

            translatedPoint.Y = rows[index] - (keyTipInformation.KeyTip.DesiredSize.Height / 2.0);
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => this.keyTipInformations.Count;

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return this.keyTipInformations[index].KeyTip;
        }

        #endregion

        #region Logging

        [Conditional("DEBUG")]
        private void LogDebug(string format, params object[] args)
        {
            var message = this.GetMessageLog(format, args);

            Debug.WriteLine(message, "KeyTipAdorner");
        }

        [Conditional("TRACE")]
        private void LogTrace(string format, params object[] args)
        {
            var message = this.GetMessageLog(format, args);

            Trace.WriteLine(message, "KeyTipAdorner");
        }

        private string GetMessageLog(string format, object[] args)
        {
            var name = GetControlLogText(this.AdornedElement);

            var message = $"[{name}] {string.Format(format, args)}";
            return message;
        }

        private static string GetControlLogText(UIElement control)
        {
            var name = control.GetType().Name;

            if (control is IHeaderedControl headeredControl)
            {
                name += $" ({headeredControl.Header})";
            }

            return name;
        }

        #endregion
    }
}