using System;
using System.Linq;

namespace Nfield.Quota.Helpers
{
    internal static class FrameExtensions
    {
        public static int CalculateMaxAllowed(this QuotaFrameLevel level, QuotaFrame frame)
        {
            // if there are no nested variables, the max allowed is just the max target
            if (level.Variables.Any())
            {
                // each nested variable's levels can be filled independently, so we
                // only want to consider the highest one (that's the one that limits
                // the global number of completes)
                var maxForChildren = level.Variables.Aggregate(0, (total, variable) =>
                {
                    // recursive step.. see other method below
                    var variableMax = variable.CalculateMaxAllowed(frame);

                    return Math.Max(variableMax, total);
                });

                // if the parent level ('level' here) has a higher max target than
                // its children, the children still limit the global number of
                // completes. so we must take the minimum of these two values
                return Math.Min(level.MaxTarget ?? int.MaxValue, maxForChildren);
            }

            // if the max target is not set, that is equivalent to having an
            // "infinite" max target, so we just pick a pretty big number :)
            return level.MaxTarget ?? int.MaxValue;
        }

        public static int CalculateMaxAllowed(this QuotaFrameVariable variable, QuotaFrame frame)
        {
            // the maximum possible number of completes for all child levels is the
            // sum of the max targets of the levels (even for multi questions).
            return variable.Levels.Aggregate(0, (total, level) =>
            {
                // recursive step.. see other method above
                var levelMax = level.CalculateMaxAllowed(frame);

                // if any of the child levels had a null target, the max target will
                // be 'infinite' (actually just the max int value). we don't want to
                // overflow so we check for that.
                if (levelMax == int.MaxValue || total == int.MaxValue)
                {
                    return int.MaxValue;
                }

                return levelMax + total;
            });
        }
    }
}
