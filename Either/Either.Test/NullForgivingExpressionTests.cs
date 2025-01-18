using System.Linq;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class NullForgivingExpressionTests
{
    [Theory]
    [InlineData(new[] { "int", "string" }, new[] { "int", "string" })]
    [InlineData(new[] { "int", "bool?" }, new[] { "int", "bool" })]
    public async Task Switch_Stmt_With_Null_Forgiving_Expr_And_No_Null_Case_Succeeds(string[] typesToCheck, string[] untaggedCasesChecked)
    {
        var casesChecked = untaggedCasesChecked.Select(c => (c, false)).ToArray();
        var code = Shared.GenerateSwitchStmt(typesToCheck, [casesChecked], isNullForgiving: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(new[] { "int", "string" }, new[] { "int", "string" }, new[] { "null" })]
    [InlineData(new[] { "int", "bool?" }, new[] { "int", "bool" }, new[] { "null" })]
    public async Task Switch_Stmt_With_Null_Forgiving_Expr_And_Null_Case_Complains(string[] typesToCheck, string[] untaggedCasesChecked, string[] taggedCasesChecked)
    {
        var casesChecked = untaggedCasesChecked.Select(c => (c, false))
            .Concat(taggedCasesChecked.Select(c => (c, true)))
            .ToArray();
        var code = Shared.GenerateSwitchStmt(typesToCheck, [casesChecked], isNullForgiving: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    //TODO when all types are non-nullable, null-forgiving is redundant
}
