using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideController : MonoBehaviour {

    public Animator animChar;
    public GameObject user;

    public float letterPause = 0.1f;
    public string[] message;
    public Text dialogue;

    private bool talking;
    private int i;

	// Use this for initialization
	void Start () {
        animChar = GetComponent<Animator>();
        talking = false;
        message = new string[3];
        message[0] = "Hi! Welcome to our New Data Center!";
        message[1] = "Our center allows you to have fun with Data!";
        message[2] = "First, you will go through our tutorial and then start wiring things up!";
        i = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (!talking && Mathf.Abs(user.transform.position.z) - Mathf.Abs(this.transform.position.z)  < 2f && Input.GetKeyDown(KeyCode.X)) {
            conversation();
        }
	}

    void conversation() {
        talking = true;
        animChar.SetBool("isTalking", true);
        StartCoroutine(startText());
    }
    IEnumerator startText() {
        foreach (char letter in message[i].ToCharArray()) {
            if (Input.GetKeyDown(KeyCode.S)) {
                if (!dialogue.text.Equals(letter))
                {
                    dialogue.text = message[i];
                }
                else {
                    i++;
                }
            }
            dialogue.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
    }
}
