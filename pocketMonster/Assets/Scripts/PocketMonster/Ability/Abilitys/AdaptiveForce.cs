using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveForce : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Adaptive Force";
        oneTime = true;
        instantEffect = true;
        attackAbility = true;
        abilityDescription = "Deals twice the damage if the pocketmonster used a move that is same type as the pocketmonster on the next atack. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseTheirTurnAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        for (int i = 0; i < ownPocketMonster.stats.typing.Count; i++) {
            if (ownPocketMonster.stats.typing[i] == move.moveType)
            {
                opponentPocketMonster.amountOfDamageTaken *= 2;
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " did double damage due to the " + abilityName + ".", 
                    false, false, false, false);
                hasBeenUsed = true;
            }
        }
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.abilityOnTheirTurn = true;
        hasBeenUsed = false;
    }

    public override float CalculateExtraDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        for (int i = 0; i < pocketMonster.stats.typing.Count; i++)
        {
            if (move.moveType == pocketMonster.stats.typing[i])
            {
                damageDealt *= 2;
            }
        }

        return damageDealt;
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        int chosenMove = trainerAi.ChooseAttackMove(target, player);
        bool sameType = false;

        for (int i = 0; i < pocketMonster.stats.typing.Count; i++) {
            if (pocketMonster.moves[chosenMove].moveType == pocketMonster.stats.typing[i])
            {
                sameType = true;
            }
        }

        if (sameType)
        {
            float damageDealt = trainerAi.CalculateComparativeDamage(pocketMonster, target, player);
            if (damageDealt / 2 >= target.health)
            {
                return false;
            } else
            {
                return true;
            }
        } else
        {
            return false;
        }
    }
}