using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfoScreenManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas = null;

    [SerializeField]
    private Sprite arrowIcon = null, typeChart = null, statusChart = null, map = null, pocketMonsterTeam = null, fullStatShowCase = null,
        movesButtons = null, switchButtons = null, iconExplanation = null;

    [SerializeField]
    private Button buttonPrefab = null;

    [SerializeField]
    private Text textPrefab = null;

    [SerializeField]
    private Image imagePrefab = null, bg = null;

    [SerializeField]
    private string gameScene = "";

    private int page = 1, lastPage = 5;

    private List<Text> allTexts = new List<Text>();
    private List<Image> allImages = new List<Image>();

    private Button prevPageButton = null, nextPageButton = null;
    private Text pageCount = null;
    private int maxFontSize = 0;

    // Start is called before the first frame update
    void Start()
    {
        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        CreateControlButtons();
        FlipPage(0);
    }

    private void FlipPage(int flipPageNumber)
    {
        page += flipPageNumber;

        prevPageButton.gameObject.SetActive(true);
        nextPageButton.gameObject.SetActive(true);

        if (page == 1)
        {
            prevPageButton.gameObject.SetActive(false);
        } else if (page == lastPage)
        {
            nextPageButton.gameObject.SetActive(false);
        }

        pageCount.text = page + "/" + lastPage;

        DeleteAllPageContents();
        switch (page)
        {
            case 1:
                CreateFirstPage();
                break;
            case 2:
                CreateSecondPage();
                break;
            case 3:
                CreateThirdPage();
                break;
            case 4:
                CreateFourthPage();
                break;
            case 5:
                CreateFifthPage();
                break;
        }
    }

    private void CreateFirstPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Controls out of battle";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIPosition(controlsExplanation.gameObject, 1.85f, 1.45f, 2, 2.5f, 1, -1);
        controlsExplanation.text = "Use WSAD to move around. Press Z to start running.\n\nPress X to open the type chart. Left click on the type chart to " +
            "switch to the statusses chart.\n\nPress C to open up the pocketmonster menu. Hover over moves/ items/ abilitys to see what it does. Click on a" +
            " pocketmonster to drag it around and switch positions in the party.\n\nPress V to open up the guantlet map. Click and move the mouse around " +
            "to move the map. Hover over the texts to see what certain objectives do.\n\nPress S outside of battle to save the game.";
        StartCoroutine(SetMaxFontSize(controlsExplanation));
        allTexts.Add(controlsExplanation);

        Image typeChartImage = Instantiate(imagePrefab);
        SetUIPosition(typeChartImage.gameObject, 4.5f, 4f, 2.02f, 2.5f, 1, 1);
        typeChartImage.sprite = typeChart;
        typeChartImage.color = Color.white;
        allImages.Add(typeChartImage);

        Image statusChartImage = Instantiate(imagePrefab);
        SetUIPosition(statusChartImage.gameObject, 4.5f, 4f, 3.75f, 2.5f, 1, 1);
        statusChartImage.sprite = statusChart;
        statusChartImage.color = Color.white;
        allImages.Add(statusChartImage);

        Image pocketMonsterImage = Instantiate(imagePrefab);
        SetUIPosition(pocketMonsterImage.gameObject, 4.5f, 4f, 2.61f, 7.5f, 1, 1);
        pocketMonsterImage.sprite = pocketMonsterTeam;
        pocketMonsterImage.color = Color.white;
        allImages.Add(pocketMonsterImage);

        Image guantletImage = Instantiate(imagePrefab);
        SetUIPosition(guantletImage.gameObject, 4.5f, 4f, 2.61f, 2.6f, -1, 1);
        guantletImage.sprite = map;
        guantletImage.color = Color.white;
        allImages.Add(guantletImage);
    }

    private void CreateSecondPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "In battle mechanics prt.1";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIPosition(controlsExplanation.gameObject, 1.85f, 1.45f, 2, 2.5f, 1, -1);
        controlsExplanation.text = "Click on the move buttons to use attacks in battle. Hover over them to see what they do.\n\nOn the left side the switch" +
            "buttons can be used to switch around pocketmonsters. Hover over them to see what state the pocketmonster is in.\n\nOn the top sides of the " +
            "screen are both pocketmonster stats and abilitys. Unfold with Z to see all the stats and hover over abilitys to see what they do.\n\nWhile " +
            "battle actions are performed the textboxes will pop-up. Click to make them go away faster.";
        StartCoroutine(SetMaxFontSize(controlsExplanation));
        allTexts.Add(controlsExplanation);

        Image moveButtonImage = Instantiate(imagePrefab);
        SetUIPosition(moveButtonImage.gameObject, 2.4f, 8.5f, 2.1f, 2.5f, 1, 1);
        moveButtonImage.sprite = movesButtons;
        moveButtonImage.color = Color.white;
        allImages.Add(moveButtonImage);

        Image switchButtonImage = Instantiate(imagePrefab);
        SetUIPosition(switchButtonImage.gameObject, 4f, 4f, 2.5f, 3.8f, 1, 1);
        switchButtonImage.sprite = switchButtons;
        switchButtonImage.color = Color.white;
        allImages.Add(switchButtonImage);

        Image fullStatsImage = Instantiate(imagePrefab);
        SetUIPosition(fullStatsImage.gameObject, 8f, 3f, 3f, 2.95f, -1, 1);
        fullStatsImage.sprite = fullStatShowCase;
        fullStatsImage.color = Color.white;
        allImages.Add(fullStatsImage);
    }

    private void CreateThirdPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "In battle mechanics prt.2";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIPosition(controlsExplanation.gameObject, 1.5f, 1.45f, 0, 2.5f, 1, -1);
        controlsExplanation.text = "On switch in if the pocketmonster has an active ability the game will ask if it wants to use the ability.\n\nIf the" +
            " move the pocketmonster is" +
            " using is the same type as the user it get's a 1.5 times power boost (A stab move).\n\nCrits deal 1.5 times the damage and ignore lowered " +
            "attacking stats and higher defenses.\n\nSuper effective moves deal 2 times damage and not very effetcive moves deal 0.5 times damage.\n\n" +
            "If an stat is raised by one stage it's getting 50% added from the base stat (so with 50 attack, 25 attack would be added). If an stat is lowered" +
            " the stat is divided by 1.5 if it's lowered by one stage and divided by 4 if it's lowered by 6 stages.";
        StartCoroutine(SetMaxFontSize(controlsExplanation));
        allTexts.Add(controlsExplanation);
    }

    private void CreateFourthPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Icons";
        allTexts.Add(titel);

        Image iconsExplanationImage = Instantiate(imagePrefab);
        SetUIPosition(iconsExplanationImage.gameObject, 1.6f, 1.4f, 0, 2.5f, 1, -1);
        iconsExplanationImage.sprite = iconExplanation;
        iconsExplanationImage.color = Color.white;
        allImages.Add(iconsExplanationImage);
    }

    private void CreateFifthPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Last words";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIPosition(controlsExplanation.gameObject, 1.5f, 1.45f, 0, 2.5f, 1, -1);
        controlsExplanation.text = "The goal of the game is to get to the final battle without losing a single battle in between. You're team will be " +
            "healed after every battle and after every battle something can be collected to add and upgrade the team.";
        controlsExplanation.resizeTextForBestFit = false;
        controlsExplanation.fontSize = maxFontSize;
        allTexts.Add(controlsExplanation);
    }

    private void CreateControlButtons()
    {
        prevPageButton = Instantiate(buttonPrefab);
        SetUIPosition(prevPageButton.gameObject, 6, 8, 2.05f, 2.1f, -1, 1);
        prevPageButton.onClick.AddListener(() => FlipPage(-1));
        prevPageButton.GetComponentInChildren<Text>().text = "";
        prevPageButton.transform.GetChild(0).GetComponent<Image>().sprite = arrowIcon;
        Vector2 size = prevPageButton.GetComponent<RectTransform>().sizeDelta;
        prevPageButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size.x / 1.2f, size.y / 1.2f);
        prevPageButton.transform.Rotate(0, 0, 180);

        nextPageButton = Instantiate(buttonPrefab);
        SetUIPosition(nextPageButton.gameObject, 6, 8, 2.05f, 2.1f, -1, -1);
        nextPageButton.onClick.AddListener(() => FlipPage(1));
        nextPageButton.GetComponentInChildren<Text>().text = "";
        nextPageButton.transform.GetChild(0).GetComponent<Image>().sprite = arrowIcon;
        nextPageButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size.x / 1.2f, size.y / 1.2f);

        Button startGameButton = Instantiate(buttonPrefab);
        SetUIPosition(startGameButton.gameObject, 4, 8, 0, 2.35f, -1, -1);
        startGameButton.onClick.AddListener(GoToGameScene);
        startGameButton.GetComponentInChildren<Text>().text = "Play!";

        pageCount = Instantiate(textPrefab);
        SetUIPosition(pageCount.gameObject, 12, 14, 0, 2f, -1, -1);
        pageCount.alignment = TextAnchor.MiddleCenter;
    }

    private void GoToGameScene()
    {
        SceneManager.LoadScene(gameScene);
    }

    private void SetUIPosition(GameObject uiObject, float xSize, float ySize, float xPos, float yPos, float yPlacement, float xPlacement,
    int index = 0, bool moveVertically = false, bool moveSideWard = false)
    {
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
        }
        else
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

    private IEnumerator SetMaxFontSize(Text text)
    {
        yield return new WaitForEndOfFrame();
        maxFontSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
    }

    private void DeleteAllPageContents()
    {
        for (int i = 0; i < allImages.Count; i++)
        {
            Destroy(allImages[i].gameObject);
        }
        allImages.Clear();

        for (int i = 0; i < allTexts.Count; i++)
        {
            Destroy(allTexts[i].gameObject);
        }
        allTexts.Clear();
    }
}
