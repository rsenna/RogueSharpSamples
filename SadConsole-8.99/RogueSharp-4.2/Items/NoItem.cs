using RogueSharpSamples.SadConsoleV8NetCore.Core;

namespace RogueSharpSamples.SadConsoleV8NetCore.Items
{
    public class NoItem : Item
    {
        public NoItem()
        {
            Name = "None";
            RemainingUses = 1;
        }

        protected override bool UseItem()
        {
            return false;
        }
    }
}
