using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RunSettingsManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas = null;

    [SerializeField]
    private Button buttonPrefab = null;

    [SerializeField]
    private Text textPrefab = null, hoverTextPrefab = null;

    [SerializeField]
    private Image bg = null;

    [SerializeField]
    private string gameScene = "", chooseGameStateScene = "";

    [SerializeField]
    private Dropdown dropdownPrefab = null;

    public int lenghtOfRun = 6;

    private int minimalLenghtOfRun = 4;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        CreateUI();
    }

    private void CreateUI()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Game settings";

        Text setLengthOfRunText = Instantiate(hoverTextPrefab);
        SetUIStats.SetUIPosition(canvas, setLengthOfRunText.gameObject, 3, 12, 3.25f, 10, 1, 1);
        setLengthOfRunText.text = "Set length of run: ";
        setLengthOfRunText.GetComponent<HoverableUiElement>().SetText("Decide throught how many big gauntlets you want to go before the final battle.");

        Dropdown setLenghtRunDropDown = Instantiate(dropdownPrefab);
        SetUIStats.SetUIPosition(canvas, setLenghtRunDropDown.gameObject, 18, 10, 12, 9.5f, 1, -1);
        setLenghtRunDropDown.ClearOptions();
        List<string> lenghtOptions = new List<string>();
        for (int i = minimalLenghtOfRun; i <= 12; i++)
        {
            int number = i - 2;
            lenghtOptions.Add(number.ToString());
        }
        setLenghtRunDropDown.AddOptions(lenghtOptions);
        setLenghtRunDropDown.onValueChanged.AddListener(SetLengthOfRun);
        setLenghtRunDropDown.value = 4;

        Button startGameButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, startGameButton.gameObject, 4, 8, 0, 2.65f, -1, -1);
        startGameButton.onClick.AddListener(() => SwitchScene(gameScene));
        startGameButton.GetComponentInChildren<Text>().text = "Play!";

        Button returnToChooseGameStateButton = Instantiate(buttonPrefab);
        SetUIStats.SetUIPosition(canvas, returnToChooseGameStateButton.gameObject, 6, 10, 0, 2.05f, -1, -1);
        returnToChooseGameStateButton.onClick.AddListener(() => SwitchScene(chooseGameStateScene));
        returnToChooseGameStateButton.GetComponentInChildren<Text>().text = "Return to savestate selection";
    }

    private void SetLengthOfRun(int setting)
    {
        lenghtOfRun = minimalLenghtOfRun + setting;
    }

    private void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
