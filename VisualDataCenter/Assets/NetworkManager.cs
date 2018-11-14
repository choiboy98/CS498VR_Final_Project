using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour {
    public GameObject routerPrefab;
    public GameObject switchPrefab;

	// Use this for initialization
	void Start () {
		// Connect to server
	}

    void CreateRouter()
    {
        GameObject router = Instantiate(routerPrefab, transform.position + transform.forward, Quaternion.identity);
    }

    IEnumerator CreateSwitch()
    {
        GameObject switch_ = Instantiate(switchPrefab, transform.position + transform.forward, Quaternion.identity);
        string postData = @"{""compute_id"": ""vm"",""x"": 10,""y"": 10}";
        Debug.Log(postData);
        // Project ID might need to be changed
        UnityWebRequest post = UnityWebRequest.Post("http://192.17.11.161:3080/v2/projects/3c6171ad-26b2-4af9-91f1-742c5a8eee34/appliances/1966b864-93e7-32d5-965f-001384eec461", postData);
        post.SetRequestHeader("Content-Type", "application/json");
        yield return post.SendWebRequest();
        if (post.isNetworkError || post.isHttpError)
        {
            Debug.Log(post.error);
            Debug.Log(post.downloadHandler.text);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRouter();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(CreateSwitch());
        }
	}
}
