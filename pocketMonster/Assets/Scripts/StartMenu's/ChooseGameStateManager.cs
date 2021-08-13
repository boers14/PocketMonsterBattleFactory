using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseGameStateManager : MonoBehaviour
{
    [SerializeField]
    private Button uiButton = null;

    [SerializeField]
    private Text uiText = null;

    [SerializeField]
    private Canvas canvas = null;

    [SerializeField]
    private Image bg = null;

    [SerializeField]
    private string startScene = "", gameScene = "", runSettingsScene = "";

    [SerializeField]
    private GameObject loadObject = null;

    private GameObject load = null;

    void Start()
    {
        load = Instantiate(loadObject);

        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;

        Text titel = Instantiate(uiText);
        titel.transform.SetParent(canvas.transform);

        Vector2 titelSize = Vector2.zero;
        titelSize.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 3;
        titelSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 3;
        titel.rectTransform.sizeDelta = titelSize;

        Vector3 titelPos = Vector3.zero;
        titelPos.y = canvas.GetComponent<RectTransform>().sizeDelta.y - titelSize.y * 1.9f;
        titel.rectTransform.localPosition = titelPos;

        titel.text = "Choose game state";
        titel.alignment = TextAnchor.MiddleCenter;
        titel.resizeTextMaxSize = 70;
        titel.fontStyle = FontStyle.Bold;

        Button newGameButton = Instantiate(uiButton);
        newGameButton.transform.SetParent(canvas.transform);

        Vector2 buttonSize = Vector2.zero;
        buttonSize.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 6;
        buttonSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 4;
        newGameButton.GetComponent<RectTransform>().sizeDelta = buttonSize;

        Vector3 newGamePos = Vector3.zero;
        newGamePos.y = buttonSize.y / 1.8f;
        newGameButton.GetComponent<RectTransform>().localPosition = newGamePos;

        newGameButton.GetComponentInChildren<Text>().text = "New Game";
        newGameButton.onClick.AddListener(() => switchScene(runSettingsScene, false));

        Button loadGameButton = Instantiate(uiButton);
        loadGameButton.transform.SetParent(canvas.transform);
        loadGameButton.interactable = SaveSytem.CheckIfFileExist();
        loadGameButton.GetComponent<RectTransform>().sizeDelta = buttonSize;

        Vector2 infoButtonPos = Vector3.zero;
        infoButtonPos.y = -buttonSize.y / 1.8f;
        loadGameButton.GetComponent<RectTransform>().localPosition = infoButtonPos;

        loadGameButton.GetComponentInChildren<Text>().text = "Load Game";
        loadGameButton.onClick.AddListener(() => switchScene(gameScene, true));

        Button backButton = Instantiate(uiButton);
        backButton.transform.SetParent(canvas.transform);
        backButton.GetComponent<RectTransform>().sizeDelta = buttonSize / 1.5f;

        Vector2 backButtonPos = Vector3.zero;
        backButtonPos.y = infoButtonPos.y - buttonSize.y;
        backButton.GetComponent<RectTransform>().localPosition = backButtonPos;

        backButton.GetComponentInChildren<Text>().text = "Back to start menu";
        backButton.onClick.AddListener(() => switchScene(startScene, false));
    }

    private void switchScene(string scene, bool createLoadObject)
    {
        if (createLoadObject)
        {
            DontDestroyOnLoad(load);
        }

        SceneManager.LoadScene(scene);
    }
}
