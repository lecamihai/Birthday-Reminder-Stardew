using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace BirthdayReminderMod
{
    public class ModEntry : Mod
    {
        private NPCDataProvider npcDataProvider;

        public override void Entry(IModHelper helper)
        {
            npcDataProvider = new NPCDataProvider();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            string season = Game1.currentSeason;
            int day = Game1.dayOfMonth;
            string dateKey = $"{season} {day}";

            foreach (var npcInfo in npcDataProvider.GetNPCData().Values)
            {
                if (npcInfo.Birthday.Equals(dateKey, StringComparison.OrdinalIgnoreCase))
                {
                    NPC gameNPC = Game1.getCharacterFromName(npcInfo.Name);
                    if (gameNPC != null)
                    {
                        string message = $"It's {npcInfo.Name}'s birthday!\nThey love:";
                        Game1.activeClickableMenu = new CustomMessageBox(message, npcInfo.LovedGifts, this.Monitor, gameNPC);
                    }
                    else
                    {
                        Monitor.Log($"NPC '{npcInfo.Name}' not found in the game.", LogLevel.Warn);
                    }
                }
            }
        }
    }
}
