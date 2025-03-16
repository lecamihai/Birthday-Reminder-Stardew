// CustomMessageBox.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using StardewValley.GameData.Objects;

namespace BirthdayReminderMod
{
      public class CustomMessageBox : IClickableMenu
    {
        private int paddingTop = 16;
        private int paddingBetween = 8;
        private int paddingBottom = 16;
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

        public CustomMessageBox(string message, List<string> lovedGifts, IMonitor monitor, NPC npc) : base(0, 0, 0, 0, true)
        {
            this.message = message;
            this.lovedGifts = lovedGifts;
            this.monitor = monitor;
            this.npc = npc;
            this.npcSprite = npc?.Sprite?.Texture;
            this.heartTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            this.lineHeight = (int)Game1.smallFont.MeasureString("A").Y;
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

            int totalHeight = paddingTop + (lines.Length * lineHeight) + paddingBetween + (lovedGifts.Count * (lineHeight + iconSpacing)) + paddingBottom;
            if (npcSprite != null) totalHeight += (int)(npc.Sprite.SourceRect.Height * spriteScale) + paddingBetween;
            this.height = totalHeight;
        }

        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
            Vector2 textPos = new Vector2(xPositionOnScreen + paddingSides, yPositionOnScreen + paddingTop);
            string[] lines = message.Split('\n');

            if (lines.Length > 0)
                b.DrawString(Game1.smallFont, lines[0], textPos, Color.Black);

            if (npcSprite != null && lines.Length > 1)
                textPos.Y = DrawNPCSpriteAndHearts(b, textPos.X, textPos.Y + lineHeight + paddingBetween);

            for (int i = 1; i < lines.Length; i++)
                b.DrawString(Game1.smallFont, lines[i], new Vector2(textPos.X, textPos.Y + (i - 1) * lineHeight), Color.Black);

            textPos.Y += (lines.Length - 1) * lineHeight + paddingBetween;
            DrawGifts(b, textPos.X, textPos.Y);
            drawMouse(b);
        }

        private float DrawNPCSpriteAndHearts(SpriteBatch b, float x, float y)
        {
            if (npcSprite == null) return y;

            Rectangle sourceRect = new Rectangle(0, 0, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight);
            int spriteY = (int)y + spriteYOffset;
            b.Draw(npcSprite, new Vector2(x, spriteY), sourceRect, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0.86f);
            DrawHearts(b, (int)x + (int)(npc.Sprite.SpriteWidth * spriteScale) + heartXOffset, spriteY + (int)(npc.Sprite.SpriteHeight * spriteScale / 2), Game1.player.getFriendshipHeartLevelForNPC(npc.Name));
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
                int itemId = GetItemId(giftId);
                float yPos = y + i * (lineHeight + iconSpacing);

                if (itemId != -1)
                {
                    Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, itemId, 16, 16);
                    b.Draw(Game1.objectSpriteSheet, new Vector2(startX, yPos), sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
                    b.DrawString(Game1.smallFont, giftName, new Vector2(startX + iconSize + iconSpacing, yPos), Color.Black);
                }
                else
                {
                    b.DrawString(Game1.smallFont, giftName, new Vector2(startX, yPos), Color.Black);
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