using UnityEngine;
using System.Collections;
using GemMine.EasyEasing;

public class PickerTweens : MonoBehaviour {

	bool isRunning = false;

	float duration;

	public bool isTweenRunning(){
		return isRunning;
	}


	bool isSmall = true;

	public bool canZoomOut() {
		return isSmall;
	}


	bool isFull = true;

	public bool canShrink() {
		return isFull;
	}


	public void ZoomOut (bool showAnimation) {
		if (showAnimation)
			duration = 1f;
		else
			duration = 0f;
		if (isRunning)
			return;

		if (!isSmall)
			return;

		isRunning = true;

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 1f,
				"to", 4f,
				"onUpdate", "onUpdateZoomScale",
				"onUpdateTarget", this.gameObject,				
				"duration", duration));

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 1f,
				"to", 0f,
				"onUpdate", "onUpdateZoomAlpha",
				"onUpdateTarget", this.gameObject,				
				"onComplete", "onCompleteZoomAlphaOut",
				"onCompleteTarget", this.gameObject,
				"duration", duration));
	}



	public void ZoomIn (bool showAnimation) {
		if (showAnimation)
			duration = 1f;
		else
			duration = 0f;

		if (isRunning)
			return;

		if (isSmall)
			return;

		isRunning = true;

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 4f,
				"to", 1f,
				"onUpdate", "onUpdateZoomScale",
				"onUpdateTarget", this.gameObject,				
				"duration", duration));

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 0f,
				"to", 1f,
				"onUpdate", "onUpdateZoomAlpha",
				"onUpdateTarget", this.gameObject,				
				"onComplete", "onCompleteZoomAlphaIn",
				"onCompleteTarget", this.gameObject,
				"duration", duration));
	}


	public void onUpdateZoomScale(float value) {
		GetComponent<RectTransform> ().localScale = new Vector3 (value, value, value);
	}

	public void onCompleteZoomScale() {
		isRunning = false;
	}

	public void onUpdateZoomAlpha(float value) {
		GetComponent<CanvasGroup> ().alpha = value;
	}

	public void onCompleteZoomAlphaOut() {
		isRunning = false;
		isSmall = false;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		GetComponent<CanvasGroup> ().alpha = 0f;
	}

	public void onCompleteZoomAlphaIn() {
		isRunning = false;
		isSmall = true;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		GetComponent<CanvasGroup> ().interactable = true;
		GetComponent<CanvasGroup> ().alpha = 1f;
	}


	public void ShrinkIn (bool showAnimation) {
		if (showAnimation)
			duration = 1f;
		else
			duration = 0f;
		
		if (isRunning)
			return;

		if (!isFull)
			return;

		isRunning = true;

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 1f,
				"to", 0f,
				"onUpdate", "onUpdateShrinkIn",
				"onUpdateTarget", this.gameObject,
				"onComplete", "onCompleteShrink",
				"onCompleteTarget", this.gameObject,
				"duration", duration));

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 1f,
				"to", 0f,
				"onUpdate", "onUpdateZoomAlpha",
				"onUpdateTarget", this.gameObject,				
				"onComplete", "onCompleteZoomAlphaIn",
				"onCompleteTarget", this.gameObject,
				"duration", duration));
	}



	public void ShrinkOut (bool showAnimation) {
		if (showAnimation)
			duration = 1f;
		else
			duration = 0f;
		
		if (isRunning)
			return;

		if (isFull)
			return;

		isRunning = true;

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 0f,
				"to", 1f,
				"onUpdate", "onUpdateShrinkIn",
				"onUpdateTarget", this.gameObject,
				"onComplete", "onCompleteShrink",
				"onCompleteTarget", this.gameObject,
				"duration", duration));

		easyEasing.ValueTo(this.gameObject,
			easyEasing.Params("easeType", "easeOutQuad",
				"from", 0f,
				"to", 1f,
				"onUpdate", "onUpdateZoomAlpha",
				"onUpdateTarget", this.gameObject,				
				"onComplete", "onCompleteZoomAlphaIn",
				"onCompleteTarget", this.gameObject,
				"duration", duration));
	}

	public void onUpdateShrinkIn(float value) {
		GetComponent<RectTransform> ().localScale = new Vector3 (1, value, 1);
	}

	public void onCompleteShrink() {
		if (isFull)
			GetComponent<RectTransform> ().localScale = new Vector3 (1, 0, 1);
		else 
			GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		isFull = !isFull;
	}


	public void startShrunk() {
		isFull = false;
		GetComponent<RectTransform> ().localScale = new Vector3 (1, 0, 1);
		GetComponent<CanvasGroup> ().alpha = 0f;
	}
}
