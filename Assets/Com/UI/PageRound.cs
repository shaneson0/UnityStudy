using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
   public class PageRound : MonoBehaviour {
        public UISprite unit;
        private int num = 1;
        private int tmpNum;
        private List<GameObject> unitList;
        private List<GameObject> pool;
        private int _selectedIndex;
        public Transform Recycle;
        public UIWidget Parent;

        private void Start() { 
            unitList = new List<GameObject>(); 
            unitList.Add(unit.gameObject);
            pool = new List<GameObject>();
            if (tmpNum > 0) {
                SetNum(tmpNum);
            }
            selectedIndex = _selectedIndex;
        }

        public void SetNum(int num) {
            if (pool == null) {
                tmpNum = num;
                return;
            }
            GameObject gameObj;
            if (this.num < num) {
                for (var i = this.num; i < num; i++) {
                    if (pool.Count > 0) {
                        gameObj = pool[0];
                        pool.RemoveAt(0);
                    } else {
                        gameObj = Instantiate(unit.gameObject) as GameObject;
                    }
                    gameObj.transform.parent = transform;
                    gameObj.transform.localPosition = new Vector3(30 * i, 0);
                    gameObj.transform.localScale = Vector3.one;
                    gameObj.SetActive(true);
                    unitList.Add(gameObj);
                }
            } else if (this.num > num) {
                for (var i = this.num - 1; i >= num; i--) {
                    gameObj = unitList[i].gameObject;
                    gameObj.transform.parent = Recycle;
                    gameObj.SetActive(false);
                    unitList.RemoveAt(i);
                    pool.Add(gameObj);
                }
            }
            this.num = num;
            ResetPos();
        }

        private void ResetPos() {
            var width = 30 * (num - 1) + unit.width;
            var y = transform.localPosition.y;
            transform.localPosition = new Vector3((Parent.width - width) / 2f, y);
        }

        public int selectedIndex {
            set {
                if (pool == null) return;
                _selectedIndex = value;
                for (var i = 0; i < num; i++) {
                    var go = unitList[i];
                    go.transform.GetChild(0).gameObject.SetActive(i == value);
                }
            }
            get { return _selectedIndex; }
        }
    }
}
