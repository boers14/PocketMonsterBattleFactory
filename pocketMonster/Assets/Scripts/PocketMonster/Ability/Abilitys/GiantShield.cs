using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantShield : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Giant Shield";
        oneTime = false;
        instantEffect = true;
        defenseAbility = true;
        abilityDescription = "Halves the damage taken from the next attack, but halves damage dealt next attack. Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        hasBeenUsed = false;
        ownPocketMonster.abilityOnTheirTurn = true;
        ownPocketMonster.useInAttackAbility = true;
    }

    public override void UseTheirTurnAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        opponentPocketMonster.amountOfDamageTaken /= 2;

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " did halve damage due to the " + abilityName + ".", false, false, false, false);
    }

    public override void UseInAttackAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.amountOfDamageTaken /= 2;

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". " + ownPocketMonster.stats.name + " halved the damage " +
            "taken.", false, false, false, false);
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        float damageDealt = trainerAi.CalculateComparativeDamage(pocketMonster, target, player);

        if (damageDealt / 2 >= target.health)
        {
            return true;
        } else if (damageDealt >= target.health)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public override float CalculateDecreasedDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        damageDealt /= 2;
        return damageDealt;
    }
}
