namespace Nfield.Quota
{
    public class QuotaFrame
    {
        public QuotaFrame()
        {
            VariableDefinitions = new QuotaVariableDefinitionCollection(this);
            FrameVariables = new QuotaFrameVariableCollection(this);
        }

        public string Id { get; set; }

        public int? Target { get; set; }

	    public int Successful { get; set; }

        public QuotaVariableDefinitionCollection VariableDefinitions { get;}

        public QuotaFrameVariableCollection FrameVariables { get; }
    }
}
