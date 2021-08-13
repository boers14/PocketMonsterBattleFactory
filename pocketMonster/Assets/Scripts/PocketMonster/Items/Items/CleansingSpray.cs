using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansingSpray : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Cleansing spray";
        itemDescription = "On switch sets status of both pocketmonsters to none.";
        itemSort = ItemSort.General;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster,
        InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (effectedPocketMonster.currentStatus != PocketMonster.StatusEffects.None || opponentPocketMonster.currentStatus != PocketMonster.StatusEffects.None)
        {
            string message = "";

            if (effectedPocketMonster.currentStatus != PocketMonster.StatusEffects.None)
            {
                message += effectedPocketMonster.stats.name;
                effectedPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
                effectedPocketMonster.CheckForParalazys();
            }

            if (opponentPocketMonster.currentStatus != PocketMonster.StatusEffects.None)
            {
                if (message != "")
                {
                    message += " and ";
                } 

                message += opponentPocketMonster.stats.name;
                opponentPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
                opponentPocketMonster.CheckForParalazys();
            }

            inBattleTextManager.QueMessage("The status of " + message + " got set to none due to " +
                "the " + name + " of " + effectedPocketMonster.stats.name + ".", false, false, false, false);
        }
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Bloated && boostableStat.typeStat == BoostableStat.TypeStat.OffensiveSpecial)
        {
            offensiveStat *= 2;
        } else if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Burned && boostableStat.typeStat == BoostableStat.TypeStat.OffensivePhysical)
        {
            offensiveStat *= 2;
        }

        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        PlayerBattle playerBattle = player.GetComponent<PlayerBattle>();

        if (playerBattle.currentPocketMonster.currentStatus != PocketMonster.StatusEffects.Airborne)
        {
            offensiveStat /= 2;
        }

        return offensiveStat;
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Airborne)
        {
            defensiveStat *= 2;
        }

        return defensiveStat;
    }

    public override float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Paralyzed)
        {
            speedStat *= 2;
        }
        return speedStat;
    }

    public override int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        boostAmount = (int)BoostableStat.BoostAmount.X0;
        return boostAmount;
    }
}
