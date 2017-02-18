using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    public class CHBox : UISprite{
        public CGrid Content;
        public CScrollBar Bar;
        private float contentWidth;
        private Vector3 contentPos;

        protected override void OnStart(){
            base.OnStart();
            contentPos = Content.transform.localPosition;
            Content.Reposition();
            if (Bar != null){
                Bar.OnChangeFun = OnScroll;
            }
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.transform.parent, Content.transform);
            contentWidth = b.size.x;
            ResetBar();
        }

        public void AddChild(Transform child){
            child.parent = Content.transform;
            Content.GetComponent<CGrid>().Reposition();
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.transform.parent, Content.transform);
            contentWidth = b.size.x;
            ResetBar();
        }

        public void RemoveChild(Transform child){
            for (int i = 0; i < Content.transform.childCount; i++){
                Transform t = Content.transform.GetChild(i);
                if (t == child){
                    GameObject.Destroy(t.gameObject);
                    Content.GetComponent<CGrid>().Reposition();
                    Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.transform.parent, Content);
                    contentWidth = b.size.x;
                    ResetBar();
                    break;
                }
            }
        }

        private void OnScroll(GameObject go, float v){
            float cx = Mathf.Lerp(0, width - contentWidth, v);
            contentPos.x = cx;
            Content.transform.localPosition = contentPos;
        }

        private void ResetBar(){
            Bar.gameObject.SetActive(contentWidth > this.width);
            if (Bar.gameObject.activeSelf == true){
                Bar.BarSize = this.width/contentWidth;
            }
        }
    }
}