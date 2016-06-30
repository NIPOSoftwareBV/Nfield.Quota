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
                var definition = quotaFrame.VariableDefinitions.First(vd => vd.Id == variableId);
                var variable = new QuotaFrameVariable
                {
                    Id = Guid.NewGuid().ToString(),
                    DefinitionId = definition.Id
                };

                foreach (var definitionLevel in definition.Levels)
                {
                    var frameLevel = new QuotaFrameLevel
                    {
                        Id = Guid.NewGuid().ToString(),
                        DefinitionId = definitionLevel.Id
                    };
                    variable.Levels.Add(frameLevel);

                    foreach (var childBuilder in _childBuilders)
                    {
                        childBuilder.Build(quotaFrame);
                    }
                }

                currentRoot.Add(variable);
            }
        }
    }
}