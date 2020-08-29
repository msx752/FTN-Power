using Fortnite.External.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Fortnite.Tests
{
    public class ExternalApiBaseUnit
    {
        public ExternalApiBaseUnit()
        {
            var cultureInfo = new CultureInfo("en-GB");
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            Api = new ExternalApi();
        }
        private ExternalApi _api;

        public ExternalApi Api
        {
            get { return _api; }
            set { _api = value; }
        }

    }
}
