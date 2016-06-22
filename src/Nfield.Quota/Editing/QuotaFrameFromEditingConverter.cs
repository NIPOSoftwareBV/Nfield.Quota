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
            AddFrameVariables(builder, sourceFrame.FrameVariables);

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
                    lb => AddLevelDefinitions(lb, vd.Levels)
                );
            }
        }

        private static void AddLevelDefinitions(
            QuotaVariableDefinitionBuilder builder, IEnumerable<QuotaLevelDefinition> levelDefinitions)
        {
            foreach (var ld in levelDefinitions.OrderBy(l => l.DisplayIndex))
            {
                builder.Level(
                    ld.Id,
                    ld.Name
                );
            }
        }

        private static void AddFrameVariables(
            QuotaFrameBuilder builder, IEnumerable<QuotaFrameVariable> frameVariables)
        {
            foreach (var frameVariable in frameVariables.OrderBy(v => v.DisplayIndex))
            {
                builder.FrameVariable(
                    frameVariable.DefinitionId,
                    frameVariable.Id,
                    lb => AddFrameLevels(lb, frameVariable.Levels)
                );
            }
        }

        private static void AddFrameLevels(
            QuotaFrameVariableBuilder builder, IEnumerable<QuotaFrameLevel> levels)
        {
            foreach (var frameLevel in levels)
            {
                builder.Level(
                    frameLevel.DefinitionId,
                    frameLevel.Id,
                    frameLevel.Target,
                    frameLevel.Successful,
                    lb => AddFrameVariables(lb, frameLevel.Variables)
                );
            }
        }

        private static void AddFrameVariables(
            QuotaFrameLevelBuilder builder, IEnumerable<QuotaFrameVariable> frameVariables)
        {
            foreach (var frameVariable in frameVariables)
            {
                builder.Variable(
                    frameVariable.DefinitionId,
                    frameVariable.Id,
                    lb => AddFrameLevels(lb, frameVariable.Levels)
                );
            }
        }
    }
}