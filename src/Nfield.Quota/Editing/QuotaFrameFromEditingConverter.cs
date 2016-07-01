using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Editing
{
    public class QuotaFrameFromEditingConverter
    {
        public static Quota.QuotaFrame Convert(QuotaFrame sourceFrame)
        {
            var definitions = RewriteVariableDefinitions(sourceFrame.VariableDefinitions);
            var frameVariables = RewriteFrameVariables(sourceFrame.FrameVariables);

            return new Quota.QuotaFrame(definitions, frameVariables)
            {
                Id = sourceFrame.Id,
                Target = sourceFrame.Target,
                Successful = sourceFrame.Successful,
            };
        }

        private static IEnumerable<Quota.QuotaVariableDefinition> RewriteVariableDefinitions(
            IEnumerable<QuotaVariableDefinition> variableDefinitions)
        {
            return variableDefinitions.OrderBy(vd => vd.DisplayIndex).Select(vd =>
            {
                var levels = RewriteLevelDefinitions(vd.Levels);
                return new Quota.QuotaVariableDefinition(levels)
                {
                    Id = vd.Id,
                    Name = vd.Name,
                    OdinVariableName = vd.OdinVariableName
                };
            }).ToList();
        }

        private static IEnumerable<Quota.QuotaLevelDefinition> RewriteLevelDefinitions(
            IEnumerable<QuotaLevelDefinition> levels)
        {
            return levels.OrderBy(ld => ld.DisplayIndex).Select(ld =>
                new Quota.QuotaLevelDefinition
                {
                    Id = ld.Id,
                    Name = ld.Name,
                }).ToList();
        }

        private static IEnumerable<Quota.QuotaFrameVariable> RewriteFrameVariables(
            IEnumerable<QuotaFrameVariable> variables)
        {
            return variables.OrderBy(v => v.DisplayIndex).Select(v =>
            {
                var levels = RewriteFrameLevels(v.Levels);
                return new Quota.QuotaFrameVariable(levels)
                {
                    Id = v.Id,
                    DefinitionId = v.DefinitionId
                };
            }).ToList();
        }

        private static IEnumerable<Quota.QuotaFrameLevel> RewriteFrameLevels(
            IEnumerable<QuotaFrameLevel> levels)
        {
            return levels.Select(l =>
            {
                var variables = RewriteFrameVariables(l.Variables);
                return new Quota.QuotaFrameLevel(variables)
                {
                    Id = l.Id,
                    DefinitionId = l.DefinitionId,
                    Target = l.Target,
                    Successful = l.Successful,
                };
            }).ToList();
        }
    }
}