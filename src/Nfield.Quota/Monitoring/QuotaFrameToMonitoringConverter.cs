using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Monitoring
{
    public class QuotaFrameToMonitoringConverter
    {
        public static QuotaFrame Convert(Quota.QuotaFrame sourceFrame)
        {
            return new QuotaFrame
            {
                Id = sourceFrame.Id,
                Target = sourceFrame.Target,
                Successful = sourceFrame.Successful,
                Variables = RewriteVariables(
                    sourceFrame.VariableDefinitions.ToList(),
                    sourceFrame.FrameVariables)
            };
        }

        private static IEnumerable<QuotaFrameVariable> RewriteVariables(
            IList<QuotaVariableDefinition> variableDefinitionsRoot,
            IEnumerable<Quota.QuotaFrameVariable> sourceVariables)
        {
            return sourceVariables.Select(sourceVar =>
            {
                var variableDefinition = variableDefinitionsRoot.First(vd => vd.Id == sourceVar.DefinitionId);

                return new QuotaFrameVariable
                {
                    Id = sourceVar.Id,
                    Name = variableDefinition.Name,
                    Levels = RewriteLevels(variableDefinitionsRoot, sourceVar)
                };
            }).ToList(); // execute straight away
        }

        private static IEnumerable<QuotaFrameLevel> RewriteLevels(
            IList<QuotaVariableDefinition> variableDefinitionsRoot,
            Quota.QuotaFrameVariable sourceVariable)
        {
            var variableDefinition = variableDefinitionsRoot.First(vd => vd.Id == sourceVariable.DefinitionId);

            int displayIndex = 0;
            return sourceVariable.Levels.Select(sourceLvl =>
            {
                var levelDefinition = variableDefinition.Levels.First(ld => ld.Id == sourceLvl.DefinitionId);

                return new QuotaFrameLevel
                {
                    Id = sourceLvl.Id,
                    DisplayIndex = displayIndex++,
                    Name = levelDefinition.Name,
                    Target = sourceLvl.Target,
                    Successful = sourceLvl.Successful,
                    Variables = sourceLvl.Variables != null
                        ? RewriteVariables(variableDefinitionsRoot, sourceLvl.Variables)
                        : null
                };
            }).ToList(); // execute straight away
        }
    }
}