using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;

namespace RogueSharpSamples.SadConsoleV8NetCore.Systems
{
    public class MessageLog
    {
        private readonly Queue<string> _lines;

        public MessageLog()
            => _lines = new Queue<string>();

        public void Add(string message)
        {
            _lines.Enqueue(message);

            if (_lines.Count > 9)
            {
                _lines.Dequeue();
            }
        }

        public void Draw(Console console)
        {
            //console.Clear();
            var lines = _lines.ToArray();

            for (var i = 0; i < lines.Count(); i++)
            {
                console.Print(1, i + 1, lines[i], Color.White);
            }
        }
    }
}
