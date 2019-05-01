using NUnit.Framework;
using System;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaVariableDefinitionTests
    {
        [Test]
        public void Test_OperatorEquals_ReferenceEquals()
        {
            var variable = new QuotaVariableDefinition();
            var variableReference = variable;

            Assert.True(variable == variableReference);
        }

        [Test]
        public void Test_OperatorEquals_ComparisonWithNull()
        {
            QuotaVariableDefinition variable = new QuotaVariableDefinition();
            Assert.IsFalse(variable == null);
            Assert.IsFalse(null == variable);
        }

        [Test]
        public void Test_OperatorNotEquals_ReferenceEquals()
        {
            var variable = new QuotaVariableDefinition();
            var variableReference = variable;
            Assert.IsFalse(variable != variableReference);
        }

        [Test]
        public void Test_OperatorNotEquals_ComparisonWithNull()
        {
            QuotaVariableDefinition variable = null;
            Assert.IsFalse(variable != null);
            Assert.IsFalse(null != variable);
        }

        [Test]
        public void Test_OperatorEquals_ValuesEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            Assert.IsTrue(variable1 == variable2);
        }

        [Test]
        public void Test_OperatorEquals_ValuesNotEqual_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "differentVar",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            Assert.IsFalse(variable1 == variable2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesEqual_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            Assert.IsFalse(variable1 != variable2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesNotEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "differentVar",
                OdinVariableName = "odinVar",
                IsMulti = true,
                IsSelectionOptional = true,
            };

            Assert.IsTrue(variable1 != variable2);
        }
    }
}
