using RogueSharp.SadConsole.Playground.Main.Core;

namespace RogueSharp.SadConsole.Playground.Main.Items
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
