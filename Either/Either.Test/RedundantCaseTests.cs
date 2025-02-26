using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class RedundantCaseTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int", "t:string s" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int", "t:string s" })]
    public async Task Switch_With_A_Redundant_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int", "t:null" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int", "t:null" })]
    public async Task Switch_With_A_Redundant_Null_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int", "t:string s", "t:decimal", "t:null" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int", "t:string s", "t:decimal", "t:null" })]
    public async Task Switch_With_Multiple_Redundant_Cases_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);

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

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int", "t:default" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int", "t:_" })]
    public async Task Switch_With_Both_Cases_Handled_And_A_Redundant_Default_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("Both");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int", "decimal" }, new[] { "bool b", "int", "decimal d", "t:default" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int", "decimal" }, new[] { "bool b", "int", "decimal d", "t:_" })]
    public async Task Switch_With_All_Cases_Handled_And_A_Redundant_Default_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("All");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool b", "int", "t:decimal d", "t:default" })]
    [InlineData(SwitchType.Expr, new[] { "bool", "int" }, new[] { "bool b", "int", "t:decimal d", "t:_" })]
    public async Task Switch_With_A_Redundant_Regular_Case_And_A_Redundant_Default_Case_Complains(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked);

        var expectedRegular = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("decimal");

        var expectedDefault = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(1)
            .WithArguments("Both");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedRegular, expectedDefault);
    }
}
