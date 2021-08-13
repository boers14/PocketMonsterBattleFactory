using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuParticles : MonoBehaviour
{
    public Particle particleToSpawn = null;

    [SerializeField]
    private Canvas canvas = null;

    [System.NonSerialized]
    public bool particleSystemIsOn = true;

    private float maxRangeToSpawn = 0, timer = 0;

    [SerializeField]
    private float chanceOfParticleSpawn = 0, particleSpawnInterval = 0, backWardsOffset = 10;

    private List<Particle> particles = new List<Particle>();

    private void Start()
    {
        maxRangeToSpawn = canvas.GetComponent<RectTransform>().sizeDelta.x + (canvas.GetComponent<RectTransform>().sizeDelta.x / backWardsOffset * 2);

        SetUIStats.SetUIPosition(canvas, gameObject, -backWardsOffset, -backWardsOffset, 2, 2, 1, 1);
    }

    private void FixedUpdate()
    {
        if (!particleSystemIsOn) { return; }

        if (timer <= 0)
        {
            timer = particleSpawnInterval;
            if ((int)Random.Range(1, 101) <= chanceOfParticleSpawn)
            {
                Particle newParticle = Instantiate(particleToSpawn);
                newParticle.transform.SetParent(canvas.transform);
                newParticle.menuParticles = this;

                Vector3 newPos = GetComponent<RectTransform>().position;
                newPos.x += (float)Random.Range(0, maxRangeToSpawn);
                newParticle.GetComponent<RectTransform>().position = newPos;

                particles.Add(newParticle);
            }
        }

        timer -= Time.fixedDeltaTime;
    }

    public void RemoveParticleFromList(Particle particle)
    {
        particles.Remove(particle);
        Destroy(particle.gameObject);
    }
}
