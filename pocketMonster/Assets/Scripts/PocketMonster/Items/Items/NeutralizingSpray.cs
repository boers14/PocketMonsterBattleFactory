using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralizingSpray : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Neutralizing spray";
        itemDescription = "On switch resets stats of both pocketmonsters.";
        itemSort = ItemSort.General;
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster,
        InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.ResetStats(false, false);
        opponentPocketMonster.ResetStats(false, false);
        player.SetSwitchButtonDescription();
        inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " and " + opponentPocketMonster.stats.name + " stats got reset due to " +
            "the " + name + " of " + effectedPocketMonster.stats.name + ".", false, false, false, false);
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        offensiveStat = boostableStat.baseStat;
        return offensiveStat;
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        defensiveStat = boostableStat.baseStat;
        return defensiveStat;
    }

    public override float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        speedStat = boostableStat.baseStat;
        return speedStat;
    }

    public override int CalcaluteDifferenceInBoostAmount(int boostAmount, BoostableStat boostableStat)
    {
        boostAmount = (int)BoostableStat.BoostAmount.X0;
        return boostAmount;
    }
}
