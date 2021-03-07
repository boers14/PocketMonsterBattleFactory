using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainSplit : PocketMonsterAbility
{
    private bool colorPlayerText = false, colorAiText = false, colorPlayerRed = false, colorAiRed = false;

    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Pain Split";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "The pocketmonster tries to equal the health to same amount as the opponent but not higher then the max amount of health." +
            " Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        float differenceInHealth = (opponentPocketMonster.health - ownPocketMonster.health) / 2;
        bool alreadyAtMaxHealth = false;

        if (differenceInHealth < 0)
        {
            differenceInHealth *= -1;
        }

        if (opponentPocketMonster.health > ownPocketMonster.health)
        {
            if (ownPocketMonster.health == ownPocketMonster.stats.maxHealth)
            {
                alreadyAtMaxHealth = true;
            }

            ownPocketMonster.health += differenceInHealth;
            opponentPocketMonster.health -= differenceInHealth;
            ownPocketMonster.RecalculateHealth();

            if (differenceInHealth != 0)
            {
                setPocketMonsterText(player, opponentPocketMonster, true);
                if (!alreadyAtMaxHealth)
                {
                    setPocketMonsterText(player, ownPocketMonster, false);
                }
            }
        }
        else
        {
            if (opponentPocketMonster.health == opponentPocketMonster.stats.maxHealth)
            {
                alreadyAtMaxHealth = true;
            }

            opponentPocketMonster.health += differenceInHealth;
            ownPocketMonster.health -= differenceInHealth;
            opponentPocketMonster.RecalculateHealth();

            if (differenceInHealth != 0)
            {
                setPocketMonsterText(player, ownPocketMonster, true);
                if (!alreadyAtMaxHealth)
                {
                    setPocketMonsterText(player, opponentPocketMonster, false);
                }
            }
        }

        inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " used " + abilityName + ". Both pocketmonsters equaled their health.",
            colorAiText, colorPlayerText, colorPlayerRed, colorAiRed);

        colorPlayerText = false;
        colorPlayerRed = false;
        colorAiText = false;
        colorAiRed = false;
        base.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
    }

    private void setPocketMonsterText(PlayerBattle player, PocketMonster pocketMonster, bool red)
    {
        if (player.pocketMonsters.Contains(pocketMonster))
        {
            colorPlayerText = true;
            colorPlayerRed = red;
        }
        else
        {
            colorAiText = true;
            colorAiRed = red;
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        if (pocketMonster.health < target.health)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
