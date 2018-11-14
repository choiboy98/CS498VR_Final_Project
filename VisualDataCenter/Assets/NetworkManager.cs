using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    public GameObject routerPrefab;

	// Use this for initialization
	void Start () {
		// Connect to server
	}

    void CreateRouter()
    {
        GameObject router = Instantiate(routerPrefab, transform.position + transform.forward, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRouter();
        }
	}
}
