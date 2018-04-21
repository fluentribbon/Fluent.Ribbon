namespace Fluent.Metro.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    /// <summary>
    /// Just a <see cref="FreezableCollection{T}"/> for <see cref="Behavior"/>
    /// </summary>
    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }
    }
}