using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: Guid("d849f751-e8f4-480d-acc0-6148eafcaafc")]

[assembly: XmlnsPrefix("urn:fluent-ribbon", "fluent")]
[assembly: XmlnsDefinition("urn:fluent-ribbon", "Fluent")]
[assembly: XmlnsDefinition("urn:fluent-ribbon", "Fluent.Converters")]
[assembly: XmlnsDefinition("urn:fluent-ribbon", "Fluent.TemplateSelectors")]
[assembly: XmlnsDefinition("urn:fluent-ribbon", "Fluent.Theming")]
[assembly: XmlnsDefinition("urn:fluent-ribbon", "Fluent.Metro.Behaviours")]

// Allow internals to be accessible by the unit test code:
[assembly: InternalsVisibleTo("Fluent.Ribbon.Showcase")]
[assembly: InternalsVisibleTo("Fluent.Tests")]