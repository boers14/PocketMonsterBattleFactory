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
    private string gameScene = "";

    [SerializeField]
    private Dropdown dropdownPrefab = null;

    public int lenghtOfRun = 6;

    private int minimalLenghtOfRun = 4;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        CreateUI();
    }

    private void CreateUI()
    {
        Text titel = Instantiate(textPrefab);
        titel.fontStyle = FontStyle.Bold;
        SetUIPosition(titel.gameObject, 4, 10, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;
        titel.text = "Game settings";

        Text setLengthOfRunText = Instantiate(hoverTextPrefab);
        SetUIPosition(setLengthOfRunText.gameObject, 3, 12, 3.25f, 10, 1, 1);
        setLengthOfRunText.text = "Set length of run: ";
        setLengthOfRunText.GetComponent<HoverableUiElement>().SetText("Decide throught how many big gauntlets you want to go before the final battle.");

        Dropdown setLenghtRunDropDown = Instantiate(dropdownPrefab);
        SetUIPosition(setLenghtRunDropDown.gameObject, 18, 10, 12, 9.5f, 1, -1);
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
        SetUIPosition(startGameButton.gameObject, 4, 8, 0, 2.35f, -1, -1);
        startGameButton.onClick.AddListener(GoToGameScene);
        startGameButton.GetComponentInChildren<Text>().text = "Play!";
    }

    private void SetLengthOfRun(int setting)
    {
        lenghtOfRun = minimalLenghtOfRun + setting;
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

    private void GoToGameScene()
    {
        SceneManager.LoadScene(gameScene);
    }
}
