//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using MingUI.Com.Utils;
using System;
using UnityEngine;

/// <summary>
/// Event Hook class lets you easily add remote event listener functions to an object.
/// Example usage: UIEventListener.Get(gameObject).onClick += MyClickFunction;
/// </summary>

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	public delegate void VoidDelegate (GameObject go);
	public delegate void BoolDelegate (GameObject go, bool state);
	public delegate void FloatDelegate (GameObject go, float delta);
	public delegate void VectorDelegate (GameObject go, Vector2 delta);
	public delegate void ObjectDelegate (GameObject go, GameObject obj);
	public delegate void KeyCodeDelegate (GameObject go, KeyCode key);

	public object parameter;

	public VoidDelegate onSubmit;
	public VoidDelegate onClick;
	public VoidDelegate onDoubleClick;
	public BoolDelegate onHover;
	public BoolDelegate onPress;
	public BoolDelegate onSelect;
	public FloatDelegate onScroll;
	public VoidDelegate onDragStart;
	public VectorDelegate onDrag;
	public VoidDelegate onDragOver;
	public VoidDelegate onDragOut;
	public VoidDelegate onDragEnd;
	public ObjectDelegate onDrop;
	public KeyCodeDelegate onKey;
	public BoolDelegate onTooltip;
    public VoidDelegate onOver;
    public VoidDelegate onOut;
    public VoidDelegate onMouseOver;
    public VoidDelegate onMouseUp;

	bool isColliderEnabled
	{
		get
		{
            return true;
			Collider c = GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
	}

	void OnSubmit ()				{ if (isColliderEnabled && onSubmit != null) DoVoidDelegate( onSubmit,gameObject); }
	void OnClick ()					{ if (isColliderEnabled && onClick != null) DoVoidDelegate(onClick,gameObject); }
    void OnDoubleClick()            { if (isColliderEnabled && onDoubleClick != null) DoVoidDelegate(onDoubleClick, gameObject); }
	void OnHover (bool isOver)		{ if (isColliderEnabled && onHover != null) DoBoolDelegate( onHover,gameObject, isOver); }
    void OnPress(bool isPressed)    { if (isColliderEnabled && onPress != null) DoBoolDelegate(onPress,gameObject, isPressed); }
    void OnSelect(bool selected)    { if (isColliderEnabled && onSelect != null) DoBoolDelegate(onSelect,gameObject, selected); }
	void OnScroll (float delta)		{ if (onScroll != null) DoFloatDelegate( onScroll,gameObject, delta); }
    void OnDragStart()              { if (onDragStart != null) DoVoidDelegate(onDragStart,gameObject); }
	void OnDrag (Vector2 delta)		{ if (onDrag != null) DoVectorDelegate( onDrag,gameObject, delta); }
	void OnDragOver ()				{ if (isColliderEnabled && onDragOver != null) DoVoidDelegate( onDragOver,gameObject); }
	void OnDragOut ()				{ if (isColliderEnabled && onDragOut != null) DoVoidDelegate( onDragOut,gameObject); }
	void OnDragEnd ()				{ if (onDragEnd != null) DoVoidDelegate( onDragEnd,gameObject); }
	void OnDrop (GameObject go)		{ if (isColliderEnabled && onDrop != null) DoObjectDelegate( onDrop,gameObject, go); }
	void OnKey (KeyCode key)		{ if (isColliderEnabled && onKey != null) DoKeyCodeDelegate( onKey,gameObject, key); }
	void OnTooltip (bool show)		{ if (isColliderEnabled && onTooltip != null) DoBoolDelegate( onTooltip,gameObject, show); }

    void OnOver() { if (onOver != null) DoVoidDelegate( onOver,gameObject); }
    void OnOut() { if (onOut != null) DoVoidDelegate( onOut,gameObject); }

    void OnMouseUp() { if (onMouseUp != null)DoVoidDelegate( onMouseUp,gameObject); }
	/// <summary>
	/// Get or add an event listener to the specified game object.
	/// </summary>

	static public UIEventListener Get (GameObject go)
	{
		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}

    public void DoClick() {
        OnClick();
    }
    public static void DoVoidDelegate(VoidDelegate func, GameObject arg) {
        try {
            func(arg);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }

    public static void DoBoolDelegate(BoolDelegate func, GameObject arg,bool state) {
        try {
            func(arg,state);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }


    public static void DoFloatDelegate(FloatDelegate func, GameObject arg,float delta) {
        try {
            func(arg,delta);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }

    public static void DoVectorDelegate(VectorDelegate func, GameObject arg, Vector2 delta) {
        try {
            func(arg, delta);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }

    public static void DoObjectDelegate(ObjectDelegate func, GameObject arg, GameObject obj) {
        try {
            func(arg, obj);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }

    public static void DoKeyCodeDelegate(KeyCodeDelegate func, GameObject arg, KeyCode key) {
        try {
            func(arg, key);
        } catch (Exception ex) {
            //TopTip.addTip(a.Method.Name + "执行出错" + ex.ToString());
            FuncUtil.ShowError(func.Method.Name + "执行出错" + ex.ToString());
        }
    }
}
