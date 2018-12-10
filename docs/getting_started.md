# Prerequisites
In order to use this library you will need the following:

* A compatible framework:
    * .Net Framework 4.6.1 or later
    * .Net Standard 2.0 or later
    * .Net Core 2.0 or later
* An account for [Nfield] with access to the API

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

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(200)
    .Build();
```

This quota frame contains the minimum quota configuration, it only sets an overall target.

To identify groups, we can add a quota variable definition.

```csharp
var quotaFrame = new QuotaFrameBuilder()
    .Target(200)
    .VariableDefinition(
      variableName: "Gender",
      odinVariableName: "gender",
      levelNames: new [] { "Male", "Female" })
    .Build();
```

This created a quota frame with a single _gender_ variable with two levels (groups). The value provided through the 
`odinVariableName` parameter refers to the name of the `*SAMPLEDATA` variable that should be used in ODIN questionnaire
to assign a respondent to a group.

To refer to this variable add the following to you ODIN questionnaire:  

```text
*SAMPLEDATA gender
```
 
After defining our variable we need to create the structure of the quota frame, but first we'll add another variable to make the example a bit more 'realistic'.

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
      root => root.Variable("gender",
        gender => gender.Variable("region")
        )
      )
    .Build();
```
This creates a quota frame that interlocks the region variable with the gender variable. This will allow setting the 
target for a region in context of a gender.

```csharp
quotaFrame["Gender", "Male"].Target = 100;
quotaFrame["Gender", "Male"]["Region", "North"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "South"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "West"].Target = 25;
quotaFrame["Gender", "Male"]["Region", "East"].Target = 25;
// etc.
```  
Using indexers you can easily setup targets for the nested variables that were setup in the _Structure_ call in the 
previous code sample. If you index on a variable that is not available at that depth in the quota tree an 
[InvalidOperationException] will be thrown that will indicate the variable that cannot be found.

As demonstrated above it is possible to set targets on levels higher in the hierarchy. Keep in mind that in 
Nfield this target is a minimum, only the overall target is a maximum value.

# Next steps

Now that you have been introduced to the basics of setting up quota, refer to the [samples] for more
complex quota frames. 

[Nfield]: http://www.nfieldmr.com
[NuGet]: https://www.nuget.org/packages/Nfield.Quota
[samples]: samples.md
[InvalidOperationException]: https://msdn.microsoft.com/en-us/library/system.invalidoperationexception(v=vs.110).aspx
