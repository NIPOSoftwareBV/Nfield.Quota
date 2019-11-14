using Nfield.Quota.Builders;
using Nfield.Quota.Models;
using Nfield.Quota.Persistence;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    public class QuotaFrameEncoderTests
    {
        [Test]
        public void OutputIsCamelCased()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string>() { "level" })
                .Structure(sb => sb.Variable("var"))
                .Build();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Contain("id"));
            Assert.That(json, Does.Contain("variableDefinitions"));
            Assert.That(json, Does.Contain("frameVariables"));
        }

        [Test]
        public void OutputDoesNotContainTargetsWhenTargetShouldNotBeIncluded()
        {
            var frame = new QuotaFrameBuilder()
                .Target(100)
                .VariableDefinition("varName", new List<string>() { "level1", "level2" })
                .Structure(sb => sb.Variable("varName"))
                .Build();

            frame["varName", "level1"].Target = 60;
            frame["varName", "level1"].MaxTarget = 65;

            // The default does not include targets.
            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Not.Contain("target"));
            Assert.That(json, Does.Not.Contain("maxTarget"));
        }

        [Test]
        public void OutputContainsTargetsWhenTargetShouldBeIncluded()
        {
            var frame = new QuotaFrameBuilder()
                .Target(100)
                .VariableDefinition("varName", new List<string>() { "level1", "level2" })
                .Structure(sb => sb.Variable("varName"))
                .Build();

            frame["varName", "level1"].Target = 60;
            frame["varName", "level1"].MaxTarget = 65;

            var json = QuotaFrameEncoder.Encode(frame, new QuotaFrameEncoderOptions() { IncludeTargets = true });

            Assert.That(json, Does.Contain("\"target\": 100"));
            Assert.That(json, Does.Contain("\"target\": 60"));
            Assert.That(json, Does.Contain("\"maxTarget\": 65"));
        }

        [Test]
        public void OutputDoesNotContainTargetWhenTargetIsLeftEmpty()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string>() { "level" })
                .Structure(sb => sb.Variable("var"))
                .Build();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Not.Contain("target"));
            Assert.That(json, Does.Not.Contain("maxTarget"));
        }

        [Test]
        public void EmptyCollectionsAreIncluded()
        {
            var frame = new QuotaFrame();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Contain("variableDefinitions"));
            Assert.That(json, Does.Contain("frameVariables"));
        }

        [Test]
        public void IsHiddenIsSerialized()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string> { "level" })
                .Structure(sb => sb.Variable("var"))
                .Build();

            frame["var"].IsHidden = true;
            frame["var", "level"].IsHidden = true;

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(Regex.Matches(json, @"""isHidden"": true").Count, Is.EqualTo(2));
        }

        [Test]
        public void IsSelectionOptionalIsSerialized()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string> { "level" }, VariableSelection.Optional)
                .Structure(sb => sb.Variable("var"))
                .Build();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(Regex.Matches(json, @"""isSelectionOptional"": true").Count, Is.EqualTo(1));
        }

        [Test]
        public void IsMultiIsSerialized()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string> { "level" }, isMulti: true)
                .Structure(sb => sb.Variable("var"))
                .Build();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(Regex.Matches(json, @"""isMulti"": true").Count, Is.EqualTo(1));
        }
    }
}