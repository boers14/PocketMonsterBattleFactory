using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindfulBand : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Mindful band";
        itemDescription = "Raises special defense by one stage at start of battle.";
        startOfBattleMessage = "special defense got raised by one stage.";
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.specialDefense.GetStatChanges(1);
        effectedPocketMonster.RecalculateStatsAfterStatus();
        base.GrantOnSwitchInEffect(effectedPocketMonster, opponentPocketMonster, inBattleTextManager, player);
    }

    public override float CalculateDefensiveStatChanges(float defensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.DefensiveSpecial)
        {
            defensiveStat += boostableStat.baseStat * 0.5f;
        }
        return defensiveStat;
    }
}
