namespace Fluent.Tests.Themes;

#if NET462
#else
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Windows.Media;
using ControlzEx.Theming;
using Fluent.Helpers.ColorHelpers;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

[TestFixture]
[Explicit]
public class UpdateGeneratorParametersTest
{
    [Test]
    public void UpdatePalette()
    {
        var file = @"C:\DEV\OSS_Own\FluentRibbon\Fluent.Ribbon\Fluent.Ribbon\Themes\Themes\GeneratorParameters.json";
        var output = @"C:\DEV\OSS_Own\FluentRibbon\Fluent.Ribbon\Fluent.Ribbon\Themes\Themes\GeneratorParameters_new.json";

        var generatorParameters = JsonSerializer.Deserialize<ThemeGenerator.ThemeGeneratorParameters>(File.ReadAllText(file, Encoding.UTF8));

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { ExcludeEmptyStrings } }
        };

#pragma warning disable CS0618 // Type or member is obsolete
        var colorPalette = new ColorPalette(Colors.White);
#pragma warning restore CS0618 // Type or member is obsolete

        foreach (var colorScheme in generatorParameters.ColorSchemes)
        {
            var accentColor = (Color)ColorConverter.ConvertFromString(colorScheme.Values["Fluent.Ribbon.Colors.AccentBase"]);
            colorPalette.BaseColor = accentColor;
            colorPalette.UpdatePaletteColors();

            colorScheme.Values["AccentLight1"] = colorPalette.Palette[4].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentLight1.Foreground"] = colorPalette.Palette[4].ContrastColor.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentLight2"] = colorPalette.Palette[3].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentLight2.Foreground"] = colorPalette.Palette[3].ContrastColor.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentLight3"] = colorPalette.Palette[2].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentLight3.Foreground"] = colorPalette.Palette[2].ContrastColor.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark1"] = colorPalette.Palette[6].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark1.Foreground"] = colorPalette.Palette[6].ContrastColor.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark2"] = colorPalette.Palette[7].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark2.Foreground"] = colorPalette.Palette[7].ContrastColor.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark3"] = colorPalette.Palette[8].Color.ToString(CultureInfo.InvariantCulture);
            colorScheme.Values["AccentDark3.Foreground"] = colorPalette.Palette[8].ContrastColor.ToString(CultureInfo.InvariantCulture);
        }

        File.WriteAllText(output, JsonSerializer.Serialize(generatorParameters, options), Encoding.UTF8);
        return;

        static void ExcludeEmptyStrings(JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            foreach (var jsonPropertyInfo in jsonTypeInfo.Properties)
            {
                if (jsonPropertyInfo.PropertyType == typeof(string))
                {
                    jsonPropertyInfo.ShouldSerialize = static (obj, value) => string.IsNullOrEmpty((string)value) is false;
                }
            }
        }
    }
}
#endif