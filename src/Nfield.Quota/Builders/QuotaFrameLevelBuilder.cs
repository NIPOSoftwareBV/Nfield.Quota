using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameLevelBuilder
    {
        private readonly string _id;
        private readonly string _definitionId;
        private readonly int? _target;
        private readonly int _successful;
        private readonly List<QuotaFrameVariableBuilder> _variableBuilders = new List<QuotaFrameVariableBuilder>();

        public QuotaFrameLevelBuilder(string id, string definitionId, int? target = null, int successful = 0)
        {
            _id = id;
            _definitionId = definitionId;
            _target = target;
            _successful = successful;
        }

        public void Add(QuotaFrameVariableBuilder builder)
        {
            _variableBuilders.Add(builder);
        }

        public void Build(QuotaFrameVariable quotaFrameVariable)
        {
            var quotaFrameLevel = new QuotaFrameLevel
            {
                Id = _id,
                DefinitionId = _definitionId,
                Target = _target,
                Successful = _successful
            };

            quotaFrameVariable.Levels.Add(quotaFrameLevel);

            foreach (var builder in _variableBuilders)
            {
                builder.Build(quotaFrameLevel);
            }
        }

        public QuotaFrameLevelBuilder Variable(
            string definitionId,
            Action<QuotaFrameVariableBuilder> buildAction
            )
        {
            return Variable(definitionId, Guid.NewGuid().ToString(), buildAction);           
        }
       public QuotaFrameLevelBuilder Variable(
            string definitionId,
            string id,
            Action<QuotaFrameVariableBuilder> buildAction
            )
        {
            var builder = new QuotaFrameVariableBuilder(id, definitionId);
            buildAction(builder);
            Add(builder);
            return this;
        }
    }
}