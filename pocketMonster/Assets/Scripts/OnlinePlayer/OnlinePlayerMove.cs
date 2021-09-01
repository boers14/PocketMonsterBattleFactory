using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerMove
{
    public int playerIndex, moveIndex, switchIndex;

    public bool wantsSwitch;

    public OnlinePlayerMove(int playerIndex, int moveIndex, int switchIndex, bool wantsSwitch)
    {
        this.playerIndex = playerIndex;
        this.moveIndex = moveIndex;
        this.switchIndex = switchIndex;
        this.wantsSwitch = wantsSwitch;
    }
}
