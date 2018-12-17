using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class GNS3Handle
{
    public NetworkManager manager;
    public readonly string url;
    private List<Appliance> appliances;

    public GNS3Handle(string ip, int port, NetworkManager manager_)
    {
        manager = manager_;
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
        else
        {
            onSuccess();
            yield return ListAppliances(
                (Appliances appliances_) => appliances = appliances_.appliances,
                () => Debug.Log("ListAppliances failed")
            );
        }
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

    public List<Appliance> GetAppliances()
    {
        return appliances;
    }

    public GNS3ProjectHandle ProjectHandle(string id)
    {
        return new GNS3ProjectHandle(this, id);
    }
}

public class GNS3ProjectHandle
{
    private List<Node> nodes;
    private List<Link> links;
    private readonly string url;
    private readonly GNS3Handle handle;

    public GNS3ProjectHandle(GNS3Handle handle_, string project_id)
    {
        nodes = new List<Node>();
        links = new List<Link>();
        handle = handle_;
        url = handle.url + "projects/" + project_id;
    }

    [System.Serializable]
    public class Links
    {
        public List<Link> links;

        public static Links CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Links>(json);
        }
    }

    [System.Serializable]
    public class Link
    {
        public string link_id;
        public List<Node> nodes;

        public static Link CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<Link>(json);
        }
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
        public List<Port> ports;
        public int console;
        public string console_host;
        public string console_type;

        // If this is a node in a Link then this represents that data, otherwise ignore
        public int adapter_number;
        public int port_number;
    }

    [System.Serializable]
    public class Port
    {
        public int adapter_number;
        public string link_type;
        public string name;
        public int port_number;
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

    public IEnumerator CreateAppliance(string appliance_id)
    {
        // Get type of appliance
        var appliances = handle.GetAppliances();
        GNS3Handle.Appliance appliance = appliances.Find(app => app.appliance_id == appliance_id);
        if (appliance == null)
        {
            Debug.Log("Appliance " + appliance_id + " seems to not exist");
            yield break;
        }

        string postData = @"{""compute_id"": ""vm"",""x"": 10,""y"": 10}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);

        var request = new UnityWebRequest(url + "/appliances/" + appliance_id, "POST");
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
            yield return ListNodes(
                (List<Node> newNodes) =>
                {
                    var oldNodes = GetNodes();

                    if (newNodes.Count != oldNodes.Count + 1)
                    {
                        Debug.Log("Tried to update project handle nodes but new node count (" + newNodes.Count.ToString() + ") does not match with old node count (" + oldNodes.Count.ToString() + ")");
                        return;
                    }

                    foreach (var node in newNodes)
                    {
                        if (!oldNodes.Contains(node))
                        {
                            nodes.Add(node);
                            GameObject newAppliance = null;
                            if (appliance.category == "switch")
                            {
                                newAppliance = GameObject.Instantiate(
                                    handle.manager.switchPrefab,
                                    handle.manager.transform.position + handle.manager.transform.forward,
                                    Quaternion.identity
                                );
                            }
                            else
                            {
                                newAppliance = GameObject.Instantiate(
                                    handle.manager.routerPrefab,
                                    handle.manager.transform.position + handle.manager.transform.forward,
                                    Quaternion.identity
                                );
                            }
                            var deviceScript = newAppliance.GetComponent<NetworkDevice>();
                            if (deviceScript == null)
                            {
                                Debug.Log("CreateAppliance error: device does not have a NetworkDevice script attached to it");
                                return;
                            }
                            deviceScript.node = node;
                            Debug.Log("Successfully added new " + node.node_type + " node with id " + node.node_id);
                        }
                    }
                },
                () => Debug.Log("Failed to update project handle nodes")
            );
            //
            Debug.Log(request.downloadHandler.text);
        }
    }

    public IEnumerator ListNodes(Action<List<Node>> onSuccess, Action onFailure)
    {
        var request = new UnityWebRequest(url + "/nodes", "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            onFailure();
            Debug.Log("Error: " + request.downloadHandler.text);
        }
        else
        {
            // :(
            var nodes = JsonUtility.FromJson<Nodes>("{\"nodes\":" + request.downloadHandler.text + "}");
            onSuccess(nodes.nodes);
        }
    }

    [System.Serializable]
    public class LinkInput
    {
        public LinkInput(string nodeA_id, string nodeB_id, int nodeA_portNumber, int nodeB_portNumber)
        {
            nodes = new List<LinkInputNode>();
            var a = new LinkInputNode();
            a.adapter_number = nodeA_portNumber;
            a.port_number = nodeA_portNumber;
            a.node_id = nodeA_id;
            var b = new LinkInputNode();
            b.adapter_number = nodeB_portNumber;
            b.port_number = nodeB_portNumber;
            b.node_id = nodeB_id;
            nodes.Add(a);
            nodes.Add(b);
        }

        public List<LinkInputNode> nodes;
    }

    [System.Serializable]
    public class LinkInputNode
    {
        public int adapter_number;
        public string node_id;
        public int port_number;
    }

    public IEnumerator CreateLink(string nodeA_id, string nodeB_id, int nodeA_portNumber, int nodeB_portNumber)
    {
        var input = new LinkInput(nodeA_id, nodeB_id, nodeA_portNumber, nodeB_portNumber);
        string postData = JsonUtility.ToJson(input);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);

        var request = new UnityWebRequest(url + "/links", "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.downloadHandler.text);
            yield break;
        }

        // TODO instantiate some kind of link object
        // GameObject switch_ = Instantiate(switchPrefab, transform.position + transform.forward, Quaternion.identity);
        Link link = Link.CreateFromJSON(request.downloadHandler.text);
        links.Add(link);
        Debug.Log("Successfully created link with id " + link.link_id + " connecting nodes " + link.nodes[0].node_id + " and " + link.nodes[1].node_id);
    }

    public IEnumerator StartNode(string id)
    {
        var request = new UnityWebRequest(url + "/nodes/" + id + "/start", "POST");

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Failed to start node " + id);
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Successfully started node " + id);
            Debug.Log(request.downloadHandler.text);
        }

    }
    
    public IEnumerator StopNode(string id)
    {
        var request = new UnityWebRequest(url + "/nodes/" + id + "/stop", "POST");

        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Failed to stop node " + id);
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Successfully stopped node " + id);
            Debug.Log(request.downloadHandler.text);
        }
    }

    public class ConsoleConnectArgs
    {
        public Node node;
        public TcpClient client;
        public BlockingQueue<string> read;
        public BlockingQueue<string> write;
    }

    // This starts a thread that runs a TCP connection to the input node console and returns a read queue and a write queue
    public void ConnectTo(ConsoleConnectArgs args)
    {
        var thread = new Thread(start: new ParameterizedThreadStart(runTCPThread));
        thread.Start(args);
    }

    private void runTCPThread(object args_)
    {
        ConsoleConnectArgs args = (ConsoleConnectArgs)args_;
        Node node = args.node;
        var read = args.read;
        var write = args.write;
        TcpClient client = new TcpClient();

        Debug.Log("Attempting to connect to console at " + node.console_host + ":" + node.console.ToString());
        try
        {
            IPAddress ipAddress = IPAddress.Parse(node.console_host);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, node.console);
            client.Connect(ipEndPoint);

            args.client = client;

            var readThread = new Thread(start: new ParameterizedThreadStart(runTCPReadThread));
            var writeThread = new Thread(start: new ParameterizedThreadStart(runTCPWriteThread));

            readThread.Start(args);
            Thread.Sleep(15000);
            writeThread.Start(args);
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("ArgumentNullException: " + e);
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
    }

    private void runTCPWriteThread(object args_)
    {
        ConsoleConnectArgs args = (ConsoleConnectArgs)args_;
        var client = args.client;
        var writeQueue = args.write;
        var stream = client.GetStream();

        while (true)
        {
            var toWrite = writeQueue.Dequeue();
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(toWrite);
            try
            {
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.Log("write Exception: " + e);
                return;
            }
        }
    }

    private void runTCPReadThread(object args_)
    {
        ConsoleConnectArgs args = (ConsoleConnectArgs)args_;
        var client = args.client;
        var readQueue = args.read;
        var stream = client.GetStream();

        int bytes;
        Byte[] data = new Byte[256];
        try
        {
            while ((bytes = stream.Read(data, 0, data.Length)) != 0)
            {
                bytes = stream.Read(data, 0, data.Length);
                var response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                readQueue.Enqueue(response);
                Debug.Log("response: " + response);
            }
        }
        catch (SocketException e)
        {
            Debug.Log("ReadThread: " + e);
        }
        finally
        {
            client.Close();
            Debug.Log("Leaving ReadThread");
        }
    }

    public List<Port> GetAvailablePorts(string node_id)
    {
        Node node = nodes.Find(n => n.node_id == node_id);
        if (node == null)
        {
            Debug.Log("No such node " + node_id);
            return new List<Port>();
        }
        var ports = new List<Port>();
        foreach (var link in links)
        {
            
        }
        return new List<Port>();
    }

    public List<Node> GetNodes()
    {
        return nodes;
    }

    public List<Link> GetLinks()
    {
        return links;
    }
}

