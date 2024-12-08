using System.Collections.Generic;

namespace BirthdayReminderMod
{
    public class NPCDataProvider
    {
        public Dictionary<string, NPCInfo> GetNPCData()
        {
            return new Dictionary<string, NPCInfo>
            {
                // Spring
                { "Kent", new NPCInfo("Kent", "spring 4", new List<string> { "Fiddlehead Risotto", "Roasted Hazelnuts" }) },
                { "Lewis", new NPCInfo("Lewis", "spring 7", new List<string> { "Autumn's Bounty", "Glazed Yams", "Green Tea", "Hot Pepper", "Vegetable Medley" }) },
                { "Vincent", new NPCInfo("Vincent", "spring 10", new List<string> { "Cranberry Candy", "Frog Egg", "Ginger Ale", "Grape", "Pink Cake", "Snail" }) },
                { "Haley", new NPCInfo("Haley", "spring 14", new List<string> { "Coconut", "Sunflower", "Fruit Salad", "Pink Cake" }) },
                { "Pam", new NPCInfo("Pam", "spring 18", new List<string> { "Beer", "Cactus Fruit", "Glazed Yams", "Mead", "Pale Ale", "Parsnip", "Parsnip Soup", "Pina Colada" }) },
                { "Shane", new NPCInfo("Shane", "spring 20", new List<string> { "Beer", "Hot Pepper", "Pepper Poppers", "Pizza" }) },
                { "Pierre", new NPCInfo("Pierre", "spring 26", new List<string> { "Fried Calamari" }) },
                { "Emily", new NPCInfo("Emily", "spring 27", new List<string> { "Amethyst", "Aquamarine", "Cloth", "Emerald", "Jade", "Parrot Egg", "Ruby", "Survival Burger", "Topaz", "Wool" }) },

                // Summer
                { "Jas", new NPCInfo("Jas", "summer 4", new List<string> { "Ancient Doll", "Fairy Box", "Fairy Rose", "Pink Cake", "Plum Pudding" }) },
                { "Gus", new NPCInfo("Gus", "summer 8", new List<string> { "Diamond", "Escargot", "Fish Taco", "Orange", "Tropical Curry" }) },
                { "Maru", new NPCInfo("Maru", "summer 10", new List<string> { "Battery Pack", "Cauliflower", "Cheese Cauliflower", "Diamond", "Gold Bar", "Iridium Bar", "Miner's Treat", "Pepper Poppers", "Radioactive Bar", "Rhubarb Pie", "Strawberry" }) },
                { "Alex", new NPCInfo("Alex", "summer 13", new List<string> { "Complete Breakfast", "Salmon Dinner" }) },
                { "Sam", new NPCInfo("Sam", "summer 17", new List<string> { "Cactus Fruit", "Maple Bar", "Pizza", "Tigerseye" }) },
                { "Demetrius", new NPCInfo("Demetrius", "summer 19", new List<string> { "Bean Hotpot", "Ice Cream", "Rice Pudding", "Strawberry" }) },
                { "Dwarf", new NPCInfo("Dwarf", "summer 22", new List<string> { "Amethyst", "Aquamarine", "Emerald", "Jade", "Lava Eel", "Lemon Stone", "Omni Geode", "Ruby", "Topaz" }) },
                { "Willy", new NPCInfo("Willy", "summer 24", new List<string> { "Catfish", "Diamond", "Gold Bar", "Iridium Bar", "Mead", "Octopus", "Pumpkin", "Sea Cucumber", "Sturgeon" }) },
                { "Leo", new NPCInfo("Leo", "summer 26", new List<string> { "Duck Feather", "Mango", "Ostrich Egg", "Parrot Egg", "Poi" }) },
                
                // Fall
                { "Penny", new NPCInfo("Penny", "fall 2", new List<string> { "Diamond", "Emerald", "Melon", "Poppy", "Poppyseed Muffin", "Red Plate", "Roots Platter", "Sandfish", "Tom Kha Soup" }) },
                { "Elliott", new NPCInfo("Elliott", "fall 5", new List<string> { "Crab Cakes", "Duck Feather", "Lobster", "Pomegranate", "Squid Ink", "Tom Kha Soup" }) },
                { "Jodi", new NPCInfo("Jodi", "fall 11", new List<string> { "Chocolate Cake", "Crispy Bass", "Diamond", "Eggplant Parmesan", "Fried Eel", "Pancakes", "Rhubarb Pie", "Vegetable Medley" }) },
                { "Abigail", new NPCInfo("Abigail", "fall 13", new List<string> { "Amethyst", "Banana Pudding", "Pumpkin", "Blackberry Cobbler", "Chocolate Cake", "Spicy Eel", "Pufferfish" }) },
                { "Sandy", new NPCInfo("Sandy", "fall 15", new List<string> { "Crocus", "Daffodil", "Mango Sticky Rice", "Sweet Pea" }) },
                { "Marnie", new NPCInfo("Marnie", "fall 18", new List<string> { "Diamond", "Farmer's Lunch", "Pink Cake", "Pumpkin Pie" }) },
                { "Robin", new NPCInfo("Robin", "fall 21", new List<string> { "Goat Cheese", "Peach", "Spaghetti" }) },
                { "George", new NPCInfo("George", "fall 24", new List<string> { "Fried Mushroom", "Leek" }) },

                // Winter
                { "Krobus", new NPCInfo("Krobus", "winter 1", new List<string> { "Diamond", "Iridium Bar", "Monster Musk", "Pumpkin", "Void Egg", "Void Mayonnaise", "Wild Horseradish" }) },
                { "Linus", new NPCInfo("Linus", "winter 3", new List<string> { "Cactus Fruit", "Coconut", "Dish o' The Sea", "Yam" }) },
                { "Caroline", new NPCInfo("Caroline", "winter 7", new List<string> { "Fish Taco", "Green Tea", "Summer Spangle", "Tropical Curry" }) },
                { "Sebastian", new NPCInfo("Sebastian", "winter 10", new List<string> { "Frog Egg", "Frozen Tear", "Obsidian", "Pumpkin Soup", "Sashimi", "Void Egg" }) },
                { "Harvey", new NPCInfo("Harvey", "winter 14", new List<string> { "Coffee", "Pickles", "Super Meal", "Truffle Oil", "Wine" }) },
                { "Wizard", new NPCInfo("Wizard", "winter 17", new List<string> { "Purple Mushroom", "Solar Essence", "Super Cucumber", "Void Essence" }) },
                { "Evelyn", new NPCInfo("Evelyn", "winter 20", new List<string> { "Beet", "Chocolate Cake", "Diamond", "Fairy Rose", "Stuffing", "Tulip" }) },
                { "Leah", new NPCInfo("Leah", "winter 23", new List<string> { "Goat Cheese", "Poppyseed Muffin", "Salad", "Stir Fry", "Truffle", "Vegetable Medley", "Wine" }) },
                { "Clint", new NPCInfo("Clint", "winter 26", new List<string> { "Amethyst", "Aquamarine", "Artichoke Dip", "Emerald", "Gold Bar", "Iridium Bar", "Jade", "Ruby", "Omni Geode", "Topaz" }) }
            };
        }
    }
}
