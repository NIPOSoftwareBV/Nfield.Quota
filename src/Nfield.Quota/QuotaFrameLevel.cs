using System.Collections.Generic;

namespace Nfield.Quota
{
    public class QuotaFrameLevel
    {
        public QuotaFrameLevel()
        {
            Variables = new List<QuotaFrameVariable>();
        }

        public string Id { get; set; }

        public string DefinitionId { get; set; }

        public IEnumerable<QuotaFrameVariable> Variables { get; }

        public int? Target { get; set; }

        public int Successful { get; set; }
    }
}