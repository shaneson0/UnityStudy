using MingUI.Com.Utils;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    public class CCheckBox : UIToggle{
        private UIEventListener.BoolDelegate _changeFun;
        public UILabel Label;
        public string Text{
            set { Label.text = value; }
            get { return Label.text; }
        }

        public bool Selected{
            set{
                if (base.value != value){
                    base.value = value;
                }
                if (_changeFun != null){
                    _changeFun(gameObject, value);
                }
            }
            get { return base.value; }
        }

        public void OnClick() {
            Selected = !Selected;
        }

        public void AddChangeFun(UIEventListener.BoolDelegate fun){
            if (_changeFun == null){
                _changeFun = fun;
            }
            else{
               Debug.LogError("CButton only support one function");
            }
        }

        public void SetSelectedWithOutFun(bool value) {
            if (base.value != value) {
                base.value = value;
            }
        }

        public void RemoveChangeFun(){
            _changeFun = null;
        }

        private bool _isEnabled = true;
        public bool isEnable {
            set {
                if (value != _isEnabled) {
                    _isEnabled = value;
                    var color = value ? new Color(237 / 255f, 227 / 255f, 187 / 255f) : new Color(115 / 255f, 115 / 255f, 115 / 255f);
                    Component[] uiws = gameObject.GetComponentsInChildren(typeof(UIWidget));
                    for (int index = 0; index < uiws.Length; index++) {
                        UIWidget w = uiws[index] as UIWidget;
                        if (w != activeSprite || Selected == true) {
                            w.color = color;
                        }
                    }
                    setBoxColliderEnable(value);
                }
            }
            get {
                return _isEnabled;
            }
        }

        public void setBoxColliderEnable(bool value) { 
            Component box = gameObject.GetComponent(typeof(BoxCollider));
            (box as BoxCollider).enabled = value;
        }

        void OnHover(bool isSelected) {
            FuncUtil.SetCursor(isSelected ? "CURSOR_CLICK_OVER" : "CURSOR_NORMAL");
        }

        void OnPress(bool isPress) {
            FuncUtil.SetCursor(isPress ? "CURSOR_CLICK_DOWN" : "CURSOR_NORMAL");
        }
    }
}