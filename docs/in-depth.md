### Structs? Why?

Short answer: they're not nullable!

In the following example:
```C#
switch (shape.Thing)
{
    // ...
}
```
if shape was a class instance, it would be possible for it to be null, running the risk of a null reference exception. Making it a struct eliminates that risk.

Choosing structs did complicate things in other respects though, but I decided that was the right trade-off to make.

### `Thing`... again, why?

`Thing` is a getter-only property of type `object`. 

Given this:
```C#
Either<int, bool> x;
```
it is possible to write integer and boolean values straight into `x` by leveraging C#'s implicit conversion operators. It is however not as seemlessly possible to retrieve the underlying value out of it, hence the need to write `x.Thing`.

#### Yeah but why name it `Thing`?
If it sounds blunt, that's sort-of intentional. If I had named it `Value`, dealing with something like this:
```C#
Either<int, bool>? x; //notice ? making x nullable
```
would inevitably result in code like `x.Value.Value` which is awesome if you're trolling around, but that's not my intention here.

I eventually decided against your typical `Value`s, `Item`s, and `Entity`s of the world in favor of `Thing` which unambigiously announces exactly what you're working with.

### How type checking/matching works

Considering this example:
```C#
void M(Either<char, string, List<char>> text)
{
    switch (text.Thing)
    {
        case char c:
            break;
        case IEnumerable<char> cs:
            break;
    }
}
```
You probably expect that to work and it would compile and run just fine as `string` and `List<char>` both derive from `IEnumerable<char>` but the analyzer flags this as an error.

When working on this project, I picked the simplest possible, one to one, type checking/matching approach. Which means that if you switch on `Either<T1, T2, T3>` you have to have `case T1`, `case T2`, `case T3`. No base types, no nothing.

The reason?

This is a practical decision that makes analysis dead simple. I expect the scenarios with a common base type to be relatively rare and when they show up, all you have to do is go through the small inconvenience of handling multiple cases instead of one.

For the example above:
```C#
case string s:
    break;
case List<char> l:
    break;
```
instead of:
```C#
case IEnumerable<char> cs:
    break;
```

Same goes for when clauses, they complicate analysis way too much which is why I decided not to support them:
```C#
void M(Either<int, bool> x)
{
    switch (x.Thing)
    {
        case int:
            break;
        case bool when DateTime.Now.DayOfWeek == DayOfWeek.Friday:
            break;
    }
}
```
the when clause here will be flagged as a compiler error as it only sometimes covers the bool case.

### Nullability

It's an involved subject with C# generally and it's an involved subject with this specific project too.

Any time you have a reference type as a part of the union, you'll be asked to handle the null case:
```C#
void M(Either<int, string> x)
{
    switch (x.Thing)
    {
        case int:
            break;
        case string:
            break;
        case null: // string being nullable makes this necessary
            break;
    }
}
```
Same is true for nullable value types:
```C#
Either<int, bool?> x
```

If you're sure that `x.Thing` can never be null in your particular situation, you can use the `!` [trust me bro operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving) to avoid handling `case null`:
```C#
switch (x.Thing!)
{
    // ...
}
```

If the `Either` type you're using itself is nullable, you can use the `?.` null-conditional access operator to switch on it and you'll again be asked to handle the null case:
```C#
void M(Either<int, bool>? x)
{
    switch (x?.Thing) // null-conditional access
    {
        case int:
            break;
        case bool:
            break;
        case null: // neither int nor bool are nullable but x itself is
            break;
    }
}
```

#### How about nullable reference types?

Not yet supported, but on the list of things to do.

### The default case

The `default:` case acts as you'd expect in the switch statement, it removes the need to handle any cases. Same goes for the `_` with switch expressions.

### What else?

Another analyzer that comes with this package reports any redundant cases:
```C#
void M(Either<int, bool> x)
{
    switch (x.Thing)
    {
        case int:
            break;
        case bool:
            break;
        case string: // you'll get a warning here since string is not one of the possible types
            break;
    }
}
```

Same goes for default if all cases are handled:
```C#
void M(Either<int, bool> x)
{
    switch (x.Thing)
    {
        case int:
            break;
        case bool:
            break;
        default: // warning since both possible types are handled, there's no need for default
            break;
    }
}
```

And the trust me bro operator as well:
```C#
void M(Either<int, bool> x)
{
    switch (x.Thing!) // neither int nor bool are nullable making the ! redundant
    {
        // ...
    }
}
```


