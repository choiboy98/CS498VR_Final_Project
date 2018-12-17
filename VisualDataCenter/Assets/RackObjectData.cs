using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RackObjectData : MonoBehaviour {

    public class ObjectData
    {
        public GameObject obj;
        public bool is_router;
        public int id;
    }

    // routers
    public ObjectData[] routers = new ObjectData[12];

    // switches
    public ObjectData[] switches = new ObjectData[12];

    GameObject router_1;
    GameObject router_2;
    GameObject router_3;
    GameObject router_4;
    GameObject router_5;
    GameObject router_6;
    GameObject router_7;
    GameObject router_8;
    GameObject router_9;
    GameObject router_10;
    GameObject router_11;
    GameObject router_12;

    GameObject switch_1;
    GameObject switch_2;
    GameObject switch_3;
    GameObject switch_4;
    GameObject switch_5;
    GameObject switch_6;
    GameObject switch_7;
    GameObject switch_8;
    GameObject switch_9;
    GameObject switch_10;
    GameObject switch_11;
    GameObject switch_12;

    // Use this for initialization
    void Start () {

        router_1 = GameObject.Find("/Racks/Front Left/Server 1");
        router_2 = GameObject.Find("/Racks/Front Left/Server 2");
        router_3 = GameObject.Find("/Racks/Front Left/Server 3");
        router_4 = GameObject.Find("/Racks/Front Left/Server 4");

        router_5 = GameObject.Find("/Racks/Front Mid/Server 1");
        router_6 = GameObject.Find("/Racks/Front Mid/Server 2");
        router_7 = GameObject.Find("/Racks/Front Mid/Server 3");
        router_8 = GameObject.Find("/Racks/Front Mid/Server 4");

        router_9 = GameObject.Find("/Racks/Front Right/Server 1");
        router_10 = GameObject.Find("/Racks/Front Right/Server 2");
        router_11 = GameObject.Find("/Racks/Front Right/Server 3");
        router_12 = GameObject.Find("/Racks/Front Right/Server 4");

        switch_1 = GameObject.Find("/Racks/Back Left/Server 1");
        switch_2 = GameObject.Find("/Racks/Back Left/Server 2");
        switch_3 = GameObject.Find("/Racks/Back Left/Server 3");
        switch_4 = GameObject.Find("/Racks/Back Left/Server 4");

        switch_5 = GameObject.Find("/Racks/Back Mid/Server 1");
        switch_6 = GameObject.Find("/Racks/Back Mid/Server 2");
        switch_7 = GameObject.Find("/Racks/Back Mid/Server 3");
        switch_8 = GameObject.Find("/Racks/Back Mid/Server 4");

        switch_9 = GameObject.Find("/Racks/Back Right/Server 1");
        switch_10 = GameObject.Find("/Racks/Back Right/Server 2");
        switch_11 = GameObject.Find("/Racks/Back Right/Server 3");
        switch_12 = GameObject.Find("/Racks/Back Right/Server 4");

        // set up stuff
        for (int i = 0; i < 12; i++)
        {
            routers[i] = new ObjectData();
            routers[i].is_router = true;

            switches[i] = new ObjectData();
            switches[i].is_router = false;
        }

        // set up GameObjects
        routers[0].obj = router_1;
        routers[1].obj = router_2;
        routers[2].obj = router_3;
        routers[3].obj = router_4;
        routers[4].obj = router_5;
        routers[5].obj = router_6;
        routers[6].obj = router_7;
        routers[7].obj = router_8;
        routers[8].obj = router_9;
        routers[9].obj = router_10;
        routers[10].obj = router_11;
        routers[11].obj = router_12;

        switches[0].obj = switch_1;
        switches[1].obj = switch_2;
        switches[2].obj = switch_3;
        switches[3].obj = switch_4;
        switches[4].obj = switch_5;
        switches[5].obj = switch_6;
        switches[6].obj = switch_7;
        switches[7].obj = switch_8;
        switches[8].obj = switch_9;
        switches[9].obj = switch_10;
        switches[10].obj = switch_11;
        switches[11].obj = switch_12;

        // set all inactive
        for (int i = 0; i < 12; i++)
        {
            routers[i].obj.SetActive(false);
            switches[i].obj.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
