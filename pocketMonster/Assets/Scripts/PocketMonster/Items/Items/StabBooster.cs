using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabBooster : PocketMonsterItem
{
    public override void SetStats()
    {
        attackTurn = true;
        name = "Stab booster";
        itemDescription = "Increases damage done with stab moves by 30%, but lowers non stab move damage by 20%.";
        itemSort = ItemSort.GeneralOffense;
    }

    public override void GrantAttackTurnEffect(PocketMonster effectedPocketMonster, PocketMonsterMoves move,
        PocketMonster opponentPocketmonster, InBattleTextManager inBattleTextManager)
    {
        bool getStabBoost = CheckIfSameType(effectedPocketMonster, move);

        if (move.moveSort != PocketMonsterMoves.MoveSort.Status && opponentPocketmonster.amountOfDamageTaken > 0)
        {
            if (getStabBoost)
            {
                opponentPocketmonster.amountOfDamageTaken *= 1.3f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " did increased damage due to the " + name + "."
                    , false, false, false, false);
            } else
            {
                opponentPocketmonster.amountOfDamageTaken *= 0.8f;
                inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " did decreased damage due to the " + name + "."
                    , false, false, false, false);
            }
        }
    }

    public override float CalculateDamageForAi(float damageDone, PocketMonster pocketMonster, float damageMultiplier, PocketMonsterMoves move, int index)
    {
        bool getStabBoost = CheckIfSameType(pocketMonster, move);

        if (getStabBoost)
        {
            damageDone *= 1.3f;
        }
        else
        {
            damageDone *= 0.8f;
        }

        return damageDone;
    }

    private bool CheckIfSameType(PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        bool sameType = false;
        for (int i = 0; i < pocketMonster.stats.typing.Count; i++)
        {
            if (move.moveType == pocketMonster.stats.typing[i])
            {
                sameType = true;
            }
        }

        return sameType;
    }
}
