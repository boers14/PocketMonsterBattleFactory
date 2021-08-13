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
    private string gameScene = "", startScene = "";

    private int page = 1, lastPage = 5;

    private List<Text> allTexts = new List<Text>();
    private List<Image> allImages = new List<Image>();

    private Button prevPageButton = null, nextPageButton = null;
    private Text pageCount = null;

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
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Controls out of battle";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, controlsExplanation.gameObject, 1.85f, 1.45f, 2, 2.5f, 1, -1);
        controlsExplanation.text = "Use WAD to move around. Press Z to start running.\n\nPress X to open the type chart. Left click on the type chart to " +
            "switch to the statusses chart.\n\nPress C to open up the pocketmonster menu. Hover over moves/ items/ abilitys to see what it does. Click on a" +
            " pocketmonster to drag it around and switch positions in the party.\n\nPress V to open up the guantlet map. Click and move the mouse around " +
            "to move the map. Hover over the texts to see what certain objectives do.\n\nPress S outside of battle to save the game.";
        allTexts.Add(controlsExplanation);

        Image typeChartImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, typeChartImage.gameObject, 4.5f, 4f, 2.02f, 2.5f, 1, 1);
        typeChartImage.sprite = typeChart;
        typeChartImage.color = Color.white;
        allImages.Add(typeChartImage);

        Image statusChartImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, statusChartImage.gameObject, 4.5f, 4f, 3.75f, 2.5f, 1, 1);
        statusChartImage.sprite = statusChart;
        statusChartImage.color = Color.white;
        allImages.Add(statusChartImage);

        Image pocketMonsterImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, pocketMonsterImage.gameObject, 4.5f, 4f, 2.61f, 7.5f, 1, 1);
        pocketMonsterImage.sprite = pocketMonsterTeam;
        pocketMonsterImage.color = Color.white;
        allImages.Add(pocketMonsterImage);

        Image guantletImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, guantletImage.gameObject, 4.5f, 4f, 2.61f, 2.6f, -1, 1);
        guantletImage.sprite = map;
        guantletImage.color = Color.white;
        allImages.Add(guantletImage);
    }

    private void CreateSecondPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "In battle mechanics prt.1";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, controlsExplanation.gameObject, 1.85f, 1.45f, 2, 2.5f, 1, -1);
        controlsExplanation.text = "Click on the move buttons to use attacks in battle. Hover over them to see what they do.\n\nOn the left side the switch" +
            "buttons can be used to switch around pocketmonsters. Hover over them to see what state the pocketmonster is in.\n\nOn the top sides of the " +
            "screen are both pocketmonster stats and abilitys. Unfold with Z to see all the stats and hover over abilitys to see what they do.\n\nWhile " +
            "battle actions are performed the textboxes will pop-up. Click to make them go away faster.";
        allTexts.Add(controlsExplanation);

        Image moveButtonImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, moveButtonImage.gameObject, 2.4f, 8.5f, 2.1f, 2.5f, 1, 1);
        moveButtonImage.sprite = movesButtons;
        moveButtonImage.color = Color.white;
        allImages.Add(moveButtonImage);

        Image switchButtonImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, switchButtonImage.gameObject, 4f, 4f, 2.5f, 3.8f, 1, 1);
        switchButtonImage.sprite = switchButtons;
        switchButtonImage.color = Color.white;
        allImages.Add(switchButtonImage);

        Image fullStatsImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, fullStatsImage.gameObject, 8f, 3f, 3f, 2.95f, -1, 1);
        fullStatsImage.sprite = fullStatShowCase;
        fullStatsImage.color = Color.white;
        allImages.Add(fullStatsImage);
    }

    private void CreateThirdPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "In battle mechanics prt.2";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, controlsExplanation.gameObject, 1.5f, 1.45f, 0, 2.5f, 1, -1);
        controlsExplanation.text = "On switch in if the pocketmonster has an active ability the game will ask if it wants to use the ability.\n\nIf the" +
            " move the pocketmonster is" +
            " using is the same type as the user it get's a 1.5 times power boost (A stab move).\n\nCrits deal 1.5 times the damage and ignore lowered " +
            "attacking stats and higher defenses.\n\nSuper effective moves deal 2 times damage and not very effetcive moves deal 0.5 times damage.\n\n" +
            "If an stat is raised by one stage it's getting 50% added from the base stat (so with 50 attack, 25 attack would be added). If an stat is lowered" +
            " the stat is divided by 1.5 if it's lowered by one stage and divided by 4 if it's lowered by 6 stages.";
        allTexts.Add(controlsExplanation);
    }

    private void CreateFourthPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Icons";
        allTexts.Add(titel);

        Image iconsExplanationImage = Instantiate(imagePrefab);
        SetUIStats.SetUIPosition(canvas, iconsExplanationImage.gameObject, 1.6f, 1.4f, 0, 2.5f, 1, -1);
        iconsExplanationImage.sprite = iconExplanation;
        iconsExplanationImage.color = Color.white;
        allImages.Add(iconsExplanationImage);
    }

    private void CreateFifthPage()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Pickups & goal";
        allTexts.Add(titel);

        Text controlsExplanation = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, controlsExplanation.gameObject, 1.5f, 1.45f, 0, 2.5f, 1, -1);
        controlsExplanation.text = "The maximum amount of pocketmonsters is 4. The maximum amount of moves per pocketmonster is 4. The maximum amount of items " +
            "per pocketmonster is 3. The maximum amount of teambuffs is 3. If something exceeds the maximum the game will ask te remove something to add something." +
            " Removing a pocketmonster and adding a new one will give the same amount of moves as the old pocketmonster, with the same items. When picking a " +
            "pocketmonster hover over the ability to see what it does. When adding moves/ items hover over the question to see what they do." +
            " \n\nThe goal of the game is to get to the final battle without losing a single battle in between. You're team will be " +
            "healed after every battle and after every battle something can be collected to add and upgrade the team.";
        allTexts.Add(controlsExplanation);
    }

    private void CreateControlButtons()
    {
        prevPageButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, prevPageButton.gameObject, 6, 8, 2.05f, 2.1f, -1, 1);
        prevPageButton.onClick.AddListener(() => FlipPage(-1));
        prevPageButton.GetComponentInChildren<Text>().text = "";
        prevPageButton.transform.GetChild(0).GetComponent<Image>().sprite = arrowIcon;
        Vector2 size = prevPageButton.GetComponent<RectTransform>().sizeDelta;
        prevPageButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size.x / 1.2f, size.y / 1.2f);
        prevPageButton.transform.Rotate(0, 0, 180);

        nextPageButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, nextPageButton.gameObject, 6, 8, 2.05f, 2.1f, -1, -1);
        nextPageButton.onClick.AddListener(() => FlipPage(1));
        nextPageButton.GetComponentInChildren<Text>().text = "";
        nextPageButton.transform.GetChild(0).GetComponent<Image>().sprite = arrowIcon;
        nextPageButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(size.x / 1.2f, size.y / 1.2f);

        Button startGameButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, startGameButton.gameObject, 4, 8, 0, 2.35f, -1, -1);
        startGameButton.onClick.AddListener(() => SwitchScene(gameScene));
        startGameButton.GetComponentInChildren<Text>().text = "Play!";

        pageCount = Instantiate(textPrefab);
        SetUIStats.SetUIPosition(canvas, pageCount.gameObject, 12, 14, 0, 2f, -1, -1);
        pageCount.alignment = TextAnchor.MiddleCenter;

        Button returnToStartMenuButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, returnToStartMenuButton.gameObject, 6, 14, 2, 2, 1, -1);
        returnToStartMenuButton.onClick.AddListener(() => SwitchScene(startScene));
        returnToStartMenuButton.GetComponentInChildren<Text>().text = "Return to startscreen";
    }

    private void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
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
