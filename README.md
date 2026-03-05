![sharp or flat](./icon/icon128.png)
# Either

This project enables you to use union types with native C# switch exhaustiveness checking. It does so by providing several types and relevant Roslyn diagnostics around them. Install the [Either.Main](https://www.nuget.org/packages/Either.Main) nuget package to get the full experience.

One of those types is `Either<T1, T2>`. Use it for declaration like so:
```C#
Either<int, bool> x;
```
and you can assign any integer or boolean to `x`:
```C#
x = 5; // this works
x = true; // so does this
```
after which you can use a switch statement(or expression) to find out what exact value it holds:
```C#
switch (x.Thing)
{
  case int i:
    break;
  //deliberately skipped the bool case
}
```
at this point a Roslyn analyzer will kick in and actually break the build informing you that you've forgotten to handle `case bool`.

You can, of course, configure the analyzer to issue a warning instead if that's your preference.

Wait, `x.Thing`?

Yeah, more on that later.

If you want named union types you can do that too like so:
```C#
[NamedUnionType]
public partial struct Shape : IEither<Circle, Rectangle>;
```
a Roslyn code generator will pick this up and generate the implementation of the `IEither` interface as the second part of this partial struct enabling you to use it with a switch the same way:
```C#
var perimeter = shape.Thing switch
{
    Circle c => 2 * Math.PI * c.Radius,
    Rectangle t => 2 * (t.Width + t.Length),
};
```
and if at some point you need to support another possible shape:
```C#
[NamedUnionType]
public partial struct Shape : IEither<Circle, Rectangle, Triangle>;
```
every relevant switch statement/expression will light up in your codebase and make sure you don't miss handling the new case.

That's the gist of how this package functions. [This document](./docs/in-depth.md) goes into more detail.

## The vision for the project
I'm aware that C# is close to getting union types natively, but I still decided to write this project as I think it's a good implementation of the feature.

There's still a number of outstanding bugs/features that need to be addressed until the project can be considered more or less complete.

My aim is to provide an implementation that comfortably covers 80-90% of real-life scenarios. Some minor features and edge cases might fall beyond the scope of this project as I'm a single developer with limited free time and I have to be practical about goals I try to achieve.

## Contributing
(Assuming there's any interest whatsoever)

I welcome bug reports. 

However, I'm not yet ready to accept PRs/code contributions. I'm new to running an open source project and I have next no idea how to do it properly(for now).


