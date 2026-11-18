namespace Fluent.Tests.Controls;

using System.ComponentModel;
using System.Windows.Data;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// <see cref="Ribbon.SelectedTabIndex"/> and <see cref="Ribbon.SelectedTabItem"/> don't bind TwoWay by default,
/// so <c>SelectedTabIndex="{Binding CurrentTab}"</c> (the usual way for an app to select tabs from its view model) is a OneWay binding.
/// The ribbon kept the two properties in sync with its tab control, and with each other, through their CLR setters (SetValue),
/// which replaced such a binding with a local value: the app could no longer select tabs.
/// The ribbons here turn <see cref="Ribbon.AutomaticStateManagement"/> off, so they don't load a state saved by another test.
/// </summary>
[TestFixture]
public class RibbonSelectedTabOneWayBindingTests
{
    [Test]
    public void Selecting_a_tab_from_the_view_model_keeps_a_OneWay_binding_on_SelectedTabIndex()
    {
        var model = new TabModel { Index = 0 };
        var (ribbon, tabs) = CreateRibbon();
        ribbon.SetBinding(Ribbon.SelectedTabIndexProperty, new Binding(nameof(TabModel.Index)) { Source = model, Mode = BindingMode.OneWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[0]), "Precondition: the view model's tab is selected.");

            model.Index = 2;

            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[2]), "The view model must select the tab.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabIndexProperty), Is.Not.Null, "The app's OneWay binding on SelectedTabIndex was lost.");

