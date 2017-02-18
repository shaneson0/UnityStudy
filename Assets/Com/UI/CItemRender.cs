using Assets.Scripts.Com.UI.Base;
using MingUI.Com.Utils;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CItemRender : DisplayObj {

        protected object _data;

        public int index;

        public bool isRollOver;

        private UIWidget widget;

        public CItemRender() {

        }

        public CItemRender(string goName,bool createGO = true):base(goName, createGO){

        }
        virtual public object Data {
            set { _data = value; }
            get { return _data; }
        }

        virtual public void SetCondition<T>(T condition) {
           
        }

        virtual public void Act(params object[] arg) {

        }

        virtual public void SetObjByURL(string url) {
            SetObjAndInstantiate(FuncUtil.GetUIAssetByPath(url) as GameObject);
        }

        virtual public void SetSelected(bool isSelected) {
            
        }

        public void SetAlpha(float v) {
            if (widget == null) widget = GetComponent<UIWidget>();
            if (widget != null) widget.alpha = v;
        }
    }
}