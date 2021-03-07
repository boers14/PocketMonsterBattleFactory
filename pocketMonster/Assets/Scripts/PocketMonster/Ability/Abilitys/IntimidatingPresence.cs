using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntimidatingPresence : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Intimidating Presence";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "While this pocketmonster is in battle it will lower the attack stat of the opponent after every attack. " +
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
        if (opponentPocketMonster.stats.attack.boostAmount > BoostableStat.BoostAmount.XNegative6)
        {
            opponentPocketMonster.stats.attack.GetStatChanges(-1);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " lowered the opponents attack stat due to it's " + abilityName + ".",
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
            if (target.stats.baseAttack + 15 >= target.stats.baseSpecialAttack || target.stats.baseAttack >= 100)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
