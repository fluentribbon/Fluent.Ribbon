namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using Fluent.Internal;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="Ribbon"/>.
    /// </summary>
    public class RibbonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonAutomationPeer([NotNull] Ribbon owner)
            : base(owner)
        {
            this.OwningRibbon = owner;
        }

        private Ribbon OwningRibbon { get; }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = this.GetLocalizedControlTypeCore();
            }

            return name;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "Ribbon";
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                    return this;

                case PatternInterface.Scroll:
                {
                    ItemsControl? ribbonTabControl = this.OwningRibbon.TabControl;
                    if (ribbonTabControl is not null)
                    {
                        var automationPeer = CreatePeerForElement(ribbonTabControl);
                        if (automationPeer is not null)
                        {
                            return automationPeer.GetPattern(patternInterface);
                        }
                    }

                    break;
                }
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var children = new List<AutomationPeer>();

            // If Ribbon is Collapsed, dont show anything in the UIA tree
            if (this.OwningRibbon.IsCollapsed)
            {
                return children;
            }

            if (this.OwningRibbon.QuickAccessToolBar is not null)
            {
                var automationPeer = CreatePeerForElement(this.OwningRibbon.QuickAccessToolBar);

                if (automationPeer is not null)
                {
                    children.Add(automationPeer);
                }
            }

            if (this.OwningRibbon.Menu is not null)
            {
                var automationPeer = this.CreatePeerForMenu();

                if (automationPeer is not null)
                {
                    children.Add(automationPeer);
                }
            }

            // Directly forward the children from the tab control
            if (this.OwningRibbon.TabControl is not null)
            {
                var automationPeer = CreatePeerForElement(this.OwningRibbon.TabControl);

                if (automationPeer is not null)
                {
                    // Resetting the children cache might call a recursive loop...
                    //automationPeer.ResetChildrenCache();

                    var ribbonTabs = automationPeer.GetChildren();
                    children.AddRange(ribbonTabs);
                    // Resetting the children cache might call a recursive loop...
                    //ribbonTabs.ForEach(x => x.ResetChildrenCache());
                }
            }

            return children;
        }

        /// <inheritdoc/>
        protected override bool IsOffscreenCore()
        {
            return this.OwningRibbon.IsCollapsed
                   || base.IsOffscreenCore();
        }

        #region IExpandCollapseProvider Members

        /// <inheritdoc />
        void IExpandCollapseProvider.Collapse()
        {
            this.OwningRibbon.IsMinimized = true;
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Expand()
        {
            this.OwningRibbon.IsMinimized = false;
        }

        /// <inheritdoc />
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState => this.OwningRibbon.IsMinimized ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                                      oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                                      newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        #endregion

        /// <summary>
        /// Creates the <see cref="AutomationPeer"/> for <see cref="Ribbon.Menu"/>.
        /// </summary>
        protected virtual AutomationPeer? CreatePeerForMenu()
        {
            if (this.OwningRibbon.Menu is null)
            {
                return null;
            }

            var automationPeer = CreatePeerForElement(this.OwningRibbon.Menu);
            if (automationPeer is null)
            {
                var menu = (UIElement?)UIHelper.FindImmediateVisualChild<Backstage>(this.OwningRibbon.Menu, x => x.Visibility == Visibility.Visible) ?? UIHelper.FindImmediateVisualChild<ApplicationMenu>(this.OwningRibbon.Menu, x => x.Visibility == Visibility.Visible);

                if (menu is not null)
                {
                    automationPeer = CreatePeerForElement(menu);
                }
            }

            return automationPeer;
        }
    }
}
