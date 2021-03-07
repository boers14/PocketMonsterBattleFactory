using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartingGift : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Parting Gift";
        onDeath = true;
        abilityDescription = "Raises all stats by one stage of the next pocketmonster in the party when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        List<PocketMonster> pocketmonsterTeam = GetTeamToAffect(ownPocketMonster, player);
        PocketMonster buffedPocketMonster = null;

        buffedPocketMonster = GetBuffedPocketMonster(buffedPocketMonster, ownPocketMonster, pocketmonsterTeam);

        if (buffedPocketMonster != null)
        {
            buffedPocketMonster.stats.specialAttack.GetStatChanges(1);
            buffedPocketMonster.stats.attack.GetStatChanges(1);
            buffedPocketMonster.stats.specialDefense.GetStatChanges(1);
            buffedPocketMonster.stats.defense.GetStatChanges(1);
            buffedPocketMonster.stats.speed.GetStatChanges(1);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " buffed a team member.", false, false, false, false);
        } else
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to buff a team member.", false, false, false, false);
        }
    }
}
