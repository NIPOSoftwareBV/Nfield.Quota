using System;
using System.Collections.Generic;
using System.Linq;
using Nfield.Quota.Helpers;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameStructureBuilder
    {
        private readonly IDictionary<string, bool> _variableNames;
        private readonly IList<QuotaFrameStructureBuilder> _childBuilders;

        public QuotaFrameStructureBuilder()
        {
            _variableNames = new Dictionary<string, bool>();
            _childBuilders = new List<QuotaFrameStructureBuilder>();
        }

        public QuotaFrameStructureBuilder Variable(string variableName, bool isForAllocationOnly = false)
        {
            _variableNames.Add(variableName, isForAllocationOnly);
            return this;
        }

        public QuotaFrameStructureBuilder Variable(
            string variableName,
            Action<QuotaFrameStructureBuilder> buildAction,
            bool isForAllocationOnly = false)
        {
            Ensure.ArgumentNotNull(buildAction, nameof(buildAction));

            var childBuilder = new QuotaFrameStructureBuilder();
            buildAction(childBuilder);
            _childBuilders.Add(childBuilder);
            return Variable(variableName, isForAllocationOnly);
        }

        public void Build(QuotaFrame quotaFrame)
        {
            Ensure.ArgumentNotNull(quotaFrame, nameof(quotaFrame));

            BuildVariable(quotaFrame, quotaFrame.FrameVariables);
        }

        private void BuildVariable(
            QuotaFrame quotaFrame,
            ICollection<QuotaFrameVariable> currentRoot)
        {
            foreach (var variableName in _variableNames)
            {
                var variableDefinition = quotaFrame.VariableDefinitions.First(vd => vd.Name == variableName.Key);
                var variable = new QuotaFrameVariable
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = variableDefinition.Id,
                    Name = variableDefinition.Name,
                    IsForAllocationOnly = variableDefinition.IsForAllocationOnly || variableName.Value
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
            foreach (var levelDefinition in newLevels)
            {
                var frameLevel = new QuotaFrameLevel
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = levelDefinition.Id,
                    Name = levelDefinition.Name
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