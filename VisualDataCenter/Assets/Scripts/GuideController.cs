using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideController : MonoBehaviour {

    public Animator animChar;
    public GameObject user;
    public GameObject canvas;
    public GameObject initialCanvas;

    public float letterPause = 0.07f;
    public string[] message;
    public Text dialogue;

    private bool talking;
    private bool secondTalk;
    private bool thirdTalk;
    private bool forthTalk;
    private bool tutorialReady;
    private bool tutorialStart;
    private bool isTalking;

    public Transform[] target;
    public float speed;

    private int current;
    private float time;

    public GameObject redX;
    public GameObject whiteX;
    public GameObject textX;

	// Use this for initialization
	void Start () {
        animChar = GetComponent<Animator>();
        talking = false;
        secondTalk = false;
        thirdTalk = false;
        forthTalk = false;
        tutorialReady = false;
        tutorialStart = false;
        isTalking = false;

        time = 0;

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
            if (current < target.Length && transform.position != target[current].position)
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
        
        if (!talking && Mathf.Abs(user.transform.position.z) - Mathf.Abs(this.transform.position.z) < 4f && (OVRInput.GetDown(OVRInput.Button.One)) && !isTalking)
        {
            this.GetComponent<AudioSource>().Play();
            initialCanvas.SetActive(false);
            conversation(message[0]);
        }
        else if (secondTalk && (OVRInput.GetDown(OVRInput.Button.One)) && !thirdTalk && !isTalking)
        {
            conversation(message[1]);
        }
        else if (thirdTalk && (OVRInput.GetDown(OVRInput.Button.One)) && !forthTalk && !isTalking)
        {
            conversation(message[2]);
        }
        else if (forthTalk && (OVRInput.GetDown(OVRInput.Button.One)) && !tutorialReady && !isTalking)
        {
            conversation(message[3]);
        }
        else if (tutorialReady && (OVRInput.GetDown(OVRInput.Button.One)) && !isTalking)
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
        redX.SetActive(false);
        whiteX.SetActive(false);
        textX.SetActive(false);
        isTalking = true;
        foreach (char letter in text.ToCharArray()) {
            dialogue.text += letter;
            yield return new WaitForSeconds(letterPause);
        }

        isTalking = false;

        redX.SetActive(true);
        textX.SetActive(true);
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
            redX.SetActive(false);
            whiteX.SetActive(false);
            textX.SetActive(false);
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
