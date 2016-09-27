using System.Windows;
using System.Windows.Interactivity;

namespace Fluent.Metro.Behaviours
{
    /// <summary>
    /// Enables the use of behaviors in styles
    /// </summary>
    public class StylizedBehaviors
    {
        private static readonly DependencyProperty OriginalBehaviorProperty = DependencyProperty.RegisterAttached("OriginalBehaviorInternal", typeof(Behavior), typeof(StylizedBehaviors), new PropertyMetadata());

        /// <summary>
        /// Using a DependencyProperty as the backing store for Behaviors.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            "Behaviors",
            typeof(StylizedBehaviorCollection),
            typeof(StylizedBehaviors),
            new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Gets Behaviors for element
        /// </summary>
        public static StylizedBehaviorCollection GetBehaviors(DependencyObject uie)
        {
            return (StylizedBehaviorCollection)uie.GetValue(BehaviorsProperty);
        }

        /// <summary>
        /// Sets Behaviors for element
        /// </summary>
        public static void SetBehaviors(DependencyObject uie, StylizedBehaviorCollection value)
        {
            uie.SetValue(BehaviorsProperty, value);
        }

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return obj.GetValue(OriginalBehaviorProperty) as Behavior;
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

        private static void OnPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var uie = dpo as UIElement;

            if (uie == null)
            {
                return;
            }

            var itemBehaviors = Interaction.GetBehaviors(uie);

            var newBehaviors = e.NewValue as StylizedBehaviorCollection;
            var oldBehaviors = e.OldValue as StylizedBehaviorCollection;

            if (ReferenceEquals(newBehaviors, oldBehaviors))
            {
                return;
            }

            if (oldBehaviors != null)
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

            if (newBehaviors != null)
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
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }
    }
}