using System.Linq;
using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        [Test]
        public void Definitions_UniqueIdsAndNames_HappyPath()
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
        public void Definitions_CannotBeEmpty()
        {
            var quotaFrame = new QuotaFrame();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions cannot be empty."));
        }

        [Test]
        public void Definitions_EveryVariableNeedsAtLeastTwoLevels()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions has variables with less than two or no levels. Affected variable definition id: 'varId'"));
        }

        [Test]
        public void Definitions_UniqueIdsAndNames_NonUniqueIdInLevel()
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
        public void Definitions_UniqueIdsAndNames_NonUniqueNameInLevel()
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
        public void Definitions_UniqueIdsAndNames_NonUniqueIdAccrossLevelAndVariable()
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
        public void Definitions_UniqueIdsAndNames_NonUniqueNameAccrossLevelAndVariable()
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
        public void Frame_UniqueIds_NonUniqueAcrossVarAndLevels()
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
        public void Frame_UniqueIds_NonUniqueWithinLevels()
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
        public void Frame_VariableWithInvalidReferenceToDefinition()
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
        public void Frame_LevelWithInvalidReferenceToDefinition()
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
        public void Frame_WrongLevelsUnderVariable_OneMissing()
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
        public void Frame_WrongLevelsUnderVariable_OneExtra()
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

        /*[Test]
        public void NewBuilderSyntax()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .SetupDefinitions(defBuilder =>
                {
                    defBuilder.VariableDefinition("var1Id", "varName", "odinVarName", lvlDefbuilder =>
                    {
                        lvlDefbuilder.LevelDefinition("var1lvl1Id", "Level 1");
                        lvlDefbuilder.LevelDefinition("var1lvl2Id", "Level 2");
                    });
                    defBuilder.VariableDefinition("var2Id", "varName", "odinVarName", lvlDefbuilder =>
                    {
                        lvlDefbuilder.LevelDefinition("var2lvl1Id", "Level 1");
                        lvlDefbuilder.LevelDefinition("var2lvl2Id", "Level 2");
                        lvlDefbuilder.LevelDefinition("var2lvl3Id", "Level 3");
                    });
                    defBuilder.VariableDefinition("var3Id", "varName", "odinVarName", lvlDefbuilder =>
                    {
                        lvlDefbuilder.LevelDefinition("var3lvl1Id", "Level 1");
                        lvlDefbuilder.LevelDefinition("var3lvl2Id", "Level 2");
                    });
                })
                .SetupStructure(strucBuilder =>
                {
                    strucBuilder.AddVariable("var1Id", sb => sb.AddVariable("var2Id"));
                    strucBuilder.AddVariable("var3Id");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }*/
    }
}
