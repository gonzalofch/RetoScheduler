using Microsoft.Extensions.Localization;
using RetoScheduler.Enums;
using RetoScheduler.Extensions;
using System.Collections.Generic;
using System.Globalization;

namespace RetoScheduler.Localization
{
    public class SchedulerLocalizer : IStringLocalizer
    {
        private Dictionary<string, string> _traductions;

        public SchedulerLocalizer()
        {
            _traductions = GetCurrentCultureText();
        }

        private Dictionary<string, string> GetCurrentCultureText()
        {

            if (CultureInfo.CurrentCulture.ToString() == Cultures.en_US.GetDescription())
            {
                return SchedulerEnglishTexts.Traductions;
            }
            return SchedulerSpanishTexts.Traductions;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (_traductions.TryGetValue(name, out string value))
                {
                    return new LocalizedString(name, value);
                };

                return default;
            }
        }

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }
    }
}
