using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadTheHealth : PocketMonsterAbility
{
    private bool colorPlayer = false, colorAi = false;

    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Spread The Health";
        oneTime = false;
        instantEffect = true;
        abilityDescription = "Take 50% of current health and take 50% damage from the amount healed in total, heal the two pocketmonsters adjacent " +
            "in the party for half of the amount. Doesn't work for full health or fainted pocketmonsters. Take 25% of current health for 1 pocketmonster. " +
            "Can be used multiple times.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        List<PocketMonster> healedPocketMonsters = new List<PocketMonster>();
        int inNeedOfHealCounter = 0;

        if (player.pocketMonsters.Contains(ownPocketMonster))
        {
            colorPlayer = true;
            GetPocketMonstersInNeedOfHealth(healedPocketMonsters, ownPocketMonster, player.pocketMonsters);
        }
        else
        {
            colorAi = true;
            GetPocketMonstersInNeedOfHealth(healedPocketMonsters, ownPocketMonster, player.opponentTrainer.pocketMonsters);
        }

        for (int i = 0; i < healedPocketMonsters.Count; i++)
        {
            if (!healedPocketMonsters[i].fainted && healedPocketMonsters[i].health < healedPocketMonsters[i].stats.maxHealth)
            {
                inNeedOfHealCounter++;
            }
        }

        if (inNeedOfHealCounter == 1)
        {
            HealPocketMonsters(healedPocketMonsters, ownPocketMonster, 4, 1);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " healed one of it's team members at the cost of it's own health.", 
                colorAi, colorPlayer, true, true);
        } else if (inNeedOfHealCounter == 2)
        {
            HealPocketMonsters(healedPocketMonsters, ownPocketMonster, 2, 2);
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " healed two of it's team members at the cost of it's own health.",
                colorAi, colorPlayer, true, true);
        } else
        {
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " failed to heal it's team members.", false, false, false, false);
        }

        player.SetSwitchButtonDescription();
        colorPlayer = false;
        colorAi = false;
    }

    private void GetPocketMonstersInNeedOfHealth(List<PocketMonster> healedPocketMonsters, PocketMonster ownPocketMonster, 
        List<PocketMonster> listToSearchThrough)
    {
        int neededIndex = 0;

        if (listToSearchThrough.Count > 1)
        {
            int indexOfCurrent = listToSearchThrough.IndexOf(ownPocketMonster);
            for (int i = 0; i < 2; i++)
            {
                int addedNumber = 1 - (i % 2 * 2);
                if (indexOfCurrent + addedNumber > listToSearchThrough.Count - 1)
                {
                    neededIndex = 0;
                }
                else if (indexOfCurrent + addedNumber < 0)
                {
                    neededIndex = listToSearchThrough.Count - 1;
                }
                else
                {
                    neededIndex = indexOfCurrent + addedNumber;
                }

                if (!healedPocketMonsters.Contains(listToSearchThrough[neededIndex]))
                {
                    healedPocketMonsters.Add(listToSearchThrough[neededIndex]);
                }
            }
        }
    }

    private void HealPocketMonsters(List<PocketMonster> healedPocketMonsters, PocketMonster ownPocketMonster, int healthDevision, int addedHealthDevision)
    {
        float healthGiven = ownPocketMonster.health / healthDevision;
        ownPocketMonster.health -= healthGiven / 2;
        for (int i = 0; i < healedPocketMonsters.Count; i++)
        {
            if (!healedPocketMonsters[i].fainted && healedPocketMonsters[i].health < healedPocketMonsters[i].stats.maxHealth)
            {
                healedPocketMonsters[i].health += healthGiven / addedHealthDevision;
                healedPocketMonsters[i].RecalculateHealth();
            }
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        List<PocketMonster> healedPocketMonsters = new List<PocketMonster>();
        int inNeedOfHealCounter = 0;

        GetPocketMonstersInNeedOfHealth(healedPocketMonsters, pocketMonster, player.opponentTrainer.pocketMonsters);

        for (int i = 0; i < healedPocketMonsters.Count; i++)
        {
            if (!healedPocketMonsters[i].fainted && healedPocketMonsters[i].health < healedPocketMonsters[i].stats.maxHealth)
            {
                inNeedOfHealCounter++;
            }
        }

        if (inNeedOfHealCounter > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
