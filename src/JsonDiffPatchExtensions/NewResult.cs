namespace JsonDiffPatchExtensions;

public record struct NewResult(object? Value)
{
    public static NewResult From(object? val) => new NewResult(val);
}
