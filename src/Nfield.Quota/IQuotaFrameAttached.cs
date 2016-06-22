namespace Nfield.Quota
{
    /// <summary>
    /// Interface to attach to a quota frame to an object
    /// </summary>
    public interface IQuotaFrameAttached
    {
        void AttachTo(QuotaFrame quotaFrame);
    }
}
