using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckTypeWeaknesses : MonoBehaviour
{
    [SerializeField]
    private PocketMonster basePocketMonster = null;

    [SerializeField]
    private Image baseImage = null;

    [SerializeField]
    private Text uiText = null;

    [SerializeField]
    private float barWidth = 2;

    private List<Image> imagesForTypes = new List<Image>();
    private List<Text> textsForTypes = new List<Text>();

    private List<Image> imagesForStatus = new List<Image>();
    private List<Text> textsForStatus = new List<Text>();

    private List<PocketMonsterStats.Typing> allMoveTypes = new List<PocketMonsterStats.Typing>();

    private bool isUIOn = false;

    private bool showTypes = true;

    void Start()
    {
        FillPocketMonsterTypesList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isUIOn = true;
            CreateUI();
            SwitchBetweenUI();
        } else if (Input.GetMouseButtonDown(0))
        {
            if (isUIOn)
            {
                showTypes = !showTypes;
            }
            SwitchBetweenUI();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            DisableUI();
        }
    }

    private void SwitchBetweenUI()
    {
        if (!isUIOn)
        {
            return;
        }

        if (showTypes)
        {
            EnableUIElements(imagesForStatus, false);
            EnableUIElements(textsForStatus, false);
            EnableUIElements(imagesForTypes, true);
            EnableUIElements(textsForTypes, true);
        } else
        {
            EnableUIElements(imagesForTypes, false);
            EnableUIElements(textsForTypes, false);
            EnableUIElements(imagesForStatus, true);
            EnableUIElements(textsForStatus, true);
        }
    }

    private void DisableUI()
    {
        isUIOn = false;

        ClearList(imagesForTypes);
        ClearList(textsForTypes);
        ClearList(imagesForStatus);
        ClearList(textsForStatus);
    }

    private void ClearList(List<Image> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
        list.Clear();
    }

    private void ClearList(List<Text> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
        list.Clear();
    }

    private void EnableUIElements(List<Image> images, bool enabled)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].enabled = enabled;
        }
    }

    private void EnableUIElements(List<Text> texts, bool enabled)
    {
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].enabled = enabled;
        }
    }

    private void CreateUI()
    {
        GameObject canvas = GameObject.Find("Canvas");

        Image bg = Instantiate(baseImage);
        bg.transform.SetParent(canvas.transform);
        bg.GetComponent<RectTransform>().localPosition = Vector3.zero;
        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        imagesForTypes.Add(bg);
        imagesForStatus.Add(bg);

        for (int i = 0; i < allMoveTypes.Count * 2 + 2; i++)
        {
            Image line = Instantiate(baseImage);
            line.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;

            if ((i % 2) == 0)
            {
                if (i < 2)
                {
                    size.x = canvas.GetComponent<RectTransform>().sizeDelta.x;
                } else
                {
                    size.x = canvas.GetComponent<RectTransform>().sizeDelta.x - canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2);
                }
            }
            else
            {
                if (i < 2)
                {
                    size.x = canvas.GetComponent<RectTransform>().sizeDelta.y;
                }
                else
                {
                    size.x = canvas.GetComponent<RectTransform>().sizeDelta.y - canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2);
                }
            }

            size.y = barWidth;
            line.rectTransform.sizeDelta = size;
            line.rectTransform.rotation = Quaternion.Euler(0, 0, (i % 2) * 90);

            Vector3 pos = Vector3.zero;
            if ((i % 2) == 0)
            {
                pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2) * (1 + (i / 2));
                if (i >= 2)
                {
                    pos.x = (canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2)) / 2;
                }
            }
            else
            {
                pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 2 + canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2) * (1 + ((i - 1) / 2));
                if (i >= 2)
                {
                    pos.y = (-canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2)) / 2;
                }
            }

            line.rectTransform.localPosition = pos;
            line.color = Color.white;
            imagesForTypes.Add(line);
        }

        for (int i = 0; i < 2; i++)
        {
            Text textBox = Instantiate(uiText);
            textBox.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;

            if ((i % 2) == 0)
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.x - canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2);
                size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2);
            }
            else
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.y - canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2);
                size.y = canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2);
            }

            textBox.rectTransform.sizeDelta = size;
            textBox.rectTransform.rotation = Quaternion.Euler(0, 0, (i % 2) * 90);

            Vector3 pos = Vector3.zero;
            if ((i % 2) == 0)
            {
                pos.x = (canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2)) / 2;
                pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2;
                textBox.text = "Offensive capabilities";
            }
            else
            {
                pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 2 + size.y / 2;
                pos.y = (-canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2)) / 2;
                textBox.text = "Defensive capabilities";
            }

            textBox.rectTransform.localPosition = pos;
            textBox.color = Color.white;
            textBox.alignment = TextAnchor.MiddleCenter;
            textsForTypes.Add(textBox);
        }

        Vector2 textBoxSize = Vector2.zero;
        textBoxSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / (allMoveTypes.Count + 2);
        textBoxSize.y = canvas.GetComponent<RectTransform>().sizeDelta.y / (allMoveTypes.Count + 2);

        float currentY = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - textBoxSize.y * 1.5f;
        float baseX = -canvas.GetComponent<RectTransform>().sizeDelta.x / 2 + textBoxSize.x * 1.5f;
        float currentX = baseX;
        int typeCounter = 0;
        int moveCounter = 0;

        for (int i = 0; i < (allMoveTypes.Count + 1) * (allMoveTypes.Count + 1); i++)
        {
            Text textBox = Instantiate(uiText);
            textBox.transform.SetParent(canvas.transform);
            textBox.rectTransform.sizeDelta = textBoxSize;
            string text = "Type chart";

            if (i < 11)
            {
                if (i != 0)
                {
                    SetTypeTextColor(textBox, allMoveTypes[typeCounter]);
                    text = allMoveTypes[typeCounter].ToString();
                    typeCounter++;
                    currentX += textBoxSize.x;
                } else
                {
                    textBox.color = Color.white;
                }
            }
            else
            {
                if (i % 11 == 0)
                {
                    if (typeCounter == 10)
                    {
                        typeCounter = 0;
                    }
                    else
                    {
                        typeCounter++;
                    }

                    currentX = baseX;
                    currentY -= textBoxSize.y;
                    SetTypeTextColor(textBox, allMoveTypes[typeCounter]);
                    text = allMoveTypes[typeCounter].ToString();
                    moveCounter = 0;
                }
                else
                {
                    currentX += textBoxSize.x;
                    float damageMultiplier = basePocketMonster.CalculateInTypingWithGivenType(allMoveTypes[moveCounter], allMoveTypes[typeCounter]);
                    text = damageMultiplier + "x";
                    SetMultiplierTextColor(textBox, damageMultiplier);
                    moveCounter++;
                }
            }

            Vector3 pos = Vector3.zero;
            pos.x = currentX;
            pos.y = currentY;
            textBox.rectTransform.localPosition = pos;

            textBox.text = text;
            textBox.alignment = TextAnchor.MiddleCenter;
            textsForTypes.Add(textBox);
        }

        int amountOfStatusses = System.Enum.GetNames(typeof(PocketMonster.StatusEffects)).Length;

        for (int i = 0; i < amountOfStatusses; i++)
        {
            Image line = Instantiate(baseImage);
            line.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;
            if (i == 0)
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.y;
            } else
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.x;
            }
            size.y = barWidth;
            line.rectTransform.sizeDelta = size;

            if (i == 0)
            {
                line.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
            }

            Vector3 pos = Vector3.zero;

            if (i == 0)
            {
                pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 4;
            } else
            {
                pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - i * (canvas.GetComponent<RectTransform>().sizeDelta.y / amountOfStatusses);
            }

            line.rectTransform.localPosition = pos;
            line.color = Color.white;
            imagesForStatus.Add(line);
        }

        PocketMonster.StatusEffects status = 0;

        for (int i = 0; i < amountOfStatusses * 2; i++)
        {
            Text info = Instantiate(uiText);
            info.transform.SetParent(canvas.transform);
            info.color = Color.white;

            Vector2 size = Vector2.zero;
            if ((i % 2) == 0)
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 4;
            }
            else
            {
                size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 1.5f;
            }
            size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / amountOfStatusses;
            info.rectTransform.sizeDelta = size;

            Vector3 pos = Vector3.zero;

            if ((i % 2) == 0)
            {
                pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 4 - size.x / 2;
                pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2 - i / 2 * size.y;
            }
            else
            {
                pos.x = -canvas.GetComponent<RectTransform>().sizeDelta.x / 4 + size.x / 2;
                pos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2 - (i - 1) / 2 * size.y;
            }

            string text = "";

            if ((i % 2) == 0)
            {
                if (i < 2)
                {
                    text = "Status";
                } else
                {
                    text = status.ToString();
                    switch (status)
                    {
                        case PocketMonster.StatusEffects.Burned:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Fire);
                            break;
                        case PocketMonster.StatusEffects.Bloated:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Water);
                            break;
                        case PocketMonster.StatusEffects.Nearsighted:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Light);
                            break;
                        case PocketMonster.StatusEffects.Trapped:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Earth);
                            break;
                        case PocketMonster.StatusEffects.Airborne:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Wind);
                            break;
                        case PocketMonster.StatusEffects.Poisened:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Chemical);
                            break;
                        case PocketMonster.StatusEffects.Leeched:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Grass);
                            break;
                        case PocketMonster.StatusEffects.Cursed:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Spooky);
                            break;
                        case PocketMonster.StatusEffects.Paralyzed:
                            SetTypeTextColor(info, PocketMonsterStats.Typing.Static);
                            break;
                        case PocketMonster.StatusEffects.None:
                            break;
                        default:
                            break;
                    }
                }
            } else
            {
                if (i < 2)
                {
                    text = "Description";
                } else
                {
                    switch (status)
                    {
                        case PocketMonster.StatusEffects.Burned:
                            text = "Halves damage dealt by physical attacks and deal 5% of max health as damage at the end of each turn.";
                            break;
                        case PocketMonster.StatusEffects.Bloated:
                            text = "Halves damage dealt by special attacks and deal 5% of max health as damage at the end of each turn.";
                            break;
                        case PocketMonster.StatusEffects.Nearsighted:
                            text = "Decreases the accuracy of all moves by 15%.";
                            break;
                        case PocketMonster.StatusEffects.Trapped:
                            text = "The pocketmonster can't switch out and deal 5% of max health as damage at the end of each turn.";
                            break;
                        case PocketMonster.StatusEffects.Airborne:
                            text = "Take double damage from attacks of opponent pocketmonster.";
                            break;
                        case PocketMonster.StatusEffects.Poisened:
                            text = "Deal 15% of max health as damage at the end of each turn.";
                            break;
                        case PocketMonster.StatusEffects.Leeched:
                            text = "Deal 8% of max health as damage at the end of each turn and heal as much as damage dealt back to the opposing pocketmonster.";
                            break;
                        case PocketMonster.StatusEffects.Cursed:
                            text = "Moves cost 3 power points and deal 5% of max health as damage at the end of each turn.";
                            break;
                        case PocketMonster.StatusEffects.Paralyzed:
                            text = "At end of turn quarters the speed stat.";
                            break;
                        case PocketMonster.StatusEffects.None:
                            break;
                        default:
                            break;
                    }
                    status++;
                }
            }

            info.text = text;

            info.rectTransform.localPosition = pos;
            textsForStatus.Add(info);
        }
    }

    private void SetMultiplierTextColor(Text text, float multiplier)
    {
        if (multiplier > 1)
        {
            text.color = Color.green; 
        } else if (multiplier < 1)
        {
            text.color = Color.red;
        } else
        {
            text.color = Color.white;
        }
    }

    public void SetTypeTextColor(Text text, PocketMonsterStats.Typing moveType)
    {
        switch (moveType)
        {
            case PocketMonsterStats.Typing.Chemical:
                text.color = new Color32(248, 0, 255, 255);
                break;
            case PocketMonsterStats.Typing.Earth:
                text.color = new Color32(160, 82, 45, 255);
                break;
            case PocketMonsterStats.Typing.Fire:
                text.color = new Color32(255, 0, 0, 255);
                break;
            case PocketMonsterStats.Typing.Grass:
                text.color = new Color32(0, 255, 0, 255);
                break;
            case PocketMonsterStats.Typing.Light:
                text.color = new Color32(255, 215, 0, 255);
                break;
            case PocketMonsterStats.Typing.Regular:
                text.color = new Color32(255, 255, 255, 255);
                break;
            case PocketMonsterStats.Typing.Spooky:
                text.color = new Color32(163, 0, 255, 255);
                break;
            case PocketMonsterStats.Typing.Static:
                text.color = new Color32(255, 255, 0, 255);
                break;
            case PocketMonsterStats.Typing.Water:
                text.color = new Color32(0, 103, 255, 255);
                break;
            case PocketMonsterStats.Typing.Wind:
                text.color = new Color32(0, 255, 255, 255);
                break;
        }
    }

    private void FillPocketMonsterTypesList()
    {
        int amountOfTypes = System.Enum.GetNames(typeof(PocketMonsterStats.Typing)).Length;
        PocketMonsterStats.Typing type = 0;

        for (int i = 0; i < amountOfTypes - 1; i++)
        {
            allMoveTypes.Add(type);
            type++;
        }
    }
}
