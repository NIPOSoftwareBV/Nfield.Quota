namespace Nfield.Quota
{
    public class QuotaFrameVariable : IQuotaFrameAttached
    {
        public QuotaFrameVariable()
        {
            Levels = new QuotaFrameLevelCollection();
        }
        public QuotaFrame QuotaFrame { get; private set; }

        public string Id { get; set; }

        public string DefinitionId { get; set; }

        public QuotaFrameLevelCollection Levels { get; }

        void IQuotaFrameAttached.AttachTo(QuotaFrame quotaFrame)
        {
            QuotaFrame = quotaFrame;
            ((IQuotaFrameAttached)Levels).AttachTo(quotaFrame);
        }
    }
}