namespace Fluent.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// Synchronizes a target collection with a source collection in a one way fashion.
    /// </summary>
    public class CollectionSyncHelper<TItem>
    {
        /// <summary>
        /// Creates a new instance with <paramref name="source"/> as <see cref="Source"/> and <paramref name="target"/> as <see cref="Target"/>.
        /// </summary>
        public CollectionSyncHelper(ObservableCollection<TItem> source, ObservableCollection<TItem> target)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Target = target ?? throw new ArgumentNullException(nameof(target));

            this.SyncTarget();

            this.Source.CollectionChanged += this.SourceOnCollectionChanged;
        }

        /// <summary>
        /// The source collection.
        /// </summary>
        public ObservableCollection<TItem> Source { get; }

        /// <summary>
        /// The target collection.
        /// </summary>
        public ObservableCollection<TItem> Target { get; }

        /// <summary>
        /// Clears <see cref="Target"/> and then copies all items from <see cref="Source"/> to <see cref="Target"/>.
        /// </summary>
        private void SyncTarget()
        {
            this.Target.Clear();

            foreach (var item in this.Source)
            {
                this.Target.Add(item);
            }
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = (TItem)e.NewItems[i];
                        this.Target.Insert(e.NewStartingIndex + i, item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.Target.Remove((TItem)item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        this.Target.Remove((TItem)item);
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.Target.Add((TItem)item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.SyncTarget();

                    break;
            }
        }
    }
}