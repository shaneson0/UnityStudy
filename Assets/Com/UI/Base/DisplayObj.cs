using System.Runtime.CompilerServices;
using Assets.Scripts.Com.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
using MingUI.Com.Utils;

namespace Assets.Scripts.Com.UI.Base{
    public class DisplayObj : Obj{
        private string _name;
        protected UIWidget _widget;
        protected DisplayObj instance;

        public string name{
            set{
                _name = value;
                if (go != null){
                    go.name = _name;
                }
            }
            get { return _name; }
        }

        public GameObject go { get; private set; }

        public Transform tran { get; private set; }

        public DisplayObj(){
        }

        public DisplayObj(string goName, bool createGO = true){
            name = goName;
            if (createGO == true){
                go = new GameObject(name);
                instance = this;
                go.SetDisplayObj(ref instance);
                tran = go.transform;
            }
        }

        public virtual void SetGO(GameObject gameObject){
            if (go == null){
                go = gameObject;
                instance = this;
                go.SetDisplayObj(ref instance);
                tran = go.transform;
            }
            else{
                Debug.LogError("DisplayObj's GameObject is not null!");
            }
        }

        public void SetObjAndInstantiate(UnityEngine.Object obj){
            if (obj == null){
                Debug.Log("没有这个对象" + this.GetType().ToString());
                return;
            }
            
            SetGO(Object.Instantiate(obj) as GameObject);
            FuncUtil.AddUIDep(obj, go);
        }


        public void SetParent(Transform parent){
            if (tran != null){
                tran.parent = parent;
                tran.localScale = Vector3.one;
                UIWidget[] list = go.GetComponentsInChildren<UIWidget>(true);
                foreach (UIWidget w in list){
                    w.ParentHasChanged();
                }
            }
        }

        public virtual void SetParent(GameObject parent){
            if (parent == null) throw new ArgumentNullException("parent");
            if (tran != null){
                tran.parent = parent.transform;
                tran.localScale = Vector3.one;
                UIWidget[] list = go.GetComponentsInChildren<UIWidget>(true);
                foreach (UIWidget w in list){
                    w.ParentHasChanged();
                }
                if (parent.GetDisplayObj() != null){
                    alpha = parent.GetDisplayObj().alpha;
                }
            }
        }

        public Transform parent{
            get { return tran.parent; }
        }

        public Transform GetChildByName(string name){
            return DisplayUtil.GetChildByName(tran, name);
        }

        public T GetChildComponentByName<T>(string name) where T : Component{
            Transform tran = GetChildByName(name);
            if (tran != null){
                return tran.GetComponent<T>();
            }
            else{
                return null;
            }
        }

        public virtual MeshCollider AddMeshCollider(GameObject tarGO){
            tarGO = tarGO ?? go;
            var collider = tarGO.GetComponent<MeshCollider>();
            if (collider == null){
                collider = tarGO.AddComponent<MeshCollider>();
            }
            return collider;
        }

        public void AddMeshCollider(){
            if (go == null){
                return;
            }
            Renderer[] list = go.GetComponentsInChildren<Renderer>(true);
            if (list != null){
                foreach (Renderer render in list){
                    GameObject rendeGO = render.gameObject;
                    var collider = rendeGO.GetComponent<CapsuleCollider>();
                    if (collider == null){
                        collider = rendeGO.AddComponent<CapsuleCollider>();
                    }
                    collider.radius = 0.4f;
                    collider.height = 2;
                }
            }
        }

        public virtual void Remove(){
            Object.Destroy(go);
        }

        public virtual void SetActive(bool isAct){
            if (go != null){
                go.SetActive(isAct);
            }
        }

        public bool IsRemove(){
            return go == null;
        }

        public bool IsCreate(){
            return go != null;
        }

        public Vector3 forward{
            get { return go.transform.forward; }
        }

        public Vector3 position{
            get { return tran.position; }
            set { tran.position = value; }
        }

        public Vector3 localPosition{
            get { return tran.localPosition; }
            set{
                tran.localPosition = value;
            }
        }
         
