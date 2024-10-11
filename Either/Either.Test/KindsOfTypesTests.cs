using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

[TestClass]
public class KindsOfTypesTests
{
    [TestMethod]
    public async Task Given_Switch_Statement_With_Matching_Generic_Types_Succeed()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<List<int>, Dictionary<bool, double>, Either<float, HashSet<decimal>>> x)
    {
        switch (x.Value)
        {
            case List<int>:
                break;
            case Dictionary<bool, double>:
            case Either<float, HashSet<decimal>>:
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
    public async Task Given_Switch_Statement_With_Mismatching_Generic_Type_Fail()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<int, List<int>> x)
    {
        {|#0:switch|} (x.Value)
        {
            case int:
                break;
            {|#1:case List<bool>:|}
                break;
            case null:
                break;
        }
    }
}
";
        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "List<int>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("List<bool>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_Partially_Mismatching_Generic_Type_Fail()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<int, Dictionary<int, string>> x)
    {
        {|#0:switch|} (x.Value)
        {
            case int:
                break;
            {|#1:case Dictionary<int, decimal>:|}
                break;
            case null:
                break;
        }
    }
}
";
        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "Dictionary<int, string>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("Dictionary<int, decimal>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);
    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_Partially_Mismatching_Nested_Generic_Type_Fail()
    {
        var code = "using System.Collections.Generic;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<int, HashSet<List<int>>> x)
    {
        {|#0:switch|} (x.Value)
        {
            case int:
                break;
            {|#1:case HashSet<List<decimal>>:|}
                break;
            case null:
                break;
        }
    }
}
";
        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "HashSet<List<int>>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("HashSet<List<decimal>>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);

    }

    [TestMethod]
    public async Task Given_Switch_Statement_With_Matching_Delegate_Types_Succeed()
    {
        var code = "using System;"
            + Shared.Structs
            + @"
class C
{
    void M(Either<Action, Func<string>> x)
    {
        switch (x.Value)
        {
            case Action:
                break;
            case Func<string>:
                break;
            case null:
                break;
        }
    }
}
";
        await VerifyCS.VerifyAnalyzerAsync(code);

    }

//    [TestMethod]
//    public async Task Given_Switch_Statement_With_Matching_Tuple_Types_Succeed()//problematic
//    {
//        var code = "using System.Collections.Generic;"
//            + Shared.Structs
//            + @"
//class C
//{
//    void M(Either<(int, string), (string, decimal?)> x)
//    {
//        switch (x.Value)
//        {
//            case (int, string):
//                break;
//            case (string s, decimal? d):
//                break;
//            //case Person { Age: var a }:
//            //    break;
//        }
//    }
//}

////public class Person
////{
////    public int Age { get; set; }
////}

//";
//        await VerifyCS.VerifyAnalyzerAsync(code);
//    }


//    [TestMethod]
//    public async Task list_of_tuples() //problematic
//    {
//        var code = "using System.Collections.Generic;"
//            + Shared.Structs
//            + @"
//class C
//{
//    void M(Either<List<(int, string)>, int> x)
//    {
//        switch (x.Value)
//        {
//            case int:
//                break;
//            case List<(int a, string b)>: //this fails!!!!
//                break;
//            case null:
//                break;
//        }
//    }
//}

//";
//        await VerifyCS.VerifyAnalyzerAsync(code);
//    }
}
