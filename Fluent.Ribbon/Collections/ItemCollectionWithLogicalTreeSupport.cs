namespace Fluent.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Special collection with support for logical children of a parent object.
    /// </summary>
    /// <typeparam name="TItem">The type for items.</typeparam>
    public class ItemCollectionWithLogicalTreeSupport<TItem> : ObservableCollection<TItem>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parent">The parent which supports logical children.</param>
        public ItemCollectionWithLogicalTreeSupport(ILogicalChildSupport parent)
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// The parent object which support logical children.
        /// </summary>
        public ILogicalChildSupport Parent { get; }

        /// <summary>
        /// Adds all items to the logical tree of <see cref="Parent"/>.
        /// </summary>
        public void AquireLogicalOwnership()
        {
            foreach (var item in this.Items)
            {
                this.Parent.AddLogicalChild(item);
            }
        }

        /// <summary>
        /// Removes all items from the logical tree of <see cref="Parent"/>.
        /// </summary>
        public void ReleaseLogicalOwnership()
        {
            foreach (var item in this.Items)
            {
                this.Parent.RemoveLogicalChild(item);
            }
        }

        /// <summary>
        /// Gets all items where the logical parent is <see cref="Parent"/>.
        /// </summary>
        public IEnumerable<TItem> GetLogicalChildren()
        {
            foreach (var item in this.Items)
            {
                if (item is DependencyObject dependencyObject
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    && ReferenceEquals(LogicalTreeHelper.GetParent(dependencyObject), this.Parent))
                {
                    yield return item;
                }
            }
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);

            this.Parent.AddLogicalChild(item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            this.Parent.RemoveLogicalChild(this[index]);

            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, TItem item)
        {
            var oldItem = this[index];

            if (oldItem != null)
            {
                this.Parent.RemoveLogicalChild(oldItem);
            }

            base.SetItem(index, item);
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (var item in this.Items)
            {
                this.Parent.RemoveLogicalChild(item);
            }

            base.ClearItems();
        }
    }
}