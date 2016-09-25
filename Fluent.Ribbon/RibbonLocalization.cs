namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Fluent.Localization;

    /// <summary>
    /// Contains localizable Ribbon's properties. 
    /// Set Culture property to change current Ribbon localization or 
    /// set properties independently to use your localization
    /// </summary>
    public class RibbonLocalization : INotifyPropertyChanged
    {
        private readonly Dictionary<string, Type> localizationMap;

        private CultureInfo culture;

        private RibbonLocalizationBase localization;

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs then property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // Raises PropertYChanegd event
        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Static instance of <see cref="RibbonLocalization"/> to ease it's usage in XAML.
        /// </summary>
        public static RibbonLocalization Current { get; } = new RibbonLocalization();

        /// <summary>
        /// Gets or sets current culture used for localization.
        /// </summary>
        public CultureInfo Culture
        {
            get { return this.culture; }
            set
            {
                if (!Equals(this.culture, value))
                {
                    this.culture = value;
                    this.LoadCulture(this.culture);
                    this.RaisePropertyChanged(nameof(this.Culture));
                }
            }
        }

        /// <summary>
        /// Gets or sets the current localization.
        /// </summary>
        public RibbonLocalizationBase Localization
        {
            get { return this.localization; }
            set
            {
                if (!Equals(this.localization, value))
                {
                    this.localization = value;
                    this.RaisePropertyChanged(nameof(this.Localization));
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonLocalization()
        {
            var localizationClasses = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "Fluent.Localization.Languages");

#if NET45
            this.localizationMap = localizationClasses.ToDictionary(x => x.GetCustomAttribute<RibbonLocalizationAttribute>().CultureName, x => x);
#else
            this.localizationMap = localizationClasses.ToDictionary(x => x.GetCustomAttributes(typeof(RibbonLocalizationAttribute), false).OfType<RibbonLocalizationAttribute>().First().CultureName, x => x);
#endif

            this.Culture = CultureInfo.CurrentUICulture;
        }

        private void LoadCulture(CultureInfo requestedCulture)
        {
            Type localizationClass;
            if (this.localizationMap.TryGetValue(requestedCulture.Name, out localizationClass))
            {
                this.Localization = (RibbonLocalizationBase)Activator.CreateInstance(localizationClass);
                return;
            }

            if (this.localizationMap.TryGetValue(requestedCulture.TwoLetterISOLanguageName, out localizationClass))
            {
                this.Localization = (RibbonLocalizationBase)Activator.CreateInstance(localizationClass);
                return;
            }

            Trace.WriteLine($"Localization for culture \"{requestedCulture.DisplayName}\" with culture name \"{requestedCulture.Name}\" and LCID \"{requestedCulture.LCID}\" could not be found. Falling back to english.");

            this.Localization = RibbonLocalizationBase.FallbackLocalization;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static IList<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToList();
        }
    }
}