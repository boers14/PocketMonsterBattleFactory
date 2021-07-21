using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPotion : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Gravity potion";
        itemDescription = "On switch in give the airborne status to both pocketmonsters. Overwrites other statusses.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster,
        InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.currentStatus = PocketMonster.StatusEffects.Airborne;
        opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.Airborne;

        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " and " + opponentPocketMonster.stats.name + " got airborne due to " +
            "the " + name + " of " + effectedPocketMonster.stats.name + ".", false, false, false, false);
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        PlayerBattle playerBattle = player.GetComponent<PlayerBattle>();

        if (playerBattle.currentPocketMonster.currentStatus != PocketMonster.StatusEffects.Airborne)
        {
            offensiveStat = offensiveStat * 2;
        }
        return offensiveStat;
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (pocketMonster.currentStatus != PocketMonster.StatusEffects.Airborne)
        {
            defensiveStat = defensiveStat / 2;
        }
        return defensiveStat;
    }
}
