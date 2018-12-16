using System;
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

    public IEnumerator HealthCheck(Action onSuccess, Action onFailure)
    {
        var request = new UnityWebRequest(url + "version", "GET");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            onFailure();
        }
        onSuccess();
    }

    [System.Serializable]
    public class Projects
    {
        public List<Project> projects;

        public static Projects CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Projects>(json);
        }
    }

    [System.Serializable]
    public class Project
    {
        public string name;
        public string project_id;

        public static Project CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Project>(json);
        }
    }

    public IEnumerator ListProjects(Action<Projects> onSuccess, Action onFailure)
    {
        var request = new UnityWebRequest(url + "projects", "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            onFailure();
        }
        // :(
        var projects = JsonUtility.FromJson<Projects>("{\"projects\":"+request.downloadHandler.text+"}");
        onSuccess(projects);
    }
}

public class NetworkManager : MonoBehaviour {
    public GameObject routerPrefab;
    public GameObject switchPrefab;
    private GNS3Handle handle;
    private string address = "http://192.168.56.1:3080/v2";
    private string projectId = "836c3bbb-6aa6-4817-8c73-4697a1946d4e";

    // Use this for initialization
    void Start() {
        // Connect to server
        handle = new GNS3Handle("192.168.56.1", 3080);
        StartCoroutine(handle.HealthCheck(
            () => Debug.Log("Connection is good"),
            () => Debug.Log("Connection is bad")
        ));
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRouter();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(CreateSwitch());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(handle.ListProjects(
                (GNS3Handle.Projects projects) => {
                    foreach (var project in projects.projects)
                    {
                        Debug.Log(project.name + " " + project.project_id);
                    }
                },
                () => Debug.Log("Failed")
            ));
        }

    }
}
