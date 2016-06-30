using System;
using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameStructureBuilder
    {
        private readonly IList<string> _variableIds;
        private readonly IList<QuotaFrameStructureBuilder> _childBuilders;

        public QuotaFrameStructureBuilder()
        {
            _variableIds = new List<string>();
            _childBuilders = new List<QuotaFrameStructureBuilder>();
        }

        public QuotaFrameStructureBuilder Variable(string variableId)
        {
            _variableIds.Add(variableId);
            return this;
        }

        public QuotaFrameStructureBuilder Variable(
            string variableId,
            Action<QuotaFrameStructureBuilder> buildAction)
        {
            var childBuilder = new QuotaFrameStructureBuilder();
            buildAction(childBuilder);
            _childBuilders.Add(childBuilder);
            return Variable(variableId);
        }

        public void Build(QuotaFrame quotaFrame)
        {
            BuildVariable(quotaFrame, quotaFrame.FrameVariables);
        }

        private void BuildVariable(
            QuotaFrame quotaFrame,
            ICollection<QuotaFrameVariable> currentRoot)
        {
            foreach (var variableId in _variableIds)
            {
                var variableDefinition = quotaFrame.VariableDefinitions.First(vd => vd.Id == variableId);
                var variable = new QuotaFrameVariable
                {
                    Id = Guid.NewGuid().ToString(),
                    DefinitionId = variableDefinition.Id
                };

                BuildLevel(quotaFrame, variableDefinition.Levels, variable);

                currentRoot.Add(variable);
            }
        }

        private void BuildLevel(
            QuotaFrame quotaFrame,
            IEnumerable<QuotaLevelDefinition> newLevels,
            QuotaFrameVariable variable)
        {
            foreach (var definitionLevel in newLevels)
            {
                var frameLevel = new QuotaFrameLevel
                {
                    Id = Guid.NewGuid().ToString(),
                    DefinitionId = definitionLevel.Id
                };

                variable.Levels.Add(frameLevel);

                // Recurse for every added level
                foreach (var childBuilder in _childBuilders)
                {
                    childBuilder.BuildVariable(quotaFrame, frameLevel.Variables);
                }
            }
        }
    }
}