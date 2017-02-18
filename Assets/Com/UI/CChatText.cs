using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    public class CChatText : UISprite {
        public int MaxLabel = 30;
        public UIPanel Content;
        public UILabel Label;
        public CScrollBar Bar;
        private float totalHeight = 0;
        //未有使用
        //private List<GameObject> pool = new List<GameObject>();
        private List<GameObject> inUse = new List<GameObject>();
        private Vector2 maskOffset;
        private Vector3 contentPos;
        protected override void OnStart(){
            base.OnStart();
            Bar.OnChangeFun = OnScroll;
            contentPos = Content.transform.localPosition;
            maskOffset = Content.clipOffset;
        }

        public void AppendText(string s){
            if (inUse.Count < MaxLabel){
                GameObject go = Instantiate(Label.gameObject) as GameObject;
                go.transform.parent = Content.transform;
                go.transform.localScale = Vector3.one;
                go.GetComponent<UILabel>().text = s;
                inUse.Add(go);
            }
            else{
                if (inUse.Count > 0){
                    GameObject first = inUse[0];
                    inUse.RemoveAt(0);
                    first.transform.parent = null;
                    first.GetComponent<UILabel>().text = s;
                    first.transform.parent = Content.transform;
                    first.gameObject.transform.localScale = Vector3.one;
                }
            }
            Content.GetComponent<CGrid>().Reposition();
            totalHeight = 0;
            for (int i = 0; i < inUse.Count; i++){
                UIWidget w = inUse[i].GetComponent<UIWidget>();
                totalHeight += w.height;
            }
            Bar.gameObject.SetActive(totalHeight > this.height);
            if (Bar.gameObject.activeSelf == true){
                Bar.BarSize = this.height / totalHeight;
                Bar.value = 1; 
            }
        }

        private void OnScroll(GameObject go,float v){
            float cy = Mathf.Lerp(0, totalHeight - height, v);
            contentPos.y = cy;
            Content.transform.localPosition = contentPos;
            maskOffset.y = -cy;
            Content.clipOffset = maskOffset;
        }
    }
}