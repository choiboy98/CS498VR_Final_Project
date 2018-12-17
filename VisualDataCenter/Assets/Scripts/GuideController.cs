using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideController : MonoBehaviour {

    public Animator animChar;
    public GameObject user;
    public GameObject canvas;

    public float letterPause = 0.1f;
    public string[] message;
    public Text dialogue;

    private bool talking;
    private bool secondTalk;
    private bool thirdTalk;
    private bool forthTalk;
    private bool tutorialReady;
    private bool tutorialStart;

    public Transform[] target;
    public float speed;

    private int current;

	// Use this for initialization
	void Start () {
        animChar = GetComponent<Animator>();
        talking = false;
        secondTalk = false;
        thirdTalk = false;
        forthTalk = false;
        tutorialReady = false;
        tutorialStart = false;

        message = new string[5];
        message[0] = "Hi! Welcome to our New Data Center!";
        message[1] = "Explore routers and see your result!";
        message[2] = "First, you will go through our tutorial.";
        message[3] = "Then, you will get to explore!";
        message[4] = "Alright, follow me!";

        current = 0;
    }
	
	// Update is called once per frame
	void Update () {

        handleConversation();
        if (tutorialStart) {
          
            animChar.SetBool("isTutorial", true);
            if (transform.position != target[current].position && current < target.Length)
            {
                handleRotation(current);
                Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
                GetComponent<Rigidbody>().MovePosition(pos);
            }
            else {
                current += 1;
            }
        }
	}

    void handleConversation() {
        if (!talking && Mathf.Abs(user.transform.position.z) - Mathf.Abs(this.transform.position.z) < 4f && Input.GetKeyDown(KeyCode.X))
        {
            conversation(message[0]);
        }
        else if (secondTalk && Input.GetKeyDown(KeyCode.X) && !thirdTalk)
        {
            conversation(message[1]);
        }
        else if (thirdTalk && Input.GetKeyDown(KeyCode.X) && !forthTalk)
        {
            conversation(message[2]);
        }
        else if (forthTalk && Input.GetKeyDown(KeyCode.X) && !tutorialReady)
        {
            conversation(message[3]);
        }
        else if (tutorialReady && Input.GetKeyDown(KeyCode.X))
        {
            conversation(message[4]);
        }
    }

    void conversation(string text) {
        canvas.SetActive(true);
        talking = true;
        animChar.SetBool("isTalking", true);
        StartCoroutine(startText(text));
    }
    IEnumerator startText(string text) {
        dialogue.text = "";
        foreach (char letter in text.ToCharArray()) {
            dialogue.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
        if (!secondTalk)
        {
            secondTalk = true;
        }
        else if (!thirdTalk)
        {
            thirdTalk = true;
        }
        else if (!forthTalk)
        {
            forthTalk = true;
        }
        else if (!tutorialReady)
        {
            tutorialReady = true;
        }
        else {
            animChar.SetBool("isTalking", false);
            canvas.SetActive(false);
            tutorialStart = true;
        }
    }

    void handleRotation(int current) {
        if (current == 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (current == 1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (current == 2)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (current == 3)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
