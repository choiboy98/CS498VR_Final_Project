using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class server : MonoBehaviour {

    public bool highlight;
    public GameObject hand;
    public Renderer mesh;
    Color temp;

    // Use this for initialization
    void Start () {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        hand = GameObject.Find("/OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor");
        highlight = false;
        mesh = GameObject.Find("panel_for_server").GetComponent<MeshRenderer>();
        temp = mesh.material.color;
    }
	
	// Update is called once per frame
	void Update () {

        if (highlight)
        {
            mesh.material.color = Color.yellow;
        }
        else mesh.material.color = temp; 
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == hand) highlight = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == hand) highlight = false;
    }
}
