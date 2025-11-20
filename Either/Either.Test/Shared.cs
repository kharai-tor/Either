global using System.Threading.Tasks;
global using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhymesOfUncertainty.Test;

internal static class Shared
{
    internal static readonly string Types = @"
readonly struct Either<T1, T2>
{
    public object Thing { get; }
}

readonly struct Either<T1, T2, T3>
{
    public object Thing { get; }
}

interface IEither<T1, T2>
{
    object Thing { get; }
}

struct IntOrBool : IEither<int, bool>
{
    public object Thing { get; }
}

struct Result<T> : IEither<T, System.Exception>
{
    public object Thing { get; }
}
";

    internal static string GenerateSwitch(SwitchType switchType, IList<string> typesToCheck, IList<string> casesChecked, SwitchGenerationOptions options = null)
    {
        var unionType = $"Either<{string.Join(", ", typesToCheck)}>";
        return GenerateSwitchForANamedUnion(switchType, unionType, casesChecked, options);
    }

    internal static string GenerateSwitchForANamedUnion(SwitchType switchType, string namedUnionType, IList<string> casesChecked, SwitchGenerationOptions options = null)
    {
        return switchType switch
        {
            SwitchType.Stmt => GenerateSwitchStmt(namedUnionType, casesChecked, options ?? new()),
            SwitchType.Expr => GenerateSwitchExpr(namedUnionType, casesChecked, options ?? new()),
            _ => throw new ArgumentOutOfRangeException(nameof(switchType)),
        };
    }

    private static string GenerateSwitchStmt(string unionType, IList<string> casesChecked, SwitchGenerationOptions options)
    {
        int tagNumber = 0;

        var code =
            string.Join("\n", (options.Usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Types +
@$"
class C
{{
    void M({unionType} x)
    {{
        {TagIfNecessary("switch", options.TagSwitch)} ({Expr()})
        {{
            {string.Join("\r\n            ", casesChecked.Select(GetCase).Select(c => $"{c}\n                break;"))}
        }}
    }}
}}
";
        return code;

        string Expr()
        {
            return TagIfNecessary(options.IsNullForgiving ? "x.Thing!" : "x.Thing", options.TagExpr);
        }

        string GetCase(string caseChecked)
        {
            return string.Join("\n            ", caseChecked.Split('|').Select(GetSingleCase));
        }

        string GetSingleCase(string @case)
        {
            var tagged = @case.StartsWith("t:");
            @case = tagged ? @case[2..] : @case;
            return @case is "default" ? TagIfNecessary("default:", tagged) : TagIfNecessary($"case {@case}:", tagged);
        }

        string TagIfNecessary(string s, bool necessary)
        {
            return necessary ? $"{{|#{tagNumber++}:{s}|}}" : s;
        }
    }

    private static string GenerateSwitchExpr(string unionType, IList<string> casesChecked, SwitchGenerationOptions options)
    {
        int tagNumber = 0;

        var code =
            string.Join("\n", (options.Usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Types +
@$"
class C
{{
    int M({unionType} x)
    {{
        return {Expr()} {TagIfNecessary("switch", options.TagSwitch)}
        {{
            {string.Join(",\n            ", casesChecked.Select(GetCase))}
        }};
    }}
}}
";
        return code;

        string Expr()
        {
            return TagIfNecessary(options.IsNullForgiving ? "x.Thing!" : "x.Thing", options.TagExpr);
        }

        string GetCase(string caseChecked)
        {
            var tagged = caseChecked.StartsWith("t:");
            caseChecked = tagged ? caseChecked[2..] : caseChecked;
            return $"{TagIfNecessary(caseChecked, tagged)} => 0";
        }

        string TagIfNecessary(string s, bool necessary)
        {
            return necessary ? $"{{|#{tagNumber++}:{s}|}}" : s;
        }
    }
}

public enum SwitchType
{
    Stmt,
    Expr,
}

internal class SwitchGenerationOptions
{
    internal bool TagSwitch { get; init; } = false;
    internal string[] Usings { get; init; } = default;
    internal bool IsNullForgiving { get; init; } = false;
    internal bool TagExpr { get; init; } = false;
}