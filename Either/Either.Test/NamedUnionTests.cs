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
}
