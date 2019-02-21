using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nfield.Quota.Builders;
using Nfield.Quota.Models;
using Nfield.Quota.Persistence;
using NUnit.Framework;

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
        public void OutputDoesNotContainSuccessful()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("var", new List<string>() { "level" })
                .Structure(sb => sb.Variable("var"))
                .Build();

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Not.Contain("successful"));
            // Assert.That(json, Is.Not.StringContaining("50")); TODO Test properly (level id can contain this)
        }

        [Test]
        public void OutputDoesNotContainTargetWhenTargetIsSet()
        {
            var frame = new QuotaFrameBuilder()
                .VariableDefinition("varName", new List<string>() { "level" })
                .Structure(sb => sb.Variable("varName"))
                .Build();

            frame["varName", "level"].Target = 60;

            var json = QuotaFrameEncoder.Encode(frame);

            Assert.That(json, Does.Not.Contain("target"));
            // Assert.That(json, Is.Not.StringContaining("100")); TODO Test properly (level id can contain this)
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
    }
}