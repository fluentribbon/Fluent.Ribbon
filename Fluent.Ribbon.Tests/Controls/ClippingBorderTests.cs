namespace Fluent.Tests.Controls;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// <see cref="ClippingBorder"/> clips its child by using a <see cref="VisualBrush"/> as <see cref="UIElement.OpacityMask"/>.
/// The visual of that brush is not part of the visual tree and only gets measured/arranged in <see cref="ClippingBorder"/>.ArrangeOverride.
/// If the mask gets created after the last arrange pass the mask visual has a size of 0x0 and the child is completely invisible
/// until something causes a new arrange pass (mouse over, resizing etc.).
/// </summary>
[TestFixture]
public class ClippingBorderTests
{
    private const double Size = 100;

    private static readonly Color childColor = Colors.Red;

    [Test]
    public void OpacityMaskVisualKeepsSizeAfterLayoutUpdated()
    {
        var border = CreateClippingBorder(new CornerRadius(10));

        border.Measure(new Size(Size, Size));
        border.Arrange(new Rect(0, 0, Size, Size));

        var maskVisual = (Border)((VisualBrush)border.Child!.OpacityMask).Visual;

        Assert.That(maskVisual.RenderSize, Is.EqualTo(new Size(Size, Size)), "Size directly after arrange.");

        // Processes the layout queue and raises LayoutUpdated, which is where VisualBrush does its own layout pass for visuals that are not connected to a visual tree.
        border.UpdateLayout();

        Assert.That(maskVisual.RenderSize, Is.EqualTo(new Size(Size, Size)), "Size after UpdateLayout.");
    }

    [Test]
    public void ChildIsVisibleWhenCornerRadiusIsSetBeforeFirstLayout()
    {
        var border = CreateClippingBorder(new CornerRadius(10));

        Layout(border);

        AssertChildIsVisibleAndClipped(border);
    }

    [Test]
    public void ChildIsVisibleWhenCornerRadiusIsSetAfterFirstLayout()
    {
        var border = CreateClippingBorder(default);

        Layout(border);
        AssertChildIsVisible(border);

        border.CornerRadius = new CornerRadius(10);
        border.UpdateLayout();

        AssertChildIsVisibleAndClipped(border);
    }

    [Test]
    public void ChildIsVisibleWhenCornerRadiusIsSetAfterFirstLayoutInWindow()
    {
        var border = CreateClippingBorder(default);

        using var window = new TestRibbonWindow(border);
        UIHelper.DoEvents();
        AssertChildIsVisible(border);

        border.CornerRadius = new CornerRadius(10);
        UIHelper.DoEvents();

        AssertChildIsVisibleAndClipped(border);
    }

    [Test]
    public void ClippingFollowsCornerRadiusChanges()
    {
        var border = CreateClippingBorder(new CornerRadius(5));

        Layout(border);

        // (6,6) is inside the rounded rectangle when the corner radius is 5
        Assert.That(GetPixel(border, 6, 6).A, Is.EqualTo(255));

        border.CornerRadius = new CornerRadius(30);
        border.UpdateLayout();

        AssertOpacityMaskIsInSync(border);

        // (6,6) is outside the rounded rectangle when the corner radius is 30
        Assert.That(GetPixel(border, 6, 6).A, Is.EqualTo(0));
    }

    [Test]
    public void ChildIsVisibleWhenChildIsReplacedAfterFirstLayout()
    {
        var border = CreateClippingBorder(new CornerRadius(10));
        var oldChild = border.Child;

        Layout(border);
        AssertChildIsVisibleAndClipped(border);

        border.Child = new Border { Background = new SolidColorBrush(childColor) };
        border.UpdateLayout();

        AssertChildIsVisibleAndClipped(border);
        Assert.That(oldChild.OpacityMask, Is.Null, "The opacity mask of the old child should be removed.");
    }

    [Test]
    public void ChildIsVisibleWhenSizeChangesAfterFirstLayout()
    {
        var border = CreateClippingBorder(new CornerRadius(10));

        Layout(border);
        AssertChildIsVisibleAndClipped(border);

        border.Width = Size * 2;
        border.Measure(new Size(border.Width, border.Height));
        border.Arrange(new Rect(0, 0, border.Width, border.Height));
        border.UpdateLayout();

        AssertChildIsVisibleAndClipped(border);
        Assert.That(GetPixel(border, (int)(Size * 1.5), (int)(Size / 2)), Is.EqualTo(childColor));
    }

    [Test]
    public void ChildIsVisibleWhenInitiallyCollapsedInWindow()
    {
        var border = CreateClippingBorder(new CornerRadius(10));
        border.Visibility = Visibility.Collapsed;

        using var window = new TestRibbonWindow(border);
        UIHelper.DoEvents();

        border.Visibility = Visibility.Visible;
        UIHelper.DoEvents();

        AssertChildIsVisibleAndClipped(border);
    }

    [Test]
    public void OpacityMaskIsInSyncWhenUsedInControlTemplate()
    {
        var button = new Fluent.Button
        {
            Header = "Test",
            CornerRadius = new CornerRadius(10),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };

        using var window = new TestRibbonWindow(button);
        UIHelper.DoEvents();

        var border = FindVisualChildren<ClippingBorder>(button).Single();

        AssertOpacityMaskIsInSync(border);
    }

    private static ClippingBorder CreateClippingBorder(CornerRadius cornerRadius)
    {
        return new ClippingBorder
        {
            Width = Size,
            Height = Size,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            CornerRadius = cornerRadius,
            Child = new Border { Background = new SolidColorBrush(childColor) }
        };
    }

    private static void Layout(FrameworkElement element)
    {
        element.Measure(new Size(element.Width, element.Height));
        element.Arrange(new Rect(0, 0, element.Width, element.Height));
        element.UpdateLayout();
    }

    private static void AssertChildIsVisible(ClippingBorder border)
    {
        Assert.That(GetPixel(border, (int)(border.RenderSize.Width / 2), (int)(border.RenderSize.Height / 2)), Is.EqualTo(childColor), "Child should be visible.");
    }

    private static void AssertChildIsVisibleAndClipped(ClippingBorder border)
    {
        Assert.Multiple(() =>
        {
            AssertOpacityMaskIsInSync(border);

            AssertChildIsVisible(border);

            Assert.That(GetPixel(border, 0, 0).A, Is.EqualTo(0), "Top left corner should be clipped.");
            Assert.That(GetPixel(border, (int)border.RenderSize.Width - 1, (int)border.RenderSize.Height - 1).A, Is.EqualTo(0), "Bottom right corner should be clipped.");
        });
    }

    private static void AssertOpacityMaskIsInSync(ClippingBorder border)
    {
        var mask = border.Child?.OpacityMask as VisualBrush;

        Assert.That(mask, Is.Not.Null, "Child should have a VisualBrush as opacity mask.");

        var maskVisual = (Border)mask!.Visual;

        Assert.Multiple(() =>
        {
            Assert.That(maskVisual.RenderSize, Is.EqualTo(border.RenderSize), "Size of the opacity mask visual should match the size of the ClippingBorder.");
            Assert.That(maskVisual.CornerRadius, Is.EqualTo(border.CornerRadius), "CornerRadius of the opacity mask visual should match the CornerRadius of the ClippingBorder.");
        });
    }

    private static Color GetPixel(FrameworkElement element, int x, int y)
    {
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(element.RenderSize.Width), (int)Math.Ceiling(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);

        var pixel = new byte[4];
        bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

        // Pbgra32 => B, G, R, A
        return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
    }

    private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild)
            {
                yield return typedChild;
            }

            foreach (var descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }
}
