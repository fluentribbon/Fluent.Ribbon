namespace Fluent.Metro.Behaviours
{
    using System.Diagnostics;
    using System.Windows;
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Enables the use of behaviors in styles
    /// </summary>
    public static class StylizedBehaviors
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for behaviors.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty
            = DependencyProperty.RegisterAttached("Behaviors",
                                                  typeof(StylizedBehaviorCollection),
                                                  typeof(StylizedBehaviors),
                                                  new FrameworkPropertyMetadata(null, OnBehaviorsChanged));

        /// <summary>
        /// Gets the behaviors associated with <paramref name="dpo"/>
        /// </summary>
        public static StylizedBehaviorCollection GetBehaviors(DependencyObject dpo)
        {
            return (StylizedBehaviorCollection)dpo.GetValue(BehaviorsProperty);
        }

        /// <summary>
        /// Sets the behaviors associated with <paramref name="dpo"/>
        /// </summary>
        public static void SetBehaviors(DependencyObject dpo, StylizedBehaviorCollection value)
        {
            dpo.SetValue(BehaviorsProperty, value);
        }

        private static void OnBehaviorsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = dpo as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            if (e.OldValue == e.NewValue)
            {
                return;
            }

            var itemBehaviors = Interaction.GetBehaviors(frameworkElement);

            frameworkElement.Unloaded -= FrameworkElementUnloaded;

            if (e.OldValue is StylizedBehaviorCollection oldBehaviors)
            {
                foreach (var behavior in oldBehaviors)
                {
                    var index = GetIndexOf(itemBehaviors, behavior);
                    if (index >= 0)
                    {
                        itemBehaviors.RemoveAt(index);
                    }
                }
            }

            if (e.NewValue is StylizedBehaviorCollection newBehaviors)
            {
                foreach (var behavior in newBehaviors)
                {
                    var index = GetIndexOf(itemBehaviors, behavior);
                    if (index < 0)
                    {
                        var clone = (Behavior)behavior.Clone();
                        SetOriginalBehavior(clone, behavior);
                        itemBehaviors.Add(clone);
                    }
                }
            }

            if (itemBehaviors.Count > 0)
            {
                frameworkElement.Unloaded += FrameworkElementUnloaded;
            }

            frameworkElement.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private static void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            Debug.WriteLine("Dispatcher.ShutdownStarted");
        }

        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            // BehaviorCollection doesn't call Detach, so we do this
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            var itemBehaviors = Interaction.GetBehaviors(frameworkElement);

            foreach (var behavior in itemBehaviors)
            {
                behavior.Detach();
            }

            frameworkElement.Loaded += FrameworkElementLoaded;
        }

        private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            frameworkElement.Loaded -= FrameworkElementLoaded;
            var itemBehaviors = Interaction.GetBehaviors(frameworkElement);

            foreach (var behavior in itemBehaviors)
            {
                behavior.Attach(frameworkElement);
            }
        }

        private static int GetIndexOf(BehaviorCollection itemBehaviors, Behavior behavior)
        {
            var index = -1;

            var orignalBehavior = GetOriginalBehavior(behavior);

            for (var i = 0; i < itemBehaviors.Count; i++)
            {
                var currentBehavior = itemBehaviors[i];
                if (ReferenceEquals(currentBehavior, behavior)
                    || ReferenceEquals(currentBehavior, orignalBehavior))
                {
                    index = i;
                    break;
                }

                var currentOrignalBehavior = GetOriginalBehavior(currentBehavior);
                if (ReferenceEquals(currentOrignalBehavior, behavior)
                    || ReferenceEquals(currentOrignalBehavior, orignalBehavior))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyProperty OriginalBehaviorProperty
            = DependencyProperty.RegisterAttached("OriginalBehavior",
                                                  typeof(Behavior),
                                                  typeof(StylizedBehaviors),
                                                  new UIPropertyMetadata(null));

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return obj.GetValue(OriginalBehaviorProperty) as Behavior;
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }
    }
}