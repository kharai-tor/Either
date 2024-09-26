namespace Either;

readonly struct Either<T1, T2>
{
    public object Value { get; private init; }

    public static implicit operator Either<T1, T2>(T1 t1)
    {
        return new Either<T1, T2> { Value = t1 };
    }

    public static implicit operator Either<T1, T2>(T2 t2)
    {
        return new Either<T1, T2> { Value = t2 };
    }
}

readonly struct Either<T1, T2, T3>
{
    public object Value { get; private init; }

    public static implicit operator Either<T1, T2, T3>(T1 v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }

    public static implicit operator Either<T1, T2, T3>(T2 v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }

    public static implicit operator Either<T1, T2, T3>(T3 v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }

    public static implicit operator Either<T1, T2, T3>(Either<T1, T2> v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }

    public static implicit operator Either<T1, T2, T3>(Either<T1, T3> v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }

    public static implicit operator Either<T1, T2, T3>(Either<T2, T3> v)
    {
        return new Either<T1, T2, T3> { Value = v };
    }
}
