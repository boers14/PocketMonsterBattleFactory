using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBreath : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Last Breath";
        onDeath = true;
        abilityDescription = "When the pocketmonster faints it uses the attack that fainted it against the opponent. If the pocketmonster didnt faint " +
            "by an attack from the opponent it will use a random attacking move from it's own move pool.";
        base.SetAbilityStats(player);
    }

    public override void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager, bool isPlayer)
    {
        onDeath = false;

        if (!opponentPocketMonster.fainted)
        {
            if (move == null)
            {
                List<PocketMonsterMoves> pocketMonsterMoves = new List<PocketMonsterMoves>();

                for (int i = 0; i < ownPocketMonster.moves.Count; i++)
                {
                    if (ownPocketMonster.moves[i].currentPowerPoints > 0 && ownPocketMonster.moves[i].baseDamage > 0)
                    {
                        pocketMonsterMoves.Add(ownPocketMonster.moves[i]);
                    }
                }

                if (pocketMonsterMoves.Count > 0)
                {
                    move = pocketMonsterMoves[Random.Range(0, pocketMonsterMoves.Count - 1)];
                }
            }

            if (move != null)
            {
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". " + ownPocketMonster.stats.name + " attacked with the " +
                    "move " + move.moveName + ".", false, false, false, false);
                ownPocketMonster.DealDamage(move, opponentPocketMonster, player, !isPlayer);
            }
            else
            {
                inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". It failed.", false, false, false, false);
            }
        }
    }
}
