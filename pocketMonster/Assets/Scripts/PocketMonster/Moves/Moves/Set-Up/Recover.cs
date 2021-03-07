using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recover : PocketMonsterMoves
{
    public override void SetMoveStats()
    {
        accuracy = 100;
        powerPoints = 10;
        moveName = "Recover";
        moveType = PocketMonsterStats.Typing.Regular;
        moveSort = MoveSort.Status;
        typeOfBoost = TypeOfBoostMove.Recovery;
        hasSideEffect = true;
        chanceOfSideEffect = 100;
        moveDescription = "Restore 50% of you're max health. 10 power points.";
    }

    public override void GrantSideEffect(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, float damageDone, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        bool notMaxHealth = false;

        if (ownPocketMonster.health < ownPocketMonster.stats.maxHealth)
        {
            notMaxHealth = true;
        }

        ownPocketMonster.health += ownPocketMonster.stats.maxHealth * 0.5f;
        ownPocketMonster.RecalculateHealth();
        string message = ownPocketMonster.stats.name + " used " + moveName + ". " + ownPocketMonster.stats.name + " restored it's health.";

        if (notMaxHealth)
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
            message += " It failed.";
            inBattleTextManager.QueMessage(message, false, false, false, false);
        }
    }
}
