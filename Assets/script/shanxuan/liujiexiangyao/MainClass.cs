using UnityEngine;
using System.Collections;

public class MainClass : MonoBehaviour {

    private Level levelclass;

	// Use this for initialization
	void Start () {
	    
        Object gameobject = Resources.Load("LiujieXiangYaoCailiao/background") as Object ;
        GameObject background = Instantiate(gameobject) as GameObject;

        levelclass = new Level(1,background);
        //levelclass.SetPosition();
	}
	

}
