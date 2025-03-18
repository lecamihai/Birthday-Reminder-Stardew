//ModEntry.cs
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System.Collections.Generic;
using StardewValley.GameData.Objects;

namespace BirthdayReminderMod
{
    public class ModEntry : Mod
    {
        private NPCDataProvider npcDataProvider;
        private List<NPC> birthdayNPCs = new List<NPC>();
        private ClickableTextureComponent birthdayButton;
        private bool buttonVisible;

        public override void Entry(IModHelper helper)
        {
            npcDataProvider = new NPCDataProvider();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            birthdayNPCs.Clear();
            string dateKey = $"{Game1.currentSeason} {Game1.dayOfMonth}";

            foreach (var npcInfo in npcDataProvider.GetNPCData().Values)
            {
                if (npcInfo.Birthday.Equals(dateKey, StringComparison.OrdinalIgnoreCase))
                {
                    if (Game1.getCharacterFromName(npcInfo.Name) is NPC npc)
                        birthdayNPCs.Add(npc);
                }
            }
            buttonVisible = birthdayNPCs.Count > 0;
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
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
                var npcInfo = npcDataProvider.GetNPCData()[npc.Name];
                IClickableMenu.drawHoverText(Game1.spriteBatch,  $"It's {npc.Name}'s birthday!", Game1.smallFont);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft && 
                buttonVisible && 
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
                    var npcInfo = npcDataProvider.GetNPCData()[npc.Name];
                    Game1.activeClickableMenu = new CustomMessageBox(
                        $"It's {npc.Name}'s birthday!\nThey love:",
                        npcInfo.LovedGifts,
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