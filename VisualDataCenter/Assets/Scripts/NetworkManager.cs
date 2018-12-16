using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GNS3Handle
{
    public readonly string url;

    public GNS3Handle(string ip, int port)
    {
        url = "http://" + ip + ":" + port.ToString() + "/v2/";
        Debug.Log("Creating GNS3 handle at url " + url);
    }

    public IEnumerator CheckHealth(Action onSuccess, Action onFailure)
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
    public class Appliances
    {
        public List<Appliance> appliances;

        public static Appliances CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Appliances>(json);
        }
    }

    [System.Serializable]
    public class Appliance
    {
        public string appliance_id;
        public string category;
        public string name;
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
        else
        {
            // :(
            var projects = JsonUtility.FromJson<Projects>("{\"projects\":" + request.downloadHandler.text + "}");
            onSuccess(projects);
        }
    }

    public IEnumerator ListAppliances(Action<Appliances> onSuccess, Action onFailure)
    {
        var request = new UnityWebRequest(url + "appliances", "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            onFailure();
        }
        else
        {
            // :(
            var appliances = JsonUtility.FromJson<Appliances>("{\"appliances\":" + request.downloadHandler.text + "}");
            onSuccess(appliances);
        }
    }

    public GNS3ProjectHandle ProjectHandle(string id)
    {
        return new GNS3ProjectHandle(this, id);
    }
}

public class GNS3ProjectHandle
{
    private readonly string url;

    public GNS3ProjectHandle(GNS3Handle handle, string project_id)
    {
        url = handle.url + "projects/" + project_id;
    }

    [System.Serializable]
    public class Nodes {
        public List<Node> nodes;

        public static Nodes CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Nodes>(json);
        }
    }

    [System.Serializable]
    public class Node
    {
        public string name;
        public string node_id;
        public string status;
        public string node_type;
    }

    public IEnumerator CheckHealth(Action onSuccess, Action onFailure)
    {
        Debug.Log("GET " + url);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            onFailure();
        } else
        {
            onSuccess();
        }
    }

    public IEnumerator CreateAppliance(string application_id)
    {
        string postData = @"{""compute_id"": ""vm"",""x"": 10,""y"": 10}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);

        var request = new UnityWebRequest(url + "/appliances/" + application_id, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // TODO
            // GameObject switch_ = Instantiate(switchPrefab, transform.position + transform.forward, Quaternion.identity);
            Debug.Log(request.downloadHandler.text);
        }
    }

    public IEnumerator ListNodes()
    {
        var request = new UnityWebRequest(url + "/nodes", "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            // onFailure();
            Debug.Log("Error: " + request.downloadHandler.text);
        }
        else
        {
            // :(
            var nodes = JsonUtility.FromJson<Nodes>("{\"nodes\":" + request.downloadHandler.text + "}");
            foreach (var node in nodes.nodes)
            {
                Debug.Log(node.name + " " + node.node_id + " " + node.status + " " + node.node_type); 
            }
            // onSuccess(appliances);
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
    void Start() {
        // Connect to server
        handle = new GNS3Handle("192.168.56.1", 3080);
        StartCoroutine(handle.CheckHealth(
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(handle.ListAppliances(
                (GNS3Handle.Appliances appliances) => {
                    foreach (var appliance in appliances.appliances)
                    {
                        Debug.Log(appliance.name + " " + appliance.appliance_id);
                    }
                },
                () => Debug.Log("Failed")
            ));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var projectHandle = handle.ProjectHandle("7ad8c8fd-ccc9-4ec6-b1c6-4b8c27d5c71c");
            StartCoroutine(projectHandle.CheckHealth(
                () => Debug.Log("Project 7ad8c8fd-ccc9-4ec6-b1c6-4b8c27d5c71c connection is good"),
                () => Debug.Log("Project 7ad8c8fd-ccc9-4ec6-b1c6-4b8c27d5c71c connection is bad")
            ));
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            var projectHandle = handle.ProjectHandle("7ad8c8fd-ccc9-4ec6-b1c6-4b8c27d5c71c");
            StartCoroutine(projectHandle.CreateAppliance("1966b864-93e7-32d5-965f-001384eec461"));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            var projectHandle = handle.ProjectHandle("7ad8c8fd-ccc9-4ec6-b1c6-4b8c27d5c71c");
            StartCoroutine(projectHandle.ListNodes());
        }
    }
}
