// CustomMessageBox.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using StardewValley.GameData.Objects;
using System.Linq;

namespace BirthdayReminderMod
{
      public class CustomMessageBox : IClickableMenu
    {
        private int paddingTop = 16;
        private int paddingBetween = 8;
        private int paddingSides = 16;
        private int iconSize = 32;
        private int iconSpacing = 8;
        private float spriteScale = 3f;
        private Texture2D npcSprite;
        private Texture2D heartTexture;
        private int maxHearts = 10;
        private int heartsPerRow = 5;
        private int heartScale = 4;
        private int heartSpacing = 4;
        private int spriteYOffset = -10;
        private int heartXOffset = 20;
        private int heartYOffset = 10;
        private string message;
        private List<string> lovedGifts;
        private IMonitor monitor;
        private NPC npc;
        private int lineHeight;
        private string currentLocation;
        private NPCDataProvider npcDataProvider;
        
        private string untilTime;

        public CustomMessageBox(string message, List<string> lovedGifts, IMonitor monitor, NPC npc, Dictionary<string, List<ScheduleSlot>> schedule, NPCDataProvider npcDataProvider) : base(0, 0, 0, 0, true)
        {
            this.message = message;
            this.lovedGifts = lovedGifts;
            this.monitor = monitor;
            this.npc = npc;
            this.npcSprite = npc?.Sprite?.Texture;
            this.heartTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            this.lineHeight = (int)Game1.smallFont.MeasureString("A").Y;
            this.npcDataProvider = npcDataProvider;

            // Get the current location and "until" time
            var (location, untilTime) = GetCurrentLocation(npc, schedule);
            this.currentLocation = location;
            this.untilTime = untilTime;

            CalculateDimensions();
            this.xPositionOnScreen = Game1.uiViewport.Width - this.width - 16;
            this.yPositionOnScreen = 320;
        }

        private void CalculateDimensions()
        {
            string[] lines = message.Split('\n');
            float maxLineWidth = 0f;
            foreach (var line in lines)
            {
                float lineWidth = Game1.smallFont.MeasureString(line).X;
                if (lineWidth > maxLineWidth) maxLineWidth = lineWidth;
            }

            float giftsWidth = 0f;
            foreach (var gift in lovedGifts)
            {
                string giftName = GetItemName(gift);
                float giftWidth = Game1.smallFont.MeasureString(giftName).X + iconSize + iconSpacing;
                if (giftWidth > giftsWidth) giftsWidth = giftWidth;
            }

            int heartsWidth = heartsPerRow * (7 * heartScale + heartSpacing) + heartSpacing;
            int combinedWidth = 0;
            if (npcSprite != null)
            {
                int spriteWidth = (int)(npc.Sprite.SourceRect.Width * spriteScale);
                combinedWidth = spriteWidth + heartSpacing + heartsWidth + heartXOffset;
            }

            int totalWidth = Math.Max((int)maxLineWidth, Math.Max((int)giftsWidth, combinedWidth)) + paddingSides * 2;
            this.width = totalWidth < 300 ? 300 : totalWidth;

            // Calculate the height based on the actual draw order
            int totalHeight = paddingTop;

            // 1. Birthday message
            totalHeight += lines.Length * lineHeight;

            // 2. NPC sprite and hearts
            if (npcSprite != null)
            {
                totalHeight += paddingBetween; // Space after message
                totalHeight += (int)(npc.Sprite.SourceRect.Height * spriteScale); // Sprite height
                totalHeight += paddingBetween; // Space after sprite
                totalHeight += maxHearts / heartsPerRow * (7 * heartScale + heartSpacing); // Hearts height
            }

            // 3. Location text
            totalHeight += paddingBetween; // Space after hearts
            totalHeight += 2 * lineHeight; // "Current Location" lines
            if (!string.IsNullOrEmpty(this.untilTime))
            {
                totalHeight += lineHeight; // "Until" line
            }

            // 4. Loved gifts
            totalHeight += paddingBetween; // Space after location text
            totalHeight += (lovedGifts.Count * lineHeight) + (Math.Max(0, lovedGifts.Count - 1) * iconSpacing);

            this.height = totalHeight - 50;
        }

