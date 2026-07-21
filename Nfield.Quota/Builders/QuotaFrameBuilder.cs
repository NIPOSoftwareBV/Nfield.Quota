using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nfield.Quota.Helpers;
using Nfield.Quota.Models;

namespace Nfield.Quota.Builders
{
    public class QuotaFrameBuilder
    {
        private int? _target;
        private bool _considerActiveAsSuccessful;
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
                Target = _target,
                ConsiderActiveAsSuccessful = _considerActiveAsSuccessful
            };

            foreach (var builder in _variableDefinitionBuilders)
            {
                builder.Build(frame);
            }

            _structureBuilder.Build(frame);

            return frame;
        }

        public QuotaFrameBuilder Target(int? target)
        {
            _target = target;
            return this;
        }

        public QuotaFrameBuilder ConsiderActiveAsSuccessful(bool considerActiveAsSuccessful)
        {
            _considerActiveAsSuccessful = considerActiveAsSuccessful;
            return this;
        }

        public QuotaFrameBuilder VariableDefinition(
            string variableName,
            string odinVariableName,
            IEnumerable<string> levelNames,
            VariableSelection selection = VariableSelection.NotApplicable,
            bool isMulti = false,
            bool isTargetable = false,
            bool isForAllocationOnly = false)
        {
            bool? isSelectionOptional = null;
            switch (selection)
            {
                case VariableSelection.Mandatory:
                    isSelectionOptional = false;
                    break;
                case VariableSelection.Optional:
                    isSelectionOptional = true;
                    break;
            }

            var variableDefinitionBuilder = new QuotaVariableDefinitionBuilder(
                Guid.NewGuid(),
                variableName,
                odinVariableName,
                levelNames,
                isSelectionOptional,
                isMulti,
                isTargetable,
                isForAllocationOnly
                );
            Add(variableDefinitionBuilder);
            return this;
        }

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "We want variableName in lowercase and we don't care about locale")]
        public QuotaFrameBuilder VariableDefinition(
            string variableName,
            IEnumerable<string> levelNames,
            VariableSelection selection = VariableSelection.NotApplicable,
            bool isMulti = false,
            bool isTargetable = false,
            bool isForAllocationOnly = false)
        {
            Ensure.ArgumentNotNull(variableName, nameof(variableName));

            return VariableDefinition(variableName, variableName.ToLowerInvariant(), levelNames, selection, isMulti, isTargetable, isForAllocationOnly);
        }

        public QuotaFrameBuilder Structure(
            Action<QuotaFrameStructureBuilder> buildAction)
        {
            Ensure.ArgumentNotNull(buildAction, nameof(buildAction));

            buildAction(_structureBuilder);
            return this;
        }
    }
}