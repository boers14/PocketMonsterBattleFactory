using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneLastIntimidation : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "One Last Intimidation";
        onDeath = true;
        abilityDescription = "Drops the attaking stats of the opponent to the minimum when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        if (!opponentPocketMonster.fainted)
        {
            opponentPocketMonster.stats.attack.GetStatChanges(-12);
            opponentPocketMonster.stats.specialAttack.GetStatChanges(-12);
            inBattleTextManager.QueMessage("The attacking stats of " + opponentPocketMonster.stats.name + " got dropped to the minimum.",
                false, false, false, false);
        }
    }
}
