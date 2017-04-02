namespace Fluent.Extensibility
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface which allows extension of the KeyTip system.
    /// </summary>
    public interface IKeyTipInformationProvider
    {
        /// <summary>
        /// Gets a list of <see cref="KeyTipInformation"/> which belong to the current instance.
        /// </summary>
        /// <param name="hide">Defines if the created <see cref="KeyTip"/> should be hidden or not.</param>
        /// <returns>A list of <see cref="KeyTipInformation"/> which belong to the current instance.</returns>
        IEnumerable<KeyTipInformation> GetKeyTipInformations(bool hide);
    }
}