        public float x{
            set{
                Vector3 v = tran.localPosition;
                v.x = value;
                tran.localPosition = v;
            }
            get { return tran.localPosition.x; }
        }

        public float y{
            set{
                Vector3 v = tran.localPosition;
                v.y = value;
                tran.localPosition = v;
            }
            get { return tran.localPosition.y; }
        }

        public float z{
            set{
                Vector3 v = tran.localPosition;
                v.z = value;
                tran.localPosition = v;
            }
            get { return tran.localPosition.z; }
        }

        public Quaternion rotation{
            set { go.transform.rotation = value; }
            get { return go.transform.rotation; }
        }

        public Vector3 localEulerAngles{
            set { tran.localEulerAngles = value; }
            get { return tran.localEulerAngles; }
        }

        public Quaternion localRotation{
            set { tran.localRotation = value; }
            get { return tran.localRotation; }
        }

        public float angleY{
            get { return rotation.eulerAngles.y; }
        }

        public Vector3 localScale{
            get { return tran.localScale; }
            set { tran.localScale = value; }
        }

        private float _alpha = -1;

        public float alpha{
            set{
                _alpha = value;
                Renderer[] tarList = go.GetComponentsInChildren<Renderer>();
                if (tarList != null){
                    foreach (Renderer render in tarList){
                        Material mat = render.material;
                        if (mat.HasProperty("_Color") == true){
                            Color color = mat.color;
                            color.a = value;
                            mat.color = color;
                            render.material = mat;
                        }
                    }
                }
            }
            get{
                if (_alpha != -1){
                    return _alpha;
                }
                return 1;
            }
        }

        public void Original(){
            localScale = Vector3.one;
            localPosition = Vector3.zero;
            localEulerAngles = Vector3.zero;
        }

        public virtual int width { get; set; } //UI才用到这2个

        public virtual int height { get; set; }

        protected UIWidget widget{
            get{
                if (_widget == null){

                    _widget =  GetChildComponentByName<UIWidget>(go.name) ?? AddComponent<UIWidget>();
                }
                return _widget;
            }
            set { _widget = value; }
        }

        public Component AddComponent(string componentName){
            return UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, "Assets/Scripts/Com/UI/Base/DisplayObj.cs (264,20)", componentName);
        }

        public T AddComponent<T>() where T : Component{
            return go.AddComponent<T>();
        }

        public Component GetComponent(string componentName){
            return go.GetComponent(componentName);
        }

        public T GetComponent<T>() where T : Component{
            return go.GetComponent<T>();
        }

        public bool IsActive(){
            return go != null && go.activeSelf;
        }

        public int layer{
            set{
                go.layer = value;
                Transform[] list = DisplayUtil.getChildList(tran);
                foreach (Transform item in list){
                    item.gameObject.layer = value;
                }
            }
            get { return go.layer; }
        }

        public void DontDestroyOnLoad(){
            Object.DontDestroyOnLoad(go);
        }

        public void Destroy() {
            Object.Destroy(go);
        }

        public DisplayObj Instantiate(Vector3 goPos, Quaternion goRotation){
            DisplayObj obj = new DisplayObj(name + "(copy)");
            obj.SetObjAndInstantiate(go);
            obj.localPosition = goPos;
            obj.localRotation = goRotation;
            return obj;
        }

        public int GetID(){
            return go.GetInstanceID();
        }

        public void SetOutsideScreen() {
            tran.position = new Vector3(99999, 99999);
        }

        //setActive为true触发
        public virtual void OnEnable(){
//            Debug.Log("OnEnable:" + this.name);
        }
        //setActive为false触发
        public virtual void OnDisable(){
//            Debug.Log("OnDisable:" + this.name);
        }
        //进入视野时不会触发，基本没用，需挂载有render脚本的物体上
        public virtual void OnBecameVisible(){
//            Debug.Log("OnBecameVisible:" + this.name);
        }

        public virtual void OnBecameInvisible(){
//            Debug.Log("OnBecameInvisible:" + this.name);
        }
    }
}