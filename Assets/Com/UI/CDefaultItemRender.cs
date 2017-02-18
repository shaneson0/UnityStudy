using Assets.Scripts.Com.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class CDefaultItemRender : CItemRender {
        protected UILabel Label;
        protected BoxCollider box;
        protected UIWidget uiw;
        public CDefaultItemRender() {
            SetGO(new GameObject());
            go.layer = 5;
            uiw = AddComponent<UIWidget>();
            uiw.pivot = UIWidget.Pivot.TopLeft;
            uiw.autoResizeBoxCollider = true;
            box = AddComponent<BoxCollider>();
            Label = UICreater.CreateLabel(null, 0, 0, 158, 12, tran, new Color(1f, 229 / 255f, 178 / 255f), 12, FontStyle.Bold);
            Label.autoResizeBoxCollider = true;
            Label.alignment = NGUIText.Alignment.Center;
            Label.pivot = UIWidget.Pivot.TopLeft;

        }

        public override void SetCondition<T>(T condition) {
            List<object> list = condition as List<object>;
            if (list == null) {
                return;
            }
            Label.overflowMethod = (UILabel.Overflow)Convert.ToInt32(list[0]);
            Label.alignment = (NGUIText.Alignment)Convert.ToInt32(list[1]);
            Label.fontSize = Convert.ToInt32(list[2]);
            if (list.Count >= 4) {
                Label.color = (Color)list[3];
            }
        }
        public override object Data {
            set {
                base.Data = value;
                Label.text = value.ToString();
                if (box != null) NGUITools.UpdateWidgetCollider(box, true);
            }
            get { return base.Data; }
        }

        public override int width {
            get { return uiw.width; }
            set {
                uiw.width = value;
                Label.width = Convert.ToInt32(value / Label.transform.localScale.x);
            }
        }

        public override int height {
            get { return uiw.height; }
            set {
                uiw.height = value;
                Label.height = Convert.ToInt32(value / Label.transform.localScale.y);
            }
        }

        public UILabel label {
            get { return Label; }
        }
    }
}