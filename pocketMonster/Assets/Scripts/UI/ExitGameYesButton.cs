using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitGameYesButton : MonoBehaviour
{
    [SerializeField]
    private string startScene = "";

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SwitchToStartScene);
    }

    private void SwitchToStartScene()
    {
        SceneManager.LoadScene(startScene);
    }
}
