using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllWill : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Ill Will";
        onDeath = true;
        abilityDescription = "Gives all enemy pocketmonsters a random status condition if they had none when fainted.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        List<PocketMonster> teamToDebuff = GetTeamToAffect(opponentPocketMonster, player);
        int nonAffectedCounter = 0;

        for (int i = 0; i < teamToDebuff.Count; i++)
        {
            if (!teamToDebuff[i].fainted && teamToDebuff[i].currentStatus == PocketMonster.StatusEffects.None)
            {
                int randomStatus = Random.Range(1, System.Enum.GetNames(typeof(PocketMonster.StatusEffects)).Length);
                PocketMonster.StatusEffects status = PocketMonster.StatusEffects.None;
                status -= randomStatus;
                teamToDebuff[i].currentStatus = status;
            } else
            {
                nonAffectedCounter++;
            }
        }

        string teamText = "";

        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            teamText = "The entire opponent team";
        } else
        {
            teamText = "You're entire team";
        }

        if (nonAffectedCounter == teamToDebuff.Count)
        {
            inBattleTextManager.QueMessage(teamText + " was not affected by " + abilityName + ".", false, false, false, false);
        } else
        {
            inBattleTextManager.QueMessage(teamText + " got a status condition.", false, false, false, false);
        }
    }
}
