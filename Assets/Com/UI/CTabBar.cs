using Assets.Scripts.Com.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CTabBar : CRadioGroup {
        private int btnSpace = 0;
        private Color normalColor = new Color(191 / 255f, 178 / 255f, 115 / 255f);
        private Color selColor = new Color(255 / 255f, 245 / 255f, 140 / 255f);
        private Color outLineColor = Color.black;
        private Color selOutLineColor = Color.black;
        private UILabel.Effect effect = UILabel.Effect.None;
        private bool isSelOutLine = false;
        private Vector3 tog0Pos;

        public void SetColor(Color normal, Color sel) {
            normalColor = normal;
            selColor = sel;
        }

        public void SetOutLine(Color col) {
            outLineColor = col;
            effect = UILabel.Effect.Outline;
            if (_nowUseList != null) {
                for (int i = 0, len = _nowUseList.Count; i < len; i++) {
                    CButtonToggle btn = _nowUseList[i];
                    btn.Label.effectStyle = effect;
                    btn.Label.effectColor = outLineColor;
                }
            }
        }

        public void SetSelOutLine(Color colNor, Color colSel) {
            SetOutLine(colNor);
            isSelOutLine = true;
            selOutLineColor = colSel;
            if (_nowUseList != null) {
                for (int i = 0, len = _nowUseList.Count; i < len; i++) {
                    CButtonToggle btn = _nowUseList[i];
                    btn.Label.applyGradient = false;
                }
            }
        }
        public override void Reset() {
            base.Reset();
            for (int i = 0, len = _nowUseList.Count; i < len; i++) {
                CButtonToggle btn = _nowUseList[i];
                if (btn.Label != null) {
                    btn.Label.effectStyle = effect;
                    btn.Label.effectColor = outLineColor;
                }
                btn.TextColor = normalColor;
                if (i == 0) {
                    tog0Pos = btn.transform.localPosition;
                }
            }
        }

        protected override void ChangeIndex(bool needFun = true) {
            if (_nowUseList == null) {
                return;
            }
            selBtn = null;
            for (int i = 0, len = _nowUseList.Count; i < len; i++) {
                CButtonToggle btn = _nowUseList[i];
                btn.seleted = (Convert.ToInt32(btn.gameObject.GetData()) == index);
                if (btn.seleted) {
                    selBtn = btn;
                    btn.TextColor = selColor;
                    if (isSelOutLine)
                        btn.Label.effectColor = selOutLineColor;
                } else {
                    btn.TextColor = normalColor;
                    if (isSelOutLine)
                        btn.Label.effectColor = outLineColor;
                }
            }
            if (selBtn != null && needFun && changeFun != null && index >= 0) {
                var data = selBtn.gameObject.GetData();
                changeFun.DynamicInvoke(data != null ? Convert.ToInt32(data) : index);
            }
        }
        public void RemoveItems() {
            if (_nowUseList != null) {
                foreach (CButtonToggle tog in _nowUseList) {
                    _unUseList.Add(tog);
                    tog.gameObject.SetActive(false);
                }
                _nowUseList.Clear();
            }
        }

        //在初始化的时候就给所有的CButtonToggle设置好对应的index了 如果是新加的 请调用一次Reset去重设他的index
        public void AddItem(CButtonToggle tog) {
            _unUseList.Remove(tog);
            _nowUseList.Add(tog);
            tog.gameObject.SetActive(true);
        }

        public void RemoveItem(CButtonToggle tog) {
            _unUseList.Add(tog);
            _nowUseList.Remove(tog);
            tog.gameObject.SetActive(false);
        }

        public void RemoveItem(int index) {
            if (_dic.ContainsKey(index)) {
                RemoveItem(_dic[index]);
            }
        }

        public void setBtnSpace(int space) {
            btnSpace = space;
        }

        public void UpdateItemPos() {
            for (int i = 0, len = _nowUseList.Count; i < len; i++) {
                CButtonToggle tol = _nowUseList[i];
                float x = tol.transform.localPosition.x;
                float y = tol.transform.localPosition.y;
                if (i != 0) {
                    CButtonToggle tol2 = _nowUseList[i - 1];
                    if (isHorizon) {
                        x = tol2.transform.localPosition.x + tol2.width + btnSpace;
                    } else {
                        y = tol2.transform.localPosition.y - tol2.height - btnSpace;
                    }
                } else {
                    if (isHorizon) {
                        x = tog0Pos.x;
                    } else {
                        y = tog0Pos.y;
                    }
                }
                tol.transform.localPosition = new Vector3(x, y, 0);
            }
        }

        public void SetTog0Pos(Vector3 pos) {
            tog0Pos = pos;
        }
    }
}