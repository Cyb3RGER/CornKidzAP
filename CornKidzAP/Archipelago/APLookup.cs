using System.Collections.Generic;
using System.Linq;

namespace CornKidzAP.Archipelago;

public static class APLookup
{
    public static Dictionary<TKey, TValue> InvertDictionary<TKey, TValue>(Dictionary<TValue, TKey> dictionary)
    {
        return dictionary.ToDictionary(pair => pair.Value, pair => pair.Key);
    }

    public const long BaseID = 3116411600;

    public static readonly Dictionary<long, int> APLocIdToSaveItemId = new()
    {
        { BaseID + 0, 116 }, //Park Outside: XP Cube Near Slide
        { BaseID + 1, 100 }, //Park Outside: XP Cube On Slide
        { BaseID + 2, 118 }, //Park Outside: XP Cube Under Slide
        { BaseID + 3, 113 }, //Park Outside: XP Cube Bouncy Moose Near Alexis
        { BaseID + 4, 106 }, //Park Outside: XP Cube Above Top Hat
        { BaseID + 5, 101 }, //Park Outside: XP Cube Underwater #2
        { BaseID + 6, 102 }, //Park Outside: XP Cube Underwater #1
        { BaseID + 7, 103 }, //Park Outside: XP Cube Behind Cracked Wall 2
        { BaseID + 8, 104 }, //Park Outside: XP Cube Behind Cracked Wall 1
        { BaseID + 9, 119 }, //Park Outside: XP Cube Inside Boulder
        { BaseID + 10, 105 }, //Park Outside: XP Cube Above Boost Pad
        { BaseID + 11, 122 }, //Park Outside: XP Cube From Stone Statue 1
        { BaseID + 12, 123 }, //Park Outside: XP Cube From Stone Statue 2
        { BaseID + 13, 124 }, //Park Outside: XP Cube From Stone Statue 3
        { BaseID + 14, 112 }, //Park Outside: XP Cube Near Upper Fence
        { BaseID + 15, 120 }, //Park Outside: XP Cube Behind Slam Pillar
        { BaseID + 16, 121 }, //Park Outside: XP Cube Above Slam Pillar
        { BaseID + 17, 126 }, //Park Attic: XP Cube Behind Dirt 1
        { BaseID + 18, 127 }, //Park Attic: XP Cube Behind Dirt 2
        { BaseID + 19, 128 }, //Park Attic: XP Cube On Top Ledge 1
        { BaseID + 20, 130 }, //Park Attic: XP Cube On Top Ledge 2
        { BaseID + 21, 129 }, //Park Attic: XP Cube Slam Block Hallway
        { BaseID + 22, 117 }, //Park Inside: XP Cube Behind Bookcase
        { BaseID + 23, 107 }, //Park Inside: XP Cube On Wall Platform
        { BaseID + 24, 115 }, //Park Inside: XP Cube In Cage
        { BaseID + 25, 114 }, //Park Inside: XP Cube On Pole
        { BaseID + 26, 131 }, //Park Inside: XP Cube In Leaf Tunnel
        { BaseID + 27, 125 }, //Park Outside: XP Cube Bouncy Moose On Roof
        { BaseID + 28, 109 }, //Park Outside: XP Cube Above Boost Rings 1
        { BaseID + 29, 110 }, //Park Outside: XP Cube Above Boost Rings 2
        { BaseID + 30, 111 }, //Park Outside: XP Cube Above Boost Rings 3
        { BaseID + 31, 108 }, //Park Outside: XP Cube Behind Cracked Wall 3
        { BaseID + 32, 135 }, //Park Outside: Red Screw Under Boulder
        { BaseID + 33, 137 }, //Park Attic: Red Screw Near Trash Can 1
        { BaseID + 34, 136 }, //Park Attic: Red Screw At Top
        { BaseID + 35, 134 }, //Park Outside: Red Screw Above Inside Entrance
        { BaseID + 36, 138 }, //Park Inside: Red Screw Behind Loose Wall Panel
        { BaseID + 37, 133 }, //Park Outside: Red Screw Across Jump Pads
        { BaseID + 38, 139 }, //Park Outside: Chameleon Moth Bush
        { BaseID + 39, 140 }, //Park Inside: Chameleon Moth Painting
        { BaseID + 40, 143 }, //Park Attic: XP Crystal
        { BaseID + 41, 144 }, //Park Outside: XP Crystal From Garbage Grump
        { BaseID + 42, 142 }, //Park Outside: Crazy Mirror - Climb To The Top
        { BaseID + 43, 141 }, //Park Inside: Crazy Mirror - Bounce To The Top
        { BaseID + 44, 132 }, //Park Outside: Crank In Chest
        { BaseID + 45, 152 }, //Park Outside: Trash Can Near Top Hat
        { BaseID + 46, 153 }, //Park Attic: Trash Can 1
        { BaseID + 47, 151 }, //Park Attic: Trash Can 2
        { BaseID + 48, 148 }, //Park Outside: Trash Can Near Garbage Grump
        { BaseID + 49, 155 }, //Park Inside: Trash Can Near XP Cube In Cage
        { BaseID + 50, 150 }, //Park Outside: Trash Can On Roof
        { BaseID + 51, 149 }, //Park Outside: Trash Can Across Jump Pads
        { BaseID + 52, 154 }, //Park Inside: Trash Can Behind Level 3 Door
        { BaseID + 53, 254 }, //Hollow: XP Cube Bridge 1
        { BaseID + 54, 275 }, //Hollow: XP Cube Bridge 2
        { BaseID + 55, 276 }, //Hollow: XP Cube Bridge 3
        { BaseID + 56, 209 }, //Hollow: XP Cube On Trees
        { BaseID + 57, 225 }, //Hollow: XP Cube By Owlloh 1
        { BaseID + 58, 226 }, //Hollow: XP Cube By Owlloh 2
        { BaseID + 59, 216 }, //Hollow: XP Cube Pillar By Music Box 1
        { BaseID + 60, 217 }, //Hollow: XP Cube Pillar By Music Box 2
        { BaseID + 61, 266 }, //Hollow: XP Cube Above Bouncy Leaf
        { BaseID + 62, 232 }, //Hollow: XP Cube In Trinket Shop
        { BaseID + 63, 231 }, //Hollow: XP Cube On Ledge By Clock 1
        { BaseID + 64, 230 }, //Hollow: XP Cube On Ledge By Clock 2
        { BaseID + 65, 224 }, //Hollow: XP Cube On Pillar Outside Church
        { BaseID + 66, 265 }, //Hollow: XP Cube Merry-Go-Round Pig
        { BaseID + 67, 215 }, //Hollow: XP Cube Gas Tunnel 1
        { BaseID + 68, 214 }, //Hollow: XP Cube Gas Tunnel 2
        { BaseID + 69, 233 }, //Hollow: XP Cube On Pole In Church
        { BaseID + 70, 255 }, //Hollow: XP Cube Inside Church Window 1
        { BaseID + 71, 271 }, //Hollow: XP Cube Inside Church Window 2
        { BaseID + 72, 273 }, //Hollow Graveyard: XP Cube On Pole 1
        { BaseID + 73, 261 }, //Hollow Graveyard: XP Cube On Snippins Tree
        { BaseID + 74, 274 }, //Hollow Graveyard: XP Cube On Pole 2
        { BaseID + 75, 253 }, //Hollow Graveyard: XP Cube Above Fish
        { BaseID + 76, 256 }, //Hollow Graveyard: XP Cube Underwater By Fish
        { BaseID + 77, 262 }, //Hollow Graveyard: XP Cube Tombstone Code 1 (12311321)
        { BaseID + 78, 263 }, //Hollow Graveyard: XP Cube Tombstone Code 2 (23111323)
        { BaseID + 79, 264 }, //Hollow Graveyard: XP Cube Tombstone Code 3 (21132113)
        { BaseID + 80, 210 }, //Hollow: XP Cube Balcony By Climbable Tree 1
        { BaseID + 81, 211 }, //Hollow: XP Cube Balcony By Climbable Tree 2
        { BaseID + 82, 218 }, //Hollow: XP Cube On Bat Tower 1
        { BaseID + 83, 219 }, //Hollow: XP Cube On Bat Tower 2
        { BaseID + 84, 242 }, //Hollow: XP Cube Inside Drill Tower 1
        { BaseID + 85, 244 }, //Hollow: XP Cube Inside Drill Tower 2
        { BaseID + 86, 246 }, //Hollow: XP Cube Inside Drill Tower 3
        { BaseID + 87, 248 }, //Hollow: XP Cube Under Ramp By Music Box
        { BaseID + 88, 220 }, //Hollow: XP Cube Behind Bars 1
        { BaseID + 89, 221 }, //Hollow: XP Cube Behind Bars 2
        { BaseID + 90, 236 }, //Hollow: XP Cube Under Church Stairs 1
        { BaseID + 91, 238 }, //Hollow: XP Cube Under Church Stairs 2
        { BaseID + 92, 212 }, //Hollow Graveyard: XP Cube In Dirt Below Dragon 1
        { BaseID + 93, 213 }, //Hollow Graveyard: XP Cube In Dirt Below Dragon 2
        { BaseID + 94, 222 }, //Hollow Graveyard: XP Cube On House Roof 1
        { BaseID + 95, 223 }, //Hollow Graveyard: XP Cube On House Roof 2
        { BaseID + 96, 272 }, //Hollow Graveyard: XP Cube Inside House Behind Chimney
        { BaseID + 97, 229 }, //Hollow Ravine: XP Cube Between Wallgrab Points
        { BaseID + 98, 227 }, //Hollow Ravine: XP Cube On Vine 1
        { BaseID + 99, 228 }, //Hollow Ravine: XP Cube On Vine 2
        { BaseID + 100, 277 }, //Hollow: XP Cube In Box Across Ravine
        { BaseID + 101, 278 }, //Hollow: XP Cube In Music Box 1
        { BaseID + 102, 279 }, //Hollow: XP Cube In Music Box 2
        { BaseID + 103, 234 }, //Hollow: XP Cube On Wall By Merry-Go-Round 1
        { BaseID + 104, 235 }, //Hollow: XP Cube On Wall By Merry-Go-Round 2
        { BaseID + 105, 240 }, //Hollow: XP Cube Behind Sanitary Zoo
        { BaseID + 106, 252 }, //Hollow: XP Cube On Tree Near Zoo
        { BaseID + 107, 250 }, //Hollow Reversed: XP Cube Under Dragon Elevator
        { BaseID + 108, 267 }, //Hollow Reversed: XP Cube In Fountain 1
        { BaseID + 109, 268 }, //Hollow Reversed: XP Cube In Fountain 2
        { BaseID + 110, 269 }, //Hollow Reversed: XP Cube In Fountain 3
        { BaseID + 111, 270 }, //Hollow Reversed: XP Cube In Fountain 4
        { BaseID + 112, 257 }, //Hollow: Inside The Tree XP Cube Fish Timer
        { BaseID + 113, 259 }, //Hollow: Inside The Tree XP Cube Under Tree
        { BaseID + 114, 258 }, //Hollow: Inside The Tree XP Cube Near Metal Worm
        { BaseID + 115, 260 }, //Hollow Graveyard: XP Cube Above Ma Spider Coffin
        { BaseID + 116, 203 }, //Hollow: Red Screw Above Clock
        { BaseID + 117, 200 }, //Hollow: Red Screw In Alcove Under Drill Tower Entrance
        { BaseID + 118, 202 }, //Hollow Graveyard: Red Screw Behind House
        { BaseID + 119, 208 }, //Hollow Graveyard: Red Screw In Dragon Cave
        { BaseID + 120, 201 }, //Hollow: Red Screw Near Rotatable Pillar
        { BaseID + 121, 207 }, //Hollow Zoo: Red Screw
        { BaseID + 122, 204 }, //Hollow Reversed: Red Screw Inside The Tree Side Entrance
        { BaseID + 123, 205 }, //Hollow: Inside The Tree Red Screw By Snippins Stump
        { BaseID + 124, 206 }, //Hollow: Inside The Tree Red Screw Above Bridge
        { BaseID + 125, 297 }, //Hollow: Chameleon Moth Bush
        { BaseID + 126, 296 }, //Hollow Graveyard: Chameleon Moth Gravestone
        { BaseID + 127, 283 }, //Hollow: XP Crystal Above Fountain
        { BaseID + 128, 282 }, //Hollow: XP Crystal On Church Roof
        { BaseID + 129, 291 }, //Hollow Graveyard: XP Crystal Reward From Ipsam Jr.
        { BaseID + 130, 286 }, //Hollow: XP Crystal In Cage Near Music Box
        { BaseID + 131, 285 }, //Hollow Graveyard: XP Crystal In House 1st Floor Behind Painting
        { BaseID + 132, 295 }, //Hollow Graveyard: XP Crystal From Dragon
        { BaseID + 133, 280 }, //Hollow: XP Crystal Above Entrance
        { BaseID + 134, 292 }, //Hollow: XP Crystal In Zombie Chamber
        { BaseID + 135, 294 }, //Hollow: XP Crystal Pig Stuck In Owl Pellet
        { BaseID + 136, 281 }, //Hollow: XP Crystal In Cracked Pillar Outside Church
        { BaseID + 137, 293 }, //Hollow Reversed: XP Crystal Merry-Go-Round Pig
        { BaseID + 138, 287 }, //Hollow Reversed: XP Crystal Behind Bluwinkle In Trinket Shop
        { BaseID + 139, 289 }, //Hollow Graveyard: Crazy Mirror - Catch The Cans
        { BaseID + 140, 284 }, //Hollow: Crazy Mirror - Dig To The Top
        { BaseID + 141, 288 }, //Hollow Reversed: Crazy Mirror - Light The Cubes!
        { BaseID + 142, 290 }, //Hollow Zoo: Crazy Mirror - Land Safely
        { BaseID + 143, 298 }, //Hollow Graveyard: Crank In House 1st Floor
        { BaseID + 144, 299 }, //Hollow Ravine: Crank
        { BaseID + 145, 306 }, //Hollow: Disco Ball In Trinket Shop
        { BaseID + 146, 300 }, //Hollow: Disco Ball Above Dragon
        { BaseID + 147, 308 }, //Hollow: Disco Ball Above Music Box
        { BaseID + 148, 302 }, //Hollow: Disco Ball In Zombie Chamber
        { BaseID + 149, 304 }, //Hollow: Disco Ball Clean Zoo
        { BaseID + 150, 319 }, //Hollow Graveyard: Bottle Cap On Ma Spider Gravestone
        { BaseID + 151, 317 }, //Hollow Graveyard: Bottle Cap In Fish Tree Stump
        { BaseID + 152, 311 }, //Hollow Ravine: Bottle Cap
        { BaseID + 153, 313 }, //Hollow: Bottle Cap On Zoo Roof
        { BaseID + 154, 315 }, //Hollow: Bottle Cap In Cage Near Zoo
        { BaseID + 155, 146 }, //Park Inside: Void Screw Near Bookcase
        { BaseID + 156, 324 }, //Hollow: Void Screw Outside Trinket Shop
        { BaseID + 157, 321 }, //Hollow: Void Screw At Top Of Church
        { BaseID + 158, 323 }, //Hollow: Void Screw In Sky
        { BaseID + 159, 501 }, //Some Other Place: Void Screw
        { BaseID + 160, 322 }, //Hollow: Inside The Tree Void Screw
        { BaseID + 161, 325 }, //Hollow: Void Screw In Zombie Chamber All Fish Killed
        { BaseID + 162, 145 }, //Park Outside: Void Screw Under Top Hat
        { BaseID + 163, 147 }, //Park Outside: Void Screw Out Of Bounds
        { BaseID + 164, 411 }, //Anxiety Tower: Void Screw 1
        { BaseID + 165, 412 }, //Anxiety Tower: Void Screw 2
        { BaseID + 166, 414 }, //Anxiety Tower: Crank
        { BaseID + 167, 326 }, //Hollow: Mega Dream Soda
        { BaseID + 168, 500 }, //Some Other Place: Mega Dream Soda
        { BaseID + 171, 310 }, //Hollow Graveyard: Cheese Grater In Ma Spider Grave
    };

