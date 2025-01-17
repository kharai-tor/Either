global using TaggedCase = (string Case, bool Tagged);
using System.Linq;

namespace RhymesOfUncertainty.Test;

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

    internal static string GenerateSwitchStmt
    (
        string[] typesToCheck,
        Either<TaggedCase, TaggedCase[]>[] casesChecked,
        bool tagSwitch = false,
        string[] usings = default,
        bool isNullForgiving = false
    )
    {
        int tagNumber = 0;

        var code =
            string.Join("\n", (usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Structs +
@$"
class C
{{
    void M(Either<{string.Join(", ", typesToCheck)}> x)
    {{
        {TagIfNecessary("switch", tagSwitch)} ({(isNullForgiving ? "x.Value!" : "x.Value")})
        {{
            {string.Join("\n", casesChecked.Select(GetCase).Select(c => $"{c} break;"))}
        }}
    }}
}}
";
        return code;

        string GetCase(Either<TaggedCase, TaggedCase[]> caseChecked)
        {
            return caseChecked.Value switch
            {
                TaggedCase tc => GetSingleCase(tc.Case, tc.Tagged),
                TaggedCase[] tcs => string.Join("\n", tcs.Select(tc => GetSingleCase(tc.Case, tc.Tagged)))
            };
        }

        string GetSingleCase(string @case, bool tagged)
        {
            return @case is "default" ? TagIfNecessary("default:", tagged) : TagIfNecessary($"case {@case}:", tagged);
        }

        string TagIfNecessary(string s, bool necessary)
        {
            return necessary ? $"{{|#{tagNumber++}:{s}|}}" : s;
        }
    }

    internal static string GenerateSwitchExpr(string[] typesToCheck, TaggedCase[] casesChecked, bool tagSwitch = false, string[] usings = default)
    {
        int tagNumber = 0;

        var code =
            string.Join("\n", (usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Structs +
@$"
class C
{{
    int M(Either<{string.Join(", ", typesToCheck)}> x)
    {{
        return x.Value {TagIfNecessary("switch", tagSwitch)}
        {{
            {string.Join(",\n", casesChecked.Select(GetCase))}
        }};
    }}
}}
";
        return code;

        string GetCase(TaggedCase tc, int index)
        {
            return $"{TagIfNecessary(tc.Case, tc.Tagged)} => {index}";
        }

        string TagIfNecessary(string s, bool necessary)
        {
            return necessary ? $"{{|#{tagNumber++}:{s}|}}" : s;
        }
    }
}