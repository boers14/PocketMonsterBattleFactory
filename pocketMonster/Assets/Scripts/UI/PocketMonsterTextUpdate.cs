using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterTextUpdate
{
    public PocketMonster pocketMonster;

    public PocketMonsterMoves pocketMonsterMove;

    public PocketMonster.StatusEffects status;

    public string health, player;

    public bool battleConditions, colorText, red, showParticles;

    public PocketMonsterTextUpdate(PocketMonster pocketMonster = null, PocketMonsterMoves move = null, PocketMonster.StatusEffects status = 0,
        string health = "", string player = "", bool battleConditions = false, bool colorText = false, bool red = false, bool showParticles = false)
    {
        this.pocketMonster = pocketMonster;
        pocketMonsterMove = move;
        this.status = status;
        this.health = health;
        this.player = player;
        this.battleConditions = battleConditions;
        this.colorText = colorText;
        this.red = red;
        this.showParticles = showParticles;
    }

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
