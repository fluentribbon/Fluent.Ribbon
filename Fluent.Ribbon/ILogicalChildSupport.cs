namespace Fluent
{
    /// <summary>
    /// Adds support for forwarding AddLogicalChild and RemoveLogicalChild.
    /// </summary>
    public interface ILogicalChildSupport
    {
        /// <summary>Adds the provided object to the logical tree of this element. </summary>
        /// <param name="child">Child element to be added.</param>
        void AddLogicalChild(object child);

        /// <summary>
        ///     Removes the provided object from this element's logical tree. <see cref="T:System.Windows.FrameworkElement" />
        ///     updates the affected logical tree parent pointers to keep in sync with this deletion.
        /// </summary>
        /// <param name="child">The element to remove.</param>
        void RemoveLogicalChild(object child);
    }
}