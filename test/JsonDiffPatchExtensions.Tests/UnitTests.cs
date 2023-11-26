namespace JsonDiffPatchExtensions.Tests;

using Xunit;

public class UnitTests
{


    [Fact]
    public void JsonDiffPatchWrapper_NoDiffs_ReturnsNull()
    {
        var objectOne = new { 
            TestField = "hello"
        };

         var objectTwo = new { 
            TestField = "hello"
        };

        var diffObj = JsonDiffPatchWrapper.Diff(OldResult.From(objectOne), NewResult.From(objectTwo));

        Assert.Null(diffObj);

    }

    [Fact]
    public void JsonDiffPatchWrapper_Diff_Ok()
    {

        var objectOne = new { 
            TestField = "hello",
            A = "A"
        };

         var objectTwo = new { 
            TestField = "hello"
        };

        var diffObj = JsonDiffPatchWrapper.Diff(OldResult.From(objectOne), NewResult.From(objectTwo));

        Assert.NotNull(diffObj);
    }

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
}