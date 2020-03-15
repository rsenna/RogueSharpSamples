namespace RogueSharp.SadConsole.Playground.Main.Interfaces
{
    public interface IItem
    {
        string Name { get; }
        int RemainingUses { get; }

        bool Use();
    }
}
