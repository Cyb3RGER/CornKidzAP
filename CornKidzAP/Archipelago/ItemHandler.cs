namespace CornKidzAP.Archipelago;

public enum Moves
{
    Jump,
    Punch,
    Climb,
    Slam,
    Headbutt,
    WallJump,
    Swim,
    Crouch,
}

public enum Traps
{
    SlapTrap,
    SlipTrap,
    SkidTrap,
}

public static class ItemHandler
{
    //TBD: keep item counts to resync??
    public static void AddXP(int amount)
    {
        CornKidzAP.Logger.LogDebug($"AddXP {amount} {UI.instance is null}");
        GameCtrl.instance.data.itemTotal += amount;
        if (UI.instance is null) return;
        UI.instance.displayXP();
    }

    public static void AddBottleCap()
    {
        CornKidzAP.Logger.LogDebug($"AddHP {UI.instance is null}");
        ++GameCtrl.instance.data.HPupTotal;
        if (UI.instance is null) return;
        UI.instance.displayHPup();
    }

    public static void AddLevelItem(int level)
    {
        CornKidzAP.Logger.LogDebug($"AddLevelItem {level} {UI.instance is null}");
        ++GameCtrl.instance.data.lvlItems1[level];
        if (UI.instance is null || GameCtrl.instance.currentWorld != level) return;
        UI.instance.displayLvlItem1();
    }

    public static void AddVoidScrew()
    {
        CornKidzAP.Logger.LogDebug($"AddVoidScrew {UI.instance is null}");
        ++GameCtrl.instance.data.screwTotal;
        if (UI.instance is null) return;
        UI.instance.displayScrews();
    }

    public static void AddScrewCrank(int level)
    {
        CornKidzAP.Logger.LogDebug($"AddScrewCrank {level} {UI.instance is null}");
        ++GameCtrl.instance.data.cranks[level];
        if (UI.instance is null || GameCtrl.instance.currentWorld != level) return;
        UI.instance.displayCrank();
    }

    public static void AddMaxHP()
    {
        CornKidzAP.Logger.LogDebug($"AddMaxHP {UI.instance is null}");
        ++GameCtrl.instance.data.maxHP;
        GameCtrl.instance.HP = GameCtrl.instance.data.maxHP;
        if (UI.instance is null) return;
        UI.instance.displayHP();
    }

    public static void AddGrater()
    {
        CornKidzAP.Logger.LogDebug($"AddGrater {UI.instance is null}");
        GameCtrl.instance.data.items[310] = true;
    }

    public static void AddUpgrade(int index)
    {
        CornKidzAP.Logger.LogDebug($"AddUpgrade {UI.instance is null}");
        GameCtrl.instance.data.upgrades[index] = true;
    }

    public static void UnlockMove(Moves move)
    {
        CornKidzAP.Logger.LogDebug($"UnlockMove {move} {UI.instance is null}");
        //ToDo
    }

    public static void AddRat()
    {
        CornKidzAP.Logger.LogDebug($"AddRat {UI.instance is null} {ArchipelagoClient.ArchipelagoData.Rats}");
        ++ArchipelagoClient.ArchipelagoData.Rats;
        if (UI.instance is null) return;
        if (GameCtrl.instance.currentWorld != 2 || GameCtrl.instance.goToDoor != 14) return;
        UI.instance.displayScore();
    }

    public static void AddHP()
    {
        CornKidzAP.Logger.LogDebug($"AddHP {UI.instance is null}");
        if (GameCtrl.instance.HP < GameCtrl.instance.data.maxHP)
            ++GameCtrl.instance.HP;
        if (UI.instance is null) return;
        UI.instance.displayHP();
    }

    public static void AddTrap(Traps trap)
    {
        CornKidzAP.Logger.LogDebug($"AddTrap {trap} {UI.instance is null}");
        ArchipelagoClient.APTrapHandler.AddTrap(trap);
    }

    public static void SetSwitch(int id)
    {
        CornKidzAP.Logger.LogDebug($"SetSwitch {id} {UI.instance is null}");
        GameCtrl.instance.data.switches[id] = true;
    }

    public static void AddFish()
    {
        CornKidzAP.Logger.LogDebug($"AddFish {UI.instance is null} {ArchipelagoClient.ArchipelagoData.Fish}");
        ++ArchipelagoClient.ArchipelagoData.Fish;
    }
}