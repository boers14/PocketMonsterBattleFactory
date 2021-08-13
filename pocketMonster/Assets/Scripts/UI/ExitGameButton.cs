using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject exitGameMenu = null;

    [System.NonSerialized]
    public bool menuIsEnabled = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(EnableMenu);
    }

    public void EnableMenu()
    {
        menuIsEnabled = !menuIsEnabled;
        exitGameMenu.SetActive(menuIsEnabled);
        exitGameMenu.transform.SetAsLastSibling();
    }
}
