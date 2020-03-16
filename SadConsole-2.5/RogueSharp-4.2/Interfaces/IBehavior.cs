using RogueSharpSamples.LegacySadConsole.Core;
using RogueSharpSamples.LegacySadConsole.Systems;

namespace RogueSharpSamples.LegacySadConsole.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
