namespace Either.Test;

internal static class Shared
{
    internal static readonly string Structs = @"
readonly struct Either<T1, T2>
{
    public object Value { get; }
}
readonly struct Either<T1, T2, T3>
{
    public object Value { get; }
}
";
}
