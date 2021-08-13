using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
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
    private string startScreen = "";

    [SerializeField]
    private MenuParticles menuParticles = null;

    [SerializeField]
    private Particle wonParticle = null, lostParticle = null;

    void Start()
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.playerBattle.GetComponent<PlayerPocketMonsterMenu>().CreateBg();
        gameManager.playerBattle.GetComponent<PlayerPocketMonsterMenu>().CreateAllPocketMonstersUI();
        gameManager.playerBattle.GetComponent<PlayerPocketMonsterMenu>().AdjustMenuScale(1.4f);

        bg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;

        Text titel = Instantiate(uiText);
        titel.fontStyle = FontStyle.Bold;
        SetUIStats.SetUIPosition(canvas, titel.gameObject, 2, 7, 0, 2, 1, -1);
        titel.resizeTextMaxSize = 150;
        titel.alignment = TextAnchor.MiddleCenter;

        if (gameManager.playerWon)
        {
            titel.text = "Victory!!!";
            titel.color = Color.green;
            menuParticles.particleToSpawn = wonParticle;
        } else
        {
            titel.text = "Defeat...";
            titel.color = Color.red;
            menuParticles.particleToSpawn = lostParticle;
        }

        Button startGameButton = Instantiate(uiButton);
        SetUIStats.SetUIPosition(canvas, startGameButton.gameObject, 4, 8, 0, 2.05f, -1, -1);
        startGameButton.onClick.AddListener(SwitchScene);
        startGameButton.GetComponentInChildren<Text>().text = "Return to start screen";

        Destroy(gameManager.gameObject);
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene(startScreen);
    }
}
