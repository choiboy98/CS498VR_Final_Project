using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCharacterControllerOne : MonoBehaviour {

    public Animator animChar;


    public Transform[] target;
    public float speed;

    private int current;

    // Use this for initialization
    void Start () {
        animChar = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update () {
        if (transform.position != target[current].position && current < target.Length)
        {
            animChar.SetBool("isTutorial", true);
            animChar.SetBool("idleTutorial", false);
            handleRotation(current);
            Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
        }
        else
        {
            StartCoroutine(pause());
            current = (current + 1) % target.Length;
        }
    }

    IEnumerator pause() {
        print("pausing");
        animChar.SetBool("idleTutorial", true);
        animChar.SetBool("isTutorial", false);
        yield return new WaitForSeconds(3);
    }

    void handleRotation(int current)
    {
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
