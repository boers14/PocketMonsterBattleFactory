using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Jetpack";
        itemDescription = "On switch in give the airborne status to the pocketmonster, but speed is raised by 2 stages. Will not work if the pocketmonster " +
            "already has a status except for airborne.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (effectedPocketMonster.currentStatus == PocketMonster.StatusEffects.None || effectedPocketMonster.currentStatus == PocketMonster.StatusEffects.Airborne)
        {
            string message = "";

            if (effectedPocketMonster.currentStatus == PocketMonster.StatusEffects.None)
            {
                effectedPocketMonster.currentStatus = PocketMonster.StatusEffects.Airborne;
                message += effectedPocketMonster.stats.name + " got airborne. ";
            }

            effectedPocketMonster.stats.speed.GetStatChanges(2);
            inBattleTextManager.QueMessage(message + effectedPocketMonster.stats.name + " raised it's speed by 2 stages due to it's " + name + "."
                ,false, false, false, false);
        }
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (pocketMonster.currentStatus == PocketMonster.StatusEffects.None)
        {
            defensiveStat = defensiveStat / 2;
        }
        return defensiveStat;
    }

    public override float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        speedStat += boostableStat.baseStat;
        return speedStat;
    }
}
