using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ZicoresTradingPostNotifier.Controls
{
    public class GridViewColumnEx : GridViewColumn
    {
        public GridViewColumnEx()
        {

        }

        String identifier = "";

        public String Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
    }
}
