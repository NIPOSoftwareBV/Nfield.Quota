namespace Nfield.Quota.Persistence
{
    /// <summary>
    /// Options for the <see cref="QuotaFrameEncoder"/>
    /// </summary>
    public class QuotaFrameEncoderOptions
    {
        /// <summary>
        /// The default options to use when not supply any options to the encoder
        /// </summary>
        public static readonly QuotaFrameEncoderOptions Default = new QuotaFrameEncoderOptions
        {
            IncludeTargets = false
        };

        /// <summary>
        /// Indication whether the encoder output should contain the target and/or maxTargets
        /// </summary>
        public bool IncludeTargets { get; set; }
    }
}
