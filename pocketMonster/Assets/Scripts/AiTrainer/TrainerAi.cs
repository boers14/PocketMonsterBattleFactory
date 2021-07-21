using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerAi : MonoBehaviour
{
    public List<PocketMonster> pocketMonsters = new List<PocketMonster>();

    public PocketMonster currentPocketMonster = null;

    public TrainerStats stats;

    [SerializeField]
    private float disFromAi = 0;

    public bool wantsToSwitch = false, firstTurn = true, checkIfWantsToUseAbility = false, healNextPocketMonster = false;

    private InBattleTextManager inBattleTextManager;

    public void AddpocketMonsters(List<PocketMonster> pocketMonstersThisBattle, List<PocketMonsterItem> teamBuffsThisBattle, PlayerBattle player)
    {
        stats.teamBuffs.Clear();
        for (int i = 0; i < teamBuffsThisBattle.Count; i++)
        {
            stats.teamBuffs.Add(teamBuffsThisBattle[i]);
        }

        pocketMonsters.Clear();
        for (int i = 0; i < pocketMonstersThisBattle.Count; i++)
        {
            PocketMonster newPocketMonster = pocketMonstersThisBattle[i];
            newPocketMonster.moves = pocketMonstersThisBattle[i].moves;
            newPocketMonster.items = pocketMonstersThisBattle[i].items;

            newPocketMonster.transform.position = new Vector3(transform.position.x + transform.forward.x * disFromAi, transform.position.y,
                transform.position.z + transform.forward.z * disFromAi);
            newPocketMonster.transform.eulerAngles = transform.eulerAngles;
            newPocketMonster.SetAllStats();
            newPocketMonster.gameObject.SetActive(false);

            pocketMonsters.Add(newPocketMonster);
            pocketMonsters[i].ResetHealth();
            pocketMonsters[i].SetInBattleTextManager(inBattleTextManager);
            pocketMonsters[i].FillItemsAffectedStatsList(stats.teamBuffs, player);
        }

        currentPocketMonster = pocketMonsters[0];
        currentPocketMonster.gameObject.SetActive(true);
    }

    public void MakeDecision(PocketMonster target, PlayerBattle player)
    {
        bool canSwitch = CheckIfCanSwitch();

        if (canSwitch)
        {
            bool switchTime = false;
            int numberForSwitching = 1;

            if (stats.intelligence >= 60)
            {
                switchTime = CheckIfWantsSwitch(target, player);
            }
            else if (stats.intelligence >= 40)
            {
                if (numberForSwitching == Random.Range(1, 4))
                {
                    switchTime = CheckIfWantsSwitch(target, player);
                }
            }
            else if (stats.intelligence >= 20)
            {
                switchTime = false;
            }
            else if (stats.intelligence >= 0)
            {
                if (numberForSwitching == Random.Range(1, 5))
                {
                    switchTime = true;
                }
            }

            if (switchTime)
            {
                wantsToSwitch = true;
            }
            else
            {
                wantsToSwitch = false;
            }
        } else
        {
            wantsToSwitch = false;
        }
    }

    public void PerformAction(PocketMonster target, PlayerBattle player)
    {
        if (wantsToSwitch)
        {
            inBattleTextManager.QueMessage("Opponent switched pocketmonster.", false, false, false, false);
            SwitchPocketMonster(player, target);
        } else
        {
            int outOfPPCounter = 0;

            for (int i = 0; i < currentPocketMonster.moves.Count; i++)
            {
                if (currentPocketMonster.moves[i].currentPowerPoints <= 0)
                {
                    outOfPPCounter++;
                }
            }

            if (outOfPPCounter == currentPocketMonster.moves.Count)
            {
                CreateStruggle();
            }

            int chosenMove = 0;

            if (stats.intelligence >= 50)
            {
                chosenMove = ChooseAttackMove(target, player);
            } else
            {
                chosenMove = Random.Range(0, currentPocketMonster.moves.Count);
                while (currentPocketMonster.moves[chosenMove].currentPowerPoints == 0)
                {
                    chosenMove = Random.Range(0, currentPocketMonster.moves.Count);
                }
            }

            currentPocketMonster.DealDamage(currentPocketMonster.moves[chosenMove], player.currentPocketMonster, player, true);
        }
    }

    public int ChooseAttackMove(PocketMonster target, PlayerBattle player)
    {
        int chosenMove = 0;
        float highestDamageDealt = 0;

        for  (int i = 0; i < currentPocketMonster.moves.Count; i++)
        {
            if (currentPocketMonster.moves[i].currentPowerPoints > 0)
            {
                float damageDealt = CalculateDamageDealt(currentPocketMonster, currentPocketMonster.moves[i], target, player);

                if (damageDealt > highestDamageDealt)
                {
                    highestDamageDealt = damageDealt;
                    chosenMove = i;
                }
            }
        }

        return chosenMove;
    }

    public bool CheckIfWantsSwitch(PocketMonster target, PlayerBattle player)
    {
        bool isFasterOrDoesntDie = false;
        bool wantsSwitch = false;
        bool resists = false;
        bool weakness = false;
        float totalDamageMultiplier = 0;
        float highestDamageDealt = 0;
        bool getsKilled = false;

        for (int i = 0; i < currentPocketMonster.moves.Count; i++)
        {
            if (currentPocketMonster.moves[i].currentPowerPoints > 0)
            {
                float damageDealt = CalculateDamageDealt(currentPocketMonster, currentPocketMonster.moves[i], target, player);

                if (damageDealt > highestDamageDealt)
                {
                    highestDamageDealt = damageDealt;
                }
            }
        }

        if (stats.intelligence >= 90)
        {
            for (int i = 0; i < target.stats.typing.Count; i++)
            {
                AdaptableMove adaptableMove = new AdaptableMove();
                adaptableMove.SetMoveStats();
                adaptableMove.moveType = target.stats.typing[i];

                if (target.stats.baseSpecialAttack > target.stats.baseAttack)
                {
                    adaptableMove.moveSort = PocketMonsterMoves.MoveSort.Special;
                }

                float damageDealt = CalculateDamageDealt(target, adaptableMove, currentPocketMonster, player);

                if (damageDealt > currentPocketMonster.health)
                {
                    getsKilled = true;
                }
            }
        }

        if (highestDamageDealt >= target.health)
        {
            if (stats.intelligence >= 90)
            {
                if (target.stats.speed.actualStat > currentPocketMonster.stats.speed.actualStat)
                {
                    if (!getsKilled)
                    {
                        isFasterOrDoesntDie = true;
                    }
                }
                else
                {
                    isFasterOrDoesntDie = true;
                    getsKilled = false;
                }
            }
            else
            {
                isFasterOrDoesntDie = true;
            }
        }

        if (!isFasterOrDoesntDie || getsKilled)
        {
            totalDamageMultiplier = CalculateTotalDamageMultiplier(currentPocketMonster, target);

            if (totalDamageMultiplier < 1)
            {
                resists = true;
            }
            else if (totalDamageMultiplier > 1)
            {
                weakness = true;
            }

            if (!resists)
            {
                for (int i = 0; i < pocketMonsters.Count; i++)
                {
                    if (i != pocketMonsters.IndexOf(currentPocketMonster) && !pocketMonsters[i].fainted)
                    {
                        totalDamageMultiplier = CalculateTotalDamageMultiplier(pocketMonsters[i], target);
                        bool canTakeDamageOnSwitch = true;
                        if (stats.intelligence >= 90)
                        {
                            canTakeDamageOnSwitch = CheckIfWinsMatchup(target, pocketMonsters[i], player);
                        }

                        if (canTakeDamageOnSwitch)
                        {
                            if (weakness)
                            {
                                if (totalDamageMultiplier <= 1)
                                {
                                    wantsSwitch = true;
                                }
                            }
                            else
                            {
                                if (totalDamageMultiplier < 1)
                                {
                                    wantsSwitch = true;
                                }
                                else if (totalDamageMultiplier <= 1)
                                {
                                    float damageToCompare = CalculateComparativeDamage(pocketMonsters[i], target, player);

                                    if (damageToCompare > highestDamageDealt || getsKilled)
                                    {
                                        wantsSwitch = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return wantsSwitch;
    }

    private bool CheckIfWinsMatchup(PocketMonster target, PocketMonster pocketMonster, PlayerBattle player)
    {
        bool getsKilled = false;
        for (int i = 0; i < target.stats.typing.Count; i++)
        {
            AdaptableMove adaptableMove = new AdaptableMove();
            adaptableMove.SetMoveStats();
            adaptableMove.moveType = target.stats.typing[i];

            if (target.stats.baseSpecialAttack > target.stats.baseAttack)
            {
                adaptableMove.moveSort = PocketMonsterMoves.MoveSort.Special;
            }

            float damageDealt = CalculateDamageDealt(target, adaptableMove, pocketMonster, player);
            float speed = pocketMonster.stats.speed.actualStat;

            if (stats.intelligence >= 100)
            {
                for (int j = 0; j < pocketMonster.items.Count; j++)
                {
                    speed = pocketMonster.items[j].CalculateSpeedStatChanges(speed, pocketMonster.stats.speed, pocketMonster);
                }

                for (int j = 0; j < stats.teamBuffs.Count; j++)
                {
                    speed = stats.teamBuffs[j].CalculateSpeedStatChanges(speed, pocketMonster.stats.speed, pocketMonster);
                }
            }

            if (target.stats.speed.actualStat > speed)
            {
                if (damageDealt > pocketMonster.health)
                {
                    getsKilled = true;
                }
            } else
            {
                if (damageDealt * 2 > pocketMonster.health)
                {
                    getsKilled = true;
                }
            }
        }

        if (getsKilled)
        {
            return false;
        } else
        {
            return true;
        }
    }

    private float CalculateDamageDealt(PocketMonster pocketMonster, PocketMonsterMoves move, PocketMonster target, PlayerBattle player)
    {
        float damageDone = move.baseDamage;
        bool critical = false;
        bool checkForExtraItems = false;

        if (stats.intelligence >= 100)
        {
            if (pocketMonster != currentPocketMonster && pocketMonsters.Contains(pocketMonster))
            {
                checkForExtraItems = true;
            }
        }

        if (pocketMonster.stats.critChance >= 50)
        {
            critical = true;
        }

        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            damageDone = pocketMonster.CalculateCriticalDamage(damageDone, critical, pocketMonster.stats.attack, checkForExtraItems);

            if (stats.intelligence >= 70)
            {
                if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Burned)
                {
                    damageDone /= 2;
                }
            }
        }
        else if (move.moveSort == PocketMonsterMoves.MoveSort.Special)
        {
            damageDone = pocketMonster.CalculateCriticalDamage(damageDone, critical, pocketMonster.stats.specialAttack, checkForExtraItems);

            if (stats.intelligence >= 70)
            {
                if (pocketMonster.currentStatus == PocketMonster.StatusEffects.Bloated)
                {
                    damageDone /= 2;
                }
            }
        }

        float damageMultiplier = 1;

        for (int i = 0; i < pocketMonster.stats.typing.Count; i++)
        {
            if (pocketMonster.stats.typing[i] == move.moveType)
            {
                damageMultiplier += 0.5f;
            }
        }

        if (critical)
        {
            damageMultiplier += 0.5f;
        }

        damageDone *= damageMultiplier;

        damageDone = CalculateDamageTaken(move, damageDone, target, pocketMonster, critical, player);

        if (stats.intelligence >= 70)
        {
            if (target.currentStatus == PocketMonster.StatusEffects.Airborne)
            {
                damageDone *= 2;
            }
        }

        return damageDone;
    }

    private float CalculateDamageTaken(PocketMonsterMoves move, float damageTaken, PocketMonster target, PocketMonster pocketMonster, bool crit,
        PlayerBattle player)
    {
        float damageMultiplier = target.CalculateInTyping(move.moveType);
        damageTaken *= damageMultiplier;
        bool checkForExtraItems = false;

        if (stats.intelligence >= 100)
        {
            if (!pocketMonsters.Contains(pocketMonster) && target != currentPocketMonster)
            {
                checkForExtraItems = true;
            }
        }

        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            damageTaken = target.CalculateCriticalWhenDefending(damageTaken, crit, target.stats.defense, checkForExtraItems);
        }
        else if (move.moveSort == PocketMonsterMoves.MoveSort.Special)
        {
            damageTaken = target.CalculateCriticalWhenDefending(damageTaken, crit, target.stats.specialDefense, checkForExtraItems);
        }

        if (stats.intelligence >= 70)
        {
            if (pocketMonster.ability.attackAbility)
            {
                if (!pocketMonster.ability.hasBeenUsed)
                {
                    damageTaken = pocketMonster.ability.CalculateExtraDamageDealtThroughAbility(damageTaken, pocketMonster, move);
                }
            }

            if (target.ability.defenseAbility)
            {
                if (target.useInAttackAbility)
                {
                    damageTaken = target.ability.CalculateDecreasedDamageDealtThroughAbility(damageTaken, target, move);
                }
            }

            damageTaken = CalculateItemDamage(damageTaken, target.items, pocketMonster, damageMultiplier, move, false);
            damageTaken = CalculateItemDamage(damageTaken, player.teamBuffs, pocketMonster, damageMultiplier, move, false);
            damageTaken = CalculateItemDamage(damageTaken, pocketMonster.items, pocketMonster, damageMultiplier, move, true);
            damageTaken = CalculateItemDamage(damageTaken, stats.teamBuffs, pocketMonster, damageMultiplier, move, true);
        }

        return damageTaken;
    }

    private float CalculateItemDamage(float damageTaken, List<PocketMonsterItem> items, PocketMonster pocketMonster, float damageMultiplier,
        PocketMonsterMoves move, bool ownItems)
    {
        if (ownItems)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].attackTurn)
                {
                    damageTaken = items[i].CalculateDamageForAi(damageTaken, pocketMonster, damageMultiplier, move, i);
                }
            }

        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].defenseTurn)
                {
                    damageTaken = items[i].CalculateDamageForAi(damageTaken, pocketMonster, damageMultiplier, move, i);
                }
            }
        }

        return damageTaken;
    }

    public float CalculateTotalDamageMultiplier(PocketMonster pocketMonster, PocketMonster target)
    {
        float totalDamageMultiplier = 0;

        for (int i = 0; i < target.stats.typing.Count; i++)
        {
            float damageMultiplier = pocketMonster.CalculateInTyping(target.stats.typing[i]);
            if (damageMultiplier > totalDamageMultiplier)
            {
                totalDamageMultiplier = damageMultiplier;
            }
        }

        return totalDamageMultiplier;
    }

    public float CalculateComparativeDamage(PocketMonster pocketMonster, PocketMonster target, PlayerBattle player)
    {
        float damageToCompare = 0;
        for (int i = 0; i < pocketMonster.moves.Count; i++)
        {
            if (pocketMonster.moves[i].currentPowerPoints > 0)
            {
                float damageDealt = CalculateDamageDealt(pocketMonster, pocketMonster.moves[i], target, player);

                if (damageDealt > damageToCompare)
                {
                    damageToCompare = damageDealt;
                }
            }
        }

        return damageToCompare;
    }

    private void CreateStruggle()
    {
        currentPocketMonster.moves.Clear();

        Struggle struggle = new Struggle();
        struggle.SetMoveStats();
        currentPocketMonster.moves.Add(struggle);
        currentPocketMonster.ResetPP();
    }

    public void SwitchPocketMonster(PlayerBattle player, PocketMonster target)
    {
        currentPocketMonster.ResetStats(true, true);
        List<PocketMonster> possibleSwitches = new List<PocketMonster>();

        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            if (!pocketMonsters[i].fainted && i != pocketMonsters.IndexOf(currentPocketMonster))
            {
                possibleSwitches.Add(pocketMonsters[i]);
            }
        }

        if (possibleSwitches.Count == 0)
        {
            bool battleOver = CheckIfBattleOver();

            if (battleOver && !inBattleTextManager.queFinish)
            {
                inBattleTextManager.QueFinish(true, true);
            } else if (!battleOver)
            {
                print("What is this switch?");
            }
            return;
        }

        if (stats.intelligence >= 40)
        {
            currentPocketMonster = GetBestPocketMonsterToSwitchIn(target, possibleSwitches, player);
        } else
        {
            currentPocketMonster = possibleSwitches[Random.Range(0, possibleSwitches.Count)];
        }
        player.SetOpponentPocketMonster(currentPocketMonster);
        player.CreateOwnPocketMosterUI();

        if (healNextPocketMonster)
        {
            healNextPocketMonster = false;
            if (currentPocketMonster.health < currentPocketMonster.stats.maxHealth)
            {
                currentPocketMonster.health = currentPocketMonster.stats.maxHealth;
                inBattleTextManager.QueMessage("The opponent sent out " + currentPocketMonster.stats.name + ". It got healed to full health.",
                    true, false, false, false);
            }
            else
            {
                inBattleTextManager.QueMessage("The opponent sent out " + currentPocketMonster.stats.name + ".", false, false, false, false);
            }
        }
        else
        {
            inBattleTextManager.QueMessage("The opponent sent out " + currentPocketMonster.stats.name + ".", false, false, false, false);
        }

        currentPocketMonster.GetOnSwitchItemEffects(stats.teamBuffs, player, target);

        if (!player.currentPocketMonster.fainted)
        {
            UseAbilityForPocketMonster(player, target);
        } else
        {
            checkIfWantsToUseAbility = true;
        }
        currentPocketMonster.SetParticleSystem();
    }

    private PocketMonster GetBestPocketMonsterToSwitchIn(PocketMonster target, List<PocketMonster> possibleSwitches, PlayerBattle player)
    {
        List<PocketMonster> switchIns = new List<PocketMonster>();
        PocketMonster switchIn = null;
        float totalDamageMultiplier = 1;
        float highestDamage = 0;

        for (int i = 0; i < possibleSwitches.Count; i++)
        {
            totalDamageMultiplier = CalculateTotalDamageMultiplier(possibleSwitches[i], target);
            bool canSurvive = true;

            if(stats.intelligence >= 90)
            {
                canSurvive = CheckIfWinsMatchup(target, possibleSwitches[i], player);
            }

            if (totalDamageMultiplier < 1 && canSurvive)
            {
                switchIns.Add(possibleSwitches[i]);
            }
        }

        if (switchIns.Count == 0)
        {
            for (int i = 0; i < possibleSwitches.Count; i++)
            {
                totalDamageMultiplier = CalculateTotalDamageMultiplier(possibleSwitches[i], target);
                bool canSurvive = true;

                if (stats.intelligence >= 90)
                {
                    canSurvive = CheckIfWinsMatchup(target, possibleSwitches[i], player);
                }

                if (totalDamageMultiplier <= 1 && canSurvive)
                {
                    switchIns.Add(possibleSwitches[i]);
                }
            }
        }

        if (switchIns.Count == 0)
        {
            for (int i = 0; i < possibleSwitches.Count; i++)
            {
                switchIns.Add(possibleSwitches[i]);
            }
        }

        for (int i = 0; i < switchIns.Count; i++)
        {
            float damageToCompare = CalculateComparativeDamage(switchIns[i], target, player);

            if (damageToCompare >= highestDamage)
            {
                highestDamage = damageToCompare;
                switchIn = switchIns[i];
            }
        }

        return switchIn;
    }

    public bool CheckIfCanSwitch()
    {
        int faintedCounter = CountFaintedPocketMonsters();

        if (faintedCounter < pocketMonsters.Count - 1 && currentPocketMonster.currentStatus != PocketMonster.StatusEffects.Trapped)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UseAbilityForPocketMonster(PlayerBattle player,PocketMonster target)
    {
        checkIfWantsToUseAbility = false;
        if (!currentPocketMonster.ability.onDeath)
        {
            if (stats.intelligence >= 80)
            {
                bool useAbility = currentPocketMonster.ability.GetDecisionForTrainerAi(this, currentPocketMonster, player, target);
                if (useAbility)
                {
                    currentPocketMonster.UseAbility(currentPocketMonster, player.currentPocketMonster, player);
                }
            }
            else if (stats.intelligence >= 60)
            {
                bool useAbility = currentPocketMonster.ability.GetDecisionForTrainerAi(this, currentPocketMonster, player, target);
                int numberForabilityUse = 1;
                if (numberForabilityUse == Random.Range(1, 3) && useAbility)
                {
                    currentPocketMonster.UseAbility(currentPocketMonster, player.currentPocketMonster, player);
                }
            }
            else if (stats.intelligence >= 30)
            {
                int numberForabilityUse = 1;
                if (numberForabilityUse == Random.Range(1, 3))
                {
                    currentPocketMonster.UseAbility(currentPocketMonster, player.currentPocketMonster, player);
                }
            }
            else if (stats.intelligence >= 10)
            {
                int numberForabilityUse = 1;
                if (numberForabilityUse == Random.Range(1, 5))
                {
                    currentPocketMonster.UseAbility(currentPocketMonster, player.currentPocketMonster, player);
                }
            }
        }
    }

    public bool CheckIfBattleOver()
    {
        int faintedCounter = CountFaintedPocketMonsters();

        if (faintedCounter == pocketMonsters.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int CountFaintedPocketMonsters()
    {
        int faintedCounter = 0;
        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            if (pocketMonsters[i].fainted)
            {
                faintedCounter++;
            }
        }
        return faintedCounter;
    }

    public void CreateTeamBuffMessages()
    {
        for (int i = 0; i < stats.teamBuffs.Count; i++)
        {
            if (stats.teamBuffs[i].onSwitch)
            {
                inBattleTextManager.QueMessage("Enemy's team their " + stats.teamBuffs[i].startOfBattleMessage, false, false, false, false);
            }
        }
    }

    public void SetInBattleTextManager(InBattleTextManager inBattleTextManager)
    {
        this.inBattleTextManager = inBattleTextManager;
    }
}
