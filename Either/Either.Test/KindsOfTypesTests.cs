using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class KindsOfTypesTests
{
    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "List<int>", "Dictionary<bool, double>", "Either<float, HashSet<decimal>>" }, new[] { "List<int>", "Dictionary<bool, double>|Either<float, HashSet<decimal>>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "List<int>", "Dictionary<bool, double>", "Either<float, HashSet<decimal>>" }, new[] { "List<int>", "Dictionary<bool, double>", "Either<float, HashSet<decimal>>", "null" })]
    public async Task Switch_With_Matching_Generic_Types_Succeeds(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { Usings = ["System.Collections.Generic"] });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "int", "List<int>" }, new[] { "int", "t:List<bool>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "int", "List<int>" }, new[] { "int", "t:List<bool>", "null" })]
    public async Task Switch_With_Mismatching_Generic_Type_Fails(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true, Usings = ["System.Collections.Generic"] });

        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "List<int>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("List<bool>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "int", "Dictionary<int, string>" }, new[] { "int", "t:Dictionary<int, decimal>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "int", "Dictionary<int, string>" }, new[] { "int", "t:Dictionary<int, decimal>", "null" })]
    public async Task Switch_With_Partially_Mismatching_Generic_Type_Fails(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true, Usings = ["System.Collections.Generic"] });

        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "Dictionary<int, string>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("Dictionary<int, decimal>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "int", "HashSet<List<int>>" }, new[] { "int", "t:HashSet<List<decimal>>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "int", "HashSet<List<int>>" }, new[] { "int", "t:HashSet<List<decimal>>", "null" })]
    public async Task Switch_With_Partially_Mismatching_Nested_Generic_Type_Fails(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true, Usings = ["System.Collections.Generic"] });

        var expectedNotExhaustive = VerifyCS.Diagnostic(EitherAnalyzer.NotExhaustiveId)
            .WithLocation(0)
            .WithArguments("Case", "HashSet<List<int>>", "is");

        var expectedRedundantCase = VerifyCS.Diagnostic(EitherAnalyzer.RedundantCaseId)
            .WithLocation(1)
            .WithArguments("HashSet<List<decimal>>");

        await VerifyCS.VerifyAnalyzerAsync(code, expectedNotExhaustive, expectedRedundantCase);
    }

    [Theory]
    [InlineData(SwitchType.Stmt, new[] { "Action", "Func<string>" }, new[] { "Action", "Func<string>", "null" })]
    [InlineData(SwitchType.Expr, new[] { "Action", "Func<string>" }, new[] { "Action", "Func<string>", "null" })]
    public async Task Switch_With_Matching_Delegate_Types_Succeeds(SwitchType switchType, string[] typesToCheck, string[] casesChecked)
    {
        var code = Shared.GenerateSwitch(switchType, typesToCheck, casesChecked, new() { TagSwitch = true, Usings = ["System"] });
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    //    [Fact]
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
