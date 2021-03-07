using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private Transform player = null;

    [SerializeField]
    private float distanceXZ = 0, distanceY = 0, smoothPos = 0, smoothRot = 0, lookDownFactor = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player)
        {
            GameObject[] possiblePlayer = GameObject.FindGameObjectsWithTag("Player");
            if (possiblePlayer.Length > 0)
            {
                player = possiblePlayer[0].transform;
            }
            return;
        }

        Vector3 newRotation = Vector3.zero;
        newRotation += transform.eulerAngles;
        newRotation += player.eulerAngles;
        newRotation /= 2;

        if (player.eulerAngles.y - transform.localEulerAngles.y < -300)
        {
            newRotation.y = 360 + (player.eulerAngles.y / 4f);
        }
        else if (player.eulerAngles.y - transform.localEulerAngles.y > 300)
        {
            newRotation.y = (player.eulerAngles.y - 360) / 4f;
        }

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(lookDownFactor, newRotation.y, 0), Time.deltaTime * smoothRot);

        transform.position = Vector3.Slerp(transform.position, new Vector3(player.transform.position.x + -player.transform.forward.x * distanceXZ,
            player.transform.position.y + distanceY, player.transform.position.z + -player.transform.forward.z * distanceXZ), Time.deltaTime * smoothPos);
    }
}
