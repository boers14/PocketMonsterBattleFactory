using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPocketMonsterMenu : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField]
    private Text textObject = null, hoverText = null;

    private List<Text> texts = new List<Text>();
    private List<Text> hoverableTexts = new List<Text>();

    private List<Text> pocketMontersInformations = new List<Text>(); 

    [SerializeField]
    private Image bg = null;

    private List<Image> bgs = new List<Image>();

    [SerializeField]
    private float teamBuffTextPos = 7.5f;

    private bool isUiOn = false;

    private Text targetedObject = null;

    private Vector3 oldTargetedObjectPos;

    private int indexGrabbedPocketMonster = 0;

    private List<Text> grabbedObject = new List<Text>();

    private void Start()
    {
        CreateBg();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateAllPocketMonstersUI();
        } else if(Input.GetMouseButton(0))
        {
            GrabPocketMonsterInformation();
        } else if(Input.GetMouseButtonUp(0))
        {
            ReorderTeamStructure();
        } else if (Input.GetKeyUp(KeyCode.C))
        {
            RemovePocketMonsterUI();
        }
    }

    private void GrabPocketMonsterInformation()
    {
        if (!isUiOn)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;

        for (int i = 0; i < hoverableTexts.Count; i++)
        {
            hoverableTexts[i].GetComponent<HoverableUiElement>().PointerExit();
        }

        if (targetedObject == null)
        {
            for (int i = 0; i < pocketMontersInformations.Count; i++)
            {
                if (mousePos.x >= pocketMontersInformations[i].transform.position.x - pocketMontersInformations[i].rectTransform.sizeDelta.x / 2 &&
                    mousePos.x <= pocketMontersInformations[i].transform.position.x + pocketMontersInformations[i].rectTransform.sizeDelta.x / 2 &&
                    mousePos.y >= pocketMontersInformations[i].transform.position.y - pocketMontersInformations[i].rectTransform.sizeDelta.y / 2 &&
                    mousePos.y <= pocketMontersInformations[i].transform.position.y + pocketMontersInformations[i].rectTransform.sizeDelta.y / 2)
                {
                    grabbedObject.Add(pocketMontersInformations[i]);
                    targetedObject = pocketMontersInformations[i];
                    oldTargetedObjectPos = pocketMontersInformations[i].transform.position;
                    indexGrabbedPocketMonster = i;
                }

                if (targetedObject != null)
                {
                    continue;
                }
            }
        }

        if (targetedObject != null)
        {
            targetedObject.transform.position = mousePos;
        }
    }

    private void ReorderTeamStructure()
    {
        if (!isUiOn)
        {
            return;
        }

        if (targetedObject != null)
        {
            bool objectIsPlaced = false;
            for (int i = 0; i < pocketMontersInformations.Count; i++)
            {
                if (targetedObject == null)
                {
                    continue;
                }

                if (targetedObject.transform.position.x >= pocketMontersInformations[i].transform.position.x - pocketMontersInformations[i].rectTransform.sizeDelta.x / 2 &&
                    targetedObject.transform.position.x <= pocketMontersInformations[i].transform.position.x + pocketMontersInformations[i].rectTransform.sizeDelta.x / 2 &&
                    targetedObject.transform.position.y >= pocketMontersInformations[i].transform.position.y - pocketMontersInformations[i].rectTransform.sizeDelta.y / 2 &&
                    targetedObject.transform.position.y <= pocketMontersInformations[i].transform.position.y + pocketMontersInformations[i].rectTransform.sizeDelta.y / 2)
                {
                    if (!grabbedObject.Contains(pocketMontersInformations[i]))
                    {
                        targetedObject.transform.position = pocketMontersInformations[i].transform.position;
                        pocketMontersInformations[i].transform.position = oldTargetedObjectPos;

                        PocketMonster pocketMonster = gameManager.playerPocketMonsters[indexGrabbedPocketMonster];
                        gameManager.playerPocketMonsters[indexGrabbedPocketMonster] = gameManager.playerPocketMonsters[i];
                        gameManager.playerPocketMonsters[i] = pocketMonster;

                        Text text = pocketMontersInformations[indexGrabbedPocketMonster];
                        pocketMontersInformations[indexGrabbedPocketMonster] = pocketMontersInformations[i];
                        pocketMontersInformations[i] = text;

                        targetedObject = null;
                        objectIsPlaced = true;
                        grabbedObject.Clear();
                    }
                }
            }

            if (!objectIsPlaced)
            {
                targetedObject.transform.position = oldTargetedObjectPos;
                targetedObject = null;
                grabbedObject.Clear();
            }
        }
    }

    private void CreateAllPocketMonstersUI()
    {
        if (isUiOn)
        {
            return;
        }

        for (int i = 0; i < bgs.Count; i++)
        {
            bgs[i].gameObject.SetActive(true);
            bgs[i].transform.SetAsLastSibling();
        }

        isUiOn = true;
        GameObject canvas = GameObject.Find("Canvas");

        for (int i = 0; i < gameManager.playerPocketMonsters.Count; i++)
        {
            float halfScreenSize = 0;
            float abilityTextYSize = 0;

            Text statsInfo = Instantiate(textObject);
            statsInfo.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;
            size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - canvas.GetComponent<RectTransform>().sizeDelta.y / (teamBuffTextPos * 2);
            size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 6;

            halfScreenSize = size.y;
            abilityTextYSize = halfScreenSize / 11;
            size.y -= abilityTextYSize;
            statsInfo.rectTransform.sizeDelta = size;

            float startPos = -canvas.GetComponent<RectTransform>().sizeDelta.x / 2;

            if (i > 1)
            {
                startPos = 0;
            }

            Vector3 pos = Vector3.zero;
            pos.x = startPos + size.x / 2;
            pos.y = size.y / 2 + (size.y / 2 * -(i % 2 * 2)) - canvas.GetComponent<RectTransform>().sizeDelta.y / (teamBuffTextPos * 2) + (abilityTextYSize * (1 - (i % 2)));
            statsInfo.rectTransform.localPosition = pos;

            statsInfo.text = GetComponent<PlayerBattle>().SetInformation(gameManager.playerPocketMonsters[i],
                gameManager.playerPocketMonsters[i].stats.maxHealth.ToString(), "Pocketmonster " + (i + 1) + " stats", false, 
                PocketMonster.StatusEffects.None, true, false, true, false);
            statsInfo.color = Color.white;
            texts.Add(statsInfo);

            int infoMovesCount = gameManager.playerPocketMonsters[i].moves.Count + gameManager.playerPocketMonsters[i].items.Count + 2;

            if (gameManager.playerPocketMonsters[i].items.Count == 0)
            {
                infoMovesCount++;
            }

            Vector2 infoMovesSize = Vector2.zero;
            infoMovesSize.y = halfScreenSize / infoMovesCount;
            infoMovesSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 3;

            Vector3 infoMovesPos = Vector3.zero;
            infoMovesPos.x = statsInfo.rectTransform.localPosition.x + statsInfo.GetComponent<RectTransform>().sizeDelta.x / 2 +
                infoMovesSize.x / 2;

            Text allPocketMonsterInfoObject = Instantiate(textObject);
            allPocketMonsterInfoObject.transform.SetParent(canvas.transform);
            Vector2 allPocketMonsterInfoSize = Vector2.zero;
            allPocketMonsterInfoSize.y = halfScreenSize;
            allPocketMonsterInfoSize.x = size.x + infoMovesSize.x;
            allPocketMonsterInfoObject.rectTransform.sizeDelta = allPocketMonsterInfoSize;

            Vector3 allPocketMonsterInfoPos = Vector3.zero;
            allPocketMonsterInfoPos.x = ((pos.x - size.x / 2) + (infoMovesPos.x + infoMovesSize.x / 2)) / 2;
            allPocketMonsterInfoPos.y = halfScreenSize / 2 + (halfScreenSize / 2 * -(i % 2 * 2)) - canvas.GetComponent<RectTransform>().sizeDelta.y / (teamBuffTextPos * 2);
            allPocketMonsterInfoObject.rectTransform.localPosition = allPocketMonsterInfoPos;

            statsInfo.transform.SetParent(allPocketMonsterInfoObject.transform);

            for (int j = 0; j < infoMovesCount; j++)
            {
                Text movesItemInfo = null;

                if (j == 0 || j == gameManager.playerPocketMonsters[i].moves.Count + 1)
                {
                    movesItemInfo = Instantiate(textObject);
                    if (j == 0)
                    {
                        movesItemInfo.text = "Moves:";
                    } else
                    {
                        movesItemInfo.text = "Items:";
                    }
                    texts.Add(movesItemInfo);
                    movesItemInfo.color = Color.white;
                } else
                {
                    if (j > gameManager.playerPocketMonsters[i].moves.Count + 1 && gameManager.playerPocketMonsters[i].items.Count == 0)
                    {
                        movesItemInfo = Instantiate(textObject);
                        movesItemInfo.text = "None";
                        texts.Add(movesItemInfo);
                        movesItemInfo.color = Color.white;
                    } else
                    {
                        movesItemInfo = Instantiate(hoverText);
                        if (j < gameManager.playerPocketMonsters[i].moves.Count + 1) {
                            movesItemInfo.text = j + "." + gameManager.playerPocketMonsters[i].moves[j - 1].moveName;
                            movesItemInfo.GetComponent<HoverableUiElement>().SetText(gameManager.playerPocketMonsters[i].moves[j - 1].moveDescription);
                            GetComponent<CheckGauntletMap>().SetTextDescription(movesItemInfo, gameManager.playerPocketMonsters[i].moves[j - 1]);
                            GetComponent<CheckTypeWeaknesses>().SetTypeTextColor(movesItemInfo, gameManager.playerPocketMonsters[i].moves[j - 1].moveType);
                        } else
                        {
                            movesItemInfo.text = j - 1 - gameManager.playerPocketMonsters[i].moves.Count + "." + gameManager.playerPocketMonsters[i].items[j - gameManager.playerPocketMonsters[i].moves.Count - 2].name;
                            movesItemInfo.GetComponent<HoverableUiElement>().SetText(gameManager.playerPocketMonsters[i].items[j - gameManager.playerPocketMonsters[i].moves.Count - 2].itemDescription);
                            movesItemInfo.color = Color.white;
                        }
                        hoverableTexts.Add(movesItemInfo);
                    }
                }

                movesItemInfo.transform.SetParent(canvas.transform);

                movesItemInfo.rectTransform.sizeDelta = infoMovesSize;

                infoMovesPos.y = halfScreenSize / 2 + (halfScreenSize / 2 * -(i % 2 * 2)) - canvas.GetComponent<RectTransform>().sizeDelta.y / (teamBuffTextPos * 2);
                infoMovesPos.y += halfScreenSize / 2 - infoMovesSize.y / 2 - (infoMovesSize.y * j);
                movesItemInfo.rectTransform.localPosition = infoMovesPos;

                movesItemInfo.alignment = TextAnchor.MiddleLeft;
                movesItemInfo.transform.SetParent(allPocketMonsterInfoObject.transform);
            }

            if (gameManager.playerPocketMonsters[i].chosenAbility != null)
            {
                Text abilityInfo = Instantiate(hoverText);
                abilityInfo.transform.SetParent(canvas.transform);

                Vector2 abilityInfoSize = Vector2.zero;
                abilityInfoSize.y = abilityTextYSize;
                abilityInfoSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 6;
                abilityInfo.rectTransform.sizeDelta = abilityInfoSize;

                Vector3 abilityinfoPos = Vector3.zero;
                abilityinfoPos.x = pos.x;
                abilityinfoPos.y = pos.y - size.y / 2;
                abilityInfo.rectTransform.localPosition = abilityinfoPos;

                abilityInfo.text = "Ability: " + gameManager.playerPocketMonsters[i].chosenAbility.abilityName;
                abilityInfo.GetComponent<HoverableUiElement>().SetText(gameManager.playerPocketMonsters[i].chosenAbility.abilityDescription);
                abilityInfo.alignment = TextAnchor.LowerLeft;
                abilityInfo.color = Color.white;
                hoverableTexts.Add(abilityInfo);
                abilityInfo.transform.SetParent(allPocketMonsterInfoObject.transform);
            }

            pocketMontersInformations.Add(allPocketMonsterInfoObject);
        }

        Text teamBuffText = Instantiate(textObject);
        teamBuffText.transform.SetParent(canvas.transform);

        Vector2 sizeTeamBuffText = Vector2.zero;
        sizeTeamBuffText.y = canvas.GetComponent<RectTransform>().sizeDelta.y / teamBuffTextPos;
        sizeTeamBuffText.x = canvas.GetComponent<RectTransform>().sizeDelta.x;
        teamBuffText.rectTransform.sizeDelta = sizeTeamBuffText;

        Vector3 posTeamBuffText = Vector3.zero;
        posTeamBuffText.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - sizeTeamBuffText.y / 2;
        teamBuffText.rectTransform.localPosition = posTeamBuffText;

        teamBuffText.text = "Teambuffs: ";

        for (int i = 0; i < gameManager.teamBuffsOfPlayer.Count; i++)
        {
            teamBuffText.text += (i + 1) + ". " + gameManager.teamBuffsOfPlayer[i].name + ": " + gameManager.teamBuffsOfPlayer[i].itemDescription + "\n";
        }

        if (teamBuffText.text == "Teambuffs: ")
        {
            teamBuffText.text += "None";
        }

        teamBuffText.color = Color.white;
        teamBuffText.resizeTextForBestFit = true;
        texts.Add(teamBuffText);
    }

    private void CreateBg()
    {
        GameObject canvas = GameObject.Find("Canvas");
        Image newBg = Instantiate(bg);
        newBg.transform.SetParent(canvas.transform);
        newBg.rectTransform.localPosition = Vector3.zero;
        newBg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        bgs.Add(newBg);

        for (int i = 0; i < 3; i++)
        {
            Image line = Instantiate(bg);
            line.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;

            if ((i % 2) == 0)
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.x;
            }
            else
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.y;
            }

            size.y = 2;

            line.rectTransform.sizeDelta = size;
            line.rectTransform.rotation = Quaternion.Euler(0, 0, (i % 2) * 90);

            Vector3 pos = Vector3.zero;

            if (i == 0)
            {
                pos.y -= canvas.GetComponent<RectTransform>().sizeDelta.y / (teamBuffTextPos * 2);
            }
            else if (i == 1)
            {
                pos.y -= canvas.GetComponent<RectTransform>().sizeDelta.y / teamBuffTextPos;
            }
            else
            {
                pos.y += canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - canvas.GetComponent<RectTransform>().sizeDelta.y / teamBuffTextPos;
            }

            pos.y += size.y / 2;

            line.rectTransform.localPosition = pos;

            line.color = Color.white;
            bgs.Add(line);
        }

        RemovePocketMonsterUI();
    }

    private void RemovePocketMonsterUI()
    {
        isUiOn = false;

        for(int i = 0; i < texts.Count; i++)
        {
            Destroy(texts[i].gameObject);
        }

        texts.Clear();

        for (int i = 0; i < hoverableTexts.Count; i++)
        {
            Destroy(hoverableTexts[i].gameObject);
        }

        hoverableTexts.Clear();

        for (int i = 0; i < bgs.Count; i++)
        {
            bgs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < pocketMontersInformations.Count; i++)
        {
            Destroy(pocketMontersInformations[i].gameObject);
        }

        pocketMontersInformations.Clear();
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
