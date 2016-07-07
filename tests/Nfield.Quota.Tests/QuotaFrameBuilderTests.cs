using System;
using System.Diagnostics;
using Nfield.Quota.Builders;
using NUnit.Framework;
using System.Linq;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaFrameBuilderTests
    {
        [Test]
        public void BuildingSimpleTreeCreatesCorrectQuotaFrame()
        {
            Guid result;
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .Target(10)
                .VariableDefinition(
                    variableId: "varId", variableName: "varName",
                    odinVariableName: "odinVarName", levelNames: new[] { "level1Name", "level2Name" })
                .Structure(sb =>
                {
                    sb.Variable("varName");
                })
                .Build();

            quotaFrame["varName", "level1Name"].Target = 6;
            quotaFrame["varName", "level2Name"].Target = 4;

            Assert.That(quotaFrame.Id, Is.EqualTo("id"));
            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Debug.Assert(quotaFrame.Target != null, "quotaFrame.Target != null");
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.VariableDefinitions.First().Id, Is.EqualTo("varId"));
            Assert.That(quotaFrame.VariableDefinitions.First().Name, Is.EqualTo("varName"));
            Assert.That(quotaFrame.VariableDefinitions.First().OdinVariableName, Is.EqualTo("odinVarName"));
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.First().Levels.First().Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.First().Name, Is.EqualTo("level1Name"));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.First().Levels.ElementAt(1).Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.ElementAt(1).Name, Is.EqualTo("level2Name"));

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Id, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().DefinitionId, Is.EqualTo("varId"));
            Assert.That(quotaFrame.FrameVariables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.First().Id, out result), Is.True);
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.First().DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Target, Is.EqualTo(6));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Id, out result), Is.True);
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.ElementAt(1).DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Target, Is.EqualTo(4));
        }

        [Test]
        public void BuildingNestedTreeCreatesCorrectQuotaFrame()
        {
            Guid result;
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .Target(10)
                .VariableDefinition(variableId: "gender",levelNames: new[]
                {
                   "Male", "Female"
                })
                .VariableDefinition(variableId: "region", levelNames: new []
                {
                     "North", "South"
                })
                .Structure(gender =>
                gender.Variable("gender",region => region.Variable("region")))
                .Build();

            quotaFrame["gender", "Male"].Target = 6;
            quotaFrame["gender", "Male"]["region", "North"].Target = 3;
            quotaFrame["gender", "Male"]["region", "South"].Target = 3;

            quotaFrame["gender", "Female"].Target = 4;
            quotaFrame["gender", "Female"]["region", "North"].Target = 2;
            quotaFrame["gender", "Female"]["region", "South"].Target = 2;

            Assert.That(quotaFrame.Id, Is.EqualTo("id"));
            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Debug.Assert(quotaFrame.Target != null, "quotaFrame.Target != null");
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(2));

            Assert.That(quotaFrame.VariableDefinitions.First().Id, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions.First().Name, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions.First().OdinVariableName, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.First().Levels.First().Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.First().Name, Is.EqualTo("Male"));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.First().Levels.ElementAt(1).Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.First().Levels.ElementAt(1).Name, Is.EqualTo("Female"));

            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).Id, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).Name, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).OdinVariableName, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.ElementAt(1).Levels.First().Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).Levels.First().Name, Is.EqualTo("North"));
            Assert.That(Guid.TryParse(quotaFrame.VariableDefinitions.ElementAt(1).Levels.ElementAt(1).Id, out result), Is.True);
            Assert.That(quotaFrame.VariableDefinitions.ElementAt(1).Levels.ElementAt(1).Name, Is.EqualTo("South"));

            //// Bored yet?

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().DefinitionId, Is.EqualTo("gender"));
            Assert.That(quotaFrame.FrameVariables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.First().DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Target, Is.EqualTo(6));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().DefinitionId, Is.EqualTo("region"));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.First().DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.First().Target, Is.EqualTo(3));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.ElementAt(1).DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.ElementAt(1).Target, Is.EqualTo(3));


            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.ElementAt(1).DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Target, Is.EqualTo(4));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().DefinitionId, Is.EqualTo("region"));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.First().DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.First().Target, Is.EqualTo(2));
            Assert.That(Guid.TryParse(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.ElementAt(1).DefinitionId, out result), Is.True);
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.ElementAt(1).Target, Is.EqualTo(2));
        }
    }
}