using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : MonoBehaviour
{
    public PocketMonster currentPocketMonster, opponentPocketMonster, previousPocketMonster;

    public List<PocketMonster> pocketMonsters = new List<PocketMonster>();

    [SerializeField]
    private Button buttonToSpawn = null, regularButtonToSpawn = null;

    private List<Button> actionButtons = new List<Button>(), switchButtons = new List<Button>(), useAbilityButtons = new List<Button>();

    [SerializeField]
    private Text prefabText = null, hoverText = null;

    private Text pocketMonsterInformation = null, opponentPocketMonsterInformation = null, endOfBattleText = null, askAbilityText = null,
        pocketMonsterAbilityText = null, opponentPocketMonsterAbilityText = null;

    public TrainerAi opponentTrainer = null;

    public bool showAll = false, currentPocketmonsterIsFainted = true, isInAbilityMenu = false, lastPocketmonsterFainted = false, 
        healNextPocketMonster = false;

    [SerializeField]
    private float disFromPlayer = 0;

    public List<PocketMonsterItem> teamBuffs = new List<PocketMonsterItem>();

    private InBattleTextManager inBattleTextManager;

    public List<List<PocketMonsterMoves>> pocketMonsterMoves = new List<List<PocketMonsterMoves>>();

    public PocketMonsterTextUpdate playerUpdate, aiUpdate;

    private int coloredTextPlayerCounter = 0, coloredTextAiCounter = 0;

    public Sprite chemicalSprite = null, staticSprite = null, waterSprite = null, grassSprite = null, fireSprite = null,
        spookySprite = null, earthSprite = null, lightSprite = null, regularSprite = null, windSprite = null;

    public Material chemicalMat = null, staticMat = null, waterMat = null, grassMat = null, fireMat = null,
        spookyMat = null, earthMat = null, lightMat, regularMat = null, windMat = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            showAll = true;
            UpdatePlayerText(playerUpdate);
            UpdateOpponentText(aiUpdate);
        } else if (Input.GetKeyUp(KeyCode.Z))
        {
            showAll = false;
            UpdatePlayerText(playerUpdate);
            UpdateOpponentText(aiUpdate);
        }
    }

    public void AddpocketMonster(List<PocketMonster> pocketMonstersThisBattle, List<PocketMonsterItem> teamBuffsThisBattle)
    {
        teamBuffs.Clear();
        for (int i = 0; i < teamBuffsThisBattle.Count; i++)
        {
            teamBuffs.Add(teamBuffsThisBattle[i]);
        }

        pocketMonsterMoves.Clear();

        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            Destroy(pocketMonsters[i].gameObject);
        }
        pocketMonsters.Clear();

        for (int i = 0; i < pocketMonstersThisBattle.Count; i++)
        {
            PocketMonster newPocketMonster = Instantiate(pocketMonstersThisBattle[i]);
            newPocketMonster.moves = pocketMonstersThisBattle[i].moves;
            newPocketMonster.items = pocketMonstersThisBattle[i].items;
            newPocketMonster.transform.position = new Vector3(transform.position.x + transform.forward.x * disFromPlayer, transform.position.y,
                transform.position.z + transform.forward.z * disFromPlayer);
            newPocketMonster.transform.eulerAngles = transform.eulerAngles;
            newPocketMonster.SetAllStats();

            List<PocketMonsterMoves> savedMoves = new List<PocketMonsterMoves>();
            for (int j = 0; j < newPocketMonster.moves.Count; j++)
            {
                PocketMonsterMoves moveToSave = (PocketMonsterMoves)System.Activator.CreateInstance(newPocketMonster.moves[j].GetType());
                moveToSave.SetMoveStats();
                savedMoves.Add(moveToSave);
            }
            pocketMonsterMoves.Add(savedMoves);

            newPocketMonster.gameObject.SetActive(false);
            pocketMonsters.Add(newPocketMonster);
            pocketMonsters[i].ResetHealth();
            pocketMonsters[i].SetInBattleTextManager(inBattleTextManager);
            pocketMonsters[i].FillItemsAffectedStatsList(teamBuffs, this);
        }

        currentPocketMonster = pocketMonsters[0];
        currentPocketMonster.gameObject.SetActive(true);
        previousPocketMonster = currentPocketMonster;
    }

    public void CreateUI()
    {
        GameObject canvas = GameObject.Find("Canvas");

        CreateOwnPocketMosterUI();

        Text newInfo = Instantiate(prefabText);
        newInfo.color = Color.white;
        pocketMonsterInformation = newInfo;
        SetUIPosition(pocketMonsterInformation.gameObject, 4, 2.5f, 2, 2, 1, 1);

        pocketMonsterAbilityText = InstantiateAbilityText(canvas, pocketMonsterInformation, true, currentPocketMonster);

        pocketMonsterInformation.text = SetInformation(currentPocketMonster, currentPocketMonster.health.ToString(), "You", true,
            currentPocketMonster.currentStatus, showAll, true, true, false);

        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            Button newButton = Instantiate(buttonToSpawn);
            switchButtons.Add(newButton);

            if (i == 0)
            {
                newButton.GetComponentInChildren<Text>().text = "Active";
            }
            else
            {
                newButton.GetComponentInChildren<Text>().text = pocketMonsters[i].stats.name;
            }

            newButton.GetComponent<HoverableUiElement>().SetYSize(3);
            newButton.GetComponent<HoverableUiElement>().SetText(SetInformation(pocketMonsters[i], pocketMonsters[i].health.ToString(),
                "Pocketmonster" + (i + 1), true, pocketMonsters[i].currentStatus, true, false, true, true));
            SetSwitchButtonOnClick(i);
            SetUIPosition(newButton.gameObject, 4, 12, 2, 0, 1, 1, i, true, false);
        }

        Text opponentInfo = Instantiate(prefabText);
        opponentInfo.color = Color.white;
        opponentInfo.alignment = TextAnchor.UpperRight;
        opponentPocketMonsterInformation = opponentInfo;
        SetUIPosition(opponentPocketMonsterInformation.gameObject, 4, 2.5f, 2, 2, 1, -1);

        opponentPocketMonsterAbilityText = InstantiateAbilityText(canvas, opponentPocketMonsterInformation, false, opponentPocketMonster);

        opponentPocketMonsterInformation.text = SetInformation(opponentPocketMonster, opponentPocketMonster.health.ToString(), "Opponent", true,
            opponentPocketMonster.currentStatus, showAll, true, false, false);

        for (int i = 0; i < teamBuffs.Count; i++)
        {
            if (teamBuffs[i].onSwitch)
            {
                inBattleTextManager.QueMessage("You're team's " + teamBuffs[i].startOfBattleMessage, false, false, false, false);
            }
        }
        opponentTrainer.CreateTeamBuffMessages();

        currentPocketMonster.GetOnSwitchItemEffects(teamBuffs, this, opponentPocketMonster);
        opponentPocketMonster.GetOnSwitchItemEffects(opponentTrainer.stats.teamBuffs, this, currentPocketMonster);

        PocketMonsterTextUpdate playerTextUpdate = new PocketMonsterTextUpdate();
        playerTextUpdate = SetPocketMonsterTextUpdate(currentPocketMonster, currentPocketMonster.currentStatus, currentPocketMonster.health.ToString(), 
            "You", true, false, false, playerTextUpdate);
        playerUpdate = playerTextUpdate;

        PocketMonsterTextUpdate aiTextUpdate = new PocketMonsterTextUpdate();
        aiTextUpdate = SetPocketMonsterTextUpdate(opponentPocketMonster, opponentPocketMonster.currentStatus, opponentPocketMonster.health.ToString(),
            "Opponent", true, false, false, aiTextUpdate);
        aiUpdate = aiTextUpdate;

        Text abilityText = Instantiate(prefabText);
        abilityText.color = Color.white;
        askAbilityText = abilityText;
        SetUIPosition(askAbilityText.gameObject, 3, 6, 0, 2, 1, 1);

        for (int i = 0; i < 2; i++)
        {
            Button newButton = Instantiate(regularButtonToSpawn);
            useAbilityButtons.Add(newButton);

            if (i == 0)
            {
                newButton.GetComponentInChildren<Text>().text = "Yes";
            } else
            {
                newButton.GetComponentInChildren<Text>().text = "No";
            }

            SetUseAbilityOnClick(i);
            SetUIPosition(newButton.gameObject, 2, 8, 2, 2, -1, 1, i, false, true);
        }
        

        if (!currentPocketMonster.ability.onDeath)
        {
            enableAbilityMenu(true);
        } else
        {
            opponentTrainer.UseAbilityForPocketMonster(this, currentPocketMonster);
            currentPocketmonsterIsFainted = false;
            opponentTrainer.firstTurn = false;
            enableAbilityMenu(false);
        }
    }

    private void SetUseAbilityOnClick(int index)
    {
        useAbilityButtons[index].onClick.AddListener(() => OnAbilityUsage(index));
    }

    private void OnAbilityUsage(int index)
    {
        lastPocketmonsterFainted = currentPocketmonsterIsFainted;

        if (currentPocketmonsterIsFainted)
        {
            currentPocketmonsterIsFainted = false;
        }

        if (opponentTrainer.firstTurn || opponentTrainer.checkIfWantsToUseAbility)
        {
            opponentTrainer.UseAbilityForPocketMonster(this, previousPocketMonster);
        }

        if (index == 0)
        {
            currentPocketMonster.UseAbility(currentPocketMonster, opponentPocketMonster, this);
        }

        if (!opponentTrainer.firstTurn)
        {
            if (!opponentPocketMonster.fainted && !lastPocketmonsterFainted)
            {
                opponentTrainer.PerformAction(previousPocketMonster, this);
            }

            if (!lastPocketmonsterFainted)
            {
                currentPocketMonster.GetEveryTurnItemEffects(this, true, teamBuffs, opponentTrainer.currentPocketMonster);
            }

            if (!opponentPocketMonster.fainted && !lastPocketmonsterFainted)
            {
                opponentPocketMonster.GetEveryTurnItemEffects(this, false, opponentTrainer.stats.teamBuffs, currentPocketMonster);
            }

            if (opponentPocketMonster.fainted)
            {
                opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
            }

            SetSwitchButtonDescription();
        } else
        {
            opponentTrainer.firstTurn = false;
        }

        enableAbilityMenu(false);
    }

    public void enableAbilityMenu(bool enabled)
    {
        if (enabled)
        {
            askAbilityText.text = "Use ability " + currentPocketMonster.chosenAbility.abilityName + "?";
        }

        askAbilityText.gameObject.SetActive(enabled);

        for (int i = 0; i < useAbilityButtons.Count; i++)
        {
            useAbilityButtons[i].gameObject.SetActive(enabled);
        }

        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].gameObject.SetActive(!enabled);
        }

        for (int i = 0; i < switchButtons.Count; i++)
        {
            switchButtons[i].gameObject.SetActive(!enabled);
        }

        isInAbilityMenu = enabled;
    }

    private void SetSwitchButtonOnClick(int index)
    {
        switchButtons[index].onClick.AddListener(() => OnSwitchAction(index));
    }

    private void OnSwitchAction(int index)
    {
        if (!pocketMonsters[index].fainted && currentPocketMonster.currentStatus != PocketMonster.StatusEffects.Trapped)
        {
            int indexOfCurrent = pocketMonsters.IndexOf(currentPocketMonster);

            if (indexOfCurrent != index)
            {
                previousPocketMonster = pocketMonsters[indexOfCurrent];
                lastPocketmonsterFainted = currentPocketmonsterIsFainted;

                currentPocketMonster.ResetStats(true, true);
                currentPocketMonster.gameObject.SetActive(false);

                if (!currentPocketmonsterIsFainted)
                {
                    opponentTrainer.MakeDecision(previousPocketMonster, this);
                }

                if (opponentPocketMonster.fainted)
                {
                    opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
                }

                currentPocketMonster = pocketMonsters[index];
                currentPocketMonster.GetOnSwitchItemEffects(teamBuffs, this, opponentPocketMonster);
                CreateOwnPocketMosterUI();

                if (healNextPocketMonster)
                {
                    healNextPocketMonster = false;
                    if (currentPocketMonster.health < currentPocketMonster.stats.maxHealth)
                    {
                        currentPocketMonster.health = currentPocketMonster.stats.maxHealth;
                        inBattleTextManager.QueMessage("You switched to " + currentPocketMonster.stats.name + ". It got healed to full health.", 
                            false, true, false, false);
                    }
                    else
                    {
                        inBattleTextManager.QueMessage("You switched to " + currentPocketMonster.stats.name +".", false, false, false, false);
                    }
                }
                else
                {
                    inBattleTextManager.QueMessage("You switched to " + currentPocketMonster.stats.name + ".", false, false, false, false);
                }

                currentPocketMonster.gameObject.SetActive(true);

                for (int i = 0; i < switchButtons.Count; i++)
                {
                    if (i == index)
                    {
                        switchButtons[i].GetComponentInChildren<Text>().text = "Active";
                    } else if (pocketMonsters[i].fainted)
                    {
                        switchButtons[i].GetComponentInChildren<Text>().text = "Fainted";
                    } else
                    {
                        switchButtons[i].GetComponentInChildren<Text>().text = pocketMonsters[i].stats.name;
                    }
                }

                if (!currentPocketMonster.ability.hasBeenUsed && !currentPocketMonster.ability.onDeath)
                {
                    enableAbilityMenu(true);
                }
                else
                {
                    EnableButtons(true, false);

                    if (!currentPocketmonsterIsFainted)
                    {
                        opponentTrainer.PerformAction(previousPocketMonster, this);
                    }

                    if (!currentPocketmonsterIsFainted)
                    {
                        currentPocketMonster.GetEveryTurnItemEffects(this, true, teamBuffs, opponentTrainer.currentPocketMonster);
                    }

                    if (!lastPocketmonsterFainted)
                    {
                        opponentPocketMonster.GetEveryTurnItemEffects(this, false, opponentTrainer.stats.teamBuffs, currentPocketMonster);
                    }

                    if (!currentPocketMonster.fainted)
                    {
                        currentPocketmonsterIsFainted = false;
                    }
                }

                SetSwitchButtonDescription();
            }
        }
    }

    public void SetSwitchButtonDescription()
    {
        for (int i = 0; i < switchButtons.Count; i++)
        {
            string health = Mathf.RoundToInt(pocketMonsters[i].health).ToString();

            if (pocketMonsters[i].health <= 0)
            {
                health = "Fainted";
            }

            bool showTeamBuffBuffs = true;

            if (pocketMonsters[i] == currentPocketMonster)
            {
                showTeamBuffBuffs = false;
            }

            switchButtons[i].GetComponent<HoverableUiElement>().SetText(SetInformation(pocketMonsters[i], health,
                "Pocketmonster" + (i + 1), true, pocketMonsters[i].currentStatus, true, false, true, showTeamBuffBuffs));
        }
    }

    public void SetOpponentPocketMonster(PocketMonster pocketMonster)
    {
        opponentPocketMonster = pocketMonster;
    }

    public void SetOpponentTrainer(TrainerAi opponent)
    {
        opponentTrainer = opponent;
    }

    public void CreateOwnPocketMosterUI()
    {
        if (currentPocketMonster.fainted)
        {
            return;
        }

        for (int i = 0; i < actionButtons.Count; i++)
        {
            Destroy(actionButtons[i].gameObject);
        }
        actionButtons.Clear();

        for (int i = 0; i < currentPocketMonster.moves.Count; i++)
        {
            Button newButton = Instantiate(buttonToSpawn);
            actionButtons.Add(newButton);
            SetButtonText(i);

            SetUIPosition(newButton.gameObject, currentPocketMonster.moves.Count, 8, 2, 2, -1, 1, i, false, true);

            SetMoveButtonOnClick(i);
        }
    }

    public void SetButtonText(int index)
    {
        string effectiveness = "";
        float effectivenessFactor = opponentPocketMonster.CalculateInTyping(currentPocketMonster.moves[index].moveType);

        if (currentPocketMonster.moves[index].moveSort != PocketMonsterMoves.MoveSort.Status)
        {
            if (effectivenessFactor > 1)
            {
                effectiveness = "Super effective";
            }
            else if (effectivenessFactor < 1)
            {
                effectiveness = "Not very effective";
            }
            else
            {
                effectiveness = "Regular effective";
            }
        }

        string powerPointText = currentPocketMonster.moves[index].currentPowerPoints + "/" + currentPocketMonster.moves[index].powerPoints;

        if (currentPocketMonster.moves[index].currentPowerPoints < 0)
        {
            powerPointText = "0/" + currentPocketMonster.moves[index].powerPoints;
        }

        string typeText = "Type: " + currentPocketMonster.moves[index].moveType;

        actionButtons[index].GetComponent<HoverableUiElement>().SetSetInformationWithImages(true);
        actionButtons[index].GetComponent<HoverableUiElement>().SetText(currentPocketMonster.moves[index].moveDescription);
        actionButtons[index].GetComponentInChildren<Text>().text = typeText + "\n" +
            currentPocketMonster.moves[index].moveName + "\n" + powerPointText + "\n" + effectiveness;

        int textLenght = 0;
        string longestText = "";
        List<string> textCompareList = new List<string>();
        textCompareList.AddRange(new string[] { typeText, currentPocketMonster.moves[index].moveName, powerPointText, effectiveness });

        for (int i = 0; i < textCompareList.Count; i++)
        {
            if (textLenght < textCompareList[i].Length)
            {
                textLenght = textCompareList[i].Length;
                longestText = textCompareList[i];
            }
        }

        SetButtonColor(actionButtons[index], currentPocketMonster.moves[index], longestText, 0.01f);
    }

    public void SetButtonColor(Button button, PocketMonsterMoves move, string text, float time)
    {
        ColorBlock colors = button.colors;
        Sprite sprite = null;

        switch (move.moveType)
        {
            case PocketMonsterStats.Typing.Chemical:
                colors.normalColor = new Color32(248, 0, 255, 255);
                colors.highlightedColor = new Color32(196, 0, 202, 255);
                colors.pressedColor = new Color32(161, 0, 165, 255);
                colors.selectedColor = new Color32(161, 0, 165, 255);
                sprite = chemicalSprite;

                break;
            case PocketMonsterStats.Typing.Earth:
                colors.normalColor = new Color32(160, 82, 45, 255);
                colors.highlightedColor = new Color32(125, 64, 36, 255);
                colors.pressedColor = new Color32(98, 51, 29, 255);
                colors.selectedColor = new Color32(98, 51, 29, 255);
                sprite = earthSprite;
                break;
            case PocketMonsterStats.Typing.Fire:
                colors.normalColor = new Color32(255, 0, 0, 255);
                colors.highlightedColor = new Color32(210, 0, 0, 255);
                colors.pressedColor = new Color32(170, 0, 0, 255);
                colors.selectedColor = new Color32(170, 0, 0, 255);
                sprite = fireSprite;
                break;
            case PocketMonsterStats.Typing.Grass:
                colors.normalColor = new Color32(0, 255, 0, 255);
                colors.highlightedColor = new Color32(0, 210, 0, 255);
                colors.pressedColor = new Color32(0, 170, 0, 255);
                colors.selectedColor = new Color32(0, 170, 0, 255);
                sprite = grassSprite;
                break;
            case PocketMonsterStats.Typing.Light:
                colors.normalColor = new Color32(255, 215, 0, 255);
                colors.highlightedColor = new Color32(218, 184, 0, 255);
                colors.pressedColor = new Color32(184, 155, 0, 255);
                colors.selectedColor = new Color32(184, 155, 0, 255);
                sprite = lightSprite;
                break;
            case PocketMonsterStats.Typing.None:
                break;
            case PocketMonsterStats.Typing.Regular:
                sprite = regularSprite;
                break;
            case PocketMonsterStats.Typing.Spooky:
                colors.normalColor = new Color32(163, 0, 255, 255);
                colors.highlightedColor = new Color32(130, 0, 204, 255);
                colors.pressedColor = new Color32(106, 0, 166, 255);
                colors.selectedColor = new Color32(106, 0, 166, 255);
                sprite = spookySprite;
                break;
            case PocketMonsterStats.Typing.Static:
                colors.normalColor = new Color32(255, 255, 0, 255);
                colors.highlightedColor = new Color32(210, 210, 0, 255);
                colors.pressedColor = new Color32(170, 170, 0, 255);
                colors.selectedColor = new Color32(170, 170, 0, 255);
                sprite = staticSprite;
                break;
            case PocketMonsterStats.Typing.Water:
                colors.normalColor = new Color32(0, 103, 255, 255);
                colors.highlightedColor = new Color32(0, 85, 210, 255);
                colors.pressedColor = new Color32(0, 68, 170, 255);
                colors.selectedColor = new Color32(0, 68, 170, 255);
                sprite = waterSprite;
                break;
            case PocketMonsterStats.Typing.Wind:
                colors.normalColor = new Color32(0, 255, 255, 255);
                colors.highlightedColor = new Color32(0, 210, 210, 255);
                colors.pressedColor = new Color32(0, 170, 170, 255);
                colors.selectedColor = new Color32(0, 170, 170, 255);
                sprite = windSprite;
                break;
        }

        button.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        button.transform.GetChild(0).GetComponent<Image>().color = colors.pressedColor;
        button.colors = colors;
        button.GetComponent<HoverableUiElement>().SetTypeImageAndAccuracy(sprite, move.accuracy.ToString(), colors.pressedColor, move.baseDamage.ToString(),
            move.moveSort);
        StartCoroutine(SetIconPos(button, text, time));
    }

    private IEnumerator SetIconPos(Button button, string text, float time)
    {
        yield return new WaitForSeconds(time);

        if (button != null)
        {
            button.GetComponentInChildren<Text>().font.RequestCharactersInTexture(text, 
                button.GetComponentInChildren<Text>().cachedTextGenerator.fontSizeUsedForBestFit, FontStyle.Normal);

            float width = 0;
            int spaceCounts = 0;

            foreach (char c in text)
            {
                CharacterInfo charInfo;
                if (c == ' ')
                {
                    spaceCounts++;
                }
                else
                {
                    button.GetComponentInChildren<Text>().font.GetCharacterInfo(c, out charInfo,
                        button.GetComponentInChildren<Text>().cachedTextGenerator.fontSizeUsedForBestFit);
                    width += charInfo.glyphWidth;
                }
            }
            float textWidth = width + ((0.278f * button.GetComponentInChildren<Text>().cachedTextGenerator.fontSizeUsedForBestFit) * spaceCounts);

            RectTransform iconTranform = button.transform.GetChild(0).GetComponent<RectTransform>();

            Vector2 initialSize = Vector2.zero;
            initialSize.x = button.GetComponent<RectTransform>().sizeDelta.y;
            initialSize.y = button.GetComponent<RectTransform>().sizeDelta.y;
            iconTranform.sizeDelta = initialSize;

            Vector3 pos = Vector3.zero;
            pos.x = button.GetComponentInChildren<Text>().GetComponent<RectTransform>().localPosition.x -
                textWidth * 0.4f - iconTranform.sizeDelta.x * 0.75f;
            iconTranform.localPosition = pos;

            while (iconTranform.position.x - iconTranform.sizeDelta.x / 2 <= button.GetComponent<RectTransform>().position.x -
                button.GetComponent<RectTransform>().sizeDelta.x / 2)
            {
                Vector2 size = iconTranform.sizeDelta;
                size.x -= 0.01f;
                size.y -= 0.01f;
                iconTranform.sizeDelta = size;

                pos.x = button.GetComponentInChildren<Text>().GetComponent<RectTransform>().localPosition.x -
                    textWidth * 0.4f - iconTranform.sizeDelta.x * 0.75f;
                iconTranform.localPosition = pos;
            }
        }
    }

    private void SetMoveButtonOnClick(int index)
    {
        actionButtons[index].onClick.AddListener(() => SetUsableMove(index));
    }

    public void QuePocketMonsterTextUpdate(float health, bool particles, bool colorText, bool red = true)
    {
        string pocketMonsterHealth = DecideIfFainted(health, true);

        inBattleTextManager.AddPlayerUpdate(currentPocketMonster, currentPocketMonster.currentStatus, pocketMonsterHealth, "You", true, colorText, red,
            particles);
    }

    public void UpdatePlayerText(PocketMonsterTextUpdate textUpdate)
    {
        textUpdate.pocketMonster.chosenAbility.SetAbilityStats(this);
        pocketMonsterAbilityText.text = "Ability: " + textUpdate.pocketMonster.chosenAbility.abilityName;
        pocketMonsterAbilityText.GetComponent<HoverableUiElement>().SetText(textUpdate.pocketMonster.chosenAbility.abilityDescription);

        if (textUpdate.colorText && !textUpdate.showParticles)
        {
            StartCoroutine(ColorText(pocketMonsterInformation, textUpdate.red, true));
        }

        if (textUpdate.showParticles)
        {
            StartCoroutine(textUpdate.pocketMonster.PlayParticlesAttack(opponentPocketMonster, this, textUpdate, pocketMonsterInformation, true));
        }
        else
        {
            pocketMonsterInformation.text = SetInformation(textUpdate.pocketMonster, textUpdate.health, textUpdate.player, textUpdate.battleConditions,
                textUpdate.status, showAll, true, true, false);
        }
    }

    public void QueOpponentPocketMonsterUpdate(float health, bool particles, bool colorText, bool red = true)
    {
        string pocketMonsterHealth = DecideIfFainted(health, false);

        inBattleTextManager.AddAiupdate(opponentPocketMonster, opponentPocketMonster.currentStatus, pocketMonsterHealth, "Opponent", true, colorText, red,
            particles);
    }

    public void UpdateOpponentText(PocketMonsterTextUpdate textUpdate)
    {
        textUpdate.pocketMonster.chosenAbility.SetAbilityStats(this);
        opponentPocketMonsterAbilityText.text = "Ability: " + textUpdate.pocketMonster.chosenAbility.abilityName;
        opponentPocketMonsterAbilityText.GetComponent<HoverableUiElement>().SetText(textUpdate.pocketMonster.chosenAbility.abilityDescription);

        if (textUpdate.colorText && !textUpdate.showParticles)
        {
            StartCoroutine(ColorText(opponentPocketMonsterInformation, textUpdate.red, false));
        }

        if (textUpdate.showParticles)
        {
            StartCoroutine(textUpdate.pocketMonster.PlayParticlesAttack(currentPocketMonster, this, textUpdate, opponentPocketMonsterInformation, false));
        } else
        {
            opponentPocketMonsterInformation.text = SetInformation(textUpdate.pocketMonster, textUpdate.health, textUpdate.player, textUpdate.battleConditions,
                textUpdate.status, showAll, true, false, false);
        }
    }

    public void UpdatePocketMonsterText(Text text, PocketMonsterTextUpdate textUpdate, bool player)
    {
        text.text = SetInformation(textUpdate.pocketMonster, textUpdate.health, textUpdate.player, textUpdate.battleConditions,
                textUpdate.status, showAll, true, false, false);

        if (textUpdate.colorText)
        {
            StartCoroutine(ColorText(text, textUpdate.red, player));
        }
    }

    private string DecideIfFainted(float health, bool player)
    {
        int roundedHealth = Mathf.RoundToInt(health);
        string pocketMonsterHealth = roundedHealth.ToString();

        if (roundedHealth <= 0)
        {
            pocketMonsterHealth = "Fainted";

            if (player)
            {
                currentPocketmonsterIsFainted = true;
            }
        }

        return pocketMonsterHealth;
    }

    public void SetSwitchButtonsForWhenPocketMonsterIsFainted()
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < switchButtons.Count; i++)
        {
            switchButtons[i].gameObject.SetActive(true);
        }

        int index = pocketMonsters.IndexOf(currentPocketMonster);
        switchButtons[index].GetComponentInChildren<Text>().text = "Fainted";
    }

    private string GetItemString(PocketMonster pocketMonster)
    {
        string items = "";

        if (pocketMonster.items.Count > 0)
        {
            for (int i = 0; i < pocketMonster.items.Count; i++)
            {
                if (i < pocketMonster.items.Count - 1)
                {
                    items += " " + pocketMonster.items[i].name + ",";
                }
                else
                {
                    items += " " + pocketMonster.items[i].name;
                }
            }
        } else
        {
            items = " None";
        }

        return items;
    }

    private string GetTeamBuffString(List<PocketMonsterItem> teamBuffThatIsShown)
    {
        string teamBuffsText = "";

        if (teamBuffThatIsShown.Count > 0)
        {
            for (int i = 0; i < teamBuffThatIsShown.Count; i++)
            {
                if (i < teamBuffThatIsShown.Count - 1)
                {
                    teamBuffsText += " " + teamBuffThatIsShown[i].name + ",";
                }
                else
                {
                    teamBuffsText += " " + teamBuffThatIsShown[i].name;
                }
            }
        }
        else
        {
            teamBuffsText = " None";
        }

        return teamBuffsText;
    }

    private string GetTypingString(PocketMonster pocketMonster)
    {
        string typing = "";

        for (int i = 0; i < pocketMonster.stats.typing.Count; i++)
        {
            if (i < pocketMonster.stats.typing.Count - 1)
            {
                typing += " " + pocketMonster.stats.typing[i] + ",";
            }
            else
            {
                typing += " " + pocketMonster.stats.typing[i];
            }
        }

        return typing;
    }

    public string SetInformation(PocketMonster pocketMonster, string health, string person, bool showBattleConditions, PocketMonster.StatusEffects status,
        bool showEverything, bool showTeamBuffs, bool player, bool showTeamBuffAffectedStats)
    {
        string typing = GetTypingString(pocketMonster);

        string text = person + ":\n" + "Name: " + pocketMonster.stats.name + "\nTyping:" + typing + "\nHealth: " + health;

        if (showBattleConditions)
        {
            string items = GetItemString(pocketMonster);
            text += "\nItems:" + items + "\nStatus: " + status;
        }

        if (showEverything)
        {
            int attack = (int)pocketMonster.stats.attack.actualStat;
            int defense = (int)pocketMonster.stats.defense.actualStat;
            int specialAttack = (int)pocketMonster.stats.specialAttack.actualStat;
            int specialDefense = (int)pocketMonster.stats.specialDefense.actualStat;
            int speed = (int)pocketMonster.stats.speed.actualStat;

            if (showTeamBuffAffectedStats)
            {
                List<string> teambuffTypes = new List<string>();
                for (int i = 0; i < teamBuffs.Count; i++)
                {
                    string teambuffType = teamBuffs[i].GetType().ToString();
                    teambuffTypes.Add(teambuffType);
                }

                for (int i = 0; i < teambuffTypes.Count; i++)
                {
                    switch(teambuffTypes[i])
                    {
                        case "MuscleBand":
                            attack += (int)pocketMonster.stats.attack.baseStat / 2;
                            break;
                        case "MindfulBand":
                            specialDefense += (int)pocketMonster.stats.specialDefense.baseStat / 2;
                            break;
                        case "MindBand":
                            specialAttack += (int)pocketMonster.stats.specialAttack.baseStat / 2;
                            break;
                        case "ArmorBand":
                            defense += (int)pocketMonster.stats.defense.baseStat / 2;
                            break;
                        case "Accelerator":
                            speed += (int)pocketMonster.stats.speed.baseStat / 2;
                            break;
                    }
                }
            }

            text += "\nAttack: " + attack + "\nDefense: " + defense + "\nSpecial attack: " + specialAttack + "\nSpecial defense: " + 
            specialDefense + "\nSpeed: " + speed + "\nCritchance: " + pocketMonster.stats.critChance + "%";

            if (showTeamBuffs)
            {
                string teamBuffsText = "";

                if (player)
                {
                    teamBuffsText = GetTeamBuffString(teamBuffs);
                } else
                {
                    teamBuffsText = GetTeamBuffString(opponentTrainer.stats.teamBuffs);
                }

                text += "\nTeamBuffs:" + teamBuffsText;
            }
        }

        return text;
    }

    private void SetUsableMove(int index)
    {
        if (currentPocketMonster.moves[index].currentPowerPoints <= 0)
        {
            return;
        }

        PocketMonster usedByYouForThisTurn = currentPocketMonster;
        PocketMonster usedByOpponentThisTurn = opponentPocketMonster;

        opponentTrainer.MakeDecision(currentPocketMonster, this);

        if (opponentTrainer.wantsToSwitch)
        {
            opponentTrainer.PerformAction(currentPocketMonster, this);
            currentPocketMonster.DealDamage(currentPocketMonster.moves[index], opponentPocketMonster, this, false);

            if (opponentPocketMonster.fainted)
            {
                opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
            }
        }
        else
        {
            if (currentPocketMonster.stats.speed.actualStat > opponentPocketMonster.stats.speed.actualStat)
            {
                PerformActionWhereYouAreFaster(index);
            }
            else if (currentPocketMonster.stats.speed.actualStat < opponentPocketMonster.stats.speed.actualStat)
            {
                PerformActionWhereOpponentIsFaster(index);
            } else
            {
                float speedTie = Random.Range(1, 101);

                if (speedTie <= 50)
                {
                    PerformActionWhereYouAreFaster(index);
                } else
                {
                    PerformActionWhereOpponentIsFaster(index);
                }
            }
        }

        if (!usedByYouForThisTurn.fainted)
        {
            currentPocketMonster.GetEveryTurnItemEffects(this, true, teamBuffs, opponentTrainer.currentPocketMonster);

            if (currentPocketMonster.fainted)
            {
                currentPocketMonster.currentStatus = PocketMonster.StatusEffects.None;
            }
            else
            {
                int outOfPPCounter = 0;

                for (int i = 0; i < currentPocketMonster.moves.Count; i++)
                {
                    if (currentPocketMonster.moves[i].currentPowerPoints <= 0)
                    {
                        outOfPPCounter++;
                    }
                }

                if (outOfPPCounter == currentPocketMonster.moves.Count)
                {
                    CreateStruggle();
                }
                else
                {
                    for (int i = 0; i < actionButtons.Count; i++)
                    {
                        SetButtonText(i);
                    }
                }
            }
        }

        if (!usedByOpponentThisTurn.fainted)
        {
            opponentPocketMonster.GetEveryTurnItemEffects(this, false, opponentTrainer.stats.teamBuffs, currentPocketMonster);
            if (opponentPocketMonster.fainted)
            {
                opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
            }
        }

        SetSwitchButtonDescription();
    }

    private void CreateStruggle()
    {
        currentPocketMonster.moves.Clear();

        Struggle struggle = new Struggle();
        struggle.SetMoveStats();
        currentPocketMonster.moves.Add(struggle);
        currentPocketMonster.ResetPP();
        CreateOwnPocketMosterUI();
    }

    private void PerformActionWhereYouAreFaster(int index)
    {
        currentPocketMonster.DealDamage(currentPocketMonster.moves[index], opponentPocketMonster, this, false);

        if (!opponentPocketMonster.fainted)
        {
            opponentTrainer.PerformAction(currentPocketMonster, this);
            if (opponentPocketMonster.fainted)
            {
                opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
            }
        }
        else
        {
            opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
        }
    }

    private void PerformActionWhereOpponentIsFaster(int index)
    {
        opponentTrainer.PerformAction(currentPocketMonster, this);

        if (opponentPocketMonster.fainted)
        {
            if (currentPocketMonster.moves[index].moveSort == PocketMonsterMoves.MoveSort.Status)
            {
                currentPocketMonster.DealDamage(currentPocketMonster.moves[index], opponentPocketMonster, this, false);
            }
            opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
        } else if (!currentPocketMonster.fainted)
        {
            currentPocketMonster.DealDamage(currentPocketMonster.moves[index], opponentPocketMonster, this, false);

            if (opponentPocketMonster.fainted)
            {
                opponentTrainer.SwitchPocketMonster(this, currentPocketMonster);
            }
        }
    }

    public void CheckIfBattleOver()
    {
        bool finished = opponentTrainer.CheckIfBattleOver();
        if (finished)
        {
            inBattleTextManager.QueFinish(true, true);
        }

        int faintedCounter = 0;
        for (int i = 0; i < pocketMonsters.Count; i++)
        {
            if (pocketMonsters[i].fainted)
            {
                faintedCounter++;
            }
        }

        if (faintedCounter == pocketMonsters.Count)
        {
            inBattleTextManager.QueFinish(true, false);
        }
    }

    public IEnumerator ColorText(Text text, bool red, bool player)
    {
        if (text != null)
        {
            if (red)
            {
                text.color = Color.red;
            }
            else
            {
                text.color = Color.green;
            }
        }

        if (player)
        {
            coloredTextPlayerCounter++;
        } else
        {
            coloredTextAiCounter++;
        }

        yield return new WaitForSeconds(1f);

        if (player)
        {
            coloredTextPlayerCounter--;
        } else
        {
            coloredTextAiCounter--;
        }

        if (text != null)
        {
            if (player)
            {
                if (coloredTextPlayerCounter <= 0)
                {
                    text.color = Color.white;
                }
            } else
            {
                if (coloredTextAiCounter <= 0)
                {
                    text.color = Color.white;
                }
            }
        }
    }

    public IEnumerator SetVictoryText(bool hasWon)
    {
        opponentTrainer.gameObject.GetComponent<OverworldTrainer>().GoToBattleArena(hasWon);
        ClearUI();
        Text victoryText = Instantiate(prefabText);
        endOfBattleText = victoryText;
        SetUIPosition(endOfBattleText.gameObject, 2, 6, 0, 0, 1, 1);
        endOfBattleText.resizeTextMaxSize = 70;
        endOfBattleText.alignment = TextAnchor.MiddleCenter;
        string defeatText = "Defeat";

        if (hasWon)
        {
            endOfBattleText.text = "Victory";
            endOfBattleText.color = Color.green;
        }
        else
        {
            endOfBattleText.text = defeatText;
            endOfBattleText.color = Color.red;
        }

        yield return new WaitForSeconds(1);

        Destroy(endOfBattleText.gameObject);
    }

    private PocketMonsterTextUpdate SetPocketMonsterTextUpdate(PocketMonster pocketMonster, PocketMonster.StatusEffects status, string health, string player, bool battleConditions,
        bool colorText, bool red, PocketMonsterTextUpdate pocketMonsterTextUpdate)
    {
        pocketMonsterTextUpdate.pocketMonster = pocketMonster;
        pocketMonsterTextUpdate.player = player;
        pocketMonsterTextUpdate.health = health;
        pocketMonsterTextUpdate.battleConditions = battleConditions;
        pocketMonsterTextUpdate.status = status;
        pocketMonsterTextUpdate.colorText = colorText;
        pocketMonsterTextUpdate.red = red;

        return pocketMonsterTextUpdate;
    }

    private Text InstantiateAbilityText(GameObject canvas, Text information, bool left, PocketMonster pocketMonster)
    {
        Text abilityText = Instantiate(hoverText);
        abilityText.transform.SetParent(canvas.transform);
        abilityText.rectTransform.sizeDelta = new Vector2(information.rectTransform.sizeDelta.x, information.rectTransform.sizeDelta.y / 6);
        abilityText.rectTransform.localPosition = new Vector3(information.rectTransform.localPosition.x,
          information.rectTransform.localPosition.y - information.rectTransform.sizeDelta.y / 2 - abilityText.rectTransform.sizeDelta.y / 3, 0);
        abilityText.color = Color.white;
        if (left)
        {
            abilityText.alignment = TextAnchor.LowerLeft;
        } else
        {
            abilityText.alignment = TextAnchor.LowerRight;
        }

        pocketMonster.chosenAbility.SetAbilityStats(this);
        abilityText.text = "Ability: " + pocketMonster.chosenAbility.abilityName;
        abilityText.GetComponent<HoverableUiElement>().SetText(pocketMonster.chosenAbility.abilityDescription);

        return abilityText;
    }

    public void SetUIPosition(GameObject uiObject, float xSize, float ySize, float xPos, float yPos, float yPlacement, float xPlacement,
        int index = 0, bool moveVertically = false, bool moveSideWard = false)
    {
        GameObject canvas = GameObject.Find("Canvas");
        uiObject.transform.SetParent(canvas.transform);

        Vector2 size = Vector2.zero;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / ySize;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / xSize;
        uiObject.GetComponent<RectTransform>().sizeDelta = size;

        Vector3 pos = Vector3.zero;

        if (yPos != 0)
        {
            if (moveVertically)
            {
                pos.y = (canvas.GetComponent<RectTransform>().sizeDelta.y / yPos * yPlacement) - (size.y / 2 * yPlacement) - size.y * index;
            }
            else
            {
                pos.y = (canvas.GetComponent<RectTransform>().sizeDelta.y / yPos * yPlacement) - (size.y / 2 * yPlacement);
            }
        } else
        {
            if (moveVertically)
            {
                pos.y -= size.y * index;
            }
        }

        if (xPos != 0)
        {
            if (moveSideWard)
            {
                pos.x = (-canvas.GetComponent<RectTransform>().sizeDelta.x / xPos * xPlacement) + (size.x / 2 * xPlacement) + size.x * index;
            }
            else
            {
                pos.x = (-canvas.GetComponent<RectTransform>().sizeDelta.x / xPos * xPlacement) + (size.x / 2 * xPlacement);
            }
        }

        uiObject.GetComponent<RectTransform>().localPosition = pos;
    }

    private void ClearUI()
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            Destroy(actionButtons[i].gameObject);
        }
        actionButtons.Clear();

        for (int i = 0; i < switchButtons.Count; i++)
        {
            Destroy(switchButtons[i].gameObject);
        }
        switchButtons.Clear();

        for (int i = 0; i < useAbilityButtons.Count; i++)
        {
            Destroy(useAbilityButtons[i].gameObject);
        }
        useAbilityButtons.Clear();

        Destroy(opponentPocketMonsterInformation.gameObject);
        Destroy(pocketMonsterInformation.gameObject);
        Destroy(opponentPocketMonsterAbilityText.gameObject);
        Destroy(pocketMonsterAbilityText.gameObject);
    }

    public void EnableButtons(bool enabled, bool disableAbilitys)
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].gameObject.SetActive(enabled);
        }

        for (int i = 0; i < switchButtons.Count; i++)
        {
            switchButtons[i].gameObject.SetActive(enabled);
        }

        if (disableAbilitys)
        {
            for (int i = 0; i < useAbilityButtons.Count; i++)
            {
                useAbilityButtons[i].gameObject.SetActive(enabled);
            }

            askAbilityText.gameObject.SetActive(enabled);
        }
    }

    public void SetInBattleTextManager(InBattleTextManager inBattleTextManager)
    {
        this.inBattleTextManager = inBattleTextManager;
    }
}
