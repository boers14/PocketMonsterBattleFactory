using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerator : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Accelerator";
        itemDescription = "Raises speed by one stage at start of battle.";
        startOfBattleMessage = "speed got raised by one stage.";
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.speed.GetStatChanges(1);
        effectedPocketMonster.RecalculateStatsAfterStatus();
        base.GrantOnSwitchInEffect(effectedPocketMonster, opponentPocketMonster, inBattleTextManager, player);
    }

    public override float CalculateSpeedStatChanges(float speedStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        speedStat += boostableStat.baseStat * 0.5f;
        return speedStat;
    }
}
