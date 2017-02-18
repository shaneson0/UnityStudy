using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.MingUI;
using MingUI.Com.Manager;

public class TestInit : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Object testobject = Resources.Load("Button") as Object;
        GameObject gameobject = Instantiate(testobject) as GameObject;

        CButton btn = gameobject.GetComponent<CButton>();
        btn.AddClick(clickevent);

	}

    private int i;
    void clickevent(GameObject go)
    {
        Debug.Log("pass" + i++);
    }

	// Update is called once per frame
	void Update () {
        UILoopManager.FrameLoop();

    }
}
