using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZicoresUtils.Zicore.Configuration
{
    public interface IUserData
    {
        void Save(string path);
        void Load(string path);
    }
}
