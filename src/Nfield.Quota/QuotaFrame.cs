using System;
using System.Collections.Generic;
using System.Linq;

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

        public QuotaFrameVariable this[string variableName]
        {
            get
            {
                var variable = FrameVariables.FirstOrDefault(v => v.Name == variableName);
                if (variable == null)
                {
                    throw new InvalidOperationException(
                        $"Cannot find variable named '{variableName}' in the root of the frame.");
                }

                return variable;
            }
        }

        public QuotaFrameLevel this[string variableName, string levelName]
        {
            get
            {
                var variable = this[variableName];
                
                var level = variable.Levels.FirstOrDefault(l => l.Name == levelName);
                if (level == null)
                {
                    throw new InvalidOperationException(
                        $"Cannot find level named '{levelName}' on variable '{variableName}' (id: '{variable.Id}')");
                }

                return level;
            }
        }
    }
}
