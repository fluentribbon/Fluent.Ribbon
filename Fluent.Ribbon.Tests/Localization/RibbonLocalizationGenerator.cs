namespace Fluent.Tests.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Fluent.Localization;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonLocalizationGeneratorTest
    {
        [Test]
        [Explicit("This test is used to update/re-generate all translations.")]
        public void Generate()
        {
            // When adding new translateable strings you should add the property as virtual in RibbonLocalizationBase and override the property in the English localization.
            // That way you can compile and then run the "test" afterwards.

            RibbonLocalizationGenerator.Generate(RibbonLocalizationGenerator.LocalizationGenerationMode.Measure);
            //RibbonLocalizationGenerator.Generate(RibbonLocalizationGenerator.LocalizationGenerationMode.Translate);
            RibbonLocalizationGenerator.Generate(RibbonLocalizationGenerator.LocalizationGenerationMode.GenerateFallbacks);
        }
    }

    internal static class RibbonLocalizationGenerator
    {
        internal enum LocalizationGenerationMode
        {
            GenerateFallbacks,
            Measure,
            Translate
        }

        public static void Generate(LocalizationGenerationMode localizationGenerationMode)
        {
            var localizationClasses = GetTypesInNamespace(typeof(RibbonLocalizationBase).Assembly, "Fluent.Localization.Languages");

            var localizationMap = localizationClasses.ToDictionary(x => x.GetCustomAttributes(typeof(RibbonLocalizationAttribute), false).OfType<RibbonLocalizationAttribute>().First(), x => x);

            if (localizationGenerationMode == LocalizationGenerationMode.Measure)
            {
                var englishTextLength = GetLocalizationProperties().Sum(x => ((string)x.GetValue(RibbonLocalizationBase.FallbackLocalization))!.Length);
                var measuredLength = englishTextLength * (localizationMap.Count - 1);

                Trace.WriteLine($"Translating would consume {measuredLength} characters.");

                return;
            }

            foreach (var mapEntry in localizationMap) //.Where(x => x.Key.CultureName == "de"))
            {
                var attribute = mapEntry.Key;
                var localizationClass = mapEntry.Value;
                var localization = (RibbonLocalizationBase)Activator.CreateInstance(localizationClass);

                var localizations = GenerateLocalizations(localization, localizationGenerationMode);

                var additionalTranslations = new StringBuilder();

                var template = $@"#pragma warning disable

namespace Fluent.Localization.Languages
{{
    [RibbonLocalization(""{attribute.DisplayName}"", ""{attribute.CultureName}"")]
    public class {localizationClass.Name} : RibbonLocalizationBase
    {{
{localizations}{additionalTranslations.ToString().TrimEnd()}
    }}
}}";
                var path = Path.Combine(Path.GetDirectoryName(localizationClass.Assembly.Location)!, @"..\..\..\..\Fluent.Ribbon\Localization\Languages", $"{localizationClass.Name}.cs");

                path = Path.GetFullPath(path);

                File.WriteAllText(path, template);
            }
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        private static IEnumerable<PropertyInfo> GetLocalizationProperties()
        {
            foreach (var propertyInfo in typeof(RibbonLocalizationBase).GetProperties().OrderBy(x => x.Name))
            {
                if (propertyInfo.Name is nameof(RibbonLocalizationBase.CultureName)
                    or nameof(RibbonLocalizationBase.DisplayName))
                {
                    continue;
                }

                yield return propertyInfo;
            }
        }

        private static string GenerateLocalizations(RibbonLocalizationBase localization, LocalizationGenerationMode localizationGenerationMode)
        {
            var result = new StringBuilder();

            foreach (var propertyInfo in GetLocalizationProperties())
            {
                var value = (string)propertyInfo.GetValue(localization);
                result.AppendLine(CreatePropertyLine(localization, propertyInfo.Name, value, localizationGenerationMode));
            }

            return result.ToString().TrimEnd();
        }

        private static string CreatePropertyLine(RibbonLocalizationBase localization, string propertyName, string value, LocalizationGenerationMode localizationGenerationMode)
        {
            return $@"        public override string {propertyName} {{ get; }} = {TextOrFallback(localization, propertyName, value, localizationGenerationMode)};";
        }

        private static string TextOrFallback(RibbonLocalizationBase localization, string propertyName, string existingTranslationText, LocalizationGenerationMode localizationGenerationMode)
        {
            if (localization.GetType().Name == RibbonLocalizationBase.FallbackLocalization.GetType().Name)
            {
                return $@"""{existingTranslationText.FixNewLineEscaping()}""";
            }

            var englishText = (string)RibbonLocalizationBase.FallbackLocalization.GetType().GetProperty(propertyName)!.GetValue(RibbonLocalizationBase.FallbackLocalization);

            string translatedText;
            switch (localizationGenerationMode)
            {
                case LocalizationGenerationMode.GenerateFallbacks:
                    if (string.IsNullOrEmpty(existingTranslationText)
                        || existingTranslationText == englishText)
                    {
                        return $"FallbackLocalization.{propertyName} /* {englishText} */";
                    }
                    else
                    {
                        translatedText = existingTranslationText;
                    }

                    break;

                case LocalizationGenerationMode.Translate:
                    translatedText = RibbonLocalizationTranslator.Translate(englishText, new System.Globalization.CultureInfo(localization.CultureName!));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(localizationGenerationMode), localizationGenerationMode, null);
            }

            return $@"""{translatedText.FixNewLineEscaping()}""";
        }

        private static string FixNewLineEscaping(this string input)
        {
#pragma warning disable CA1307
            return input.Replace("\n", "\\n");
#pragma warning restore CA1307
        }
    }
}