public class NetworkManager : MonoBehaviour {
    public GameObject routerPrefab;
    public GameObject switchPrefab;

    private GNS3Handle handle;
    private GNS3ProjectHandle projectHandle;

    BlockingQueue<string> reader;
    BlockingQueue<string> writer;

    // Use this for initialization
    void Start() {
        handle = new GNS3Handle("192.168.56.1", 3080, this);
        projectHandle = handle.ProjectHandle("abc46e15-c32a-45ae-9e86-e896ea0afac2");
        StartCoroutine(handle.CheckHealth(
            () => Debug.Log("Connection is good"),
            () => Debug.Log("Connection is bad")
        ));
        StartCoroutine(projectHandle.CheckHealth(
            () => Debug.Log("Project connection is good"),
            () => Debug.Log("Project connection is bad")
        ));

        reader = new BlockingQueue<string>();
        writer = new BlockingQueue<string>();
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
            var appliances = handle.GetAppliances();
            foreach (var appliance in appliances)
            {
                Debug.Log(appliance.name + " " + appliance.appliance_id + " " + appliance.category);
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var nodes = projectHandle.GetNodes();
            var args = new GNS3ProjectHandle.ConsoleConnectArgs();
            args.node = nodes[0];
            args.write = writer;
            args.read = reader;
            projectHandle.ConnectTo(args);
            Thread.Sleep(100);
            writer.Enqueue("\r\r\r\rconf t\rint fa0/0\rip address 192.168.1.1 255.255.255.0\r");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(projectHandle.CreateAppliance("7465a102-5c54-4cc6-ab76-7e917955223b"));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(projectHandle.ListNodes(
                (List<GNS3ProjectHandle.Node> nodes) =>
                {
                    foreach (var node in nodes)
                    {
                        Debug.Log(node.name + " " + node.console_type);
                    }
                },
                () => Debug.Log("ListNodes failed")
            ));
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(projectHandle.CreateLink("0453abfb-900b-44cb-811c-ea77e79fda6c", "5b2ca014-913b-4a72-b6bc-23588cd34c48", 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(projectHandle.StartNode("0453abfb-900b-44cb-811c-ea77e79fda6c"));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(projectHandle.StopNode("0453abfb-900b-44cb-811c-ea77e79fda6c"));
        }
    }
}
