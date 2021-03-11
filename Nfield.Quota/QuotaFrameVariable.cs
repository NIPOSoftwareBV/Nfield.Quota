using System;
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

        public Guid Id { get; set; }

        public Guid DefinitionId { get; set; }

        public string Name { get; set; }

        public bool IsHidden { get; set; }

        public ICollection<QuotaFrameLevel> Levels { get; }
    }
}