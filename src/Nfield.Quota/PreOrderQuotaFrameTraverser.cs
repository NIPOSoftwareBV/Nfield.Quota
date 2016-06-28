using System;
using System.Collections.Generic;

namespace Nfield.Quota
{
    //todo immutable
    public class PreOrderQuotaFrameTraverser
    {
        public QuotaFrame Traverse(
            QuotaFrame frame,
            Action<QuotaFrameLevel> levelOperation)
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
                level => { } // no-op for level
                );
        }

        public QuotaFrame Traverse(
            QuotaFrame frame,
            Action<QuotaFrameVariable> variableOperation,
            Action<QuotaFrameLevel> levelOperation)
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
                Visit(context, variable.Levels);
            }
        }

        private void Visit(ActionContext context, IEnumerable<QuotaFrameLevel> levels)
        {
            if (levels == null)
            {
                return; // stop
            }

            foreach (var level in levels)
            {
                context.LevelOperation(level);
                Visit(context, level.Variables);
            }
        }

        private class ActionContext
        {
            public Action<QuotaFrameVariable> VariableOperation { get; set; }
            public Action<QuotaFrameLevel> LevelOperation { get; set; }
        }
    }
}