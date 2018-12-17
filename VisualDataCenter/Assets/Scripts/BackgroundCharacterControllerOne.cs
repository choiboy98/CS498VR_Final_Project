using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCharacterControllerOne : MonoBehaviour {

    public Animator animChar;


    public Transform[] target;
    public float speed;

    private int current;
    private float time;

    // Use this for initialization
    void Start () {
        animChar = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update () {
        if (time < 2f)
        {
            time += Time.deltaTime;
            animChar.SetBool("idleTutorial", true);
            animChar.SetBool("isTutorial", false);
        }
        else
        {
            animChar.SetBool("idleTutorial", false);
            if (transform.position != target[current].position && current < target.Length)
            {
                animChar.SetBool("isTutorial", true);
                handleRotation(current);
                Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
                GetComponent<Rigidbody>().MovePosition(pos);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                current = (current + 1) % target.Length;
                time = 0;
            }
        }
    }

    void handleRotation(int current)
    {
        if (current == 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (current == 1)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
}
