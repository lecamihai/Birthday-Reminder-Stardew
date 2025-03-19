// NPCDataProvider.cs
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
                { "Kent", new NPCInfo("Kent", "spring", 4, new List<string> { "607", "649" }) },
                { "Lewis", new NPCInfo("Lewis", "spring", 7, new List<string> { "200", "208", "235", "614", "260" }) },
                { "Vincent", new NPCInfo("Vincent", "spring", 10, new List<string> { "221", "398", "612", "721", "903"}) },
                { "Haley", new NPCInfo("Haley", "spring", 14, new List<string> { "221", "421", "610", "88" }) },
                { "Pam", new NPCInfo("Pam", "spring", 18, new List<string> { "24", "90", "199", "208", "303", "346", "459", "873" }) },
                { "Shane", new NPCInfo("Shane", "spring", 20, new List<string> { "206", "215", "260", "346" }) },
                { "Pierre", new NPCInfo("Pierre", "spring", 26, new List<string> { "202", "Book_PriceCatalogue" }) },
                { "Emily", new NPCInfo("Emily", "spring", 27, new List<string> { "60", "62", "64", "66", "68", "70", "241", "428", "440"}) },

                // Summer
                { "Jas", new NPCInfo("Jas", "summer", 4, new List<string> { "221", "595", "604", "103" }) },
                { "Gus", new NPCInfo("Gus", "summer", 8, new List<string> { "72", "213", "635", "729", "907" }) },
                { "Maru", new NPCInfo("Maru", "summer", 10, new List<string> { "72", "197", "190", "215", "222", "243", "336", "337", "400", "787", "910", "122" }) },
                { "Alex", new NPCInfo("Alex", "summer", 13, new List<string> { "201", "212", "Book_Defense" }) },
                { "Sam", new NPCInfo("Sam", "summer", 17, new List<string> { "90", "206", "562", "731" }) },
                { "Demetrius", new NPCInfo("Demetrius", "summer", 19, new List<string> { "207", "232", "233", "400" }) },
                { "Dwarf", new NPCInfo("Dwarf", "summer", 22, new List<string> { "554", "60", "62", "64", "66", "68", "70", "749", "162" }) },
                { "Willy", new NPCInfo("Willy", "summer", 24, new List<string> { "72", "143", "149", "154", "276", "337", "698", "459", "336", "Book_Crabbing", "Book_Roe" }) },
                { "Leo", new NPCInfo("Leo", "summer", 26, new List<string> { "444", "289", "834", "906"}) },

                // Fall
                { "Penny", new NPCInfo("Penny", "fall", 2, new List<string> { "60", "376", "651", "72", "164", "218", "230", "244", "254"}) },
                { "Elliott", new NPCInfo("Elliott", "fall", 5, new List<string> { "715", "732", "218", "444", "637", "814" }) },
                { "Jodi", new NPCInfo("Jodi", "fall", 11, new List<string> { "72", "200", "211", "214", "220", "222", "225", "231" }) },
                { "Abigail", new NPCInfo("Abigail", "fall", 13, new List<string> { "66", "128", "220", "226", "276", "611", "904", "Book_Void" }, 
                    new Dictionary<string, List<ScheduleSlot>>
                    {   
                        // First the Rain Condition
                        // Rainy day schedule (Option 1)
                        { "rain", new List<ScheduleSlot>
                            {
                                new ScheduleSlot(900, 1100, "In her room."),
                                new ScheduleSlot(1100, 1300, "Leaves her room to stand in Pierre's General Store."),
                                new ScheduleSlot(1300, 1500, "Goes to the kitchen."),
                                new ScheduleSlot(1500, 2200, "Returns to her room from the kitchen."),
                                new ScheduleSlot(2200, 2600, "Goes to Bed.")
                            }
                        },
                        // Green Rain schedule ( Option 2 )
                        { "GreenRain", new List<ScheduleSlot>
                            {
                                new ScheduleSlot(900, 1100, "Leaving her room to go to the kitchen."),
                                new ScheduleSlot(1100, 1400, "Leaves the kitchen to stand in Pierre's General Store."),
                                new ScheduleSlot(1400, 1450, "Leaving the house to go to the Stardrop Saloon."),
                                new ScheduleSlot(1450, 2100, "In the Stardrop Saloon, sitting on a sofa in the arcade."),
                                new ScheduleSlot(2100, 2200, "Heads home."),
                                new ScheduleSlot(2200, 2600, "Goes to Bed")
                            }
                        },
                         // Fall 11 and 25 (Player has 6 hearts with Sebastian or Abigail)
                         // Default fall schedule (used on her birthday)
                        { "fall", new List<ScheduleSlot>
                            {
                                new ScheduleSlot(900, 1030, "Leaves her room to go to the kitchen."),
                                new ScheduleSlot(1030, 1300, "Leaves the kitchen to stand in Pierre's store"),
                                new ScheduleSlot(1300, 1330, "Standing on the bridge near JojaMart."),
                                new ScheduleSlot(1330, 1630, "Heads home."),
                                new ScheduleSlot(1630, 1720, "In her room, playing video games."),
                                new ScheduleSlot(1930, 2600, "Goes to Bed")
                            }
                        },
                        // Abigail has love interest in Sebastian that is what bellow is
                        // Not yet integrated
                        // Fall 11 and 25 (No player has 6 hearts with Sebastian or Abigail)
                        { "fall_but_no_hearts_to_either", new List<ScheduleSlot>
                            {
                                new ScheduleSlot(900, 1030, "In bed."),
                                new ScheduleSlot(1030, 1200, "Leaves Pierre's store."),
                                new ScheduleSlot(1200, 1700, "Arrives in Sebastian’s room."),
                                new ScheduleSlot(1700, 1830, "Leaves Sebastian’s room."),
                                new ScheduleSlot(1830, 2200, "Returns home."),
                                new ScheduleSlot(2200, 2600, "Back in room."),
                                new ScheduleSlot(1930, 2600, "Goes to Bed")
                            }
                        },
                    }) 
                },
                { "Sandy", new NPCInfo("Sandy", "fall", 15, new List<string> { "18", "402", "418", "905" }) },
                { "Marnie", new NPCInfo("Marnie", "fall", 18, new List<string> { "72", "221", "240", "608" }) },
                { "Robin", new NPCInfo("Robin", "fall", 21, new List<string> { "224", "426", "636", "Book_Woodcutting" }) },
                { "George", new NPCInfo("George", "fall", 24, new List<string> { "20", "205" }) },

                // Winter
                { "Krobus", new NPCInfo("Krobus", "winter", 1, new List<string> { "72", "16", "276", "337", "305", "308", "879", "Book_Void" }) },
                { "Linus", new NPCInfo("Linus", "winter", 3, new List<string> { "88", "90", "234", "242", "280", "Book_Trash" }) },
                { "Caroline", new NPCInfo("Caroline", "winter", 7, new List<string> { "213", "614", "593", "907" }) },
                { "Sebastian", new NPCInfo("Sebastian", "winter", 10, new List<string> { "84", "227", "236", "575", "305"}) },
                { "Harvey", new NPCInfo("Harvey", "winter", 14, new List<string> { "348", "237", "432", "395", "342" }) },
                { "Wizard", new NPCInfo("Wizard", "winter", 17, new List<string> { "155", "422", "769", "768", "Book_Mystery" }) },
                { "Evelyn", new NPCInfo("Evelyn", "winter", 20, new List<string> { "72", "220", "239", "284", "591", "595", "Raisins" }) },
                { "Leah", new NPCInfo("Leah", "winter", 23, new List<string> { "196", "200", "348", "606", "651", "426", "430" }) },
                { "Clint", new NPCInfo("Clint", "winter", 26, new List<string> { "60", "62", "64", "66", "68", "70", "336", "337", "605", "649", "749" }) }
            };
        }
    }
}