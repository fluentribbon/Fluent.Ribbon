﻿// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for dropdown button.
    /// </summary>
    public class FluentButtonAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The class's owner.</param>
        public FluentButtonAutomationPeer([NotNull] FrameworkElement owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "FluentButton";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            return ((IHeaderedControl)this.Owner).Header?.ToString();
        }
    }
}
