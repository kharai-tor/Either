using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class ExhaustivenessTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool" })]
    public async Task Switch_With_An_Unhandled_Type_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" })]
    public async Task Switch_With_Two_Unhandled_Types_Complains(SwitchType switchType, string[] typesToCheck)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, [], new() { TagSwitch = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "string", "int" }, new[] { "string s", "int" })]
    [InlineData(SwitchType.Expr, new[] { "string", "int" }, new[] { "string s", "int" })]
    public async Task Switch_With_A_Class_Requires_Null_Case_Handling(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int?" }, new[] { "bool b", "int" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int?" }, new[] { "bool b", "int" })]
    public async Task Switch_With_A_Nullable_Value_Type_Requires_Null_Case_Handling(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true });
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int" })]
    public async Task Switch_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "string", "int", "List<bool>" }, new[] { "string|int", "List<bool>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "string", "int", "List<bool>" }, new[] { "string", "int", "List<bool>", "null" })]
    public async Task Switch_With_All_Cases_Handled_Succeeds(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new()
        {
            TagSwitch = true,
            Usings = ["System.Collections.Generic"]
        });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "string", "int", "bool" }, new[] { "default" })]
    [InlineData(SwitchType.Expr, new[] { "string", "int", "bool" }, new[] { "_" })]
    public async Task Switch_With_Only_Default_Case_Succeeds(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }
}
