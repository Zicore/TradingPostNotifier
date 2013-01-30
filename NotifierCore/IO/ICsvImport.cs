using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scraper.IO
{
    interface ICsvImport
    {
        void Import(String filePath);
    }
}
