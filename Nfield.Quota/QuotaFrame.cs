using System;
using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota
{
    public class QuotaFrame : IQuotaCell
    {
        public QuotaFrame()
        {
            VariableDefinitions = new QuotaVariableDefinitionCollection();
            FrameVariables = new List<QuotaFrameVariable>();
        }

        public QuotaFrame(
            IEnumerable<QuotaVariableDefinition> variableDefinitions,
            IEnumerable<QuotaFrameVariable> frameVariables)
        {
            VariableDefinitions = new QuotaVariableDefinitionCollection(variableDefinitions);
            FrameVariables = new List<QuotaFrameVariable>(frameVariables);
        }

        public int? Target { get; set; }
        public int? MaxOvershoot { get; set; }

        // implementation of IQuotaCell is for internal use only
        int? IQuotaCell.Target => null; // the target on the quota frame acts as a max target
        int? IQuotaCell.MaxTarget => Target;
        string IQuotaCell.Name => "root level";
        Guid IQuotaCell.Id => default;

        public QuotaVariableDefinitionCollection VariableDefinitions { get; }

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
