using RogueSharpSample1.Core;

namespace RogueSharpSample1.Items
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
