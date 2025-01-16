using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class ExhaustivenessTests
{
    [TestMethod]
    public async Task Switch_Stmt_With_An_Unhandled_Type_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_An_Unhandled_Type_Complains_2()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains_2()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Two_Unhandled_Types_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Two_Unhandled_Types_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_A_Class_Requires_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchStmt(["string", "int"], [("string s", false), ("int", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Class_Requires_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchExpr(["string", "int"], [("string s", false), ("int", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_A_Nullable_Value_Type_Requires_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int?"], [("bool b", false), ("int", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Nullable_Value_Type_Requires_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int?"], [("bool b", false), ("int", false)], tagSwitch: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false)], tagSwitch: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false)], tagSwitch: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_All_Cases_Handled_Succeeds()
    {
        var code = Shared.GenerateSwitchStmt(
            ["string", "int", "List<bool>"],
            [new[] { ("string", false), ("int", false) }, ("List<bool>", false), ("null", false)],
            tagSwitch: true,
            ["System.Collections.Generic"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_All_Cases_Handled_Succeeds()
    {
        var code = Shared.GenerateSwitchExpr(
            ["string", "int", "List<bool>"],
            [("string", false), ("int", false), ("List<bool>", false), ("null", false)],
            tagSwitch: true,
            ["System.Collections.Generic"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Stmt_With_Only_Default_Case_Succeeds()
    {
        var code = Shared.GenerateSwitchStmt(["string", "int", "bool"], [("default", false)], tagSwitch: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Default_Case_Succeeds()
    {
        var code = Shared.GenerateSwitchExpr(["string", "int", "bool"], [("_", false)], tagSwitch: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }
}
