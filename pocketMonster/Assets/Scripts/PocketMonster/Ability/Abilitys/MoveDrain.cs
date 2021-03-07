using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDrain : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Move Drain";
        onDeath = true;
        abilityDescription = "When the pocketmonster faints it drains all the pp from the move that fainted it. If it didn't by a move from the opponent it" +
            " will drain the pp from a random move from the opponent.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, 
        InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        if (!opponentPocketMonster.fainted)
        {
            if (move != null)
            {
                move.currentPowerPoints = 0;
            }
            else
            {
                List<PocketMonsterMoves> pocketMonsterMoves = new List<PocketMonsterMoves>();

                for (int i = 0; i < opponentPocketMonster.moves.Count; i++)
                {
                    if (opponentPocketMonster.moves[i].currentPowerPoints > 0)
                    {
                        pocketMonsterMoves.Add(opponentPocketMonster.moves[i]);
                    }
                }

                if (pocketMonsterMoves.Count > 0)
                {
                    move = pocketMonsterMoves[Random.Range(0, pocketMonsterMoves.Count - 1)];
                    move.currentPowerPoints = 0;
                }
            }

            if (move.moveName == "Struggle")
            {
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". It failed.", false, false, false, false);
            }
            else
            {
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". " + opponentPocketMonster.stats.name + " lost all it's" +
                    " pp of the move " + move.moveName + ".", false, false, false, false);
            }
        }
    }
}
