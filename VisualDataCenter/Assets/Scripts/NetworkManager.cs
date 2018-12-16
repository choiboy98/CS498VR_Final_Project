using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GNS3Handle
{
    private string url;

    public GNS3Handle(string ip, int port)
    {
        url = "http://" + ip + ":" + port.ToString() + "/v2/";
        Debug.Log("Creating GNS3 handle at url " + url);
    }

    public IEnumerator HealthCheck()
    {
        var request = new UnityWebRequest(url + "version", "GET");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("HealthCheck: GNS3Handle appears to be unhealthy");
        } else
        {
            Debug.Log("HealthCheck: GNS3Handle is healthy");
        }
    }

}

public class NetworkManager : MonoBehaviour {
    public GameObject routerPrefab;
    public GameObject switchPrefab;
    private GNS3Handle handle;
    private string address = "http://192.168.56.1:3080/v2";
    private string projectId = "836c3bbb-6aa6-4817-8c73-4697a1946d4e";

    // Use this for initialization
    void Start () {
        // Connect to server
        handle = new GNS3Handle("192.168.56.1", 3080);
        StartCoroutine(handle.HealthCheck());
    }

    IEnumerator CreateRouter()
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
        }
        else
        {
            GameObject router = Instantiate(routerPrefab, transform.position + transform.forward, Quaternion.identity);
        }
    }

    IEnumerator ListNodes()
    {
        string url = address + "/projects/" + projectId + "/nodes";
        Debug.Log("GET " + url);

        var request = new UnityWebRequest(url, "GET");

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.downloadHandler.text);
        } else
        {
            Debug.Log(request.downloadHandler.text);
        }
        // TODO parse JSON to get the node ids
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(ListNodes());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Click Z");
            StartCoroutine(handle.HealthCheck());
        }
	}
}
