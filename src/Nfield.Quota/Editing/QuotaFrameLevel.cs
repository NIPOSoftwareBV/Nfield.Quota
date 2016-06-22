using System.Collections.Generic;

namespace Nfield.Quota.Editing
{
    public class QuotaFrameLevel
    {
        public string Id { get; set; }
        public string DefinitionId { get; set; }
        public int? Target { get; set; }
        public int Successful { get; set; }
        public IEnumerable<QuotaFrameVariable> Variables { get; set; }

        public QuotaFrameLevel()
        {
            Variables = new List<QuotaFrameVariable>();
        }
    }
}