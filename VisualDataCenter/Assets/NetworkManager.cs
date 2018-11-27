using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        string postData = @"{""compute_id"": ""vm"",""x"": 10,""y"": 10}";
        string url = "http://192.168.56.1:3080/v2/projects/836c3bbb-6aa6-4817-8c73-4697a1946d4e/appliances/1966b864-93e7-32d5-965f-001384eec461";

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Sent!");
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.downloadHandler.text);
        } else
        {
            GameObject switch_ = Instantiate(switchPrefab, transform.position + transform.forward, Quaternion.identity);
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
