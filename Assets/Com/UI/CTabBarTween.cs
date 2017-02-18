using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;

namespace MingUI.Com.UI {
    public enum TabBarTweenSide {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public sealed class CTabBarTween : CTabBar {
        public TabBarTweenSide side = TabBarTweenSide.RIGHT;
        public float tweenOffset = 25f;
        public bool isOverOffset = true; //划过按钮是否偏移
        public float startPos { get; private set; }
        public Action<CButtonToggle, bool> hoverEvent;
        public bool moveOnOver = true;
        private Dictionary<CButtonToggle, bool> tweenState;
        private Dictionary<CButtonToggle, int> tweenTargetPos; 

        public override void Reset() {
            base.Reset();
            if (_nowUseList != null && _nowUseList.Count > 0) {
                tweenState = new Dictionary<CButtonToggle, bool>();
                tweenTargetPos = new Dictionary<CButtonToggle, int>();
                foreach (var tog in _nowUseList) {
                    if (moveOnOver) {
                        EventUtil.AddHover(tog.gameObject, OnTogHover, tog);
                    }
                    tweenState[tog] = false;
                    tweenTargetPos[tog] = 0;
                }
                if (side == TabBarTweenSide.UP || side == TabBarTweenSide.DOWN) {
                    startPos = _nowUseList[0].transform.localPosition.y;
                } else {
                    startPos = _nowUseList[0].transform.localPosition.x;
                }
            }
        }

        void OnTogHover(object arg, bool isHover) {
            var tog = arg as CButtonToggle;
            if (tog == null) return;
            if (hoverEvent != null) {
                hoverEvent.Invoke(tog, isHover);
            }
            if (!isOverOffset) return;
            var go = tog.gameObject;
            var targetPos = startPos;
            if (isHover || _nowUseList.IndexOf(tog) == _index) {
                if (side == TabBarTweenSide.LEFT || side == TabBarTweenSide.UP) {
                    targetPos -= tweenOffset;
                } else {
                    targetPos += tweenOffset;
                }
            }
            //iTween.Stop(go);
            //改变字体颜色什么的
            
            if (tweenState[tog]) {
                tweenTargetPos[tog] = (int)targetPos;
                return;
            }
            if (side == TabBarTweenSide.LEFT || side == TabBarTweenSide.RIGHT) {
                iTween.MoveTo(go,
                    iTween.Hash("x", targetPos, "time", 0.2f, "islocal", true, "easeType", iTween.EaseType.easeInOutCirc,
                    "oncompletetarget", this, "oncomplete", "ResetState", "oncompleteparams", new object[] { tog }));
            } else {
                iTween.MoveTo(go,
                    iTween.Hash("y", targetPos, "time", 0.2f, "islocal", true, "easeType", iTween.EaseType.easeInOutCirc,
                    "oncompletetarget", this, "oncomplete", "ResetState", "oncompleteparams", new object[] { tog }));
            }
            tweenState[tog] = true;
            tweenTargetPos[tog] = (int)targetPos;
        }
        protected override void ChangeIndex(bool needFun = true) {
            base.ChangeIndex(needFun);
            for (int i = 0; i < allBox.Count; i++) {
                int boxIndex =(int)allBox[i].gameObject.GetData();
                if (index != boxIndex) {
                    var tog = allBox[i];
                    tweenTargetPos[tog] = (int)startPos;
                    ResetState(tog);
                } else /*if (moveOnOver == false)*/ {
                    var tog = allBox[i];
                    var targetPos = startPos;
                    if (side == TabBarTweenSide.LEFT || side == TabBarTweenSide.UP) {
                        targetPos -= tweenOffset;
                    } else {
                        targetPos += tweenOffset;
                    }
                    tweenTargetPos[tog] = (int)targetPos;
                    ResetState(tog);
                }
            }
        }
        public void ResetState(CButtonToggle tog) {
            if (side == TabBarTweenSide.LEFT || side == TabBarTweenSide.RIGHT) {
                if ((int)tog.transform.localPosition.x != tweenTargetPos[tog]) {
                    iTween.MoveTo(tog.gameObject,
                    iTween.Hash("x", tweenTargetPos[tog], "time", 0.2f, "islocal", true, "easeType", iTween.EaseType.easeInOutCirc,
                    "oncompletetarget", this, "oncomplete", "ResetState", "oncompleteparams", new object[] { tog }));
                } else {
                    tweenState[tog] = false;
                }
            } else {
                if ((int)tog.transform.localPosition.y != tweenTargetPos[tog]) {
                    iTween.MoveTo(tog.gameObject,
                    iTween.Hash("y", tweenTargetPos[tog], "time", 0.2f, "islocal", true, "easeType", iTween.EaseType.easeInOutCirc,
                    "oncompletetarget", this, "oncomplete", "ResetState", "oncompleteparams", new object[] { tog }));
                } else {
                    tweenState[tog] = false;
                }
            }
        }

        new public void AddItem(CButtonToggle tog) {
            base.AddItem(tog);
            tweenState[tog] = false;
            tweenTargetPos[tog] = 0;
            if (moveOnOver) {
                EventUtil.AddHover(tog.gameObject, OnTogHover, tog.gameObject);
            }
        }

        new public void RemoveItem(CButtonToggle tog) {
            tweenState.Remove(tog);
            tweenTargetPos.Remove(tog);
            EventUtil.RemoveHover(tog.gameObject, OnTogHover);
            base.RemoveItem(tog);
        }

        new public void RemoveItems() {
            if (_nowUseList != null) {
                foreach (var tog in _nowUseList) {
                    EventUtil.RemoveHover(tog.gameObject, OnTogHover);
                }
            }
            tweenState.Clear();
            tweenTargetPos.Clear();
            base.RemoveItems();
        }
    }
}
