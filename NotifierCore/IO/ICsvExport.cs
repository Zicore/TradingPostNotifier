using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotifierCore.IO
{
    interface ICsvExport
    {
        void Export(String filePath);
    }
}
