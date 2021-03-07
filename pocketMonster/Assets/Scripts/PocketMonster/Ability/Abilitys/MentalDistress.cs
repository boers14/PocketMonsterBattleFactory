using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalDistress : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Mental Distress";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "While this pocketmonster is in battle it will lower the special attack stat of the oppenent after every attack. " +
            "One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.abilityOnTheirTurn = true;
    }

    public override void UseTheirTurnAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move, InBattleTextManager inBattleTextManager)
    {
        ownPocketMonster.abilityOnTheirTurn = true;
        if (opponentPocketMonster.stats.specialAttack.boostAmount > BoostableStat.BoostAmount.XNegative6)
        {
            opponentPocketMonster.stats.specialAttack.GetStatChanges(-1);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " lowered the opponents special attack stat due to it's " + abilityName + ".",
                false, false, false, false);
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        bool canSwitch = trainerAi.CheckIfCanSwitch();
        bool wantSwitch = trainerAi.CheckIfWantsSwitch(target, player);

        if (canSwitch && wantSwitch)
        {
            return false;
        }
        else
        {
            if (target.stats.baseSpecialAttack + 15 >= target.stats.baseAttack || target.stats.baseSpecialAttack >= 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
