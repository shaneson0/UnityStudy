using MingUI.Com.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Assets.NGUI.Scripts.Editor {
    [CustomEditor(typeof(CMultiTree))]
    class CMultiTreeInspector : CCanvasInspector {
        protected override void DrawCustomProperties() {
            NGUIEditorTools.DrawProperty("Overlay", serializedObject, "Overlay");
            NGUIEditorTools.DrawProperty("spSelected", serializedObject, "spSelected");
            NGUIEditorTools.DrawProperty("Recycle", serializedObject, "Recycle");
            base.DrawCustomProperties();
        }
    }
}
