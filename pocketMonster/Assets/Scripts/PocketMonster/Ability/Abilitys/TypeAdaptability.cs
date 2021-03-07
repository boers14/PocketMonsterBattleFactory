using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeAdaptability : PocketMonsterAbility
{
    public override void SetAbilityStats(PlayerBattle player)
    {
        abilityName = "Type Adaptability";
        oneTime = true;
        instantEffect = true;
        abilityDescription = "Become the type that resists the opponent pocketmonsters types attacks. One time use.";
        base.SetAbilityStats(player);
    }

    public override void UseInstantAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager)
    {
        List<PocketMonsterStats.Typing> typeChanges = new List<PocketMonsterStats.Typing>();
        List<PocketMonsterStats.Typing> allTypes = new List<PocketMonsterStats.Typing>();
        List<PocketMonsterStats.Typing> storedTypings = new List<PocketMonsterStats.Typing>();

        int typeCounter = 1;

        for (int i = 0; i < System.Enum.GetNames(typeof(PocketMonsterStats.Typing)).Length - 1; i++)
        {
            PocketMonsterStats.Typing typing = PocketMonsterStats.Typing.None;
            typing -= typeCounter;
            allTypes.Add(typing);
            typeCounter++;
        }

        FindOptimalTyping(allTypes, typeChanges, opponentPocketMonster, 0.25f, storedTypings);

        if (typeChanges.Count < 2)
        {
            FindOptimalTyping(allTypes, typeChanges, opponentPocketMonster, 0.5f, storedTypings);
        }

        if (typeChanges.Count == 1 && storedTypings.Count > 0)
        {
            typeChanges.Add(storedTypings[0]);
        }

        if (typeChanges.Count > 0)
        {
            ownPocketMonster.stats.typing = typeChanges;
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " changed it's typing to resist it's oppenent typing.", false, false, false, false);
        } else
        {
            hasBeenUsed = false;
            inBattleTextManager.QueMessage(ownPocketMonster.stats.name + " couldn't find a type that resist both of the opponents types.",
                false, false, false, false);
        }

        if (!player.pocketMonsters.Contains(ownPocketMonster))
        {
            player.CreateOwnPocketMosterUI();
        }
    }

    private void FindOptimalTyping(List<PocketMonsterStats.Typing> allTypes, List<PocketMonsterStats.Typing> typeChanges, 
        PocketMonster opponentPocketMonster, float minumumResist, List<PocketMonsterStats.Typing> storedTypings)
    {
        for (int i = 0; i < allTypes.Count; i++)
        {
            float damageMultiplier = 1;

            for (int j = 0; j < opponentPocketMonster.stats.typing.Count; j++)
            {
                damageMultiplier *= opponentPocketMonster.CalculateInTypingWithGivenType(opponentPocketMonster.stats.typing[j], allTypes[i]);
            }

            if (damageMultiplier <= minumumResist)
            {
                bool addSecondTyping = true;
                if (typeChanges.Count == 1)
                {
                    if (minumumResist > 0.25f)
                    {
                        for (int j = 0; j < opponentPocketMonster.stats.typing.Count; j++)
                        {
                            damageMultiplier = opponentPocketMonster.CalculateInTypingWithGivenType(opponentPocketMonster.stats.typing[j], allTypes[i]);
                            float damageMulitplierToCompare = opponentPocketMonster.CalculateInTypingWithGivenType(opponentPocketMonster.stats.typing[j], typeChanges[0]);
                            if (damageMultiplier == damageMulitplierToCompare)
                            {
                                storedTypings.Add(allTypes[i]);
                                addSecondTyping = false;
                            }
                        }
                    }
                }

                if (typeChanges.Count < 2)
                {
                    if (addSecondTyping)
                    {
                        typeChanges.Add(allTypes[i]);
                    }
                }
            }
        }
    }

    public override bool GetDecisionForTrainerAi(TrainerAi trainerAi, PocketMonster pocketMonster, PlayerBattle player, PocketMonster target)
    {
        float totalDamageMultiplier = trainerAi.CalculateTotalDamageMultiplier(pocketMonster, target);

        if (totalDamageMultiplier >= 1)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
