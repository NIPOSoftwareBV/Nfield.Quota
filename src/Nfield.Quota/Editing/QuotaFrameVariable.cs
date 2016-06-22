using System.Collections.Generic;

namespace Nfield.Quota.Editing
{
    public class QuotaFrameVariable
    {
        public string Id { get; set; }
        public string DefinitionId { get; set; }
        public IEnumerable<QuotaFrameLevel> Levels { get; set; }
        public int DisplayIndex { get; set; }

        public QuotaFrameVariable()
        {
            Levels = new List<QuotaFrameLevel>();
        }
    }
}