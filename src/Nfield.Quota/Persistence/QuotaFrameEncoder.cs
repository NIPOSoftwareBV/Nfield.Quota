using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Nfield.Quota.Persistence
{
    public static class QuotaFrameEncoder
    {
        public static string Encode(QuotaFrame frame)
        {
            var resolver = new QuotaFrameContractResolver();
            resolver.Ignore(
                typeof(QuotaFrame),
                nameof(QuotaFrame.Target),
                nameof(QuotaFrame.Successful));

            resolver.Ignore(
                typeof(QuotaFrameLevel),
                nameof(QuotaFrameLevel.QuotaFrame),
                nameof(QuotaFrameLevel.Successful),
                nameof(QuotaFrameLevel.Target));

            resolver.Ignore(
                typeof(QuotaFrameVariable),
                nameof(QuotaFrameVariable.QuotaFrame));

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(frame, settings);
        }

    }
}
