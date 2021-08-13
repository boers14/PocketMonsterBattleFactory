using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOverParticleSystemLost : BattleOverParticleSystems
{
    public override void SetParticleSystemForPlayer()
    {
        player.GetComponent<PlayerBattle>().lostParticleSystem = GetComponent<ParticleSystem>();
    }
}
