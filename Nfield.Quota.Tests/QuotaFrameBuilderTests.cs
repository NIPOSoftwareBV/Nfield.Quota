using System.Diagnostics;
using Nfield.Quota.Builders;
using NUnit.Framework;
using System.Linq;
using Nfield.Quota.Models;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaFrameBuilderTests
    {
        [Test]
        public void BuildingSimpleTreeCreatesCorrectQuotaFrame()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Target(10)
                .VariableDefinition("varName", "odinVarName", new[] { "level1Name", "level2Name" }, VariableSelection.Optional)
                .Structure(sb =>
                {
                    sb.Variable("varName");
                })
                .Build();

            quotaFrame["varName", "level1Name"].Target = 6;
            quotaFrame["varName", "level2Name"].Target = 4;

            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Debug.Assert(quotaFrame.Target != null, "quotaFrame.Target != null");
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(1));
            var variable = quotaFrame.VariableDefinitions.First();
            Assert.That(variable.Id, Is.Not.Null);
            Assert.That(variable.Name, Is.EqualTo("varName"));
            Assert.That(variable.OdinVariableName, Is.EqualTo("odinVarName"));
            Assert.That(variable.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(variable.Levels.Count, Is.EqualTo(2));
            Assert.That(variable.Levels.First().Name, Is.EqualTo("level1Name"));
            Assert.That(variable.Levels.ElementAt(1).Name, Is.EqualTo("level2Name"));

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().DefinitionId, Is.EqualTo(variable.Id));
            Assert.That(quotaFrame.FrameVariables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Target, Is.EqualTo(6));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Target, Is.EqualTo(4));
        }

        [Test]
        public void BuildingNestedTreeCreatesCorrectQuotaFrame()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Target(10)
                .VariableDefinition("gender", new[]
                {
                   "Male", "Female"
                }, VariableSelection.Optional)
                .VariableDefinition("region", new []
                {
                     "North", "South"
                })
                .Structure(root =>
                    root.Variable("gender",
                        gender => gender.Variable("region")))
                .Build();

            quotaFrame["gender", "Male"].Target = 6;
            quotaFrame["gender", "Male"]["region", "North"].Target = 3;
            quotaFrame["gender", "Male"]["region", "South"].Target = 3;

            quotaFrame["gender", "Female"].Target = 4;
            quotaFrame["gender", "Female"]["region", "North"].Target = 2;
            quotaFrame["gender", "Female"]["region", "South"].Target = 2;

            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Debug.Assert(quotaFrame.Target != null, "quotaFrame.Target != null");
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(2));

            var genderVariable = quotaFrame.VariableDefinitions.First();
            Assert.That(genderVariable.Id, Is.Not.Null);
            Assert.That(genderVariable.Name, Is.EqualTo("gender"));
            Assert.That(genderVariable.OdinVariableName, Is.EqualTo("gender"));
            Assert.That(genderVariable.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(genderVariable.Levels.Count, Is.EqualTo(2));
            Assert.That(genderVariable.Levels.First().Name, Is.EqualTo("Male"));
            Assert.That(genderVariable.Levels.ElementAt(1).Name, Is.EqualTo("Female"));

            var regionVariable = quotaFrame.VariableDefinitions.ElementAt(1);
            Assert.That(regionVariable.Id, Is.Not.Null);
            Assert.That(regionVariable.Name, Is.EqualTo("region"));
            Assert.That(regionVariable.OdinVariableName, Is.EqualTo("region"));
            Assert.That(regionVariable.IsSelectionOptional, Is.EqualTo(null));
            Assert.That(regionVariable.Levels.Count, Is.EqualTo(2));
            Assert.That(regionVariable.Levels.First().Name, Is.EqualTo("North"));
            Assert.That(regionVariable.Levels.ElementAt(1).Name, Is.EqualTo("South"));

            //// Bored yet?

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().DefinitionId, Is.EqualTo(genderVariable.Id));
            Assert.That(quotaFrame.FrameVariables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Target, Is.EqualTo(6));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().DefinitionId, Is.EqualTo(regionVariable.Id));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.First().Target, Is.EqualTo(3));
            Assert.That(quotaFrame.FrameVariables.First().Levels.First().Variables.First().Levels.ElementAt(1).Target, Is.EqualTo(3));

            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Target, Is.EqualTo(4));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().DefinitionId, Is.EqualTo(regionVariable.Id));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.First().Target, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables.First().Levels.ElementAt(1).Variables.First().Levels.ElementAt(1).Target, Is.EqualTo(2));
        }
    }
}