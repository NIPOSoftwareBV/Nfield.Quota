using System.Collections.Generic;

namespace Nfield.Quota
{
    public class QuotaFrameVariable
    {
        public QuotaFrameVariable()
        {
            Levels = new List<QuotaFrameLevel>();
        }

        public QuotaFrameVariable(IEnumerable<QuotaFrameLevel> levels)
        {
            Levels = new List<QuotaFrameLevel>(levels);
        }

        public string Id { get; set; }

        public string DefinitionId { get; set; }

        public string Name { get; set; }

        public ICollection<QuotaFrameLevel> Levels { get; }
    }
}