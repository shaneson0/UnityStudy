using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI{
    /// <summary>
    /// 这个类可以实现HBox，VBox，TileList的功能，对子对象的尺寸不要求一样
    /// 不带滚动条
    /// </summary>
    public class CGrid : MonoBehaviour{
        public int ColNum = 1; //列数
        public int HGap; //间隙，ByItemSize为false时，可以当作子对象尺寸为0
        public int VGap;
        public bool ByItemSize; //是否使用子对象尺寸来排序，false时使用HGap,VGap，true时用子对象尺寸+Gap
        public int PaddingLeft; //可以不从0,0点开始排
        public int PaddingTop;

        protected System.Type _itemRender = typeof (CDefaultItemRender);

        protected List<object> _dataProvider;
        protected List<CItemRender> _itemPool = new List<CItemRender>();

        private void Start(){
            //            Reposition();
        }

        public int Height { get; private set; }

        public System.Type itemRender{
            set{
                _itemRender = value;
                //CItemRender item = (CItemRender) Activator.CreateInstance(_itemRender);
                //DestroyImmediate(item.go);
            }
        }

        public bool IsSame<T>(object a, object b) {
            if (a == null || b == null) return false;
            IList<object> list1 = a as IList<object>;
            IList<T> list2 = b as IList<T>;
            if (list1 == null || list1.Count == 0 || list2 == null || list2.Count == 0 || list1.Count != list2.Count) return false;
            var len = list1.Count;
            for (var i = 0; i < len; i++) {
                if (!list1[i].Equals(list2[i])) return false;
            }
            return true;
        }

        public void SetDataProvider<T>(IEnumerable<T> value) {
            if (IsSame<T>(_dataProvider, value)) {
                return;
            }   
            _dataProvider = _dataProvider ?? new List<object>();
            while (_dataProvider.Count > 0){
                _dataProvider.RemoveAt(0);
            }
            if (value == null){
                return;
            }
            foreach (var t in value){
                _dataProvider.Add(t);
            }

            int curLen = _dataProvider.Count;
            for (var i = 0; i < curLen; i++){
                CItemRender item;
                if (_itemPool.Count < curLen){
                    item = (CItemRender) Activator.CreateInstance(_itemRender);
                    item.name = "Item" + i;
                    _itemPool.Add(item);
                }
                else{
                    item = _itemPool[i];
                }
                item.Data = _dataProvider[i];
                item.SetParent(gameObject);
                item.SetActive(true);
            }
            for (var i = curLen; i < _itemPool.Count; i++){
                CItemRender item = _itemPool[i];
                item.SetActive(false);
            }
            Reposition();
        }

        public virtual void Reposition(){
            if (ColNum == 0){
                //排成一行
                RepositionInOneRow();
                return;
            }
            int maxItemHeight = 0;
            int endX = PaddingLeft; //上个item的x+width
            int endY = PaddingTop; //上一行的y+最大的itemHeight
            int curIndex = 0;
            for (int i = 0; i < transform.childCount; i++){
                Transform t = transform.GetChild(i);
                if (t.gameObject.activeSelf == false){
                    continue;
                }
                int row = ColNum == 0 ? 0 : Mathf.FloorToInt(curIndex/ColNum);
                int col = ColNum == 0 ? curIndex : curIndex%ColNum;
                int x;
                int y;
                bool isRowLast = (curIndex % ColNum == ColNum - 1); //行的最后一个
                if (ByItemSize == true){
                    bool isNewRow = (curIndex%ColNum == 0);
                    UIWidget w = t.GetComponent<UIWidget>();

                    if (isNewRow == true){
                        x = PaddingLeft;
                        maxItemHeight = w.height;
                    }
                    else{
                        x = endX + HGap;
                        if (w.height > maxItemHeight){
                            maxItemHeight = w.height;
                        }
                    }
                    endX = x + w.width;
                    if (row == 0){
                        y = endY;
                    }
                    else{
                        y = endY - VGap;
                    }
                    if (isRowLast == true){
                        if (row == 0){
                            endY = endY - maxItemHeight;
                        }
                        else{
                            endY = endY - VGap - maxItemHeight;
                        }
                        Height = Math.Abs(endY);
                    }
                }
                else{
                    x = PaddingLeft + col*HGap;
                    y = PaddingTop - row*VGap;
                }
                t.localPosition = new Vector3(x, y, 0);
                curIndex++;
            }
            if (transform.childCount == 0) {
                Height = 0;
            }
            else {
                int rowNum = ColNum == 0 ? 0 : Mathf.FloorToInt((curIndex-1) / ColNum);
                Height = PaddingTop + (ByItemSize ? (rowNum + 1) * (VGap + maxItemHeight) : (rowNum + 1) * VGap);
            }
        }

        private void RepositionInOneRow(){
            int endX = PaddingLeft; //上个item的x+width
            if (_dataProvider == null){
                for (int i = 0; i < transform.childCount; i++){
                    Transform t = transform.GetChild(i);
                    if (ByItemSize == true){
                        int x = i == 0 ? endX : endX + HGap;
                        UIWidget w = t.GetComponent<UIWidget>();
                        endX = x + w.width;
                        t.localPosition = new Vector3(x, PaddingTop, 0);
                    }
                    else{
                        t.localPosition = new Vector3(PaddingLeft + i*HGap, PaddingTop, 0);
                    }
                }
            }
            else{
                for (int i = 0; i < _dataProvider.Count; i++){
                    var t = _itemPool[i].tran;
                    if (ByItemSize == true){
                        int x = i == 0 ? endX : endX + HGap;
                        UIWidget w = t.GetComponent<UIWidget>();
                        endX = x + w.width;
                        t.localPosition = new Vector3(x, PaddingTop, 0);
                    }
                    else{
                        t.localPosition = new Vector3(PaddingLeft + i*HGap, PaddingTop, 0);
                    }
                }
            }
        }

        public void ActFun(params object[] arg){
            foreach (var items in _itemPool){
                items.Act();
            }
        }
    }
}