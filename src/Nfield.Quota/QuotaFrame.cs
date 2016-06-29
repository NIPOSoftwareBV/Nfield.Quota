using System.Collections.Generic;

namespace Nfield.Quota
{
    public class QuotaFrame
    {
        public QuotaFrame()
        {
            VariableDefinitions = new List<QuotaVariableDefinition>();
            FrameVariables = new List<QuotaFrameVariable>();
        }

        public string Id { get; set; }

        public int? Target { get; set; }

	    public int Successful { get; set; }

        public ICollection<QuotaVariableDefinition> VariableDefinitions { get;}

        public ICollection<QuotaFrameVariable> FrameVariables { get; }
    }
}
