using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class NullForgivingExpressionTests
{
    [TestMethod]
    public async Task Given_Switch_Statement_With_Null_Forgiving_Expr_Dont_Require_Null_Case_To_Be_Handled()
    {
        var code = Shared.Structs
            + @"
class C
{
    void M(Either<int, string> x)
    {
        switch (x.Value!)
        {
            case int:
                break;
            case string:
                break;
        }
    }
}
";
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_Null_Forgiving_Expr_And_Null_Case_Warn()
    {
        var code = Shared.Structs
            + @"
class C
{
    void M(Either<int, string> x)
    {
        switch (x.Value!)
        {
            case int:
                break;
            case string:
                break;
            {|#0:case null:|}
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("null");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    //TODO when all types are non-nullable, null-forgiving is redundant
}
