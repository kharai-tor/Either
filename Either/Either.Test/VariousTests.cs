using VerifyCS = RhymesOfUncertainty.Test.CSharpAnalyzerVerifier<RhymesOfUncertainty.EitherAnalyzer>;

namespace RhymesOfUncertainty.Test;

public class VariousTests
{
    [Fact]
    public async Task Switch_Stmt_With_When_Clause_Complains()
    {
        var code = Shared.Types + """
            class C
            {
                void M(Either<int, bool> x)
                {
                    switch (x.Thing)
                    {
                        case int {|#0:when true|}:
                            break;
                        case bool:
                            break;
                    }
                }
            }
            """;

        var expected = VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_When_Clause_Complains()
    {
        var code = Shared.Types + """
            class C
            {
                int M(Either<int, bool> x)
                {
                    return x.Thing switch
                    {
                        int {|#0:when true|} => 0,
                        bool => 0
                    };
                }
            }
            """;

        var expected = VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Expr_With_When_Clause_After_Discard_Complains()
    {
        var code = Shared.Types + """
            class C
            {
                int M(Either<int, bool> x)
                {
                    return x.Thing switch
                    {
                        int => 0,
                        _ {|#0:when true|} => 0
                    };
                }
            }
            """;

        var expected = VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(0);
        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task Switch_Stmt_With_Two_When_Clauses_Complains_Twice()
    {
        var code = Shared.Types + """
            class C
            {
                void M(Either<int, bool> x)
                {
                    switch (x.Thing)
                    {
                        case int {|#0:when true|}:
                            break;
                        case bool {|#1:when 3 > 4|}:
                            break;
                    }
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code, [
            VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(0),
            VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(1),
        ]);
    }

    [Fact]
    public async Task Switch_Expr_With_Two_When_Clauses_Complains_Twice()
    {
        var code = Shared.Types + """
            class C
            {
                int M(Either<int, bool> x)
                {
                    return x.Thing switch
                    {
                        int {|#0:when true|} => 0,
                        _ {|#1:when 3 < 4|} => 0
                    };
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code, [
            VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(0),
            VerifyCS.Diagnostic(EitherAnalyzer.UnsupportedWhenClauseId).WithLocation(1),
        ]);
    }
}
