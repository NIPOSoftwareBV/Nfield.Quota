using System;
using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota.Builders2
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
            foreach (var variableId in _variableIds)
            {
                var definition = quotaFrame.VariableDefinitions.First(vd => vd.Id == variableId);
                var variable = new QuotaFrameVariable()
                {
                    Id = Guid.NewGuid().ToString(),
                    DefinitionId = definition.Id
                };

                foreach (var definitionLevel in definition.Levels)
                {
                    var frameLevel = new QuotaFrameLevel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DefinitionId = definitionLevel.Id
                    };
                    variable.Levels.Add(frameLevel);
                }

                //todo wire up
                //quotaFrame.FrameVariables
            }
        }
    }
}