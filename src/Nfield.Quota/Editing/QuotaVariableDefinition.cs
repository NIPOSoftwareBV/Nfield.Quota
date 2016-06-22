using System.Collections.Generic;

namespace Nfield.Quota.Editing
{
    public class QuotaVariableDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OdinVariableName { get; set; }
        public IEnumerable<QuotaLevelDefinition> Levels { get; set; }
        public int DisplayIndex { get; set; }

        public QuotaVariableDefinition()
        {
            Levels = new List<QuotaLevelDefinition>();
        }
    }
}