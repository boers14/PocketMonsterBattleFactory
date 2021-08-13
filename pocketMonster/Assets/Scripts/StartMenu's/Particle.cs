using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField]
    private float timeAlive = 5, speed = 0.05f, swingAmount = 0.2f, swingSpeed = 10;

    [System.NonSerialized]
    public MenuParticles menuParticles = null;

    private RectTransform rectTransform = null;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        Vector3 newPos = rectTransform.position;
        newPos.y += speed;
        newPos.x += Mathf.Sin(timeAlive * swingAmount) * swingSpeed;
        rectTransform.position = newPos;
        
        timeAlive -= Time.deltaTime;
        if (timeAlive <= 0)
        {
            menuParticles.RemoveParticleFromList(this);
        }
    }
}
