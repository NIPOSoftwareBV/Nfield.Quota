using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// WARNING: If this test breaks due to changes made to the structure, be very careful and consider backword compatibility with older (serialized) frames!!
        /// </remarks>>
        [Test]
        public void CanDeserialzeACommonV1Structure()
        {
            var filePath = Asset.GetAbsolutePath("glu-quota-format-v1-common.json");
            var jsonFrame = File.ReadAllText(filePath);

            var frame = QuotaFrameDecoder.Decode(jsonFrame);

            Assert.That(frame, Is.Not.Null);
            Assert.That(frame.Id, Is.EqualTo("frameId"));
            Assert.That(frame.Target, Is.Null);
            Assert.That(frame.VariableDefinitions, Has.Count.EqualTo(1));
            Assert.That(frame.FrameVariables, Has.Count.EqualTo(1));

            var varDef = frame.VariableDefinitions.First();
            Assert.That(varDef.Id, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(varDef.Name, Is.EqualTo("Region"));
            Assert.That(varDef.OdinVariableName, Is.EqualTo("regionOdin"));
            Assert.That(varDef.Levels, Has.Count.EqualTo(1));

            var levelDef = varDef.Levels.First();
            Assert.That(levelDef.Id, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(levelDef.Name, Is.EqualTo("Haarlem"));

            var variable = frame.FrameVariables.First();
            Assert.That(variable.Id, Is.EqualTo(Guid.Parse("5B7BCBB5-CF2B-4FE1-B5C3-42D440438268")));
            Assert.That(variable.DefinitionId, Is.EqualTo(Guid.Parse("234F6E29-A43B-41C7-A99C-6C8FFD177998")));
            Assert.That(variable.Levels, Has.Count.EqualTo(1));

            var level = variable.Levels.First();
            Assert.That(level.Id, Is.EqualTo(Guid.Parse("002CA83D-7768-4C12-8B74-4C20C165AA32")));
            Assert.That(level.DefinitionId, Is.EqualTo(Guid.Parse("BAD227D2-4DBD-47BA-B8B7-B03B89B42B47")));
            Assert.That(level.Target, Is.Null);
            Assert.That(level.Variables, Has.Count.EqualTo(0));
        }
    }
}
