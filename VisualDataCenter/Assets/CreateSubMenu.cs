using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CreateSubMenu : MonoBehaviour
{

    public GameObject main;

    public bool is_router;
    public int id_no;

    private Vector3 menu_sleep = new Vector3(0, 0, 0);
    private Vector3 menu_awake = new Vector3(0.0005f, 0.0005f, 0.0005f);
    private Vector3 menu_choice_awake = new Vector3(1, 1, 1);

    float routerMessageTimer = 100.0f;
    float messageDuration = 2.0f;

    GameObject player;

    private GameObject[] menu_options = new GameObject[3];
    GameObject menu;
    GameObject ok;
    GameObject type;
    GameObject id;

    Text title;
    Text ok_text;
    Text type_text;
    Text id_text;

    Color panel_default = Color.white;
    Color panel_highlight = Color.cyan;

    // Use this for initialization
    void Start()
    {
        // player
        player = GameObject.Find("/OVRPlayerController");

        // Main Context Menu
        menu = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu");

        // Main Menu Choices
        type = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/Type");
        id = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/ID");
        ok = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/Ok");  


        // Main Menu Text Objects
        title = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/Title").GetComponent<Text>();
        type_text = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/Type/Text").GetComponent<Text>();
        id_text = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/ID/Text").GetComponent<Text>();
        ok_text = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create SubMenu/Ok/Text").GetComponent<Text>();

        title.text = "Device Created";
        type_text.text = "Device Type: ";
        id_text.text = "ID: ";
        ok_text.text = "Ok";

        // Main Menu Panel Objects
        type.GetComponent<Image>().color = panel_default;
        id.GetComponent<Image>().color = panel_default;
        ok.GetComponent<Image>().color = panel_highlight;

        // put context menu to sleep
        //menu.transform.localScale = menu_sleep;
        //type.transform.localScale = menu_sleep;
        //id.transform.localScale = menu_sleep;
        //ok.transform.localScale = menu_sleep;

        // set booleans
        //activate_create = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(id_no.ToString());
        Debug.Log(is_router);
        if (is_router)
            type_text.text = "Device Type: Router"; 
        else
            type_text.text = "Device Type: Switch";

        id_text.text = "ID: " + id_no.ToString();

        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two)) {

            type_text.text = "Device Type: ";
            id_text.text = "ID: ";

            main.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void wakeMenu()
    {
        menu.transform.localScale = menu_awake;

        for (int i = 0; i < menu_options.Length; i++)
        {
            menu_options[i].transform.localScale = menu_choice_awake;
        }

        player.GetComponent<OVRPlayerController>().EnableRotation = false;
    }

    public void sleepMenu()
    {
        menu.transform.localScale = menu_sleep;

        for (int i = 0; i < menu_options.Length; i++)
        {
            menu_options[i].transform.localScale = menu_sleep;
        }

        // reset text
        type_text.text = "Device Type: ";
        id_text.text = "ID: ";

        //activate_create = false;

        player.GetComponent<OVRPlayerController>().EnableRotation = true;
    }
}