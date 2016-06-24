using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Nfield.Quota
{
    internal class QuotaFrameValidator : AbstractValidator<QuotaFrame>
    {
        public QuotaFrameValidator()
        {
            RuleFor(qf => qf.VariableDefinitions)
                .NotNull().NotEmpty()
                .WithMessage("Quota frame definitions cannot be empty.");

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueIds)
                .WithMessage("Quota frame definitions contain a duplicate id. Duplicate id: '{DuplicateValue}'");

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueNames)
                .WithMessage("Quota frame definitions contain a duplicate name. Duplicate name: '{DuplicateValue}'");

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveVariablesWithAtLeastTwoLevels)
                .WithMessage("Quota frame definitions has variables with less than two or no levels. Affected variable definition id: '{VariableDefinitionId}'");

            RuleFor(qf => qf.FrameVariables)
                .Must(HaveUniqueIds)
                .WithMessage("Quota frame contains a duplicate id. Duplicate id: '{DuplicateValue}'");

            RuleFor(qf => qf.FrameVariables)
                .Must(ReferenceDefinitions)
                .WithMessage("Quota frame contains a reference to a non-existing definition. Definition id: '{DefinitionId}'");

            RuleFor(qf => qf.FrameVariables)
                .Must(HaveTheSameLevelsUnderAVariableAsTheLinkedVariableDefinition)
                .WithMessage("Quota frame contains a variable that doesnt have all the defined levels associated. Affected variable id: '{AffectedVariableId}', missing level definition id: '{MissingLevelDefinitionId}'");
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateValue(context, usedIds, variableDefinition.Id))
                {
                    return false;
                }

                foreach (var levelDefinition in variableDefinition.Levels)
                {
                    if (IsDuplicateValue(context, usedIds, levelDefinition.Id))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool HaveUniqueNames(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            var usedNames = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateValue(context, usedNames, variableDefinition.Name))
                {
                    return false;
                }

                foreach (var levelDefinition in variableDefinition.Levels)
                {
                    if (IsDuplicateValue(context, usedNames, levelDefinition.Name))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool HaveVariablesWithAtLeastTwoLevels(
            QuotaFrame frame,
            QuotaVariableDefinitionCollection varDefinitions,
            PropertyValidatorContext context)
        {
            foreach (var varDefinition in varDefinitions)
            {
                if (varDefinition.Levels.Count < 2)
                {
                    context.MessageFormatter.AppendArgument("VariableDefinitionId", varDefinition.Id);
                    return false;
                }
            }

            return true;
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            QuotaFrameVariableCollection variables,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<string>();

            var hasDuplicate = false;
            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    if (IsDuplicateValue(context, usedIds, variable.Id))
                    {
                        hasDuplicate = true;
                    }
                },
                level =>
                {
                    if (IsDuplicateValue(context, usedIds, level.Id))
                    {
                        hasDuplicate = true;
                    }
                });

            return !hasDuplicate;
        }

        private static bool ReferenceDefinitions(
            QuotaFrame frame,
            QuotaFrameVariableCollection variables,
            PropertyValidatorContext context)
        {
            var variableIds = new HashSet<string>(
                frame.VariableDefinitions.Select(vd => vd.Id));
            var levelIds = new HashSet<string>(
                frame.VariableDefinitions.SelectMany(vd => vd.Levels).Select(ld => ld.Id));

            var traverser = new PreOrderQuotaFrameTraverser();
            var hasInvalidReference = false;
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    if (!variableIds.Contains(variable.DefinitionId))
                    {
                        context.MessageFormatter.AppendArgument("DefinitionId", variable.DefinitionId);
                        hasInvalidReference = true;
                    }
                },
                level =>
                {
                    if (!levelIds.Contains(level.DefinitionId))
                    {
                        context.MessageFormatter.AppendArgument("DefinitionId", level.DefinitionId);
                        hasInvalidReference = true;
                    }
                });

            return !hasInvalidReference;
        }

        private static bool HaveTheSameLevelsUnderAVariableAsTheLinkedVariableDefinition(
            QuotaFrame frame,
            QuotaFrameVariableCollection variables,
            PropertyValidatorContext context)
        {
            bool hasMissingLevel = false;
            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    var levelIds = variable.Levels.Select(l => l.Id);
                    var levelDefIds = frame.VariableDefinitions
                        .First(vd => vd.Id == variable.DefinitionId)
                        .Levels.Select(l => l.Id);

                    var complement = levelDefIds.Except(levelIds).ToList(); // Present in lhs, not in rhs
                    if (complement.Any())
                    {
                        context.MessageFormatter.AppendArgument("AffectedVariableId", variable.Id);
                        context.MessageFormatter.AppendArgument("MissingLevelDefinitionId", complement.First());
                        hasMissingLevel = true;
                    }
                });


            return !hasMissingLevel;
        }

        // Assumes set.Add returns false if value already in collection
        private static bool IsDuplicateValue(PropertyValidatorContext context, ISet<string> set, string entry)
        {
            var couldAdd = set.Add(entry);
            if (!couldAdd)
            {
                context.MessageFormatter.AppendArgument("DuplicateValue", entry);
                return true;
            }

            return false;
        }
    }
}
