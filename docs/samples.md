# Samples

This section contains a number of example quota frames with different structures.

## Minimal

The minimal quota frame only sets a total target.

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("721C2E57-1232-48E3-9C3C-822D89B0B635") // the ID of the survey
    .Target(100)
    .Build();
``` 

## One Variable

A small quota frame containing only a single variable with targets.

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("721C2E57-1232-48E3-9C3C-822D89B0B635") // the ID of the survey
    .Target(100)
    .VariableDefinition(
      variableId: "gender",
      variableName: "Gender",
      odinVariableName: "GENDER",
      levels: new [] { "Male", "Female" })
    .Structure(
      root => root.Variable("Gender")
    .Build();
quotaFrame["Gender", "Male"].Target = 50;
quotaFrame["Gender", "Female"].Target = 50;
```

## Two independent variables

This quota frame contains two independent variables. 

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("721C2E57-1232-48E3-9C3C-822D89B0B635") // the ID of the survey
    .Target(100)
    .VariableDefinition(
      variableId: "gender",
      variableName: "Gender",
      odinVariableName: "GENDER",
      levels: new [] { "Male", "Female" })
    .VariableDefinition(
      variableId: "region",
      variableName: "Region",
      odinVariableName: "REGION",
      levels : new [] { "North", "South", "West", "East"})
    .Structure(
      root => {
        root.Variable("Gender");
        root.Variable("Region");
    .Build();
quotaFrame["Gender", "Male"].Target = 50;
quotaFrame["Gender", "Female"].Target = 50;
quotaFrame["Region", "North"].Target = 25;
quotaFrame["Region", "South"].Target = 25;
quotaFrame["Region", "West"].Target = 25;
quotaFrame["Region", "East"].Target = 25;
```

## Two interlocked variables

This quota frame contains two interlocked variables, allowing more fine-grained control over the distribution
of respondents. 

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("721C2E57-1232-48E3-9C3C-822D89B0B635") // the ID of the survey
    .Target(200)
    .VariableDefinition(
      variableId: "gender",
      variableName: "Gender",
      odinVariableName: "GENDER",
      levels: new [] { "Male", "Female" })
    .VariableDefinition(
      variableId: "region",
      variableName: "Region",
      odinVariableName: "REGION",
      levels : new [] { "North", "South", "West", "East"})
    .Structure(
      root => root.Variable("Gender", gender =>
        gender.Variable("Region")
        )
      )
    .Build();
quotaFrame["Gender", "Male"].Target = 100;
quotaFrame["Gender", "Male"]["Region", "North"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "South"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "West"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "East"].Target = 25;

quotaFrame["Gender", "Female"].Target = 100;
quotaFrame["Gender", "Female"]["Region", "North"].Target = 25;
quotaFrame["Gender", "Female"]["Region", "South"].Target = 25;
quotaFrame["Gender", "Female"]["Region", "West"].Target = 25;
quotaFrame["Gender", "Female"]["Region", "East"].Target = 25;
```
