using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOverParticleSystems : MonoBehaviour
{
    [System.NonSerialized]
    public Transform player = null, mainCamera = null;

    [SerializeField]
    private Vector3 offsetFromCam = Vector3.zero;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (!player)
        {
            GameObject possiblePlayer = GameObject.FindGameObjectWithTag("Player");
            if (possiblePlayer != null)
            {
                player = possiblePlayer.transform;
                SetParticleSystemForPlayer();
            }
            return;
        }

        Vector3 nextPos = mainCamera.position;
        nextPos += mainCamera.forward * offsetFromCam.z;
        nextPos += mainCamera.up * offsetFromCam.y;
        transform.position = nextPos;
    }

    public virtual void SetParticleSystemForPlayer()
    {

    }
}