    public static readonly Dictionary<long, int> APLocIdToUpgradeId = new()
    {
        { BaseID + 169, 1 }, //Hollow: Drill
        { BaseID + 170, 2 }, //Hollow: Fall Warp
    };

    public static readonly Dictionary<long, int> APLocIdToAchievementId = new()
    {
        { BaseID + 182, 1 }, //Achievement: Little Corn Cadet
        { BaseID + 183, 2 }, //Achievement: XP-ansion Pak
        { BaseID + 184, 3 }, //Achievement: maXPower
        { BaseID + 185, 4 }, //Achievement: ...I'm a Lasagna Hog
        { BaseID + 186, 5 }, //Achievement: Get N(achos) or Get Out
        { BaseID + 187, 6 }, //Achievement: Anxiety Attack
        { BaseID + 188, 7 }, //Achievement: Private Screw'l
        { BaseID + 189, 8 }, //Achievement: Annoyed the Void
        { BaseID + 190, 9 }, //Achievement: High Bread Heaven
        { BaseID + 191, 10 }, //Achievement: Smoking Kills
        { BaseID + 192, 11 }, //Achievement: Magical Tetnis Challenge
        { BaseID + 193, 12 }, //Achievement: Corn Syrup
        { BaseID + 194, 13 }, //Achievement: Feast Fit For a Kid
        { BaseID + 195, 14 }, //Achievement: Heroes in a Whole Shell
        { BaseID + 196, 15 }, //Achievement: Highdive
    };

