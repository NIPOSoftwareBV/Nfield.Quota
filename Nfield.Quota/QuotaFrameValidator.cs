using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nfield.Quota.Helpers;

namespace Nfield.Quota
{
    public class QuotaFrameValidator : AbstractValidator<QuotaFrame>
    {
        public QuotaFrameValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(qf => qf.VariableDefinitions)
                .Must(HaveUniqueIds)
                    .WithMessage("Quota frame definitions contain a duplicate id. Duplicate id: '{DuplicateValue}'")
                    .WithErrorCode("duplicate-definition-id")
                .Must(HaveUniqueVariableNames)
                    .WithMessage("Quota frame definitions contain a duplicate variable name. Duplicate name: '{DuplicateValue}'")
                    .WithErrorCode("duplicate-variable-name")
                .Must(HaveUniqueLevelNamesPerVariable)
                    .WithMessage("Quota frame definitions contain a duplicate level name. Duplicate name: '{DuplicateValue}'")
                    .WithErrorCode("duplicate-level-name")
                .Must(HaveVariablesWithAtLeastOneLevel)
                    .WithMessage("Quota frame definitions has variables with no levels. Affected variable definition id: '{VariableDefinitionId}'")
                    .WithErrorCode("no-levels")
                .Must(HaveValidOdinVariableName)
                    .WithMessage("Odin variable name invalid. Odin variable names can only contain numbers, letters and '_' and cannot be empty. They can only start with a letter. First character cannot be '_' or a number. Variable definition Id '{DefId}' with name '{DefName}' has an invalid Odin Variable Name '{InvalidOdin}'")
                    .WithErrorCode("invalid-odin-variable-name");

