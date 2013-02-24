using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Notifier
{
    public class Language : BindableBase
    {
        private String _language = "en-us";
        public String LanguageKey
        {
            get { return _language; }
            set { _language = value; }
        }

        public Language()
        {

        }

        public Language(string language)
        {
            this.LanguageKey = language;
        }

        public Language(String language, String displayName)
        {
            this.LanguageKey = language;
            this.DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
