using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BirthdayReminderMod
{
    public class CustomMessageBox : IClickableMenu
    {
        // Layout configuration
        private readonly int paddingTop = 16;
        private readonly int paddingBetween = 8;
        private readonly int paddingBottom = 16;
        private readonly int paddingSides = 16;
        private readonly int iconSize = 32;
        private readonly int iconSpacing = 8;

        // Positioning offsets for the box
        private readonly int offsetX = 16;
        private readonly int offsetY = 280;

        // NPC sprite and hearts
        private readonly float spriteScale = 3f;
        private readonly Texture2D npcSprite;
        private readonly Texture2D heartTexture;
        private readonly int maxHearts = 10;
        private readonly int heartsPerRow = 5;
        private readonly int heartSourceWidth = 7;
        private readonly int heartScale = 4;
        private readonly int heartSpacing = 4;

        // Offsets for sprite and hearts
        private readonly int spriteYOffset = -30;
        private readonly int heartXOffset = 20;
        private readonly int heartYOffset = 10;

        private readonly string message;
        private readonly List<string> lovedGifts;
        private readonly IMonitor monitor;
        private readonly NPC npc;

        private int lineHeight;

        public CustomMessageBox(string message, List<string> lovedGifts, IMonitor monitor, NPC npc)
            : base(0, 0, 0, 0, true)
        {
            this.message = message;
            this.lovedGifts = lovedGifts;
            this.monitor = monitor;
            this.npc = npc;

            this.npcSprite = npc?.Sprite?.Texture;
            this.heartTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");

            this.lineHeight = (int)Game1.smallFont.MeasureString("A").Y;

            // Calculate dimensions
            CalculateDimensions();

            // Position top-right
            int screenWidth = Game1.uiViewport.Width;
            int screenHeight = Game1.uiViewport.Height;
            this.xPositionOnScreen = screenWidth - this.width - offsetX;
            this.yPositionOnScreen = offsetY;
        }

        private void CalculateDimensions()
        {
            string[] lines = message.Split('\n');
            int numberOfMessageLines = lines.Length;

            // Get max line width for the message
            float maxLineWidth = 0f;
            foreach (var line in lines)
            {
                float lineWidth = Game1.smallFont.MeasureString(line).X;
                if (lineWidth > maxLineWidth)
                    maxLineWidth = lineWidth;
            }

            // Calculate width needed for gift icons and text
            float giftsWidth = 0f;
            foreach (var gift in lovedGifts)
            {
                float giftWidth = Game1.smallFont.MeasureString(gift).X + iconSize + iconSpacing;
                if (giftWidth > giftsWidth)
                    giftsWidth = giftWidth;
            }

            // Calculate hearts width
            int heartFullWidth = heartSourceWidth * heartScale;
            int heartsWidth = heartsPerRow * (heartFullWidth + heartSpacing) + heartSpacing;

            int combinedSpriteHeartsWidth = 0;
            if (npcSprite != null)
            {
                int spriteWidth = (int)(npc.Sprite.SourceRect.Width * spriteScale);
                combinedSpriteHeartsWidth = spriteWidth + heartSpacing + heartsWidth + heartXOffset;
            }

            int totalWidth = Math.Max((int)maxLineWidth, Math.Max((int)giftsWidth, combinedSpriteHeartsWidth));
            totalWidth += paddingSides * 2;

            // Minimum width
            int minWidth = 300;
            if (totalWidth < minWidth)
                totalWidth = minWidth;

            int totalHeight = paddingTop 
                              + (numberOfMessageLines * lineHeight) 
                              + paddingBetween
                              + (lovedGifts.Count * (lineHeight + iconSpacing)) 
                              + paddingBottom;

            if (npcSprite != null)
            {
                int spriteHeight = (int)(npc.Sprite.SourceRect.Height * spriteScale);
                totalHeight += spriteHeight + paddingBetween;
            }

            this.width = totalWidth;
            this.height = totalHeight;
        }

        public override void draw(SpriteBatch b)
        {
            DrawBox(b);

            float textStartX = this.xPositionOnScreen + paddingSides;
            float textStartY = this.yPositionOnScreen + paddingTop;

            string[] lines = message.Split('\n');

            // Draw main message lines
            if (lines.Length > 0)
            {
                b.DrawString(Game1.smallFont, lines[0], new Vector2(textStartX, textStartY), Color.Black);
            }

            // Draw NPC sprite and hearts
            if (npcSprite != null && lines.Length > 1)
            {
                textStartY = DrawNPCSpriteAndHearts(b, textStartX, textStartY + lineHeight + paddingBetween);
            }

            // Draw remaining message lines (e.g., "They love:")
            for (int i = 1; i < lines.Length; i++)
            {
                b.DrawString(Game1.smallFont, lines[i], new Vector2(textStartX, textStartY + (i - 1) * lineHeight), Color.Black);
            }

            // Move below the last line of message
            textStartY += (lines.Length - 1) * lineHeight + paddingBetween;

            // Draw gifts
            DrawGifts(b, textStartX, textStartY);

            this.drawMouse(b);
        }

        private void DrawBox(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);
        }

        private float DrawNPCSpriteAndHearts(SpriteBatch b, float textStartX, float currentY)
        {
            int spriteWidth = (int)(npc.Sprite.SourceRect.Width * spriteScale);
            int spriteHeight = (int)(npc.Sprite.SourceRect.Height * spriteScale);
            int spriteX = (int)textStartX;
            int spriteY = (int)(currentY + spriteYOffset);

            // Draw NPC sprite
            b.Draw(
                npcSprite,
                new Vector2(spriteX, spriteY),
                npc.Sprite.SourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                spriteScale,
                SpriteEffects.None,
                0.86f
            );

            // Draw hearts
            DrawHearts(b, spriteX + spriteWidth + heartSpacing + heartXOffset, spriteY + (spriteHeight / 2), Game1.player.getFriendshipHeartLevelForNPC(npc.Name));

            return spriteY + spriteHeight + paddingBetween;
        }

        private void DrawHearts(SpriteBatch b, int startX, int centerY, int affectionLevel)
        {
            int heartFullWidth = heartSourceWidth * heartScale;
            int heartRows = (int)Math.Ceiling((float)maxHearts / heartsPerRow);

            // Position hearts so they are roughly centered vertically
            int totalHeartsHeight = heartRows * (heartFullWidth + heartSpacing) - heartSpacing;
            int startY = centerY - totalHeartsHeight / 2 + heartYOffset;

            for (int i = 0; i < maxHearts; i++)
            {
                int row = i / heartsPerRow;
                int col = i % heartsPerRow;

                bool filled = i < affectionLevel;
                Rectangle sourceRect = new Rectangle(211 + (filled ? 0 : 7), 428, 7, 6);

                b.Draw(
                    heartTexture,
                    new Vector2(
                        startX + col * (heartFullWidth + heartSpacing),
                        startY + row * (heartFullWidth + heartSpacing)
                    ),
                    sourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    heartScale,
                    SpriteEffects.None,
                    0.87f
                );
            }
        }

        private void DrawGifts(SpriteBatch b, float textStartX, float textStartY)
        {
            for (int i = 0; i < lovedGifts.Count; i++)
            {
                string giftName = lovedGifts[i];
                int itemId = GetItemId(giftName);
                int yPosition = (int)(textStartY + i * (lineHeight + iconSpacing));
                float currentX = textStartX;

                if (itemId != -1)
                {
                    // Draw icon
                    Texture2D texture = Game1.objectSpriteSheet;
                    Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(texture, itemId, 16, 16);
                    b.Draw(texture, new Vector2(currentX, yPosition), sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);

                    currentX += iconSize + iconSpacing;
                }

                // Draw gift name
                b.DrawString(Game1.smallFont, giftName, new Vector2(currentX, yPosition), Color.Black);
            }
        }

        private int GetItemId(string itemName)
        {
            if (Constants.ItemNameToIdMap.TryGetValue(itemName, out int id))
            {
                return id;
            }

            this.monitor.Log($"Item ID for '{itemName}' not found.", LogLevel.Warn);
            return -1;
        }
    }
}
