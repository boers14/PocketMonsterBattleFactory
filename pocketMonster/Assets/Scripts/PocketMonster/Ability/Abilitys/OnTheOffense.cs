using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTheOffense : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "On The Offense";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Become airborne and take 50% recoil on every attack, but increase attack and special attack by two stages. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.currentStatus = PocketMonster.StatusEffects.Airborne;
        ownPocketMonster.stats.attack.GetStatChanges(2);
        ownPocketMonster.stats.specialAttack.GetStatChanges(2);
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " increased its attack and special attack by two stages, but became airborne.", 
            false, false, false, false);
        ownPocketMonster.abilityOnTheirTurn = true;
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    public override void UseTheirTurnAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.abilityOnTheirTurn = true;

        if (opponentPocketMonster.amountOfDamageTaken > 0)
        {
            if (opponentPocketMonster.amountOfDamageTaken > opponentPocketMonster.health)
            {
                ownPocketMonster.health -= Mathf.Ceil(opponentPocketMonster.health / 2);
            }
            else
            {
                ownPocketMonster.health -= Mathf.Ceil(opponentPocketMonster.amountOfDamageTaken / 2);
            }
            string message = ownPocketMonster.stats.name + " took 50% recoil from the attack.";

            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                inBattleTextManager.QueMessage(message, false, true, true, false);
            }
            else
            {
                inBattleTextManager.QueMessage(message, true, false, false, true);
            }
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return GetDecisionForTrainerAiInCaseOfSwapping(trainerAi, target);
    }
}
