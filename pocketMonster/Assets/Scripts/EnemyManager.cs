using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.NonSerialized]
    public GameManager gameManager = null;
    private TerrainManager terrainManager;

    public List<PocketMonsterItem> teamBuffsOfAi = new List<PocketMonsterItem>();

    [System.NonSerialized]
    public int teamsCreated = 0, amountOfPocketMonsters = 1, amountOfMoves = 1, amountOfItems = 0, currentIntelligence = 0;

    [System.NonSerialized]
    public bool addMove = false;

    private bool addSameMoveTypes = false;

    [System.NonSerialized]
    public int amountOfTeamBuffs = 0;

    public enum MovesToGive
    {
        ResistedAndFromWorstStat,
        Resisted,
        Neutral,
        Random,
        Effective
    }

    public MovesToGive movesToGive = MovesToGive.ResistedAndFromWorstStat;

    public virtual List<PocketMonster> createTeamForAi(TrainerAi trainerAi, bool filterStatusMoves)
    {
        CalculateTeamStats();

        List<PocketMonster> possiblePocketMonsters = new List<PocketMonster>();
        possiblePocketMonsters = CreatePossiblePocketMonsterList(possiblePocketMonsters);

        List<PocketMonster> randomListOfPocketMonsters = new List<PocketMonster>();
        List<PocketMonster> enemyPocketMonsterTeam = new List<PocketMonster>();
        for (int i = 0; i < amountOfPocketMonsters; i++)
        {
            PocketMonster randomPocketMonster = ChooseRandomPocketMonster(randomListOfPocketMonsters, possiblePocketMonsters);
            while (randomPocketMonster == null)
            {
                randomPocketMonster = ChooseRandomPocketMonster(randomListOfPocketMonsters, possiblePocketMonsters);
            }
            randomListOfPocketMonsters.Add(randomPocketMonster);

            PocketMonster enemyPocketMonster = Instantiate(randomPocketMonster);

            List<PocketMonsterMoves> randomMoveList = new List<PocketMonsterMoves>();
            for (int j = 0; j < amountOfMoves; j++)
            {
                List<PocketMonsterMoves> movesToSelectFrom = new List<PocketMonsterMoves>();

                if (j == 0)
                {
                    movesToSelectFrom = gameManager.CreateListForStabMoveForPocketMonster(enemyPocketMonster, movesToSelectFrom);
                    movesToSelectFrom = SelectMovesFromMovesList(movesToSelectFrom, enemyPocketMonster, true);
                }
                else
                {
                    movesToSelectFrom = gameManager.GetMovesToChooseFrom(movesToSelectFrom, enemyPocketMonster);
                    movesToSelectFrom = SelectMovesFromMovesList(movesToSelectFrom, enemyPocketMonster, false);
                }

                PocketMonsterMoves move = gameManager.ChooseRandomMove(randomMoveList, movesToSelectFrom);
                while (move == null || move.moveSort == PocketMonsterMoves.MoveSort.Status && filterStatusMoves)
                {
                    move = gameManager.ChooseRandomMove(randomMoveList, movesToSelectFrom);
                    if (!addSameMoveTypes)
                    {
                        if (randomMoveList.Count > 0)
                        {
                            if (move != null)
                            {
                                for (int k = 0; k < randomMoveList.Count; k++)
                                {
                                    if (move == null)
                                    {
                                        continue;
                                    }

                                    if (move.moveType == randomMoveList[k].moveType)
                                    {
                                        move = null;
                                    }
                                }
                            }
                        }
                    }
                }

                randomMoveList.Add(move);
                PocketMonsterMoves addedMove = (PocketMonsterMoves)System.Activator.CreateInstance(move.GetType());
                addedMove.SetMoveStats();
                enemyPocketMonster.moves.Add(addedMove);
            }
            addSameMoveTypes = false;

            List<PocketMonsterItem> pocketMonsterItemsToChooseFrom = new List<PocketMonsterItem>();
            pocketMonsterItemsToChooseFrom = GetPocketMonsterItemsList(pocketMonsterItemsToChooseFrom, enemyPocketMonster);

            for (int j = 0; j < amountOfItems; j++)
            {
                PocketMonsterItem item = pocketMonsterItemsToChooseFrom[Random.Range(0, pocketMonsterItemsToChooseFrom.Count)];

                while (enemyPocketMonster.items.Contains(item))
                {
                    item = pocketMonsterItemsToChooseFrom[Random.Range(0, pocketMonsterItemsToChooseFrom.Count)];
                }

                enemyPocketMonster.items.Add(item);
            }

            enemyPocketMonster.chosenAbility = enemyPocketMonster.possibleAbilitys[Random.Range(0, enemyPocketMonster.possibleAbilitys.Count)];

            enemyPocketMonsterTeam.Add(enemyPocketMonster);
        }

        teamBuffsOfAi.Clear();

        teamBuffsOfAi = CreateTeambuffsList();

        if (trainerAi != null)
        {
            trainerAi.stats.intelligence = currentIntelligence;
        }

        return enemyPocketMonsterTeam;
    }

    public List<PocketMonsterItem> CreateTeambuffsList()
    {
        List<PocketMonsterItem> teamBuffs = new List<PocketMonsterItem>();

        for (int i = 0; i < amountOfTeamBuffs; i++)
        {
            PocketMonsterItem teamBuff = gameManager.allTeamBuffs[Random.Range(0, gameManager.allTeamBuffs.Count)];

            while (teamBuffs.Contains(teamBuff))
            {
                teamBuff = gameManager.allTeamBuffs[Random.Range(0, gameManager.allTeamBuffs.Count)];
            }

            teamBuffs.Add(teamBuff);
        }

        return teamBuffs;
    }

    private List<PocketMonster> CreatePossiblePocketMonsterList(List<PocketMonster> possiblePocketMonsters)
    {
        if (teamsCreated <= 7)
        {
            possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 1, 1, true);

            if (possiblePocketMonsters.Count < amountOfPocketMonsters)
            {
                possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 1, 2, true);
            }

            if (possiblePocketMonsters.Count < amountOfPocketMonsters)
            {
                possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 0.5f, 1, true);
            }
        } else if (teamsCreated <= 20)
        {
            if (teamsCreated >= 12)
            {
                movesToGive = MovesToGive.Random;
            } else
            {
                movesToGive = MovesToGive.Neutral;
            }

            possiblePocketMonsters = gameManager.allPocketMonsters;
        } else
        {
            movesToGive = MovesToGive.Effective;

            possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 1, 1, false);

            if (possiblePocketMonsters.Count < amountOfPocketMonsters)
            {
                possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 1, 0.5f, false);
            }

            if (possiblePocketMonsters.Count < amountOfPocketMonsters)
            {
                possiblePocketMonsters = CreateSpecificTeamList(possiblePocketMonsters, 2, 1, false);
            }
        }

        return possiblePocketMonsters;
    }

    private List<PocketMonster> CreateSpecificTeamList(List<PocketMonster> possiblePocketMonsters, float minimumResist, float minimumWeakness, bool weak)
    {
        for (int i = 0; i < gameManager.allPocketMonsters.Count; i++)
        {
            bool canBeAdded = false;
            for (int j = 0; j < gameManager.playerPocketMonsters.Count; j++)
            {
                float totalDamageMultiplierWeakness = 0;
                float totalDamageMultiplierResist = 0;
                for (int k = 0; k < gameManager.allPocketMonsters[i].stats.typing.Count; k++)
                {
                    float damageMultiplierWeakness = gameManager.playerPocketMonsters[j].CalculateInTyping(gameManager.allPocketMonsters[i].stats.typing[k]);
                    if (damageMultiplierWeakness > totalDamageMultiplierWeakness)
                    {
                        totalDamageMultiplierWeakness = damageMultiplierWeakness;
                    }
                }

                for (int k = 0; k < gameManager.playerPocketMonsters[j].stats.typing.Count; k++)
                {
                    float damageMultiplierResist = gameManager.allPocketMonsters[i].CalculateInTyping(gameManager.playerPocketMonsters[j].stats.typing[k]);
                    if (damageMultiplierResist > totalDamageMultiplierResist)
                    {
                        totalDamageMultiplierResist = damageMultiplierResist;
                    }
                }

                if (weak)
                {
                    if (totalDamageMultiplierWeakness < minimumWeakness && totalDamageMultiplierResist > minimumResist)
                    {
                        canBeAdded = true;
                    }
                } else
                {
                    if (totalDamageMultiplierWeakness > minimumWeakness && totalDamageMultiplierResist < minimumResist)
                    {
                        canBeAdded = true;
                    }
                }
            }

            if (canBeAdded)
            {
                possiblePocketMonsters.Add(gameManager.allPocketMonsters[i]);
            }
        }

        return possiblePocketMonsters;
    }

    private PocketMonster ChooseRandomPocketMonster(List<PocketMonster> randomListOfPocketMonsters, List<PocketMonster> possiblePocketMonsters)
    {
        PocketMonster randomPocketMonster = possiblePocketMonsters[Random.Range(0, possiblePocketMonsters.Count)];

        if (randomListOfPocketMonsters.Contains(randomPocketMonster))
        {
            return null;
        }
        else
        {
            return randomPocketMonster;
        }
    }

    private List<PocketMonsterMoves> SelectMovesFromMovesList(List<PocketMonsterMoves> specificPocketMonsterMoves, PocketMonster pocketMonster, 
        bool isStabMove)
    {
        List<PocketMonsterMoves> oldSpecificPocketMonsterMoves = new List<PocketMonsterMoves>();
        oldSpecificPocketMonsterMoves = specificPocketMonsterMoves;

        switch (movesToGive)
        {
            case MovesToGive.ResistedAndFromWorstStat:
                specificPocketMonsterMoves = CreateBadMoveSet(specificPocketMonsterMoves, pocketMonster);
                addSameMoveTypes = true;
                break;
            case MovesToGive.Resisted:
                specificPocketMonsterMoves = SelectAllPossibleMoveFromList(specificPocketMonsterMoves, oldSpecificPocketMonsterMoves, isStabMove,
                    0.5f, true);
                break;
            case MovesToGive.Neutral:
                specificPocketMonsterMoves = SelectAllPossibleMoveFromList(specificPocketMonsterMoves, oldSpecificPocketMonsterMoves, isStabMove,
                    1f, true);
                break;
            case MovesToGive.Random:
                break;
            case MovesToGive.Effective:
                specificPocketMonsterMoves = SelectAllPossibleMoveFromList(specificPocketMonsterMoves, oldSpecificPocketMonsterMoves, isStabMove,
                    2f, false);
                break;
        }

        return specificPocketMonsterMoves;
    }

    private List<PocketMonsterMoves> SelectAllPossibleMoveFromList(List<PocketMonsterMoves> specificPocketMonsterMoves, 
        List<PocketMonsterMoves> oldSpecificPocketMonsterMoves, bool isStabMove, float neededDamageMultiplier, bool lesserThen)
    {
        specificPocketMonsterMoves = GetMovesFromSpecicMovesList(specificPocketMonsterMoves, neededDamageMultiplier, lesserThen);
        if (!isStabMove)
        {
            int amountOfDifferentTypes = CheckForEnoughMoveTypes(specificPocketMonsterMoves);

            if (specificPocketMonsterMoves.Count < amountOfMoves - 1 || amountOfDifferentTypes < amountOfMoves)
            {
                specificPocketMonsterMoves = GetMovesFromSpecicMovesListBasedOnBool(oldSpecificPocketMonsterMoves, neededDamageMultiplier, lesserThen);
                amountOfDifferentTypes = CheckForEnoughMoveTypes(specificPocketMonsterMoves);
                if (amountOfDifferentTypes < amountOfMoves)
                {
                    addSameMoveTypes = true;
                }
            }
        }
        else if (specificPocketMonsterMoves.Count < 1)
        {
            specificPocketMonsterMoves = oldSpecificPocketMonsterMoves;
        }

        return specificPocketMonsterMoves;
    }

    private int CheckForEnoughMoveTypes(List<PocketMonsterMoves> specificPocketMonsterMoves)
    {
        int amountOfDifferentTypes = 0;
        List<PocketMonsterStats.Typing> typings = new List<PocketMonsterStats.Typing>();
        for (int i = 0; i < specificPocketMonsterMoves.Count; i++)
        {
            if (!typings.Contains(specificPocketMonsterMoves[i].moveType) && specificPocketMonsterMoves[i].moveSort != PocketMonsterMoves.MoveSort.Status)
            {
                typings.Add(specificPocketMonsterMoves[i].moveType);
                amountOfDifferentTypes++;
            }
        }

        return amountOfDifferentTypes;
    }

    private List<PocketMonsterMoves> GetMovesFromSpecicMovesList(List<PocketMonsterMoves> specificPocketMonsterMoves, float neededDamageMultiplier,
        bool lesserThen)
    {
        List<PocketMonsterMoves> newSpecificPocketMonsterMoves = new List<PocketMonsterMoves>();
        for (int i = 0; i < specificPocketMonsterMoves.Count; i++)
        {
            int counter = 0;
            for (int j = 0; j < gameManager.playerPocketMonsters.Count; j++)
            {
                float damageMultiplier = gameManager.playerPocketMonsters[j].CalculateInTyping(specificPocketMonsterMoves[i].moveType);
                if (lesserThen)
                {
                    if (damageMultiplier <= neededDamageMultiplier)
                    {
                        counter++;
                    }
                } else
                {
                    if (damageMultiplier >= neededDamageMultiplier)
                    {
                        counter++;
                    }
                }
            }

            if (counter == gameManager.playerPocketMonsters.Count)
            {
                newSpecificPocketMonsterMoves.Add(specificPocketMonsterMoves[i]);
            }
        }

        return newSpecificPocketMonsterMoves;
    }

    private List<PocketMonsterMoves> GetMovesFromSpecicMovesListBasedOnBool(List<PocketMonsterMoves> specificPocketMonsterMoves, float neededDamageMultiplier,
    bool lesserThen)
    {
        List<PocketMonsterMoves> newSpecificPocketMonsterMoves = new List<PocketMonsterMoves>();
        for (int i = 0; i < specificPocketMonsterMoves.Count; i++)
        {
            bool canBeAdded = false;
            for (int j = 0; j < gameManager.playerPocketMonsters.Count; j++)
            {
                float damageMultiplier = gameManager.playerPocketMonsters[j].CalculateInTyping(specificPocketMonsterMoves[i].moveType);
                if (lesserThen)
                {
                    if (damageMultiplier <= neededDamageMultiplier)
                    {
                        canBeAdded = true;
                    }
                }
                else
                {
                    if (damageMultiplier >= neededDamageMultiplier)
                    {
                        canBeAdded = true;
                    }
                }
            }

            if (canBeAdded)
            {
                newSpecificPocketMonsterMoves.Add(specificPocketMonsterMoves[i]);
            }
        }

        specificPocketMonsterMoves.AddRange(newSpecificPocketMonsterMoves);

        return specificPocketMonsterMoves;
    }

    private List<PocketMonsterMoves> CreateBadMoveSet(List<PocketMonsterMoves> specificPocketMonsterMoves, PocketMonster pocketMonster)
    {
        specificPocketMonsterMoves.Clear();
        for (int i = 0; i < gameManager.allPocketMonstersMoves.Count; i++)
        {
            for (int j = 0; j < pocketMonster.stats.typing.Count; j++)
            {
                if (gameManager.allPocketMonstersMoves[i].moveType == pocketMonster.stats.typing[j])
                {
                    if (pocketMonster.stats.baseAttack > pocketMonster.stats.baseSpecialAttack)
                    {
                        if (gameManager.allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Special)
                        {
                            specificPocketMonsterMoves.Add(gameManager.allPocketMonstersMoves[i]);
                        }
                    }
                    else if (pocketMonster.stats.baseSpecialAttack > pocketMonster.stats.baseAttack)
                    {
                        if (gameManager.allPocketMonstersMoves[i].moveSort == PocketMonsterMoves.MoveSort.Physical)
                        {
                            specificPocketMonsterMoves.Add(gameManager.allPocketMonstersMoves[i]);
                        }
                    }
                    else
                    {
                        if (gameManager.allPocketMonstersMoves[i].moveSort != PocketMonsterMoves.MoveSort.Status)
                        {
                            specificPocketMonsterMoves.Add(gameManager.allPocketMonstersMoves[i]);
                        }
                    }
                }
            }
        }

        return specificPocketMonsterMoves;
    }

    private List<PocketMonsterItem> GetPocketMonsterItemsList(List<PocketMonsterItem> listOfItemsForPocketMonster, PocketMonster pocketMonster)
    {
        List<PocketMonsterItem.ItemSort> typesOfitemsAccepted = new List<PocketMonsterItem.ItemSort>();

        typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.General);

        if (pocketMonster.stats.baseSpeed >= 120)
        {
            typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
            if (pocketMonster.stats.baseAttack > pocketMonster.stats.baseSpecialAttack)
            {
                typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
            }
            else if (pocketMonster.stats.baseAttack < pocketMonster.stats.baseSpecialAttack)
            {
                typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
            }
            else
            {
                typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
                typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
            }
        }

        if (pocketMonster.stats.baseDefense >= 85 || pocketMonster.stats.baseSpecialDefense >= 85 || pocketMonster.stats.maxHealth >= 400)
        {
            typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.Defense);
        }


        if (pocketMonster.stats.baseAttack >= 100 && pocketMonster.stats.baseSpecialAttack >= 100)
        {
            typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
            typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
            typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
        }
        else if (pocketMonster.stats.baseSpeed >= 50)
        {
            if (pocketMonster.stats.baseAttack > pocketMonster.stats.baseSpecialAttack)
            {
                if (pocketMonster.stats.baseSpecialAttack + 15 >= pocketMonster.stats.baseAttack)
                {
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
                }
                else
                {
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
                }
            }
            else
            {
                if (pocketMonster.stats.baseAttack + 15 >= pocketMonster.stats.baseSpecialAttack)
                {
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.PhysicalOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
                }
                else
                {
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.GeneralOffense);
                    typesOfitemsAccepted.Add(PocketMonsterItem.ItemSort.SpecialOffense);
                }
            }
        }

        for (int i = 0; i < gameManager.allPocketMonsterItems.Count; i++)
        {
            if (typesOfitemsAccepted.Contains(gameManager.allPocketMonsterItems[i].itemSort))
            {
                listOfItemsForPocketMonster.Add(gameManager.allPocketMonsterItems[i]);
            }
        }

        return listOfItemsForPocketMonster;
    }

    public virtual void CalculateTeamStats()
    {
        switch (terrainManager.whatToSpawn)
        {
            case TerrainManager.MapChunksToSpawn.SecondMoveSelection:
                amountOfPocketMonsters++;
                break;
            case TerrainManager.MapChunksToSpawn.ThirdPocketMonster:
                amountOfMoves++;
                movesToGive = MovesToGive.Resisted;
                break;
            case TerrainManager.MapChunksToSpawn.StartOfGauntlet:
                break;
            case TerrainManager.MapChunksToSpawn.MidGauntlet:
                if (amountOfPocketMonsters < 4)
                {
                    amountOfPocketMonsters++;
                }
                break;
            case TerrainManager.MapChunksToSpawn.EndOfGuantlet:
                if (amountOfMoves == 3)
                {
                    amountOfMoves++;
                }
                else if (amountOfMoves < 4 && addMove)
                {
                    amountOfMoves++;
                    addMove = false;
                }
                else
                {
                    addMove = true;
                }

                if (amountOfPocketMonsters < 4)
                {
                    amountOfPocketMonsters++;
                }
                break;
        }

        teamsCreated++;

        if (teamsCreated != 0 && teamsCreated % 2 == 0)
        {
            currentIntelligence += 10;
        }

        if (gameManager.lastBattle)
        {
            currentIntelligence = 100;
        }

        if (teamsCreated != 0 && teamsCreated % 6 == 0 && amountOfItems < 3)
        {
            amountOfItems++;
        }
    }

    public void LoadSavedStats(PlayerData data)
    {
        teamsCreated = data.teamsCreated;
        amountOfPocketMonsters = data.amountOfPocketMonsters;
        amountOfMoves = data.amountOfMoves;
        amountOfItems = data.amountOfItems;
        currentIntelligence = data.currentIntelligence;
        amountOfTeamBuffs = data.amountOfTeamBuffs;
        addMove = data.addMove;
        movesToGive = (MovesToGive)System.Enum.Parse(typeof(MovesToGive), data.movesToGive);
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void SetTerrainManager(TerrainManager terrainManager)
    {
        this.terrainManager = terrainManager;
    }
}
