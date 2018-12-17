using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    Queue<string> textQueue = new Queue<string>();
    Text[] text;
    int commandLen = 7;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < commandLen; i++){
            text[i] = GameObject.find("ConsoleWindow" + i).getComponent<Text>()
        }
	}
	
	// Update is called once per frame
	void Update () {
        setConsoleText(s);
	}
    
    String public setConsoleText(String s){
        if (textQueue.Count <= commandLen){
            textQueue.Enqueue(s);
            for (int i = 0; i < textQueue.Length; i++){
                text[i].text = textQueue[i];
            }
            return;
        }
        textQueue.Dequeue();
        textQueue.Enqueue(s);
        for (int i = 0; i < textQueue.Length; i++){
            text[i].text = textQueue[i];
        }
    }
}

