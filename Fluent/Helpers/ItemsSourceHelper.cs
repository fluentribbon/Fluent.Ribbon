using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Fluent
{
    public static class ItemsSourceHelper
    {

        public static void ItemsSourceChanged<T>(ObservableCollection<T> collection, DataTemplate dataTemplate, DependencyPropertyChangedEventArgs e,
            NotifyCollectionChangedEventHandler collectionChangedHandler = null)
            where T : DependencyObject
        {
            IEnumerable oldViewModelCollection = (IEnumerable)e.OldValue;
            IEnumerable newViewModelCollection = (IEnumerable)e.NewValue;

            if (collectionChangedHandler == null)
            {
                collectionChangedHandler = (sender, args) => { ItemsSource_CollectionChanged(collection, dataTemplate, sender, args); };
            }

            if (oldViewModelCollection != null)
            {
                if (oldViewModelCollection is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)oldViewModelCollection).CollectionChanged -= collectionChangedHandler;
                collection.Clear();
            }
            if (newViewModelCollection != null)
            {
                if (newViewModelCollection is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)newViewModelCollection).CollectionChanged += collectionChangedHandler;
                ResetCollection<T>(collection, dataTemplate, newViewModelCollection);
            }
        }

        private static void ResetCollection<T>(ObservableCollection<T> collection, DataTemplate dataTemplate, IEnumerable viewModelCollection)
            where T : DependencyObject
        {
            collection.Clear();
            foreach (var item in viewModelCollection)
            {
                if (dataTemplate != null)
                {
                    T dObject = dataTemplate.LoadContent() as T;
                    if (dObject is FrameworkElement)
                    {
                        ((FrameworkElement)(DependencyObject)dObject).DataContext = item;
                    }
                    collection.Add(dObject);
                }
                else if (item is T) //make sure a converter is used in the binding to do the translation
                {
                    collection.Add((T)item);
                }                
            }
        }

        public static void ItemsSource_CollectionChanged<T>(ObservableCollection<T> collection, DataTemplate dataTemplate, object sender, NotifyCollectionChangedEventArgs e)
            where T : DependencyObject
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            if (dataTemplate != null)
                            {
                                T dObject = dataTemplate.LoadContent() as T;
                                if (dObject is FrameworkElement)
                                {
                                    ((FrameworkElement)(DependencyObject)dObject).DataContext = e.NewItems[i];
                                }
                                collection.Add(dObject);
                            }
                            else if (e.NewItems[i] is T) //make sure a converter is used in the binding to do the translation
                            {
                                collection.Add((T)e.NewItems[i]); 
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var dObject = collection[e.OldStartingIndex];
                            collection.RemoveAt(e.OldStartingIndex);
                            collection.Insert(e.NewStartingIndex + i, dObject);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            collection.RemoveAt(e.OldStartingIndex);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var dObject = collection[e.OldStartingIndex];
                            collection.RemoveAt(e.OldStartingIndex);
                        }

                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            if (dataTemplate != null)
                            {
                                T dObject = dataTemplate.LoadContent() as T;
                                if (dObject is FrameworkElement)
                                {
                                    ((FrameworkElement)(DependencyObject)dObject).DataContext = e.NewItems[i];
                                }
                                collection.Insert(e.NewStartingIndex + i, dObject);
                            }
                            else if (e.NewItems[i] is T) //make sure a converter is used in the binding to do the translation
                            {
                                collection.Add((T)e.NewItems[i]);
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        ResetCollection<T>(collection, dataTemplate, sender as IEnumerable);
                        break;
                    }
            }
        }

        public static int SelectorItemsSource_CollectionChanged<T>(ObservableCollection<T> collection, DataTemplate dataTemplate, object sender, NotifyCollectionChangedEventArgs e, int selectedIndex)
            where T : DependencyObject
        {
            int currentIndex = selectedIndex;
            T currentSelected = collection[currentIndex];

            ItemsSource_CollectionChanged(collection, dataTemplate, sender, e);

            if (collection.Count == 0)
                selectedIndex = -1;
            else if (collection.Count - 1 >= currentIndex && currentIndex >= 0 && collection[currentIndex] == currentSelected)
                selectedIndex = currentIndex;
            else if (collection.Contains(currentSelected))
                selectedIndex = collection.IndexOf(currentSelected);
            else
                selectedIndex = 0;
            return selectedIndex;
        }

    }
}
