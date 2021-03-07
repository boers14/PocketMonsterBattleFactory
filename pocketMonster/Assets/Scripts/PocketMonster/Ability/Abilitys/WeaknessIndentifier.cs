using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessIndentifier : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Weakness Indentifier";
        onDeath = true;
        abilityDescription = "Drops the defensive stats of the opponent to the minimum when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        if (!opponentPocketMonster.fainted)
        {
            opponentPocketMonster.stats.defense.GetStatChanges(-12);
            opponentPocketMonster.stats.specialDefense.GetStatChanges(-12);
            inBattleTextManager.QueMessage("The defensive stats of " + opponentPocketMonster.stats.name + " got dropped to the minimum.",
                false, false, false, false);
        }
    }
}
