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
        private string fromTime;
        private string untilTime;

        public CustomMessageBox(string message, List<string> lovedGifts, IMonitor monitor, NPC npc, NPCDataProvider npcDataProvider):base(0, 0, 0, 0, true)
        {
            this.message = message;
            this.lovedGifts = lovedGifts;
            this.monitor = monitor;
            this.npc = npc;
            this.npcSprite = npc?.Sprite?.Texture;
            this.heartTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            this.lineHeight = (int)Game1.smallFont.MeasureString("A").Y;
            this.npcDataProvider = npcDataProvider;

            // Get the current location, start time, and end time
            var (location, fromTime, untilTime) = GetCurrentLocation(npc);
            this.currentLocation = location;
            this.fromTime = fromTime;
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

            // Calculate height
            int totalHeight = paddingTop;

            // 1. Birthday message lines
            totalHeight += lines.Length * lineHeight;

            // 2. NPC sprite and hearts
            if (npcSprite != null)
            {
                totalHeight += paddingBetween;
                totalHeight += (int)(npc.Sprite.SourceRect.Height * spriteScale);
                totalHeight += paddingBetween;
                totalHeight += (maxHearts / heartsPerRow) * (7 * heartScale + heartSpacing);
            }

            // 3. Location text
            totalHeight += paddingBetween;
            int maxTextWidth = this.width - 2 * paddingSides;

            // "Current Schedule:" line
            totalHeight += lineHeight;

            // Current location lines
            List<string> wrappedCurrentLocation = WrapTextWithBullet(currentLocation, Game1.smallFont, maxTextWidth);
            totalHeight += wrappedCurrentLocation.Count * lineHeight;

            // Schedule time lines
            if (!string.IsNullOrEmpty(fromTime) && !string.IsNullOrEmpty(untilTime))
            {
                string scheduleTimeText = $"From {fromTime} - {untilTime}";
                List<string> wrappedScheduleTime = WrapTextWithBullet(scheduleTimeText, Game1.smallFont, maxTextWidth);
                totalHeight += wrappedScheduleTime.Count * lineHeight;
            }

            // 4. Loved gifts
            totalHeight += paddingBetween;
            totalHeight += (lovedGifts.Count * lineHeight) + (Math.Max(0, lovedGifts.Count - 1) * iconSpacing);

            this.height = totalHeight - 90;
        }

        private (string Location, string FromTime, string UntilTime) GetCurrentLocation(NPC npc)
        {
            if (npc == null)
            {
                monitor.Log("NPC is null. Cannot determine location.", LogLevel.Error);
                return ("Unknown Location", null, null);
            }

            try
            {
                // Load NPC schedule from game files
                Dictionary<string, string> scheduleData = Game1.content.Load<Dictionary<string, string>>($"Characters\\schedules\\{npc.Name}");

                // Determine schedule key
                string scheduleKey = Game1.currentSeason;

                // 1. Check Green Rain schedule
                if (Game1.IsGreenRainingHere())
                {
                    scheduleKey = "GreenRain";
                }
                // 2. Check birthday schedule (season_day)
                else
                {
                    string birthdayKey = $"{Game1.currentSeason}_{Game1.dayOfMonth}";
                    if (scheduleData.ContainsKey(birthdayKey))
                    {
                        scheduleKey = birthdayKey;
                    }
                    // 3. Check day-of-week with current season (season_dayOfWeek)
                    else
                    {
                        string dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                        string seasonDayOfWeekKey = $"{Game1.currentSeason}_{dayOfWeek}";
                        if (scheduleData.ContainsKey(seasonDayOfWeekKey))
                        {
                            scheduleKey = seasonDayOfWeekKey;
                        }
                        // 4. Check standalone day-of-week
                        else if (scheduleData.ContainsKey(dayOfWeek))
                        {
                            scheduleKey = dayOfWeek;
                        }
                    }
                }

                // 5. Check active festivals
                foreach (string festivalId in Game1.netWorldState.Value.ActivePassiveFestivals)
                {
                    if (scheduleData.ContainsKey(festivalId))
                    {
                        scheduleKey = festivalId;
                        break;
                    }
                }

                // 6. Fallback logic: Try current season -> spring
                if (!scheduleData.ContainsKey(scheduleKey))
                {
                    if (scheduleKey == Game1.currentSeason && scheduleData.ContainsKey("spring"))
                    {
                        scheduleKey = "spring";
                        monitor.Log($"Using spring schedule as fallback for {npc.Name}", LogLevel.Debug);
                    }
                    else
                    {
                        monitor.Log($"No schedule found for {npc.Name} with key {scheduleKey}", LogLevel.Warn);
                        return ("Unknown Location", null, null);
                    }
                }

                // Parse schedule script
                string scheduleScript = scheduleData[scheduleKey];
                List<ScheduleSlot> scheduleSlots = new List<ScheduleSlot>();
                string[] segments = scheduleScript.Split('/');
                for (int i = 0; i < segments.Length; i++)
                {
                    string[] parts = segments[i].Split(' ');
                    if (parts.Length < 4) continue;

                    int startTime = int.Parse(parts[0]);
                    int endTime = i < segments.Length - 1 ? int.Parse(segments[i + 1].Split(' ')[0]) : 2600;
                    string location = parts[1];

                    scheduleSlots.Add(new ScheduleSlot(startTime, endTime, location));
                }

                // Sort schedule slots by start time
                scheduleSlots = scheduleSlots.OrderBy(s => s.StartTime).ToList();

                // Get the current time
                int currentTime = Game1.timeOfDay;

                // Handle "sleeping" state before the first activity
                if (currentTime < 600)
                {
                    return ("Sleeping", "12:00 AM", FormatTime(600));
                }

                // Handle "sleeping" state before the first scheduled activity
                if (scheduleSlots.Count > 0 && currentTime < scheduleSlots[0].StartTime)
                {
                    return ("Sleeping", "6:00 AM", FormatTime(scheduleSlots[0].StartTime));
                }

                // Find the current time slot
                for (int i = 0; i < scheduleSlots.Count; i++)
                {
                    var currentSlot = scheduleSlots[i];
                    var nextSlot = i < scheduleSlots.Count - 1 ? scheduleSlots[i + 1] : null;

                    if (currentTime >= currentSlot.StartTime && 
                        (nextSlot == null || currentTime < nextSlot.StartTime))
                    {
                        return (currentSlot.Location, 
                                FormatTime(currentSlot.StartTime), 
                                FormatTime(nextSlot?.StartTime ?? 2600));
                    }
                }

                // Handle "sleeping" state after the last activity
                if (scheduleSlots.Count > 0 && currentTime >= scheduleSlots.Last().StartTime)
                {
                    return ("Sleeping", FormatTime(scheduleSlots.Last().StartTime), "2:00 AM");
                }

                // Default to "Sleeping" if no schedule is found
                return ("Sleeping", "12:00 AM", "2:00 AM");
            }
            catch (Exception ex)
            {
                monitor.Log($"Error loading schedule for {npc.Name}: {ex.Message}", LogLevel.Error);
                return ("Error loading schedule", null, null);
            }
        }

        private List<string> WrapTextWithBullet(string text, SpriteFont font, float maxWidth)
        {
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(text))
                return lines;

            string[] words = text.Split(' ');
            string bullet = "- ";
            string indent = "  ";
            float bulletWidth = font.MeasureString(bullet).X;
            float indentWidth = font.MeasureString(indent).X;

            string currentLine = bullet;
            float currentWidth = bulletWidth;

            foreach (string word in words)
            {
                string wordWithSpace = word + " ";
                float wordWidth = font.MeasureString(wordWithSpace).X;

                if (currentWidth + wordWidth > maxWidth)
                {
                    lines.Add(currentLine.TrimEnd());
                    currentLine = indent + wordWithSpace;
                    currentWidth = indentWidth + wordWidth;
                }
                else
                {
                    currentLine += wordWithSpace;
                    currentWidth += wordWidth;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
                lines.Add(currentLine.TrimEnd());

            return lines;
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

            // Draw the current location and schedule times
            int maxTextWidth = this.width - 2 * paddingSides;

            // Draw "Current Schedule:"
            b.DrawString(Game1.smallFont, "Current Schedule:", textPos, Color.Black);
            textPos.Y += lineHeight;

            // Draw schedule times if applicable
            if (!string.IsNullOrEmpty(fromTime) && !string.IsNullOrEmpty(untilTime))
            {
                string scheduleTimeText = $"{fromTime} - {untilTime}";
                List<string> wrappedScheduleTime = WrapTextWithBullet(scheduleTimeText, Game1.smallFont, maxTextWidth);
                foreach (string line in wrappedScheduleTime)
                {
                    b.DrawString(Game1.smallFont, line, textPos, Color.Black);
                    textPos.Y += lineHeight;
                }
            }

            // Draw wrapped current location lines
            List<string> wrappedCurrentLocation = WrapTextWithBullet(currentLocation, Game1.smallFont, maxTextWidth);
            foreach (string line in wrappedCurrentLocation)
            {
                b.DrawString(Game1.smallFont, line, textPos, Color.Black);
                textPos.Y += lineHeight;
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