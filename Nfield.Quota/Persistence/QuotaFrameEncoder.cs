using Newtonsoft.Json;

namespace Nfield.Quota.Persistence
{
    public class QuotaFrameEncoderOptions
    {
        public static QuotaFrameEncoderOptions Default = new QuotaFrameEncoderOptions
        {
            IncludeTargets = false
        };

        public bool IncludeTargets { get; private set; }
    }
    public static class QuotaFrameEncoder
    {
        public static string Encode(QuotaFrame frame)
        {
            return Encode(frame, QuotaFrameEncoderOptions.Default);
        }

        public static string Encode(QuotaFrame frame, QuotaFrameEncoderOptions options)
        {
            var resolver = new QuotaFrameContractResolver();

            if (!options.IncludeTargets)
            {
                resolver.Ignore(
                    typeof(QuotaFrame),
                    nameof(QuotaFrame.Target));

                resolver.Ignore(
                    typeof(QuotaFrameLevel),
                    nameof(QuotaFrameLevel.Target),
                    nameof(QuotaFrameLevel.MaxTarget));
            }

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
