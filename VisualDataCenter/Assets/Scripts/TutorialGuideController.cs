using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGuideController : MonoBehaviour {

    public Animator animChar;
    public GameObject user;
    public GameObject canvas;
    public GameObject initialCanvas;

    public float letterPause = 0.07f;
    public string[] message;
    public Text dialogue;

    private int current;
    private float time;

    public Transform[] target;
    public float speed;

    // Use this for initialization
    void Start () {
        animChar = GetComponent<Animator>();

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

        handleRotation(current);

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
}
