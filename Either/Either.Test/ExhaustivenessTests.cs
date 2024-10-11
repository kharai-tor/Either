using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class ExhaustivenessTests
{
    [TestMethod]
    public async Task Switch_Stmt_With_An_Unhandled_Type_Complains()
    {
        var code = GenerateSwitchStmt(["bool", "int"], ["bool b"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool b => 1"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_An_Unhandled_Type_Complains_2()
    {
        var code = GenerateSwitchStmt(["bool", "int"], ["bool"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains_2()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool => 1"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Two_Unhandled_Types_Complains()
    {
        var code = GenerateSwitchExpr(["bool", "int"], []);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Two_Unhandled_Types_Complains()
    {
        var code = GenerateSwitchExpr(["bool", "int"], []);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_A_Class_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchStmt(["string", "int"], ["string s", "int"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Class_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["string", "int"], ["string s => 1", "int => 2"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_A_Nullable_Value_Type_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchStmt(["bool", "int?"], ["bool b", "int"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Nullable_Value_Type_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["bool", "int?"], ["bool b => 1", "int => 2"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling()
    {
        var code = GenerateSwitchStmt(["bool", "int"], ["bool b", "int"]);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool b => 1", "int => 2"]);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_All_Cases_Handled_Succeeds()
    {
        var code = GenerateSwitchStmt(
            ["string", "int", "List<bool>"],
            [new[] { "string", "int" }, "List<bool>", "null"],
            ["System.Collections.Generic"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_All_Cases_Handled_Succeeds()
    {
        var code = GenerateSwitchExpr(
            ["string", "int", "List<bool>"],
            ["string => 1", "int => 2", "List<bool> => 3", "null => 4"],
            ["System.Collections.Generic"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Only_Default_Case_Succeeds()
    {
        var code = GenerateSwitchStmt(["string", "int", "bool"], ["default"]);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Default_Case_Succeeds()
    {
        var code = GenerateSwitchExpr(["string", "int", "bool"], ["_ => 1"]);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    private static string GenerateSwitchStmt(string[] typesToCheck, Either<string, string[]>[] casesChecked, string[] usings = default)
    {
        var code =
            string.Join("\n", (usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Shared.Structs +
@$"
class C
{{
    void M(Either<{string.Join(", ", typesToCheck)}> x)
    {{
        {{|#0:switch|}} (x.Value)
        {{
            {string.Join("\n", casesChecked.Select(GetCase).Select(c => $"{c} break;"))}
        }}
    }}
}}
";
        return code;

        static string GetCase(Either<string, string[]> caseChecked)
        {
            return caseChecked.Value switch
            {
                string @case => GetSingleCase(@case),
                string[] cases => string.Join("\n", cases.Select(GetSingleCase))
            };
        }

        static string GetSingleCase(string @case)
        {
            return @case is "default" ? "default:" : $"case {@case}:";
        }
    }

    private static string GenerateSwitchExpr(string[] typesToCheck, string[] casesChecked, string[] usings = default)
    {
        var code =
            string.Join("\n", (usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Shared.Structs +
@$"
class C
{{
    int M(Either<{string.Join(", ", typesToCheck)}> x)
    {{
        return x.Value {{|#0:switch|}}
        {{
            {string.Join(",\n", casesChecked)}
        }};
    }}
}}
";
        return code;
    }
}
