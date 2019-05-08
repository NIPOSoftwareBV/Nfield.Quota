using NUnit.Framework;
using System;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaLevelDefinitionTests
    {
        [Test]
        public void Test_OperatorEquals_ReferenceEquals()
        {
            var level = new QuotaLevelDefinition();
            var levelReference = level;

            Assert.True(level == levelReference);
        }

        [Test]
        public void Test_OperatorEquals_ComparisonWithNull()
        {
            var level = new QuotaLevelDefinition();
            Assert.IsFalse(level == null);
            Assert.IsFalse(null == level);
        }

        [Test]
        public void Test_OperatorNotEquals_ReferenceEquals()
        {
            var level = new QuotaLevelDefinition();
            var levelReference = level;
            Assert.IsFalse(level != levelReference);
        }

        [Test]
        public void Test_OperatorNotEquals_ComparisonWithNull()
        {
            QuotaLevelDefinition level = null;
            Assert.IsFalse(level != null);
            Assert.IsFalse(null != level);
        }

        [Test]
        public void Test_OperatorEquals_ValuesEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var level1 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            var level2 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            Assert.IsTrue(level1 == level2);
        }

        [Test]
        public void Test_OperatorEquals_ValuesNotEqual_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            var level1 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            var level2 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "differentLevel"
            };

            Assert.IsFalse(level1 == level2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesEqual_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            var level1 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            var level2 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            Assert.IsFalse(level1 != level2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesNotEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var level1 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "level"
            };

            var level2 = new QuotaLevelDefinition
            {
                Id = id,
                Name = "differentLevel"
            };

            Assert.IsTrue(level1 != level2);
        }
    }
}
