using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CButtonToggle : CButton {
        public string selectSprite;
        private bool _isSeleted;
        private UISprite spLock;
        public override void SetState(UIButtonColor.State state, bool immediate) {
            if (_isSeleted) {
                this.mState = State.Pressed;
                if (!mInitDone) normalSprite = mSprite.spriteName;
                SetSprite(selectSprite);
                UpdateColor(immediate);
            } else {
                base.SetState(state, immediate);
            }
        }

        public bool seleted {
            set {
                _isSeleted = value;
                if (_isSeleted) {
                    SetState(UIButtonColor.State.Pressed, false);
                } else {
                    SetState(UIButtonColor.State.Normal, false);
                }
                if (relateChild) {
                    GetChildBtns();
                    foreach (Component child in childBtn) {
                        if (child != this) {
                            CButtonToggle temp = child as CButtonToggle;
                            if (temp != null) {
                                temp.seleted = _isSeleted;
                            }
                        }
                    }
                }
            }
            get {
                return _isSeleted;
            }
        }

        //public void AddLock() {
        //    spLock = spLock ?? UICreater.CreateSprite(GameConfig.MING_UI_ATLAS, "lock1Small", 0, 0, transform,0,0,100);
        //    spLock.MakePixelPerfect();
        //    spLock.transform.localScale = Vector3.one;
        //    spLock.transform.localPosition = new Vector3(width/2,-height/2,0);
        //}

        //public void RemoveLock() {
        //    if (spLock != null) {
        //        spLock.spriteName = "";
        //    }
        //}

    }


}
