﻿using RogueSharp;
using RogueSharpSamples.LegacySadConsole.Core;
using RogueSharpSamples.LegacySadConsole.Interfaces;
using RogueSharpSamples.LegacySadConsole.Systems;

namespace RogueSharpSamples.LegacySadConsole.Behaviors
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
