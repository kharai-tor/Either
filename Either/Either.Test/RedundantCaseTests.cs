using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class RedundantCaseTests
{
    [Fact]
    public async Task Switch_Stmt_With_A_Redundant_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false), ("string s", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_A_Redundant_Case_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false), ("string s", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Stmt_With_A_Redundant_Null_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false), ("null", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_A_Redundant_Null_Case_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false), ("null", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Stmt_With_Multiple_Redundant_Cases_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false), ("string s", true), ("decimal", true), ("null", true)]);

        var expectedString = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");

        var expectedDecimal = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("decimal");

        var expectedNull = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(2)
            .WithArguments("null");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedString, expectedDecimal, expectedNull);
    }

    [Fact]
    public async Task Switch_Expr_With_Multiple_Redundant_Cases_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false), ("string s", true), ("decimal", true), ("null", true)]);

        var expectedString = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");

        var expectedDecimal = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("decimal");

        var expectedNull = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(2)
            .WithArguments("null");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedString, expectedDecimal, expectedNull);
    }

    [Fact]
    public async Task Switch_Stmt_With_Both_Cases_Handled_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false), ("default", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("Both");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_Both_Cases_Handled_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false), ("_", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("Both");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Stmt_With_All_Cases_Handled_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int", "decimal"], [("bool b", false), ("int", false), ("decimal d", false), ("default", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("All");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_All_Cases_Handled_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int", "decimal"], [("bool b", false), ("int", false), ("decimal d", false), ("_", true)]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("All");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Stmt_With_A_Redundant_Regular_Case_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["bool", "int"], [("bool b", false), ("int", false), ("decimal d", true), ("default", true)]);

        var expectedRegular = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("decimal");

        var expectedDefault = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(1)
            .WithArguments("Both");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedRegular, expectedDefault);
    }

    [Fact]
    public async Task Switch_Expr_With_A_Redundant_Regular_Case_And_A_Redundant_Default_Case_Complains()
    {
        var code = Shared.GenerateSwitchExpr(["bool", "int"], [("bool b", false), ("int", false), ("decimal d", true), ("_", true)]);

        var expectedRegular = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("decimal");

        var expectedDefault = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(1)
            .WithArguments("Both");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedRegular, expectedDefault);
    }
}
