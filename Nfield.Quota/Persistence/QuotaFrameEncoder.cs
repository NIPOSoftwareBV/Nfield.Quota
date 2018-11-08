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
                nameof(QuotaFrame.Target));

            resolver.Ignore(
                typeof(QuotaFrameLevel),
                nameof(QuotaFrameLevel.Target));

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented
            };

            settings.Converters.Add(new GuidJsonConverter());

            return JsonConvert.SerializeObject(frame, settings);
        }

    }
}
