using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mergant : MonoBehaviour
{
    public GameObject player = null;

    public float meetDistance = 0;

    public bool wantsToGive = true;

    public GameManager gameManager;

    public virtual void FixedUpdate()
    {
        if (!player)
        {
            GameObject[] possiblePlayer = GameObject.FindGameObjectsWithTag("Player");
            if (possiblePlayer.Length > 0)
            {
                player = possiblePlayer[0];
            }
            return;
        }
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
