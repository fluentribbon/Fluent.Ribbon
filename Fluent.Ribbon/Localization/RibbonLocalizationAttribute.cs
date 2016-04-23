namespace Fluent.Localization
{
    using System;

    /// <summary>
    /// Attribute class providing informations about a localization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RibbonLocalizationAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="displayName">Specifies the display name.</param>
        /// <param name="cultureName">Specifies the culture name.</param>
        public RibbonLocalizationAttribute(string displayName, string cultureName)
        {
            this.DisplayName = displayName;
            this.CultureName = cultureName;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        public string CultureName { get; }
    }
}