    public static readonly Dictionary<long, int> APLocIdToSwitchId = new()
    {
        { BaseID + 172, 236 }, //Hollow: Inside The Tree Metal Worm
        { BaseID + 203, 238 }, //Hollow Graveyard: Fish Killed
        { BaseID + 204, 239 }, //Hollow: Inside The Tree Fish 1
        { BaseID + 205, 240 }, //Hollow: Inside The Tree Fish 2
        { BaseID + 209, 512 }, //Some Other Place: Switch Near Green Headband
        { BaseID + 210, 513 }, //Some Other Place: Switch Above Pool
        { BaseID + 211, 514 }, //Some Other Place: Switch In 2010
        { BaseID + 212, 515 }, //Some Other Place: Switch In Test Zone
    };

    public static readonly Dictionary<long, int> APLocIdToRatIndex = new()
    {
        { BaseID + 197, 0 }, //Hollow Zoo: Rat Above Fresh Air
        { BaseID + 198, 1 }, //Hollow Zoo: Rat In Sand Cage
        { BaseID + 199, 2 }, //Hollow Zoo: Rat On Metal Worm Enclosure
        { BaseID + 200, 3 }, //Hollow Zoo: Rat In Wall Cage
        { BaseID + 201, 4 }, //Hollow Zoo: Rat In Cage Above Entrance
        { BaseID + 202, 5 }, //Hollow Zoo: Rat By Grass Hill
    };

