namespace Fluent.Metro.Behaviours
{
    using System.Windows.Interactivity;
    using Fluent.Metro.Controls;

    /// <summary>
    /// Encapsulates the use of <see cref="WindowSettings.SaveProperty"/>
    /// </summary>
    public class WindowsSettingBehavior : Behavior<RibbonWindow>
    {
        /// <summary>
        /// Called when behavior is being attached
        /// </summary>
        protected override void OnAttached()
        {
            WindowSettings.SetSave(AssociatedObject, AssociatedObject.SaveWindowPosition);
        }
    }
}