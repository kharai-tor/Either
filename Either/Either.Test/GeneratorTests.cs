using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RhymesOfUncertainty.Test;

public class GeneratorTests
{
    private static readonly string Prefix = """
        using System;
        using RhymesOfUncertainty;
        
        namespace RhymesOfUncertainty
        {
            [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
            public class NamedUnionTypeAttribute : Attribute;
        
            public interface IEither<T1, T2>;
        }
        """;

    [Fact]
    public async Task Generator_With_2_Concrete_Type_Arguments_Works()
    {
        var context = new CSharpSourceGeneratorTest<NamedUnionGenerator, DefaultVerifier>();

        context.TestCode = Prefix + """
            namespace Consumer
            {
                [NamedUnionType]
                public partial struct IntOrBool : IEither<int, bool>;
            }
            """;

        context.TestState.GeneratedSources.Add((typeof(NamedUnionGenerator), "IntOrBool.g.cs", NamedUnionGenerator.GenerateCode("Consumer", "IntOrBool", ["int", "bool"])));

        await context.RunAsync();
    }

    [Fact]
    public async Task Generator_With_Generic_Type_Arguments_Works()
    {
        var context = new CSharpSourceGeneratorTest<NamedUnionGenerator, DefaultVerifier>();

        context.TestCode = Prefix + """
            namespace Consumer
            {
                [NamedUnionType]
                public partial struct Result<T> : IEither<T, Exception>;
            }
            """;

        context.TestState.GeneratedSources.Add((typeof(NamedUnionGenerator), "Result.g.cs", NamedUnionGenerator.GenerateCode("Consumer", "Result<T>", ["T", "System.Exception"])));

        await context.RunAsync();
    }
}
