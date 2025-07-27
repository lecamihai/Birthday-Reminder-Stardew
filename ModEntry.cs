//ModEntry.cs
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace BirthdayReminderMod
{
    public class ModEntry : Mod
    {
        private List<NPC> birthdayNPCs = new List<NPC>();
        private ClickableTextureComponent birthdayButton;
        private bool buttonVisible;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked; // Add this line
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Check if the cutscene has ended and reset buttonVisible
            if (!Game1.eventUp)
            {
                // Re-evaluate buttonVisible based on whether there are birthdays
                buttonVisible = birthdayNPCs.Count > 0;
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            birthdayNPCs.Clear();
            string currentSeason = Game1.currentSeason;
            int currentDay = Game1.dayOfMonth;

            Utility.ForEachVillager(npc =>
            {
                if (npc.isBirthday() && npc.CanReceiveGifts())
                    this.birthdayNPCs.Add(npc);
                return true;
            });
            buttonVisible = birthdayNPCs.Count > 0;
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Hide the button if a cutscene is active
            if (Game1.eventUp)
            {
                return; // Don't set buttonVisible to false here
            }

            // Only proceed if the button is supposed to be visible
            if (!buttonVisible) return;

            int x = Game1.uiViewport.Width - 300 + 10;
            int y = 255;

            birthdayButton = new ClickableTextureComponent(
                new Rectangle(x, y, 48, 48),
                Game1.objectSpriteSheet,
                new Rectangle(221 % 24 * 16, 221 / 24 * 16, 16, 16),
                3f
            );

            birthdayButton.draw(Game1.spriteBatch);

            if (birthdayButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                NPC npc = birthdayNPCs[0];
                var npcInfo = npc.GetData();
                IClickableMenu.drawHoverText(Game1.spriteBatch,  $"It's {npc.Name}'s birthday!", Game1.smallFont);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Check if the button is visible and the player is not in a cutscene
            if (!buttonVisible || Game1.eventUp)
                return;

            // Check if the left mouse button or the "A" button on the controller is pressed
            if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA) && 
                birthdayButton?.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)) == true)
            {
                // Handle button click
                if (Game1.activeClickableMenu is CustomMessageBox)
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                    this.Helper.Input.Suppress(e.Button);
                }
                else
                {
                    NPC npc = birthdayNPCs[0];
                    var npcInfo = npc.GetData();
                    var lovedGifts = Game1.NPCGiftTastes[npc.Name].Split('/')[NPC.gift_taste_love + 1].Split(' ');
                    Game1.activeClickableMenu = new CustomMessageBox(
                        $"It's {npc.Name}'s birthday!\nThey love:",
                        lovedGifts.ToList(),
                        Monitor,
                        npc
                    );
                    Game1.playSound("bigSelect");
                    this.Helper.Input.Suppress(e.Button);
                }
            }
        }
    }
}