            RuleFor(qf => qf.FrameVariables)
                .Must(HaveUniqueIds)
                    .WithMessage("Quota frame contains a duplicate id. Duplicate id: '{DuplicateValue}'")
                    .WithErrorCode("duplicate-frame-id")
                .Must(ReferenceDefinitions)
                    .WithMessage("Quota frame contains a reference to a non-existing definition. Definition id: '{DefinitionId}'")
                    .WithErrorCode("missing-definition")
                .Must(HaveTheSameLevelsUnderAVariableAsTheLinkedVariableDefinition)
                    .WithMessage("Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: '{AffectedFrameVariableId}', missing level definition id: '{MissingLevelDefinitionId}'")
                    .WithErrorCode("missing-level")
                .Must(HaveValidTotalTarget)
                    .WithMessage("Target invalid. All Targets must be of a positive value. Quota frame total target has a negative value '{InvalidTarget}'")
                    .WithErrorCode("negative-gross-target")
                .Must(HaveValidLevelTargets)
                    .WithMessage("Target invalid. All Targets must be of a positive value. Frame level Id '{LevelId}' with name '{LevelName}' has an invalid negative target '{InvalidTarget}'")
                    .WithErrorCode("negative-min-target")
                .Must(HaveValidMaxOvershoot)
                    .WithMessage("Max overshoot '{MaxOvershoot}' is invalid for frame level id '{LevelId}' with name '{LevelName}'. Max overshoot values can only be set on leaf nodes, and must be positive.")
                .Must(HaveValidLevelMaxTargets)
                    .WithMessage("Target invalid. All Targets must be of a positive value. Frame level Id '{LevelId}' with name '{LevelName}' has an invalid negative maximum target '{InvalidTarget}'")
                    .WithErrorCode("negative-max-target")
                .Must(HaveVariablesWithAtLeastOneVisibleLevel)
                    .WithMessage("Quota frame invalid. Frame has variables with no visible levels. Affected variable name: '{VariableName}'. If you don't care about any levels under variable '{VariableName}', consider hiding that variable instead.")
                    .WithErrorCode("no-visible-levels")
                .Must(MultiVariablesHaveLevelsWithoutVariables)
                    .WithMessage("Quota frame invalid. Multi variable '{VariableName}', level Id '{LevelId}' with name '{LevelName}' has variables")
                    .WithErrorCode("nested-under-multi")
                .Must(HaveConsistentMinAndMaxTargetsForEachLevel)
                    .WithMessage("Quota frame is invalid. Minimum target for level '{LevelName}' under '{VariableName}' (Id '{LevelId}') is greater than the maximum target for that level.")
                    .WithErrorCode("inconsistent-targets")
                .Must(HaveNestedMinLevelsSumToLessThanMaxTargetForEachLevel)
                    .WithMessage("Quota frame is invalid. Minimum targets for nested levels under variable '{VariableName}' with id '{VariableId}' require more completes than the maximum target for parent level '{LevelName}' with id '{LevelId}'. Expected at least {Sum}, but was {MaxTarget}.")
                    .WithErrorCode("nested-levels-exceed-parent-max")
                .Must(HaveNestedMaxLevelsSumToMoreThanMinTargetForEachLevel)
                    .WithMessage("Quota frame is invalid. Maximum targets for nested levels under level '{LevelName}' with id '{LevelId}' sum to less than the minimum target. Expected at most {Sum}, but was {MinTarget}.")
                    .WithErrorCode("nested-levels-less-than-parent-min");
        }

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            IEnumerable<QuotaVariableDefinition> varDefinitions,
            ValidationContext<QuotaFrame> context)
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

        private static bool HaveUniqueIds(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
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
                (variable, level) =>
                {
                    if (IsDuplicateValue(context, usedIds, level.Id))
                    {
                        hasDuplicate = true;
                    }
                });

            return !hasDuplicate;
        }

        private static bool HaveUniqueVariableNames(
            QuotaFrame frame,
            IEnumerable<QuotaVariableDefinition> varDefinitions,
            ValidationContext<QuotaFrame> context)
        {
            var usedNames = new HashSet<string>();

            foreach (var variableDefinition in varDefinitions)
            {
                if (IsDuplicateValue(context, usedNames, variableDefinition.Name))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HaveUniqueLevelNamesPerVariable(
            QuotaFrame frame,
            IEnumerable<QuotaVariableDefinition> varDefinitions,
            ValidationContext<QuotaFrame> context)
        {
            foreach (var variableDefinition in varDefinitions)
            {
                var usedNames = new HashSet<string>();

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
            ValidationContext<QuotaFrame> context)
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

        private static bool HaveValidOdinVariableName(
            QuotaFrame frame,
            ICollection<QuotaVariableDefinition> varDefinitions,
            ValidationContext<QuotaFrame> context)
        {
            // don't allow unicode whitespace or special characters
            // For more explanation why we do this see this link https://stackoverflow.com/questions/16416610/replace-unicode-space-characters
            var invalidExpression = new Regex(
                @"^[^ \u00A0\u1680​\u180e\u2000-\u2009\u200a​\u200b​\u202f\u205f​\u3000.,\/#!$%\^&\*;:{}=\-`~()@<>|'""\+\\\[\]\?]+$");
            var startsWithExpression = new Regex(
                @"^[_0-9]+");

            foreach (var quotaVariableDefinition in varDefinitions)
            {
                if (quotaVariableDefinition.OdinVariableName != null
                    && !startsWithExpression.Match(quotaVariableDefinition.OdinVariableName).Success
                    && invalidExpression.Match(quotaVariableDefinition.OdinVariableName).Success)
                {
                    continue;
                }

                context.MessageFormatter.AppendArgument("DefId", quotaVariableDefinition.Id);
                context.MessageFormatter.AppendArgument("DefName", quotaVariableDefinition.Name);
                context.MessageFormatter.AppendArgument("InvalidOdin", quotaVariableDefinition.OdinVariableName);
                return false;
            }
            return true;
        }

        private static bool ReferenceDefinitions(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
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
                (variable, level) =>
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
            ValidationContext<QuotaFrame> context)
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

        private static bool HaveValidTotalTarget(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            ValidationContext<QuotaFrame> context)
        {
            var inValidTarget = false;
            if (frame.Target < 0)
            {
                context.MessageFormatter.AppendArgument("InvalidTarget", frame.Target);
                inValidTarget = true;
            }

            return !inValidTarget;
        }

        private static bool HaveValidMaxOvershoot(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            ValidationContext<QuotaFrame> context)
        {

            // note: the frame itself cannot have max overshoot, because
            // it is never a leaf node

            var traverser = new PreOrderQuotaFrameTraverser();

            bool allOk = true;
            traverser.Traverse(frame, (variable, level) =>
            {
                allOk &= ValidateQuotaCell(level, context);
            });

            return allOk;
        }

        private static bool ValidateQuotaCell(IQuotaCell cell, ValidationContext<QuotaFrame> context)
        {
            var isPositive = !cell.MaxOvershoot.HasValue || cell.MaxOvershoot.Value >= 0;
            var isLeafOrEmpty = !cell.Variables.Any() || cell.MaxOvershoot == null;

            if (!isPositive || !isLeafOrEmpty)
            {
                context.MessageFormatter.AppendArgument("LevelId", cell.Id);
                context.MessageFormatter.AppendArgument("LevelName", cell.Name);
                context.MessageFormatter.AppendArgument("MaxOvershoot", cell.MaxOvershoot);

                return false;
            }

            return true;
        }

        private static bool HaveValidLevelTargets(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            ValidationContext<QuotaFrame> context)
        {
            var inValidTarget = false;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                (variable, level) =>
                {
                    if (!(level.Target < 0)) return;
                    context.MessageFormatter.AppendArgument("LevelId", level.Id);
                    context.MessageFormatter.AppendArgument("LevelName", level.Name);
                    context.MessageFormatter.AppendArgument("InvalidTarget", level.Target);
                    inValidTarget = true;
                });

            return !inValidTarget;
        }

        private static bool HaveValidLevelMaxTargets(
            QuotaFrame frame,
            ICollection<QuotaFrameVariable> frameVariables,
            ValidationContext<QuotaFrame> context)
        {
            var inValidTarget = false;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                (variable, level) =>
                {
                    if (!(level.MaxTarget < 0)) return;
                    context.MessageFormatter.AppendArgument("LevelId", level.Id);
                    context.MessageFormatter.AppendArgument("LevelName", level.Name);
                    context.MessageFormatter.AppendArgument("InvalidTarget", level.MaxTarget);
                    inValidTarget = true;
                });

            return !inValidTarget;
        }

        private static bool HaveVariablesWithAtLeastOneVisibleLevel(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
        {
            var isValid = true;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                variable =>
                {
                    // if the variable is hidden, we don't care about the children
                    // but if it isn't, at least one child should be visible also
                    if (!variable.IsHidden && !variable.Levels.Any(l => !l.IsHidden))
                    {
                        context.MessageFormatter.AppendArgument("VariableName", variable.Name);
                        isValid = false;
                    }
                });

            return isValid;
        }

        private static bool HaveConsistentMinAndMaxTargetsForEachLevel(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
        {
            var isValid = true;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse(
                frame,
                (variable, level) =>
                {
                    if (level.Target > level.MaxTarget)
                    {
                        context.MessageFormatter.AppendArgument("VariableName", variable.Name);
                        context.MessageFormatter.AppendArgument("LevelName", level.Name);
                        context.MessageFormatter.AppendArgument("LevelId", level.Id);
                        isValid = false;
                    }
                });

            return isValid;
        }

        private static bool MultiVariablesHaveLevelsWithoutVariables(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
        {
            var isValid = true;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse( // always walks whole tree, might want to change this
                frame,
                (variable, level) =>
                {
                    var currentVariable = frame.VariableDefinitions.First(vd => vd.Id == variable.DefinitionId);
                    if (currentVariable.IsMulti && level.Variables.Count > 0)
                    {
                        context.MessageFormatter.AppendArgument("VariableName", currentVariable.Name);
                        context.MessageFormatter.AppendArgument("LevelId", level.Id);
                        context.MessageFormatter.AppendArgument("LevelName", level.Name);
                        isValid = false;
                    }
                });

            return isValid;
        }

        private static bool HaveNestedMinLevelsSumToLessThanMaxTargetForEachLevel(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
        {
            bool ProcessLevel(QuotaFrameVariable variable, IEnumerable<IQuotaCell> parents)
            {
                var definition = frame.VariableDefinitions.Single(d => d.Id == variable.DefinitionId);

                // sum of targets, ignoring null values
                var minimumCompletesForChildren = 0;

                if (definition.IsMulti)
                {
                    // for multi variables, each level can be selected at the same time, so
                    // must take the maximum of the targets
                    minimumCompletesForChildren = variable.Levels.Aggregate(0, (total, level) => Math.Max(total, (level.Target ?? 0)));
                }
                else
                {
                    // for normal non-multi variables, each level must be achieved, so
                    // we must take the sum of the targets
                    minimumCompletesForChildren = variable.Levels.Aggregate(0, (total, level) => total + (level.Target ?? 0));
                }

                var isValid = true;
                foreach (var parent in parents)
                {
                    if (parent.MaxTarget != null && parent.MaxTarget < minimumCompletesForChildren)
                    {
                        context.MessageFormatter.AppendArgument("VariableName", variable.Name);
                        context.MessageFormatter.AppendArgument("VariableId", variable.Id);
                        context.MessageFormatter.AppendArgument("LevelId", parent.Id);
                        context.MessageFormatter.AppendArgument("LevelName", parent.Name);
                        context.MessageFormatter.AppendArgument("Sum", minimumCompletesForChildren);
                        context.MessageFormatter.AppendArgument("MaxTarget", parent.MaxTarget);

                        isValid = false;
                    }
                }

                foreach (var level in variable.Levels)
                {
#pragma warning disable S1854 // Unused assignments should be removed
                    var newParents = parents.Concat(new[] { level });
#pragma warning restore S1854 // Unused assignments should be removed

                    foreach (var nestedVariable in level.Variables)
                    {
                        isValid &= ProcessLevel(nestedVariable, newParents);
                    }
                }

                return isValid;
            }

            var isTreeValid = true;
            foreach (var variable in variables)
            {
                isTreeValid &= ProcessLevel(variable, new [] { frame });
            }

            return isTreeValid;
        }

        private static bool HaveNestedMaxLevelsSumToMoreThanMinTargetForEachLevel(
            QuotaFrame frame,
            IEnumerable<QuotaFrameVariable> variables,
            ValidationContext<QuotaFrame> context)
        {
            var isTreeValid = true;

            var traverser = new PreOrderQuotaFrameTraverser();
            traverser.Traverse(frame, (variable, level) =>
            {
                if (level.Target == null)
                {
                    return;
                }

                var maxFromChildren = level.CalculateMaxAllowed(frame);

                if (level.Target > maxFromChildren)
                {
                    isTreeValid = false;

                    context.MessageFormatter.AppendArgument("LevelId", level.Id);
                    context.MessageFormatter.AppendArgument("LevelName", level.Name);
                    context.MessageFormatter.AppendArgument("Sum", maxFromChildren);
                    context.MessageFormatter.AppendArgument("MinTarget", level.Target);
                }
            });

            return isTreeValid;
        }

        // Assumes set.Add returns false if value already in collection
        private static bool IsDuplicateValue<TFrame, T>(ValidationContext<TFrame> context, ISet<T> set, T entry)
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
