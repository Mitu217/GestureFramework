using UnityEngine;
using System.Collections;
using GestureLibrary;

public class SampleGestureController : MonoBehaviour, IGestureController {

	// Use this for initialization
	void Start () {
		GestureManager.Instance.RegisterController (this);
	}
	
	#region IGestureController implementation

	public void OnGestureDown (GestureInfo info)
	{
		// Down時に呼ばれます

		// キャンセル命令発行
		//GestureManager.Instance.BreakTouchEvent():
	}

	public void OnGestureUp (GestureInfo info)
	{
		// Up時に呼ばれます
	}

	public void OnGestureDrag (GestureInfo info)
	{
		// Drag時に呼ばれます
	}

	public int Order {
		get {
			// 処理の優先度を返します
			// 番号が若い方が優先度高
			return 0;
		}
	}

	public bool IsGestureProcess {
		get {
			// true: このControllerのイベントは呼ばれます
			// false: このControllerのイベントは呼ばれません
			return true;
		}
	}

	#endregion
}
