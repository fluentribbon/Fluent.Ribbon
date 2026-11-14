namespace Fluent.Tests.Controls;

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// The display options button next to the tabs has "Expand ribbon" and "Minimize ribbon" menu items.
/// Their IsChecked was bound TwoWay (the default for MenuItem.IsChecked) to <see cref="RibbonTabControl.IsMinimized"/>, so a click wrote it,
/// and the tab control's TwoWay style binding then wrote <see cref="Ribbon.IsMinimized"/> with SetValue, which replaced an app's OneWay binding.
/// Same bug as the tab double-click (<see cref="RibbonTabItemDoubleClickTests"/>), on the fourth way to minimize the ribbon.
/// The ribbons here turn <see cref="Ribbon.AutomaticStateManagement"/> off, so they don't load a state saved by another test.
/// </summary>
[TestFixture]
public class RibbonDisplayOptionsMinimizeTests
{
    [Test]
    public void Minimize_ribbon_menu_item_keeps_a_OneWay_binding_on_IsMinimized()
    {
        var settings = new RibbonSettings();
        var ribbon = CreateRibbon(settings, BindingMode.OneWay);

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);

            Click(GetMenuItem(ribbon, RibbonLocalization.Current.Localization.MinimizeRibbon));

            Assert.That(ribbon.IsMinimized, Is.True, "Precondition: the menu item must minimize the ribbon.");
            Assert.That(ribbon.TabControl!.IsMinimized, Is.True, "Precondition: the menu item must minimize the tabs.");
            Assert.That(settings.IsMinimized, Is.False, "Precondition: a OneWay binding never writes back to the settings.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.IsMinimizedProperty), Is.Not.Null, "The app's OneWay binding on Ribbon.IsMinimized was lost.");

            // The settings still hold false (OneWay never wrote back), so set true first to get a change notification through.
            settings.IsMinimized = true;
            settings.IsMinimized = false;
            Assert.That(ribbon.IsMinimized, Is.False, "The app's settings must still restore the ribbon.");
            Assert.That(ribbon.TabControl.IsMinimized, Is.False, "The app's settings must still restore the tabs.");
        }
    }

    [Test]
    public void Expand_ribbon_menu_item_keeps_a_OneWay_binding_on_IsMinimized()
    {
        var settings = new RibbonSettings { IsMinimized = true };
        var ribbon = CreateRibbon(settings, BindingMode.OneWay);

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);
            Assert.That(ribbon.TabControl!.IsMinimized, Is.True, "Precondition: the app's settings minimize the tabs.");

            Click(GetMenuItem(ribbon, RibbonLocalization.Current.Localization.ExpandRibbon));

            Assert.That(ribbon.IsMinimized, Is.False, "Precondition: the menu item must expand the ribbon.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.IsMinimizedProperty), Is.Not.Null, "The app's OneWay binding on Ribbon.IsMinimized was lost.");

            settings.IsMinimized = false;
            settings.IsMinimized = true;
            Assert.That(ribbon.IsMinimized, Is.True, "The app's settings must still minimize the ribbon.");
            Assert.That(ribbon.TabControl.IsMinimized, Is.True, "The app's settings must still minimize the tabs.");
        }
    }

    /// <summary>
    /// Guard: with a TwoWay binding the menu items must still reach the app's settings, and their check marks must follow the ribbon.
    /// </summary>
    [Test]
    public void Menu_items_reach_a_TwoWay_binding_and_show_the_state()
    {
        var settings = new RibbonSettings();
        var ribbon = CreateRibbon(settings, BindingMode.TwoWay);

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);

            var minimize = GetMenuItem(ribbon, RibbonLocalization.Current.Localization.MinimizeRibbon);
            var expand = GetMenuItem(ribbon, RibbonLocalization.Current.Localization.ExpandRibbon);

            Click(minimize);
            Assert.That(settings.IsMinimized, Is.True, "Minimize must reach the TwoWay source.");
            Assert.That(minimize.IsChecked, Is.True, "Minimize must be checked while the ribbon is minimized.");
            Assert.That(expand.IsChecked, Is.False, "Expand must not be checked while the ribbon is minimized.");

            Click(expand);
            Assert.That(settings.IsMinimized, Is.False, "Expand must reach the TwoWay source.");
            Assert.That(minimize.IsChecked, Is.False, "Minimize must not be checked while the ribbon is expanded.");
            Assert.That(expand.IsChecked, Is.True, "Expand must be checked while the ribbon is expanded.");

            settings.IsMinimized = true;
            Assert.That(minimize.IsChecked, Is.True, "The check marks must follow the app's settings.");
            Assert.That(expand.IsChecked, Is.False, "The check marks must follow the app's settings.");
        }
    }

    /// <summary>
    /// Guard: a <see cref="RibbonTabControl"/> used without a ribbon has no ribbon to change, so the menu items must minimize and restore
    /// the tabs themselves (<see cref="RibbonTabControl.SetIsMinimizedFromUserAction"/> sets <see cref="RibbonTabControl.IsMinimized"/> then),
    /// and their check marks must show the state.
    /// </summary>
    [Test]
    public void Menu_items_minimize_and_restore_tabs_without_a_ribbon()
    {
        var tabControl = new RibbonTabControl { CanMinimize = true };
        tabControl.Items.Add(new RibbonTabItem { Header = "Home" });

        using (new TestRibbonWindow(tabControl))
        {
            tabControl.ApplyTemplate();
            UIHelper.DoEvents();

            var minimize = GetMenuItem(tabControl, RibbonLocalization.Current.Localization.MinimizeRibbon);
            var expand = GetMenuItem(tabControl, RibbonLocalization.Current.Localization.ExpandRibbon);

            Assert.That(tabControl.IsMinimized, Is.False, "Precondition: the tabs are not minimized.");

            Click(minimize);
            Assert.That(tabControl.IsMinimized, Is.True, "Minimize must minimize the tabs.");
            Assert.That(minimize.IsChecked, Is.True, "Minimize must be checked while the tabs are minimized.");
            Assert.That(expand.IsChecked, Is.False, "Expand must not be checked while the tabs are minimized.");

            Click(expand);
            Assert.That(tabControl.IsMinimized, Is.False, "Expand must restore the tabs.");
            Assert.That(minimize.IsChecked, Is.False, "Minimize must not be checked while the tabs are expanded.");
            Assert.That(expand.IsChecked, Is.True, "Expand must be checked while the tabs are expanded.");
        }
    }

    private static Ribbon CreateRibbon(RibbonSettings settings, BindingMode mode)
    {
        var ribbon = new Ribbon { CanMinimize = true, AutomaticStateManagement = false };
        ribbon.Tabs.Add(new RibbonTabItem { Header = "Home" });
        ribbon.SetBinding(Ribbon.IsMinimizedProperty, new Binding(nameof(RibbonSettings.IsMinimized)) { Source = settings, Mode = mode });
        return ribbon;
    }

    private static void Prepare(Ribbon ribbon)
    {
        ribbon.ApplyTemplate();
        UIHelper.DoEvents();

        Assert.That(ribbon.TabControl, Is.Not.Null, "Precondition: the ribbon template must create its RibbonTabControl.");
        ribbon.TabControl!.ApplyTemplate();
    }

    private static System.Windows.Controls.MenuItem GetMenuItem(Ribbon ribbon, string header)
    {
        return GetMenuItem(ribbon.TabControl!, header);
    }

    private static System.Windows.Controls.MenuItem GetMenuItem(RibbonTabControl tabControl, string header)
    {
        var button = tabControl.Template.FindName("PART_DisplayOptionsButton", tabControl) as DropDownButton;
        Assert.That(button, Is.Not.Null, "Precondition: the tab control template must have the display options button.");

        var item = button!.Items.OfType<System.Windows.Controls.MenuItem>().FirstOrDefault(x => Equals(x.Header, header));
        Assert.That(item, Is.Not.Null, $"Precondition: the display options menu must have the item \"{header}\".");

        return item!;
    }

    // What a mouse click on a menu item does: a checkable item toggles IsChecked, then raises Click (MenuItem.OnClickCore, internal).
    private static void Click(System.Windows.Controls.MenuItem item)
    {
        var onClickCore = typeof(System.Windows.Controls.MenuItem).GetMethod("OnClickCore", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(onClickCore, Is.Not.Null, "Precondition: MenuItem.OnClickCore must exist.");

        onClickCore!.Invoke(item, new object[] { true });
        UIHelper.DoEvents();
    }

    private sealed class RibbonSettings : INotifyPropertyChanged
    {
        private bool isMinimized;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsMinimized
        {
            get => this.isMinimized;
            set
            {
                if (this.isMinimized == value)
                {
                    return;
                }

                this.isMinimized = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsMinimized)));
            }
        }
    }
}
