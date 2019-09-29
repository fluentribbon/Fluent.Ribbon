namespace Fluent.Tests.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using Fluent.Tests.Helper;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonGroupBoxTests
    {
        [Test]
        public void Size_Should_Change_On_Group_State_Change_When_Items_Are_Bound()
        {
            var items = new List<ItemViewModel>
                        {
                            new ItemViewModel()
                        };

            var ribbonGroupBox = new RibbonGroupBox
                           {
                               ItemsSource = items,
                               ItemTemplate = CreateDataTemplateForItemViewModel()
                           };

            using (new TestRibbonWindow(ribbonGroupBox))
            {
                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Small;
                    UIHelper.DoEvents();

                    Assert.That(items.First().ControlSize, Is.EqualTo(RibbonControlSize.Small));
                }

                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Middle;
                    UIHelper.DoEvents();

                    Assert.That(items.First().ControlSize, Is.EqualTo(RibbonControlSize.Middle));
                }

                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Large;
                    UIHelper.DoEvents();

                    Assert.That(items.First().ControlSize, Is.EqualTo(RibbonControlSize.Large));
                }
            }
        }

        [Test]
        public void Size_Should_Change_On_Group_State_Change_When_Items_Are_Ribbon_Controls()
        {
            var ribbonGroupBox = new RibbonGroupBox();

            ribbonGroupBox.Items.Add(new Fluent.Button());

            using (new TestRibbonWindow(ribbonGroupBox))
            {
                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Small;
                    UIHelper.DoEvents();

                    Assert.That(ribbonGroupBox.Items.OfType<Fluent.Button>().First().Size, Is.EqualTo(RibbonControlSize.Small));
                }

                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Middle;
                    UIHelper.DoEvents();

                    Assert.That(ribbonGroupBox.Items.OfType<Fluent.Button>().First().Size, Is.EqualTo(RibbonControlSize.Middle));
                }

                {
                    ribbonGroupBox.State = RibbonGroupBoxState.Large;
                    UIHelper.DoEvents();

                    Assert.That(ribbonGroupBox.Items.OfType<Fluent.Button>().First().Size, Is.EqualTo(RibbonControlSize.Large));
                }
            }
        }

        private static DataTemplate CreateDataTemplateForItemViewModel()
        {
            var dataTemplate = new DataTemplate(typeof(ItemViewModel));

            var factory = new FrameworkElementFactory(typeof(Fluent.Button));
            factory.SetBinding(RibbonProperties.SizeProperty, new Binding(nameof(ItemViewModel.ControlSize)) { Mode = BindingMode.TwoWay });

            //set the visual tree of the data template
            dataTemplate.VisualTree = factory;

            return dataTemplate;
        }
    }

    public class ItemViewModel
    {
        public RibbonControlSize ControlSize { get; set; }
    }
}