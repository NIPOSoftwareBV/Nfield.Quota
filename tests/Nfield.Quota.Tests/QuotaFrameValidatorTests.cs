using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nfield.Quota.Builders;
using NUnit.Framework;

namespace Nfield.Quota.Tests
{
    [TestFixture]
    internal class QuotaFrameValidatorTests
    {
        [Test]
        public void Definitions_HappyPath()
        {
            var quotaFrame = new QuotaFrameBuilder()
                .Id(Guid.NewGuid().ToString())
                .VariableDefinition(Guid.NewGuid().ToString(), "varName", "odinVarName",
                    new List<string>() {"level1Name", "level2Name"})
                .Structure(sb => { })
                .Build();

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Definitions_EveryVariableNeedsAtLeastOneLevel()
        {
            var variableId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions has variables with no levels. Affected variable definition id: '{0}'",
                variableId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };
            var variable = new QuotaVariableDefinition
            {
                Id = variableId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };
            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CannotContainDuplicateIds()
        {
            var nonUniqueId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };
            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = nonUniqueId,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = nonUniqueId,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CannotContainDuplicateNames()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = nonUniqueName
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = nonUniqueName
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate name. Duplicate name: 'non-unique'"));
        }

        [Test]
        public void Definitions_CannotContainDuplicateIdsBetweenVariablesAndLevels()
        {
            var nonUniqueId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame definitions contain a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = nonUniqueId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = nonUniqueId,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Definitions_CannotContainDuplicateNamesBetweenVariablesAndLevels()
        {
            const string nonUniqueName = "non-unique";

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = nonUniqueName,
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = nonUniqueName
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo("Quota frame definitions contain a duplicate name. Duplicate name: 'non-unique'"));
        }

        [Test]
        public void Frame_HappyPath()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = Guid.NewGuid().ToString()
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel2);

