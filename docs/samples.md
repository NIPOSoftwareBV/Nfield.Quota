# Samples

This section contains a number of example quota frames with different structures.

## Minimal

The minimal quota frame only sets a total target.

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(100)
    .Build();
``` 

## One Variable

A small quota frame containing only a single variable with targets.

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(100)
    .VariableDefinition(
      variableName: "Gender",
      odinVariableName: "gender",
      levelNames: new [] { "Male", "Female" })
    .Structure(
      root => root.Variable("Gender")
      )
    .Build();
quotaFrame["Gender", "Male"].Target = 50;
quotaFrame["Gender", "Female"].Target = 50;
```

## Two independent variables

This quota frame contains two independent variables. 

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(100)
    .VariableDefinition(
      variableName: "Gender",
      odinVariableName: "gender",
      levelNames: new [] { "Male", "Female" })
    .VariableDefinition(
      variableName: "Region",
      odinVariableName: "region",
      levelNames : new [] { "North", "South", "West", "East"})
    .Structure(
      root => {
        root.Variable("Gender");
        root.Variable("Region");
      })
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

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(200)
    .VariableDefinition(
      variableName: "Gender",
      odinVariableName: "gender",
      levelNames: new [] { "Male", "Female" })
    .VariableDefinition(
      variableName: "Region",
      odinVariableName: "region",
      levelNames : new [] { "North", "South", "West", "East"})
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
