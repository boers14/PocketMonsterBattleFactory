using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
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
    private string chooseGameStateScene = "", instructionScene = "";

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager != null)
        {
            Destroy(gameManager);
        }

        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;

        Text titel = Instantiate(uiText);
        titel.transform.SetParent(canvas.transform);

        Vector2 titelSize = Vector2.zero;
        titelSize.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 3;
        titelSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 3;
        titel.rectTransform.sizeDelta = titelSize;

        Vector3 titelPos = Vector3.zero;
        titelPos.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 2 - titelSize.y / 2;
        titel.rectTransform.localPosition = titelPos;

        titel.text = "Pocketmonster Battle Factory";
        titel.alignment = TextAnchor.MiddleCenter;
        titel.resizeTextMaxSize = 70;
        titel.fontStyle = FontStyle.Bold;

        Button startButton = Instantiate(uiButton);
        startButton.transform.SetParent(canvas.transform);

        Vector2 buttonSize = Vector2.zero;
        buttonSize.y = canvas.GetComponent<RectTransform>().sizeDelta.y / 4;
        buttonSize.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 4;
        startButton.GetComponent<RectTransform>().sizeDelta = buttonSize;

        Vector2 startButtonPos = Vector3.zero;
        startButtonPos.y = buttonSize.y / 4;
        startButton.GetComponent<RectTransform>().localPosition = startButtonPos;

        startButton.GetComponentInChildren<Text>().text = "Play!";
        startButton.onClick.AddListener(() => switchScene(chooseGameStateScene));

        Button infoButton = Instantiate(uiButton);
        infoButton.transform.SetParent(canvas.transform);

        Vector2 infoButtonSize = Vector2.zero;
        infoButtonSize.y = buttonSize.y / 1.5f;
        infoButtonSize.x = buttonSize.x / 1.5f;
        infoButton.GetComponent<RectTransform>().sizeDelta = infoButtonSize;

        Vector2 infoButtonPos = Vector3.zero;
        infoButtonPos.y = startButtonPos.y - buttonSize.y / 2 - infoButtonSize.y / 2 - buttonSize.y / 8;
        infoButton.GetComponent<RectTransform>().localPosition = infoButtonPos;

        infoButton.GetComponentInChildren<Text>().text = "Instructions";
        infoButton.onClick.AddListener(() => switchScene(instructionScene));

        Button quitButton = Instantiate(uiButton);
        quitButton.transform.SetParent(canvas.transform);

        quitButton.GetComponent<RectTransform>().sizeDelta = infoButtonSize;

        Vector2 quitButtonPos = Vector3.zero;
        quitButtonPos.y = infoButtonPos.y - (infoButtonSize.y / 2 * 2) - buttonSize.y / 8;
        quitButton.GetComponent<RectTransform>().localPosition = quitButtonPos;

        quitButton.GetComponentInChildren<Text>().text = "Exit game";
        quitButton.onClick.AddListener(ExitGame);
    }

    private void switchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
