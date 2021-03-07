using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAbsorption : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Damage Absorption";
        oneTime = true;
        instantEffect = false;
        defenseAbility = true;
        abilityDescription = "The pocketmonster heals for the amount of damage dealt from the next attack instead of taking damage. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInAttackAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager)
    {
        float amountHealed = ownPocketMonster.amountOfDamageTaken;
        ownPocketMonster.health += amountHealed;

        if (ownPocketMonster.health > ownPocketMonster.stats.maxHealth)
        {
            amountHealed -= ownPocketMonster.health - ownPocketMonster.stats.maxHealth;
            ownPocketMonster.health = ownPocketMonster.stats.maxHealth;
        }

        ownPocketMonster.amountOfDamageTaken = 0;
        string message = opponentPocketMonster.stats.name + " used move " + move.moveName + ". " + ownPocketMonster.stats.name + " " +
                "healed for " + Mathf.RoundToInt(amountHealed) + ".";

        if (amountHealed > 0)
        {
            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                inBattleTextManager.QueMessage(message, false, true, false, false);
            }
            else
            {
                inBattleTextManager.QueMessage(message, true, false, false, false);
            }
        } else
        {
            inBattleTextManager.QueMessage(message, false, false, false, false);
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return true;
    }

    public override float CalculateDecreasedDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        if (move.hasSideEffect)
        {
            damageDealt = 0.1f;
        }
        else
        {
            damageDealt = 0;
        }

        return damageDealt;
    }
}
