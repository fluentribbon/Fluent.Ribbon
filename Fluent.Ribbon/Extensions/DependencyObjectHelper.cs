using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace Fluent.Extensions
{
  public static class DependencyObjectHelper
  {
    public static List<DependencyProperty> GetDependencyProperties(Object element)
    {
      List<DependencyProperty> properties = new List<DependencyProperty>();
      MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
      if (markupObject != null)
      {
        foreach (MarkupProperty mp in markupObject.Properties)
        {
          if (mp.DependencyProperty != null)
          {
            properties.Add(mp.DependencyProperty);
          }
        }
      }

      return properties;
    }

    public static List<DependencyProperty> GetAttachedProperties(Object element)
    {
      List<DependencyProperty> attachedProperties = new List<DependencyProperty>();
      MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
      if (markupObject != null)
      {
        foreach (MarkupProperty mp in markupObject.Properties)
        {
          if (mp.IsAttached)
          {
            attachedProperties.Add(mp.DependencyProperty);
          }
        }
      }

      return attachedProperties;
    }
  }
}
