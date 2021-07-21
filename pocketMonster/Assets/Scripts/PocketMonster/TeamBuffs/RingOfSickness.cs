using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfSickness : PocketMonsterItem
{
    public override void SetStats()
    {
        onSwitch = true;
        name = "Ring of sickness";
        itemDescription = "Inflicts a random status on the opponent on switch in if it had no status.";
        startOfBattleMessage = "pocketmonsters will status the opposing team when switching in, when they have none.";
    }

    public override void GrantOnSwitchInEffect(PocketMonster effectedPocketMonster, PocketMonster opponentPocketMonster, InBattleTextManager inBattleTextManager, PlayerBattle player)
    {
        if (opponentPocketMonster.currentStatus == PocketMonster.StatusEffects.None)
        {
            int randomStatus = Random.Range(1, System.Enum.GetNames(typeof(PocketMonster.StatusEffects)).Length);
            PocketMonster.StatusEffects status = PocketMonster.StatusEffects.None;
            status -= randomStatus;
            opponentPocketMonster.currentStatus = status;
            inBattleTextManager.QueMessage(effectedPocketMonster.stats.name + " inflicted the " + status + " status on the opponent due to the " + name
                + ".", false, false, false, false);
        }
    }
}
