using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindBand : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Mind band";
        itemDescription = "Raises special attack by one stage at start of battle.";
        startOfBattleMessage = "special attack got raised by one stage.";
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        effectedPocketMonster.stats.specialAttack.GetStatChanges(1);
        base.GrantOnSwitchInEffect(effectedPocketMonster, opponentPocketMonster, inBattleTextManager, player);
    }

    public override float CalculateOffensiveStatChanges(float offensiveStat, BoostableStat boostableStat, PocketMonster pocketMonster)
    {
        if (boostableStat.typeStat == BoostableStat.TypeStat.OffensiveSpecial)
        {
            offensiveStat += boostableStat.baseStat * 0.5f;
        }
        return offensiveStat;
    }
}
