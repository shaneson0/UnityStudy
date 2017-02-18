using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.Utils;

public class Level  {

    private int level;
    private GameObject gamelevel;
    private UISprite myblock;

    //下面是gameOBject下面的组件
    private UISprite levelsprite;

    public Level( int level , GameObject parent )
    {
        this.level = level;
        LoadGameOBject(level,parent);
    }
    
    /*
     * 外部调用接口
     */
    public void SetPosition(int x , int y)
    {
        levelsprite.transform.localPosition = new Vector3(x, y, 0);
       
    }

    //外部接口：用于点击图片的时候变大

    public void ChangeBig()
    {
        this.levelsprite.transform.localScale = new Vector3(1.2F, 1.2F, 0);
        this.myblock.gameObject.SetActive(false);
    }

    //外部接口：用于点击图片的时候变小

    public void ChangeSmall()
    {
        this.levelsprite.transform.localScale = new Vector3(1F, 1F, 0);
        this.myblock.gameObject.SetActive(true);
    }

    void LoadGameOBject(int level , GameObject parent)
    {
        string name = "LiujieXiangYaoCailiao/level/level" + level.ToString();
        Object gameobject = Resources.Load(name);
        this.gamelevel = GameObject.Instantiate(gameobject) as GameObject;




        this.levelsprite = gamelevel.GetComponentInChildren<UISprite>();
        levelsprite.transform.SetParent(parent.transform);

        UISprite[] spritelist = gamelevel.GetComponentsInChildren<UISprite>();
        foreach( UISprite s in spritelist )
        {
            if( s.name == "block" )
            {
                this.myblock = s;
            }
        }


        CButton button = gamelevel.GetComponentInChildren<CButton>();
        button.AddMouseDown(ChangeSize);
        button.AddRollOver(ChangeSize);
        EventUtil.AddHover(button.gameObject, delegate(object obj, bool isHover){
            GameObject tobj = obj as GameObject;

            
            if (isHover)
            {
                tobj.transform.localScale =  new Vector3(1.2F, 1.2F, 0);
                this.myblock.gameObject.SetActive(false);
            }
            else
            {
                tobj.transform.localScale = new Vector3(1F, 1F, 0);
                this.myblock.gameObject.SetActive(true);
            }
        });
    }

    void ChangeSize(GameObject go)
    {
        Debug.Log("change size");
        go.transform.localScale = new Vector3(1.2F, 1.2F, 0);
    }




}
