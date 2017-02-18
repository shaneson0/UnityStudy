using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class UIUtil {
        public static float UIRootScale {
            get {
                if (UIRoot.list.Count >= 0) {
                    return UIRoot.list[0].transform.localScale.x;
                }
                return 1;
            }
        }

        public static Vector3 ScaleInUIRoot(Transform transform) {
            Vector3 nowScale = transform.localScale;
            Transform nowTran = transform.parent;
            while (nowTran != null && nowTran != UIRoot.list[0].transform) {
                nowScale.x *= nowTran.localScale.x;
                nowScale.y *= nowTran.localScale.y;
                nowScale.z *= nowTran.localScale.z;
                nowTran = nowTran.parent;
            }
            return nowScale;
        }

        public static int GetTopY(Transform Content) {
            int topY = 0;
            UIWidget[] widgets = Content.GetComponentsInChildren<UIWidget>(true);
            float rootScale = 1;
            if (Content.IsChildOf(UIRoot.list[0].transform)) {
                rootScale = UIRootScale;
            }
            for (int i = 0; i < widgets.Length; i++) {
                UIWidget w = widgets[i];
                Vector3 pos = w.transform.position / rootScale - Content.position / rootScale;
                int newTop = (int)pos.y - (int)(w.height * w.transform.localScale.y);
                if (newTop > topY) {
                    topY = newTop;
                }
            }
            return topY;
        }

        public static int getBottomY(Transform Content) {
            int bottomY = 0;
            UIWidget[] widgets = Content.GetComponentsInChildren<UIWidget>(true);
            float rootScale = 1;
            if (Content.IsChildOf(UIRoot.list[0].transform)) {
                rootScale = UIRootScale;
            }
            for (int i = 0; i < widgets.Length; i++) {
                UIWidget w = widgets[i];
                Vector3 pos = w.transform.position / rootScale - Content.position / rootScale;
                //解决Label清晰度方案带来的缩放
                int newBottom = (int)pos.y - (int)(w.height *w.transform.localScale.x);
                if (newBottom < bottomY) {
                    bottomY = newBottom;
                }
            }
            return bottomY;
        }

        public static int getRightX(Transform Content) {
            int rightX = 0;
            UIWidget[] widgets = Content.GetComponentsInChildren<UIWidget>(true);
            float rootScale = 1;
            if (Content.IsChildOf(UIRoot.list[0].transform)) {
                rootScale = UIRootScale;
            }
            for (int i = 0; i < widgets.Length; i++) {
                UIWidget w = widgets[i];
                Vector3 pos = w.transform.position / rootScale - Content.position / rootScale;
                int newRight = (int)pos.x + w.width;
                if (newRight > rightX) {
                    rightX = newRight;
                }
            }
            return rightX;
        }

        public static int getTotalHeight(Transform Content) {
            float topY = float.MinValue;
            float bottomY = float.MaxValue;
            UIWidget[] widgets = Content.GetComponentsInChildren<UIWidget>(true);
            if (widgets.Length == 0) {
                return 0;
            }
            float rootScale = 1;
            if (Content.IsChildOf(UIRoot.list[0].transform)) {
                rootScale = UIRootScale;
            }
            for (int i = 0; i < widgets.Length; i++) {
                UIWidget w = widgets[i];
                Vector3 pos = w.transform.position / rootScale - Content.position / rootScale;
                if (pos.y > topY) {
                    topY = pos.y;
                }
                float newBottom = pos.y - w.height;
                if (newBottom < bottomY) {
                    bottomY = newBottom;
                }
            }
            return (int)(topY - bottomY);
        }

        public static Vector2 CenterToBottomLeft(Vector2 v) {
            v.x += Screen.width / 2;
            v.y += Screen.height / 2;
            return v;
        }

        public static Vector2 BottomLeftToCenter(Vector2 v) {
            v.x -= Screen.width / 2;
            v.y -= Screen.height / 2;
            return v;
        }
    }
}