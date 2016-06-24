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
        public void Frame_UniqueIds_HappyPath()
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
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", nonUniqueName, variableReference =>
                {
                    variableReference.Level("level1Id", nonUniqueName, 6, 2);
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
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition("varId", "varName", "odinVarName", var =>
                {
                    var.Level("level1Id", "level1Name");
                    var.Level("level2Id", "level2Name");
                })
                .FrameVariable("varId", "varReferenceId", variableReference =>
                {
                    variableReference.Level("level1Id", nonUniqueName, 6, 2);
                    variableReference.Level("level2Id", nonUniqueName, 4, 3);
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame contains a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void Frame_UniqueIds_VariableWithInvalidReferenceToDefinition()
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
        public void Frame_UniqueIds_LevelWithInvalidReferenceToDefinition()
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
    }
}
