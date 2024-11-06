// ModEntry.cs
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace BirthdayReminderMod
{
    public class ModEntry : Mod
    {
        // Store NPC data
        private Dictionary<string, NPCInfo> npcData;

        public override void Entry(IModHelper helper)
        {
            // Initialize NPC data using the data provider
            npcData = NPCDataProvider.GetNPCData();

            // Subscribe to the DayStarted event
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Get the current season and day
            string season = Game1.currentSeason;
            int day = Game1.dayOfMonth;
            string dateKey = $"{season} {day}";

            // Check if today is any NPC's birthday
            foreach (var npc in npcData.Values)
            {
                if (npc.Birthday.Equals(dateKey, StringComparison.OrdinalIgnoreCase))
                {
                    // Display birthday message without listing gifts
                    string message = $"It's {npc.Name}'s birthday!\nThey love:";

                    // Find the NPC instance in the game
                    NPC gameNPC = Game1.getCharacterFromName(npc.Name);

                    if (gameNPC != null)
                    {
                        // Show the custom message box with the NPC instance
                        Game1.activeClickableMenu = new CustomMessageBox(message, npc.LovedGifts, this.Monitor, gameNPC);
                    }
                    else
                    {
                        Monitor.Log($"NPC '{npc.Name}' not found in the game.", LogLevel.Warn);
                    }
                }
            }
        }
    }
}
