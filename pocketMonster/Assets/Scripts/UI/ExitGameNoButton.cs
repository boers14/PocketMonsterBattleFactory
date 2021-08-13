using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameNoButton : MonoBehaviour
{
    [System.NonSerialized]
    public ExitGameButton exitGameButton = null;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(CloseMenu);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.V) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CloseMenu();
        }
    }

    private void CloseMenu()
    {
        exitGameButton.EnableMenu();
    }
}
