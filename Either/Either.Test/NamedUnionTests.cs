using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class NamedUnionTests
{
    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_An_Unhandled_Named_Union_Type_Complains(SwitchType switchType)
    {
        var code = Shared.GenerateSwitchForANamedUnion(switchType, "IntOrBool", ["int"], new() { TagSwitch = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "bool", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_An_Unhandled_Generic_Named_Union_Type_Complains(SwitchType switchType)
    {
        var code = Shared.GenerateSwitchForANamedUnion(switchType, "Result<int>", ["int", "null"], new() { TagSwitch = true, Usings = ["System"] });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "Exception", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_A_Named_Union_Type_And_Null_Forgiving_Expr_And_No_Null_Case_Succeeds(SwitchType switchType)
    {
        var code = Shared.GenerateSwitchForANamedUnion(switchType, "IntOrString", ["int", "string"], new() { IsNullForgiving = true });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_A_Named_Union_Type_And_Null_Forgiving_Expr_And_Null_Case_Complains(SwitchType switchType)
    {
        var code = Shared.GenerateSwitchForANamedUnion(switchType, "IntOrString", ["int", "string", "t:null"], new() { IsNullForgiving = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }


    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_A_Named_Union_Type_And_Null_Forgiving_Expr_And_No_Nullable_Types_Complains(SwitchType switchType)
    {
        var code = Shared.GenerateSwitchForANamedUnion(switchType, "IntOrBool", ["int", "bool"], new() { IsNullForgiving = true, TagExpr = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantNullForgivingExprId)
            .WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }
}
