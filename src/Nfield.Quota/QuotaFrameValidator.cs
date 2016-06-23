using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;

namespace Nfield.Quota
{
    internal class QuotaFrameValidator : AbstractValidator<QuotaFrame>
    {
        public QuotaFrameValidator()
        {
            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueIds)
                .WithMessage("Quota frame definitions contain a duplicate id. Duplicate id: '{DuplicateId}'");

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueNames)
                .WithMessage("Quota frame definitions contain a duplicate name. Duplicate name: '{DuplicateName}'");
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateId(context, usedIds, variableDefinition.Id))
                {
                    return false;
                }

                foreach (var levelDefinition in variableDefinition.Levels)
                {
                    if (IsDuplicateId(context, usedIds, levelDefinition.Id))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsDuplicateId(PropertyValidatorContext context, ISet<string> set, string entry)
        {
            var couldAdd = set.Add(entry);
            if (!couldAdd)
            {
                context.MessageFormatter.AppendArgument("DuplicateId", entry);
                return true;
            }

            return false;
        }

        private static bool HaveUniqueNames(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            var usedNames = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateName(context, usedNames, variableDefinition.Name))
                {
                    return false;
                }

                foreach (var levelDefinition in variableDefinition.Levels)
                {
                    if (IsDuplicateName(context, usedNames, levelDefinition.Name))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsDuplicateName(PropertyValidatorContext context, ISet<string> set, string entry)
        {
            var couldAdd = set.Add(entry);
            if (!couldAdd)
            {
                context.MessageFormatter.AppendArgument("DuplicateName", entry);
                return true;
            }

            return false;
        }
    }
}