    public static readonly Dictionary<long, string> APLocIdToString = new()
    {
        { BaseID + 173, "OH DEAR GOD! THIS GUY WAS A SICKO!!" }, //Hollow Graveyard: OH DEAR GOD! THIS GUY WAS A SICKO!!
        { BaseID + 174, "HOWDY" }, //Hollow Zoo: HOWDY (Greet Metal Worm)
        { BaseID + 175, "CAN SOMEBODY TELL ME WHAT THESE STUPID DUCK THINGS EVEN ARE?" }, //Some Other Place: STUPID DUCK THING
        { BaseID + 176, "DANG... WHY COULDN'T I HAVE BEEN TRAPPED IN A REOCCURING DREAM WITH HER INSTEAD OF ALEXIS?" }, //Some Other Place: Sybil Painting
        { BaseID + 177, "WHAT IS THIS? ALEXIS STEALING MY CLOTHES?" }, //Some Other Place: Ally Painting
        { BaseID + 206, "WEAR BLUE HEADBAND + SHIRT?" }, //Some Other Place: Blue Headband
        { BaseID + 207, "WEAR GREEN HEADBAND + SHIRT?" }, //Some Other Place: Green Headband
        { BaseID + 208, "WEAR BLACK HEADBAND + SHIRT?" }, //Anxiety Tower: Black Headband Under Entrance Soda Machine
    };

