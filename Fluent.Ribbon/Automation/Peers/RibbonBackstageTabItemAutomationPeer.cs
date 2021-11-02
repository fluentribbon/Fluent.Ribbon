namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="BackstageTabItem"/>.
    /// </summary>
    public class RibbonBackstageTabItemAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonBackstageTabItemAutomationPeer([NotNull] BackstageTabItem owner)
            : base(owner)
        {
            this.OwningBackstageTabItem = owner;
        }

        private BackstageTabItem OwningBackstageTabItem { get; }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TabItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string? GetNameCore()
        {
            var name = AutomationProperties.GetName(this.Owner);

            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as IHeaderedControl)?.Header as string;
            }

            return name;
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var children = GetHeaderChildren() ?? new List<AutomationPeer>();

            if (this.OwningBackstageTabItem.IsSelected == false)
            {
                return children;
            }

            if (this.OwningBackstageTabItem.TabControlParent?.SelectedContentHost is not null)
            {
                var contentHostPeer = new FrameworkElementAutomationPeer(this.OwningBackstageTabItem.TabControlParent.SelectedContentHost);
                var contentChildren = contentHostPeer.GetChildren();

                if (contentChildren is not null)
                {
                    children.AddRange(contentChildren);
                }
            }

            return children;

            List<AutomationPeer>? GetHeaderChildren()
            {
                if (this.OwningBackstageTabItem.Header is string)
                {
                    return null;
                }

                if (this.OwningBackstageTabItem.HeaderContentHost is not null)
                {
                    return new FrameworkElementAutomationPeer(this.OwningBackstageTabItem.HeaderContentHost).GetChildren();
                }

                return null;
            }
        }
    }
}