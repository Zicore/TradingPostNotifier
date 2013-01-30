using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zicore.Xml
{
    interface ISavableLoadable
    {
        event EventHandler Saving;
        event EventHandler Loading;
    }
}
