using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using VerifyCS = RhymesOfUncertainty.Test.CSharpCodeFixVerifier<RhymesOfUncertainty.EitherAnalyzer, RhymesOfUncertainty.RedundantCaseCodeFixProvider>;

namespace RhymesOfUncertainty.Test;

public class RedundantCaseFixerTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int" }, new[] { "string" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int" }, new[] { "string", "decimal" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int" }, new[] { "string s" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int" }, new[] { "string", "decimal d" })]
    public async Task Fixer_Removes_Redundant_Casees_From_Switch(SwitchType switchType, string[] typesToCheck, string[] untaggedCasesChecked, string[] taggedCasesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, untaggedCasesChecked, taggedCasesChecked);
        var diagnostics = new List<DiagnosticResult>
        {
            VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
                .WithLocation(0)
                .WithArguments("string")
        };
        if (taggedCasesChecked.Length == 2)
        {
            diagnostics.Add(VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
                .WithLocation(1)
                .WithArguments("decimal"));
        }
        var fixedCode = Shared.GenerateSwitch(switchType, typesToCheck, untaggedCasesChecked, []);
        await VerifyCS.VerifyCodeFixAsync(code, diagnostics.ToArray(), fixedCode);
    }
}