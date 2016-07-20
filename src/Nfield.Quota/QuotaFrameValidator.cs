using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Nfield.Quota
{
    public class QuotaFrameValidator : AbstractValidator<QuotaFrame>
    {
        public QuotaFrameValidator()
        {
            // Note: This only holds for rules defined after the SAME RuleFor() call
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueIds)
                .WithMessage("Quota frame definitions contain a duplicate id. Duplicate id: '{DuplicateValue}'")
                .Must(HaveUniqueNames)
                .WithMessage("Quota frame definitions contain a duplicate name. Duplicate name: '{DuplicateValue}'")
                .Must(HaveVariablesWithAtLeastOneLevel)
                .WithMessage("Quota frame definitions has variables with no levels. Affected variable definition id: '{VariableDefinitionId}'");

            RuleFor(qf => qf.FrameVariables)
                .Must(HaveUniqueIds)
                .WithMessage("Quota frame contains a duplicate id. Duplicate id: '{DuplicateValue}'")
                .Must(ReferenceDefinitions)
                .WithMessage(
                    "Quota frame contains a reference to a non-existing definition. Definition id: '{DefinitionId}'")
                .Must(HaveTheSameLevelsUnderAVariableAsTheLinkedVariableDefinition)
                .WithMessage(
                    "Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: '{AffectedFrameVariableId}', missing level definition id: '{MissingLevelDefinitionId}'")
                .Must(HaveVariablesWithTheSameVariablesUnderEveryLevel)
                .WithMessage(
                    "Quota frame invalid. All levels of a variable should have the same variables underneath. Frame variable id '{AffectedFrameVariableId}' has a mismatch for level '{MismatchLevelId}'")
                .Must(HaveValidTotalTarget)
                .WithMessage(
                    "Target invalid. All Targets must be of a positive value. Quota frame total target has a negative value '{InvalidTarget}'")
                .Must(HaveValidLevelTargets)
                .WithMessage(
                    "Target invalid. All Targets must be of a positive value. Frame level Id '{LevelId}' with name '{LevelName}' has an invalid negative target '{InvalidTarget}'")
                .Must(HaveValidOdinVariableName)
                .WithMessage(
                    "Odin variable name invalid. Odin variable names can only contain numbers, letters and '_'. They can only ​start with​ a letter. First character cannot be '_' or a number. Variable definition Id '{DefId}' with name '{DefName}' has an invalid Odin Variable Name '{InvalidOdin}'");
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            IEnumerable<QuotaVariableDefinition> varDefinitions,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<Guid>();

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
            IEnumerable<QuotaVariableDefinition> varDefinitions,
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

        private static bool HaveVariablesWithAtLeastOneLevel(
            QuotaFrame frame,
            IEnumerable<QuotaVariableDefinition> varDefinitions,
            PropertyValidatorContext context)
        {
            foreach (var varDefinition in varDefinitions)
            {
                if (varDefinition.Levels.Count < 1)
                {
                    context.MessageFormatter.AppendArgument("VariableDefinitionId", varDefinition.Id);
                    return false;
                }
            }

            return true;
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            PropertyValidatorContext context)
        {
            var usedIds = new HashSet<Guid>();

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
            IEnumerable<QuotaFrameVariable> variables,
            PropertyValidatorContext context)
        {
            var variableIds = new HashSet<Guid>(
                frame.VariableDefinitions.Select(vd => vd.Id));
            var levelIds = new HashSet<Guid>(
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
            IEnumerable<QuotaFrameVariable> variables,
            PropertyValidatorContext context)
        {
            var hasMissingLevel = false;
            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    var varDefLevelIds = frame.VariableDefinitions
                        .First(vd => vd.Id == variable.DefinitionId)
                        .Levels.Select(l => l.Id);
                    var frameVarLevelIds = variable.Levels.Select(l => l.DefinitionId);

                    var complement = varDefLevelIds.Except(frameVarLevelIds).ToList(); // Present in lhs, not in rhs
                    if (complement.Any())
                    {
                        context.MessageFormatter.AppendArgument("AffectedFrameVariableId", variable.Id);
                        context.MessageFormatter.AppendArgument("MissingLevelDefinitionId", complement.First());
                        hasMissingLevel = true;
                    }
                });


            return !hasMissingLevel;
        }

        private static bool HaveVariablesWithTheSameVariablesUnderEveryLevel(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            PropertyValidatorContext context)
        {
            var hasInvalidChilds = false;
            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    var requiredDirectVarDefIds = variable.Levels
                        .First().Variables.Select(v => v.DefinitionId) // assume first is leading
                        .ToList();

                    foreach (var level in variable.Levels.Skip(1)) // Skip first
                    {
                        var levelDirectVarDefIds = level.Variables.Select(v => v.DefinitionId);
                        
                        if (!requiredDirectVarDefIds.SequenceEqual(levelDirectVarDefIds))
                        {
                            context.MessageFormatter.AppendArgument("AffectedFrameVariableId", variable.Id);
                            context.MessageFormatter.AppendArgument("MismatchLevelId", level.Id);
                            hasInvalidChilds = true;
                        }
                    }
                });


            return !hasInvalidChilds;
        }

        private static bool HaveValidTotalTarget(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            PropertyValidatorContext context)
        {
            var inValidTarget = false;
            if (frame.Target < 0)
            {
                context.MessageFormatter.AppendArgument("InvalidTarget", frame.Target);
                inValidTarget = true;
            }

           return !inValidTarget;
        }

        private static bool HaveValidLevelTargets(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            PropertyValidatorContext context)
        {
            var inValidTarget = false;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                level =>
                {
                    if (!(level.Target < 0)) return;
                    context.MessageFormatter.AppendArgument("LevelId", level.Id);
                    context.MessageFormatter.AppendArgument("LevelName", level.Name);
                    context.MessageFormatter.AppendArgument("InvalidTarget", level.Target);
                    inValidTarget = true;
                });

            return !inValidTarget;
        }

        private static bool HaveValidOdinVariableName(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            PropertyValidatorContext context)
        {
            var expression =new Regex("^([a-zA-Z][a-zA-Z0-9_]*)?$");

            foreach (var quotaVariableDefinition in frame.VariableDefinitions)
            {
                if (expression.Match(quotaVariableDefinition.OdinVariableName).Success) continue;
                context.MessageFormatter.AppendArgument("DefId", quotaVariableDefinition.Id);
                context.MessageFormatter.AppendArgument("DefName", quotaVariableDefinition.Name);
                context.MessageFormatter.AppendArgument("InvalidOdin", quotaVariableDefinition.OdinVariableName);
                return false;
            }
            return true;
        }
        
        // Assumes set.Add returns false if value already in collection
        private static bool IsDuplicateValue<T>(PropertyValidatorContext context, ISet<T> set, T entry)
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
