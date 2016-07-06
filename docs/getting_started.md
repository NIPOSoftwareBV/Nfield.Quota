# Prerequisites
In order to use this library you will need the following:

- .Net Framework 4.0 or later
- An account for [Nfield] with access to the API

# Installation
The preferred way to use this library in your solutions is to install the associated [NuGet] package. You can install 
the software by issueing the following command in the Package Manager console.
```
PM> Install-Package Nfield.Quota
```

# Set up a quota frame
The single most important class in this assembly is the _QuotaFrame_ class. This class will hold the variable 
definitions and the targets.

Let's start by creating the simplest quota frame.

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("<survey ID>")
    .Target(200)
    .Build();
```

This quota frame contains the minimum quota configuration, it only sets an overall target.

To identify groups, we can add a quota variable definition.

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("<survey ID>")
    .Target(200)
    .VariableDefinition(
      variableId: "gender",
      variableName: "Gender",
      odinVariableName: "GENDER",
      levels: new [] { "Male", "Female" })
    .Build();
```

This created a quota frame with a single _gender_ variable with two levels (groups). The value provided through the 
_odinVariableName_ parameter refers to the name of the _*SAMPLEDATA_ variable that should be used in ODIN script
to assign a respondent to a group.

To refer to this variable add the following to you ODIN questionnaire:

```ODIN
*SAMPLEDATA GENDER
```

After defining our variable we need to create the structure of the quota frame, but first we'll add another variable to make the example a bit more 'realistic'.

```
var quotaFrame = new QuotaFrameBuilder()
    .Id("<survey ID>")
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
      root => root.Variable("gender",
        gender => gender.Variable("region")
        )
      )
    .Build();
```
This creates a quota frame that nests the region variable under the gender variable. This will allow setting the target 
for a region in context of a gender.

```
quotaFrame["Gender", "Male"].Target = 100;
quotaFrame["Gender", "Male"]["Region", "North"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "South"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "West"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "East"].Target = 25;
// etc.
```  
Using indexers you can easily setup targets for the nested variables that were setup in the _Structure_ call in the 
previous code sample. If you index on a variable that is not available at that depth in the quota tree an 
[InvalidOperationException] will be thrown.

As demonstrated above it is possible to set target on higher level variables. Keep in mind that in Nfield this target
is a minimum, only the overall target is a maximum value.

# Next steps

Now that you have been introduced to the basics of setting up quota, refer to the [samples] for more
complex quota frames. 

[Nfield]: http://www.nfieldmr.com
[NuGet]: http://nuget.org
[samples]: samples.md
[InvalidOperationException]: https://msdn.microsoft.com/en-us/library/system.invalidoperationexception(v=vs.110).aspx