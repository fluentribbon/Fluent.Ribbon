namespace Fluent.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using Fluent.Converters;

    /// <summary>
    /// ddd
    /// </summary>
    [TypeConverter(typeof(KeysConverter))]
    public class KeysCollection : List<Key>
    {
    }
}
