using Assets.Scripts.Com.Utils;
using MingUI.Com.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CLoopList : MonoBehaviour {
        public UIPanel _content;
        public Transform recycle;
        private System.Type _itemRender;
        public UISprite mainBorder;
        public float itemWidth;
        public int mainSizeX;
        public int mainSizeY;
        public int offY;
        public bool alwaysMove = true;
        public CButton btnLeft;
        public CButton btnRight;
        private List<CItemRender> _itemPool;
        public List<CItemRender> _allItem;
        private List<object> _dataProvider;

        public int mainIndex = 3;
        public float speed;
        public bool stopOnOver;
        private bool isHoverItem;
        private float posMin;
        private float posMax;
        private Action<object> changeFun;
        private int oriSizeX;
        private int oriSizeY;
        public CItemRender lastMainItem;
        public Action<object> OnChange {
            set {
                changeFun = value;
            }
        }

        private UIPanel content {
            get {
                _content = _content ?? GetComponent<UIPanel>();
                return _content;
            }
        }

        private object lastDataProvider;
        public void SetDataProvider<T>(IEnumerable<T> value) {
            if (_itemRender == null) {
                Debug.LogError("set itemRender first!");
                return;
            }
            if (_itemPool == null) {
                _itemPool = new List<CItemRender>();
            }
            if (_allItem == null) {
                _allItem = new List<CItemRender>();
                btnLeft.AddClick(ScrollPre);
                btnRight.AddClick(ScrollNext);
            }
            SetData<T>(value);
            //_dataProvider = (value ?? new List<T>()) as List<object>;

            bool isEnough = _allItem.Count >= _dataProvider.Count;
            int n = Mathf.Abs(_allItem.Count - _dataProvider.Count);
            CItemRender item;
            while (n > 0) {
                if (isEnough == true) {
                    item = _allItem[_allItem.Count - 1];
                    _itemPool.Add(item);
                    _allItem.RemoveAt(_allItem.Count - 1);
                    item.SetParent(recycle);
                    item.tran.localPosition = Vector3.zero;
                    item.SetActive(false);
                } else {
                    if (_itemPool.Count > 0) {
                        item = _itemPool[0];
                        _itemPool.RemoveAt(0);
                        item.SetActive(true);
                    } else {
                        item = (CItemRender)Activator.CreateInstance(_itemRender);
                        EventUtil.AddHover(item, OnHover);
                        //item.width = itemWidth;
                    }
                    item.SetParent(_content.transform);
                    _allItem.Add(item);
                }
                n--;
            }
            ResetItems();
            lastDataProvider = value;
            lastMainItem = _allItem[mainIndex];
            oriSizeX = _allItem[mainIndex].width;
            oriSizeY = _allItem[mainIndex].height;
            SetMainItemRender(lastMainItem, false);
        }

        private void OnHover(object arg, bool isHover) {
            isHoverItem = isHover;
        }

        int scrollNum = 0;
        bool isMoving = false;
        private void ScrollPre(GameObject obj) {
            //正在移动被点击时，直接重置
            if (isMoving) {
                return;
            }
            SetMainItemRender(lastMainItem, false);
            for (int i = _allItem.Count - 1; i >= 0; i--) {
                CItemRender item = _allItem[i];
                Vector3 nextPos = new Vector3(item.x - itemWidth, item.y, item.z);
                iTween.MoveTo(item.go, iTween.Hash("position", nextPos, "isLocal", true, "speed", speed, "easetype", iTween.EaseType.easeInOutSine, "oncomplete", "AfterScrollPre", "oncompletetarget", this));
            }
            isMoving = true;
        }

        public void AfterScrollPre() {
            scrollNum++;
            //移动结束
            if (scrollNum >= _allItem.Count) {
                scrollNum = 0;
                //移到最后，要返回到最前面
                _allItem[0].x = _allItem[_allItem.Count - 1].x + itemWidth;
                _allItem.Sort(sortItem);
                isMoving = false;
                lastMainItem = _allItem[mainIndex];
                SetMainItemRender(lastMainItem, true);
                changeFun.DynamicInvoke(_allItem[mainIndex].Data);
            }
        }

        public void ScrollNext(GameObject obj) {
            //正在移动被点击时，直接重置
            if (isMoving) {
                return;
            }
            SetMainItemRender(lastMainItem, false);
            for (int i = _allItem.Count - 1; i >= 0; i--) {
                CItemRender item = _allItem[i];
                Vector3 nextPos = new Vector3(item.x + itemWidth, item.y, item.z);
                iTween.MoveTo(item.go, iTween.Hash("position", nextPos, "isLocal", true, "speed", speed, "easetype", iTween.EaseType.easeInOutSine, "oncomplete", "AfterScrollNext", "oncompletetarget", this));
            }
            isMoving = true;
        }

        public void AfterScrollNext() {
            scrollNum++;
            //移动结束
            if (scrollNum >= _allItem.Count) {
                scrollNum = 0;
                //移到最后，要返回到最前面
                _allItem[_allItem.Count - 1].x = _allItem[0].x - itemWidth;
                _allItem.Sort(sortItem);
                isMoving = false;
                lastMainItem = _allItem[mainIndex];
                SetMainItemRender(lastMainItem, true);
                changeFun.DynamicInvoke(_allItem[mainIndex].Data);
            }
        }

        private int curTimes = 0;
        private bool isNext = true;
        //转指定次数，负数为向前转
        public void ScrollByTimes(int times) {
            if (isMoving) {
                return;
            }
            if (times == 0) {
                changeFun.DynamicInvoke(_allItem[mainIndex].Data);
                return;
            }
            curTimes = Mathf.Abs(times);
            SetMainItemRender(lastMainItem, false);
            isNext = times > 0;
            ScrollUpdate();
            isMoving = true;
        }

        private void ScrollUpdate() {
            curTimes--;
            if (curTimes < 0) {
                lastMainItem = _allItem[mainIndex];
                SetMainItemRender(lastMainItem, true);
                changeFun.DynamicInvoke(_allItem[mainIndex].Data);
                isMoving = false;
                return;
            }
            for (int i = _allItem.Count - 1; i >= 0; i--) {
                CItemRender item = _allItem[i];
                if (isNext) {
                    Vector3 nextPos = new Vector3(item.x + itemWidth, item.y, item.z);
                    iTween.MoveTo(item.go, iTween.Hash("position", nextPos, "isLocal", true, "speed", speed * (curTimes + 1), "easetype", iTween.EaseType.linear, "oncomplete", "AfterScrollUpdate", "oncompletetarget", this));
                } else {
                    Vector3 nextPos = new Vector3(item.x - itemWidth, item.y, item.z);
                    iTween.MoveTo(item.go, iTween.Hash("position", nextPos, "isLocal", true, "speed", speed * (curTimes + 1), "easetype", iTween.EaseType.linear, "oncomplete", "AfterScrollUpdate", "oncompletetarget", this));
                }
            }
        }

        public void AfterScrollUpdate() {
            scrollNum++;
            //移动结束
            if (scrollNum >= _allItem.Count) {
                scrollNum = 0;
                //移到最后，要返回到最前面
                if (isNext)
                    _allItem[_allItem.Count - 1].x = _allItem[0].x - itemWidth;
                else
                    _allItem[0].x = _allItem[_allItem.Count - 1].x + itemWidth;
                _allItem.Sort(sortItem);
                ScrollUpdate();
            }
        }


        //private void OnLoop()
        //{
        //    if (stopOnOver && isHoverItem)
        //    {
        //        return;
        //    }
        //    float leftX = content.baseClipRegion.x - content.baseClipRegion.z / 2 + offSetLeft;
        //    float rightX = content.baseClipRegion.z / 2 + content.baseClipRegion.x + offSetRight;
        //    for (int i = _allItem.Count - 1; i >= 0; i--)
        //    {
        //        CItemRender item = _allItem[i];
        //        item.x -= speed;
        //        if (item.x < leftX)
        //        {
        //            float offX = leftX - item.x;
        //            item.x = Math.Max(rightX, _allItem[_allItem.Count - 1].x + itemWidth);
        //            item.x += offX;
        //        }
        //    }
        //    _allItem.Sort(sortItem);
        //}

        private int sortItem(CItemRender a, CItemRender b) {
            if (a.x < b.x) return -1;
            if (a.x > b.x) return 1;
            return 0;
        }

        private void SetData<T>(IEnumerable<T> value) {
            _dataProvider = _dataProvider ?? new List<object>();
            while (_dataProvider.Count > 0) {
                _dataProvider.RemoveAt(0);
            }
            if (value == null) {
                return;
            }
            foreach (var t in value) {
                _dataProvider.Add(t);
            }
        }

        private void ResetItems() {
            CItemRender item;
            for (int i = 0; i < _allItem.Count; i++) {
                item = _allItem[i];
                item.name = "Item" + i;
                CItemRender w = item;
                w.Data = _dataProvider[i];

                float itemX, itemY;
                //一列一列的顺序？
                itemY = content.baseClipRegion.w / 2 + content.baseClipRegion.y + offY;
                itemX = (i - 1) * itemWidth;
                item.tran.localPosition = new Vector3(itemX, itemY, 0);
            }
        }


        public System.Type itemRender {
            set {
                _itemRender = value;
            }
        }
        private float width {
            get {
                return _content.width;
            }
        }

        private void SetMainItemRender(CItemRender item, bool flag) {
            mainBorder.gameObject.SetActive(flag);
            if (flag) {
                item.height = mainSizeY;
                item.width = mainSizeX;
                //mainBorder.pivot = UIWidget.Pivot.Center;
                //mainBorder.transform.localPosition = new Vector3(item.x + item.width / 2, item.y - item.height / 2);
            } else {
                item.height = oriSizeY;
                item.width = oriSizeX;
            }
        }
    }
}
