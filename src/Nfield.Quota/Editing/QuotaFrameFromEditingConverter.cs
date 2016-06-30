using System.Collections.Generic;
using System.Linq;
using Nfield.Quota.Builders;

namespace Nfield.Quota.Editing
{
    public class QuotaFrameFromEditingConverter
    {
        public static Quota.QuotaFrame Convert(QuotaFrame sourceFrame)
        {
            var builder = new QuotaFrameBuilder()
                .Id(sourceFrame.Id)
                .Target(sourceFrame.Target)
                .Successful(sourceFrame.Successful);

            AddVariableDefinitions(builder, sourceFrame.VariableDefinitions);
            AddStructure(builder, sourceFrame.FrameVariables);

            // todo add targets

            return builder.Build();
        }

        private static void AddVariableDefinitions(
            QuotaFrameBuilder builder, IEnumerable<QuotaVariableDefinition> variableDefinitions)
        {
            foreach (var vd in variableDefinitions.OrderBy(d => d.DisplayIndex))
            {
                builder.VariableDefinition(
                    vd.Id,
                    vd.Name,
                    vd.OdinVariableName,
                    vd.Levels.OrderBy(l => l.DisplayIndex).Select(l => l.Id).ToList()
                );
            }
        }

        private static void AddStructure(
            QuotaFrameBuilder builder, IEnumerable<QuotaFrameVariable> frameVariables)
        {
            foreach (var frameVariable in frameVariables.OrderBy(v => v.DisplayIndex))
            {
                /*builder.FrameVariable(
                    frameVariable.DefinitionId,
                    frameVariable.Id,
                    lb => AddFrameLevels(lb, frameVariable.Levels)
                );*/
                //todo
            }
        }
    }
}