using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.Utils;
using MingUI.Com.Manager;

public class LiuJieInit : MonoBehaviour {


    private int CurrentIndex;
    private Level[] pictureList;

	// Use this for initialization
	void Start () {
	    
        Object gameobject = Resources.Load("LiujieXiangYaoCailiao/background") as Object  ;
        GameObject gamemain = Instantiate(gameobject) as GameObject;


        InitButton(gamemain);
        InitPicture(gamemain);

    }

    void InitPicture(GameObject gamemain)
    {
        pictureList = new Level[5] {new Level(1,gamemain) ,new Level(2,gamemain) , new Level(3,gamemain) ,new Level(4,gamemain) ,new Level(5,gamemain)};


        pictureList[0].SetPosition(-260, 30);
        pictureList[1].SetPosition( -140, 30);
        pictureList[2].SetPosition( -20 , 30);
        pictureList[3].SetPosition( 100, 30);
        pictureList[4].SetPosition( 220, 30);
        
        //默认是第一个
        CurrentIndex = 0;
        pictureList[0].ChangeBig();

    }

    void InitButton(GameObject gamemain)
    {
        //init Button
        CButton[] Blist = gamemain.GetComponentsInChildren<CButton>();
        foreach( CButton button in Blist )
        {
            if( button.name == "next")
            {
                button.AddClick(Reflash);
            }
            else if( button.name == "max")
            {
                button.AddClick(Maxflash);
            }
            else if( button.name == "reward")
            {
                button.AddClick(GetReward);
            }
        }


    }

    void Reflash(GameObject go)
    {
        int index = Random.Range(0, 5);
        Debug.Log(index);
        pictureList[CurrentIndex].ChangeSmall();
        pictureList[index].ChangeBig();
        CurrentIndex = index;
    }

    void Maxflash(GameObject go)
    {
        Debug.Log("max flash");
    }

    void GetReward(GameObject go)
    {
        Debug.Log("get reward");
    }

    void Update()
    {
        UILoopManager.FrameLoop();

    }

}
