namespace Nfield.Quota.Builders2
{
    public class QuotaLevelDefinitionBuilder
    {
        private readonly string _id;
        private readonly string _name;

        public QuotaLevelDefinitionBuilder(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public void Build(QuotaVariableDefinition variableDefinition)
        {
            var level = new QuotaLevelDefinition
            {
                Id = _id,
                Name = _name
            };
            variableDefinition.Levels.Add(level);
        }
    }
}