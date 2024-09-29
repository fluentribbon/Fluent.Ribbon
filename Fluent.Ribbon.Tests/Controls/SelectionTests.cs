namespace Fluent.Tests.Controls;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

public class SelectionTests
{
    [Test]
    public void TestBackstageTabControl()
    {
        TestSelection<BackstageTabControl, BackstageTabItem>(hasInitialSelection: true);
    }

    [Test]
    public void TestStartScreenTabControl()
    {
        TestSelection<StartScreenTabControl, BackstageTabItem>();
    }

    [Test]
    public void TestGallery()
    {
        TestSelection<Gallery, GalleryItem>();
    }

    [Test]
    public void TestInRibbonGallery()
    {
        TestSelection<InRibbonGallery, GalleryItem>();
    }

    [Test]
    public void TestRibbonTabControl()
    {
        TestSelection<RibbonTabControl, RibbonTabItem>();
    }

    [Test]
    public void TestListBox()
    {
        TestSelection<ListBox, ListBoxItem>();
    }

    private static void TestSelection<TContainer, TItem>(bool hasInitialSelection = false)
        where TContainer : Selector, new()
        where TItem : DependencyObject, new()
    {
        var container = new TContainer();
        var item1 = new TItem();
        var item2 = new TItem();

        using var testWindow = new TestRibbonWindow(container);

        var eventCounter = hasInitialSelection ? -1 : 0;
        object oldItem = null;
        object newItem = null;

        container.SelectionChanged += ControlOnSelectionChanged;

        container.Items.Add(item1);
        container.Items.Add(item2);

        UIHelper.DoEvents();

        Assert.That(container.SelectedItem, Is.Null.Or.EqualTo(item1));
        Assert.That(item1.GetValue(Selector.IsSelectedProperty), Is.False.Or.EqualTo(hasInitialSelection));
        Assert.That(item2.GetValue(Selector.IsSelectedProperty), Is.False);
        Assert.That(eventCounter, Is.EqualTo(0));
        Assert.That(oldItem, Is.Null);
        Assert.That(newItem, Is.Null.Or.EqualTo(item1));

        item2.SetValue(Selector.IsSelectedProperty, true);
        Assert.That(container.SelectedItem, Is.EqualTo(item2));
        Assert.That(item1.GetValue(Selector.IsSelectedProperty), Is.False);
        Assert.That(item2.GetValue(Selector.IsSelectedProperty), Is.True);
        Assert.That(eventCounter, Is.EqualTo(1));
        Assert.That(oldItem, Is.EqualTo(null).Or.EqualTo(item1));
        Assert.That(newItem, Is.EqualTo(item2));

        item1.SetValue(Selector.IsSelectedProperty, true);
        Assert.That(container.SelectedItem, Is.EqualTo(item1));
        Assert.That(item1.GetValue(Selector.IsSelectedProperty), Is.True);
        Assert.That(item2.GetValue(Selector.IsSelectedProperty), Is.False);
        Assert.That(eventCounter, Is.EqualTo(2));
        Assert.That(oldItem, Is.EqualTo(item2));
        Assert.That(newItem, Is.EqualTo(item1));

        item2.SetValue(Selector.IsSelectedProperty, true);
        Assert.That(container.SelectedItem, Is.EqualTo(item2));
        Assert.That(item1.GetValue(Selector.IsSelectedProperty), Is.False);
        Assert.That(item2.GetValue(Selector.IsSelectedProperty), Is.True);
        Assert.That(eventCounter, Is.EqualTo(3));
        Assert.That(oldItem, Is.EqualTo(item1));
        Assert.That(newItem, Is.EqualTo(item2));

        return;

        void ControlOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ++eventCounter;
            oldItem = e.RemovedItems.OfType<object>().FirstOrDefault();
            newItem = e.AddedItems.OfType<object>().FirstOrDefault();
        }
    }
}