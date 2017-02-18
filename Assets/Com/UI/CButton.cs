using Assets.Scripts.Com.Managers;
using Assets.Scripts.Com.MingUI.Skins;
using Assets.Scripts.Com.Utils;
using MingUI.Com.Manager;
using MingUI.Com.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    /// <summary>
    /// 2个gameObject,btn包Label
    /// </summary>
    public class CButton : UIButton {
        public const float CLICK_GAP = 0.18f;
        private UIEventListener.VoidDelegate _clickFun;
        private UIEventListener.VoidDelegate _doubleClickFun;
        private UIEventListener.VoidDelegate _mouseDownFun;
        private UIEventListener.VoidDelegate _mouseUpFun;
        private UIEventListener.VoidDelegate _rollOverFun;
        private UIEventListener.VoidDelegate _rollOutFun;
        public string ToolTip = "";
        public float tipDely;
        public int tipWidth;
        public UILabel Label;

        public bool relateChild = false;
        protected List<Component> childBtn;
        private ButtonSkin skin;

        private bool _isEnable = true;
        public float clickGap = CLICK_GAP;
        private bool clickTiming = false;

        public Color defaultColor = new Color(1, 229 / 255f, 178 / 255f);
        public Color GrayColor = new Color(204 / 255f, 204 / 255f, 204 / 255f);

        private void Awake() {
            if (Label != null) {
                defaultColor = Label.color;
            }
        }
        public string Text {
            set { if (Label != null) Label.text = value; }
            get {
                if (Label != null) return Label.text;
                return "";
            }
        }

        public Color TextColor {
            set {
                defaultColor = value;
                if (Label != null) Label.color = value;
            }
        }

        public NGUIText.Alignment TextAlign {
            set { if (Label != null) Label.alignment = value; }
        }

        public void SetSkin(ButtonSkin _skin) {
            skin = _skin;
            mSprite.atlas = skin.atlas;
            normalSprite = skin.spNameNormal;
            hoverSprite = skin.spNameHover;
            pressedSprite = skin.spNamePressed;
            disabledSprite = skin.spNameDisabled;
        }


        protected override void OnPress(bool b) {
            if (isEnabled == false) {
                return;
            }
            base.OnPress(b);
            if (b == true) {
                if (_mouseDownFun != null) {
                    _mouseDownFun(gameObject);
                }
                FuncUtil.SetCursor("CURSOR_CLICK_DOWN");
            } else {
                if (_mouseUpFun != null) {
                    _mouseUpFun(gameObject);
                }
                //if (!UICamera.IsPressIng && FuncUtil.IsUICursor()) {
                //    FuncUtil.SetCursor("CURSOR_NORMAL");
                //}
            }

            if (relateChild) {
                GetChildBtns();
                foreach (Component child in childBtn) {
                    if (child != this) {
                        (child as CButton).isEnabled = true;
                        (child as CButton).OnPress(b);
                    }
                }
            }
        }

        protected override void OnHover(bool b) {
            base.OnHover(b);
            if (b && ToolTip != "") {
                ToolTipManager.Show(ToolTip, tipDely, tipWidth);
            } else if (b == false) {
                ToolTipManager.Hide();
            }
            if (b == true && _isEnable) {
                FuncUtil.SetCursor("CURSOR_CLICK_OVER");
                //currentTouch为null是为了防止在按钮上放开鼠标时触发OnHover
                //当鼠标离开时currentTouch为null，鼠标进入时currentTouch也为null，鼠标按下时才不为null
                if (UICamera.currentTouch == null) {
                    if (_rollOverFun != null) {
                        _rollOverFun(gameObject);
                    }
                }
            } else if (_isEnable == false) {
                FuncUtil.SetCursor("CURSOR_NORMAL");
            } else {
                if (_rollOutFun != null) {
                    _rollOutFun(gameObject);
                }
                //if (/*!UICamera.IsPressIng && */FuncUtil.IsUICursor()) {
                //    FuncUtil.SetCursor("CURSOR_NORMAL");
                //}
            }
            if (relateChild) {
                GetChildBtns();
                foreach (Component child in childBtn) {
                    if (child != this) {
                        (child as CButton).isEnabled = true;
                        (child as CButton).OnHover(b);
                    }
                }
            }
        }

        protected override void OnClick() {
            if (clickTiming==false) {
                clickTiming = true;
                UILoopManager.SetTimeout(OnClickTimeOut, clickGap);
                if (isEnabled == false) {
                    return;
                }
                base.OnClick();
                if (_clickFun != null) {
                    _clickFun(gameObject);
                }
                if (relateChild) {
                    GetChildBtns();
                    foreach (Component child in childBtn) {
                        if (child != this) {
                            (child as CButton).OnClick();
                        }
                    }
                }
            }
        }

        private void OnClickTimeOut() {
            clickTiming = false;
        }

        protected void OnDoubleClick() {
            if (isEnabled == false) {
                return;
            }
            if (_doubleClickFun != null) {
                _doubleClickFun(gameObject);
            }
        }

        private void OnTooltip(bool show) {
            return;
            if (show == true) {
                if (ToolTip != null) {
                    UITooltip.ShowText(ToolTip);
                }
            } else {
                UITooltip.ShowText(null);
            }
        }

        public void AddClick(UIEventListener.VoidDelegate fun) {
            if (_clickFun == null) {
                _clickFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveClick() {
            _clickFun = null;
        }

        public void AddDoubleClick(UIEventListener.VoidDelegate fun) {
            if (_doubleClickFun == null) {
                _doubleClickFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveDoubleClick() {
            _doubleClickFun = null;
        }

        public void     AddMouseDown(UIEventListener.VoidDelegate fun) {
            if (_mouseDownFun == null) {
                _mouseDownFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveMosueDown() {
            _mouseDownFun = null;
        }

        public void AddMouseUp(UIEventListener.VoidDelegate fun) {
            if (_mouseUpFun == null) {
                _mouseUpFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveMosueUp() {
            _mouseUpFun = null;
        }

        public void AddRollOver(UIEventListener.VoidDelegate fun) {
            if (_rollOverFun == null) {
                _rollOverFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveRollOver() {
            _rollOverFun = null;
        }

        public void AddRollOut(UIEventListener.VoidDelegate fun) {
            if (_rollOutFun == null) {
                _rollOutFun = fun;
            } else {
                Debug.LogError("CButton only support one function");
            }
        }

        public void RemoveRollOut() {
            _rollOutFun = null;
        }


        protected List<Component> GetChildBtns() {
            childBtn = childBtn ?? DisplayUtil.getComponentByType(this.gameObject, typeof(CButton));
            return childBtn;
        }

        public void AddToolTip(string msg, float delay = 0.2f, int textWidth = 170) {
            ToolTip = msg;
            tipDely = delay;
            tipWidth = textWidth;
        }

        public override bool isEnabled {
            get { return _isEnable; }
            set {
                if (_isEnable != value) {
                    _isEnable = value;
                    if (!_isEnable)
                        this.disabledColor = new Color(0, 0, 0);
                    if (Label != null) {
                        Label.color = _isEnable ? defaultColor : GrayColor;
                    }
                    SetState(value ? State.Normal : State.Disabled, false);
                }
            }
        }
    }
}