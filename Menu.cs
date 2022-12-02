
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
    public GameObject Inventory;

    public bool isPlayerInTheGame = false;

    GameObject player;

    FirstPersonMovement movementComponent;
    Jump jumpComponent;
    Crouch crouchComponent;

    GameObject firstPersonCamera;
    FirstPersonLook firstPersonLookComponent;
    public GameObject materials;



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1000;
        //QualitySettings.SetQualityLevel(0, true);


        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(materials);

        darker.SetActive(false);
        background.SetActive(false);
        mainScreen.SetActive(true);
        GUI.SetActive(false);
        crosshair.SetActive(false);
        DebugScreen.SetActive(false);
        Inventory.SetActive(false);


        GameObject instantiatedWorldButton = Instantiate(worldButton, new Vector3(0, 0, 0), new Quaternion(), singlePlayerView.transform);
        instantiatedWorldButton.GetComponent<Button>().onClick.AddListener(loadSinglePlayerWorld);
        instantiatedWorldButton.transform.localPosition = new Vector3(0, 0, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInTheGame) {
            if(player != null && movementComponent != null && jumpComponent != null && crouchComponent != null) {
            movementComponent.enabled = true;
            jumpComponent.enabled = true;
            crouchComponent.enabled = true;
            } else {
                assignComponents();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
                HotBarChange(false);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
                HotBarChange(true);
            }

            if(Input.GetKeyDown(KeyCode.E)) {
                InventoryStateChange(true);
            }
        } else {
            if(player != null && movementComponent != null && jumpComponent != null && crouchComponent != null) {
                movementComponent.enabled = false;
                jumpComponent.enabled = false;
                crouchComponent.enabled = false;
            } else {
                assignComponents();
            }

        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) {
            InventoryStateChange(false);
            }
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
        isPlayerInTheGame = true;
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


    public void assignComponents() {
        try {
            player = GameObject.Find("Player"); 
            movementComponent = player.GetComponent<FirstPersonMovement>();
            jumpComponent = player.GetComponent<Jump>();
            crouchComponent = player.GetComponent<Crouch>();
        } catch {
            player = null;
            movementComponent = null;
            jumpComponent = null;
            crouchComponent = null;
        }
    }

    public void InventoryStateChange(bool open) {
        if(open) {
            if(!Inventory.activeSelf) {
                Inventory.SetActive(true);
                isPlayerInTheGame = false;
                Cursor.lockState = CursorLockMode.None;
            }
        } else {
            if(Inventory.activeSelf) {
                Inventory.SetActive(false);
                isPlayerInTheGame = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
