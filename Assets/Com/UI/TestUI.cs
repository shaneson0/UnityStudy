using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    public class TestUI : MonoBehaviour{
        private GameObject list;
        private GameObject scroll;
        private GameObject vt;
        private List<object> da;

        private void Start(){
//            GameObject CameraEmptyObj = GameObject.Find("CameraEmptyObj");
//            UICamera.genericEventHandler = CameraEmptyObj;
//            list = GameObject.Find("List");
//            da = new List<object>();
//            for (int i = 0; i < 21; i++){
//                da.Add("法案的发的方法的飞洒" + i);
//            }
//            list.GetComponent<CList>().DataProvider = da;
//            GameObject btn = GameObject.Find("MyButton");
//            btn.GetComponent<CButton>().AddClick(OnClickBtn);
//            vt = GameObject.Find("VText");
//            Invoke("delay", 1);

            GameObject can = GameObject.Find("Canves");
            CButton[] b = can.GetComponentsInChildren<CButton>();
            Debug.Log(b.Length);
        }

        private void Delay(){
            List<object> d = new List<object>();
            for (int i = 0; i < 5; i++){
                d.Add("法案的发的方法的飞洒" + i);
            }
            list.GetComponent<CList>().SetDataProvider<object>(d);
        }

        private void OnClickBtn(GameObject go){
            string s = "是打发士大夫按时打发似的发阿斯顿阿嘎时代噶打";
            vt.GetComponent<CVText>().Add(s);
        }

        private void OnGUI(){
            if (GUILayout.Button("btn")) {
                Debug.Log("Clicked Button");
            }
        }
    }
}