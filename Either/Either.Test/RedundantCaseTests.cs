using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Either.Test.CSharpAnalyzerVerifier<Either.EitherAnalyzer>;

namespace Either.Test;

[TestClass]
public class RedundantCaseTests
{
    [TestMethod]
    public async Task Given_Switch_Statement_With_A_Redundant_Case_Warn()
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
            {|#0:case string s:|}
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_A_Redundant_Null_Case_Warn()
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

    [TestMethod]
    public async Task Given_Switch_Statement_With_Multiple_Redundant_Cases_Warn_On_All()
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
            {|#0:case string s:|}
            {|#1:case decimal:|}
                break;
            {|#2:case null:|}
                break;
        }
    }
}
";
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

    [TestMethod]
    public async Task Given_Switch_Statement_With_Both_Cases_Handled_And_A_Redundant_Default_Case_Warn()
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
            {|#0:default:|}
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("Both");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_All_Cases_Handled_And_A_Redundant_Default_Case_Warn()
    {
        var code = Shared.Structs + @"
class C
{
    void M(Either<bool, int, decimal> x)
    {
        switch (x.Value)
        {
            case bool b:
                break;
            case int:
            case decimal d:
                break;
            {|#0:default:|}
                break;
        }
    }
}
";
        var expected = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(0)
            .WithArguments("All");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_A_Redundant_Regular_Case_And_A_Redundant_Default_Case_Warn_On_Both()
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
            {|#0:case decimal d:|}
                break;
            {|#1:default:|}
                break;
        }
    }
}
";
        var expectedRegular = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(0)
            .WithArguments("decimal");

        var expectedDefault = VerifyCS.Diagnostic(EitherAnalyzer.RedundantDefaultId)
            .WithLocation(1)
            .WithArguments("Both");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedRegular, expectedDefault);
    }
}
