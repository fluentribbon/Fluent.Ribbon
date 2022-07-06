using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Nuke.Common.IO;

public class ResourceKeys
{
    public ResourceKeys(params string[] sourceFiles)
    {
        this.SourceFiles = sourceFiles;

        this.PeekedKeys = sourceFiles.SelectMany(x => XmlTasks.XmlPeekElements(x, "//*[@x:Key]", ("x", "http://schemas.microsoft.com/winfx/2006/xaml")))
            .ToList();

        this.XKey = XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml");

        this.ElementsWithNonTypeKeys = this.PeekedKeys
            .Where(x => x.HasAttributes && x.Attribute(this.XKey) is not null)
            .Select(GetElementAndKey)
            // Exclude type-keyed styles like x:Key="{x:Type Button}" etc.
            .Where(x => x.Key.StartsWith("{") == false)
            .ToList();

        (XElement Element, string Key) GetElementAndKey(XElement element)
        {
            return (element, element.Attribute(this.XKey)!.Value);
        }
    }

    public IReadOnlyList<string> SourceFiles { get; }

    public IReadOnlyList<XElement> PeekedKeys { get; }

    public XName XKey { get; }

    public IReadOnlyList<(XElement Element, string Key)> ElementsWithNonTypeKeys { get; }

    public IEnumerable<(string Key, string expectedKey)> CheckKeys()
    {
        foreach (var elementWithNonTypeKey in this.ElementsWithNonTypeKeys)
        {
            var requiredPrefix = GetFullPrefix(elementWithNonTypeKey.Element.Name);

            if (elementWithNonTypeKey.Key.StartsWith(requiredPrefix, StringComparison.Ordinal)
                || elementWithNonTypeKey.Key.StartsWith("Theme."))
            {
                continue;
            }

            var expectedKey = BuildExpectedKey(requiredPrefix, elementWithNonTypeKey.Key);

            //Serilog.Log.Warning($"Wrong key.{Environment.NewLine}Current : {elementWithNonTypeKey.Key}{Environment.NewLine}Expected: {expectedKey}");

            yield return (elementWithNonTypeKey.Key, expectedKey);
        }
    }

    string BuildExpectedKey(string requiredPrefix, string key)
    {
        return requiredPrefix + BuildKey(key).Replace("Fluent.Ribbon.", string.Empty);

        static string BuildKey(string key)
        {
            if (key.EndsWith("Brush"))
            {
                return key[..^"Brush".Length];
            }

            if (key.EndsWith("Color"))
            {
                return key[..^"Color".Length];
            }

            if (key.EndsWith("Style"))
            {
                return key[..^"Style".Length];
            }

            if (key.EndsWith("ControlTemplate"))
            {
                return key[..^"ControlTemplate".Length];
            }

            if (key.EndsWith("Template"))
            {
                return key[..^"Template".Length];
            }

            return key;
        }
    }

    static string GetFullPrefix(XName name) => "Fluent.Ribbon." + GetPrefixPart(name) + ".";

    static string GetPrefixPart(XName name) =>
        name.LocalName switch
        {
            "Thickness" => "Values", 
            "ApplicationMenuRightScrollViewerExtractorConverter" => "Converters",
            "Storyboard" => "Storyboards",
            "Style" => "Styles",
            "ControlTemplate" => "Templates",
            "ColorGradientItemTemplateSelector" => "Converters",
            "DataTemplate" => "DataTemplates",
            "ScreenTip" => "ScreenTips",
            "BooleanToVisibilityConverter" => "Converters",
            "MenuScrollingVisibilityConverter" => "Converters",
            "DrawingImage" => "Images",
            "String" => "Values",
            "Color" => "Colors",
            "SolidColorBrush" => "Brushes",
            "Boolean" => "Values",
            "LinearGradientBrush" => "Brushes",
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
        };

    public void FixKeys(params string[] targetFiles)
    {
        var keysToFix = this.CheckKeys()
            .ToArray();

        if (keysToFix.Length == 0)
        {
            return;
        }

        foreach (var targetFile in targetFiles)
        {
            Serilog.Log.Information(targetFile);

            var content = File.ReadAllText(targetFile);
            var contentBefore = content;

            foreach (var keyToFix in keysToFix)
            {
                content = content.Replace($"\"{keyToFix.Key}\"", $"\"{keyToFix.expectedKey}\"");
                content = content.Replace($" {keyToFix.Key}}}", $" {keyToFix.expectedKey}}}");
                content = content.Replace($" {keyToFix.Key}, ", $" {keyToFix.expectedKey}, ");
            }

            if (contentBefore != content)
            {
                File.WriteAllText(targetFile, content);
            }
        }
    }
}