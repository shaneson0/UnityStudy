using MingUI.Com.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Com.MingUI {
    public class Image : UITexture {
        private static string UI_MASK = "Materials/UI_Mash.mat";

        public Action completeAction;
        public FilterMode filterMode = FilterMode.Point;
        private string _url;
        private string _lastURL;
        private string _defaultUrl;
        private string _maskPath;
        private int _imageWidth;
        private int _imageHeight;

        private bool isMask;
        private Texture2D curTexture;
        private bool isClear;
        private bool hasMainTex;
        public bool materialMode;


        protected override void OnInit() {
            base.OnInit();
            FuncUtil.InitImage(this);
        }

        public string defaultUrl {
            set {
                _defaultUrl = value;
            }
            get {
                return _defaultUrl;
            }
        }

        private void UpdateDefaultTexture() {
            if (string.IsNullOrEmpty(_defaultUrl)) return;
            if (_url == _defaultUrl || mainTexture != null) return;
            FuncUtil.Load(_defaultUrl, (Action<String>)OnLoadTextureComplete, _defaultUrl);
        }

        public string url {
            set {
                if (!string.IsNullOrEmpty(_url) && _url == value) return;
                _lastURL = _url;
                _url = value;
                if (_url == _defaultUrl) _defaultUrl="";
                if (string.IsNullOrEmpty(_url)) {
                    mainTexture = null;
                    FuncUtil.CanDispose(_lastURL);
                    return;
                }
                isClear = false;
                UpdateDefaultTexture();
                //defaultUrl = _defaultUrl;
                //var texture = ResourceMgr.GetAssetBundleAsset(value);
                //if (texture == null) {
                //MassLoader.FuncUtil.Load(_url, OnLoadTextureComplete);
                //    return;
                //}
                //OnLoadTextureComplete(texture);
                FuncUtil.Load(_url, (Action<String>)OnLoadTextureComplete, _url);

            }

            get {
                return _url;
            }
        }

        public int imageWidth {
            set {
                _imageWidth = value;
                width = value;
            }
            get {
                return _imageWidth;
            }
        }

        public int imageHeight {
            set {
                _imageHeight = value;
                height = value;
            }
            get {
                return _imageHeight;
            }
        }

        private void OnLoadTextureComplete(string url) {
            //如果是默认图  并且实际上用的图已经加载好
            if (url == defaultUrl && (curTexture != null ||string.IsNullOrEmpty(_url))) {
                return;
            }
            //不是当前需求要的图
            if (url != defaultUrl && url != _url) {
                return;
            }
            isClear = false;
            try {
              curTexture = FuncUtil.GetUIAssetByPath(url) as Texture2D;
            } catch (Exception e){
                FuncUtil.ShowError("Image加载完毕后获取Texture2D失败：{0}", e.Message);
                return;
            }
            if (curTexture == null) {
                Clear();
                FuncUtil.ShowError("Image加载完毕后获取Texture2D失败：{0}", url);
            }
            FuncUtil.NoDispose(_url);
            UpdateBaseTexture();
            if (_imageWidth != 0 && _imageHeight != 0) {
                width = Convert.ToInt32(_imageWidth);
                height = Convert.ToInt32(_imageHeight);
            }
            OnCompleteCallBack();
        }

        public void SetMask(string path) {
            isMask = true;
            _maskPath = path;
            FuncUtil.Load(UI_MASK, (Action<string>)OnLoadMaskComplete, UI_MASK);

        }

        private void OnLoadMaskCullingComplete(string url) {
            var ttCulling = FuncUtil.GetUIAssetByPath(url) as Texture2D;
            if (material != null) {
                material.SetTexture("_Mask", ttCulling);
            }
            UpdateBaseTexture();
        }

        private void OnLoadMaskComplete(string path) {
            material = new Material(FuncUtil.GetUIAssetByPath(path) as Material);
            material.color = color;
            FuncUtil.Load(_maskPath + "", (Action<string>)OnLoadMaskCullingComplete, _maskPath);
        }

        private int loadFlag;
        private void UpdateBaseTexture() {
            if (hasDestroy || isClear) {//已经清除过的
                return;
            }
            if (isMask == true) {
                if (string.IsNullOrEmpty(_maskPath) == false) {
                    if (material == null && loadFlag < 2) {     //如果材质一直加载不出，可能会死循环，做个标记
                        loadFlag++;
                        FuncUtil.Load(UI_MASK, (Action<string>)OnLoadMaskComplete, UI_MASK);
                        return;
                    }
                    loadFlag = 0;
                    if (material != null && curTexture != material.GetTexture("_MainTex")) {
                        RemoveFromPanel();
                        material.SetTexture("_MainTex", curTexture);
                        hasMainTex = curTexture != null;
                        MarkAsChanged();
                        
                    }
                }
                mainTexture = null;
            } else {
                mainTexture = curTexture;
                if(materialMode == false) material = null;
                if (_imageWidth != 0 && _imageHeight != 0) {
                    width = Convert.ToInt32(_imageWidth);
                    height = Convert.ToInt32(_imageHeight);
                } else {
                    MakePixelPerfect();
                }
            }
            FuncUtil.CanDispose(_lastURL);
        }

        private void OnCompleteCallBack() {
            if (completeAction != null) {
                completeAction.DynamicInvoke();
            }
        }

        public void ClearTTOnly() {
            if ((isMask && material != null && material.GetTexture("_MainTex") != null) || mainTexture != null) {
                    FuncUtil.CanDispose(_url);
            } 
            mainTexture = null;
            curTexture = null;
            if (material != null && material.GetTexture("_MainTex") != null) {
                material.SetTexture("_MainTex", curTexture);
                hasMainTex = curTexture != null;
                MarkAsChanged();
            }
        }

        public bool IsClearTT() {
            return isMask ? (material == null || hasMainTex == false) : mainTexture == null;
        }


        public void Clear() {
            mainTexture = null;
            curTexture = null;
            material = null;
            _url = null;
            isClear = true;
        }

        new public Color color {
            get {
                return base.color;
            }
            set {
                base.color = value;
                if (material != null) {
                    material.SetColor("_Color", value);
                    //material.color = value;
                } else {
                }
            }
        }

    }
}