    public static readonly Dictionary<long, int> APLocIdToTestCubeIndex = new()
    {
        { BaseID + 213, 1 }, //Test Zone: Cube Behind Crank Door At Breakable Blocks
        { BaseID + 214, 2 }, //Test Zone: Cube On Timer Challenge Dirt Pillar
        { BaseID + 215, 3 }, //Test Zone: Cube Above Jump Pad Climb
        { BaseID + 216, 4 }, //Test Zone: Cube Above Far Side Pipe
        { BaseID + 217, 5 }, //Test Zone: Cube On Drill Climb
        { BaseID + 218, 6 }, //Test Zone: Cube By Swing Above Pool
        { BaseID + 219, 7 }, //Test Zone: Cube In Pool
        { BaseID + 220, 8 }, //Test Zone: Cube On Platform Above Owls
        { BaseID + 221, 9 }, //Test Zone: Cube By Moveable Boulders
        { BaseID + 222, 10 }, //Test Zone: Cube By Swing After Bubbles Near Entrance
        { BaseID + 223, 11 }, //Test Zone: Cube Above Crank Lift
        { BaseID + 224, 12 }, //Test Zone: Cube Above Ground Screw Lift
        { BaseID + 225, 13 }, //Test Zone: Cube In Breakable Block At Bomb Bird
        { BaseID + 226, 14 }, //Test Zone: Cube In Top Breakable Block
        { BaseID + 227, 15 }, //Test Zone: Cube On Dirt Pillar In Pool
        { BaseID + 228, 16 }, //Test Zone: Cube On Dirt Pillar Near Entrance
        { BaseID + 229, 17 }, //Test Zone: Cube By Swing Above Drill Climb
        { BaseID + 230, 18 }, //Test Zone: Cube In Doorway Red White Gate
        { BaseID + 231, 19 }, //Test Zone: Cube In Doorway Behind White Gate
        { BaseID + 232, 20 }, //Test Zone: Cube On Smaller Climb Wall
        { BaseID + 233, 21 }, //Test Zone: Cube On Dirt Wall With Pipes
        { BaseID + 234, 22 }, //Test Zone: Cube At Moving Red Cylinder
        { BaseID + 235, 23 }, //Test Zone: Cube At Moving Red Ball
        { BaseID + 236, 24 }, //Test Zone: Cube On Pipe Near Entrance
        { BaseID + 237, 25 }, //Test Zone: Cube In Chest
    };

