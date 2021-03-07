using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InBattleTextManager : MonoBehaviour
{
    public PlayerBattle player;
    private PocketMonster currentAiPocketMonster;

    private Vector3 pos = Vector3.zero;
    private Vector2 size = Vector2.zero;

    [SerializeField]
    private Image image = null;
    private Image textBg = null;

    [SerializeField]
    private Text text = null;
    private Text info = null;

    [SerializeField]
    private float textDisplayTime = 3;

    private List<string> dialogueLines = new List<string>();
    public List<PocketMonsterTextUpdate> playerInformations = new List<PocketMonsterTextUpdate>();
    private List<PocketMonsterTextUpdate> aiInformations = new List<PocketMonsterTextUpdate>();

    private bool isDisplayingMessages = false, playerWon = false;
    public bool queFinish = false, canSkipMessage = true;
    private int messagesSkipped = 0;
    bool setParticleEffects = false;

    private void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");

        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 2;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 4;

        pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2;

        textBg = Instantiate(image);
        textBg.transform.SetParent(canvas.transform);
        textBg.rectTransform.sizeDelta = size;
        textBg.rectTransform.localPosition = pos;
        textBg.color = Color.white;

        info = Instantiate(text);
        info.transform.SetParent(canvas.transform);
        info.rectTransform.sizeDelta = size;
        info.rectTransform.localPosition = pos;
        info.alignment = TextAnchor.MiddleCenter;

        EnableText(false);
    }

    private void FixedUpdate()
    {
        if (dialogueLines.Count > 0 && !isDisplayingMessages)
        {
            player.EnableButtons(false, true);
            isDisplayingMessages = true;
            StartCoroutine(DisplayMessages());
        }
    }

    private void Update()
    {
        if (dialogueLines.Count > 0 && isDisplayingMessages && canSkipMessage)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerInformations[0].pocketMonster.StopParticles();
                aiInformations[0].pocketMonster.StopParticles();
                messagesSkipped++;
                QueNextMessage();
            }
        }
    }

    public void QueBattleMessages(PocketMonster pocketMonster, PocketMonsterMoves move, int damageDone, float damageMultiplier, bool critical, bool fainted,
        PocketMonster opponentPocketmonster, bool missed, bool isPlayer)
    {
        setParticleEffects = true;

        string battleMessage = "";

        string effectiveness = "";

        string damageDoneText = "";

        if (!missed)
        {
            damageDoneText = " did " + damageDone + " damage";

            if (damageMultiplier > 1)
            {
                effectiveness = " Its super effective!";
            }
            else if (damageMultiplier < 1)
            {
                effectiveness = " Its not very effective.";
            }
        } else
        {
            damageDoneText = " missed";
        }

        battleMessage = pocketMonster.stats.name + damageDoneText + " with the move " + move.moveName + "." + effectiveness;

        if (!missed)
        {
            if (critical)
            {
                battleMessage += " Its a critical hit!";
            }
        }

        if (fainted)
        {
            battleMessage += " " + opponentPocketmonster.stats.name + " fainted!";
        }

        dialogueLines.Add(battleMessage);

        if (isPlayer)
        {
            if (damageDone > 0)
            {
                player.QuePocketMonsterTextUpdate(player.currentPocketMonster.health, setParticleEffects, true);
            } else
            {
                player.QuePocketMonsterTextUpdate(player.currentPocketMonster.health, false, false);
            }
            player.QueOpponentPocketMonsterUpdate(player.opponentPocketMonster.health, false, false);
        } else
        {
            if (damageDone > 0)
            {
                player.QueOpponentPocketMonsterUpdate(player.opponentPocketMonster.health, setParticleEffects, true);
            }
            else
            {
                player.QueOpponentPocketMonsterUpdate(player.opponentPocketMonster.health, false, false);
            }
            player.QuePocketMonsterTextUpdate(player.currentPocketMonster.health, false, false);
        }

    }

    public void QueMessage(string message, bool colorOpponentHealth, bool colorPlayerHealth, bool playerRed, bool opponentRed)
    {
        setParticleEffects = false;
        player.QuePocketMonsterTextUpdate(player.currentPocketMonster.health, setParticleEffects, colorPlayerHealth, playerRed);
        player.QueOpponentPocketMonsterUpdate(player.opponentPocketMonster.health, setParticleEffects, colorOpponentHealth, opponentRed);

        dialogueLines.Add(message);
    }

    public void AddPlayerUpdate(PocketMonster pocketMonster, PocketMonster.StatusEffects status, string health, string player, bool battleConditions, 
        bool colorText, bool red, bool particles)
    {
        PocketMonsterTextUpdate pocketMonsterTextUpdate = new PocketMonsterTextUpdate();
        pocketMonsterTextUpdate = SetPocketMonsterTextUpdate(pocketMonsterTextUpdate, pocketMonster, status, health, player, battleConditions, colorText, 
            red, particles);
        playerInformations.Add(pocketMonsterTextUpdate);
    }

    public void AddAiupdate(PocketMonster pocketMonster, PocketMonster.StatusEffects status, string health, string player, bool battleConditions,
        bool colorText, bool red, bool particles)
    {
        PocketMonsterTextUpdate pocketMonsterTextUpdate = new PocketMonsterTextUpdate();
        pocketMonsterTextUpdate = SetPocketMonsterTextUpdate(pocketMonsterTextUpdate, pocketMonster, status, health, player, battleConditions, colorText,
            red, particles);
        aiInformations.Add(pocketMonsterTextUpdate);
    }

    private PocketMonsterTextUpdate SetPocketMonsterTextUpdate(PocketMonsterTextUpdate pocketMonsterTextUpdate, PocketMonster pocketMonster, 
        PocketMonster.StatusEffects status, string health, string player, bool battleConditions, bool colorText, bool red, bool particles)
    {
        pocketMonsterTextUpdate.pocketMonster = pocketMonster;
        pocketMonsterTextUpdate.player = player;
        pocketMonsterTextUpdate.health = health;
        pocketMonsterTextUpdate.battleConditions = battleConditions;
        pocketMonsterTextUpdate.status = status;
        pocketMonsterTextUpdate.colorText = colorText;
        pocketMonsterTextUpdate.red = red;
        pocketMonsterTextUpdate.showParticles = particles;

        return pocketMonsterTextUpdate;
    }

    public IEnumerator DisplayMessages()
    {
        EnableText(true);
        textBg.transform.SetAsFirstSibling();
        info.text = dialogueLines[0];

        player.UpdatePlayerText(playerInformations[0]);
        PocketMonsterTextUpdate playerUpdate = new PocketMonsterTextUpdate();
        playerUpdate.BecomeCopyOfTextUpdate(playerInformations[0]);
        player.playerUpdate = playerUpdate;
        player.playerUpdate.colorText = false;
        player.playerUpdate.showParticles = false;

        player.UpdateOpponentText(aiInformations[0]);
        PocketMonsterTextUpdate aiUpdate = new PocketMonsterTextUpdate();
        aiUpdate.BecomeCopyOfTextUpdate(aiInformations[0]);
        player.aiUpdate = aiUpdate;
        player.aiUpdate.colorText = false;
        player.aiUpdate.showParticles = false;

        if (currentAiPocketMonster != aiInformations[0].pocketMonster)
        {
            if (currentAiPocketMonster != null)
            {
                currentAiPocketMonster.gameObject.SetActive(false);
            }
            currentAiPocketMonster = aiInformations[0].pocketMonster;
            currentAiPocketMonster.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(textDisplayTime);

        if (dialogueLines.Count > 0 && messagesSkipped <= 0)
        {
            QueNextMessage();
        } else if (messagesSkipped > 0)
        {
            messagesSkipped--;
        }

    }

    private void QueNextMessage()
    {
        dialogueLines.RemoveAt(0);
        playerInformations.RemoveAt(0);
        aiInformations.RemoveAt(0);

        if (dialogueLines.Count > 0)
        {
            StartCoroutine(DisplayMessages());
        }
        else
        {
            EnableText(false);
            isDisplayingMessages = false;

            if (queFinish)
            {
                StartCoroutine(player.SetVictoryText(playerWon));
                currentAiPocketMonster = null;
                queFinish = false;
            }
            else
            {
                if (player.currentPocketMonster.fainted)
                {
                    player.SetSwitchButtonsForWhenPocketMonsterIsFainted();
                }
                else if (player.isInAbilityMenu)
                {
                    player.enableAbilityMenu(true);
                }
                else
                {
                    player.EnableButtons(true, false);
                }
            }
        }
    }

    public void QueFinish(bool queFinish, bool playerWon)
    {
        this.queFinish = queFinish;
        this.playerWon = playerWon;
    }

    public void EnableText(bool enable)
    {
        info.enabled = enable;
        textBg.enabled = enable;
    }

    public void SetPlayer(PlayerBattle player)
    {
        this.player = player;
    }
}
