using System;
using System.Linq;

namespace Nfield.Quota.Helpers
{
    internal static class FrameExtensions
    {
        public static int CalculateMaxAllowed(this QuotaFrameLevel level, QuotaFrame frame)
        {
            if (level.Variables.Any())
            {
                var maxForChildren = level.Variables.Aggregate(0, (total, variable) =>
                {
                    var variableMax = variable.CalculateMaxAllowed(frame);

                    return Math.Max(variableMax, total);
                });

                return Math.Min(level.MaxTarget ?? int.MaxValue, maxForChildren);
            }

            return level.MaxTarget ?? int.MaxValue;
        }

        public static int CalculateMaxAllowed(this QuotaFrameVariable variable, QuotaFrame frame)
        {
            return variable.Levels.Aggregate(0, (total, level) =>
            {
                var levelMax = level.CalculateMaxAllowed(frame);

                if (levelMax == int.MaxValue)
                {
                    return int.MaxValue;
                }

                return levelMax + total;
            });
        }
    }
}
