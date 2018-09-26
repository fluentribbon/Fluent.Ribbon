namespace Fluent.Tests.Controls
{
    using System.Collections;
    using System.Linq;
    using System.Windows.Controls;
    using Fluent.Tests.Helper;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;
    using ComboBox = Fluent.ComboBox;
    using TextBox = Fluent.TextBox;

    [TestFixture]
    public class RibbonGroupBoxWrapPanelTests
    {
        private static RibbonGroupBox CreateRibbonGroupBox(bool isSharedSizeScope)
        {
            var ribbonGroupBox = new RibbonGroupBox
                                 {
                                     Header = "Test-Header",
                                     Height = 94
                                 };
            Grid.SetIsSharedSizeScope(ribbonGroupBox, isSharedSizeScope);

            AddControls(ribbonGroupBox.Items);

            return ribbonGroupBox;
        }

        private static void AddControls(IList items)
        {
            items.Add(new TextBox
                      {
                          Header = "First Column (1)"
                      });

            items.Add(new ComboBox
                      {
                          Header = "First Column (2)"
                      });

            items.Add(new Spinner
                      {
                          Header = "First Column Long"
                      });

            items.Add(new ComboBox
                      {
                          Header = "Second Column (1)"
                      });

            items.Add(new ComboBox
                      {
                          Header = "Second Column (2) Long Long"
                      });

            items.Add(new Spinner
                      {
                          Header = "Second Column"
                      });
        }

        private static TextBlock GetHeaderTextBlock(Control control)
        {
            return (TextBlock)control.Template.FindName("headerTextBlock", control);
        }

        [Test]
        public void SharedSizeGroupName_Should_Be_Possible_To_Opt_Out()
        {
            var ribbonGroupBox = CreateRibbonGroupBox(true);

            using (new TestRibbonWindow(ribbonGroupBox))
            {
                var controls = ribbonGroupBox.Items.Cast<Control>().ToList();

                RibbonGroupBoxWrapPanel.SetExcludeFromSharedSize(controls[1], true);
                RibbonGroupBoxWrapPanel.SetExcludeFromSharedSize(controls[5], true);

                UIHelper.DoEvents();

                // First column
                {
                    var columnControls = controls.Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new[]
                                                                                    {
                                                                                        "SharedSizeGroup_Column_1",
                                                                                        null,
                                                                                        "SharedSizeGroup_Column_1"
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                96,
                                                                                83,
                                                                                96
                                                                            }));
                }

                // Second column
                {
                    var columnControls = controls.Skip(3).Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new[]
                                                                                    {
                                                                                        "SharedSizeGroup_Column_2",
                                                                                        "SharedSizeGroup_Column_2",
                                                                                        null
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                160,
                                                                                160,
                                                                                83
                                                                            }));
                }
            }
        }

        [Test]
        public void SharedSizeGroupName_Should_Be_Set_If_RibbonGroupBox_Is_SharedSizeScope()
        {
            var ribbonGroupBox = CreateRibbonGroupBox(true);

            using (new TestRibbonWindow(ribbonGroupBox))
            {
                var controls = ribbonGroupBox.Items.Cast<Control>().ToList();

                // First column
                {
                    var columnControls = controls.Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new[]
                                                                                    {
                                                                                        "SharedSizeGroup_Column_1",
                                                                                        "SharedSizeGroup_Column_1",
                                                                                        "SharedSizeGroup_Column_1"
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                96,
                                                                                96,
                                                                                96
                                                                            }));
                }

                // Second column
                {
                    var columnControls = controls.Skip(3).Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new[]
                                                                                    {
                                                                                        "SharedSizeGroup_Column_2",
                                                                                        "SharedSizeGroup_Column_2",
                                                                                        "SharedSizeGroup_Column_2"
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                160,
                                                                                160,
                                                                                160
                                                                            }));
                }
            }
        }

        [Test]
        public void SharedSizeGroupName_Should_Not_Be_Set_If_RibbonGroupBox_Is_Not_SharedSizeScope()
        {
            var ribbonGroupBox = CreateRibbonGroupBox(false);

            using (new TestRibbonWindow(ribbonGroupBox))
            {
                var controls = ribbonGroupBox.Items.Cast<Control>().ToList();

                // First column
                {
                    var columnControls = controls.Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new string[]
                                                                                    {
                                                                                        null,
                                                                                        null,
                                                                                        null
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                83,
                                                                                83,
                                                                                96
                                                                            }));
                }

                // Second column
                {
                    var columnControls = controls.Skip(3).Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new string[]
                                                                                    {
                                                                                        null,
                                                                                        null,
                                                                                        null
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                100,
                                                                                160,
                                                                                83
                                                                            }));
                }
            }
        }

        /// <summary>
        ///     Test for not working because we have to test for <see cref="Grid.IsSharedSizeScopeProperty" /> on parent
        ///     <see cref="RibbonGroupBox" />.
        /// </summary>
        [Test]
        public void SharedSizeGroupName_Should_Not_Work_Without_RibbonGroupBox()
        {
            var ribbonGroupBoxWrapPanel = new RibbonGroupBoxWrapPanel
                                          {
                                              Height = 94
                                          };

            AddControls(ribbonGroupBoxWrapPanel.Children);

            using (new TestRibbonWindow(ribbonGroupBoxWrapPanel))
            {
                var controls = ribbonGroupBoxWrapPanel.Children.Cast<Control>().ToList();

                // First column
                {
                    var columnControls = controls.Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new string[]
                                                                                    {
                                                                                        null,
                                                                                        null,
                                                                                        null
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                83,
                                                                                83,
                                                                                96
                                                                            }));
                }

                // Second column
                {
                    var columnControls = controls.Skip(3).Take(3).ToList();

                    var columnControlsSharedSizeGroupNames = columnControls.Select(RibbonGroupBoxWrapPanel.GetSharedSizeGroupName);

                    Assert.That(columnControlsSharedSizeGroupNames, Is.EquivalentTo(new string[]
                                                                                    {
                                                                                        null,
                                                                                        null,
                                                                                        null
                                                                                    }));

                    var columnControlsHeaderWidths = columnControls.Select(x => (int)GetHeaderTextBlock(x).ActualWidth);

                    Assert.That(columnControlsHeaderWidths, Is.EquivalentTo(new[]
                                                                            {
                                                                                100,
                                                                                160,
                                                                                83
                                                                            }));
                }
            }
        }
    }
}