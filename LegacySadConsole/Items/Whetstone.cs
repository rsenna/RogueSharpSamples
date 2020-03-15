using RogueSharp.DiceNotation;
using RogueSharpSamples.LegacySadConsole.Core;
using RogueSharpSamples.LegacySadConsole.Equipment;

namespace RogueSharpSamples.LegacySadConsole.Items
{
    public class Whetstone : Item
    {
        public Whetstone()
        {
            Name = "Whetstone";
            RemainingUses = 5;
        }

        protected override bool UseItem()
        {
            var player = RogueGame.Player;

            if (player.Hand == HandEquipment.None())
            {
                RogueGame.MessageLog.Add($"{player.Name} is not holding anything they can sharpen");
            }
            else if (player.AttackChance >= 80)
            {
                RogueGame.MessageLog.Add($"{player.Name} cannot make their {player.Hand.Name} any sharper");
            }
            else
            {
                RogueGame.MessageLog.Add($"{player.Name} uses a {Name} to sharper their {player.Hand.Name}");
                player.Hand.AttackChance += Dice.Roll("1D3");
                RemainingUses--;
            }

            return true;
        }
    }
}
