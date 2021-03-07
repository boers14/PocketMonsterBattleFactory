using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterTextUpdate
{
    public PocketMonster pocketMonster;

    public PocketMonster.StatusEffects status;

    public string health, player;

    public bool battleConditions, colorText, red, showParticles;

    public void BecomeCopyOfTextUpdate(PocketMonsterTextUpdate textUpdate)
    {
        pocketMonster = textUpdate.pocketMonster;
        player = textUpdate.player;
        health = textUpdate.health;
        battleConditions = textUpdate.battleConditions;
        status = textUpdate.status;
        colorText = textUpdate.colorText;
        red = textUpdate.red;
        showParticles = false;
    }
}
