using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.MingUI;

public class handleEvent : MonoBehaviour {

    public 

	// Use this for initialization
	void Start () {
        GameObject obj = Resources.Load("Prefabs/UIPrefabs/Background", typeof(GameObject)) as GameObject;
        GameObject gameobject = Instantiate(obj) as GameObject;


        //add button event
        CButton[] buttonlist = gameobject.GetComponentsInChildren<CButton>();
        foreach (CButton button in buttonlist)
        {
            if (button.name == "button")
            {
                button.AddClick(OnClick);
            }   
        }


        //add texture evetn
        CTextInput[] textinput = gameobject.GetComponentsInChildren<CTextInput>() ;
        foreach( CTextInput input in textinput )
        {
            if( input.name == "Input" )
            {
                Debug.Log("pass");
                input.onChange.Add(new EventDelegate(this, "change"));
                input.onSubmit.Add(new EventDelegate(this, "submit"));

                //input.onSubmit.Add(eventdelegate);
            }
        }

        print(textinput.Length);


	}

    public void change(GameObject go)
    {
        Debug.Log("change pass");
    }

    public void submit(GameObject go)
    {
        Debug.Log("submit pass");
    }


    public void OnClick(GameObject go)
    {
        Debug.Log("hello MingUI");
    }
	
    
	
}
