using Microsoft.Xna.Framework;
using RogueSharp;
using RogueSharpSamples.SadConsoleV8NetCore.Equipment;
using RogueSharpSamples.SadConsoleV8NetCore.Interfaces;
using SadConsole;
using IDrawable = RogueSharpSamples.SadConsoleV8NetCore.Interfaces.IDrawable;

namespace RogueSharpSamples.SadConsoleV8NetCore.Core
{
    public class Actor : IActor, IDrawable, IScheduleable
    {
        private int _attack;
        private int _attackChance;
        private int _awareness;
        private int _defense;
        private int _defenseChance;
        private int _gold;
        private int _maxHealth;
        private int _speed;

        public Actor()
        {
            Head = HeadEquipment.None();
            Body = BodyEquipment.None();
            Hand = HandEquipment.None();
            Feet = FeetEquipment.None();
        }

        // IActor
        public HeadEquipment Head { get; set; }
        public BodyEquipment Body { get; set; }
        public HandEquipment Hand { get; set; }
        public FeetEquipment Feet { get; set; }

        public int Attack
        {
            get => _attack + Head.Attack + Body.Attack + Hand.Attack + Feet.Attack;
            set => _attack = value;
        }

        public int AttackChance
        {
            get => _attackChance + Head.AttackChance + Body.AttackChance + Hand.AttackChance + Feet.AttackChance;
            set => _attackChance = value;
        }

        public int Awareness
        {
            get => _awareness + Head.Awareness + Body.Awareness + Hand.Awareness + Feet.Awareness;
            set => _awareness = value;
        }

        public int Defense
        {
            get => _defense + Head.Defense + Body.Defense + Hand.Defense + Feet.Defense;
            set => _defense = value;
        }

        public int DefenseChance
        {
            get => _defenseChance + Head.DefenseChance + Body.DefenseChance + Hand.DefenseChance + Feet.DefenseChance;
            set => _defenseChance = value;
        }

        public int Gold
        {
            get => _gold + Head.Gold + Body.Gold + Hand.Gold + Feet.Gold;
            set => _gold = value;
        }

        public int Health { get; set; }

        public int MaxHealth
        {
            get => _maxHealth + Head.MaxHealth + Body.MaxHealth + Hand.MaxHealth + Feet.MaxHealth;
            set => _maxHealth = value;
        }

        public string Name { get; set; }

        public int Speed
        {
            get => _speed + Head.Speed + Body.Speed + Hand.Speed + Feet.Speed;
            set => _speed = value;
        }

        // IDrawable
        public Color Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(Console mapConsole, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                mapConsole.Print(X, Y, Symbol.ToString(), Color, Colors.FloorBackgroundFov);
            }
            else
            {
                mapConsole.Print(X, Y, ".", Colors.Floor, Colors.FloorBackground);
            }
        }

        // IScheduleable
        public int Time => Speed;
    }
}
