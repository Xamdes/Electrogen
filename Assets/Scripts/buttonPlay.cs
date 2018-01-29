using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonPlay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayGame);
    }
	
    void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
