namespace Fluent.Metro.Behaviours
{
    using System.Windows.Interactivity;
    using Fluent.Metro.Controls;

    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        protected override void OnAttached()
        {
            WindowSettings.SetSave(AssociatedObject, AssociatedObject.SaveWindowPosition);
        }
    }
}