using RogueSharpSample2.Core;
using RogueSharpSample2.Systems;

namespace RogueSharpSample2.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
