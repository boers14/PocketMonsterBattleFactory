using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidespreadFear : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle  player)
    {
        abilityName = "Widespread Fear";
        onDeath = true;
        abilityDescription = "Drops the attacking stats of the entire opponent team when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        List<PocketMonster> teamToDebuff = GetTeamToAffect(opponentPocketMonster, player);
        int faintedCounter = 0;

        for (int i = 0; i < teamToDebuff.Count; i++)
        {
            if (!teamToDebuff[i].fainted)
            {
                teamToDebuff[i].stats.attack.GetStatChanges(-1);
                teamToDebuff[i].stats.specialAttack.GetStatChanges(-1);
            } else
            {
                faintedCounter++;
            }
        }

        if (faintedCounter < teamToDebuff.Count)
        {
            string teamText = "";

            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                teamText = "opponent's";
            }
            else
            {
                teamText = "you're";
            }

            inBattleTextManager.QueMessage("The attacking stats of the " + teamText + " team got dropped by one stage.", false, false, false, false);
        }
    }
}
