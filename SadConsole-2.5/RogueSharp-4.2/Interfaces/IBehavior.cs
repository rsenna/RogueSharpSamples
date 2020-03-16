using RogueSharpSample1.Core;
using RogueSharpSample1.Systems;

namespace RogueSharpSample1.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
