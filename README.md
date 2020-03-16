# Purpose

This repository _started_ as a simple fork of https://github.com/FaronBracy/RogueSharpSadConsoleSamples.
Then I added some refactors. Then I tried to upgrade both RogueSharp and SadConsole.

RogueSharp was easy, hardly any breaking changes. But SadConsole is a whole another story. Original source code was using SadConsole.Core v2.5. In order to move to SadConsole v8 I had to make a lot of changes (still WIP).

I decided to keep both SadConsole versions - Sample1 is the original SadConsole legacy version, and Sample2 is the new one. Keeping them both shows how the SadConsole API has changed along time, and I think there's value in being able to easily compare between those two versions.

I also added a new Sample3, from [The SadConsole Roguelike Tutorial Series](https://ansiware.com/), which uses GoRogue instead of RogueSharp.

##### TODO
- [ ] Fix Sample2.
- [ ] Complete Sample3.

## Additional Resources

- [Creating a Roguelike Game in C#](http://roguesharp.wordpress.com/ "Creating a Roguelike Game in C#")
- [SadConsole Tutorials and Source](https://github.com/Thraka/SadConsole/wiki "SadConsole Tutorials")
- [The SadConsole Roguelike Tutorial Series](https://ansiware.com/)
- [RogueSharp](https://github.com/FaronBracy/RogueSharp)
- [GoRogue](https://github.com/Chris3606/GoRogue)
- [RogueBasin](http://www.roguebasin.com/ "RogueBasin")
	- [Articles of interest for Roguelike developers](http://www.roguebasin.com/index.php?title=Articles "Roguelike developer articles")
- [Roguelike Dev Subreddit](https://www.reddit.com/r/roguelikedev)

## License

#### Roguelike C# Samples ####

The MIT License (MIT)

Copyright for portions of this project are held by Faron Bracy (2016) and
ansi|ware (2019). All other copyright are held by Rogério Senna, 2020.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
