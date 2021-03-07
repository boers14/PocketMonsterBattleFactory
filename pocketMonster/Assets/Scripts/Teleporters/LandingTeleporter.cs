using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingTeleporter : MonoBehaviour
{
    public enum SpawnPosition
    {
        Left,
        Middle,
        Right,
        OutOfReach
    }

    public SpawnPosition spawnPosition;

    public int chunkIndex;
}
