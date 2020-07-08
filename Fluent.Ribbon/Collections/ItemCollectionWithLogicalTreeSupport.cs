namespace Fluent.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

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
        /// Gets wether this collections parent has logical ownership of the items.
        /// </summary>
        public bool IsOwningItems { get; private set; } = true;

        /// <summary>
        /// The parent object which support logical children.
        /// </summary>
        public ILogicalChildSupport Parent { get; }

        /// <summary>
        /// Adds all items to the logical tree of <see cref="Parent"/>.
        /// </summary>
        public void AquireLogicalOwnership()
        {
            if (this.IsOwningItems)
            {
                return;
            }

            this.IsOwningItems = true;

            foreach (var item in this.Items)
            {
                this.AddLogicalChild(item);
            }
        }

        /// <summary>
        /// Removes all items from the logical tree of <see cref="Parent"/>.
        /// </summary>
        public void ReleaseLogicalOwnership()
        {
            if (this.IsOwningItems == false)
            {
                return;
            }

            foreach (var item in this.Items)
            {
                this.RemoveLogicalChild(item);
            }

            this.IsOwningItems = false;
        }

        /// <summary>
        /// Gets all items where the logical parent is <see cref="Parent"/>.
        /// </summary>
        public IEnumerable<TItem> GetLogicalChildren()
        {
            if (this.IsOwningItems == false)
            {
                return Enumerable.Empty<TItem>();
            }

            return this.Items;
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);

            this.AddLogicalChild(item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            this.RemoveLogicalChild(this[index]);

            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, TItem item)
        {
            var oldItem = this[index];

            if (oldItem != null)
            {
                this.RemoveLogicalChild(oldItem);
            }

            base.SetItem(index, item);

            if (item != null)
            {
                this.AddLogicalChild(item);
            }
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (var item in this.Items)
            {
                this.RemoveLogicalChild(item);
            }

            base.ClearItems();
        }

        private void AddLogicalChild(TItem item)
        {
            if (this.IsOwningItems == false)
            {
                return;
            }

            this.Parent.AddLogicalChild(item);
        }

        private void RemoveLogicalChild(TItem item)
        {
            if (this.IsOwningItems == false)
            {
                return;
            }

            this.Parent.RemoveLogicalChild(item);
        }
    }
}