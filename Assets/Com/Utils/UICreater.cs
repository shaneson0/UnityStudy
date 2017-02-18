using Assets.Scripts.Com.MingUI;
using Assets.Scripts.Com.Utils;
using MingUI.Com.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Com.UI{
    public class UICreater{
        public const string UI_ROOT = "MingUI/Prefabs/UI Root.prefab";
        public const string BUTTON_URL = "MingUI/Prefabs/Button.prefab";
        public const string CLOUD_BUTTON_URL = "MingUI/Prefabs/CloudView.prefab";
        public const string LABEL_URL = "MingUI/Prefabs/Label.prefab";
        public const string NUMBERSTEPER_URL = "MingUI/Prefabs/NumberSteper.prefab";
        public const string NUMBER_STEPER_2 = "MingUI/Prefabs/NumberSteper2.prefab";
        public const string IMAGE_PATH = "MingUI/Prefabs/CImage.prefab";
        public const string SPRITE_PAHT = "MingUI/Prefabs/Sprite.prefab";
        public const string WIDGET_PAHT = "MingUI/Prefabs/WidgetObject.prefab";
        public const string CHECKBOX_PATH = "MingUI/Prefabs/CheckBox.prefab";
        public const string PROGRESSBAR_PATH = "MingUI/Prefabs/ProgressBar.prefab";
        public const string TILELIST_PATH = "MingUI/Prefabs/TileList.prefab";
        public const string MISSION_LIST_PATH = "MingUI/Prefabs/MissionList.prefab";
        public const string COMBOBOX_PATH = "MingUI/Prefabs/Combobox.prefab";
        public const string UI_MASK_PATH = "MingUI/Prefabs/UIMask.prefab";
        public const string TABBAR_PATH = "MingUI/Prefabs/TabBar.prefab";
        public const string TAB_BUTTON_PATH = "MingUI/Prefabs/TabButton.prefab";
        public const string PAGEBAR = "MingUI/Prefabs/PageBar.prefab";
        private const int LABEL_DEFAULT_DEPTH = 200;

        public static GameObject CreateUIRoot() {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(UI_ROOT)) as GameObject;
            return obj;
        }


        public static CButton CreateButton(string text, int depth, int x, int y, Transform parent, int w = 0, int h = 0, UIEventListener.VoidDelegate OnClick = null, string name = "Button", string toolTip = null){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(BUTTON_URL)) as GameObject;
            if (obj == null) return null;
            obj.name = name;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CButton btn = obj.GetComponent<CButton>();
            btn.Text = text ?? "";
            UISprite sp = btn.GetComponent<UISprite>();
            btn.Label.SetAnchor(btn.transform);
            btn.Label.leftAnchor.Set(0, -38);
            btn.Label.rightAnchor.Set(1, 38);
            btn.Label.topAnchor.Set(1, 32);
            btn.Label.bottomAnchor.Set(0, -40);
            sp.depth = depth;
            if (w > 0 || h > 0){
                sp.width = w;
                sp.height = h;
            }
            if (toolTip != null){
                btn.AddToolTip(toolTip);
            }
            btn.Label.depth = 200; //整个界面的文字基本不会重叠，搞成同一层避免隔开具有相同图集的其他组件的深度
            if (OnClick != null){
                //UIEventListener.Get(obj).onClick = OnClick;
                btn.AddClick(OnClick);
            }
            return btn;
        }

        public static CButton CreateImgButton(string path,string text, int depth, int x, int y, Transform parent, int w = 0, int h = 0, UIEventListener.VoidDelegate OnClick = null, string name = "Button", string toolTip = null) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(path)) as GameObject;
            if (obj == null) return null;
            obj.name = name;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CButton btn = obj.GetComponent<CButton>();
            UISprite sp = btn.GetComponent<UISprite>();
            sp.depth = depth;
            if (w > 0 || h > 0) {
                sp.width = w;
                sp.height = h;
            }
            if (toolTip != null) {
                btn.AddToolTip(toolTip);
            }
            if (OnClick != null) {
                UIEventListener.Get(obj).onClick = OnClick;
            }
            return btn;
        }

        public static CButton CreateCloud(int depth, int x, int y, Transform parent, int w = 0, int h = 0, UIEventListener.VoidDelegate OnClick = null, string name = "Button", string toolTip = null) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(CLOUD_BUTTON_URL)) as GameObject;
            if (obj == null) return null;
            obj.name = name;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CButton btn = obj.GetComponent<CButton>();
            UISprite sp = obj.GetComponent<UISprite>();
            sp.depth = depth;
            if (w > 0 || h > 0) {
                sp.width = w;
                sp.height = h;
            }
            if (toolTip != null) {
                btn.AddToolTip(toolTip);
            }
            if (OnClick != null) {
                UIEventListener.Get(obj).onClick = OnClick;
            }
            return btn;
        }

        public static UISprite CreateMask(int x, int y, Transform parent = null, int width = 0, int height = 0, int depth = 0) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(UI_MASK_PATH)) as GameObject;
            obj.transform.parent = parent;
            UIPanel panel = obj.GetComponent<UIPanel>();
            panel.depth = depth;
            UISprite sprite = obj.GetComponentInChildren<UISprite>();
            sprite.width = width;
            sprite.height = height;
            sprite.transform.localPosition=new Vector3(x, y, 0);
            sprite.transform.localScale = Vector3.one;
            return sprite;
        }


        public static UILabel CreateLabel(string text, int x, int y, int w, int h, Transform parent, Color color, int size = 12, FontStyle style = FontStyle.Bold, NGUIText.Alignment align = NGUIText.Alignment.Left, int depth = LABEL_DEFAULT_DEPTH, UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft,int zRotate = 0){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(LABEL_URL)) as GameObject;
            if (obj == null) return null;
            UILabel label = obj.GetComponent<UILabel>();
            label.text = text ?? "";
            label.width = w;
            label.height = h;
            label.fontSize = size;
            label.fontStyle = style;
            label.pivot = pivot;
            label.alignment = align;
            if (color == Color.black) color = new Color(0, 1, 1);
            label.color = color;
            label.depth = depth;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one * size/20.0f;
            obj.transform.localPosition = new Vector3(x, y, 0);
            obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRotate));
            return label;
        }

        public static UISprite CreateSprite(string atlasPath, string spriteName, int x, int y, Transform parent = null, int width = 0, int height = 0,int depth = 0){
            if (FuncUtil.GetUIAssetByPath(SPRITE_PAHT)==null) {
                return null;
            }
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(SPRITE_PAHT)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            UISprite sp = obj.GetComponent<UISprite>();
            sp.transform.localPosition = new Vector3(x, y, 0);
            sp.transform.localScale = Vector3.one;
            if (width == 0 || height == 0){
                sp.MakePixelPerfect();
            }
            else{
                sp.width = width;
                sp.height = height;
            }
            object atlasObj = FuncUtil.GetUIAssetByPath(atlasPath, typeof (UIAtlas));
            UIAtlas atlas = (atlasObj is GameObject) ? (atlasObj as GameObject).GetComponent<UIAtlas>() : atlasObj as UIAtlas;
            sp.atlas = atlas;
            sp.spriteName = spriteName;
            sp.depth = depth;
            return sp;
        }

        public static UISprite CreateSpriteClickable(string atlasPath, string spriteName, int x, int y, Transform parent = null, int width = 0, int height = 0, int depth = 0) {
            UISprite sp = CreateSprite(atlasPath, spriteName, x, y, parent, width, height, depth);
            sp.gameObject.AddComponent<BoxCollider>().size = new Vector3(width,height,1);

            return sp;
        }

        public static UISprite CreateSprite(string name, UIAtlas atlas, string spriteName, int x, int y, Transform parent = null, int width = 0, int height = 0, int depth = 8, UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft){
            if (FuncUtil.GetUIAssetByPath(SPRITE_PAHT) == null) {
                return null;
            }
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(SPRITE_PAHT)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            UISprite sprite = obj.GetComponent<UISprite>();
            UISpriteData spData = (atlas != null) ? atlas.GetSprite(spriteName) : null;
            sprite.type = (spData == null || !spData.hasBorder) ? UISprite.Type.Simple : UISprite.Type.Sliced;
            sprite.atlas = atlas;
            sprite.spriteName = spriteName;
            sprite.pivot = pivot;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            if (width == 0 || height == 0){
                sprite.MakePixelPerfect();
            }
            else{
                sprite.width = width;
                sprite.height = height;
            }
            sprite.depth = depth;
            return sprite;
        }

        public static CNumStepper CreateNumStepper(int x, int y, Transform parent){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(NUMBERSTEPER_URL)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CNumStepper step = obj.GetComponent<CNumStepper>();
            return step;
        }

        public static CNumStepper CreateNumStepper2(int x, int y, Transform parent){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(NUMBER_STEPER_2)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CNumStepper step = obj.GetComponent<CNumStepper>();
            return step;
        }

        public static Image CreateImage(int x, int y, Transform parent){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(IMAGE_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            Image asset = obj.GetComponent<Image>();
            return asset;
        }

        public static UIWidget CreateWidget(){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(WIDGET_PAHT)) as GameObject;
            if (obj == null) return null;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            UIWidget asset = obj.GetComponent<UIWidget>();
            return asset;
        }

        public static CCheckBox CreateCheckBox(string name, int x, int y, Transform parent, int width = 0, int height = 0){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(CHECKBOX_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CCheckBox asset = obj.GetComponent<CCheckBox>();
            asset.Selected = false;
            if (width > 0){
                asset.Label.width = width;
            }
            if (height > 0){
                asset.Label.height = height;
            }
            asset.Text = name;
            return asset;
        }

        public static CProgressBar CreateProgressBar(string name, int x, int y, Transform parent, int w, int h, int depth){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(PROGRESSBAR_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CProgressBar bar = obj.GetComponent<CProgressBar>();
            UISprite sprite = bar.GetComponent<UISprite>();
            sprite.width = w;
            sprite.height = h;
            sprite.depth = depth;
            UISprite overlay = bar.transform.Find("Overlay").GetComponent<UISprite>();
            overlay.depth = depth + 1;
            return bar;
        }

        public static CGrid CreateGrid(int x, int y, Transform parent, int ColNum, Type itemRender, bool ByItemSize = false){
            var widgetGrid = CreateWidget();
            widgetGrid.transform.parent = parent;
            widgetGrid.transform.localPosition = new Vector3(x, y, 0);
            var grid = widgetGrid.gameObject.AddComponent<CGrid>();
            grid.ColNum = ColNum;
            grid.itemRender = itemRender;
            grid.ByItemSize = ByItemSize;
            return grid;
        }

        //public static CTileList CreateTileList(string name, int x, int y, Transform parent, int w, int h, int depth){
        //    GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(TILELIST_PATH)) as GameObject;
        //    if (obj == null) return null;
        //    obj.transform.parent = parent;
        //    obj.transform.localScale = Vector3.one;
        //    obj.transform.localPosition = new Vector3(x, y, 0);
        //    CTileList list = obj.GetComponent<CTileList>();
        //    return list;
        //}

        public static CMissionList CreateMissionList(string name, int x, int y, Transform parent, int w, int h, int depth) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(MISSION_LIST_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CMissionList list = obj.GetComponent<CMissionList>();
            return list;
        }

        public static CCombobox CreateCombobox(string name, int x, int y, Transform parent, int w, int h, int depth){
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(COMBOBOX_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CCombobox box = obj.GetComponent<CCombobox>();
            return box;
        }

        public static CTabBar CreateTabBar(int x, int y, Transform parent, string[] items, int itemW, int itemH, int depth,int offsetX = 0) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(TABBAR_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CTabBar box = obj.GetComponent<CTabBar>();
            var defaultItem = DisplayUtil.GetChildByName(obj.transform, "Tab0");
            if (defaultItem != null) {
                GameObject.DestroyImmediate(defaultItem.gameObject);
            }
            box.RemoveItems();
            for (var i = 0; i < items.Length; i++) {
                var btn = CreateTabBarBtn(i * itemW + offsetX, 0, obj.transform, items[i], itemW, itemH, depth);
                btn.name = "Tab" + i;
            }
            box.Start();
            return box;
        }

        public static CButtonToggle CreateTabBarBtn(int x, int y, Transform parent, string name, int w, int h, int depth) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(TAB_BUTTON_PATH)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CButtonToggle box = obj.GetComponent<CButtonToggle>();
            box.width = w;
            box.height = h;
            box.Text = name;
            return box;
        }
		//渐变的label
		public static UILabel CreateGradLabel(string text, int x, int y, int w, int h, Transform parent, Color color, Color topColor, Color bottomColor, Color effectColor, int size = 12, FontStyle style = FontStyle.Bold, NGUIText.Alignment align = NGUIText.Alignment.Left, int depth = 13, UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft) {
			GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(LABEL_URL)) as GameObject;
			if (obj == null) return null;
			UILabel label = obj.GetComponent<UILabel>();
			label.text = text ?? "";
			label.width = w;
			label.height = h;
			label.fontSize = size;
			label.fontStyle = style;
			label.pivot = pivot;
			label.alignment = align;
			label.applyGradient = true;
			label.gradientTop = topColor;
			label.gradientBottom = bottomColor;
			label.effectColor = effectColor;
			label.effectStyle = UILabel.Effect.Outline;
			if (color == Color.black) color = new Color(0, 1, 1);
			label.color = color;
			label.depth = depth;
			obj.transform.parent = parent;
			obj.transform.localScale = Vector3.one * size / 20.0f;
			obj.transform.localPosition = new Vector3(x, y, 0);
			return label;
		}

        public static CPageBar CreatePageBar(int x, int y, Transform parent, int min,int max, int depth) {
            GameObject obj = UnityEngine.Object.Instantiate(FuncUtil.GetUIAssetByPath(PAGEBAR)) as GameObject;
            if (obj == null) return null;
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(x, y, 0);
            CPageBar bar = obj.GetComponent<CPageBar>();
            bar.Min = min;
            bar.Max = max;
            bar.depth = depth;
            return bar;
        }
    }
}