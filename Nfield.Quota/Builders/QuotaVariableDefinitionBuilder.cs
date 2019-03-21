using System;
using System.Collections.Generic;

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

        public QuotaVariableDefinitionBuilder(
            Guid id,
            string name,
            string odinVariableName,
            IEnumerable<string> levelNames,
            bool? isSelectionOptional = null,
            bool isMulti = false)
        {
            _id = id;
            _name = name;
            _odinVariableName = odinVariableName;
            _levelNames = levelNames;
            _isSelectionOptional = isSelectionOptional;
            _isMulti = isMulti;
        }

        public void Build(QuotaFrame quotaFrame)
        {
            var variable = new QuotaVariableDefinition
            {
                Id = _id,
                Name = _name,
                OdinVariableName = _odinVariableName,
                IsSelectionOptional = _isSelectionOptional,
                IsMulti = _isMulti
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