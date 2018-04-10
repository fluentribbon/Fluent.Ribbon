namespace Fluent.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using Fluent.Converters;

    /// <summary>
    /// Contains a list of keys that activate the key tips.
    /// </summary>
    [TypeConverter(typeof(KeysConverter))]
    public class KeyTipKeysCollection : List<Key>
    {
        /// <summary>
        /// The default keys used to activate key tips.
        /// </summary>
        public static KeyTipKeysCollection Default => new KeyTipKeysCollection { Key.LeftAlt, Key.RightAlt, Key.F10, Key.Space };
    }
}
