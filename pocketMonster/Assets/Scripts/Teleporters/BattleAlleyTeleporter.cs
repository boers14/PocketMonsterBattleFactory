using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAlleyTeleporter : LandingTeleporter
{
    public Transform connectedSpawnPlace { get; set; } = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Vector3 teleportedPos = connectedSpawnPlace.position;
            teleportedPos.y += 1;
            collision.gameObject.transform.position = teleportedPos;
        }
    }
}
