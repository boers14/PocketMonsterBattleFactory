using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PocketMonster : MonoBehaviour
{
    public PocketMonsterStats stats;

    public List<PocketMonsterMoves> moves = new List<PocketMonsterMoves>();

    public List<PocketMonsterItem> items = new List<PocketMonsterItem>();

    public float health = 0, amountOfDamageTaken = 0;

    public bool fainted = false, abilityOnTheirTurn = false, aftermathOfAbility = false, useInAttackAbility = false;

    private InBattleTextManager inBattleTextManager;

    public enum StatusEffects
    {
        Paralyzed,
        Burned,
        Poisened,
        Nearsighted,
        Cursed,
        Trapped,
        Leeched,
        Bloated,
        Airborne,
        None
    }

    public StatusEffects currentStatus = StatusEffects.None;

    public List<PocketMonsterAbility> possibleAbilitys = new List<PocketMonsterAbility>();
    public PocketMonsterAbility ability, chosenAbility;
    private bool goUp = true, isPlayingParticles = false;

    private float yMotionSpace = 0.5f;
    private Vector3 battlePos = Vector3.zero;

    private new ParticleSystem particleSystem = null;

    private List<PocketMonsterStats.Typing> originalTyping = new List<PocketMonsterStats.Typing>();

    private void Start()
    {
        SetParticleSystem();
    }

    public void SetParticleSystem()
    {
        if (particleSystem == null)
        {
            if (gameObject.GetComponent<ParticleSystem>() == null)
            {
                particleSystem = gameObject.AddComponent<ParticleSystem>();
            }
            else
            {
                particleSystem = gameObject.GetComponent<ParticleSystem>();
            }
        }

        StopParticles();
    }

    private void FixedUpdate()
    {
        Vector3 newPos = Vector3.zero;

        if (goUp)
        {
            float differenceInY = transform.position.y - battlePos.y;
            newPos.y = (yMotionSpace - differenceInY) / 50;
        } else
        {
            float differenceInY = transform.position.y - battlePos.y;
            newPos.y = (differenceInY - yMotionSpace) / 50;
        }

        transform.position += newPos;

        if (goUp)
        {
            if (transform.position.y >= battlePos.y + (yMotionSpace - yMotionSpace / 10))
            {
                goUp = false;
            }
        } else
        {
            if (transform.position.y <= battlePos.y - (yMotionSpace - yMotionSpace / 10))
            {
                goUp = true;
            }
        }

        if (particleSystem != null && !isPlayingParticles)
        {
            StopParticles();
        }
    }

    public void DealDamage(PocketMonsterMoves move, PocketMonster target, PlayerBattle player, bool isPlayer)
    {
        if (target.fainted)
        {
            return;
        }

        move.currentPowerPoints--;

        if (currentStatus == StatusEffects.Cursed)
        {
            move.currentPowerPoints -= 2;
        }

        float damageDone = move.baseDamage;
        bool critical = false;

        if (Random.Range(1, 100) < stats.critChance)
        {
            critical = true;
        }

        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            damageDone = CalculateCriticalDamage(damageDone, critical, stats.attack);
        }
        else if (move.moveSort == PocketMonsterMoves.MoveSort.Special)
        {
            damageDone = CalculateCriticalDamage(damageDone, critical, stats.specialAttack);
        }

        float damageMultiplier = 1;

        for (int i = 0; i < stats.typing.Count; i++)
        {
            if (stats.typing[i] == move.moveType)
            {
                damageMultiplier += 0.5f;
            }
        }

        if (critical)
        {
            damageMultiplier += 0.5f;
        }

        damageDone *= damageMultiplier;

        bool missed = false;
        if (move.moveSort != PocketMonsterMoves.MoveSort.Status)
        {
            if (currentStatus == StatusEffects.Nearsighted)
            {
                if (Random.Range(1, 100) >= move.accuracy - 15)
                {
                    missed = true;
                }
            }
            else
            {
                if (Random.Range(1, 100) >= move.accuracy)
                {
                    missed = true;
                }
            }
        }

        target.TakeDamage(move, damageDone, player, this, isPlayer, critical, missed);
    }

    public void TakeDamage(PocketMonsterMoves move, float damageTaken, PlayerBattle player, PocketMonster other, bool isPlayer, bool critical, bool missed)
    {
        float damageMultiplier = CalculateInTyping(move.moveType);

        if (missed)
        {
            damageMultiplier = 0;
        }

        amountOfDamageTaken = damageTaken;
        amountOfDamageTaken *= damageMultiplier;

        if (move.moveSort == PocketMonsterMoves.MoveSort.Physical)
        {
            amountOfDamageTaken = CalculateCriticalWhenDefending(amountOfDamageTaken, critical, stats.defense);
        }
        else if (move.moveSort == PocketMonsterMoves.MoveSort.Special)
        {
            amountOfDamageTaken = CalculateCriticalWhenDefending(amountOfDamageTaken, critical, stats.specialDefense);
        }

        amountOfDamageTaken += (amountOfDamageTaken * Random.Range(0.01f, 0.05f));

        if (useInAttackAbility)
        {
            ability.hasBeenUsed = true;
            useInAttackAbility = false;
            ability.UseInAttackAbility(this, other, move, inBattleTextManager);
        }

        if (other.abilityOnTheirTurn)
        {
            other.abilityOnTheirTurn = false;
            other.ability.UseTheirTurnAbility(other, this, move, inBattleTextManager);
        }

        if (!isPlayer)
        {
            other.GetBattleTurnItemEffects(player.teamBuffs, move, this, true);
            GetBattleTurnItemEffects(player.opponentTrainer.stats.teamBuffs, move, other, false);
        }
        else
        {
            other.GetBattleTurnItemEffects(player.opponentTrainer.stats.teamBuffs, move, this, true);
            GetBattleTurnItemEffects(player.teamBuffs, move, other, false);
        }

        amountOfDamageTaken = Mathf.Ceil(amountOfDamageTaken);

        if (health - amountOfDamageTaken < 0)
        {
            amountOfDamageTaken = health;
            health = 0;
        }
        else
        {
            health -= amountOfDamageTaken;
        }

        if (amountOfDamageTaken > 0 || missed)
        {
            inBattleTextManager.QueBattleMessages(other, move, Mathf.RoundToInt(amountOfDamageTaken), damageMultiplier, critical, fainted, this, missed, 
                isPlayer);
            if (amountOfDamageTaken > 0)
            {
                SetParticleMat(move, player);
            }
        }

        if (!missed)
        {
            if (move.hasSideEffect)
            {
                if (Random.Range(1, 100) >= (100 - move.chanceOfSideEffect))
                {
                    move.GrantSideEffect(other, this, amountOfDamageTaken, inBattleTextManager, player);
                }
            }
        }

        if (other.health <= 0)
        {
            other.fainted = true;
            other.currentStatus = StatusEffects.None;

            if (other.ability.onDeath)
            {
                other.ability.UseOnDeathAbility(other, this, null, inBattleTextManager, isPlayer);
            }
        }

        if (health <= 0)
        {
            fainted = true;
            currentStatus = StatusEffects.None;

            if (ability.onDeath)
            {
                ability.UseOnDeathAbility(this, other, move, inBattleTextManager, isPlayer);
            }
        }

        player.CheckIfBattleOver();

        for (int i = 0; i < other.items.Count; i++)
        {
            if (other.items[i].endOfAttackTurn)
            {
                other.items[i].GrantEndOfAttackTurnEffect(other, move, this, inBattleTextManager);
            }
        }
    }

    public float CalculateInTyping(PocketMonsterStats.Typing typingToCompare)
    {
        float damageMultiplier = 1;

        for (int i = 0; i < stats.typing.Count; i++)
        {
            damageMultiplier = GetDamageMultiplier(damageMultiplier, stats.typing[i], typingToCompare);
        }

        return damageMultiplier;
    }

    private float GetDamageMultiplier(float damageMultiplier, PocketMonsterStats.Typing type, PocketMonsterStats.Typing typeToCompare)
    {
        switch (type)
        {
            case PocketMonsterStats.Typing.Grass:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[] {
                        PocketMonsterStats.Typing.Fire, PocketMonsterStats.Typing.Chemical, PocketMonsterStats.Typing.Wind}, new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Light, PocketMonsterStats.Typing.Static }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Fire:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Wind, PocketMonsterStats.Typing.Earth }, new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Grass, PocketMonsterStats.Typing.Chemical, PocketMonsterStats.Typing.Light }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Water:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Static, PocketMonsterStats.Typing.Grass, PocketMonsterStats.Typing.Chemical }, new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Spooky, PocketMonsterStats.Typing.Fire, PocketMonsterStats.Typing.Earth }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Wind:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Static, PocketMonsterStats.Typing.Light, PocketMonsterStats.Typing.Spooky },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Earth, PocketMonsterStats.Typing.Chemical,
                        PocketMonsterStats.Typing.Grass }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Earth:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Grass, PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Fire },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Wind, PocketMonsterStats.Typing.Spooky,
                        PocketMonsterStats.Typing.Chemical }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Light:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Spooky, PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Wind },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Fire, PocketMonsterStats.Typing.Grass,
                        PocketMonsterStats.Typing.Static, PocketMonsterStats.Typing.Chemical }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Spooky:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[] {
                        PocketMonsterStats.Typing.Light, PocketMonsterStats.Typing.Chemical, PocketMonsterStats.Typing.Grass },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Wind, PocketMonsterStats.Typing.Static,
                        PocketMonsterStats.Typing.Earth, PocketMonsterStats.Typing.Regular }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Chemical:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Spooky, PocketMonsterStats.Typing.Fire, PocketMonsterStats.Typing.Earth },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Wind,
                        PocketMonsterStats.Typing.Light }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Static:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[]
                    { PocketMonsterStats.Typing.Static, PocketMonsterStats.Typing.Light, PocketMonsterStats.Typing.Earth },
                    new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Water, PocketMonsterStats.Typing.Fire,
                        PocketMonsterStats.Typing.Spooky }, typeToCompare);
                break;
            case PocketMonsterStats.Typing.Regular:
                damageMultiplier *= CalculateInMoveType(new PocketMonsterStats.Typing[] { PocketMonsterStats.Typing.Chemical }, new PocketMonsterStats.Typing[]
                { PocketMonsterStats.Typing.Light, PocketMonsterStats.Typing.Spooky }, typeToCompare);
                break;
            default:
                print("that is no type");
                break;
        }

        return damageMultiplier;
    }

    private float CalculateInMoveType(PocketMonsterStats.Typing[] superEffective, PocketMonsterStats.Typing[] notVeryEffective, PocketMonsterStats.Typing typingToCompare)
    {
        float damageMultiplier = 1;

        for (int i = 0; i < superEffective.Length; i++)
        {
            if (typingToCompare == superEffective[i])
            {
                damageMultiplier *= 2;
                continue;
            }
        }

        for (int i = 0; i < notVeryEffective.Length; i++)
        {
            if (typingToCompare == notVeryEffective[i])
            {
                damageMultiplier *= 0.5f;
                continue;
            }
        }

        return damageMultiplier;
    }

    public float CalculateCriticalDamage(float damageStat, bool critical, BoostableStat statToCheck, bool checkForExtraStats = false)
    {
        bool changedBoostAmount = false;
        int boostAmount = (int)statToCheck.boostAmount;
        float actualStat = statToCheck.actualStat;

        if (checkForExtraStats)
        {
            for (int i = 0; i < items.Count; i++)
            {
                actualStat = items[i].CalculateOffensiveStatChanges(actualStat, statToCheck, this);
                boostAmount = items[i].CalcaluteDifferenceInBoostAmount(boostAmount, statToCheck);
            }

            for (int i = 0; i < inBattleTextManager.player.opponentTrainer.stats.teamBuffs.Count; i++)
            {
                actualStat = inBattleTextManager.player.opponentTrainer.stats.teamBuffs[i].CalculateOffensiveStatChanges(actualStat, statToCheck, this);
                boostAmount = inBattleTextManager.player.opponentTrainer.stats.teamBuffs[i].CalcaluteDifferenceInBoostAmount(boostAmount, statToCheck);
            }

            if (boostAmount > (int)BoostableStat.BoostAmount.X0)
            {
                changedBoostAmount = true;
            }
        }

        if (!critical)
        {
            damageStat *= actualStat;
        }
        else
        {
            if (statToCheck.boostAmount < BoostableStat.BoostAmount.X0)
            {
                if (changedBoostAmount)
                {
                    damageStat *= actualStat;
                }
                else
                {
                    damageStat *= statToCheck.baseStat;
                }
            }
            else
            {
                damageStat *= actualStat;
            }
        }

        return damageStat;
    }

    public float CalculateCriticalWhenDefending(float damageStat, bool critical, BoostableStat statToCheck, bool checkForExtraStats = false)
    {
        bool changedBoostAmount = false;
        int boostAmount = (int)statToCheck.boostAmount;
        float actualStat = statToCheck.actualStat;

        if (checkForExtraStats)
        {
            for (int i = 0; i < items.Count; i++)
            {
                actualStat = items[i].CalculateDefensiveStatChanges(actualStat, statToCheck, this);
                boostAmount = items[i].CalcaluteDifferenceInBoostAmount(boostAmount, statToCheck);
            }

            for (int i = 0; i < inBattleTextManager.player.opponentTrainer.stats.teamBuffs.Count; i++)
            {
                actualStat = inBattleTextManager.player.opponentTrainer.stats.teamBuffs[i].CalculateDefensiveStatChanges(actualStat, statToCheck, this);
                boostAmount = inBattleTextManager.player.opponentTrainer.stats.teamBuffs[i].CalcaluteDifferenceInBoostAmount(boostAmount, statToCheck);
            }

            if (boostAmount < (int)BoostableStat.BoostAmount.X0)
            {
                changedBoostAmount = true;
            }
        }

        if (!critical)
        {
            damageStat /= actualStat;
        }
        else
        {
            if (statToCheck.boostAmount > BoostableStat.BoostAmount.X0)
            {
                if (changedBoostAmount)
                {
                    damageStat /= actualStat;
                }
                else
                {
                    damageStat /= statToCheck.baseStat;
                }
            }
            else
            {
                damageStat /= actualStat;
            }
        }

        return damageStat;
    }

    public void UseAbility(PocketMonster ownPocketMonster, PocketMonster opponentPocketMonster, PlayerBattle player)
    {
        if (ability.instantEffect)
        {
            if (ability.oneTime)
            {
                if (!ability.hasBeenUsed)
                {
                    ability.hasBeenUsed = true;
                    ability.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
                }
            } else
            {
                ability.UseInstantAbility(ownPocketMonster, opponentPocketMonster, inBattleTextManager);
            }
        } else
        {
            if (ability.oneTime)
            {
                if (!ability.hasBeenUsed)
                {
                    useInAttackAbility = true;
                }
            }
            else
            {
                useInAttackAbility = true;
            }
        }
    }

    public void ResetHealth()
    {
        for (int i = 0; i < stats.typing.Count; i++)
        {
            originalTyping.Add(stats.typing[i]);
        }

        currentStatus = StatusEffects.None;
        ResetStats(false, false);
        fainted = false;
        health = stats.maxHealth;
        ResetPP();
    }

    public void ResetPP()
    {
        for (int i = 0; i < moves.Count; i++)
        {
            moves[i].currentPowerPoints = moves[i].powerPoints;
        }
    }

    public void FillItemsAffectedStatsList(List<PocketMonsterItem> teamBuffs, PlayerBattle player)
    {
        ability = Instantiate(chosenAbility);
        ability.SetAbilityStats(player);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetStats();
        }

        for (int i = 0; i < teamBuffs.Count; i++)
        {
            teamBuffs[i].SetStats();
        }
    }

    public void GetOnSwitchItemEffects(List<PocketMonsterItem> teamBuffs, PlayerBattle player, PocketMonster opponentPocketMonster)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].onSwitch)
            {
                items[i].GrantOnSwitchInEffect(this, opponentPocketMonster, inBattleTextManager, player);
            }
        }

        for (int i = 0; i < teamBuffs.Count; i++)
        {
            if (teamBuffs[i].onSwitch)
            {
                teamBuffs[i].GrantOnSwitchInEffect(this, opponentPocketMonster, inBattleTextManager, player);
            }
        }
    }

    public void GetEveryTurnItemEffects(PlayerBattle player, bool isPlayer, List<PocketMonsterItem> teamBuffs, PocketMonster opponentPocketMonster)
    {
        if (!inBattleTextManager.queFinish)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].everyTurn)
                {
                    items[i].GrantEveryTurnEffect(this, inBattleTextManager);
                }
            }

            for (int i = 0; i < teamBuffs.Count; i++)
            {
                if (teamBuffs[i].everyTurn)
                {
                    teamBuffs[i].GrantEveryTurnEffect(this, inBattleTextManager);
                }
            }

            if (aftermathOfAbility)
            {
                aftermathOfAbility = false;
                ability.UseAbilityAftermath(this, opponentPocketMonster, inBattleTextManager);
            }

            string message = "";

            switch (currentStatus)
            {
                case StatusEffects.Burned:
                    stats.attack.actualStat = stats.attack.GetStatChanges(0) / 2;
                    message += stats.name + " lost some health from the burn.";
                    health -= Mathf.Ceil(stats.maxHealth * 0.05f);
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Bloated:
                    stats.specialAttack.actualStat = stats.specialAttack.GetStatChanges(0) / 2;
                    message += stats.name + " lost some health from being bloated.";
                    health -= Mathf.Ceil(stats.maxHealth * 0.05f);
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Nearsighted:
                    break;
                case StatusEffects.Trapped:
                    message += stats.name + " lost some health from the trap.";
                    health -= Mathf.Ceil(stats.maxHealth * 0.05f);
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Airborne:
                    stats.defense.actualStat = stats.defense.GetStatChanges(0) / 2;
                    stats.specialDefense.actualStat = stats.specialDefense.GetStatChanges(0) / 2;
                    break;
                case StatusEffects.Poisened:
                    message += stats.name + " lost some health from the poison.";
                    health -= Mathf.Ceil(stats.maxHealth * 0.15f);
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Leeched:
                    message += stats.name + " lost some health from leech. ";
                    float healthDetraction = Mathf.Ceil(stats.maxHealth * 0.08f);

                    if (healthDetraction > health)
                    {
                        healthDetraction = health;
                    }

                    health -= healthDetraction;
                    if (!opponentPocketMonster.fainted && opponentPocketMonster.health != opponentPocketMonster.stats.maxHealth)
                    {
                        message += opponentPocketMonster.stats.name + " regained some health.";
                        opponentPocketMonster.health += healthDetraction;
                        opponentPocketMonster.RecalculateHealth();
                    }
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Cursed:
                    message += stats.name + " lost some health from the curse.";
                    health -= Mathf.Ceil(stats.maxHealth * 0.05f);
                    message = CheckIfBelowZeroHealth(message);
                    break;
                case StatusEffects.Paralyzed:
                    stats.speed.actualStat = stats.speed.GetStatChanges(0) / 2;
                    break;
                case StatusEffects.None:
                    stats.speed.actualStat = stats.speed.GetStatChanges(0);
                    stats.defense.actualStat = stats.defense.GetStatChanges(0);
                    stats.specialDefense.actualStat = stats.specialDefense.GetStatChanges(0);
                    stats.attack.actualStat = stats.attack.GetStatChanges(0);
                    stats.specialAttack.actualStat = stats.specialAttack.GetStatChanges(0);
                    break;
                default:
                    break;
            }

            if (message != "")
            {
                if (isPlayer)
                {
                    if (currentStatus != StatusEffects.Leeched || opponentPocketMonster.fainted || 
                        opponentPocketMonster.health == opponentPocketMonster.stats.maxHealth)
                    {
                        inBattleTextManager.QueMessage(message, false, true, true, false);
                    }
                    else
                    {
                        inBattleTextManager.QueMessage(message, true, true, true, false);
                    }
                }
                else
                {
                    if (currentStatus != StatusEffects.Leeched || opponentPocketMonster.fainted ||
                        opponentPocketMonster.health == opponentPocketMonster.stats.maxHealth)
                    {
                        inBattleTextManager.QueMessage(message, true, false, false, true);
                    }
                    else
                    {
                        inBattleTextManager.QueMessage(message, true, true, false, true);
                    }
                }
            }

            if (fainted)
            {
                currentStatus = StatusEffects.None;

                if (ability.onDeath)
                {
                    ability.UseOnDeathAbility(this, opponentPocketMonster, null, inBattleTextManager, isPlayer);
                }
            }

            player.CheckIfBattleOver();
        }
    }

    public void RecalculateStatsAfterStatus()
    {
        switch (currentStatus)
        {
            case StatusEffects.Burned:
                stats.attack.actualStat = stats.attack.GetStatChanges(0) / 2;
                break;
            case StatusEffects.Bloated:
                stats.specialAttack.actualStat = stats.specialAttack.GetStatChanges(0) / 2;
                break;
            case StatusEffects.Nearsighted:
                break;
            case StatusEffects.Airborne:
                stats.defense.actualStat = stats.defense.GetStatChanges(0) / 2;
                stats.specialDefense.actualStat = stats.specialDefense.GetStatChanges(0) / 2;
                break;
            case StatusEffects.Paralyzed:
                stats.speed.actualStat = stats.speed.GetStatChanges(0) / 2;
                break;
            case StatusEffects.None:
                stats.speed.actualStat = stats.speed.GetStatChanges(0);
                stats.defense.actualStat = stats.defense.GetStatChanges(0);
                stats.specialDefense.actualStat = stats.specialDefense.GetStatChanges(0);
                stats.attack.actualStat = stats.attack.GetStatChanges(0);
                stats.specialAttack.actualStat = stats.specialAttack.GetStatChanges(0);
                break;
            default:
                break;
        }
    }

    public void GetBattleTurnItemEffects(List<PocketMonsterItem> teamBuffs, PocketMonsterMoves move, PocketMonster opponentPocketmonster, bool attackTurn)
    {
        if (attackTurn)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].attackTurn)
                {
                    items[i].GrantAttackTurnEffect(this, move, opponentPocketmonster, inBattleTextManager);
                }
            }

            for (int i = 0; i < teamBuffs.Count; i++)
            {
                if (teamBuffs[i].attackTurn)
                {
                    teamBuffs[i].GrantAttackTurnEffect(this, move, opponentPocketmonster, inBattleTextManager);
                }
            }
        } else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].defenseTurn)
                {
                    items[i].GrantAttackTurnEffect(this, move, opponentPocketmonster, inBattleTextManager);
                }
            }

            for (int i = 0; i < teamBuffs.Count; i++)
            {
                if (teamBuffs[i].defenseTurn)
                {
                    teamBuffs[i].GrantAttackTurnEffect(this, move, opponentPocketmonster, inBattleTextManager);
                }
            }
        }
    }

    private string CheckIfBelowZeroHealth(string message)
    {
        if (health <= 0)
        {
            fainted = true;
            message += " " + stats.name + " fainted!";
        }

        return message;
    }

    public void SetAllStats()
    {
        battlePos = transform.position;
        stats.FillBoostableStatsList();

        for (int i = 0; i < stats.boostableStats.Count; i++)
        {
            stats.boostableStats[i].SetBaseStat(stats.baseStats[i]);
        }

        stats.originalCritChance = stats.critChance;
    }

    public void ResetStats(bool resetTyping, bool resetAbilitys)
    {
        if (resetTyping)
        {
            stats.typing = originalTyping;
        }

        if (resetAbilitys)
        {
            aftermathOfAbility = false;
            abilityOnTheirTurn = false;
            useInAttackAbility = false;
        }

        for (int i = 0; i < stats.boostableStats.Count; i++)
        {
            stats.boostableStats[i].ResetStats();
        }

        RecalculateStatsAfterStatus();
        stats.critChance = stats.originalCritChance;
    }

    public void SetInBattleTextManager(InBattleTextManager inBattleTextManager)
    {
        this.inBattleTextManager = inBattleTextManager;
    }

    public float CalculateInTypingWithGivenType(PocketMonsterStats.Typing typeToCompare, PocketMonsterStats.Typing type)
    {
        float damageMultiplier = 1;

        damageMultiplier = GetDamageMultiplier(damageMultiplier, type, typeToCompare);

        return damageMultiplier;
    }

    public void RecalculateHealth()
    {
        if (health > stats.maxHealth)
        {
            health = stats.maxHealth;
        }
    }

    public IEnumerator PlayParticlesAttack(PocketMonster other, PlayerBattle playerBattle, PocketMonsterTextUpdate textUpdate, Text text, bool player)
    {
        inBattleTextManager.canSkipMessage = false;
        isPlayingParticles = true;
        particleSystem.Play();
        ParticleSystem.MainModule mainSytem = particleSystem.main;
        ParticleSystem.EmissionModule emissionSystem = particleSystem.emission;
        ParticleSystem.ShapeModule shapeType = particleSystem.shape;

        mainSytem.startLifetime = 1f;
        mainSytem.startSpeed = new ParticleSystem.MinMaxCurve(-11f, -9f);
        emissionSystem.rateOverTime = 100;
        shapeType.shapeType = ParticleSystemShapeType.Box;
        Vector3 pos = Vector3.zero;
        pos.y = CalculatePos(transform.position.y, other.transform.position.y);
        pos.z = CalculatePos(transform.position.z, other.transform.position.z);

        shapeType.position = pos;

        yield return new WaitForSeconds(1f);
        inBattleTextManager.canSkipMessage = true;
        if (gameObject.activeSelf)
        {
            StartCoroutine(PlayParticlesHit(playerBattle, textUpdate, text, player));
        }
    }

    private float CalculatePos(float pos1, float pos2)
    {
        float pos = 0;
        if (pos1 > pos2)
        {
            pos = pos1 - pos2;
        }
        else
        {
            pos = pos2 - pos1;
        }
        return pos;
    }

    public IEnumerator PlayParticlesHit(PlayerBattle playerBattle, PocketMonsterTextUpdate textUpdate, Text text, bool player)
    {
        playerBattle.UpdatePocketMonsterText(text, textUpdate, player);
        StartCoroutine(Camera.main.GetComponent<CamShake>().Shake(0.15f, 0.5f, 0.05f));

        ParticleSystem.MainModule mainSytem = particleSystem.main;
        ParticleSystem.EmissionModule emissionSystem = particleSystem.emission;
        ParticleSystem.ShapeModule shapeType = particleSystem.shape;
        ParticleSystem.SizeOverLifetimeModule sizeLifeTimeModule = particleSystem.sizeOverLifetime;

        mainSytem.startDelay = 0.5f;
        mainSytem.startLifetime = 0.7f;
        mainSytem.startSpeed = new ParticleSystem.MinMaxCurve(0.25f, 2.5f);
        emissionSystem.rateOverTime = 150;
        shapeType.position = Vector3.zero;
        shapeType.shapeType = ParticleSystemShapeType.Sphere;
        sizeLifeTimeModule.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 1);
        curve.AddKey(1, 0);
        sizeLifeTimeModule.size = new ParticleSystem.MinMaxCurve(1, curve);

        yield return new WaitForSeconds(0.3f);
        particleSystem.Stop();
        StartCoroutine(DeleteParticles());
    }

    public IEnumerator DeleteParticles()
    {
        yield return new WaitForSeconds(0.75f);
        isPlayingParticles = false;
    }

    public void StopParticles()
    {
        StopAllCoroutines();
        particleSystem.Clear();
        particleSystem.Stop();
    }

    private void SetParticleMat(PocketMonsterMoves move, PlayerBattle player)
    {
        SetParticleSystem();

        switch (move.moveType)
        {
            case PocketMonsterStats.Typing.Chemical:
                GetComponent<ParticleSystemRenderer>().material = player.chemicalMat;
                break;
            case PocketMonsterStats.Typing.Earth:
                GetComponent<ParticleSystemRenderer>().material = player.earthMat;
                break;
            case PocketMonsterStats.Typing.Fire:
                GetComponent<ParticleSystemRenderer>().material = player.fireMat;
                break;
            case PocketMonsterStats.Typing.Grass:
                GetComponent<ParticleSystemRenderer>().material = player.grassMat;
                break;
            case PocketMonsterStats.Typing.Light:
                GetComponent<ParticleSystemRenderer>().material = player.lightMat;
                break;
            case PocketMonsterStats.Typing.Regular:
                GetComponent<ParticleSystemRenderer>().material = player.regularMat;
                break;
            case PocketMonsterStats.Typing.Spooky:
                GetComponent<ParticleSystemRenderer>().material = player.spookyMat;
                break;
            case PocketMonsterStats.Typing.Static:
                GetComponent<ParticleSystemRenderer>().material = player.staticMat;
                break;
            case PocketMonsterStats.Typing.Water:
                GetComponent<ParticleSystemRenderer>().material = player.waterMat;
                break;
            case PocketMonsterStats.Typing.Wind:
                GetComponent<ParticleSystemRenderer>().material = player.windMat;
                break;
        }
    }
}