    public static readonly Dictionary<int, long> SaveItemIdToAPLocId = InvertDictionary(APLocIdToSaveItemId);
    public static readonly Dictionary<int, long> UpgradeIdToAPLocId = InvertDictionary(APLocIdToUpgradeId);
    public static readonly Dictionary<int, long> AchievementIdToAPLocId = InvertDictionary(APLocIdToAchievementId);
    public static readonly Dictionary<int, long> SwitchIdToAPLocId = InvertDictionary(APLocIdToSwitchId);
    public static readonly Dictionary<string, long> StringToAPLocId = InvertDictionary(APLocIdToString);
    public static readonly Dictionary<int, long> RatIndexToAPLocId = InvertDictionary(APLocIdToRatIndex);
    public static readonly Dictionary<int, long> TestCubeIndexToAPLocId = InvertDictionary(APLocIdToTestCubeIndex);

    public static readonly Dictionary<int, long> SwitchIdToAPItem = new()
    {
        { 108, BaseID + 10 },
        { 228, BaseID + 11 },
        { 229, BaseID + 12 },
        { 410, BaseID + 13 },
    };

    public static long? GetAPLocationForSaveItem(SaveItem saveItem)
    {
        return GetAPLocationForSaveItemId(saveItem.id);
    }

    public static long? GetAPLocationForSaveItemId(int saveItemId)
    {
        return SaveItemIdToAPLocId.TryGetValue(saveItemId, out var locId) ? locId : null;
    }

    public static int? GetSaveItemIdForAPLocation(long locId)
    {
        return APLocIdToSaveItemId.TryGetValue(locId, out var id) ? id : null;
    }

    public static long? GetAPLocationForUpgrade(UpgradeItem upgrade)
    {
        return UpgradeIdToAPLocId.TryGetValue(upgrade.id, out var locId) ? locId : null;
    }

    public static int? GetUpgradeIdForAPLocation(long locId)
    {
        return APLocIdToUpgradeId.TryGetValue(locId, out var id) ? id : null;
    }

    public static long? GetAPLocationForAchievementId(int id)
    {
        return AchievementIdToAPLocId.TryGetValue(id, out var locId) ? locId : null;
    }

    public static int? GetAchievementIdForAPLocation(long locId)
    {
        return APLocIdToAchievementId.TryGetValue(locId, out var id) ? id : null;
    }

    public static long? GetAPItemIdForCrank(int id)
    {
        return SwitchIdToAPItem.TryGetValue(id, out var itemId) ? itemId : null;
    }

    public static long? GetAPLocationForRat(Rat1 rat1)
    {
        return RatIndexToAPLocId.TryGetValue(rat1.transform.GetSiblingIndex(), out var locId) ? locId : null;
    }

    public static long? GetAPLocationForString(string textString)
    {
        return StringToAPLocId.TryGetValue(textString, out var locId) ? locId : null;
    }

    public static long? GetAPLocationForSwitchId(int id)
    {
        return SwitchIdToAPLocId.TryGetValue(id, out var locId) ? locId : null;
    }
    
    public static long? GetAPLocationForTestCubeIndex(int id)
    {
        return TestCubeIndexToAPLocId.TryGetValue(id, out var locId) ? locId : null;
    }
}