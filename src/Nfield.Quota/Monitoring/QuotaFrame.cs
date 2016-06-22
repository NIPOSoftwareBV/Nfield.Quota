using System.Collections.Generic;

namespace Nfield.Quota.Monitoring
{
    public class QuotaFrame
    {
        public string Id { get; set; }
        public IEnumerable<QuotaFrameVariable> Variables { get; set; }
        public int? Target { get; set; }
        public int Successful { get; set; }

        public QuotaFrame()
        {
            Variables = new List<QuotaFrameVariable>();
        }
    }
}
