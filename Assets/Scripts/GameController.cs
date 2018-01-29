using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    private const string messageSelect = "Please Select a Cube.";

    private string[] updateMessage = { "Watts Output: ", "Watts Required: ", "Watts Multiplier: " };

    public GameObject uiCanvas;

    public Text[] texts;

    [SerializeField]
    public float[] test;

    [SerializeField]
    public string text;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private GameObject youWin;

    private LaserController selectedLaser;

    [SerializeField]
    LaserController[] lasersList;

    // Use this for initialization
    void Start () {
        test = new float[9];
        for(int i = 0; i < 9; i++)
        {
            test[i] = 5.0f;
        }        
		player = GetComponentInChildren<PlayerController>();
        texts = uiCanvas.GetComponentsInChildren<Text>();
        lasersList = GetComponentsInChildren<LaserController>();
        youWin.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        selectedLaser = player.currentLaserSelected;
        for (int i = 0; i < 3; i++)
        {
            texts[i].text = messageSelect;
            if (selectedLaser != null)
            {
                string s = "";
                switch(i)
                {
                    case 0:
                        s = selectedLaser.GetWatts().ToString();
                        break;
                    case 1:
                        s = selectedLaser.GetRequiredWatts().ToString();
                        break;
                    case 2:
                        s = string.Concat(selectedLaser.GetMultiplier().ToString(),"x");
                        break;
                }
                texts[i].text = string.Concat(updateMessage[i], s);
            }
        }       
    }

    public float GetTest(int i)
    {
        return test[i];
    }

    public void SetTest(int loc, int val)
    {
        test[loc] = val;
    }

    public void RefreshLasers()
    {

        foreach(LaserController lc in lasersList)
        {
            lc.NullLasers();
        }
    }

    public void SetYouWin(bool b)
    {
        youWin.SetActive(b);
    }
}
