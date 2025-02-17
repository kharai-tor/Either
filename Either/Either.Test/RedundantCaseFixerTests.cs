using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Linq;
using VerifyCS = RhymesOfUncertainty.Test.CSharpCodeFixVerifier<RhymesOfUncertainty.EitherAnalyzer, RhymesOfUncertainty.RedundantCaseCodeFixProvider>;

namespace RhymesOfUncertainty.Test;

public class RedundantCaseFixerTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int", "t:string" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "t:string", "int" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "t:string", "bool", "int" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int", "t:string", "t:decimal" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "t:string", "int", "t:decimal" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int", "t:string s" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int", "t:string", "t:decimal d" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int|t:string" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "t:string|int" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int", "decimal" }, new[] { "bool", "int|t:string|decimal" }, new[] { "bool", "int|decimal" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int|t:string|t:decimal" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int|t:string s" }, new[] { "bool", "int" })]
    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int|t:string s|t:decimal" }, new[] { "bool", "int" })]

    [InlineData(SwitchType.Stmt, new[] { "bool", "int" }, new[] { "bool", "int", "t:string s|t:decimal" }, new[] { "bool", "int" })]
    public async Task Fixer_Removes_Redundant_Casees_From_Switch(SwitchType switchType, string[] typesToCheck, string[] casesBeforeFix, string[] casesAfterFix)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesBeforeFix);
        var diagnostics = new List<DiagnosticResult>
        {
            VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
                .WithLocation(0)
                .WithArguments("string")
        };
        if (casesBeforeFix.Any(@case => @case.Contains("t:decimal")))
        {
            diagnostics.Add(VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
                .WithLocation(1)
                .WithArguments("decimal"));
        }
        var fixedCode = Shared.GenerateSwitch(switchType, typesToCheck, casesAfterFix);
        await VerifyCS.VerifyCodeFixAsync(code, diagnostics.ToArray(), fixedCode);
    }
}