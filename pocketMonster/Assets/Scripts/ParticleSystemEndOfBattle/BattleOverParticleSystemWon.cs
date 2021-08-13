using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOverParticleSystemWon : BattleOverParticleSystems
{
    public override void SetParticleSystemForPlayer()
    {
        player.GetComponent<PlayerBattle>().winParticleSystem = GetComponent<ParticleSystem>();
    }
}
