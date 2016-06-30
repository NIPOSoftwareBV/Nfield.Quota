using System;
using System.Collections.Generic;

namespace Nfield.Quota.Builders
{
    public class QuotaVariableDefinitionBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _odinVariableName;
        private readonly IEnumerable<string> _levelNames;

        public QuotaVariableDefinitionBuilder(
            string id, string name, string odinVariableName, IEnumerable<string> levelNames)
        {
            _id = id;
            _name = name;
            _odinVariableName = odinVariableName;
            _levelNames = levelNames;
        }

        public void Build(QuotaFrame quotaFrame)
        {
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
                    Id = Guid.NewGuid().ToString(),
                    Name = levelName
                };
                variable.Levels.Add(level);
            }

            quotaFrame.VariableDefinitions.Add(variable);
        }
    }
}