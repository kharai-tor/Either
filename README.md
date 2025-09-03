# Either

Discriminated unions/Union types with native C# switch statement/expression exhaustiveness support.

Two nuget packages get built from this repo: [Either.Main](https://www.nuget.org/packages/Either.Main) and [Either.Analyzers](https://www.nuget.org/packages/Either.Analyzers).

The former exposes the types: `Either<T1, T2>`, `Either<T1, T2, T3>` and so on(up to `T5` for now) and the latter comes with Roslyn analyzers and code fixes that, among other things, check exhaustive usage with switch statements and expressions.

You don't need to install the two packages separately, `Either.Main` references the analyzers package so installing that one will give you the full experience.

### Still under development!
The codebase is by no means stable, there are bugs to be fixed and some features need to fully be fleshed out, hence the 0.0.x version of nuget packages. I hope to get all of it done by 1.0.0

### How to use
```C#
Either<int, bool> x = 5;
```
any value of type `int`, `bool` or whatever is listed as a type parameter for `Either<T1, ...>` can be assigned to `x`.

Afterwards you can switch on it like this:

```C#
switch (x.Thing)
{
  case int:
    break;
}
```

at this point the analyzer will kick in and complain that not all cases are being handled, namely `case bool` not being covered in this particular... well, case.

You get a similar experience with switch expressions and that's the gist of the whole thing. There are details however a lot of them:

`x.Thing` is a getter only property of type `object` that is the actual holder of whatever value you assign to `x`. The only meaningful way to read from it is via `switch`.

Why `Thing`?

I initially named it `Value` later realizing that when dealing with the nullable version - `Either<T1, ...>?`, you'd have code like `x.Value.Value` which is awesome if you're trolling around but that's not what I'm after in this case. The next candidate was `Item` but I eventually went with `Thing` because it sounds unambigious, almost blunt, announcing itself to the reader.

This might have clued you in on the next important point, `Either<T1, ...>` is a struct not a class. The reason for this is that I didn't want users to have to handle `case null` if they are using it with non-nullable type parameters such as primitive types, enums, other structs and so on... `Either<int, bool>` above being a good example. Relying on a struct is the only way I could absolutely make sure `x` can never be null.

In contrast, if you use it with a nullable type:
```C#
Either<int, bool, string> x = 5;

switch (x.Thing)
{
  case int:
    break;
  case bool:
    break;
  case string:
    break;
}
```
even with all types covered, the switch will still complain about the missing possible `case null`. If you're sure that `Thing` can't be null in your particular case, you can bypass having to handle it by using `switch (x.Thing!)` at which point the analyzer will hand the responsibility back to you, as it generally does with the `!` operator.

Like with everything, `null` complicates things. I'm planning to write a more comprehensive document that touches on all the edge cases a bit later.

If you only want to handle a subset of types and don't care about the rest, you can use the `default` case.

### What else?
One of the analyzers also checks redundant cases. Like so:
```C#
Either<int, bool> x = 5;

switch (x.Thing)
{
  case int:
    break;
  case bool:
    break;
  case string:
    break;
}
```
here you'll get a warning letting you know that `case string` should be removed. Same goes for `default`, if all possible cases are covered and you still have a `default` the analyzer will consider that to be redundant and let you know.

### What's missing
I have a list of bugs, todos, planned features. I'll turn them into github issues in the near future.

### Contributing
Assuming there's any interest for this whatsoever, I welcome bug reports, ideas as to how to solve certain issues. PRs/code contributions however I'm not ready to accept at this stage, I'm new to running an open source project and I have next no idea how to do it properly(for now).
