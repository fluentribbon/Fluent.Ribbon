namespace Fluent.Tests.Controls;

using System.Linq;
using System.Windows;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// Each filter of a gallery gets a menu item in the filter drop down, with the filter as its Tag.
/// The gallery must find the menu item of a filter by that filter, not by its title:
/// two filters can have the same title, and a filter without a title must not crash the gallery.
/// </summary>
[TestFixture]
public class GalleryFilterMenuItemTests
{
    [TestCase(typeof(Gallery))]
    [TestCase(typeof(InRibbonGallery))]
    public void Selecting_a_filter_checks_its_own_menu_item_when_titles_are_the_same(System.Type galleryType)
    {
        var first = new GalleryGroupFilter { Title = "Same" };
        var second = new GalleryGroupFilter { Title = "Same" };
        var gallery = CreateGallery(galleryType, first, second);

        using (new TestRibbonWindow(gallery))
        {
            var menuButton = GetFilterMenuButton(gallery);

            SetSelectedFilter(gallery, second);

            Assert.That(MenuItemOf(menuButton, second).IsChecked, Is.True, "The selected filter's menu item must be checked.");
            Assert.That(MenuItemOf(menuButton, first).IsChecked, Is.False, "The other filter's menu item must not be checked.");

            SetSelectedFilter(gallery, first);

            Assert.That(MenuItemOf(menuButton, first).IsChecked, Is.True, "The selected filter's menu item must be checked.");
            Assert.That(MenuItemOf(menuButton, second).IsChecked, Is.False, "The previously selected filter's menu item must be unchecked.");
        }
    }

    [TestCase(typeof(Gallery))]
    [TestCase(typeof(InRibbonGallery))]
    public void Removing_a_filter_removes_its_own_menu_item_when_titles_are_the_same(System.Type galleryType)
    {
        var first = new GalleryGroupFilter { Title = "Same" };
        var second = new GalleryGroupFilter { Title = "Same" };
        var gallery = CreateGallery(galleryType, first, second);

        using (new TestRibbonWindow(gallery))
        {
            var menuButton = GetFilterMenuButton(gallery);

            Filters(gallery).Remove(second);

            Assert.That(menuButton.Items.OfType<MenuItem>().Select(x => x.Tag), Is.EqualTo(new[] { first }), "Only the removed filter's menu item must be removed.");
        }
    }

    [TestCase(typeof(Gallery))]
    [TestCase(typeof(InRibbonGallery))]
    public void A_filter_without_a_title_can_be_selected_and_removed(System.Type galleryType)
    {
        var untitled = new GalleryGroupFilter();
        var titled = new GalleryGroupFilter { Title = "Titled" };
        var gallery = CreateGallery(galleryType, untitled, titled);

        using (new TestRibbonWindow(gallery))
        {
            var menuButton = GetFilterMenuButton(gallery);

            SetSelectedFilter(gallery, titled);
            SetSelectedFilter(gallery, untitled);

            Assert.That(MenuItemOf(menuButton, untitled).IsChecked, Is.True, "The untitled filter's menu item must be checked.");
            Assert.That(MenuItemOf(menuButton, titled).IsChecked, Is.False);

            Filters(gallery).Remove(untitled);

            Assert.That(menuButton.Items.OfType<MenuItem>().Select(x => x.Tag), Is.EqualTo(new[] { titled }), "The untitled filter's menu item must be removed.");
        }
    }

    /// <summary>
    /// A filter renamed after its menu item was created is still found.
    /// </summary>
    [TestCase(typeof(Gallery))]
    [TestCase(typeof(InRibbonGallery))]
    public void A_renamed_filter_is_still_found(System.Type galleryType)
    {
        var first = new GalleryGroupFilter { Title = "First" };
        var second = new GalleryGroupFilter { Title = "Second" };
        var gallery = CreateGallery(galleryType, first, second);

        using (new TestRibbonWindow(gallery))
        {
            var menuButton = GetFilterMenuButton(gallery);

            second.Title = "Renamed";
            SetSelectedFilter(gallery, second);

            Assert.That(MenuItemOf(menuButton, second).IsChecked, Is.True, "The renamed filter's menu item must be checked.");
        }
    }

    private static FrameworkElement CreateGallery(System.Type galleryType, params GalleryGroupFilter[] filters)
    {
        var gallery = (FrameworkElement)System.Activator.CreateInstance(galleryType);

        foreach (var filter in filters)
        {
            Filters(gallery).Add(filter);
        }

        return gallery;
    }

    private static System.Collections.ObjectModel.ObservableCollection<GalleryGroupFilter> Filters(FrameworkElement gallery)
    {
        return gallery is Gallery g ? g.Filters : ((InRibbonGallery)gallery).Filters;
    }

    private static void SetSelectedFilter(FrameworkElement gallery, GalleryGroupFilter filter)
    {
        if (gallery is Gallery g)
        {
            g.SelectedFilter = filter;
        }
        else
        {
            ((InRibbonGallery)gallery).SelectedFilter = filter;
        }
    }

    private static DropDownButton GetFilterMenuButton(FrameworkElement gallery)
    {
        UIHelper.DoEvents();

        var partName = gallery is Gallery ? "PART_DropDownButton" : "PART_FilterDropDownButton";
        var menuButton = (DropDownButton)((System.Windows.Controls.Control)gallery).Template.FindName(partName, gallery);

        Assert.That(menuButton, Is.Not.Null, "Precondition: the filter drop down button must exist in the template.");
        Assert.That(menuButton.Items.Count, Is.EqualTo(Filters(gallery).Count), "Precondition: one menu item per filter.");

        return menuButton;
    }

    private static MenuItem MenuItemOf(DropDownButton menuButton, GalleryGroupFilter filter)
    {
        var item = menuButton.Items.OfType<MenuItem>().SingleOrDefault(x => ReferenceEquals(x.Tag, filter));

        Assert.That(item, Is.Not.Null, "Precondition: the filter must have a menu item.");

        return item;
    }
}
