using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class NullForgivingExpressionTests
{
    [Fact]
    public async Task Switch_Stmt_With_Null_Forgiving_Expr_And_No_Null_Case_Succeeds()
    {
        var code = Shared.GenerateSwitchStmt(["int", "string"], [("int", false), ("string", false)], isNullForgiving: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    //TODO how about nullable structs?

    [Fact]
    public async Task Switch_Stmt_With_Null_Forgiving_Expr_And_Null_Case_Complains()
    {
        var code = Shared.GenerateSwitchStmt(["int", "string"], [("int", false), ("string", false), ("null", true)], isNullForgiving: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    //TODO when all types are non-nullable, null-forgiving is redundant
}