            quotaFrame.FrameVariables.Add(frameVariable);


            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Frame_CannotContainDuplicateIds()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var nonUniqueId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = nonUniqueId
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = nonUniqueId
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel2);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainDuplicateIdInLevels()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var nonUniqueId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a duplicate id. Duplicate id: '{0}'",
                nonUniqueId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = Guid.NewGuid().ToString()
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = nonUniqueId
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = nonUniqueId
            };
            frameVariable.Levels.Add(frameLevel2);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainAReferenceToANonExistingVariableDefinition()
        {
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var nonExistingId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                nonExistingId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = nonExistingId,
                Id = Guid.NewGuid().ToString()
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel2);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotContainAReferenceToANonExistingLevelDefinition()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var nonExistingId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                nonExistingId);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = Guid.NewGuid().ToString()
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = nonExistingId,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel2);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveLessLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var varReferenceId = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a variable that doesnt have all the defined levels associated. Affected frame variable id: '{0}', missing level definition id: '{1}'",
                varReferenceId, level2Id);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = varReferenceId
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel1); // Level 2 omitted

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void Frame_CannotHaveMoreLevelsInTheFrameThanUnderTheReferencedVariableDefinition()
        {
            var varId = Guid.NewGuid().ToString();
            var level1Id = Guid.NewGuid().ToString();
            var level2Id = Guid.NewGuid().ToString();
            var level3Id = Guid.NewGuid().ToString();
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame contains a reference to a non-existing definition. Definition id: '{0}'",
                level3Id);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable = new QuotaVariableDefinition
            {
                Id = varId,
                Name = "varName",
                OdinVariableName = "odinVarName"
            };

            var level1 = new QuotaLevelDefinition
            {
                Id = level1Id,
                Name = "level1Name"
            };
            variable.Levels.Add(level1);

            var level2 = new QuotaLevelDefinition
            {
                Id = level2Id,
                Name = "level2Name"
            };
            variable.Levels.Add(level2);

            quotaFrame.VariableDefinitions.Add(variable);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = varId,
                Id = Guid.NewGuid().ToString()
            };

            var frameLevel1 = new QuotaFrameLevel()
            {
                DefinitionId = level1Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel1);

            var frameLevel2 = new QuotaFrameLevel()
            {
                DefinitionId = level2Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel2);

            var frameLevel3 = new QuotaFrameLevel()
            {
                DefinitionId = level3Id,
                Id = Guid.NewGuid().ToString()
            };
            frameVariable.Levels.Add(frameLevel3);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void ComplexFrame_HappyPath()
        {
            var var1Id = Guid.NewGuid().ToString();
            var var2Id = Guid.NewGuid().ToString();
            var var1Level1Id = Guid.NewGuid().ToString();
            var var1Level2Id = Guid.NewGuid().ToString();
            var var2Level1Id = Guid.NewGuid().ToString();
            var var2Level2Id = Guid.NewGuid().ToString();

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable1 = new QuotaVariableDefinition
            {
                Id = var1Id,
                Name = "var1Name",
                OdinVariableName = "odinVar1Name"
            };

            var var1Level1 = new QuotaLevelDefinition
            {
                Id = var1Level1Id,
                Name = "var1Level1Name"
            };
            variable1.Levels.Add(var1Level1);

            var var1Level2 = new QuotaLevelDefinition
            {
                Id = var1Level2Id,
                Name = "var1Level2Name"
            };
            variable1.Levels.Add(var1Level2);

            quotaFrame.VariableDefinitions.Add(variable1);

            var variable2 = new QuotaVariableDefinition
            {
                Id = var2Id,
                Name = "var2Name",
                OdinVariableName = "odinVar2Name"
            };

            var var2Level1 = new QuotaLevelDefinition
            {
                Id = var2Level1Id,
                Name = "var2Level1Name"
            };
            variable2.Levels.Add(var2Level1);

            var var2Level2 = new QuotaLevelDefinition
            {
                Id = var2Level2Id,
                Name = "var2Level2Name"
            };
            variable2.Levels.Add(var2Level2);

            quotaFrame.VariableDefinitions.Add(variable2);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = var1Id,
                Id = Guid.NewGuid().ToString()
            };

            var frameVariable2 = new QuotaFrameVariable()
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid().ToString()
            };

            frameVariable2.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level1Id, Id = Guid.NewGuid().ToString() });
            frameVariable2.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level2Id, Id = Guid.NewGuid().ToString() });

            var frameLevel1Var1 = new QuotaFrameLevel()
            {
                DefinitionId = var1Level1Id,
                Id = Guid.NewGuid().ToString()
            };
            frameLevel1Var1.Variables.Add(frameVariable2);

            frameVariable.Levels.Add(frameLevel1Var1);

            var frameLevel2Var1 = new QuotaFrameLevel()
            {
                DefinitionId = var1Level2Id,
                Id = Guid.NewGuid().ToString()
            };

            var frameVariable3 = new QuotaFrameVariable()
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid().ToString()
            };

            frameVariable3.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level1Id, Id = Guid.NewGuid().ToString() });
            frameVariable3.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level2Id, Id = Guid.NewGuid().ToString() });

            frameLevel2Var1.Variables.Add(frameVariable3);

            frameVariable.Levels.Add(frameLevel2Var1);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ComplexFrame_MissingVariableUnderOneOfTheLevels()
        {
            var var1Id = Guid.NewGuid().ToString();
            var var2Id = Guid.NewGuid().ToString();
            var var1Level1Id = Guid.NewGuid().ToString();
            var var1Level2Id = Guid.NewGuid().ToString();
            var var2Level1Id = Guid.NewGuid().ToString();
            var var2Level2Id = Guid.NewGuid().ToString();
            var frameVar1Id = Guid.NewGuid().ToString();
            var frameLvl4Id = Guid.NewGuid().ToString();

            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture,
                "Quota frame invalid. All levels of a variable should have the same variables underneath. Frame variable id '{0}' has a mismatch for level '{1}'",
                frameVar1Id, frameLvl4Id);

            var quotaFrame = new QuotaFrame()
            {
                Id = Guid.NewGuid().ToString()
            };

            var variable1 = new QuotaVariableDefinition
            {
                Id = var1Id,
                Name = "var1Name",
                OdinVariableName = "odinVar1Name"
            };

            var var1Level1 = new QuotaLevelDefinition
            {
                Id = var1Level1Id,
                Name = "var1Level1Name"
            };
            variable1.Levels.Add(var1Level1);

            var var1Level2 = new QuotaLevelDefinition
            {
                Id = var1Level2Id,
                Name = "var1Level2Name"
            };
            variable1.Levels.Add(var1Level2);

            quotaFrame.VariableDefinitions.Add(variable1);

            var variable2 = new QuotaVariableDefinition
            {
                Id = var2Id,
                Name = "var2Name",
                OdinVariableName = "odinVar2Name"
            };

            var var2Level1 = new QuotaLevelDefinition
            {
                Id = var2Level1Id,
                Name = "var2Level1Name"
            };
            variable2.Levels.Add(var2Level1);

            var var2Level2 = new QuotaLevelDefinition
            {
                Id = var2Level2Id,
                Name = "var2Level2Name"
            };
            variable2.Levels.Add(var2Level2);

            quotaFrame.VariableDefinitions.Add(variable2);

            var frameVariable = new QuotaFrameVariable()
            {
                DefinitionId = var1Id,
                Id = frameVar1Id
            };

            var frameVariable2 = new QuotaFrameVariable()
            {
                DefinitionId = var2Id,
                Id = Guid.NewGuid().ToString()
            };
            
            frameVariable2.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level1Id, Id = Guid.NewGuid().ToString() });
            frameVariable2.Levels.Add(new QuotaFrameLevel { DefinitionId = var2Level2Id, Id = Guid.NewGuid().ToString() });

            var frameLevel1Var1 = new QuotaFrameLevel()
            {
                DefinitionId = var1Level1Id,
                Id = Guid.NewGuid().ToString()                
            };
            frameLevel1Var1.Variables.Add(frameVariable2);

            frameVariable.Levels.Add(frameLevel1Var1);

            var frameLevel2Var1 = new QuotaFrameLevel()
            {
                DefinitionId = var1Level2Id,
                Id = frameLvl4Id
            };
            frameVariable.Levels.Add(frameLevel2Var1);

            quotaFrame.FrameVariables.Add(frameVariable);

            var validator = new QuotaFrameValidator();
            var result = validator.Validate(quotaFrame);

            Assert.That(result.Errors.Single().ErrorMessage,
                Is.EqualTo(expectedErrorMessage));
        }
    }
}
