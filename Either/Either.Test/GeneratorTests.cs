using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace RhymesOfUncertainty.Test;

public class GeneratorTests
{
    [Fact]
    public async Task Generator_With_2_Concrete_Type_Arguments_Works()
    {
        var context = new CSharpSourceGeneratorTest<NamedUnionGenerator, DefaultVerifier>();

        context.TestCode = """
            using System;
            using RhymesOfUncertainty;

            namespace RhymesOfUncertainty
            {
                [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
                public class NamedUnionTypeAttribute : Attribute;
            
                public interface IEither<T1, T2>;
            }

            namespace Consumer
            {
                [NamedUnionType]
                public partial struct IntOrBool : IEither<int, bool>;
            }
            """;

        context.TestState.GeneratedSources.Add((typeof(NamedUnionGenerator), "IntOrBool.g.cs", NamedUnionGenerator.GenerateCode("Consumer", "IntOrBool", ["int", "bool"])));

        await context.RunAsync();
    }
}
