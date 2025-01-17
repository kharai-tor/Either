using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class NullForgivingExpressionTests
{
    [TestMethod]
    public async Task Switch_Stmt_With_Null_Forgiving_Expr_And_No_Null_Case_Succeeds()
    {
        var code = Shared.GenerateSwitchStmt(["int", "string"], [("int", false), ("string", false)], isNullForgiving: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    //TODO how about nullable structs?

    [TestMethod]
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
