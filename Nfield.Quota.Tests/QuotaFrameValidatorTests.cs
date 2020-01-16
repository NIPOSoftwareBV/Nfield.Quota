using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nfield.Quota.Builders;
using Nfield.Quota.Helpers;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        [Test]
        public void Definitions_HappyPath()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition(
                    "varName", new[] { "level1Name", "level2Name" })
                .Structure(sb => { })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Definitions_EveryVariableNeedsAtLeastOneLevel()
        {
            var variableId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions has variables with no levels. Affected variable definition id: '{0}'",
                variableId);

            var quotaFrame = new QuotaFrame();
            var variable = new QuotaVariableDefinition
            {
                Id = variableId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };
            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CannotContainDuplicateIds()
        {
            var nonUniqueId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = nonUniqueId,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = nonUniqueId,
                    Name = "level2Name"
                }
            });
            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CanContainDuplicateOdinVariableNamesAcrossVariables()
        {
            const string odinVariableName = "my_var";

            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("varName1", odinVariableName, new[] { "level1Name", "level2Name" })
                .VariableDefinition("varName2", odinVariableName, new[] { "level1Name", "level2Name" })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Definitions_CannotContainDuplicateNamesAcrossVariables()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrame();

            var variable1 = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid(),
                Name = nonUniqueName
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid(),
                Name = nonUniqueName
            };

            quotaFrame.VariableDefinitions.AddRange(new[] { variable1, variable2 });

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate variable name. Duplicate name: 'non-unique'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateNamesInLevelsUnderSameVariable()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = nonUniqueName
                },

                new QuotaLevelDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = nonUniqueName
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate level name. Duplicate name: 'non-unique'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateIdsBetweenVariablesAndLevels()
        {
            var nonUniqueId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = nonUniqueId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = nonUniqueId,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CannotHaveInvalidOdinVariableName()
        {
            const string invalidOdinVarName = "_varName";
            const string variableName = "varName";

            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition(
                    "varName", invalidOdinVarName, new[] { "level1Name", "level2Name" })
                .Structure(sb => { })
                .Build();

            var varId = quotaFrame.VariableDefinitions.Single().Id;

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Odin variable name invalid. Odin variable names can only contain numbers, letters and '_' " +
                "and cannot be empty." +
                " They can only ​start with​ a letter. First character cannot be '_' or a number." +
                " Variable definition Id '{0}' with name '{1}' has an invalid Odin Variable Name '{2}'",
                varId, variableName, invalidOdinVarName);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void MultiVariable_LevelsCannotHaveVariables()
        {
            const string multiVariableName = "varMulti";
            const string variableName = "varName";
            const string levelName = "levelName";

            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition(
                    variableName, new[] { levelName })
                .VariableDefinition(
                    multiVariableName, new[] { levelName }, isMulti: true)
                .Structure(root =>
                    root.Variable(multiVariableName,
                        builder => builder.Variable(variableName)))
                .Build();

            var id = quotaFrame.FrameVariables.First().Levels.First().Id;
            var expectedErrorMessage =
                $"Quota frame invalid. Multi variable '{multiVariableName}', level Id '{id}' with name '{levelName}' has variables";

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_HappyPath()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var quotaFrameVariableId = Guid.NewGuid();
            var quotaFrameLevelId1 = Guid.NewGuid();
            var quotaFrameLevelId2 = Guid.NewGuid();


            var quotaFrame = BuildQuotaFrame(varId, level1Id, level2Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        [TestCase(10, 10, true)]
        [TestCase(9, 10, true)]
        [TestCase(11, 10, false)]
        public void Frame_MaxTargetsMustBeGreaterThanOrEqualToMinTargets(int minTarget, int maxTarget, bool isValid)
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("var1", new[] { "a", "b" })
                .Structure(f => f.Variable("var1"))
                .Build();

            var level = quotaFrame["var1", "a"];

            level.Target = minTarget;
            level.MaxTarget = maxTarget;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.EqualTo(isValid));

            if (!isValid)
            {
                var id = level.Id;

                Assert.That(result.Errors.Single().ErrorMessage,
                    Is.EqualTo($"Quota frame is invalid. Minimum target for level 'a' under 'var1' (Id '{id}') is greater than the maximum target for that level."));
            }
        }

        [Test]
        public void Frame_Multi_MaxOfMaxTargetsMustExceedParentMinTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("top", new[] { "a", "b" })
                .VariableDefinition("nested", new[] { "c", "d" }, isMulti: true)
                .Structure(f =>
                    f.Variable("top", (top) =>
                        top.Variable("nested")))
                .Build();

            var topLevel = quotaFrame["top", "a"];
            var nestedVariable = topLevel["nested"];
            var nestedLevel1 = topLevel["nested", "c"];
            var nestedLevel2 = topLevel["nested", "d"];

            // max of nested level max targets is 12
            nestedLevel1.MaxTarget = 10;
            nestedLevel2.MaxTarget = 12;

            topLevel.Target = 11;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Maximum targets for nested levels under variable 'nested' with id '{nestedVariable.Id}' restrict completes to less than the minimum target for parent level 'a' with id '{topLevel.Id}'. Expected at most 10, but was 11."));

            // make sure that if all is good, we don't return an error
            topLevel.Target = 10;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_Multi_MaxOfMinTargetsCannotExceedParentMaxTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("top", new[] { "a", "b" })
                .VariableDefinition("nested", new[] { "c", "d" }, isMulti: true)
                .Structure(f =>
                    f.Variable("top", (top) =>
                        top.Variable("nested")))
                .Build();

            var topLevel = quotaFrame["top", "a"];
            var nestedVariable = topLevel["nested"];
            var nestedLevel1 = topLevel["nested", "c"];
            var nestedLevel2 = topLevel["nested", "d"];

            // max of nested level min targets is 12
            nestedLevel1.Target = 10;
            nestedLevel2.Target = 12;

            topLevel.MaxTarget = 11;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Minimum targets for nested levels under variable 'nested' with id '{nestedVariable.Id}' require more completes than the maximum target for parent level 'a' with id '{topLevel.Id}'. Expected at least 12, but was 11."));

            // make sure that if all is good, we don't return an error
            topLevel.MaxTarget = 12;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_SumOfMinTargetsCannotExceedParentMaxTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("top", new[] { "a", "b" })
                .VariableDefinition("nested", new[] { "c", "d" })
                .Structure(f =>
                    f.Variable("top", (top) =>
                        top.Variable("nested")))
                .Build();

            var topLevel = quotaFrame["top", "a"];
            var nestedVariable = topLevel["nested"];
            var nestedLevel1 = topLevel["nested", "c"];
            var nestedLevel2 = topLevel["nested", "d"];

            // nested level min targets sum to 22
            nestedLevel1.Target = 10;
            nestedLevel2.Target = 12;

            // parent max target is less than the sum of the nested targets
            topLevel.MaxTarget = 20;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Minimum targets for nested levels under variable 'nested' with id '{nestedVariable.Id}' require more completes than the maximum target for parent level 'a' with id '{topLevel.Id}'. Expected at least 22, but was 20."));

            // make sure that if all is good, we don't return an error
            topLevel.MaxTarget = 22;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_SumOfMinTargetsCannotExceedAncestorMaxTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("Top", new[] { "a", "b" })
                .VariableDefinition("Nested", new[] { "c", "d" })
                .VariableDefinition("DoubleNested", new[] { "e", "f" })
                .Structure(f =>
                    f.Variable("Top", top =>
                        top.Variable("Nested", nested =>
                            nested.Variable("DoubleNested"))))
                .Build();

            var topLevel = quotaFrame["Top", "a"];
            var nestedLevel = topLevel["Nested", "c"];

            var doubleNestedVariable = nestedLevel["DoubleNested"];
            var doubleNestedLevel1 = nestedLevel["DoubleNested", "e"];
            var doubleNestedLevel2 = nestedLevel["DoubleNested", "f"];

            // nested level min targets sum to 22
            doubleNestedLevel1.Target = 10;
            doubleNestedLevel2.Target = 12;

            // ancestor max target is less than the sum of the nested targets
            // note: targets for directly nested levels are all null
            topLevel.MaxTarget = 20;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Minimum targets for nested levels under variable 'DoubleNested' with id '{doubleNestedVariable.Id}' require more completes than the maximum target for parent level 'a' with id '{topLevel.Id}'. Expected at least 22, but was 20."));

            // make sure that if all is good, we don't return an error
            topLevel.MaxTarget = 22;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_SumOfMaxTargetsMustExceedParentMinTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("top", new[] { "a", "b" })
                .VariableDefinition("nested", new[] { "c", "d" })
                .Structure(f =>
                    f.Variable("top", (top) =>
                        top.Variable("nested")))
                .Build();

            var topLevel = quotaFrame["top", "a"];
            var nestedVariable = topLevel["nested"];
            var nestedLevel1 = topLevel["nested", "c"];
            var nestedLevel2 = topLevel["nested", "d"];

            // nested level max targets sum to 18
            nestedLevel1.MaxTarget = 10;
            nestedLevel2.MaxTarget = 8;

            // parent min target is more than the sum of the nested max targets
            topLevel.Target = 20;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Maximum targets for nested levels under variable 'nested' with id '{nestedVariable.Id}' restrict completes to less than the minimum target for parent level 'a' with id '{topLevel.Id}'. Expected at most 18, but was 20."));

            // make sure that if all is good, we don't return an error
            topLevel.Target = 18;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_SumOfMaxTargetsMustExceedAncestorMinTarget()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("Top", new[] { "a", "b" })
                .VariableDefinition("Nested", new[] { "c", "d" })
                .VariableDefinition("DoubleNested", new[] { "e", "f" })
                .Structure(f =>
                    f.Variable("Top", top =>
                        top.Variable("Nested", nested =>
                            nested.Variable("DoubleNested"))))
                .Build();

            var topLevel = quotaFrame["Top", "a"];
            var nestedLevel = topLevel["Nested", "c"];

            var doubleNestedVariable = nestedLevel["DoubleNested"];
            var doubleNestedLevel1 = nestedLevel["DoubleNested", "e"];
            var doubleNestedLevel2 = nestedLevel["DoubleNested", "f"];

            // nested level max targets sum to 18
            doubleNestedLevel1.MaxTarget = 10;
            doubleNestedLevel2.MaxTarget = 8;

            // ancestor min target is more than the sum of the nested max targets
            // note: targets for directly nested levels are all null
            topLevel.Target = 20;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo($"Quota frame is invalid. Maximum targets for nested levels under variable 'DoubleNested' with id '{doubleNestedVariable.Id}' restrict completes to less than the minimum target for parent level 'a' with id '{topLevel.Id}'. Expected at most 18, but was 20."));

            // make sure that if all is good, we don't return an error
            topLevel.Target = 18;
            result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Compare_Definitions_Are_Equal()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var quotaFrameVariableId = Guid.NewGuid();
            var quotaFrameLevelId1 = Guid.NewGuid();
            var quotaFrameLevelId2 = Guid.NewGuid();


            var quotaFrame1 = BuildQuotaFrame(varId, level1Id, level2Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);
            var quotaFrame2 = BuildQuotaFrame(varId, level1Id, level2Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);

            var resultVariableDefinitions = quotaFrame1.VariableDefinitions.First() == quotaFrame2.VariableDefinitions.First();
            var resultLevels1 = quotaFrame1.VariableDefinitions.First().Levels.First() == quotaFrame2.VariableDefinitions.First().Levels.First();
            var resultLevels2 = quotaFrame1.VariableDefinitions.First().Levels.First() == quotaFrame2.VariableDefinitions.First().Levels.First();

            Assert.That(resultVariableDefinitions, Is.True);
            Assert.That(resultLevels1, Is.True);
            Assert.That(resultLevels2, Is.True);
        }
        [Test]
        public void Compare_Definitions_Are_Not_Equal()
        {
            var varId1 = Guid.NewGuid();
            var varId2 = Guid.NewGuid();
            var level11Id = Guid.NewGuid();
            var level12Id = Guid.NewGuid();
            var level21Id = Guid.NewGuid();
            var level22Id = Guid.NewGuid();
            var quotaFrameVariableId = Guid.NewGuid();
            var quotaFrameLevelId1 = Guid.NewGuid();
            var quotaFrameLevelId2 = Guid.NewGuid();


            var quotaFrame1 = BuildQuotaFrame(varId1, level11Id, level12Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);
            var quotaFrame2 = BuildQuotaFrame(varId2, level21Id, level22Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);

            var resultVariableDefinitions = quotaFrame1.VariableDefinitions.First() != quotaFrame2.VariableDefinitions.First();
            var resultLevels1 = quotaFrame1.VariableDefinitions.First().Levels.First() != quotaFrame2.VariableDefinitions.First().Levels.First();
            var resultLevels2 = quotaFrame1.VariableDefinitions.First().Levels.First() != quotaFrame2.VariableDefinitions.First().Levels.First();

            Assert.That(resultVariableDefinitions, Is.True);
            Assert.That(resultLevels1, Is.True);
            Assert.That(resultLevels2, Is.True);
        }

        [Test]
        public void Compare_Collection_Definitions_Are_Equal()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var quotaFrameVariableId = Guid.NewGuid();
            var quotaFrameLevelId1 = Guid.NewGuid();
            var quotaFrameLevelId2 = Guid.NewGuid();


            var quotaFrame1 = BuildQuotaFrame(varId, level1Id, level2Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);
            var quotaFrame2 = BuildQuotaFrame(varId, level1Id, level2Id, quotaFrameVariableId, quotaFrameLevelId1, quotaFrameLevelId2);

            var resultCollectionVariableDefinitions = quotaFrame1.VariableDefinitions == quotaFrame2.VariableDefinitions;


            Assert.That(resultCollectionVariableDefinitions, Is.True);

        }
        [Test]
        public void Compare_Collection_Definitions_Are_Not_Equal()
        {
            var varId1 = Guid.NewGuid();
            var varId2 = Guid.NewGuid();
            var level11Id = Guid.NewGuid();
            var level12Id = Guid.NewGuid();
            var level21Id = Guid.NewGuid();
            var level22Id = Guid.NewGuid();
            var quotaFrameVariableId = Guid.NewGuid();
            var quotaFrameLevelId1 = Guid.NewGuid();
            var quotaFrameLevelId2 = Guid.NewGuid();


            var quotaFrame1 = BuildQuotaFrame(varId1, level11Id, level12Id, quotaFrameVariableId,
                quotaFrameLevelId1, quotaFrameLevelId2, true);
            var quotaFrame2 = BuildQuotaFrame(varId2, level21Id, level22Id, quotaFrameVariableId,
                quotaFrameLevelId1, quotaFrameLevelId2);

            var resultVariableDefinitions = quotaFrame1.VariableDefinitions != quotaFrame2.VariableDefinitions;

            Assert.That(resultVariableDefinitions, Is.True);
        }
        private static QuotaFrame BuildQuotaFrame(Guid varId, Guid level1Id, Guid level2Id,
           Guid quotaFrameVariableId, Guid quotaFrameLevelId1, Guid quotaFrameLevelId2,
           bool? isSelectionOptional = null)
        {
            var quotaFrame = new QuotaFrame();
            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName",
                IsSelectionOptional = isSelectionOptional
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },
                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = quotaFrameVariableId
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = quotaFrameLevelId1
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = quotaFrameLevelId2
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);
            return quotaFrame;
        }

        [Test]
        public void Frame_CannotContainDuplicateIds()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var nonUniqueId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = nonUniqueId
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = nonUniqueId
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = Guid.NewGuid()
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainDuplicateIdInLevels()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var nonUniqueId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = Guid.NewGuid()
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = nonUniqueId
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = nonUniqueId
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainAReferenceToANonExistingVariableDefinition()
        {
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var nonExistingId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                nonExistingId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = nonExistingId,
                Id = Guid.NewGuid()
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = Guid.NewGuid()
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainAReferenceToANonExistingLevelDefinition()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var nonExistingId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                nonExistingId);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = Guid.NewGuid()
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = nonExistingId,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = Guid.NewGuid()
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveLessLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var varReferenceId = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: '{0}', missing level definition id: '{1}'",
                varReferenceId, level2Id);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = varReferenceId
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = Guid.NewGuid()
                }
                // Level 2 omitted
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveMoreLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var varId = Guid.NewGuid();
            var level1Id = Guid.NewGuid();
            var level2Id = Guid.NewGuid();
            var level3Id = Guid.NewGuid();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                level3Id);

            var quotaFrame = new QuotaFrame();

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            variable.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = level1Id,
                    Name = "level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = level2Id,
                    Name = "level2Name"
                }
            });

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = varId,
                Id = Guid.NewGuid()
            };

            frameVariable.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = level1Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level2Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = level3Id,
                    Id = Guid.NewGuid()
                }
            });

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveLevelTargetWithNegativeValue()
        {
            const int invalidTarget = -10;
            const string lvl2Name = "level2Name";

            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("varName", "odinVarName", new[] { "level1Name", lvl2Name })
                .Structure(sb =>
                {
                    sb.Variable("varName");
                })
                .Build();

            quotaFrame["varName", lvl2Name].Target = invalidTarget;
            var lvl2Id = quotaFrame["varName", lvl2Name].Id;

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Target invalid. All Targets must be of a positive value. Frame level Id '{0}' with name '{1}' has an invalid negative target '{2}'",
                lvl2Id, lvl2Name, invalidTarget);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);


            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveLevelMaxTargetWithNegativeValue()
        {
            const int invalidTarget = -10;
            const string lvl2Name = "level2Name";

            var quotaFrame = new QuotaFrameBuilder()
                .VariableDefinition("varName", "odinVarName", new[] { "level1Name", lvl2Name })
                .Structure(sb =>
                {
                    sb.Variable("varName");
                })
                .Build();

            quotaFrame["varName", lvl2Name].MaxTarget = invalidTarget;
            var lvl2Id = quotaFrame["varName", lvl2Name].Id;

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Target invalid. All Targets must be of a positive value. Frame level Id '{0}' with name '{1}' has an invalid negative maximum target '{2}'",
                lvl2Id, lvl2Name, invalidTarget);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);


            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveTotalTargetWithNegativeValue()
        {
            const int invalidTotalTarget = -100;

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Target invalid. All Targets must be of a positive value. Quota frame total target has a negative value '{0}'",
                invalidTotalTarget);

            var quotaFrame = new QuotaFrameBuilder()
                 .VariableDefinition(
                     "varName", new[] { "level1Name", "level2Name" })
                 .Structure(sb => { })
                 .Build();
            quotaFrame.Target = invalidTotalTarget;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_VariablesShouldHaveAtLeastOneVisibleLevel()
        {
            var quotaFrame = new QuotaFrameBuilder()
                 .VariableDefinition("varName", new[] { "level1Name", "level2Name" })
                 .Structure(sb => sb.Variable("varName"))
                 .Build();

            // Hide both levels
            quotaFrame["varName", "level1Name"].IsHidden = true;
            quotaFrame["varName", "level2Name"].IsHidden = true;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame invalid. Frame has variables with no visible levels. Affected variable name: 'varName'. If you don't care about any levels under variable 'varName', consider hiding that variable instead."));
        }

        [Test]
        public void ComplexFrame_HappyPath()
        {
            var var1Id = Guid.NewGuid();
            var var2Id = Guid.NewGuid();
            var var1Level1Id = Guid.NewGuid();
            var var1Level2Id = Guid.NewGuid();
            var var2Level1Id = Guid.NewGuid();
            var var2Level2Id = Guid.NewGuid();

            var quotaFrame = new QuotaFrame();

            var variable1 = new QuotaVariableDefinition
            {
                Id = var1Id,
                Name = "var1Name",
                OdinVariableName = "odinVar1Name"
            };

            variable1.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = var1Level1Id,
                    Name = "var1Level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = var1Level2Id,
                    Name = "var1Level2Name"
                }
            });

            var variable2 = new QuotaVariableDefinition
            {
                Id = var2Id,
                Name = "var2Name",
                OdinVariableName = "odinVar2Name"
            };

            variable2.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = var2Level1Id,
                    Name = "var2Level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = var2Level2Id,
                    Name = "var2Level2Name"
                }
            });

            quotaFrame.VariableDefinitions.AddRange(new[] { variable1, variable2 });

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = var1Id,
                Id = Guid.NewGuid()
            };

            var frameVariable2 = new QuotaFrameVariable
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid()
            };

            frameVariable2.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level1Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level2Id,
                    Id = Guid.NewGuid()
                }
            });

            var frameLevel1Var1 = new QuotaFrameLevel
            {
                DefinitionId = var1Level1Id,
                Id = Guid.NewGuid()
            };
            frameLevel1Var1.Variables.Add(frameVariable2);

            frameVariable.Levels.Add(frameLevel1Var1);

            var frameLevel2Var1 = new QuotaFrameLevel
            {
                DefinitionId = var1Level2Id,
                Id = Guid.NewGuid()
            };

            var frameVariable3 = new QuotaFrameVariable
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid()
            };

            frameVariable3.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level1Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level2Id,
                    Id = Guid.NewGuid()
                }
            });

            frameLevel2Var1.Variables.Add(frameVariable3);

            frameVariable.Levels.Add(frameLevel2Var1);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ComplexFrame_MissingVariableUnderOneOfTheLevels()
        {
            var var1Id = Guid.NewGuid();
            var var2Id = Guid.NewGuid();
            var var1Level1Id = Guid.NewGuid();
            var var1Level2Id = Guid.NewGuid();
            var var2Level1Id = Guid.NewGuid();
            var var2Level2Id = Guid.NewGuid();
            var frameVar1Id = Guid.NewGuid();
            var frameLvl4Id = Guid.NewGuid();

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame invalid. All levels of a variable should have the same variables underneath. Frame variable id '{0}' has a mismatch for level '{1}'",
                frameVar1Id, frameLvl4Id);

            var quotaFrame = new QuotaFrame();

            var variable1 = new QuotaVariableDefinition
            {
                Id = var1Id,
                Name = "var1Name",
                OdinVariableName = "odinVar1Name"
            };

            variable1.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = var1Level1Id,
                    Name = "var1Level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = var1Level2Id,
                    Name = "var1Level2Name"
                }
            });

            var variable2 = new QuotaVariableDefinition
            {
                Id = var2Id,
                Name = "var2Name",
                OdinVariableName = "odinVar2Name"
            };

            variable2.Levels.AddRange(new[]
            {
                new QuotaLevelDefinition
                {
                    Id = var2Level1Id,
                    Name = "var2Level1Name"
                },

                new QuotaLevelDefinition
                {
                    Id = var2Level2Id,
                    Name = "var2Level2Name"
                }
            });

            quotaFrame.VariableDefinitions.AddRange(new[] { variable1, variable2 });

            var frameVariable = new QuotaFrameVariable
            {
                DefinitionId = var1Id,
                Id = frameVar1Id
            };

            var frameVariable2 = new QuotaFrameVariable
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid()
            };

            frameVariable2.Levels.AddRange(new[]
            {
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level1Id,
                    Id = Guid.NewGuid()
                },
                new QuotaFrameLevel
                {
                    DefinitionId = var2Level2Id,
                    Id = Guid.NewGuid()
                }
            });


            var frameLevel1Var1 = new QuotaFrameLevel
            {
                DefinitionId = var1Level1Id,
                Id = Guid.NewGuid()
            };
            frameLevel1Var1.Variables.Add(frameVariable2);

            frameVariable.Levels.Add(frameLevel1Var1);

            var frameLevel2Var1 = new QuotaFrameLevel
            {
                DefinitionId = var1Level2Id,
                Id = frameLvl4Id
            };
            frameVariable.Levels.Add(frameLevel2Var1);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void ComplexFrame_VariablesShouldHaveAtLeastOneVisibleLevel()
        {
            var quotaFrame = new QuotaFrameBuilder()
                 .VariableDefinition("varName1", new[] { "level1Name", "level2Name" })
                 .VariableDefinition("varName2", new[] { "level1Name", "level2Name" })
                 .Structure(sb => sb.Variable("varName1",
                    s => s.Variable("varName2")))
                 .Build();

            quotaFrame["varName1", "level1Name"]["varName2", "level1Name"].IsHidden = true;
            quotaFrame["varName1", "level1Name"]["varName2", "level2Name"].IsHidden = true;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame invalid. Frame has variables with no visible levels. Affected variable name: 'varName2'. If you don't care about any levels under variable 'varName2', consider hiding that variable instead."));
        }

        [Test]
        public void Definitions_OdinVariableShouldBePresent()
        {
            var var1Id = Guid.NewGuid();
            var levelId = Guid.NewGuid();

            var levelDefinitions = new[]
            {
                new QuotaLevelDefinition
                {
                    Id = levelId,
                    Name = "Level 1"
                }
            };

            var definitions = new[]
            {
                new QuotaVariableDefinition(levelDefinitions)
                {
                    Id = var1Id,
                    Name = "var 1"
                    // OdinVariableName = 
                }
            };

            var level = new QuotaFrameLevel { DefinitionId = levelId };

            var frame = new[]
            {
                new QuotaFrameVariable(new List<QuotaFrameLevel> { level })
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = var1Id
                }
            };

            var quotaFrame = new QuotaFrame(definitions, frame);
            var validator = new QuotaFrameValidator();

            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Odin variable name invalid. Odin variable names can only contain numbers, " +
                           "letters and '_' and cannot be empty. They can only ​start with​ a letter. " +
                           "First character cannot be '_' or a number. " +
                           $"Variable definition Id '{var1Id}' with name 'var 1' " +
                           "has an invalid Odin Variable Name ''"));
        }

        [Test]
        public void Definitions_OdinVariableCannotBeEmpty()
        {
            var var1Id = Guid.NewGuid();
            var levelId = Guid.NewGuid();

            var levelDefinitions = new[]
            {
                new QuotaLevelDefinition
                {
                    Id = levelId,
                    Name = "Level 1"
                }
            };

            var definitions = new[]
            {
                new QuotaVariableDefinition(levelDefinitions)
                {
                    Id = var1Id,
                    Name = "var 1",
                    OdinVariableName = ""
                }
            };

            var level = new QuotaFrameLevel { DefinitionId = levelId };

            var frame = new[]
            {
                new QuotaFrameVariable(new List<QuotaFrameLevel> { level })
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = var1Id
                }
            };

            var quotaFrame = new QuotaFrame(definitions, frame);
            var validator = new QuotaFrameValidator();

            var result = validator.Validate(quotaFrame);

            Assert.That(
                result.Errors.Single().ErrorMessage,
                Is.EqualTo("Odin variable name invalid. Odin variable names can only contain numbers, " +
                           "letters and '_' and cannot be empty. They can only ​start with​ a letter. " +
                           "First character cannot be '_' or a number. " +
                           $"Variable definition Id '{var1Id}' with name 'var 1' " +
                           "has an invalid Odin Variable Name ''"));
        }

        [TestCase("")]
        [TestCase("_")]
        [TestCase("_t")]
        [TestCase("_T")]
        [TestCase("2")]
        [TestCase("2t")]
        [TestCase("2T")]
        [TestCase("2_")]
        [TestCase("_2")]
        [TestCase("t!")]
        [TestCase("t@")]
        [TestCase("t#")]
        [TestCase("t$")]
        [TestCase("t%")]
        [TestCase("t^")]
        [TestCase("t&")]
        [TestCase("t*")]
        [TestCase("t(")]
        [TestCase("t)")]
        [TestCase("t+")]
        [TestCase("t{")]
        [TestCase("t}")]
        [TestCase("t[")]
        [TestCase("t]")]
        [TestCase("t|")]
        [TestCase(@"t\")]
        [TestCase("t'")]
        [TestCase("t?")]
        [TestCase("t/")]
        [TestCase("t<")]
        [TestCase("t>")]
        [TestCase("t,")]
        [TestCase("t.")]
        [TestCase("t:")]
        [TestCase("t;")]
        public void OdinVariable_NotValid(string odinVariable)
        {
            var result = CreateQuotaFrameResult(odinVariable);
            Assert.That(result, Is.False);
        }

        [TestCase("天")]
        [TestCase("T_")]
        [TestCase("t_")]
        [TestCase("t2")]
        [TestCase("T2")]
        [TestCase("t2_adCF")]
        [TestCase("T2_C_dE")]
        public void OdinVariable_Valid(string odinVariable)
        {
            var result = CreateQuotaFrameResult(odinVariable);
            Assert.That(result, Is.True);
        }

        private static bool CreateQuotaFrameResult(string odinVariable)
        {
            var var1Id = Guid.NewGuid();
            var levelId = Guid.NewGuid();

            var levelDefinitions = new[]
            {
                new QuotaLevelDefinition
                {
                    Id = levelId,
                    Name = "Level 1"
                }
            };

            var definitions = new[]
            {
                new QuotaVariableDefinition(levelDefinitions)
                {
                    Id = var1Id,
                    Name = "var 1",
                    OdinVariableName = odinVariable
                }
            };

            var level = new QuotaFrameLevel { DefinitionId = levelId };

            var frame = new[]
            {
                new QuotaFrameVariable(new List<QuotaFrameLevel> {level})
                {
                    Id = Guid.NewGuid(),
                    DefinitionId = var1Id
                }
            };

            var quotaFrame = new QuotaFrame(definitions, frame);
            var validator = new QuotaFrameValidator();

            return validator.Validate(quotaFrame).IsValid;
        }
    }
}