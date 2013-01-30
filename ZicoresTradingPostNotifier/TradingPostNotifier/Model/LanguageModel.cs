using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;

namespace ZicoresTradingPostNotifier.Model
{
    public class LanguageModel : BindableBase
    {
        private String _language = "en-us";
        public String LanguageKey
        {
            get { return _language; }
            set { _language = value; }
        }

        public LanguageModel()
        {

        }

        public LanguageModel(string language)
        {
            this.LanguageKey = language;
        }

        public LanguageModel(String language, String displayName)
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
