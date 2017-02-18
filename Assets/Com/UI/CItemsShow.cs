using Assets.Scripts.Com.Utils;
using MingUI.Com.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CItemsShow : MonoBehaviour {
        private UIPanel _content;
        public Transform recycle;
        private Type _itemRender;
        public float itemWidth;
        public float offSetLeft;
        public float offSetRight;
        public bool alwaysMove = true;

        private List<CItemRender> _itemPool;
        private List<CItemRender> _allItem;
        private List<object> _dataProvider;
        private object _dataCondition;

        public int offY;
        public float speed;
        public bool stopOnOver;
        public bool isStop;

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
                    }
                    item.SetParent(transform);
                    _allItem.Add(item);
                }
                n--;
            }
            ResetItems();
            lastDataProvider = value;
            if (_allItem.Count > 0) {
                if (alwaysMove || _allItem.Count * itemWidth > (content.baseClipRegion.z)) {
                    UILoopManager.AddToFrame(this, OnLoop);
                } else {
                    UILoopManager.RemoveFromFrame(this);
                }
            } else {
                UILoopManager.RemoveFromFrame(this);
            }
        }

        private void OnLoop() {
            if ((stopOnOver && IsItemOver()) || isStop) {
                return;
            }
            float leftX = content.baseClipRegion.x - content.baseClipRegion.z / 2 + offSetLeft;
            float rightX = content.baseClipRegion.z / 2 + content.baseClipRegion.x + offSetRight;
            for (int i = _allItem.Count - 1; i >= 0; i--) {
                CItemRender item = _allItem[i];
                item.x -= speed;
                if (item.x < leftX) {
                    float offX = leftX - item.x;
                    item.x = Math.Max(rightX, _allItem[_allItem.Count - 1].x + itemWidth);
                    item.x += offX;
                }
            }
            _allItem.Sort(sortItem);
        }

        private bool IsItemOver() {
            for (int i = 0; i < _allItem.Count;i++ ) {
                if (_allItem[i].isRollOver) {
                    return true;
                }
            }
            return false;
        }

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

        public void SetDataCondition<T>(T value) {
            _dataCondition = value;
        }

        private void ResetItems() {
            CItemRender item;
            for (int i = 0; i < _allItem.Count; i++) {
                item = _allItem[i];
                item.name = "Item" + i;
                CItemRender w = item;
                if (_dataCondition != null) {
                    w.SetCondition(_dataCondition);
                }
                w.Data = _dataProvider[i];

                float itemX, itemY;
                //一列一列的顺序？
                itemY = content.baseClipRegion.w / 2 + content.baseClipRegion.y + offY;
                itemX = i * itemWidth;
                item.tran.localPosition = new Vector3(itemX, itemY, 0);
                List<BoxCollider> boxList = DisplayUtil.GetComponentByType<BoxCollider>(item.go);
                for (int n = 0, len = boxList.Count; n < len;n++ ) {
                    EventUtil.AddHover(boxList[n].gameObject, OnHover, item);
                }
            }
        }

        private void OnHover(object arg,bool isHover) {
            (arg as CItemRender ).isRollOver = isHover;
        }

        public List<CItemRender> allItem() {
            return _allItem;
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
    }
}
