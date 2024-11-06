// CustomMessageBox.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BirthdayReminderMod
{
    // Custom message box class
    public class CustomMessageBox : IClickableMenu
    {
        private readonly string message;
        private readonly List<string> lovedGifts;
        private readonly IMonitor monitor;
        private readonly NPC npc;
        private readonly Texture2D npcSprite;

        // Define separate padding values
        private readonly int paddingTop = 16;
        private readonly int paddingBetween = 8;
        private readonly int paddingBottom = 16;
        private readonly int paddingSides = 16;
        private readonly int iconSize = 32;
        private readonly int iconSpacing = 8;

        // Define offset values for top-right positioning
        private readonly int offsetX = 16; // Pixels from the right edge
        private readonly int offsetY = 280; // Pixels from the top edge (increased to move it lower)

        private readonly int lineHeight;
        private readonly float spriteScale = 3f; // Fixed scaling factor for the NPC sprite

        private readonly Texture2D heartTexture;
        private readonly int maxHearts = 10; // Maximum number of hearts

        // Define separate values for heart icons
        private readonly int heartSourceWidth = 7;
        private readonly int heartScale = 4;
        private readonly int heartsPerRow = 5;
        private readonly int heartSpacing = 4; // Adjusted spacing for scaled hearts

        // **New: Offsets for Hearts**
        private readonly int heartXOffset = 20; // Move hearts 20 pixels to the right
        private readonly int heartYOffset = 10; // Move hearts 20 pixels down

        // **Existing: Y-Offset for NPC Sprite**
        private readonly int spriteYOffset = -30; // Negative value to move sprite up by 30 pixels

        public CustomMessageBox(string message, List<string> lovedGifts, IMonitor monitor, NPC npc)
            : base(0, 0, 0, 0, true)
        {
            this.message = message;
            this.lovedGifts = lovedGifts;
            this.monitor = monitor;
            this.npc = npc;

            // Load the NPC's sprite
            this.npcSprite = npc.Sprite.Texture;

            // Load the heart texture
            this.heartTexture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");

            // Calculate line height
            lineHeight = (int)Game1.smallFont.MeasureString("A").Y;

            // Split the message into lines
            string[] lines = this.message.Split('\n');
            int numberOfMessageLines = lines.Length;

            // Calculate the width based on the longest line
            float maxLineWidth = 0;
            foreach (var line in lines)
            {
                float lineWidth = Game1.smallFont.MeasureString(line).X;
                if (lineWidth > maxLineWidth)
                    maxLineWidth = lineWidth;
            }

            // Calculate the width needed for icons and text
            float giftsWidth = 0;
            foreach (var gift in lovedGifts)
            {
                float giftWidth = Game1.smallFont.MeasureString(gift).X + iconSize + iconSpacing;
                if (giftWidth > giftsWidth)
                    giftsWidth = giftWidth;
            }

            // Calculate hearts width based on rows and scaled size
            int heartWidth = heartSourceWidth * heartScale;
            int heartsWidth = heartsPerRow * (heartWidth + heartSpacing) + heartSpacing;

            // Calculate the combined width of NPC sprite and hearts if NPC sprite exists
            int combinedSpriteHeartsWidth = 0;
            if (this.npcSprite != null)
            {
                int spriteWidth = (int)(npc.Sprite.SourceRect.Width * spriteScale);
                combinedSpriteHeartsWidth = spriteWidth + heartSpacing + heartsWidth;
            }

            // Determine the maximum width required by any element or combination of elements
            int totalWidth = Math.Max((int)maxLineWidth, Math.Max((int)giftsWidth, combinedSpriteHeartsWidth));

            // Ensure totalWidth accounts for padding on both sides
            totalWidth += paddingSides * 2;

            // Set a minimum width if necessary
            int minWidth = 300;
            if (totalWidth < minWidth)
                totalWidth = minWidth;

            // Calculate height based on the number of message lines and gifts
            int totalHeight = paddingTop + (numberOfMessageLines * lineHeight) + paddingBetween + (lovedGifts.Count * (lineHeight + iconSpacing)) + paddingBottom;

            // Add height for the NPC sprite if it exists
            if (this.npcSprite != null)
            {
                int spriteHeight = (int)(npc.Sprite.SourceRect.Height * spriteScale);
                totalHeight += spriteHeight + paddingBetween;
            }

            // Set width and height
            this.width = totalWidth;
            this.height = totalHeight;

            // Position the message box at the top-right corner with offsets
            int screenWidth = Game1.uiViewport.Width;
            int screenHeight = Game1.uiViewport.Height;
            this.xPositionOnScreen = screenWidth - this.width - offsetX;
            this.yPositionOnScreen = offsetY;
        }

        public override void draw(SpriteBatch b)
        {
            // Draw the background box
            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

            // Calculate the starting positions
            float textStartX = this.xPositionOnScreen + paddingSides;
            float textStartY = this.yPositionOnScreen + paddingTop;

            // Split the message into lines
            string[] lines = this.message.Split('\n');

            // Draw the first line ("Today is ...")
            if (lines.Length > 0)
            {
                Vector2 position = new Vector2(textStartX, textStartY);
                b.DrawString(Game1.smallFont, lines[0], position, Color.Black);
            }

            // Draw the NPC sprite and hearts below the first line
            if (this.npcSprite != null && lines.Length > 1)
            {
                int spriteWidth = (int)(npc.Sprite.SourceRect.Width * spriteScale);
                int spriteHeight = (int)(npc.Sprite.SourceRect.Height * spriteScale);
                
                // Calculate sprite position with Y-Offset
                int spriteX = this.xPositionOnScreen + paddingSides;
                int spriteY = (int)(textStartY + lineHeight + paddingBetween + spriteYOffset); // **Applied Y-Offset**

                b.Draw(
                    this.npcSprite,
                    new Vector2(spriteX, spriteY),
                    new Rectangle?(this.npc.Sprite.SourceRect),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    SpriteEffects.None,
                    0.86f
                );

                // **Apply Offsets to Hearts**
                int heartX = spriteX + spriteWidth + heartSpacing + heartXOffset; // **Moved 20 pixels to the right**
                int heartY = spriteY + (spriteHeight / 2) - ((heartSourceWidth * heartScale * 2) + heartSpacing) / 2 + heartYOffset; // **Moved 20 pixels down**

                int affectionLevel = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);

                for (int i = 0; i < maxHearts; i++)
                {
                    int row = i / heartsPerRow; // Determine the row (0 or 1)
                    int col = i % heartsPerRow; // Determine the column (0 to 4)
                    Rectangle sourceRect = new Rectangle(211 + (i < affectionLevel ? 0 : 7), 428, 7, 6);
                    b.Draw(
                        this.heartTexture,
                        new Vector2(
                            heartX + col * (heartSourceWidth * heartScale + heartSpacing),
                            heartY + row * (heartSourceWidth * heartScale + heartSpacing)
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

                // Adjust textStartY to be below the sprite and hearts
                textStartY = spriteY + spriteHeight + paddingBetween;
            }

            // Draw the remaining lines ("They love:")
            for (int i = 1; i < lines.Length; i++)
            {
                Vector2 position = new Vector2(textStartX, textStartY + (i - 1) * lineHeight);
                b.DrawString(Game1.smallFont, lines[i], position, Color.Black);
            }

            // Adjust textStartY to be below the "They love:" line
            textStartY += (lines.Length - 1) * lineHeight + paddingBetween;

            // Draw the loved gifts with icons
            for (int i = 0; i < lovedGifts.Count; i++)
            {
                string giftName = lovedGifts[i];
                int itemId = GetItemId(giftName);
                if (itemId != -1)
                {
                    // Calculate Y position for each gift
                    int yPosition = (int)(textStartY + i * (lineHeight + iconSpacing));

                    // Define the X position for the icon
                    float iconX = textStartX;

                    // Draw the item icon
                    Texture2D texture = Game1.objectSpriteSheet;
                    Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(texture, itemId, 16, 16);
                    Vector2 iconPosition = new Vector2(iconX, yPosition);
                    b.Draw(texture, iconPosition, sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);

                    // Draw the item name next to the icon
                    Vector2 textPosition = new Vector2(iconPosition.X + iconSize + iconSpacing, yPosition);
                    b.DrawString(Game1.smallFont, giftName, textPosition, Color.Black);
                }
                else
                {
                    // If item ID not found, just draw the name without icon
                    int yPosition = (int)(textStartY + i * (lineHeight + iconSpacing));
                    Vector2 textPosition = new Vector2(textStartX, yPosition);
                    b.DrawString(Game1.smallFont, giftName, textPosition, Color.Black);
                }
            }

            // Draw the mouse cursor
            this.drawMouse(b);
        }

        private int GetItemId(string itemName)
        {
            // This method returns the item ID based on the item name
            // You can expand this method to include more items
            switch (itemName)
            {
                case "Coconut": return 88;
                case "Sunflower": return 421;
                case "Fruit Salad": return 610;
                case "Pink Cake": return 221;
                case "Amethyst": return 66;
                case "Banana Pudding": return 904;
                case "Pumpkin": return 276;
                case "Blackberry Cobbler": return 611;
                case "Chocolate Cake": return 220;
                case "Spicy Eel": return 226;
                case "Pufferfish": return 128;
                case "Aquamarine": return 62;
                case "Cloth": return 428;
                case "Emerald": return 60;
                case "Jade": return 70;
                case "Parrot Egg": return 832; // Corrected item ID
                case "Ruby": return 64;
                case "Survival Burger": return 241;
                case "Topaz": return 68;
                case "Wool": return 440;
                case "Goat Cheese": return 426;
                case "Poppyseed Muffin": return 651;
                case "Salad": return 196;
                case "Stir Fry": return 606;
                case "Truffle": return 430;
                case "Vegetable Medley": return 200;
                case "Wine": return 348;
                case "Battery Pack": return 787;
                case "Cauliflower": return 190;
                case "Cheese Cauliflower": return 197;
                case "Diamond": return 72;
                case "Gold Bar": return 336;
                case "Iridium Bar": return 337;
                case "Miner's Treat": return 243;
                case "Pepper Poppers": return 215;
                case "Radioactive Bar": return 910;
                case "Rhubarb Pie": return 222;
                case "Strawberry": return 400;
                case "Melon": return 254;
                case "Poppy": return 376;
                case "Red Plate": return 230;
                case "Roots Platter": return 244;
                case "Sandfish": return 164;
                case "Tom Kha Soup": return 218;
                case "Complete Breakfast": return 201;
                case "Salmon Dinner": return 212;
                case "Crab Cakes": return 732;
                case "Duck Feather": return 444;
                case "Lobster": return 715;
                case "Pomegranate": return 637;
                case "Squid Ink": return 814;
                case "Coffee": return 395;
                case "Pickles": return 342;
                case "Super Meal": return 237;
                case "Truffle Oil": return 432;
                case "Cactus Fruit": return 90;
                case "Maple Bar": return 731;
                case "Pizza": return 206;
                case "Tigerseye": return 562;
                case "Frog Egg": return 828; // Corrected item ID
                case "Frozen Tear": return 84;
                case "Obsidian": return 575;
                case "Pumpkin Soup": return 236;
                case "Sashimi": return 227;
                case "Void Egg": return 305;
                case "Beer": return 346;
                case "Hot Pepper": return 260;
                case "Fish Taco": return 213;
                case "Green Tea": return 614;
                case "Summer Spangle": return 593;
                case "Tropical Curry": return 907;
                case "Artichoke Dip": return 605;
                case "Omni Geode": return 749;
                case "Bean Hotpot": return 207;
                case "Ice Cream": return 233;
                case "Rice Pudding": return 232;
                case "Beet": return 284;
                case "Fairy Rose": return 595;
                case "Stuffing": return 239;
                case "Tulip": return 591;
                case "Fried Mushroom": return 205;
                case "Leek": return 20;
                case "Escargot": return 728;
                case "Orange": return 635;
                case "Ancient Doll": return 103;
                case "Fairy Box": return 1021; // Assuming custom item ID
                case "Plum Pudding": return 604;
                case "Crispy Bass": return 214;
                case "Eggplant Parmesan": return 231;
                case "Fried Eel": return 225;
                case "Pancakes": return 211;
                case "Roasted Hazelnuts": return 607;
                case "Autumn's Bounty": return 235;
                case "Glazed Yams": return 208;
                case "Dish o' The Sea": return 242;
                case "Yam": return 280;
                case "Farmer's Lunch": return 240;
                case "Pumpkin Pie": return 608;
                case "Mead": return 459;
                case "Pale Ale": return 303;
                case "Parsnip": return 24;
                case "Parsnip Soup": return 199;
                case "Pina Colada": return 873;
                case "Fried Calamari": return 202;
                case "Peach": return 636;
                case "Spaghetti": return 224;
                case "Cranberry Candy": return 612;
                case "Ginger Ale": return 903;
                case "Grape": return 398;
                case "Snail": return 720;
                case "Catfish": return 143;
                case "Octopus": return 149;
                case "Sea Cucumber": return 154;
                case "Sturgeon": return 698;
                case "Lava Eel": return 162;
                case "Lemon Stone": return 554;
                case "Monster Musk": return 879;
                case "Wild Horseradish": return 16;
                case "Mango": return 834;
                case "Ostrich Egg": return 289;
                case "Poi": return 906;
                case "Crocus": return 418;
                case "Daffodil": return 18;
                case "Mango Sticky Rice": return 905;
                case "Sweet Pea": return 402;
                case "Purple Mushroom": return 422;
                case "Solar Essence": return 768;
                case "Super Cucumber": return 155;
                case "Void Essence": return 769;
                case "Void Mayonnaise": return 308;

                default:
                    this.monitor.Log($"Item ID for '{itemName}' not found.", LogLevel.Warn);
                    return -1;
            }
        }
    }
}