        private (string Location, string UntilTime) GetCurrentLocation(NPC npc, Dictionary<string, List<ScheduleSlot>> schedule)
        {
            // Debug: Log the NPC and schedule
            monitor.Log($"Getting current location for NPC: {npc?.Name ?? "null"}");
            monitor.Log($"Schedule: {(schedule != null ? string.Join(", ", schedule.Keys) : "null")}");

            // Check for null NPC
            if (npc == null)
            {
                monitor.Log("NPC is null. Cannot determine location.", LogLevel.Error);
                return ("Unknown Location", null);
            }

            // Check for null schedule
            if (schedule == null)
            {
                monitor.Log("Schedule is null. Cannot determine location.", LogLevel.Error);
                return ("Unknown Location", null);
            }

            // Get the current time in the game
            int currentTime = Game1.timeOfDay;

            // Get the schedule for the current conditions (e.g., rain, married, etc.)
            string scheduleKey = npcDataProvider.GetNPCData()[npc.Name].Season;
            if (Game1.isRaining)
                scheduleKey = "rain";
            else if (Game1.IsGreenRainingHere())
                scheduleKey = "GreenRain";

            // Debug: Log the schedule key
            monitor.Log($"Using schedule key: {scheduleKey}");

            // Get the schedule for the current key
            if (schedule.TryGetValue(scheduleKey, out List<ScheduleSlot> scheduleSlots))
            {
                // Debug: Log the schedule data
                monitor.Log($"Schedule data: {string.Join(", ", scheduleSlots.Select(s => $"{s.StartTime}-{s.EndTime} {s.Location}"))}");

                // Sort schedule slots by start time
                var sortedSlots = scheduleSlots.OrderBy(s => s.StartTime).ToList();

                // Handle default pre-9AM behavior
                if (currentTime < 900 && !sortedSlots.Any(s => s.StartTime < 900))
                {
                    // NPC is at home until 9:00 AM
                    return ("Sleeping", "9:00 AM");
                }

                // Find the current time slot
                foreach (var slot in sortedSlots)
                {
                    if (currentTime >= slot.StartTime && currentTime < slot.EndTime)
                    {
                        // Return the location and the end time of the current slot
                        return (slot.Location, FormatTime(slot.EndTime));
                    }
                }

                // If no time slot matches, assume the NPC is at their default location
                // and will move to the first scheduled activity
                var firstSlot = sortedSlots.FirstOrDefault();
                if (firstSlot != null)
                {
                    return (npc.DefaultMap, FormatTime(firstSlot.StartTime));
                }
            }
            else
            {
                monitor.Log($"Schedule key '{scheduleKey}' not found in schedule.", LogLevel.Warn);
            }

            // Default location if no schedule is found
            return ("Unknown Location", null);
        }

        private string FormatTime(int time)
        {
            // Convert time from 24-hour format (e.g., 900) to 12-hour format (e.g., 9:00 AM)
            int hour = time / 100;
            int minute = time % 100;
            string period = hour < 12 ? "AM" : "PM";
            if (hour > 12) hour -= 12;
            return $"{hour}:{minute:D2} {period}";
        }

        public override void draw(SpriteBatch b)
        {
            // Draw the background texture box
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

            // Starting position for drawing text and elements
            Vector2 textPos = new Vector2(xPositionOnScreen + paddingSides, yPositionOnScreen + paddingTop);

            // Draw the birthday message
            string[] lines = message.Split('\n');
            if (lines.Length > 0)
            {
                b.DrawString(Game1.smallFont, lines[0], textPos, Color.Black);
                textPos.Y += lineHeight; // Move down for the birthday message
            }

            // Draw NPC sprite and hearts
            if (npcSprite != null)
            {
                textPos.Y += paddingBetween; // Add padding between the birthday message and the NPC sprite
                textPos.Y = DrawNPCSpriteAndHearts(b, textPos.X, textPos.Y);
            }

            // Draw the current location and "until" time
            string locationText = $"Current Schedule: \n - {currentLocation}";
            if (!string.IsNullOrEmpty(this.untilTime))
            {
                locationText += $"\n - Until {this.untilTime}";
            }
            b.DrawString(Game1.smallFont, locationText, textPos, Color.Black);

            // Move down for the location text
            textPos.Y += 2 * lineHeight; // "Current Location" and the location itself
            if (!string.IsNullOrEmpty(this.untilTime))
            {
                textPos.Y += lineHeight; // "Until X time" line
            }

            // Draw the rest of the lines (loved gifts)
            for (int i = 1; i < lines.Length; i++)
            {
                b.DrawString(Game1.smallFont, lines[i], new Vector2(textPos.X, textPos.Y + (i - 1) * lineHeight), Color.Black);
            }

            // Move down for the loved gifts
            textPos.Y += (lines.Length - 1) * lineHeight; // Remove extra paddingBetween here
            DrawGifts(b, textPos.X, textPos.Y);

            // Draw the mouse cursor
            drawMouse(b);
        }

