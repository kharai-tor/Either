using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class NullableUnionTests
{
    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_Conditional_Access_And_No_Null_Case_Complains(SwitchType switchType)
    {
        var code = Shared.GenerateSwitch(switchType, ["int", "bool"], ["int", "bool"], new() { TagSwitch = true, IsUnionTypeNullable = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt)]
    [InlineData(SwitchType.Expr)]
    public async Task Switch_With_Conditional_Access_And_Null_Case_Succeeds(SwitchType switchType)
    {
        var code = Shared.GenerateSwitch(switchType, ["int", "bool"], ["int", "bool", "null"], new() { TagSwitch = true, IsUnionTypeNullable = true });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }
}
