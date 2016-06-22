using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Editing
{
    public class QuotaFrameToEditingConverter
    {
        public static QuotaFrame Convert(Quota.QuotaFrame sourceFrame)
        {
            return new QuotaFrame
            {
                Id = sourceFrame.Id,
                Target = sourceFrame.Target,
                Successful = sourceFrame.Successful,
                VariableDefinitions = RewriteVariableDefinitions(sourceFrame.VariableDefinitions),
                FrameVariables = RewriteFrameVariables(sourceFrame.FrameVariables)
            };
        }

        private static IEnumerable<QuotaVariableDefinition> RewriteVariableDefinitions(
            IEnumerable<Quota.QuotaVariableDefinition> variableDefinitions)
        {
            var displayIndex = 0;
            return variableDefinitions.Select(vd =>
                new QuotaVariableDefinition
                {
                    Id = vd.Id,
                    Name = vd.Name,
                    OdinVariableName = vd.OdinVariableName,
                    DisplayIndex = displayIndex++,
                    Levels = RewriteLevelDefinitions(vd.Levels)
                }).ToList();
        }

        private static IEnumerable<QuotaLevelDefinition> RewriteLevelDefinitions(
            QuotaLevelDefinitionCollection levels)
        {
            var displayIndex = 0;
            return levels.Select(ld =>
                new QuotaLevelDefinition
                {
                    Id = ld.Id,
                    Name = ld.Name,
                    DisplayIndex = displayIndex++
                }).ToList();
        }

        private static IEnumerable<QuotaFrameVariable> RewriteFrameVariables(
            IEnumerable<Quota.QuotaFrameVariable> variables)
        {
            int displayIndex = 0;
            return variables.Select(v => 
                new QuotaFrameVariable
                {
                    Id = v.Id,
                    DefinitionId = v.DefinitionId,
                    DisplayIndex = displayIndex++,
                    Levels = RewriteFrameLevels(v.Levels)
                }).ToList();
        }

        private static IEnumerable<QuotaFrameLevel> RewriteFrameLevels(
            QuotaFrameLevelCollection levels)
        {
            return levels.Select(l =>
                new QuotaFrameLevel
                {
                    Id = l.Id,
                    DefinitionId = l.DefinitionId,
                    Target = l.Target,
                    Successful = l.Successful,
                    Variables = RewriteFrameVariables(l.Variables)
                }).ToList();
        }
    }
}