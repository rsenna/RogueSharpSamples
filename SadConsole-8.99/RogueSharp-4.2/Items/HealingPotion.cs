using RogueSharpSample2.Abilities;
using RogueSharpSample2.Core;

namespace RogueSharpSample2.Items
{
    public class HealingPotion : Item
    {
        public HealingPotion()
        {
            Name = "Healing Potion";
            RemainingUses = 1;
        }

        protected override bool UseItem()
        {
            var healAmount = 15;
            RogueGame.MessageLog.Add($"{RogueGame.Player.Name} consumes a {Name} and recovers {healAmount} health");

            var heal = new Heal(healAmount);

            RemainingUses--;

            return heal.Perform();
        }
    }
}
