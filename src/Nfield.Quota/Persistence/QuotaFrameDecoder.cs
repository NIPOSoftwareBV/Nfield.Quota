using Newtonsoft.Json;

namespace Nfield.Quota.Persistence
{
    public static class QuotaFrameDecoder
    {
        public static QuotaFrame Decode(string json)
        {
            return JsonConvert.DeserializeObject<QuotaFrame>(json);
        }
    }
}
