using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameBuilder
    {
        private string _id;
        private int? _target;
        private int _successful;
        private readonly IList<QuotaVariableDefinitionBuilder> _variableDefinitionBuilders;
        private readonly QuotaFrameStructureBuilder _structureBuilder;

        public QuotaFrameBuilder()
        {
            _variableDefinitionBuilders = new List<QuotaVariableDefinitionBuilder>();
            _structureBuilder = new QuotaFrameStructureBuilder();
        }

        private void Add(QuotaVariableDefinitionBuilder builder)
        {
            _variableDefinitionBuilders.Add(builder);
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

            _structureBuilder.Build(quotaFrame);

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
            IEnumerable<string> levelNames)
        {
            var variableDefinitionBuilder = new QuotaVariableDefinitionBuilder(variableId, variableName, odinVariableName, levelNames);
            Add(variableDefinitionBuilder);
            return this;
        }

        public QuotaFrameBuilder VariableDefinition(
            string variableId,
            IEnumerable<string> levelNames)
        {
            return VariableDefinition(variableId, variableId, variableId, levelNames);
        }


        public QuotaFrameBuilder Structure(
            Action<QuotaFrameStructureBuilder> buildAction)
        {
            buildAction(_structureBuilder);
            return this;
        }
    }
}