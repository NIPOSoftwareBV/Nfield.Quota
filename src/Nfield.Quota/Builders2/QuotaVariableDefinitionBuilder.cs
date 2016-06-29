using System.Collections.Generic;

namespace Nfield.Quota.Builders2
{
    public class QuotaVariableDefinitionBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _odinVariableName;
        private readonly List<QuotaLevelDefinitionBuilder> _levelDefinitionBuilders = new List<QuotaLevelDefinitionBuilder>();

        public QuotaVariableDefinitionBuilder(string id, string name, string odinVariableName)
        {
            _id = id;
            _name = name;
            _odinVariableName = odinVariableName;
        }

        public void Add(QuotaLevelDefinitionBuilder builder)
        {
            _levelDefinitionBuilders.Add(builder);
        }

        public void Build(QuotaFrame quotaFrame)
        {
            var variable = new QuotaVariableDefinition
            {
                Id = _id,
                Name = _name,
                OdinVariableName = _odinVariableName
            };
            quotaFrame.VariableDefinitions.Add(variable);

            foreach (var builder in _levelDefinitionBuilders)
            {
                builder.Build(variable);
            }
        }

        public void Level(string id)
        {
            Level(id, id);
        }

        public void Level(string id, string name)
        {
            var levelBuilder = new QuotaLevelDefinitionBuilder(id, name);
            Add(levelBuilder);
        }

    }
}