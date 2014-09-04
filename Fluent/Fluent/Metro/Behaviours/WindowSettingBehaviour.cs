namespace Fluent.Metro.Behaviours
{
    using System.Windows.Interactivity;
    using Fluent.Metro.Controls;

    public class WindowsSettingBehaviour : Behavior<RibbonWindow>
    {
        protected override void OnAttached()
        {
            WindowSettings.SetSave(AssociatedObject, AssociatedObject.SaveWindowPosition);
        }
    }
}