
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Menu : MonoBehaviour
{
    public GameObject darker;
    public GameObject singlePlayerView;
    public GameObject worldButton;
    public GameObject background;
    public GameObject GUI;

    public GameObject eventSystem;

    public GameObject mainScreen;

    public GameObject crosshair;

    public GameObject DebugScreen;

    public int HotBarFocus = 0;
    
    public GameObject HotBar;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1000;
        //QualitySettings.SetQualityLevel(0, true);


        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(eventSystem);

        darker.SetActive(false);
        background.SetActive(false);
        mainScreen.SetActive(true);
        GUI.SetActive(false);
        crosshair.SetActive(false);
        DebugScreen.SetActive(false);


        GameObject instantiatedWorldButton = Instantiate(worldButton, new Vector3(0, 0, 0), new Quaternion(), singlePlayerView.transform);
        instantiatedWorldButton.GetComponent<Button>().onClick.AddListener(loadSinglePlayerWorld);
        instantiatedWorldButton.transform.localPosition = new Vector3(0, 0, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
            HotBarChange(false);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
            HotBarChange(true);
        }
    }

    public void showSinglePlayerWorlds() {
        darker.SetActive(true);
    }

    public void showMainScreen() {
        darker.SetActive(false);
    }

    public void loadSinglePlayerWorld() {
        mainScreen.SetActive(false);
        background.SetActive(true);
        SceneManager.LoadScene("World");
    }

    public void hideBackground() {
        background.SetActive(false);
        GUI.SetActive(true);
        crosshair.SetActive(true);
        DebugScreen.SetActive(true);
    }

    public void HotBarChange(bool right) {
        if(right) {
            if(HotBarFocus == 8) {
                HotBarFocus = 0;
            } else {
                HotBarFocus++;
            }
        } else {
            if(HotBarFocus == 0) {
                HotBarFocus = 8;
            } else {
                HotBarFocus--;
            }
        }
        int index = 0;
        foreach(Transform transform in HotBar.transform) {
            if(index == HotBarFocus) {
                transform.gameObject.GetComponent<Outline>().enabled = true;
            } else {
                transform.gameObject.GetComponent<Outline>().enabled = false;
            }
            index++;
        }
    }
}
