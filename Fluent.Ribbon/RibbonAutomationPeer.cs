using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Fluent
{
  public class RibbonAutomationPeer : FrameworkElementAutomationPeer
  {
    public RibbonAutomationPeer(FrameworkElement fe):base(fe)
    {

    }

    protected override string GetAccessKeyCore()
    {
      IKeyTipedControl control = Owner as IKeyTipedControl;
      if (control != null && control.KeyTip != null)
        return control.KeyTip;
      return base.GetAccessKeyCore();
    }

    protected override string GetNameCore()
    {
      IKeyTipedControl control = Owner as IKeyTipedControl;
      string name = base.GetNameCore();

      return name;
    }
  }
}
