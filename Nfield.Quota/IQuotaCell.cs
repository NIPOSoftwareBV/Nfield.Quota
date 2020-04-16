using System;
using System.Collections.Generic;

namespace Nfield.Quota
{
    //
    // this interface is used to share logic between the quota frame (which has only a max target)
    // and quota levels.
    // before this interface, target-related validation logic had to be duplicated to work in
    // both contexts.
    // a better solution would be to have an explicit "root level" at the root of the frame, and
    // to remove the target property from the quota frame. however, this is not a backwards compatible
    // change, so we settle for this compromise.
    //
    internal interface IQuotaCell
    {
        string Name { get; }
        int? Target { get; }
        int? MaxTarget { get; }
        int? MaxOvershoot { get; }
        Guid Id { get; }

        IEnumerable<QuotaFrameVariable> Variables { get; }
    }
}