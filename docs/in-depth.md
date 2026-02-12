### Structs? Why?

Short answer: they're not nullable!

In the following example:
```C#
switch (shape.Thing)
{
    //
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