using System;
using System.Collections.Generic;
using Nfield.Quota.Helpers;

namespace Nfield.Quota.Builders
{
    public class QuotaVariableDefinitionBuilder
    {
        private readonly Guid _id;
        private readonly string _name;
        private readonly string _odinVariableName;
        private readonly IEnumerable<string> _levelNames;
        private readonly bool? _isSelectionOptional;
        private readonly bool _isMulti;
        private readonly bool _isTargetable;
        private readonly bool _isForAllocationOnly;

        public QuotaVariableDefinitionBuilder(
            Guid id,
            string name,
            string odinVariableName,
            IEnumerable<string> levelNames,
            bool? isSelectionOptional = null,
            bool isMulti = false,
            bool isTargetable = false,
            bool isForAllocationOnly = false)
        {
            _id = id;
            _name = name;
            _odinVariableName = odinVariableName;
            _levelNames = levelNames;
            _isSelectionOptional = isSelectionOptional;
            _isMulti = isMulti;
            _isTargetable = isTargetable;
            _isForAllocationOnly = isForAllocationOnly;
        }

        public void Build(QuotaFrame quotaFrame)
        {
            Ensure.ArgumentNotNull(quotaFrame, nameof(quotaFrame));

            var variable = new QuotaVariableDefinition
            {
                Id = _id,
                Name = _name,
                OdinVariableName = _odinVariableName,
                IsSelectionOptional = _isSelectionOptional,
                IsMulti = _isMulti,
                IsTargetable = _isTargetable,
                IsForAllocationOnly = _isForAllocationOnly
            };

            foreach (var levelName in _levelNames)
            {
                var level = new QuotaLevelDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = levelName
                };
                variable.Levels.Add(level);
            }

            quotaFrame.VariableDefinitions.Add(variable);
        }
    }
}