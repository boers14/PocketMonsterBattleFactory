using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReflector : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Damage Reflector";
        oneTime = true;
        instantEffect = false;
        defenseAbility = true;
        abilityDescription = "The pocketmonster takes halve damage from the next attack and deals the same amount to the opponent. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInAttackAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager)
    {
        endOfDamageCalc = true;
    }

    public override void UseEndOfDamageCalcAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        base.UseEndOfDamageCalcAbility(ownPocketMonster, opponentPocketMonster, move, inBattleTextManager);
        float damageTaken = ownPocketMonster.amountOfDamageTaken / 2;

        if (damageTaken >= ownPocketMonster.health)
        {
            damageTaken = ownPocketMonster.health;
        }

        damageTaken = Mathf.Ceil(damageTaken);

        if (damageTaken > opponentPocketMonster.health)
        {
            damageTaken = opponentPocketMonster.health;
        }

        opponentPocketMonster.health -= damageTaken;
        ownPocketMonster.amountOfDamageTaken /= 2;
        string message = ownPocketMonster.stats.name + " used ability " + abilityName + ". Reflected half of the damage back to the opponent" +
            "dealing " + damageTaken + " damage.";
        if (damageTaken > 0)
        {
            if (player.pocketMonsters.Contains(ownPocketMonster))
            {
                inBattleTextManager.QueMessage(message, true, false, false, true);
            }
            else
            {
                inBattleTextManager.QueMessage(message, false, true, true, false);
            }
        }
        else
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
        damageDealt /= 2;
        return damageDealt;
    }
}
