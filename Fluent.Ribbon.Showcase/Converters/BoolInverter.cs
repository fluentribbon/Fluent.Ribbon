
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentTest.Converters
{
    public class BoolInverter : ConfigurableBoolConverter<bool>
    {
        public BoolInverter() : base( false, true ) { }
    }
}
