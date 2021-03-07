using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldTrainer : MonoBehaviour
{
    public GameObject player = null;

    private Camera cam = null;

    [SerializeField]
    private float meetDistance = 0;

    private bool wantsBattle = true;

    private Vector3 meetPosInWorld = Vector3.zero, meetRotInWorld = Vector3.zero, originalPos = Vector3.zero;

    [SerializeField]
    private Vector3 ownBattlePos = Vector3.zero, playerBattlePos = Vector3.zero;

    private GameManager gameManager;

    private EnemyManager enemyManager;

    private TerrainManager terrainManager;

    private List<PocketMonster> pocketMonsterTeam = new List<PocketMonster>();

    private void FixedUpdate()
    {
        if (!player)
        {
            GameObject[] possiblePlayer = GameObject.FindGameObjectsWithTag("Player");
            if (possiblePlayer.Length > 0)
            {
                player = possiblePlayer[0];
                cam = Camera.main;
            }
            return;
        }

        if (wantsBattle) {
            if (Vector3.Distance(transform.position, player.transform.position) < meetDistance)
            {
                originalPos = transform.position;
                meetPosInWorld = player.transform.position;
                meetRotInWorld = player.transform.eulerAngles;
                GoToBattleArena();
                wantsBattle = false;
            }
        }
    }

    public void GoToBattleArena(bool playerWon = true)
    {
        if (wantsBattle)
        {
            gameManager.livesText.gameObject.SetActive(false);
            transform.position = ownBattlePos;
            cam.transform.position = playerBattlePos;
            pocketMonsterTeam = enemyManager.createTeamForAi(GetComponent<TrainerAi>());
            GetComponent<TrainerAi>().AddpocketMonsters(pocketMonsterTeam, enemyManager.teamBuffsOfAi, player.GetComponent<PlayerBattle>());
            LoadPlayerStats();
        } else
        {
            gameManager.livesText.gameObject.SetActive(true);

            for (int i = 0; i < gameManager.playerPocketMonsters.Count; i++)
            {
                gameManager.playerPocketMonsters[i].moves = player.GetComponent<PlayerBattle>().pocketMonsterMoves[i];
            }

            if (!playerWon)
            {
                gameManager.HandleLives();
            }

            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<PlayerBattle>().enabled = false;

            if (gameManager.lastBattle && playerWon)
            {
                gameManager.SwitchToStartScreen();
            }

            for (int i = 0; i < GetComponent<TrainerAi>().pocketMonsters.Count; i++)
            {
                GetComponent<TrainerAi>().pocketMonsters[i].gameObject.SetActive(false);
            }

            if (gameManager.lastBattle)
            {
                if (playerWon)
                {
                    Destroy(gameObject);
                    player.transform.position = meetPosInWorld;
                    cam.transform.position = meetPosInWorld;
                    player.transform.eulerAngles = meetRotInWorld;
                } else
                {
                    wantsBattle = true;
                    transform.position = originalPos;

                    Vector3 newPos = transform.position;
                    newPos.z -= 7;
                    player.transform.position = newPos;
                    cam.transform.position = newPos;
                    Vector3 newRot = transform.eulerAngles;
                    newRot.y -= 180;
                    player.transform.eulerAngles = newRot;
                }
            } else
            {
                Destroy(gameObject);
                player.transform.position = meetPosInWorld;
                cam.transform.position = meetPosInWorld;
                player.transform.eulerAngles = meetRotInWorld;
            }
        }
    }

    private void LoadPlayerStats()
    {
        player.transform.position = playerBattlePos;
        player.transform.rotation = Quaternion.identity;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.Rotate(0, 0, 0);
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerBattle>().enabled = true;
        player.GetComponent<PlayerBattle>().SetOpponentTrainer(GetComponent<TrainerAi>());
        player.GetComponent<PlayerBattle>().SetOpponentPocketMonster(GetComponent<TrainerAi>().currentPocketMonster);
        player.GetComponent<PlayerBattle>().AddpocketMonster(gameManager.playerPocketMonsters, gameManager.teamBuffsOfPlayer);
        player.GetComponent<PlayerBattle>().CreateUI();
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void SetEnemyManager(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
    }

    public void SetTerrainManager(TerrainManager terrainManager)
    {
        this.terrainManager = terrainManager;
        SetPlayerBattlePosAndOwnBattlePosCorrect();
    }

    private void SetPlayerBattlePosAndOwnBattlePosCorrect()
    {
        playerBattlePos = terrainManager.arenaPos + playerBattlePos;
        ownBattlePos = terrainManager.arenaPos + ownBattlePos;
    }
}
