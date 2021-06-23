using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private Transform player = null;

    [SerializeField]
    private float distanceXZ = 0, distanceY = 0, smoothPos = 0, smoothRot = 0, lookDownFactor = 0;

    private List<GameObject> allObjectsInFrontOfCam = new List<GameObject>();

    void FixedUpdate()
    {
        if (!player)
        {
            GameObject possiblePlayer = GameObject.FindGameObjectWithTag("Player");
            if (possiblePlayer != null)
            {
                player = possiblePlayer.transform;
            }
            return;
        }

        Vector3 lookRotation = Quaternion.LookRotation(player.forward, Vector3.up).eulerAngles;
        Vector3 newRotation = new Vector3(lookDownFactor, lookRotation.y, 0);
        Quaternion newRotationInQuaternion = Quaternion.Euler(newRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotationInQuaternion, Time.deltaTime * smoothRot);

        transform.position = Vector3.Slerp(transform.position, new Vector3(player.transform.position.x + -player.transform.forward.x * distanceXZ,
            player.transform.position.y + distanceY, player.transform.position.z + -player.transform.forward.z * distanceXZ), Time.deltaTime * smoothPos);

        for (int i = allObjectsInFrontOfCam.Count - 1; i >= 0; i--)
        {
            allObjectsInFrontOfCam[i].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            allObjectsInFrontOfCam.RemoveAt(i);
        }

        float dist = Vector3.Distance(transform.position, player.transform.position) / 1.2f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, dist))
        {
            if (hit.transform.tag == "EnvironmentObject")
            {
                allObjectsInFrontOfCam.Add(hit.transform.gameObject);
                hit.transform.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
