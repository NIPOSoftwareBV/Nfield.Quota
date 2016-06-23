using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;

namespace Nfield.Quota
{
    internal class QuotaFrameValidator : AbstractValidator<QuotaFrame>
    {
        public QuotaFrameValidator()
        {

            RuleFor(qf => qf.VariableDefinitions).Must(HaveUniqueIdsAndNames)
                .WithMessage("Quota frame defintions have duplicate id ({DuplicateId}) or name ({DuplicateName})");
        }

        private bool HaveUniqueIdsAndNames(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<string>();
            var usedNames = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateId(context, usedIds, variableDefinition.Id) &&
                    IsDuplicateName(context, usedNames, variableDefinition.Name))
                {
                    return false;
                }

                foreach (var levelDefinition in variableDefinition.Levels)
                {
                    if (IsDuplicateId(context, usedIds, levelDefinition.Id) &&
                        IsDuplicateName(context, usedNames, levelDefinition.Name))
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        private static bool IsDuplicateId(PropertyValidatorContext context, HashSet<string> collection, string entry)
        {
            var couldAdd = collection.Add(entry);
            if (!couldAdd)
            {
                context.MessageFormatter.AppendArgument("{DuplicateId}", entry);
                return true;
            }

            return false;
        }

        private static bool IsDuplicateName(PropertyValidatorContext context, HashSet<string> collection, string entry)
        {
            var couldAdd = collection.Add(entry);
            if (!couldAdd)
            {
                context.MessageFormatter.AppendArgument("{DuplicateName}", entry);
                return true;
            }

            return false;
        }
    }
}
