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

        public QuotaVariableDefinitionBuilder(
            Guid id, string name, string odinVariableName, IEnumerable<string> levelNames)
        {
            _id = id;
            _name = name;
            _odinVariableName = odinVariableName;
            _levelNames = levelNames;
        }

        public void Build(QuotaFrame quotaFrame)
        {
            //test
            var variable = new QuotaVariableDefinition
            {
                Id = _id,
                Name = _name,
                OdinVariableName = _odinVariableName
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