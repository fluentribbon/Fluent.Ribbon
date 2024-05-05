namespace Fluent.Tests.Themes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

[TestFixture]
public class MouseOverAndKeyboardFocusTriggerTest
{
    [Test]
    public void Test()
    {
        var exceptions = new List<string>
        {
            "Fluent.Ribbon.Styles.WindowCommands.CaptionButton",
            "Fluent.Ribbon.Templates.WindowCommands.CaptionButton",
            "Fluent.Ribbon.Templates.DialogLauncherButton"
        };
        var generic = (ResourceDictionary)Application.LoadComponent(new Uri("/Fluent;component/Themes/Styles.xaml", UriKind.Relative));

        var templatesToStyles = new Dictionary<ControlTemplate, Dictionary<string, Style>>();
        var invalidStylesAndTemplates = new List<string>();

        foreach (DictionaryEntry item in generic)
        {
            var resourceKey = item.Key.ToString();

            if (item.Value is Style style)
            {
                if (exceptions.Contains(resourceKey))
                {
                    continue;
                }

                var settersToCheckForTemplates = style.Setters.ToList();

                foreach (var styleTrigger in style.Triggers.OfType<Trigger>())
                {
                    settersToCheckForTemplates.AddRange(styleTrigger.Setters);
                }

                foreach (var templateSetter in settersToCheckForTemplates.OfType<Setter>().Where(x => x.Property == Control.TemplateProperty && x.Value is not null))
                {
                    var template = templateSetter.Value as ControlTemplate ?? (ControlTemplate)generic[((DynamicResourceExtension)templateSetter.Value).ResourceKey];

                    if (templatesToStyles.TryGetValue(template, out var styles) == false)
                    {
                        styles = new Dictionary<string, Style>();
                        templatesToStyles[template] = styles;
                    }

                    styles.Add(resourceKey, style);
                }

                if ((CheckFocusVisualStyle(style.Setters) || CheckTriggers(style.Triggers)) == false)
                {
                    invalidStylesAndTemplates.Add(resourceKey);
                }
            }
        }

        foreach (DictionaryEntry item in generic)
        {
            var resourceKey = item.Key.ToString();

            if (item.Value is ControlTemplate template)
            {
                if (exceptions.Contains(resourceKey))
                {
                    continue;
                }

                if (CheckTriggers(template.Triggers) == false)
                {
                    if (templatesToStyles.TryGetValue(template, out var styles))
                    {
                        foreach (var styleForTemplate in styles)
                        {
                            if (exceptions.Contains(resourceKey) == false
                                && (CheckFocusVisualStyle(styleForTemplate.Value.Setters) || CheckTriggers(styleForTemplate.Value.Triggers)) == false)
                            {
                                invalidStylesAndTemplates.Add(resourceKey + " => " + styleForTemplate.Key);
                            }
                        }
                    }
                    else
                    {
                        invalidStylesAndTemplates.Add(resourceKey);
                    }
                }
            }
        }

        if (invalidStylesAndTemplates.Count > 0)
        {
            Assert.Fail(string.Join(Environment.NewLine, invalidStylesAndTemplates));
        }
    }

    private static bool CheckFocusVisualStyle(SetterBaseCollection setters)
    {
        foreach (var setterBase in setters)
        {
            if (setterBase is Setter setter)
            {
                if (setter.Property == FrameworkElement.FocusVisualStyleProperty)
                {
                    return setter.Value is DynamicResourceExtension;
                }
            }
        }

        return false;
    }

    private static bool CheckTriggers(TriggerCollection triggers)
    {
        var hasIsMouseOverTrigger = false;
        var hasIsKeyboardFocusedTrigger = false;
        var hasIsKeyboardFocusWithinTrigger = false;

        foreach (var triggerBase in triggers)
        {
            switch (triggerBase)
            {
                case Trigger trigger when trigger.Property == UIElement.IsMouseOverProperty:
                    hasIsMouseOverTrigger = true;
                    break;
                case Trigger trigger when trigger.Property == UIElement.IsKeyboardFocusedProperty:
                    hasIsKeyboardFocusedTrigger = true;
                    break;
                case Trigger trigger when trigger.Property == UIElement.IsKeyboardFocusWithinProperty:
                    hasIsKeyboardFocusWithinTrigger = true;
                    break;

                case MultiTrigger multiTrigger:
                {
                    foreach (var condition in multiTrigger.Conditions)
                    {
                        if (condition.Property == UIElement.IsMouseOverProperty)
                        {
                            hasIsMouseOverTrigger = true;
                        }
                        else if (condition.Property == UIElement.IsKeyboardFocusedProperty)
                        {
                            hasIsKeyboardFocusedTrigger = true;
                        }
                        else if (condition.Property == UIElement.IsKeyboardFocusWithinProperty)
                        {
                            hasIsKeyboardFocusWithinTrigger = true;
                        }
                    }

                    break;
                }
            }
        }

        if (hasIsMouseOverTrigger
            && hasIsKeyboardFocusedTrigger == false
            && hasIsKeyboardFocusWithinTrigger == false)
        {
            return false;
        }

        return true;
    }
}