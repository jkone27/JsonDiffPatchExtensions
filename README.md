# JsonDiffPatchExtensions
some extensions to jsondiffpatch dotnet to allow ignoring specific fields


## Ignore Properties

```csharp
 [Fact]
    public void JsonDiffPatchWrapper_IgnoreDiff_Ok()
    {

        var objectOne = new { 
            TestField = "hello",
            A = "A"
        };

         var objectTwo = new { 
            TestField = "hello"
        };

        var diffObj = JsonDiffPatchWrapper.DiffIgnoreProperties(
            OldResult.From(objectOne), 
            NewResult.From(objectTwo),
            ignoreProperties: "A");

        Assert.Null(diffObj);
    }

```

## Ignore casing in diffs

```csharp

    [Fact]
    public void JsonDiffPatchWrapper_IgnoreCasing_Ok()
    {

        var objectOne = new { 
            TestField = "hello",
            A = "HELLO"
        };

         var objectTwo = new { 
            TestField = "hello",
            A = "hello"
        };

        var diffObj = JsonDiffPatchWrapper.DiffIgnoreCasingInValuesForProperties(
            OldResult.From(objectOne), 
            NewResult.From(objectTwo),
            ignoreCasingForProperties: "A");

        Assert.Null(diffObj);
    }
