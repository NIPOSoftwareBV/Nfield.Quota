using System.Collections.ObjectModel;

namespace Nfield.Quota
{
    public class QuotaFrameVariableCollection : Collection<QuotaFrameVariable>, IQuotaFrameAttached
    {
        public QuotaFrameVariableCollection()
        {
        }

        public QuotaFrameVariableCollection(QuotaFrame quotaFrame)
        {
            QuotaFrame = quotaFrame;
        }

        public QuotaFrame QuotaFrame { get; private set; }

        void IQuotaFrameAttached.AttachTo(QuotaFrame quotaFrame)
        {
            QuotaFrame = quotaFrame;

            foreach (var item in Items)
            {
                ((IQuotaFrameAttached)item).AttachTo(quotaFrame);
            }
        }


        protected override void InsertItem(int index, QuotaFrameVariable item)
        {
            ((IQuotaFrameAttached)item).AttachTo(QuotaFrame);
            base.InsertItem(index, item);
        }
    }
}

