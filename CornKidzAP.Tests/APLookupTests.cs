using CornKidzAP.Archipelago;

namespace CornKidzAP.Tests;

public class APLookupTests
{
    [Fact]
    public void AllAPLocIdsDistinct()
    {
        IList<IList<long>> keysGrouped = 
            [
                APLookup.APLocIdToSaveItemId.Select(x => x.Key).ToList(),
                APLookup.APLocIdToUpgradeId.Select(x => x.Key).ToList(),
                APLookup.APLocIdToString.Select(x => x.Key).ToList(),
                APLookup.APLocIdToAchievementId.Select(x => x.Key).ToList(),
                APLookup.APLocIdToRatIndex.Select(x => x.Key).ToList(),
                APLookup.APLocIdToSwitchId.Select(x => x.Key).ToList(),
            ];
        var keys = keysGrouped.SelectMany(x => x).ToList();
        var keysDistinct = keys.Distinct().ToList();
        Assert.Equal(keysDistinct.Count, keys.Count);
    }
}