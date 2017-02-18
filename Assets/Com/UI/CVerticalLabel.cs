using UnityEngine;
using System.Collections;
using Assets.Scripts.Com.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Com.MingUI{
    public class CVerticalLabel : UISprite{
        public bool isLeftToRight = true; //是否从左往右
        public string[] msgItems;
        public float columeDistance = 3; //列距
        private List<UILabel> lbls = new List<UILabel>();

        /// <summary>
        /// 设置竖排label的内容
        /// </summary>
        /// <param name="msg"></param>
        public void SetText(string msg){
            for (int i = 0; i < lbls.Count; i++){
                GameObject.Destroy(lbls[i].gameObject, 0.1f);
            }
            lbls.Clear();
            msgItems = msg.Split('#');
            for (int i = 0; i < msgItems.Length; i++){
                UILabel lbl = UICreater.CreateLabel(msgItems[i], 0, 0, 12, 22, this.transform, Color.grey);
                lbl.overflowMethod = UILabel.Overflow.ResizeHeight;
                lbl.spacingY = 2;
                lbl.transform.localPosition = (isLeftToRight ? Vector3.right*(lbl.fontSize + columeDistance)*i : Vector3.right*(msgItems.Length - i));
                lbls.Add(lbl);
            }
        }

        /// <summary>
        /// 获取第n列的内容
        /// </summary>
        /// <param name="colume"></param>
        public string GetText(int colume){
            if (lbls.Count > colume && colume >= 0)
                return lbls[colume].text;
            else
                return null;
        }

        /// <summary>
        /// 获取UIlabel们，可用于添加linkID
        /// </summary>
        /// <returns></returns>
        public List<UILabel> GetLbls(){
            return lbls;
        }
    }
}