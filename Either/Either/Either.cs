namespace RhymesOfUncertainty;

public struct Either<T1, T2>
{
    private bool _initialized;
    private object _thing;
    public object Thing 
    { 
        get
        {
            return _initialized ? _thing : default(T1);
        }
        private set
        {
            _initialized = true;
            _thing = value;
        }
    }

    public static implicit operator Either<T1, T2>(T1 t)
    {
        return new Either<T1, T2> { Thing = t };
    }

    public static implicit operator Either<T1, T2>(T2 t)
    {
        return new Either<T1, T2> { Thing = t };
    }
}

public struct Either<T1, T2, T3>
{
    private bool _initialized;
    private object _thing;
    public object Thing 
    { 
        get
        {
            return _initialized ? _thing : default(T1);
        }
        private set
        {
            _initialized = true;
            _thing = value;
        }
    }

    public static implicit operator Either<T1, T2, T3>(T1 t)
    {
        return new Either<T1, T2, T3> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3>(T2 t)
    {
        return new Either<T1, T2, T3> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3>(T3 t)
    {
        return new Either<T1, T2, T3> { Thing = t };
    }
}

public struct Either<T1, T2, T3, T4>
{
    private bool _initialized;
    private object _thing;
    public object Thing 
    { 
        get
        {
            return _initialized ? _thing : default(T1);
        }
        private set
        {
            _initialized = true;
            _thing = value;
        }
    }

    public static implicit operator Either<T1, T2, T3, T4>(T1 t)
    {
        return new Either<T1, T2, T3, T4> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T2 t)
    {
        return new Either<T1, T2, T3, T4> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T3 t)
    {
        return new Either<T1, T2, T3, T4> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4>(T4 t)
    {
        return new Either<T1, T2, T3, T4> { Thing = t };
    }
}

public struct Either<T1, T2, T3, T4, T5>
{
    private bool _initialized;
    private object _thing;
    public object Thing 
    { 
        get
        {
            return _initialized ? _thing : default(T1);
        }
        private set
        {
            _initialized = true;
            _thing = value;
        }
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T1 t)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T2 t)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T3 t)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T4 t)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t };
    }

    public static implicit operator Either<T1, T2, T3, T4, T5>(T5 t)
    {
        return new Either<T1, T2, T3, T4, T5> { Thing = t };
    }
}
