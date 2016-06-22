using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameBuilder
    {
        private string _id;
        private int? _target;
        private int _successful;
        private readonly List<QuotaVariableDefinitionBuilder> _variableDefinitionBuilders = new List<QuotaVariableDefinitionBuilder>();
        private readonly List<QuotaFrameVariableBuilder> _variableBuilders = new List<QuotaFrameVariableBuilder>();
        
        private void Add(QuotaVariableDefinitionBuilder builder)
        {
            _variableDefinitionBuilders.Add(builder);
        }
        private void Add(QuotaFrameVariableBuilder builder)
        {
            _variableBuilders.Add(builder);
        }

        public QuotaFrame Build()
        {
            var quotaFrame = new QuotaFrame
            {
                Id = _id,
                Target = _target,
                Successful = _successful
            };

            foreach (var builder in _variableDefinitionBuilders)
            {
                builder.Build(quotaFrame);
            }

            foreach (var builder in _variableBuilders)
            {
                builder.Build(quotaFrame);
            }

            return quotaFrame;
        }

        public QuotaFrameBuilder Id(string id)
        {
            _id = id;
            return this;
        }

        public QuotaFrameBuilder Target(int? target)
        {
            _target = target;
            return this;
        }

        public QuotaFrameBuilder Successful(int successful)
        {
            _successful = successful;
            return this;
        }

        public QuotaFrameBuilder VariableDefinition(
            string variableId,
            string variableName,
            string odinVariableName,
            Action<QuotaVariableDefinitionBuilder> builderAction
            )
        {
            var variableDefinitionBuilder = new QuotaVariableDefinitionBuilder(variableId, variableName, odinVariableName);
            builderAction(variableDefinitionBuilder);
            Add(variableDefinitionBuilder);
            return this;
        }

        public QuotaFrameBuilder VariableDefinition(
            string variableId,
            Action<QuotaVariableDefinitionBuilder> builderAction
            )
        {
            return VariableDefinition(variableId, variableId, variableId, builderAction);
        }

        public QuotaFrameBuilder FrameVariable(
            string definitionId,
            Action<QuotaFrameVariableBuilder> buildAction
            )
        {
            return FrameVariable(definitionId, Guid.NewGuid().ToString(), buildAction);
        }

        public QuotaFrameBuilder FrameVariable(
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