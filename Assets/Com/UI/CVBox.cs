using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    public class CVBox : UISprite{
        public Transform Content;
        public CScrollBar Bar;
        public UIPanel Mask;
        private float _contentHeight;

        protected override void OnStart(){
            base.OnStart();
            if (Content != null){
                Content.GetComponent<CGrid>().Reposition();
            }
            if (Bar != null){
                Bar.OnChangeFun = OnScroll;
            }
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.parent, Content);
            _contentHeight = b.size.y;
            resetBar();
        }

        public void AddChild(Transform child){
            child.parent = Content;
            Content.GetComponent<CGrid>().Reposition();
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.parent, Content);
            _contentHeight = b.size.y;
            resetBar();
        }

        public void RemoveChild(Transform child){
            for (int i = 0; i < Content.transform.childCount; i++){
                Transform t = Content.transform.GetChild(i);
                if (t == child){
                    GameObject.Destroy(t.gameObject);
                    Content.GetComponent<CGrid>().Reposition();
                    Bounds b = NGUIMath.CalculateRelativeWidgetBounds(Content.parent, Content);
                    _contentHeight = b.size.y;
                    resetBar();
                    break;
                }
            }
        }

        private void resetBar(){
            Bar.gameObject.SetActive(_contentHeight > Mask.height);
            if (Bar.gameObject.activeSelf == true){
                Bar.BarSize = Mask.height/_contentHeight;
            }
        }

        private void OnScroll(GameObject go,float v){
            float cy = Mathf.Lerp(0, _contentHeight - height, v);
            Content.localPosition = new Vector3(0, cy, 0);
        }
    }
}