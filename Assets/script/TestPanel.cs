using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.MingUI;

public class TestPanel {

    private GameObject prefab;
    private CButton button;

    public TestPanel()
    {
        Debug.Log("run here 3");
        Start();
    }

	// Use this for initialization
	void Start () {
        prefab = Resources.Load("TestPanel.prefab") as GameObject;
        CButton[] btns = prefab.GetComponentsInChildren<CButton>();
        foreach (CButton btn in btns)
        {
            if (btn.gameObject.name == "button")
            {
                button = btn;
                break;
            }
        }
        button.AddClick(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnClick(GameObject obj)
    {
        Debug.Log("You are clicking");
    }
}
