using System.Collections.Generic;

namespace Nfield.Quota.Editing
{
    public class QuotaFrame
    {
        public string Id { get; set; }
        public int? Target { get; set; }
	    public int Successful { get; set; }
        public IEnumerable<QuotaVariableDefinition> VariableDefinitions { get; set; }
        public IEnumerable<QuotaFrameVariable> FrameVariables { get; set; }

        public QuotaFrame()
        {
            VariableDefinitions = new List<QuotaVariableDefinition>();
            FrameVariables = new List<QuotaFrameVariable>();
        }
    }
}
