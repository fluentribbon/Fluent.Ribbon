namespace Fluent.Tests.Converters
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using Fluent.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectToImageConverterTests
    {
        [Test]
        public void TestDynamicResource()
        {
            var fluentRibbonImagesApplicationmenuResourceKey = (object)"Fluent.Ribbon.Images.ApplicationMenu";

            var expressionType = typeof(ResourceReferenceExpressionConverter).Assembly.GetType("System.Windows.ResourceReferenceExpression");

            var expression = Activator.CreateInstance(expressionType, fluentRibbonImagesApplicationmenuResourceKey);

            var convertedValue = StaticConverters.ObjectToImageConverter.Convert(new object[]
                                                                        {
                                                                            expression, // value to convert
                                                                            new ApplicationMenu() // target visual
                                                                        }, null, null, null);

            Assert.That(convertedValue, Is.Not.Null);
            Assert.That(convertedValue, Is.InstanceOf<Image>());

            var convertedImageValue = (Image)convertedValue;
            Assert.That(convertedImageValue.Source, Is.InstanceOf<DrawingImage>());

            var drawingImage = (DrawingImage)convertedImageValue.Source;
            Assert.That(drawingImage.Drawing, Is.InstanceOf<DrawingGroup>());

            var drawingGroup = (DrawingGroup)drawingImage.Drawing;

            Assert.That(drawingGroup.Children.Cast<GeometryDrawing>().Select(x => x.Geometry.ToString()),
                        Is.EquivalentTo(((DrawingGroup)((DrawingImage)Application.Current.FindResource(fluentRibbonImagesApplicationmenuResourceKey)).Drawing).Children.Cast<GeometryDrawing>().Select(x => x.Geometry.ToString())));
        }
    }
}