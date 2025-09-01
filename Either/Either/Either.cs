namespace RhymesOfUncertainty;

public struct Either<T1, T2>
{
    public object Thing { get; private set; }

    public static implicit operator Either<T1, T2>(T1 t1)
    {
        return new Either<T1, T2> { Thing = t1 };
    }

    public static implicit operator Either<T1, T2>(T2 t2)
    {
        return new Either<T1, T2> { Thing = t2 };
    }
}

public struct Either<T1, T2, T3>
{
    public object Thing { get; private set; }

    public static implicit operator Either<T1, T2, T3>(T1 t1)
    {
        return new Either<T1, T2, T3> { Thing = t1 };
    }

    public static implicit operator Either<T1, T2, T3>(T2 t2)
    {
        return new Either<T1, T2, T3> { Thing = t2 };
    }

    public static implicit operator Either<T1, T2, T3>(T3 t3)
    {
        return new Either<T1, T2, T3> { Thing = t3 };
    }
}

public struct Either<T1, T2, T3, T4>
{
    public object Thing { get; private set; }

    public static implicit operator Either<T1, T2, T3, T4>(T1 t1)
    {
        return new Either<T1, T2, T3, T4> { Thing = t1 };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T2 t2)
    {
        return new Either<T1, T2, T3, T4> { Thing = t2 };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T3 t3)
    {
        return new Either<T1, T2, T3, T4> { Thing = t3 };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T4 t4)
    {
        return new Either<T1, T2, T3, T4> { Thing = t4 };
    }
}

public struct Either<T1, T2, T3, T4, T5>
{
    public object Thing { get; private set; }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T1 t1)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t1 };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T2 t2)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t2 };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T3 t3)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t3 };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T4 t4)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t4 };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T5 t5)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t5 };
    }
}
