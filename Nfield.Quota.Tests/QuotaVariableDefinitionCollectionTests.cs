using NUnit.Framework;
using System;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaVariableDefinitionCollectionTests
    {
        [Test]
        public void Test_OperatorEquals_ReferenceEquals()
        {
            var variableCollection = new QuotaVariableDefinitionCollection();
            var variableCollectionReference = variableCollection;

            Assert.True(variableCollection == variableCollectionReference);
        }

        [Test]
        public void Test_OperatorEquals_ComparisonWithNull()
        {
            var variableCollection = new QuotaVariableDefinitionCollection();
            Assert.IsFalse(variableCollection == null);
            Assert.IsFalse(null == variableCollection);
        }

        [Test]
        public void Test_OperatorNotEquals_ReferenceEquals()
        {
            var variableCollection = new QuotaVariableDefinitionCollection();
            var variableCollectionReference = variableCollection;
            Assert.IsFalse(variableCollection != variableCollectionReference);
        }

        [Test]
        public void Test_OperatorNotEquals_ComparisonWithNull()
        {
            QuotaVariableDefinitionCollection variableCollection = null;
            Assert.IsFalse(variableCollection != null);
            Assert.IsFalse(null != variableCollection);
        }

        [Test]
        public void Test_OperatorEquals_SameOrderValuesEqual_ReturnsTrue()
        {
            var variable = new QuotaVariableDefinition();

            var variableCollection1 = new QuotaVariableDefinitionCollection();
            variableCollection1.Add(variable);

            var variableCollection2 = new QuotaVariableDefinitionCollection();
            variableCollection2.Add(variable);

            Assert.IsTrue(variableCollection1 == variableCollection2);
        }

        [Test]
        public void Test_OperatorEquals_DifferentOrderValuesEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar"
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "differentVar",
                OdinVariableName = "odinVar"
            };

            var variableCollection1 = new QuotaVariableDefinitionCollection();
            variableCollection1.Add(variable1);
            variableCollection1.Add(variable2);

            var variableCollection2 = new QuotaVariableDefinitionCollection();
            variableCollection2.Add(variable2);
            variableCollection2.Add(variable1);

            Assert.IsTrue(variableCollection1 == variableCollection2);
        }

        [Test]
        public void Test_OperatorEquals_ValuesNotEqual_ReturnsFalse()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar"
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "differentVar",
                OdinVariableName = "odinVar"
            };

            var variableCollection1 = new QuotaVariableDefinitionCollection();
            variableCollection1.Add(variable1);
            variableCollection1.Add(variable2);

            var variableCollection2 = new QuotaVariableDefinitionCollection();
            variableCollection2.Add(variable2);

            Assert.IsFalse(variableCollection1 == variableCollection2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesEqual_ReturnsFalse()
        {
            var variable = new QuotaVariableDefinition();

            var variableCollection1 = new QuotaVariableDefinitionCollection();
            variableCollection1.Add(variable);

            var variableCollection2 = new QuotaVariableDefinitionCollection();
            variableCollection2.Add(variable);

            Assert.IsFalse(variableCollection1 != variableCollection2);
        }

        [Test]
        public void Test_OperatorNotEquals_ValuesNotEqual_ReturnsTrue()
        {
            var id = Guid.NewGuid();

            var variable1 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "var",
                OdinVariableName = "odinVar"
            };

            var variable2 = new QuotaVariableDefinition
            {
                Id = id,
                Name = "differentVar",
                OdinVariableName = "odinVar"
            };

            var variableCollection1 = new QuotaVariableDefinitionCollection();
            variableCollection1.Add(variable1);
            variableCollection1.Add(variable2);

            var variableCollection2 = new QuotaVariableDefinitionCollection();
            variableCollection2.Add(variable2);

            Assert.IsTrue(variableCollection1 != variableCollection2);
        }
    }
}
