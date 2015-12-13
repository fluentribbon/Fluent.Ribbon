namespace Fluent.Metro.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    /// Just a <see cref="FreezableCollection{T}"/> for <see cref="Behavior"/>
    /// </summary>
    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:System.Windows.FreezableCollection`1"/>.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }
    }
}