using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaFrameBuilderTests
    {
        [Test]
        public void BuildingSimpleTreeCreatesCorrectQuotaFrame()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .Target(10)
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

            Assert.That(quotaFrame.Id, Is.EqualTo("id"));
            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.VariableDefinitions[0].Id, Is.EqualTo("varId"));
            Assert.That(quotaFrame.VariableDefinitions[0].Name, Is.EqualTo("varName"));
            Assert.That(quotaFrame.VariableDefinitions[0].OdinVariableName, Is.EqualTo("odinVarName"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[0].Id, Is.EqualTo("level1Id"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[0].Name, Is.EqualTo("level1Name"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[1].Id, Is.EqualTo("level2Id"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[1].Name, Is.EqualTo("level2Name"));

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables[0].Id, Is.EqualTo("varReferenceId"));
            Assert.That(quotaFrame.FrameVariables[0].DefinitionId, Is.EqualTo("varId"));
            Assert.That(quotaFrame.FrameVariables[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Id, Is.EqualTo("level1RefId"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].DefinitionId, Is.EqualTo("level1Id"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Target, Is.EqualTo(6));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Successful, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Id, Is.EqualTo("level2RefId"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].DefinitionId, Is.EqualTo("level2Id"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Target, Is.EqualTo(4));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Successful, Is.EqualTo(3));

        }

        [Test]
        public void BuildingNestedTreeCreatesCorrectQuotaFrame()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id("id")
                .Target(10)
                .VariableDefinition("gender", gender =>
                {
                    gender.Level("male", "Male");
                    gender.Level("female", "Female");
                })
                .VariableDefinition("region", region =>
                {
                    region.Level("north", "North");
                    region.Level("south", "South");
                })
                .FrameVariable("gender", gender =>
                {
                    gender.Level("male", 6, 2, male =>
                    {
                        male.Variable("region", region =>
                        {
                            region.Level("north", 3, 1);
                            region.Level("south", 3, 1);
                        });
                    });
                    gender.Level("female", 4, 3, female =>
                    {
                        female.Variable("region", region =>
                        {
                            region.Level("north", 2, 2);
                            region.Level("south", 2, 1);
                        });
                    });
                })
                .Build();

            Assert.That(quotaFrame.Id, Is.EqualTo("id"));
            Assert.That(quotaFrame.Target.HasValue, Is.True);
            Assert.That(quotaFrame.Target.Value, Is.EqualTo(10));

            Assert.That(quotaFrame.VariableDefinitions.Count, Is.EqualTo(2));

            Assert.That(quotaFrame.VariableDefinitions[0].Id, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions[0].Name, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions[0].OdinVariableName, Is.EqualTo("gender"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[0].Id, Is.EqualTo("male"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[0].Name, Is.EqualTo("Male"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[1].Id, Is.EqualTo("female"));
            Assert.That(quotaFrame.VariableDefinitions[0].Levels[1].Name, Is.EqualTo("Female"));

            Assert.That(quotaFrame.VariableDefinitions[1].Id, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions[1].Name, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions[1].OdinVariableName, Is.EqualTo("region"));
            Assert.That(quotaFrame.VariableDefinitions[1].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.VariableDefinitions[1].Levels[0].Id, Is.EqualTo("north"));
            Assert.That(quotaFrame.VariableDefinitions[1].Levels[0].Name, Is.EqualTo("North"));
            Assert.That(quotaFrame.VariableDefinitions[1].Levels[1].Id, Is.EqualTo("south"));
            Assert.That(quotaFrame.VariableDefinitions[1].Levels[1].Name, Is.EqualTo("South"));

            // Bored yet?

            Assert.That(quotaFrame.FrameVariables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables[0].DefinitionId, Is.EqualTo("gender"));
            Assert.That(quotaFrame.FrameVariables[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].DefinitionId, Is.EqualTo("male"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Target, Is.EqualTo(6));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Successful, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].DefinitionId, Is.EqualTo("region"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[0].DefinitionId, Is.EqualTo("north"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[0].Target, Is.EqualTo(3));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[0].Successful, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[1].DefinitionId, Is.EqualTo("south"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[1].Target, Is.EqualTo(3));
            Assert.That(quotaFrame.FrameVariables[0].Levels[0].Variables[0].Levels[1].Successful, Is.EqualTo(1));


            Assert.That(quotaFrame.FrameVariables[0].Levels[1].DefinitionId, Is.EqualTo("female"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Target, Is.EqualTo(4));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Successful, Is.EqualTo(3));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables.Count, Is.EqualTo(1));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].DefinitionId, Is.EqualTo("region"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels.Count, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[0].DefinitionId, Is.EqualTo("north"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[0].Target, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[0].Successful, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[1].DefinitionId, Is.EqualTo("south"));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[1].Target, Is.EqualTo(2));
            Assert.That(quotaFrame.FrameVariables[0].Levels[1].Variables[0].Levels[1].Successful, Is.EqualTo(1));

        }
    }
}