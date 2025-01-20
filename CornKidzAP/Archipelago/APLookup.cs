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
        { BaseID + 0, 116 }, //XP Cube Near Slide
        { BaseID + 1, 100 }, //XP Cube On Slide
        { BaseID + 2, 118 }, //XP Cube Under Slide
        { BaseID + 3, 113 }, //XP Cube Bouncy Moose
        { BaseID + 4, 106 }, //XP Cube Park Top Hat
        { BaseID + 5, 101 }, //XP Cube Park Water #1
        { BaseID + 6, 102 }, //XP Cube Park Water #2
        { BaseID + 7, 103 }, //XP Cube Park Behind Bomb-able Wall #1
        { BaseID + 8, 104 }, //XP Cube Park Behind Bomb-able Wall #2
        { BaseID + 9, 119 }, //XP Cube Park Across Lake under Bolder
        { BaseID + 10, 105 }, //XP Cube Park Across Lake Boost Pad
        { BaseID + 11, 122 }, //XP Cube Stone Statue #1
        { BaseID + 12, 123 }, //XP Cube Stone Statue #2
        { BaseID + 13, 124 }, //XP Cube Stone Statue #3
        { BaseID + 14, 112 }, //XP Cube Park Fence
        { BaseID + 15, 120 }, //XP Cube Lower Slam Pillar
        { BaseID + 16, 121 }, //XP Cube Upper Slam Pillar
        { BaseID + 17, 126 }, //XP Cube Attic Drill #1
        { BaseID + 18, 127 }, //XP Cube Attic Drill #2
        { BaseID + 19, 128 }, //XP Cube Attic Top Ledge #1
        { BaseID + 20, 130 }, //XP Cube Attic Top Ledge #2
        { BaseID + 21, 129 }, //XP Cube Attic Push Cube Hallway
        { BaseID + 22, 117 }, //XP Cube House Bookcase
        { BaseID + 23, 107 }, //XP Cube House Foyer #1
        { BaseID + 24, 115 }, //XP Cube House Foyer #2
        { BaseID + 25, 114 }, //XP Cube House Foyer #3
        { BaseID + 26, 131 }, //XP Cube House Foyer Tower
        { BaseID + 27, 125 }, //XP Cube Top Bouncy Moose
        { BaseID + 28, 109 }, //XP Cube Top Boost Ring Challenge #1
        { BaseID + 29, 110 }, //XP Cube Top Boost Ring Challenge #2
        { BaseID + 30, 111 }, //XP Cube Top Boost Ring Challenge #3
        { BaseID + 31, 108 }, //XP Cube Behind Bomb-able Wall Underground
        { BaseID + 32, 135 }, //Red Screw Lake Bolder
        { BaseID + 33, 137 }, //Red Screw Attic Screw Screw
        { BaseID + 34, 136 }, //Red Screw Attic Drill
        { BaseID + 35, 134 }, //Red Screw Above Crank
        { BaseID + 36, 138 }, //Red Screw House Foyer
        { BaseID + 37, 133 }, //Red Screw Top Wall Jump Challenge
        { BaseID + 38, 139 }, //Chameleon Moth Bush
        { BaseID + 39, 140 }, //Chameleon Moth Painting
        { BaseID + 40, 143 }, //XP Crystal Park Attic
        { BaseID + 41, 144 }, //XP Crystal Garbage Grump
        { BaseID + 42, 142 }, //Crazy Mirror Sewers
        { BaseID + 43, 141 }, //Crazy Mirror Foyer Level 3
        { BaseID + 44, 132 }, //Crank Across Lake
        { BaseID + 45, 152 }, //Trash Can
        { BaseID + 46, 153 }, //Trash Can Attic #1
        { BaseID + 47, 151 }, //Trash Can Attic #2
        { BaseID + 48, 148 }, //Trash Can Sewers
        { BaseID + 49, 155 }, //Trash Can Foyer
        { BaseID + 50, 150 }, //Trash Can Park Top
        { BaseID + 51, 149 }, //Trash Can Top Wall Jump Challenge
        { BaseID + 52, 154 }, //Trash Can Foyer Level 3
        { BaseID + 53, 254 }, //XP Cube Hollow Bridge #1
        { BaseID + 54, 275 }, //XP Cube Hollow Bridge #2
        { BaseID + 55, 276 }, //XP Cube Hollow Bridge #3
        { BaseID + 56, 209 }, //XP Cube Hollow On Trees
        { BaseID + 57, 225 }, //XP Cube Hollow Leaves Under Owl Tree
        { BaseID + 58, 226 }, //XP Cube Hollow Leaves On Owl Tree
        { BaseID + 59, 216 }, //XP Cube Hollow Pillar #1
        { BaseID + 60, 217 }, //XP Cube Hollow Pillar #2
        { BaseID + 61, 266 }, //XP Cube Hollow Trampoline
        { BaseID + 62, 232 }, //XP Cube Hollow Trinket Shop
        { BaseID + 63, 231 }, //XP Cube Hollow Clock Wall Climb #1
        { BaseID + 64, 230 }, //XP Cube Hollow Clock Wall Climb #2
        { BaseID + 65, 224 }, //XP Cube Hollow Pillar Outside Church
        { BaseID + 66, 265 }, //XP Cube Hollow Clockwise Vomit
        { BaseID + 67, 215 }, //XP Cube Hollow Fart Tunnel #1
        { BaseID + 68, 214 }, //XP Cube Hollow Fart Tunnel #2
        { BaseID + 69, 233 }, //XP Cube Hollow Church Pillar
        { BaseID + 70, 255 }, //XP Cube Hollow Window #1
        { BaseID + 71, 271 }, //XP Cube Hollow Window #2
        { BaseID + 72, 273 }, //XP Cube Hollow Graveyard Pole #1
        { BaseID + 73, 261 }, //XP Cube Hollow Graveyard Tree Branch
        { BaseID + 74, 274 }, //XP Cube Hollow Graveyard Pole #2
        { BaseID + 75, 253 }, //XP Cube Hollow Graveyard Behind Tree Stump
        { BaseID + 76, 256 }, //XP Cube Hollow Graveyard Water
        { BaseID + 77, 262 }, //XP Cube Hollow Graveyard Tombstone Code #1 (12311321)
        { BaseID + 78, 263 }, //XP Cube Hollow Graveyard Tombstone Code #2 (23111323)
        { BaseID + 79, 264 }, //XP Cube Hollow Graveyard Tombstone Code #3 (21132113)
        { BaseID + 80, 210 }, //XP Cube Hollow Balcony #1
        { BaseID + 81, 211 }, //XP Cube Hollow Balcony #2
        { BaseID + 82, 218 }, //XP Cube Hollow Near Bats #1
        { BaseID + 83, 219 }, //XP Cube Hollow Near Bats #2
        { BaseID + 84, 242 }, //XP Cube Hollow Drill Room #1
        { BaseID + 85, 244 }, //XP Cube Hollow Drill Room #2
        { BaseID + 86, 246 }, //XP Cube Hollow Drill Room #3
        { BaseID + 87, 248 }, //XP Cube Hollow Drill Under Ramp
        { BaseID + 88, 220 }, //XP Cube Hollow Drill Bars #1
        { BaseID + 89, 221 }, //XP Cube Hollow Drill Bars #2
        { BaseID + 90, 236 }, //XP Cube Hollow Drill Under Church #1
        { BaseID + 91, 238 }, //XP Cube Hollow Drill Under Church #2
        { BaseID + 92, 212 }, //XP Cube Hollow Drill Dragon Mountainside #1
        { BaseID + 93, 213 }, //XP Cube Hollow Drill Dragon Mountainside #2
        { BaseID + 94, 222 }, //XP Cube Hollow Haunted House Top #1
        { BaseID + 95, 223 }, //XP Cube Hollow Haunted House Top #2
        { BaseID + 96, 272 }, //XP Cube Hollow Haunted House Behind Chimney
        { BaseID + 97, 229 }, //XP Cube Hollow Ravine under Climbing Ledge
        { BaseID + 98, 227 }, //XP Cube Hollow Ravine on Vine
        { BaseID + 99, 228 }, //XP Cube Hollow Ravine under Vine
        { BaseID + 100, 277 }, //XP Cube Hollow Fenced Across Ravine
        { BaseID + 101, 278 }, //XP Cube Hollow Music Box Swim #1
        { BaseID + 102, 279 }, //XP Cube Hollow Music Box Swim #2
        { BaseID + 103, 234 }, //XP Cube Hollow Brick Wall #1
        { BaseID + 104, 235 }, //XP Cube Hollow Brick Wall #2
        { BaseID + 105, 240 }, //XP Cube Hollow Behind Sanitary Zoo
        { BaseID + 106, 252 }, //XP Cube Hollow Spider Outside Zoo
        { BaseID + 107, 250 }, //XP Cube Dragon Crank Drill
        { BaseID + 108, 267 }, //XP Cube Fountain Swim #1
        { BaseID + 109, 268 }, //XP Cube Fountain Swim #2
        { BaseID + 110, 269 }, //XP Cube Fountain Swim #3
        { BaseID + 111, 270 }, //XP Cube Fountain Swim #4
        { BaseID + 112, 257 }, //XP Cube Tree Fish Timer
        { BaseID + 113, 259 }, //XP Cube Tree Swim
        { BaseID + 114, 258 }, //XP Cube Tree Near Metal Worm
        { BaseID + 115, 260 }, //XP Cube Graveyard Spider Cave Drill
        { BaseID + 116, 203 }, //Red Screw (Hollow Town Clock)
        { BaseID + 117, 200 }, //Red Screw (Hollow Drill Tower Alcove)
        { BaseID + 118, 202 }, //Red Screw (Hollow Behind Haunted House)
        { BaseID + 119, 208 }, //Red Screw (Hollow Dragon Cave)
        { BaseID + 120, 201 }, //Red Screw (Hollow Large Crank Pit)
        { BaseID + 121, 207 }, //Red Screw (Hollow Zoo)
        { BaseID + 122, 204 }, //Red Screw (Flipped Hollow Tree Side Room)
        { BaseID + 123, 205 }, //Red Screw (Hollow Tree High Screw)
        { BaseID + 124, 206 }, //Red Screw (Hollow Tree Low Screw)
        { BaseID + 125, 297 }, //Chameleon Moth (Hollow Bush)
        { BaseID + 126, 296 }, //Chameleon Moth (Hollow Graveyard)
        { BaseID + 127, 283 }, //XP Crystal (Hollow Timed Fountain)
        { BaseID + 128, 282 }, //XP Crystal (Hollow Church Bell)
        { BaseID + 129, 291 }, //XP Crystal (Hollow Graveyard Spider Reward)
        { BaseID + 130, 286 }, //XP Crystal (Hollow Drill Pillar)
        { BaseID + 131, 285 }, //XP Crystal (Hollow Haunted House Free Bird)
        { BaseID + 132, 295 }, //XP Crystal (Hollow Dragon Cave)
        { BaseID + 133, 280 }, //XP Crystal (Hollow Above Entry)
        { BaseID + 134, 292 }, //XP Crystal (Hollow Zombie Chamber)
        { BaseID + 135, 294 }, //XP Crystal (Hollow Free Stuck Pig)
        { BaseID + 136, 281 }, //XP Crystal (Hollow Bomb Church Pillar)
        { BaseID + 137, 293 }, //XP Crystal (Flipped Hollow Counterclockwise Vomit)
        { BaseID + 138, 287 }, //XP Crystal (Flipped Hollow Gassy Moosey)
        { BaseID + 139, 289 }, //Crazy Mirror (Hollow Graveyard)
        { BaseID + 140, 284 }, //Crazy Mirror (Hollow Zoo-Side Crank)
        { BaseID + 141, 288 }, //Crazy Mirror (Flipped Hollow Dragon Crank)
        { BaseID + 142, 290 }, //Crazy Mirror (Zoo Metal Worm)
        { BaseID + 143, 298 }, //Crank (Hollow Haunted House Ground Floor)
        { BaseID + 144, 299 }, //Crank (Hollow Ravine)
        { BaseID + 145, 306 }, //Disco Ball (Hollow Trinket Shop)
        { BaseID + 146, 300 }, //Disco Ball (Hollow Dragon Chest)
        { BaseID + 147, 308 }, //Disco Ball (Hollow Music Box)
        { BaseID + 148, 302 }, //Disco Ball (Hollow Zombie Chamber)
        { BaseID + 149, 304 }, //Disco Ball (Hollow Clean Zoo)
        { BaseID + 150, 319 }, //Bottle Cap (Hollow Graveyard)
        { BaseID + 151, 317 }, //Bottle Cap (Hollow Graveyard Inside Tree Stump)
        { BaseID + 152, 311 }, //Bottle Cap (Hollow Ravine Climb)
        { BaseID + 153, 313 }, //Bottle Cap (Hollow Zoo Rooftop)
        { BaseID + 154, 315 }, //Bottle Cap (Hollow Zoo-Side Crank)
        { BaseID + 155, 146 }, //Void Screw (Park House)
        { BaseID + 156, 324 }, //Void Screw (Hollow Outside Trinket Shop)
        { BaseID + 157, 321 }, //Void Screw (Hollow Church)
        { BaseID + 158, 323 }, //Void Screw (Hollow Sky)
        { BaseID + 159, 501 }, //Void Screw (Some Other Place)
        { BaseID + 160, 322 }, //Void Screw (Tree)
        { BaseID + 161, 325 }, //Void Screw (Kill Fish)
        { BaseID + 162, 145 }, //Void Screw (Park Top Hat)
        { BaseID + 163, 147 }, //Void Screw (Park Out of Bounds)
        { BaseID + 164, 411 }, //Void Screw (Anxiety Tower #1)
        { BaseID + 165, 412 }, //Void Screw (Anxiety Tower #2)
        { BaseID + 166, 414 }, //Crank (Anxiety Tower)
        { BaseID + 167, 326 }, //Mega Dream Soda #1
        { BaseID + 168, 500 }, //Mega Dream Soda #2
        { BaseID + 171, 310 }, //Cheese Grater
    };

    public static readonly Dictionary<long, int> APLocIdToUpgradeId = new()
    {
        { BaseID + 169, 1 }, //Drill
        { BaseID + 170, 2 }, //Fall Warp
    };

    public static readonly Dictionary<long, int> APLocIdToAchievementId = new()
    {
        { BaseID + 181, 1 }, //Little Corn Cadet
        { BaseID + 182, 2 }, //XP-ansion Pak
        { BaseID + 183, 3 }, //maXPower
        { BaseID + 184, 4 }, //...I'm a Lasagna Hog
        { BaseID + 185, 5 }, //Get N(achos) or Get Out
        { BaseID + 186, 6 }, //Anxiety Attack
        { BaseID + 187, 7 }, //Private Screw'l
        { BaseID + 188, 8 }, //Annoyed the Void
        { BaseID + 189, 9 }, //High Bread Heaven
        { BaseID + 190, 10 }, //Smoking Kills
        { BaseID + 191, 11 }, //Magical Tetnis Challenge
        { BaseID + 192, 12 }, //Corn Syrup
        { BaseID + 193, 13 }, //Feast Fit For a Kid
        { BaseID + 194, 14 }, //Heroes in a Whole Shell
        { BaseID + 195, 15 }, //Highdive
    };

    private static readonly Dictionary<long, int> APLocIdToSwitchId = new()
    {
        { BaseID + 172, 236 }, //Metal Worm
        { BaseID + 202, 238 }, //Fish Graveyard Tree Stump
        { BaseID + 203, 239 }, //Fish Owl Tree #1
        { BaseID + 204, 240 }, //Fish Owl Tree #2
    };

    private static readonly Dictionary<long, int> APLocIdToRatIndex = new()
    {
        { BaseID + 196, 0 }, //Rat Above Fresh Air
        { BaseID + 197, 1 }, //Rat Sand Cage
        { BaseID + 198, 2 }, //Rat Near Entrance Next To Fence
        { BaseID + 199, 3 }, //Rat Wall Cage
        { BaseID + 200, 4 }, //Rat In Cage Above Entrance
        { BaseID + 201, 5 }, //Rat Grass Area
    };

    private static readonly Dictionary<long, string> APLocIdToString = new()
    {
        { BaseID + 173, "OH DEAR GOD! THIS GUY WAS A SICKO!!" }, //OH DEAR GOD! THIS GUY WAS A SICKO!!
        { BaseID + 174, "HOWDY" }, //HOWDY
        { BaseID + 175, "CAN SOMEBODY TELL ME WHAT THESE STUPID DUCK THINGS EVEN ARE?" }, //CAN SOMEBODY TELL ME WHAT THESE STUPID DUCK THINGS EVEN ARE?
        { BaseID + 176, "DANG... WHY COULDN'T I HAVE BEEN TRAPPED IN A REOCCURING DREAM WITH HER INSTEAD OF ALEXIS?" }, //DANG...
        { BaseID + 205, "WEAR BLUE HEADBAND + SHIRT?" }, //Blue Headband
        { BaseID + 206, "WEAR GREEN HEADBAND + SHIRT?" }, //Green Headband
        { BaseID + 207, "WEAR BLACK HEADBAND + SHIRT?" }, //Black Headband
    };


    public static readonly Dictionary<int, long> SaveItemIdToAPLocId = InvertDictionary(APLocIdToSaveItemId);
    public static readonly Dictionary<int, long> UpgradeIdToAPLocId = InvertDictionary(APLocIdToUpgradeId);
    public static readonly Dictionary<int, long> AchievementIdToAPLocId = InvertDictionary(APLocIdToAchievementId);
    public static readonly Dictionary<int, long> SwitchIdToAPLocId = InvertDictionary(APLocIdToSwitchId);
    public static readonly Dictionary<string, long> StringToAPLocId = InvertDictionary(APLocIdToString);
    public static readonly Dictionary<int, long> RatIndexToAPLocId = InvertDictionary(APLocIdToRatIndex);

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
}