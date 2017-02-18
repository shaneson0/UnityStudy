using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    private TestPanel panel;

	// Use this for initialization
	void Start () {
        Debug.Log("run here");
       
        panel = new TestPanel();

        Debug.Log("run here too");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
