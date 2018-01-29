using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class buttonExit : MonoBehaviour {
    // Use this for initialization
    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ExitGame);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
