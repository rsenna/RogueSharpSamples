﻿using Microsoft.Xna.Framework;
using RogueSharp;
using SadConsole.Consoles;

namespace RogueSharpSamples.LegacySadConsole.Interfaces
{
    public interface IDrawable
    {
        Color Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Draw(Console console, IMap map);
    }
}