        private float DrawNPCSpriteAndHearts(SpriteBatch b, float x, float y)
        {
            if (npcSprite == null) return y;

            Rectangle sourceRect = new Rectangle(0, 0, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight);
            int spriteY = (int)y + spriteYOffset;
            b.Draw(npcSprite, new Vector2(x, spriteY), sourceRect, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0.86f);

            // Draw hearts
            DrawHearts(b, (int)x + (int)(npc.Sprite.SpriteWidth * spriteScale) + heartXOffset, spriteY + (int)(npc.Sprite.SpriteHeight * spriteScale / 2), Game1.player.getFriendshipHeartLevelForNPC(npc.Name));

            // Return the Y position after drawing the sprite and hearts
            return y + (int)(npc.Sprite.SpriteHeight * spriteScale) + paddingBetween;
        }

        private void DrawHearts(SpriteBatch b, int x, int y, int hearts)
        {
            int heartSize = 7 * heartScale;
            int startY = y - (maxHearts / heartsPerRow * (heartSize + heartSpacing)) / 2 + heartYOffset;

            for (int i = 0; i < maxHearts; i++)
            {
                int row = i / heartsPerRow;
                int col = i % heartsPerRow;
                Rectangle sourceRect = new Rectangle(211 + (i < hearts ? 0 : 7), 428, 7, 6);
                b.Draw(heartTexture, new Vector2(x + col * (heartSize + heartSpacing), startY + row * (heartSize + heartSpacing)), sourceRect, Color.White, 0f, Vector2.Zero, heartScale, SpriteEffects.None, 0.87f);
            }
        }

        private void DrawGifts(SpriteBatch b, float x, float y)
        {
            float startX = x;
            for (int i = 0; i < lovedGifts.Count; i++)
            {
                string giftId = lovedGifts[i];
                string giftName = GetItemName(giftId);
                float yPos = y + i * (lineHeight + iconSpacing);

                // Attempt to retrieve the item's data
                if (Game1.objectData.TryGetValue(giftId, out var itemData))
                {
                    Texture2D spriteSheet = Game1.objectSpriteSheet; // Default to Objects_1
                    int spriteIndex = itemData.SpriteIndex;

                    // Check if the item uses Objects_2
                    if (itemData.Texture != null && itemData.Texture.Equals("TileSheets\\Objects_2", StringComparison.OrdinalIgnoreCase))
                    {
                        spriteSheet = Game1.objectSpriteSheet_2;
                    }

                    Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(spriteSheet, spriteIndex, 16, 16);
                    b.Draw(spriteSheet, new Vector2(startX, yPos), sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
                    b.DrawString(Game1.smallFont, giftName, new Vector2(startX + iconSize + iconSpacing, yPos), Color.Black);
                }
                else
                {
                    // Fallback for items not found in objectData (e.g., vanilla items by ID)
                    int itemId = GetItemId(giftId);
                    if (itemId != -1)
                    {
                        // Check if itemId is beyond the first sheet's capacity (assuming 4096 tiles per sheet)
                        Texture2D spriteSheet = itemId >= 4096 ? Game1.objectSpriteSheet_2 : Game1.objectSpriteSheet;
                        int adjustedItemId = itemId % 4096; // Adjust index for the current sheet

                        Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(spriteSheet, adjustedItemId, 16, 16);
                        b.Draw(spriteSheet, new Vector2(startX, yPos), sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
                        b.DrawString(Game1.smallFont, giftName, new Vector2(startX + iconSize + iconSpacing, yPos), Color.Black);
                    }
                    else
                    {
                        b.DrawString(Game1.smallFont, giftName, new Vector2(startX, yPos), Color.Black);
                    }
                }
            }
        }

        private int GetItemId(string itemId) => int.TryParse(itemId, out int id) ? id : -1;

        private string GetItemName(string itemId)
        {
            if (Game1.objectData.TryGetValue(itemId, out ObjectData data))
            {
                string name = data.DisplayName;
                if (name.StartsWith("[") && name.Contains("]"))
                {
                    string key = name.TrimStart('[').Split(']')[0].Replace("LocalizedText Strings\\", "Strings\\");
                    try { return Game1.content.LoadString(key); }
                    catch { monitor.Log($"Failed loading name for {key}", LogLevel.Warn); }
                }
                return name;
            }
            return "Unknown Item";
        }
    }
}