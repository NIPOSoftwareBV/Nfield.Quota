using System;
using System.Collections.Generic;

namespace Nfield.Quota
{
    //todo immutable
    public class PreOrderQuotaFrameTraverser
    {
        public QuotaFrame Traverse(
            QuotaFrame frame,
            Action<QuotaFrameVariable, QuotaFrameLevel> levelOperation)
        {
            return Traverse(
                frame,
                variable => { }, // no-op for variable
                levelOperation);
        }

        public QuotaFrame Traverse(
            QuotaFrame frame,
            Action<QuotaFrameVariable> variableOperation)
        {
            return Traverse(
                frame,
                variableOperation,
                (variable, level) => { } // no-op for level
                );
        }

        public QuotaFrame Traverse(
            QuotaFrame frame,
            Action<QuotaFrameVariable> variableOperation,
            Action<QuotaFrameVariable, QuotaFrameLevel> levelOperation)
        {
            var context = new ActionContext
            {
                VariableOperation = variableOperation,
                LevelOperation = levelOperation
            };

            Visit(context, frame.FrameVariables);

            return frame;
        }

        private void Visit(ActionContext context, IEnumerable<QuotaFrameVariable> variables)
        {
            if (variables == null)
            {
                return; // stop
            }

            foreach (var variable in variables)
            {
                context.VariableOperation(variable);
                Visit(context, variable, variable.Levels);
            }
        }

        private void Visit(ActionContext context, QuotaFrameVariable variable, IEnumerable<QuotaFrameLevel> levels)
        {
            if (levels == null)
            {
                return; // stop
            }

            foreach (var level in levels)
            {
                context.LevelOperation(variable, level);
                Visit(context, level.Variables);
            }
        }

        private class ActionContext
        {
            /// <summary>
            /// Gets passed in the variable to process
            /// </summary>
            public Action<QuotaFrameVariable> VariableOperation { get; set; }
            /// <summary>
            /// Gets passed in the variable and the level to process
            /// </summary>
            public Action<QuotaFrameVariable, QuotaFrameLevel> LevelOperation { get; set; }
        }
    }
}