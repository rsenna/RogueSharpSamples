using RogueSharp;
using RogueSharpSamples.SadConsoleV8NetCore.Core;
using RogueSharpSamples.SadConsoleV8NetCore.Interfaces;
using RogueSharpSamples.SadConsoleV8NetCore.Systems;

namespace RogueSharpSamples.SadConsoleV8NetCore.Behaviors
{
    public class ShoutForHelp : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            var didShoutForHelp = false;
            var dungeonMap = RogueGame.DungeonMap;
            var monsterFov = new FieldOfView(dungeonMap);

            monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
            foreach (var monsterLocation in dungeonMap.GetMonsterLocations())
            {
                if (monsterFov.IsInFov(monsterLocation.X, monsterLocation.Y))
                {
                    var alertedMonster = dungeonMap.GetMonsterAt(monsterLocation.X, monsterLocation.Y);
                    if (!alertedMonster.TurnsAlerted.HasValue)
                    {
                        alertedMonster.TurnsAlerted = 1;
                        didShoutForHelp = true;
                    }
                }
            }

            if (didShoutForHelp)
            {
                RogueGame.MessageLog.Add($"{monster.Name} shouts for help!");
            }

            return didShoutForHelp;
        }
    }
}
