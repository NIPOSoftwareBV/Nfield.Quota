using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameBuilder
    {
        private string _id;
        private int? _target;
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
            var frame = new QuotaFrame
            {
                Id = _id,
                Target = _target,
            };

            foreach (var builder in _variableDefinitionBuilders)
            {
                builder.Build(frame);
            }

            _structureBuilder.Build(frame);

            var validator = new QuotaFrameValidator();
            validator.Validate(frame);

            return frame;
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

        public QuotaFrameBuilder VariableDefinition(
            string variableName,
            string odinVariableName,
            IEnumerable<string> levelNames)
        {
            var variableDefinitionBuilder = new QuotaVariableDefinitionBuilder(
                Guid.NewGuid(),
                variableName,
                odinVariableName,
                levelNames
                );
            Add(variableDefinitionBuilder);
            return this;
        }

        public QuotaFrameBuilder VariableDefinition(
            string variableName,
            IEnumerable<string> levelNames)
        {
            return VariableDefinition(variableName, variableName.ToLowerInvariant(), levelNames);
        }


        public QuotaFrameBuilder Structure(
            Action<QuotaFrameStructureBuilder> buildAction)
        {
            buildAction(_structureBuilder);
            return this;
        }
    }
}