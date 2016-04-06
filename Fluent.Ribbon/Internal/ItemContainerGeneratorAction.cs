namespace Fluent.Internal
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Helper class used to queue action for completion or items changes of <see cref="ItemContainerGenerator"/>
    /// </summary>
    internal class ItemContainerGeneratorAction
    {
        /// <summary>
        /// Creates a new instance used to queue action for completion or items changes of <see cref="ItemContainerGenerator"/>
        /// </summary>
        /// <param name="generator">The <see cref="ItemContainerGenerator"/> to be used.</param>
        /// <param name="action">The <see cref="System.Action"/> that should be invoked.</param>
        public ItemContainerGeneratorAction(ItemContainerGenerator generator, Action action)
        {
            this.Generator = generator;
            this.Action = action;
        }

        /// <summary>
        /// Gets the <see cref="ItemContainerGenerator"/> to be used.
        /// </summary>
        public ItemContainerGenerator Generator { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Action"/> that should be invoked.
        /// </summary>
        public Action Action { get; private set; }

        /// <summary>
        /// Gets the current wait state. <c>true</c> in case <see cref="QueueAction"/> was called and we are waiting for the <see cref="Generator"/> to finish.
        /// </summary>
        public bool IsWaitingForGenerator { get; private set; }

        /// <summary>
        /// Queues <see cref="Action"/> for invocation.
        /// </summary>
        public void QueueAction()
        {
            if (this.Generator.Status != GeneratorStatus.ContainersGenerated)
            {
                if (this.IsWaitingForGenerator)
                {
                    return;
                }

                this.IsWaitingForGenerator = true;
                this.Generator.StatusChanged += this.HandleItemContainerGenerator_StatusChanged;
                return;
            }

            this.IsWaitingForGenerator = false;
            this.Generator.StatusChanged -= this.HandleItemContainerGenerator_StatusChanged;

            this.Action();
        }

        private void HandleItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            this.QueueAction();
        }
    }
}