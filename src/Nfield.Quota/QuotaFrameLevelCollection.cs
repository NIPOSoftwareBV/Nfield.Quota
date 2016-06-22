using System.Collections.ObjectModel;

namespace Nfield.Quota
{
    public class QuotaFrameLevelCollection : Collection<QuotaFrameLevel>, IQuotaFrameAttached
    {
        public QuotaFrame QuotaFrame { get; private set; }

        void IQuotaFrameAttached.AttachTo(QuotaFrame quotaFrame)
        {
            QuotaFrame = quotaFrame;

            foreach (var item in Items)
            {
                ((IQuotaFrameAttached)item).AttachTo(quotaFrame);
            }
        }

        protected override void InsertItem(int index, QuotaFrameLevel item)
        {
            ((IQuotaFrameAttached)item).AttachTo(QuotaFrame);

            base.InsertItem(index, item);
        }
    }
}
