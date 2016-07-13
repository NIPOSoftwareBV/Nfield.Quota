using System;
using System.Collections.Generic;

namespace Nfield.Quota
{
    public class QuotaVariableDefinition
    {
        public QuotaVariableDefinition()
        {
            Levels = new List<QuotaLevelDefinition>();
        }

        public QuotaVariableDefinition(IEnumerable<QuotaLevelDefinition> levels)
        {
            Levels = new List<QuotaLevelDefinition>(levels);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string OdinVariableName { get; set; }

        public ICollection<QuotaLevelDefinition> Levels { get; }
    }
}