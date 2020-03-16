using RogueSharpSamples.SadConsoleV8NetCore.Core;

namespace RogueSharpSamples.SadConsoleV8NetCore.Items
{
    public class TeleportScroll : Item
    {
        public TeleportScroll()
        {
            Name = "Teleport Scroll";
            RemainingUses = 1;
        }

        protected override bool UseItem()
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;

            RogueGame.MessageLog.Add($"{player.Name} uses a {Name} and reappears in another place");

            var point = map.GetRandomLocation();

            map.SetActorPosition(player, point.X, point.Y);

            RemainingUses--;

            return true;
        }
    }
}
