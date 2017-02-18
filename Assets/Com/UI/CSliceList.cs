using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Com.Utils;
using System.Collections;
using MingUI.Com.Manager;
namespace Assets.Scripts.Com.MingUI {
    public class CSliceList : CList {
        public VoidFun OnFinishRemove;
        public bool useFade;
        public int doneKey;
        public Action onDoneAct;
        public List<CItemRender> delList = new List<CItemRender>();
        public float fadeTime = 0.5f;
        public List<TweenAlpha> listAlpha = new List<TweenAlpha>();
        public List<int> deplayKeyList = new List<int>();

        public override void SetDataProvider<T>(IEnumerable<T> value) {
            StopMove();
            base.SetDataProvider<T>(value);
        }
        public void RemoveItem(CItemRender item) {
            if (item != null) {
                if (delList.Contains(item) == false) delList.Add(item);
                dataProvider.Remove(item.Data);
            }
            if (useFade) {
                OnFadeOut(item);
            } else {
                OnSliceOut(item);
            }
        }
        private void OnSliceOut(CItemRender item) {
            UILoopManager.ClearTimeout(doneKey);
            Hashtable hash = iTween.Hash("x", width, "time", fadeTime, "islocal", true);
            iTween.MoveTo(item.go, hash);
            deplayKeyList.Add(UILoopManager.SetTimeout<CItemRender, TweenAlpha>(OnMoveUp, fadeTime, item, null));
        }

        private void OnFadeOut(CItemRender item) {
            UILoopManager.ClearTimeout(doneKey);
            var tween = TweenAlpha.Begin(item.go, fadeTime, 0f);
            listAlpha.Add(tween);
            deplayKeyList.Add(UILoopManager.SetTimeout<CItemRender, TweenAlpha>(OnMoveUp, fadeTime, item, tween));
        }

        private int firstMoveItemIndex;
        private void OnMoveUp(CItemRender item, TweenAlpha tween) {
            if (tween != null) {
                UnityEngine.Object.DestroyImmediate(tween);
            }
            item.GetComponent<UIWidget>().alpha = 1;
            item.go.SetActive(false);
            firstMoveItemIndex = _allItem.IndexOf(item);
            if (_allItem.Count > firstMoveItemIndex + 1) {
                CItemRender firstItem = _allItem[firstMoveItemIndex + 1];
                float offY = item.y - firstItem.y;
                for (int n = firstMoveItemIndex, len = _allItem.Count; n < len; n++) {
                    CItemRender tempItem = _allItem[n];
                    //tempItem.SetParent(firstItem.tran);
                    iTween.Stop(tempItem.go);
                    Hashtable hash = iTween.Hash("y", tempItem.y + offY, "time", 0.5f, "islocal", true);
                    iTween.MoveTo(tempItem.go, hash);
                }
                //Hashtable hash = iTween.Hash("y", item.y, "time", 0.5f, "islocal", true);
                //iTween.MoveTo(firstItem.go, hash);
            }
            doneKey = UILoopManager.SetTimeout(OnDown, 0.6f);
        }

        //完成动画
        private void OnDown() {
            for (int i = 0, count = delList.Count; i < count; i++) {
                _itemPool.Add(delList[i]);
                _allItem.Remove(delList[i]);
                delList[i].SetParent(Recycle);
                delList[i].tran.localPosition = Vector3.zero;
            }
            StopMove();
            if (onDoneAct != null) {
                onDoneAct();
            }
            //ResetItems();
        }
        private void StopMove() {
            delList.Clear();
            if (_allItem != null) {
                for (int n = 0, len = _allItem.Count; n < len; n++) {
                    CItemRender tempItem = _allItem[n];
                    //tempItem.SetParent(firstItem.tran);
                    iTween.Stop(tempItem.go);
                }
            }
            if (_itemPool != null) {
                for (int n = 0, len = _itemPool.Count; n < len; n++) {
                    CItemRender tempItem = _itemPool[n];
                    //tempItem.SetParent(firstItem.tran);
                    iTween.Stop(tempItem.go);
                }
            }
            for (int i = 0, len = listAlpha.Count; i < len; i++) {
                if (listAlpha[i]) {
                    UnityEngine.Object.DestroyImmediate(listAlpha[i]);
                }
            }
            for (int r = 0, len = listAlpha.Count; r < len; r++) {
                UILoopManager.ClearTimeout(deplayKeyList[r]);
            }
            listAlpha.Clear();
            deplayKeyList.Clear();
            UILoopManager.ClearTimeout(doneKey);
        }

        //public override int SelectedIndex {
        //    set {
        //        _selectedIndex = value;
        //        _selectedItem = _dataProvider[_selectedIndex];
        //        Overlay.transform.localPosition = _allItem[_selectedIndex].localPosition;
        //        ScrollToIndex(_selectedIndex);
        //        OnItemPress(_allItem[_selectedIndex].go, false);
        //    }
        //    get { return _selectedIndex; }
        //}

        //public override int SelectedIndexWithOutScroll {
        //    set {
        //        _selectedIndex = value;
        //        _selectedItem = _dataProvider[_selectedIndex];
        //        Overlay.transform.localPosition = _allItem[_selectedIndex].localPosition;
        //        OnItemPress(_allItem[_selectedIndex].go, false);
        //    }
        //    get { return _selectedIndex; }
        //}
    }
}
