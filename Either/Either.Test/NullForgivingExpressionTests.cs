using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class NullForgivingExpressionTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "int", "string" }, new[] { "int", "string" })]
    [InlineData(SwitchType.Stmt, new[] { "int", "bool?" }, new[] { "int", "bool" })]
    [InlineData(SwitchType.Expr, new[] { "int", "string" }, new[] { "int", "string" })]
    [InlineData(SwitchType.Expr, new[] { "int", "bool?" }, new[] { "int", "bool" })]
    public async Task Switch_With_Null_Forgiving_Expr_And_No_Null_Case_Succeeds(SwitchType switchType, string[] typesToCheck, string[] untaggedCasesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, untaggedCasesChecked, [], isNullForgiving: true);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "int", "string" }, new[] { "int", "string" }, new[] { "null" })]
    [InlineData(SwitchType.Stmt, new[] { "int", "bool?" }, new[] { "int", "bool" }, new[] { "null" })]
    [InlineData(SwitchType.Expr, new[] { "int", "string" }, new[] { "int", "string" }, new[] { "null" })]
    [InlineData(SwitchType.Expr, new[] { "int", "bool?" }, new[] { "int", "bool" }, new[] { "null" })]
    public async Task Switch_With_Null_Forgiving_Expr_And_Null_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] untaggedCasesChecked, string[] taggedCasesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, untaggedCasesChecked, taggedCasesChecked, isNullForgiving: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool", "int" })]
    public async Task Switch_With_Null_Forgiving_Expr_And_No_Nullable_Types_Complains(SwitchType switchType, string[] typesToCheck, string[] untaggedCasesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, untaggedCasesChecked, [], isNullForgiving: true, tagExpr: true);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantNullForgivingExprId)
            .WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }
}
