namespace Nfield.Quota
{
    public class QuotaVariableDefinition
    {
        public QuotaVariableDefinition()
        {
            Levels = new QuotaLevelDefinitionCollection();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string OdinVariableName { get; set; }

        public QuotaLevelDefinitionCollection Levels { get; }
    }
}