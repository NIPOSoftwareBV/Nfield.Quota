using System;
using System.Globalization;
using System.Linq;
using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        /*
                [Test]
                public void Definitions_HappyPath()
                {
                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(Guid.NewGuid().ToString(), "varName", "odinVarName", var =>
                        {
                            var.Level(Guid.NewGuid().ToString(), "level1Name");
                            var.Level(Guid.NewGuid().ToString(), "level2Name");
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.IsValid, Is.True);
                }

                [Test]
                public void Definitions_EveryVariableNeedsAtLeastOneLevel()
                {
                    var variableId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame definitions has variables with no levels. Affected variable definition id: '{0}'",
                        variableId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(variableId, "varName", "odinVarName", var =>
                        {
                            // no levels
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Definitions_CannotContainDuplicateIds()
                {
                    var nonUniqueId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                        nonUniqueId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(Guid.NewGuid().ToString(), "varName", "odinVarName", var =>
                        {
                            var.Level(nonUniqueId, "level1Name");
                            var.Level(nonUniqueId, "level2Name");
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Definitions_CannotContainDuplicateNames()
                {
                    const string nonUniqueName = "non-unique";

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(Guid.NewGuid().ToString(), "varName", "odinVarName", var =>
                        {
                            var.Level(Guid.NewGuid().ToString(), nonUniqueName);
                            var.Level(Guid.NewGuid().ToString(), nonUniqueName);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo("Quota frame definitions contain a duplicate name. Duplicate name: 'non-unique'"));
                }

                [Test]
                public void Definitions_CannotContainDuplicateIdsBetweenVariablesAndLevels()
                {
                    var nonUniqueId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                        nonUniqueId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(nonUniqueId, "varName", "odinVarName", var =>
                        {
                            var.Level(nonUniqueId, "level1Name");
                            var.Level(Guid.NewGuid().ToString(), "level2Name");
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Definitions_CannotContainDuplicateNamesBetweenVariablesAndLevels()
                {
                    const string nonUniqueName = "non-unique";

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(Guid.NewGuid().ToString(), nonUniqueName, "odinVarName", var =>
                        {
                            var.Level(Guid.NewGuid().ToString(), nonUniqueName);
                            var.Level(Guid.NewGuid().ToString(), "level1Name");
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo("Quota frame definitions contain a duplicate name. Duplicate name: 'non-unique'"));
                }

                [Test]
                public void Frame_HappyPath()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, Guid.NewGuid().ToString(), variableReference =>
                        {
                            variableReference.Level(level1Id, Guid.NewGuid().ToString(), 6, 2);
                            variableReference.Level(level2Id, Guid.NewGuid().ToString(), 4, 3);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.IsValid, Is.True);
                }

                [Test]
                public void Frame_CannotContainDuplicateIds()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var nonUniqueId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                        nonUniqueId);


                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, nonUniqueId, variableReference =>
                        {
                            variableReference.Level(level1Id, nonUniqueId, 6, 2);
                            variableReference.Level(level2Id, Guid.NewGuid().ToString(), 4, 3);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Frame_CannotContainDuplicateIdInLevels()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var nonUniqueId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                        nonUniqueId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, Guid.NewGuid().ToString(), variableReference =>
                        {
                            variableReference.Level(level1Id, nonUniqueId, 6, 2);
                            variableReference.Level(level2Id, nonUniqueId, 4, 3);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Frame_CannotContainAReferenceToANonExistingVariableDefinition()
                {
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var nonExistingId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                        nonExistingId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(Guid.NewGuid().ToString(), "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(nonExistingId, Guid.NewGuid().ToString(), variableReference =>
                        {
                            variableReference.Level(level1Id, Guid.NewGuid().ToString(), 6, 2);
                            variableReference.Level(level2Id, Guid.NewGuid().ToString(), 4, 3);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }


                [Test]
                public void Frame_CannotContainAReferenceToANonExistingLevelDefinition()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var nonExistingId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                        nonExistingId);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, Guid.NewGuid().ToString(), variableReference =>
                        {
                            variableReference.Level(nonExistingId, Guid.NewGuid().ToString(), 6, 2);
                            variableReference.Level(level2Id, Guid.NewGuid().ToString(), 4, 3);
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Frame_CannotHaveLessLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var varReferenceId = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: '{0}', missing level definition id: '{1}'",
                        varReferenceId, level2Id);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, varReferenceId, variableReference =>
                        {
                            variableReference.Level(level1Id, Guid.NewGuid().ToString(), 6, 2);
                            // Level 2 omitted
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void Frame_CannotHaveMoreLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
                {
                    var varId = Guid.NewGuid().ToString();
                    var level1Id = Guid.NewGuid().ToString();
                    var level2Id = Guid.NewGuid().ToString();
                    var level3Id = Guid.NewGuid().ToString();
                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                        level3Id);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(varId, "varName", "odinVarName", var =>
                        {
                            var.Level(level1Id, "level1Name");
                            var.Level(level2Id, "level2Name");
                        })
                        .FrameVariable(varId, Guid.NewGuid().ToString(), variableReference =>
                        {
                            variableReference.Level(level1Id, Guid.NewGuid().ToString(), 6, 2);
                            variableReference.Level(level2Id, Guid.NewGuid().ToString(), 4, 3);
                            variableReference.Level(level3Id, Guid.NewGuid().ToString(), 4, 3); // too many
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }

                [Test]
                public void ComplexFrame_HappyPath()
                {
                    var var1Id = Guid.NewGuid().ToString();
                    var var2Id = Guid.NewGuid().ToString();
                    var var1Level1Id = Guid.NewGuid().ToString();
                    var var1Level2Id = Guid.NewGuid().ToString();
                    var var2Level1Id = Guid.NewGuid().ToString();
                    var var2Level2Id = Guid.NewGuid().ToString();

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(var1Id, "var2Name", "odinVar1Name", var =>
                        {
                            var.Level(var1Level1Id, "var1Level1Name");
                            var.Level(var1Level2Id, "var1Level2Name");
                        })
                        .VariableDefinition(var2Id, "var1Name", "odinVar2Name", var =>
                        {
                            var.Level(var2Level1Id, "var2Level1Name");
                            var.Level(var2Level2Id, "var2Level2Name");
                        })

                        .FrameVariable(var1Id, Guid.NewGuid().ToString(), rootVarBuilder =>
                        {
                            rootVarBuilder.Level(var1Level1Id, Guid.NewGuid().ToString(), 6, 2, varBuilder =>
                            {
                                varBuilder.Variable(var2Id, Guid.NewGuid().ToString(), lvlBuilder =>
                                {
                                    lvlBuilder.Level(var2Level1Id, Guid.NewGuid().ToString());
                                    lvlBuilder.Level(var2Level2Id, Guid.NewGuid().ToString());
                                });
                            });
                            rootVarBuilder.Level(var1Level2Id, Guid.NewGuid().ToString(), 4, 3, varBuilder =>
                            {
                                varBuilder.Variable(var2Id, Guid.NewGuid().ToString(), lvlBuilder =>
                                {
                                    lvlBuilder.Level(var2Level1Id, Guid.NewGuid().ToString());
                                    lvlBuilder.Level(var2Level2Id, Guid.NewGuid().ToString());
                                });
                            });
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.IsValid, Is.True);
                }

                [Test]
                public void ComplexFrame_MissingVariableUnderOneOfTheLevels()
                {
                    var var1Id = Guid.NewGuid().ToString();
                    var var2Id = Guid.NewGuid().ToString();
                    var var1Level1Id = Guid.NewGuid().ToString();
                    var var1Level2Id = Guid.NewGuid().ToString();
                    var var2Level1Id = Guid.NewGuid().ToString();
                    var var2Level2Id = Guid.NewGuid().ToString();
                    var frameVar1Id = Guid.NewGuid().ToString();
                    var frameLvl4Id = Guid.NewGuid().ToString();

                    var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                        "Quota frame invalid. All levels of a variable should have the same variables underneath. Frame variable id '{0}' has a mismatch for level '{1}'",
                        frameVar1Id, frameLvl4Id);

                    var quotaFrame = new QuotaFrameBuilder()
                        .Id(Guid.NewGuid().ToString())
                        .VariableDefinition(var1Id, "var2Name", "odinVar1Name", var =>
                        {
                            var.Level(var1Level1Id, "var1Level1Name");
                            var.Level(var1Level2Id, "var1Level2Name");
                        })
                        .VariableDefinition(var2Id, "var1Name", "odinVar2Name", var =>
                        {
                            var.Level(var2Level1Id, "var2Level1Name");
                            var.Level(var2Level2Id, "var2Level2Name");
                        })

                        .FrameVariable(var1Id, frameVar1Id, rootVarBuilder =>
                        {
                            rootVarBuilder.Level(var1Level1Id, Guid.NewGuid().ToString(), 6, 2, varBuilder =>
                            {
                                varBuilder.Variable(var2Id, Guid.NewGuid().ToString(), lvlBuilder =>
                                {
                                    lvlBuilder.Level(var2Level1Id, Guid.NewGuid().ToString());
                                    lvlBuilder.Level(var2Level2Id, Guid.NewGuid().ToString());
                                });
                            });
                            rootVarBuilder.Level(var1Level2Id, frameLvl4Id, 4, 3, varBuilder =>
                            {
                                // Missing variable
                            });
                        })
                        .Build();

                    var validator = new QuotaFrameValidator();
                    var result = validator.Validate(quotaFrame);

                    Assert.That(result.Errors.Single().ErrorMessage,
                        Is.EqualTo(expectedErrorMessage));
                }
                */

        [Test, Ignore("Will be removed")]
        public void NewBuilderSyntax()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition(
                    variableId: "var1Id", variableName: "var1Name",
                    odinVariableName: "odinVarName", levelNames: new[] {"Level 1", "Level 2"})
                .VariableDefinition("var2Id", "var2Name", "odinVarName", new[] { "Level 3", "Level 4" })
                .VariableDefinition("var3Id", "var3Name", "odinVarName", new[] { "Level 5", "Level 6", "Level 7" })
                .Structure(sb =>
                {
                    sb.Variable("var1Id", sb2 => sb2.Variable("var2Id"));
                    sb.Variable("var3Id");
                })
                .Build();

            quotaFrame["var1Name", "Level 1"]["var2Name", "Level 3"].Target = 100;
            quotaFrame["var3Name", "Level 5"].Target = 100;

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }
    }
}
