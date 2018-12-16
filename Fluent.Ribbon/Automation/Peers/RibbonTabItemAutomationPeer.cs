namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer wrapper for <see cref="RibbonTabItem"/>.
    /// </summary>
    public class RibbonTabItemAutomationPeer : FrameworkElementAutomationPeer
    {
        private RibbonTabItem OwningTab => (RibbonTabItem)this.Owner;

        //private RibbonTabHeaderDataAutomationPeer HeaderPeer
        //{
        //    get
        //    {
        //        if (this.OwningTab.Header != null)
        //        {
        //            var automationPeer = UIElementAutomationPeer.CreatePeerForElement(this.OwningTab.Header);
        //            if (automationPeer != null)
        //            {
        //                return automationPeer.EventsSource as RibbonTabHeaderDataAutomationPeer;
        //            }
        //        }

        //        return null;
        //    }
        //}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTabItemAutomationPeer([NotNull] RibbonTabItem owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> list = null;
            if (this.OwningTab.IsSelected)
            {
                //list = base.GetChildrenCore();
                list = new List<AutomationPeer>(this.OwningTab.Groups.Count);
                foreach (var @group in this.OwningTab.Groups)
                {
                    var peer = CreatePeerForElement(@group);

                    if (peer != null)
                    {
                        list.Add(peer);
                    }
                }
            }

            //if (HeaderPeer != null)
            //{
            //    if (list == null)
            //    {
            //        list = new List<AutomationPeer>(1);
            //    }

            //    list.Insert(0, HeaderPeer);
            //}

            return list;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override Rect GetBoundingRectangleCore()
        {
            //if (!this.OwningTab.IsSelected && HeaderPeer != null)
            //{
            //    return HeaderPeer.GetBoundingRectangle();
            //}
            Rect boundingRectangleCore = base.GetBoundingRectangleCore();
            //if (HeaderPeer != null)
            //{
            //    boundingRectangleCore.Union(HeaderPeer.GetBoundingRectangle());
            //}
            return boundingRectangleCore;
        }

        /// <inheritdoc />
        //protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        //{
        //    return new RibbonGroupBoxDataAutomationPeer(item, this);
        //}

        //internal override Rect GetVisibleBoundingRectCore()
        //{
        //    if (!this.OwningTab.IsSelected && HeaderPeer != null)
        //    {
        //        return HeaderPeer.GetVisibleBoundingRect();
        //    }
        //    return base.GetVisibleBoundingRectCore();
        //}

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseTabExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.EventsSource?.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseTabSelectionEvents()
        {
            AutomationPeer eventsSource = this.EventsSource;
            if (eventsSource != null)
            {
                if (this.OwningTab.IsSelected)
                {
                    eventsSource.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                }
                else
                {
                    eventsSource.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                }
            }
        }
    }
}