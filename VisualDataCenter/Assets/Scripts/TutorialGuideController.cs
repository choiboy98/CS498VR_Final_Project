﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGuideController : MonoBehaviour {

    public Animator animChar;
    public GameObject user;
    public GameObject canvas;

    public float letterPause = 0.07f;
    public string[] message;
    public Text dialogue;

    private int current;
    private float time;

    private bool talking;
    private bool secondTalk;
    private bool thirdTalk;
    private bool forthTalk;
    private bool tutorialReady;
    private bool tutorialStart;
    private bool isTalking;

    public Transform[] target;
    public float speed;

    public GameObject redX;
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
        message[0] = "This is the server room!";
        message[1] = "You can create routers and servers.";
        message[2] = "Then, you can connect them together to create networks.";
        message[3] = "Here, give it a try!";
        message[4] = "Press X to open up your Context Menu.";

        current = 0;
    }
	
	// Update is called once per frame
	void Update () {

        handleRotation(current);
        handleConversation();
        if (current < target.Length && transform.position != target[current].position)
        {
            animChar.SetBool("isTutorial", true);

            Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
        }
        else
        {
            if (current <= target.Length)
            {
                current += 1;
            }
        }

        if (current == 2) {
            this.GetComponent<AudioSource>().Play();
            animChar.SetBool("isTutorial", false);
            animChar.SetBool("idleTutorial", true);
            animChar.SetBool("isTalking", true);
        }
    }

    void handleRotation(int current)
    {
        if (current == 1)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (current == 2) {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }

    void handleConversation()
    {

        if (!talking && Mathf.Abs(user.transform.position.z) - Mathf.Abs(this.transform.position.z) < 4f && Input.GetKeyDown(KeyCode.X) && !isTalking)
        {
            this.GetComponent<AudioSource>().Play();
            conversation(message[0]);
        }
        else if (secondTalk && Input.GetKeyDown(KeyCode.X) && !thirdTalk && !isTalking)
        {
            conversation(message[1]);
        }
        else if (thirdTalk && Input.GetKeyDown(KeyCode.X) && !forthTalk && !isTalking)
        {
            conversation(message[2]);
        }
        else if (forthTalk && Input.GetKeyDown(KeyCode.X) && !tutorialReady && !isTalking)
        {
            conversation(message[3]);
        }
        else if (tutorialReady && Input.GetKeyDown(KeyCode.X) && !isTalking)
        {
            conversation(message[4]);
        }
    }

    void conversation(string text)
    {
        canvas.SetActive(true);
        talking = true;
        animChar.SetBool("isTalking", true);
        StartCoroutine(startText(text));
    }
    IEnumerator startText(string text)
    {
        dialogue.text = "";
        redX.SetActive(false);
        textX.SetActive(false);
        isTalking = true;
        foreach (char letter in text.ToCharArray())
        {
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

    }
}
