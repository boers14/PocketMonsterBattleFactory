using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterAbility : MonoBehaviour
{
    public string abilityName, abilityDescription;
    public bool instantEffect = false, oneTime = true, hasBeenUsed = false, onDeath = false, attackAbility = false, defenseAbility = false, 
        endOfDamageCalc = false;
    public PlayerBattle player;

    public virtual void SetAbilityStats(PlayerBattle player)
    {
        this.player = player;
    }

    public virtual void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        player.SetSwitchButtonDescription();
    }

    public virtual void UseInAttackAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager)
    {

    }

    public virtual void UseTheirTurnAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
    InBattleTextManager inBattleTextManager)
    {

    }

    public virtual void UseEndOfDamageCalcAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
        InBattleTextManager inBattleTextManager)
    {
        endOfDamageCalc = false;
    }

    public virtual void UseAbilityAftermath(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {

    }

    public virtual void UseOnDeathAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PocketMonsterMoves move,
    InBattleTextManager inBattleTextManager, bool isPlayer)
    {

    }

    public List<PocketMonster> GetTeamToAffect(PocketMonster ownPocketMonster, PlayerBattle player)
    {
        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            return player.pocketMonsters;
        }
        else
        {
            return player.opponentTrainer.pocketMonsters;
        }
    }

    public PocketMonster GetBuffedPocketMonster(PocketMonster buffedPocketMonster, PocketMonster ownPocketMonster,
    List<PocketMonster> listToSearchThrough)
    {
        int neededIndex = 0;

        if (listToSearchThrough.Count > 1)
        {
            int indexOfCurrent = listToSearchThrough.IndexOf(ownPocketMonster);

            if (indexOfCurrent + 1 > listToSearchThrough.Count - 1)
            {
                neededIndex = 0;
            }
            else
            {
                neededIndex = indexOfCurrent + 1;
            }

            if (!listToSearchThrough[neededIndex].fainted)
            {
                buffedPocketMonster = listToSearchThrough[neededIndex];
                return buffedPocketMonster;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public virtual float CalculateExtraDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        return damageDealt;
    }

    public virtual float CalculateDecreasedDamageDealtThroughAbility(float damageDealt, PocketMonster pocketMonster, PocketMonsterMoves move)
    {
        return damageDealt;
    }

    public virtual bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        return false;
    }

    public bool GetDecisionForTrainerAiInCaseOfSwapping(TrainerAi trainerAi, PocketMonster target)
    {
        bool canSwitch = trainerAi.CheckIfCanSwitch();
        bool wantSwitch = trainerAi.CheckIfWantsSwitch(target, player);

        if (canSwitch && wantSwitch)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
