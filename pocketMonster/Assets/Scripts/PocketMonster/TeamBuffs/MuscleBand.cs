using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleBand : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Muscle band";
        itemDescription = "Raises attack by one stage at start of battle.";
        startOfBattleMessage = "attack got raised by one stage.";
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.attack.GetStatChanges(1);
        effectedPocketMonster.RecalculateStatsAfterStatus();
        base.GrantOnSwitchInEffect(effectedPocketMonster, opponentPocketMonster, inBattleTextManager, player);
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensivePhysical)
        {
            offensiveStat += boostableStat.baseStat * 0.5f;
        }
        return offensiveStat;
    }
}
