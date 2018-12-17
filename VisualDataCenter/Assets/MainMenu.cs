using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject subMenu;
    public GameObject linkMenu;

    // MAIN MENU ONLY
    private const int max_devices   = 24;
    private const int max_routers   = 12;
    private const int max_switches  = 12;

    public GameObject[] routers     = new GameObject[max_routers];
    public GameObject[] switches    = new GameObject[max_switches];

    private Vector3[] spawn_locations   = new Vector3[max_devices];
    private bool[] active_locations     = new bool[max_devices];

    private Vector3 menu_sleep          = new Vector3(0, 0, 0);
    private Vector3 menu_awake          = new Vector3(0.0005f, 0.0005f, 0.0005f);
    private Vector3 menu_choice_awake   = new Vector3(1, 1, 1);

    private bool main_menu_active;
    private bool horiz_active;
    private bool vert_active;
    private bool left;
    private bool right;
    private bool up;
    private bool down;

    private int main_menu_option;
    private int last_option;

    float routerMessageTimer    = 100.0f;
    float messageDuration       = 2.0f;

    GameObject player;

    private GameObject[] main_menu_options = new GameObject[4];
    GameObject menu;
    GameObject create_submenu;
    GameObject create_router;
    GameObject create_switch;
    GameObject select_rs;
    GameObject link;

    Text title;
    Text router_text;
    Text switch_text;
    Text select_text;
    Text link_text;

    Color panel_default     = Color.white;
    Color panel_highlight   = Color.cyan;

    float right_stick_horiz;    // [-1, 1]
    float right_stick_vert;     // [-1, 1]

    // Use this for initialization
    void Start()
    {
        // MAIN MENU ONLY
        //spawn_locations[] = new Vector3();

        // player
        player = GameObject.Find("/OVRPlayerController");

        // Create SubMenu
        create_submenu = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor");

        // link menu
        linkMenu = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Create LinkMenu");

        // Main Context Menu
        menu = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu");

        // Main Menu Choices
        create_router =     GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Create Router");
        create_switch =     GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Create Switch");
        select_rs =         GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Select Router Switch");
        link =              GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Link");

        // set up Menu Options Array
        main_menu_options[0] = create_router;
        main_menu_options[1] = create_switch;
        main_menu_options[2] = select_rs;
        main_menu_options[3] = link;

        // Main Menu Text Objects
        title =         GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Title").GetComponent<Text>();
        router_text =   GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Create Router/Text").GetComponent<Text>();
        switch_text =   GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Create Switch/Text").GetComponent<Text>();
        select_text =   GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Select Router Switch/Text").GetComponent<Text>();
        link_text =     GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Main Menu/Link/Text").GetComponent<Text>();

        title.text = "Main Menu";
        router_text.text = "Create Router";
        switch_text.text = "Create Switch";
        select_text.text = "Select Router / Switch";
        link_text.text = "Link";

        // Main Menu Panel Objects
        create_router.GetComponent<Image>().color   = panel_default;
        create_switch.GetComponent<Image>().color   = panel_default;
        select_rs.GetComponent<Image>().color       = panel_default;
        link.GetComponent<Image>().color            = panel_default;

        // put context menu to sleep
        menu.transform.localScale           = menu_sleep;
        create_router.transform.localScale  = menu_sleep;
        create_switch.transform.localScale  = menu_sleep;
        select_rs.transform.localScale      = menu_sleep;
        link.transform.localScale           = menu_sleep;

        // set booleans
        main_menu_active    = false;
        left                = false;
        right               = false;
        up                  = false;
        down                = false;
        horiz_active        = false;
        vert_active         = false;

        // starting menu option
        main_menu_option = 0;

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two)) // 'B' button

        {
            if (!main_menu_active)  wakeMenu();
            else                    sleepMenu();
        }

        operateMenu();
    }






    void wakeMenu()
    {
        menu.transform.localScale = menu_awake;

        for (int i = 0; i < main_menu_options.Length; i++)
        {
            main_menu_options[i].transform.localScale = menu_choice_awake;
        }

        main_menu_active = true;
        main_menu_option = 0;
        main_menu_options[0].GetComponent<Image>().color = panel_highlight;

        player.GetComponent<OVRPlayerController>().EnableRotation = false;
    }

    void sleepMenu()
    {
        menu.transform.localScale = menu_sleep;

        for (int i = 0; i < main_menu_options.Length; i++)
        {
            main_menu_options[i].transform.localScale = menu_sleep;
            // ONLY FOR MAIN MENU
            main_menu_options[i].GetComponent<Image>().color = panel_default;
        }

        main_menu_active = false;

        player.GetComponent<OVRPlayerController>().EnableRotation = true;
    }

    void operateMenu()
    {
        debounceInput(right_stick_horiz, right_stick_vert);

        if (main_menu_active)
        {
            // Menu Functionality (awake)
            right_stick_horiz   = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            right_stick_vert    = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;


            updateCursor();
            selectOption(main_menu_option);
        }
        menuMessageTimer();
    }

    void menuMessageTimer()
    {
        if(routerMessageTimer < messageDuration) routerMessageTimer += Time.deltaTime;
        else router_text.text = "Create Router";
    }

    void debounceInput(float horiz, float vert)
    {
        // meant to move left/right
        if (Mathf.Abs(vert) < 0.5f)
        {
            // set inputs for one iteration
            if (left) left = false;
            if (right) right = false;

            if (horiz <= -0.8f && !horiz_active)
            {
                left = true;
                horiz_active = true;
            }
            else if (horiz >= 0.8f && !horiz_active)
            {
                right = true;
                horiz_active = true;
            }
            else if (horiz > -0.5f && horiz < 0.5f)
            {
                horiz_active = false;
            }
        }

        // meant to move up/down
        if (Mathf.Abs(horiz) < 0.5f)
        {
            // set inputs for one iteration
            if (up) up = false;
            if (down) down = false;

            if (vert <= -0.8f && !vert_active)
            {
                down = true;
                vert_active = true;
            }
            else if (vert >= 0.8f && !vert_active)
            {
                up = true;
                vert_active = true;
            }
            else if (vert > -0.5f && vert < 0.5f)
            {
                vert_active = false;
            }
        }
    }

    void updateCursor()
    {
        last_option = main_menu_option;

        switch (main_menu_option)
        {
            case 0:     // Create Router
                if (down) main_menu_option = 1;
                break;
            case 1:     // Create Switch
                if (up) main_menu_option = 0;
                if (down) main_menu_option = 2;
                break;
            case 2:     // Select Router / Switch
                if (up) main_menu_option = 1;
                if (down) main_menu_option = 3;
                break;
            case 3:     // Link
                if (up) main_menu_option = 2;
                break;
        }

        // highlight new option
        if (last_option != main_menu_option)
        {
            main_menu_options[last_option].GetComponent<Image>().color = panel_default;
            main_menu_options[main_menu_option].GetComponent<Image>().color = panel_highlight;
        }

    }

    void selectOption(int option)
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            switch (option)
            {
                case 0: createRouter(); break;
                case 1: createSwitch(); break;
                case 2: selectRS();     break;
                case 3: linkDevice();   break;
            }
        }
    }

    void createRouter()
    {
        bool found_location = true;

        /*for(int i = 0; i < active_locations.Length; i++)
        {
            //
            if(!active_locations[i])
            {
                found_location = true;
                routers[i].transform.position = spawn_locations[i];
                active_locations[i] = true;
                break;
            }
        }*/

        if(!found_location)
        {
            router_text.text = "No Routers";
            routerMessageTimer = 0;
        }
        else
        {
            create_submenu.GetComponent<CreateSubMenu>().is_router = true;
            create_submenu.GetComponent<CreateSubMenu>().id_no = 498;
            gameObject.SetActive(false);
            subMenu.SetActive(true);
        }
    }

    void createSwitch()
    {
        bool found_location = true;

        /*for(int i = 0; i < active_locations.Length; i++)
        {
            //
            if(!active_locations[i])
            {
                found_location = true;
                routers[i].transform.position = spawn_locations[i];
                active_locations[i] = true;
                break;
            }
        }*/

        if (!found_location)
        {
            router_text.text = "No Switches";
            routerMessageTimer = 0;
        }
        else
        {
            create_submenu.GetComponent<CreateSubMenu>().is_router = false;
            create_submenu.GetComponent<CreateSubMenu>().id_no = 498;
            gameObject.SetActive(false);
            subMenu.SetActive(true);
        }
    }

    void selectRS()
    {

    }

    void linkDevice()
    {
        linkMenu.GetComponent<LinkSubMenu>().menu_active = true;
        gameObject.SetActive(false);
        linkMenu.SetActive(true);
    }
}