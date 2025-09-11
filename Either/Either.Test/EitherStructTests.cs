namespace RhymesOfUncertainty.Test;

public class EitherStructTests
{
    private Either<int, bool> _z;

    private Either<int, bool> Z { get; }

    [Fact]
    public void Either_Struct_With_Non_Nullable_Type_Args_Will_Never_Hold_A_Null_Value()
    {
        var x = new Either<int, bool>();
        Assert.NotNull(x.Thing);

        var y = default(Either<int, bool>);
        Assert.NotNull(y.Thing);

        Assert.NotNull(_z.Thing);

        Assert.NotNull(Z.Thing);
    }
}