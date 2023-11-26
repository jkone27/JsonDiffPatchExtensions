namespace JsonDiffPatchExtensions;

public record struct OldResult(object? Value)
{
    public static OldResult From(object? val) => new OldResult(val);
}
