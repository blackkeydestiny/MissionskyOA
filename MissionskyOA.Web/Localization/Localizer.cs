using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Web.Localization
{
    public delegate LocalizedString Localizer(string text, params object[] args);
}
