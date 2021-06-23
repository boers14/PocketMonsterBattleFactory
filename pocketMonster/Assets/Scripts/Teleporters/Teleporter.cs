using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : LandingTeleporter
{
    public Transform connectedSpawnPlace = null;

    public bool setData = true, teleport = true, lastTeleporter = false;

    private GameManager gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && teleport && gameManager.playerPocketMonsters.Count > 0)
        {
            gameManager.ClearMergants();

            if (setData)
            {
                connectedSpawnPlace.GetComponent<BattleLandingTeleporter>().SetTeleporterData(spawnPosition);
            }

            Vector3 teleportedPos = connectedSpawnPlace.position;
            teleportedPos.y += 1;
            collision.gameObject.transform.position = teleportedPos;

            if (lastTeleporter)
            {
                gameManager.lastBattle = true;
            }
        }
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
