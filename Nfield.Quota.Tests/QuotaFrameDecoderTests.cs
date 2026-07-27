using System;
using System.IO;
using System.Linq;
using Nfield.Quota.Persistence;
using Nfield.Quota.Tests.Assets;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaFrameDecoderTests
    {
        /// <remarks>
        /// Successful and target are not stored in this structure by design, they are stored in separate tables
        /// 
        /// WARNING: If this test breaks due to changes made to the structure, be very careful and consider backward compatibility with older (serialized) frames!!
        /// </remarks>>
        [Test]
        public void CanDeserializeACommonV1Structure()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v1-common.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsTargetable, Is.False);
            Assert.That(varDef.IsForAllocationOnly, Is.False);
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.Levels, Has.Count.EqualTo(1));
            Assert.That(variable.IsHidden, Is.False); //v1 doesn't have it defined, should be false
            Assert.That(variable.IsForAllocationOnly, Is.False); //v1 doesn't have it defined, should be false

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
            Assert.That(level.IsHidden, Is.False); //v1 doesn't have it defined, should be false
        }

        [Test]
        public void CanDeserializeACommonV2StructureWithIsHidden()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v2-common-AddedIsHidden.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsTargetable, Is.False);
            Assert.That(varDef.IsForAllocationOnly, Is.False); //v2 doesn't have it defined, should be false
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.Levels, Has.Count.EqualTo(1));
            Assert.That(variable.IsHidden, Is.True);
            Assert.That(variable.IsForAllocationOnly, Is.False); //v2 doesn't have it defined, should be false

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
            Assert.That(level.IsHidden, Is.True);
        }

        [Test]
        public void CanDeserializeACommonV3StructureWithIsSelectionOptional()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v3-common-AddedIsSelectionOptional.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(varDef.IsTargetable, Is.False);
            Assert.That(varDef.IsForAllocationOnly, Is.False); //v3 doesn't have it defined, should be false
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.IsForAllocationOnly, Is.False); //v3 doesn't have it defined, should be false
            Assert.That(variable.Levels, Has.Count.EqualTo(1));

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
        }

        [Test]
        public void CanDeserializeACommonV4StructureWithIsMulti()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v4-common-AddedIsMulti.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(varDef.IsMulti);
            Assert.That(varDef.IsTargetable, Is.False);
            Assert.That(varDef.IsForAllocationOnly, Is.False); //v4 doesn't have it defined, should be false
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.IsForAllocationOnly, Is.False); //v4 doesn't have it defined, should be false
            Assert.That(variable.Levels, Has.Count.EqualTo(1));

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
        }

        [Test]
        public void CanDeserializeACommonV6StructureWithIsTargetable()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v6-common-AddedIsTargetable.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(varDef.IsMulti);
            Assert.That(varDef.IsTargetable);
            Assert.That(varDef.IsForAllocationOnly, Is.False); //v6 doesn't have it defined, should be false
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.IsForAllocationOnly, Is.False); //v6 doesn't have it defined, should be false
            Assert.That(variable.Levels, Has.Count.EqualTo(1));

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
        }

        [Test]
        public void CanDeserializeACommonV7StructureWithIsForAllocationOnly()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v7-common-AddedIsForAllocationOnly.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(2));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(2));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.IsSelectionOptional, Is.EqualTo(true));
            Assert.That(varDef.IsMulti);
            Assert.That(varDef.IsTargetable);
            Assert.That(varDef.IsForAllocationOnly);
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var varDef2 = frame.VariableDefinitions.ElementAt(1);
            Assert.That(varDef2.Id, Is.EqualTo(Guid.Parse("8D81EF06-477C-40EA-BE9E-2F54D0266C5D")));
            Assert.That(varDef2.Name, Is.EqualTo("Gender"));
            Assert.That(varDef2.OdinVariableName, Is.EqualTo("genderOdin"));
            Assert.That(varDef2.IsSelectionOptional, Is.EqualTo(false));
            Assert.That(varDef2.IsMulti, Is.EqualTo(false));
            Assert.That(varDef2.IsTargetable, Is.EqualTo(false));
            Assert.That(varDef2.IsForAllocationOnly, Is.EqualTo(false));
            Assert.That(varDef2.Levels, Has.Count.EqualTo(1));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.IsForAllocationOnly, Is.EqualTo(false));
            Assert.That(variable.Levels, Has.Count.EqualTo(1));

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.MaxTarget, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));

            var variable2 = frame.FrameVariables.ElementAt(1);
            Assert.That(variable2.Id, Is.EqualTo(Guid.Parse("A56C06CC-C12B-4543-8FC9-0434A891F309")));
            Assert.That(variable2.DefinitionId, Is.EqualTo(Guid.Parse("8D81EF06-477C-40EA-BE9E-2F54D0266C5D")));
            Assert.That(variable2.IsForAllocationOnly, Is.EqualTo(true));
            Assert.That(variable2.Levels, Has.Count.EqualTo(1));

            var level2 = variable2.Levels.First();
            Assert.That(level2.Id, Is.EqualTo(Guid.Parse("512F791A-58DB-4EBC-95B7-21D1C87E789E")));
            Assert.That(level2.DefinitionId, Is.EqualTo(Guid.Parse("A5AA65EA-EE5F-488E-9851-270DBF2C842D")));
            Assert.That(level2.Target, Is.Null);
            Assert.That(level2.MaxTarget, Is.Null);
            Assert.That(level2.Variables, Has.Count.EqualTo(0));
        }

        [Test]
        public void CanDeserializeACommonV80StructureWithRootLevelMaxOvershoot()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v80-common-AddedRootLevelMaxOvershoot.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.MaxOvershoot, Is.EqualTo(5));
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));
        }
    }
}