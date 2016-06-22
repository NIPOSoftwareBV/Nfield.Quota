using System.Collections.Generic;

namespace Nfield.Quota.Monitoring
{
    public class QuotaFrameLevel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<QuotaFrameVariable> Variables { get; set; }
        public int? Target { get; set; }
        
        public int Successful { get; set; }

        public int DisplayIndex { get; set; }

        public QuotaFrameLevel()
        {
            Variables = new List<QuotaFrameVariable>();
        }
    }
}