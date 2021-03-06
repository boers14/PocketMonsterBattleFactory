using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickArmor : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Thick Armor";
        oneTime = true;
        instantEffect = true;
        defenseAbility = true;
        abilityDescription = "If an attack would do 65 or less damage to this pocketmonster it deals no damage instead. " +
            "This effects last till the pocketmonster switches out. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.useInAttackAbility = true;
        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " activated thick armor.", false, false, false, false);
    }

    public override void UseInAttackAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        endOfDamageCalc = true;
        ownPocketMonster.useInAttackAbility = true;
    }

    public override void UseEndOfDamageCalcAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        base.UseEndOfDamageCalcAbility(ownPocketMonster, opponentPocketMonster, move, inBattleTextManager);
        if (ownPocketMonster.amountOfDamageTaken > 0)
        {
            if (ownPocketMonster.amountOfDamageTaken <= 65)
            {
                ownPocketMonster.amountOfDamageTaken = 0;
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " blocked all incoming damage from the move " + move.moveName + ".",
                    false, false, false, false);
            }
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return GetDecisionForTrainerAiInCaseOfSwapping(trainerAi, target);
    }

    public override float CalculateDecreasedDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        if (damageDealt <= 65)
        {
            if (move.hasSideEffect)
            {
                damageDealt = 0.1f;
            } else
            {
                damageDealt = 0;
            }
        }

        return damageDealt;
    }
}
