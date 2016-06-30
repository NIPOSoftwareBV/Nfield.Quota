using System;
using System.Linq;
using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        /*[Test]
        public void Definitions_HappyPath()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Definitions_EveryVariableNeedsAtLeastOneLevel()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    // no levels
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions has variables with no levels. Affected variable definition id: 'varId'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateIds()
        {
            const string nonUniqueId = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level(nonUniqueId, "level1Name");
                    var.Level(nonUniqueId, "level2Name");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateNames()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", nonUniqueName);
                    var.Level("level2Id", nonUniqueName);
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
            const string nonUniqueId = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition(nonUniqueId, "varName", "odinVarName", var =>
                {
                    var.Level(nonUniqueId, "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateNamesBetweenVariablesAndLevels()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", nonUniqueName, "odinVarName", var =>
                {
                    var.Level("level1Id", nonUniqueName);
                    var.Level("level2Id", "level1Name");
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
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", "level1RefId", 6, 2);
                    variableReference.Level("level2Id", "level2RefId", 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_CannotContainDuplicateIds()
        {
            const string nonUniqueId = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", nonUniqueId, variableReference =>
                {
                    variableReference.Level("level1Id", nonUniqueId, 6, 2);
                    variableReference.Level("level2Id", "level2RefId", 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void Frame_CannotContainDuplicateIdInLevels()
        {
            const string nonUniqueId = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", nonUniqueId, 6, 2);
                    variableReference.Level("level2Id", nonUniqueId, 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void Frame_CannotContainAReferenceToANonExistingVariableDefinition()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("NONEXISTINGDEFINITIONID", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", "level1RefId", 6, 2);
                    variableReference.Level("level2Id", "level2RefId", 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a reference to a non-existing definition. Definition id: 'NONEXISTINGDEFINITIONID'"));
        }


        [Test]
        public void Frame_CannotContainAReferenceToANonExistingLevelDefinition()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("NONEXISTINGDEFINITIONID", "level1RefId", 6, 2);
                    variableReference.Level("level2Id", "level2RefId", 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a reference to a non-existing definition. Definition id: 'NONEXISTINGDEFINITIONID'"));
        }

        [Test]
        public void Frame_CannotHaveLessLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", "level1RefId", 6, 2);
                    // Level 2 omitted
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: 'varReferenceId', missing level definition id: 'level2Id'"));
        }

        [Test]
        public void Frame_CannotHaveMoreLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", "level1RefId", 6, 2);
                    variableReference.Level("level2Id", "level2RefId", 4, 3);
                    variableReference.Level("level3Id", "level3RefId", 4, 3); // too many
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a reference to a non-existing definition. Definition id: 'level3Id'"));
        }

        [Test]
        public void ComplexFrame_HappyPath()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("var1Id", "var2Name", "odinVar1Name", var =>
                {
                    var.Level("var1level1Id", "var1Level1Name");
                    var.Level("var1level2Id", "var1Level2Name");
                })
                .VariableDefinition("var2Id", "var1Name", "odinVar2Name", var =>
                {
                    var.Level("var2level1Id", "var2Level1Name");
                    var.Level("var2level2Id", "var2Level2Name");
                })

                .FrameVariable("var1Id", "frameVar1Id", rootVarBuilder =>
                {
                    rootVarBuilder.Level("var1level1Id", "frameLvl1Id", 6, 2, varBuilder =>
                    {
                        varBuilder.Variable("var2Id", "frameVar2Id", lvlBuilder =>
                        {
                            lvlBuilder.Level("var2level1Id", "frameLvl2Id");
                            lvlBuilder.Level("var2level2Id", "frameLvl3Id");
                        });
                    });
                    rootVarBuilder.Level("var1level2Id", "frameLvl4Id", 4, 3, varBuilder =>
                    {
                        varBuilder.Variable("var2Id", "frameVar3Id", lvlBuilder =>
                        {
                            lvlBuilder.Level("var2level1Id", "frameLvl5Id");
                            lvlBuilder.Level("var2level2Id", "frameLvl6Id");
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
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("var1Id", "var2Name", "odinVar1Name", var =>
                {
                    var.Level("var1level1Id", "var1Level1Name");
                    var.Level("var1level2Id", "var1Level2Name");
                })
                .VariableDefinition("var2Id", "var1Name", "odinVar2Name", var =>
                {
                    var.Level("var2level1Id", "var2Level1Name");
                    var.Level("var2level2Id", "var2Level2Name");
                })
                .FrameVariable("var1Id", "frameVar1Id", rootVarBuilder =>
                {
                    rootVarBuilder.Level("var1level1Id", "frameLvl1Id", 6, 2, varBuilder =>
                    {
                        varBuilder.Variable("var2Id", "frameVar2Id", lvlBuilder =>
                        {
                            lvlBuilder.Level("var2level1Id", "frameLvl2Id");
                            lvlBuilder.Level("var2level2Id", "frameLvl3Id");
                        });
                    });
                    rootVarBuilder.Level("var1level2Id", "frameLvl4Id", 4, 3, varBuilder =>
                    {
                        // Missing variable
                    });
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame invalid. All levels of a variable should have the same variables underneath. Frame variable id 'frameVar1Id' has a mismatch for level 'frameLvl4Id'"));
        }*/

        [Test]
        public void NewBuilderSyntax()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition(
                    variableId: "var1Id", variableName: "var1Name",
                    odinVariableName: "odinVarName", levels: new[] {"Level 1", "Level 2"})
                .VariableDefinition("var2Id", "var2Name", "odinVarName", new[] { "Level 3", "Level 4" })
                .VariableDefinition("var3Id", "var3Name", "odinVarName", new[] { "Level 5", "Level 6", "Level 7" })
                .Structure(sb =>
                {
                    sb.Variable("var1Id", sb2 => sb2.Variable("var1Id"));
                    sb.Variable("var2Id");
                })
                .Build();

            //todo targets

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }
    }
}
