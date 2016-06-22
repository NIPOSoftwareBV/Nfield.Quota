using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameVariableBuilder
    {
        private readonly string _id;
        private readonly string _definitionId;
        private readonly List<QuotaFrameLevelBuilder> _levelBuilders = new List<QuotaFrameLevelBuilder>();

        public QuotaFrameVariableBuilder(string id, string definitionId)
        {
            _id = id;
            _definitionId = definitionId;
        }

        private void Add(QuotaFrameLevelBuilder builder)
        {
            _levelBuilders.Add(builder);
        }

        public void Build(QuotaFrame quotaFrame)
        {
            BuildVariable(variable => { quotaFrame.FrameVariables.Add(variable);});
        }

        public void Build(QuotaFrameLevel level)
        {
            BuildVariable(variable => { level.Variables.Add(variable);});
        }

        private void BuildVariable(Action<QuotaFrameVariable> action)
        {
            var quotaFrameVariable = new QuotaFrameVariable
            {
                Id = _id,
                DefinitionId = _definitionId
            };

            action(quotaFrameVariable);

            foreach (var builder in _levelBuilders)
            {
                builder.Build(quotaFrameVariable);
            }
        }

        public QuotaFrameLevelBuilder Level(string definitionId, string id, Action<QuotaFrameLevelBuilder> builderAction = null)
        {
            return Level(definitionId, id, null, 0, builderAction);
        }

        public QuotaFrameLevelBuilder Level(string definitionId, int? target, int successful, Action<QuotaFrameLevelBuilder> builderAction = null)
        {
            return Level(definitionId, Guid.NewGuid().ToString(), target, successful, builderAction);
        }

        public QuotaFrameLevelBuilder Level(string definitionId, string id, int? target, int successful, Action<QuotaFrameLevelBuilder> builderAction = null)
        {
            var levelBuilder = new QuotaFrameLevelBuilder(id, definitionId, target, successful);
            builderAction?.Invoke(levelBuilder);
            Add(levelBuilder);
            return levelBuilder;
        }

        public QuotaFrameLevelBuilder Level(string definitionId, string id, int target, Action<QuotaFrameLevelBuilder> builderAction = null)
        {
            return Level(definitionId, id, target, 0, builderAction);
        }
    }
}