            model.Index = 1;
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[1]), "The view model must still select tabs.");
        }
    }

    [Test]
    public void Clicking_a_tab_keeps_a_OneWay_binding_on_SelectedTabIndex()
    {
        var model = new TabModel { Index = 0 };
        var (ribbon, tabs) = CreateRibbon();
        ribbon.SetBinding(Ribbon.SelectedTabIndexProperty, new Binding(nameof(TabModel.Index)) { Source = model, Mode = BindingMode.OneWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);

            // What a click on a tab does (RibbonTabItem.OnMouseLeftButtonDown).
            tabs[2].IsSelected = true;
            UIHelper.DoEvents();

            Assert.That(ribbon.SelectedTabIndex, Is.EqualTo(2), "Precondition: the click must select the tab.");
            Assert.That(model.Index, Is.EqualTo(0), "Precondition: a OneWay binding never writes back to the view model.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabIndexProperty), Is.Not.Null, "The app's OneWay binding on SelectedTabIndex was lost.");

            // The view model still holds 0 (OneWay never wrote back), so set another value first to get a change notification through.
            model.Index = 1;
            model.Index = 0;
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[0]), "The view model must still select tabs after a click.");
        }
    }

    [Test]
    public void Clicking_a_tab_keeps_a_OneWay_binding_on_SelectedTabItem()
    {
        var (ribbon, tabs) = CreateRibbon();
        var model = new TabModel { Tab = tabs[0] };
        ribbon.SetBinding(Ribbon.SelectedTabItemProperty, new Binding(nameof(TabModel.Tab)) { Source = model, Mode = BindingMode.OneWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[0]), "Precondition: the view model's tab is selected.");

            tabs[2].IsSelected = true;
            UIHelper.DoEvents();

            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[2]), "Precondition: the click must select the tab.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabItemProperty), Is.Not.Null, "The app's OneWay binding on SelectedTabItem was lost.");

            model.Tab = tabs[1];
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[1]), "The view model must still select tabs after a click.");
        }
    }

    /// <summary>
    /// Guard: with a TwoWay binding a click must still reach the view model, and the view model must still select tabs.
    /// </summary>
    [Test]
    public void TwoWay_binding_on_SelectedTabIndex_receives_clicks()
    {
        var model = new TabModel { Index = 0 };
        var (ribbon, tabs) = CreateRibbon();
        ribbon.SetBinding(Ribbon.SelectedTabIndexProperty, new Binding(nameof(TabModel.Index)) { Source = model, Mode = BindingMode.TwoWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);

            tabs[2].IsSelected = true;
            UIHelper.DoEvents();

            Assert.That(model.Index, Is.EqualTo(2), "A click must reach the TwoWay source.");

            model.Index = 1;
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[1]), "The view model must still select tabs.");
            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabIndexProperty), Is.Not.Null, "The TwoWay binding must still exist.");
        }
    }

    /// <summary>
    /// Guard: inserting a tab before the selected one moves its index, which the ribbon updates itself.
    /// That update must not replace the app's OneWay binding either.
    /// </summary>
    [Test]
    public void Inserting_a_tab_before_the_selected_tab_keeps_a_OneWay_binding_on_SelectedTabIndex()
    {
        var model = new TabModel { Index = 1 };
        var (ribbon, tabs) = CreateRibbon();
        ribbon.SetBinding(Ribbon.SelectedTabIndexProperty, new Binding(nameof(TabModel.Index)) { Source = model, Mode = BindingMode.OneWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[1]), "Precondition: the view model's tab is selected.");

            ribbon.Tabs.Insert(0, new RibbonTabItem { Header = "Inserted" });
            UIHelper.DoEvents();

            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[1]), "Precondition: the selected tab stays selected.");
            Assert.That(ribbon.SelectedTabIndex, Is.EqualTo(2), "Precondition: the index follows the selected tab.");

            model.Index = 0;

            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabIndexProperty), Is.Not.Null, "The app's OneWay binding on SelectedTabIndex was lost.");
            Assert.That(ribbon.SelectedTabIndex, Is.EqualTo(0), "The view model must still select tabs.");
        }
    }

    /// <summary>
    /// Guard: moving the selected tab moves its index, which the ribbon updates itself.
    /// That update must not replace the app's OneWay binding either.
    /// </summary>
    [Test]
    public void Moving_the_selected_tab_keeps_a_OneWay_binding_on_SelectedTabIndex()
    {
        var model = new TabModel { Index = 0 };
        var (ribbon, tabs) = CreateRibbon();
        ribbon.SetBinding(Ribbon.SelectedTabIndexProperty, new Binding(nameof(TabModel.Index)) { Source = model, Mode = BindingMode.OneWay });

        using (new TestRibbonWindow(ribbon))
        {
            Prepare(ribbon);
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[0]), "Precondition: the view model's tab is selected.");

            ribbon.Tabs.Move(0, 2);
            UIHelper.DoEvents();

            Assert.That(ribbon.SelectedTabItem, Is.SameAs(tabs[0]), "Precondition: the moved tab stays selected.");
            Assert.That(ribbon.SelectedTabIndex, Is.EqualTo(2), "Precondition: the index follows the moved tab.");

            model.Index = 1;

            Assert.That(BindingOperations.GetBindingExpression(ribbon, Ribbon.SelectedTabIndexProperty), Is.Not.Null, "The app's OneWay binding on SelectedTabIndex was lost.");
            Assert.That(ribbon.SelectedTabItem, Is.SameAs(ribbon.Tabs[1]), "The view model must still select tabs.");
        }
    }

    private static (Ribbon Ribbon, RibbonTabItem[] Tabs) CreateRibbon()
    {
        var tabs = new[]
        {
            new RibbonTabItem { Header = "First" },
            new RibbonTabItem { Header = "Second" },
            new RibbonTabItem { Header = "Third" }
        };

        var ribbon = new Ribbon { AutomaticStateManagement = false };

        foreach (var tab in tabs)
        {
            ribbon.Tabs.Add(tab);
        }

        return (ribbon, tabs);
    }

    private static void Prepare(Ribbon ribbon)
    {
        ribbon.ApplyTemplate();
        UIHelper.DoEvents();

        Assert.That(ribbon.TabControl, Is.Not.Null, "Precondition: the ribbon template must create its RibbonTabControl.");
    }

    private sealed class TabModel : INotifyPropertyChanged
    {
        private int index = -1;
        private RibbonTabItem tab;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Index
        {
            get => this.index;
            set
            {
                if (this.index == value)
                {
                    return;
                }

                this.index = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Index)));
            }
        }

        public RibbonTabItem Tab
        {
            get => this.tab;
            set
            {
                if (ReferenceEquals(this.tab, value))
                {
                    return;
                }

                this.tab = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Tab)));
            }
        }
    }
}
