#if NET452
#else
namespace Fluent.Tests.Localization
{
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Google.Api.Gax.ResourceNames;
    using Google.Cloud.Translate.V3;

    public static class RibbonLocalizationTranslator
    {
        public static string Translate(string englishText, CultureInfo targetCulture)
        {
            var credentials = GetGoogleCredentials();
            var builder = new TranslationServiceClientBuilder { JsonCredentials = credentials };
            var client = builder.Build();
            var request = new TranslateTextRequest
            {
                Contents = { englishText },
                TargetLanguageCode = targetCulture.TwoLetterISOLanguageName,
                Parent = new ProjectName("fluentribbon").ToString()
            };
            var response = client.TranslateText(request);
            // response.Translations will have one entry, because request.Contents has one entry.
            var translation = response.Translations[0];
            return translation.TranslatedText;
        }

        private static string GetGoogleCredentials([CallerFilePath] string path = null)
        {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(path), "fluentribbon.google.credentials.json"), Encoding.UTF8);
        }
    }
}
#endif