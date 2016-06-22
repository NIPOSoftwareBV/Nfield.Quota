using System.Collections.Generic;

namespace Nfield.Quota.Monitoring
{
    public class QuotaFrameVariable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<QuotaFrameLevel> Levels { get; set; }
        public QuotaFrameVariable()
        {
            Levels = new List<QuotaFrameLevel>();
        }
    }
}