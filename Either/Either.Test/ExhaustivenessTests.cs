using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class ExhaustivenessTests
{
    [TestMethod]
    public async Task Given_A_Switch_Statement_With_An_Unhandled_Type_Complain()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int> x)
    {
        {|#0:switch|} (x.Value)
        {
            case bool b:
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool b => 1"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_An_Unhandled_Type_Complain_2()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int> x)
    {
        {|#0:switch|} (x.Value)
        {
            case bool:
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_An_Unhandled_Type_Complains_2()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool => 1"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "int", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_Two_Unhandled_Types_Complain()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int> x)
    {
        {|#0:switch|} (x.Value)
        {
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Two_Unhandled_Types_Complains()
    {
        var code = GenerateSwitchExpr(["bool", "int"], []);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Cases", "bool, int", "are");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_A_Class_Require_Null_Case_To_Be_Handled()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<string, int> x)
    {
        {|#0:switch|} (x.Value)
        {
            case int:
                break;
            case string s:
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Class_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["string", "int"], ["string => 1", "int => 2"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_A_Nullable_Value_Type_Require_Null_Case_To_Be_Handled()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int?> x)
    {
        {|#0:switch|} (x.Value)
        {
            case bool b:
                break;
            case int:
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Switch_Expr_With_A_Nullable_Value_Type_Requires_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["bool", "int?"], ["bool b => 1", "int => 2"]);
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "null", "is");
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_Only_Value_Types_Dont_Require_Null_To_Be_Handled()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int> x)
    {
        switch (x.Value)
        {
            case bool b:
                break;
            case int:
                break;
        }
    }
}
";
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Value_Types_Does_Not_Require_Null_Case_Handling()
    {
        var code = GenerateSwitchExpr(["bool", "int"], ["bool b => 1", "int => 2"]);
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_All_Cases_Handled_Succeed()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<string, int, List<bool>> x)
    {
        switch (x.Value)
        {
            case string:
            case int:
                break;
            case List<bool>:
               break;
            case null:
                break;
        }
    }
}
";
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_All_Cases_Handled_Succeeds()
    {
        var code = GenerateSwitchExpr(
            ["string", "int", "List<bool>"],
            ["string => 1", "int => 2", "List<bool> => 3", "null => 4"],
            ["System.Collections.Generic"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Given_A_Switch_Statement_With_No_Cases_Handled_But_A_Default_Case_Succeed()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<string, int, bool> x)
    {
        switch (x.Value)
        {
            default:
                break;
        }
    }
}
";
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Switch_Expr_With_Only_Default_Case_Succeeds()
    {
        var code = GenerateSwitchExpr(
            ["string", "int", "bool"],
            ["_ => 1"]
        );
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    private string GenerateSwitchExpr(string[] typesToCheck, string[] casesChecked, string[] usings = default)
    {
        var code =
            string.Join("\n", (usings ?? []).Select(u => $"using {u};")) +
            "\n" +
            Shared.Structs +
@$"
class C
{{
    int M(Either<{string.Join(", ", typesToCheck)}> x)
    {{
        return x.Value {{|#0:switch|}}
        {{
            {string.Join(",\n", casesChecked)}
        }};
    }}
}}
";
        return code;
    }
}
