﻿//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright (c) 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : IgnoreTimeScale
{
	public Vector3 target = Vector3.zero;
	public float strength = 10f;

	UIPanel mPanel;
	Transform mTrans;
	float mThreshold = 0f;
	UIDraggablePanel mDrag;

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Start ()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIDraggablePanel>();
		mTrans = transform;
	}

	/// <summary>
	/// Advance toward the target position.
	/// </summary>

	void Update ()
	{
		float delta = UpdateRealTimeDelta();

		if (mThreshold == 0f) mThreshold = (target - mTrans.localPosition).magnitude * 0.005f;

		Vector3 before = mTrans.localPosition;
		mTrans.localPosition = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, delta);

		Vector3 offset = mTrans.localPosition - before;
		Vector4 cr = mPanel.clipRange;
		cr.x -= offset.x;
		cr.y -= offset.y;
		mPanel.clipRange = cr;

		if (mDrag != null) mDrag.UpdateScrollbars(false);
		if (mThreshold >= (target - mTrans.localPosition).magnitude) enabled = false;
	}

	/// <summary>
	/// Start the tweening process.
	/// </summary>

	static public SpringPanel Begin (GameObject go, Vector3 pos, float strength)
	{
		SpringPanel sp = go.GetComponent<SpringPanel>();
		if (sp == null) sp = go.AddComponent<SpringPanel>();
		sp.target = pos;
		sp.strength = strength;

		if (!sp.enabled)
		{
			sp.mThreshold = 0f;
			sp.enabled = true;
		}
		return sp;
	}
}