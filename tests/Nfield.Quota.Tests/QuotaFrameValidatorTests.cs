using System.Linq;
using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        [Test]
        public void UniqueIdsAndNames_HappyPath()
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
        public void UniqueIdsAndNames_NonUniqueIdInLevel()
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
        public void UniqueIdsAndNames_NonUniqueNameInLevel()
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
        public void UniqueIdsAndNames_NonUniqueIdAccrossLevelAndVariable()
        {
            const string nonUniqueId = "non-unique";

            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .VariableDefinition(nonUniqueId, "varName", "odinVarName", var =>
                {
                    var.Level(nonUniqueId, "level1Name");
                    var.Level("level2Id", "level1Name");
                })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate id. Duplicate id: 'non-unique'"));
        }

        [Test]
        public void UniqueIdsAndNames_NonUniqueNameAccrossLevelAndVariable()
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
    }
}
