﻿using System.Text;
using Microsoft.Xna.Framework.Input;
using RogueSharp.DiceNotation;
using RogueSharp.SadConsole.Playground.Main.Core;
using RogueSharp.SadConsole.Playground.Main.Equipment;
using RogueSharp.SadConsole.Playground.Main.Items;

namespace RogueSharp.SadConsole.Playground.Main.Systems
{
    public class CommandSystem
    {
        public bool IsPlayerTurn { get; set; }

        // TODO: Weird - needed to refactor old RogueGame.Update code into HandleInput
        public bool ExitRequested { get; set; }
        public bool NextLevelRequested { get; set; }

        public bool MovePlayer(Direction direction)
        {
            int x;
            int y;

            switch (direction)
            {
                case Direction.Up:
                {
                    x = RogueGame.Player.X;
                    y = RogueGame.Player.Y - 1;
                    break;
                }
                case Direction.Down:
                {
                    x = RogueGame.Player.X;
                    y = RogueGame.Player.Y + 1;
                    break;
                }
                case Direction.Left:
                {
                    x = RogueGame.Player.X - 1;
                    y = RogueGame.Player.Y;
                    break;
                }
                case Direction.Right:
                {
                    x = RogueGame.Player.X + 1;
                    y = RogueGame.Player.Y;
                    break;
                }
                default:
                {
                    return false;
                }
            }

            if (RogueGame.DungeonMap.SetActorPosition(RogueGame.Player, x, y))
            {
                return true;
            }

            var monster = RogueGame.DungeonMap.GetMonsterAt(x, y);

            if (monster != null)
            {
                Attack(RogueGame.Player, monster);
                return true;
            }

            return false;
        }

        public void ActivateMonsters()
        {
            var scheduleable = RogueGame.SchedulingSystem.Get();
            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                RogueGame.SchedulingSystem.Add(RogueGame.Player);
            }
            else
            {
                var monster = scheduleable as Monster;

                if (monster != null)
                {
                    monster.PerformAction(this);
                    RogueGame.SchedulingSystem.Add(monster);
                }

                ActivateMonsters();
            }
        }

        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!RogueGame.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
            {
                if (RogueGame.Player.X == cell.X && RogueGame.Player.Y == cell.Y)
                {
                    Attack(monster, RogueGame.Player);
                }
            }
        }

        public void Attack(Actor attacker, Actor defender)
        {
            var attackMessage = new StringBuilder();
            var defenseMessage = new StringBuilder();

            var hits = ResolveAttack(attacker, defender, attackMessage);

            var blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            RogueGame.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                RogueGame.MessageLog.Add(defenseMessage.ToString());
            }

            var damage = hits - blocks;

            ResolveDamage(defender, damage);
        }

        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            var hits = 0;

            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);
            var attackDice = new DiceExpression().Dice(attacker.Attack, 100);

            var attackResult = attackDice.Roll();
            foreach (var termResult in attackResult.Results)
            {
                attackMessage.Append(termResult.Value + ", ");
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }

        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage,
            StringBuilder defenseMessage)
        {
            var blocks = 0;

            if (hits > 0)
            {
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defenseMessage.AppendFormat("  {0} defends and rolls: ", defender.Name);
                var defenseDice = new DiceExpression().Dice(defender.Defense, 100);

                var defenseRoll = defenseDice.Roll();
                foreach (var termResult in defenseRoll.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }

                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append("and misses completely.");
            }

            return blocks;
        }

        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;

                RogueGame.MessageLog.Add($"  {defender.Name} was hit for {damage} damage");

                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                RogueGame.MessageLog.Add($"  {defender.Name} blocked all damage");
            }
        }

        private static void ResolveDeath(Actor defender)
        {
            if (defender is Player)
            {
                RogueGame.MessageLog.Add($"  {defender.Name} was killed, GAME OVER MAN!");
            }
            else if (defender is Monster)
            {
                if (defender.Head != null && defender.Head != HeadEquipment.None())
                {
                    RogueGame.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Head);
                }

                if (defender.Body != null && defender.Body != BodyEquipment.None())
                {
                    RogueGame.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Body);
                }

                if (defender.Hand != null && defender.Hand != HandEquipment.None())
                {
                    RogueGame.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Hand);
                }

                if (defender.Feet != null && defender.Feet != FeetEquipment.None())
                {
                    RogueGame.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Feet);
                }

                RogueGame.DungeonMap.AddGold(defender.X, defender.Y, defender.Gold);
                RogueGame.DungeonMap.RemoveMonster((Monster) defender);

                RogueGame.MessageLog.Add($"  {defender.Name} died and dropped {defender.Gold} gold");
            }
        }

        public bool HandleInput(InputState inputState)
        {
            var didPlayerAct = false;

            if (inputState.IsKeyPressed(Keys.Up))
            {
                didPlayerAct = MovePlayer(Direction.Up);
            }
            else if (inputState.IsKeyPressed(Keys.Down))
            {
                didPlayerAct = MovePlayer(Direction.Down);
            }
            else if (inputState.IsKeyPressed(Keys.Left))
            {
                didPlayerAct = MovePlayer(Direction.Left);
            }
            else if (inputState.IsKeyPressed(Keys.Right))
            {
                didPlayerAct = MovePlayer(Direction.Right);
            }
            else if (inputState.IsKeyPressed(Keys.Escape))
            {
                didPlayerAct = ExitRequested = true;
            }
            else if (inputState.IsKeyPressed(Keys.OemPeriod) && RogueGame.DungeonMap.CanMoveDownToNextLevel())
            {
                didPlayerAct = NextLevelRequested = true;
            }

            if (didPlayerAct)
            {
                return true;
            }

            if (inputState.IsKeyPressed(Keys.Q))
            {
                return RogueGame.Player.QAbility.Perform();
            }

            if (inputState.IsKeyPressed(Keys.W))
            {
                return RogueGame.Player.WAbility.Perform();
            }

            if (inputState.IsKeyPressed(Keys.E))
            {
                return RogueGame.Player.EAbility.Perform();
            }

            if (inputState.IsKeyPressed(Keys.R))
            {
                return RogueGame.Player.RAbility.Perform();
            }

            var didUseItem = false;
            if (inputState.IsKeyPressed(Keys.D1))
            {
                didUseItem = RogueGame.Player.Item1.Use();
            }
            else if (inputState.IsKeyPressed(Keys.D2))
            {
                didUseItem = RogueGame.Player.Item2.Use();
            }
            else if (inputState.IsKeyPressed(Keys.D3))
            {
                didUseItem = RogueGame.Player.Item3.Use();
            }
            else if (inputState.IsKeyPressed(Keys.D4))
            {
                didUseItem = RogueGame.Player.Item4.Use();
            }

            if (didUseItem)
            {
                RemoveItemsWithNoRemainingUses();
            }

            return didUseItem;
        }

        private static void RemoveItemsWithNoRemainingUses()
        {
            if (RogueGame.Player.Item1.RemainingUses <= 0)
            {
                RogueGame.Player.Item1 = new NoItem();
            }

            if (RogueGame.Player.Item2.RemainingUses <= 0)
            {
                RogueGame.Player.Item2 = new NoItem();
            }

            if (RogueGame.Player.Item3.RemainingUses <= 0)
            {
                RogueGame.Player.Item3 = new NoItem();
            }

            if (RogueGame.Player.Item4.RemainingUses <= 0)
            {
                RogueGame.Player.Item4 = new NoItem();
            }
        }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
            RogueGame.Player.Tick();
        }
    }
}
