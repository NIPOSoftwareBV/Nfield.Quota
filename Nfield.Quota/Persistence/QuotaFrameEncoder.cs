using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nfield.Quota.Persistence
{
    public static class QuotaFrameEncoder
    {
        public static string Encode(QuotaFrame frame)
        {
            return Encode(frame, QuotaFrameEncoderOptions.Default);
        }

        public static string Encode(QuotaFrame frame, QuotaFrameEncoderOptions options)
        {
            // We can't configure our QuotaFrameContractResolver as it is just used as a blueprint
            // to generate a (cached) JsonContract which will then be used going forward to serialize a type
            // This caching is a the contract resolver type level. So we need two distinct type, hence the usage
            // of the CamelCasePropertyNamesContractResolver below when we do want our targets serialized.
            var resolver = options.IncludeTargets ? new CamelCasePropertyNamesContractResolver() : CreateQuotaFrameContractResolver();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented
            };

            settings.Converters.Add(new GuidJsonConverter());

            return JsonConvert.SerializeObject(frame, settings);
        }

        private static QuotaFrameContractResolver CreateQuotaFrameContractResolver()
        {
            // We really should have done this setup in the constructor, but as the type
            // public in this package we don't want to change the default resolver that gets
            // created using the constructor
            var resolver = new QuotaFrameContractResolver();
            resolver.Ignore(
                typeof(QuotaFrame),
                nameof(QuotaFrame.Target)
                );

            resolver.Ignore(
                typeof(QuotaFrameLevel),
                nameof(QuotaFrameLevel.Target),
                nameof(QuotaFrameLevel.MaxTarget)
                );

            return resolver;
        }
    }
}
