namespace Nfield.Quota
{
    public class QuotaFrameLevel : IQuotaFrameAttached
    {
        public QuotaFrameLevel()
        {
            Variables = new QuotaFrameVariableCollection();
        }

        public QuotaFrame QuotaFrame { get; private set; }

        public string Id { get; set; }

        public string DefinitionId { get; set; }

        public QuotaFrameVariableCollection Variables { get; }

        public int? Target { get; set; }

        public int Successful { get; set; }

        void IQuotaFrameAttached.AttachTo(QuotaFrame quotaFrame)
        {
            QuotaFrame = quotaFrame;
            ((IQuotaFrameAttached)Variables).AttachTo(quotaFrame);
        }
    }
}