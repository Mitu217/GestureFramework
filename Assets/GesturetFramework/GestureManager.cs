using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GestureLibrary
{
	public class GestureManager : MonoBehaviour 
	{
		/// <summary>
		/// タッチ情報更新（キャッシュ）
		/// </summary>
		List<GestureInfo> gestureInfo = new List<GestureInfo> ();

		/// <summary>
		/// イベント発火用クラス
		/// </summary>
		GestureEvent gestureEvent = new GestureEvent();

		/// <summary>
		/// Instance for Singleton
		/// </summary>
		private static GestureManager instance = null;
		public static GestureManager Instance {
			get {
				if (instance == null) {
					instance = (GestureManager)FindObjectOfType (typeof(GestureManager));
					if (instance == null) {
						Debug.LogWarning (typeof(GestureManager) + "is nothing");
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Registers the controller.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public void RegisterController (IGestureController controller) {
			gestureEvent.AddController (controller);
		}

		/// <summary>
		/// Unregisters the controller.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public void UnregisterController (IGestureController controller) {
			gestureEvent.RemoveController (controller);
		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		void OnDestroy() {
			// Free Instance in Memory
			instance = null;
		}
			
		void Update() {
			for (int i = 0; i < gestureInfo.Count; i++) {
				if (gestureInfo [i].IsUp) {
					gestureInfo.RemoveAt (i--);
					continue;
				}

				gestureInfo [i].IsDown = false;
				gestureInfo [i].IsUp = false;
				gestureInfo [i].IsDrag = false;
			}

			// Input Decision
			var IsInput = IsTouchPlatform()
			                  ? InputForTouch (ref gestureInfo) 
			                  : InputForMouse (ref gestureInfo);
			if (!IsInput) {
				return;
			}

			gestureEvent.FireEvent (gestureInfo);
		}
		

		/// <summary>
		/// Determines whether this instance is touch platform.
		/// </summary>
		/// <returns><c>true</c> if this instance is touch platform; otherwise, <c>false</c>.</returns>
		bool IsTouchPlatform () {
			if (Application.platform == RuntimePlatform.Android || 
				Application.platform == RuntimePlatform.IPhonePlayer) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Inputs for touch.
		/// </summary>
		/// <returns><c>true</c>, if for touch was input, <c>false</c> otherwise.</returns>
		/// <param name="info">Info.</param>
		bool InputForTouch (ref List<GestureInfo> info) {
			if (Input.touchCount == 0) {
				return false;
			}

			for (int i = 0; i < Input.touchCount; i++) {
				Touch touch = Input.touches [i];
			
				switch (touch.phase) {
				case TouchPhase.Began:
					GestureInfo gi = new GestureInfo ();
					gi.TouchId = touch.fingerId;
					gi.CurrentScreenPosition = touch.position;
					gi.DeltaDragDistance = touch.deltaPosition;
					gi.DeltaDurationTime = touch.deltaTime;
					gi.IsDown = true;
					gi.IsUp = false;
					gi.IsDrag = false;
					info.Add(gi);
					break;
				case TouchPhase.Moved:
					for (int j = 0; j < gestureInfo.Count; j++) {
						if (touch.fingerId == gestureInfo [j].TouchId) {
							gestureInfo [j].CurrentScreenPosition = touch.position;
							gestureInfo [j].DeltaDragDistance = touch.deltaPosition;
							gestureInfo [j].DeltaDurationTime = touch.deltaTime;
							gestureInfo [j].IsDrag = true;
							break;
						}
					}
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					for (int j = 0; j < gestureInfo.Count; j++) {
						if (touch.fingerId == gestureInfo [j].TouchId) {
							gestureInfo [j].CurrentScreenPosition = touch.position;
							gestureInfo [j].DeltaDragDistance = touch.deltaPosition;
							gestureInfo [j].DeltaDurationTime = touch.deltaTime;
							gestureInfo [j].IsUp = true;
							break;
						}
					}
					break;
				}
			}

			// Error Check
			if (info.Count == 0) {
				Debug.LogWarning ("【Invalid Call】It is not assumed");
				return false;
			}

			return true;
		}
			
		/// <summary>
		/// Inputs for mouse.
		/// </summary>
		/// <returns><c>true</c>, if for mouse was input, <c>false</c> otherwise.</returns>
		/// <param name="info">Info.</param>
		bool InputForMouse (ref List<GestureInfo> info) {
			#if UNITY_EDITOR
			Debug.LogWarning("【Call Check】Input For Mouse");
			#endif

			if (Input.GetMouseButtonDown (0)) { // Mouse Click Down Eent
				#if UNITY_EDITOR
				Debug.LogWarning("【Call Check】Mouse Down");
				#endif
				for (int i = 0; i < info.Count; i++) {
					// Error Check
					if (info [i].TouchId == -1) {
						Debug.LogWarning ("【Invalid Call】Second time of MouseButtonDown was called");
						info [i].IsDown = true;
						info [i].CurrentScreenPosition = Input.mousePosition;
						return true;
					}
				}

				// Create new TouchInfo 
				GestureInfo gi = new GestureInfo ();
				gi.TouchId = -1;
				gi.CurrentScreenPosition = Input.mousePosition;
				gi.IsDown = true;
				gi.IsUp = false;
				gi.IsDrag = false;
				info.Add(gi);
				return true;
			} else if (Input.GetMouseButtonUp (0)) { // Mouse Click Up Event
				#if UNITY_EDITOR
				Debug.LogWarning("【Call Check】Mouse Up");
				#endif

				for (int i = 0; i < info.Count; i++) {
					if (info [i].TouchId == -1) {
						info [i].IsUp = true;
						info [i].CurrentScreenPosition = Input.mousePosition;
						return true;
					}
				}
					
				#if UNITY_EDITOR
				Debug.LogWarning ("【Invalid Call】Target touchID isn't found");
				#endif
				return true;
			} else if (Input.GetMouseButton (0)) { // Mouse Click Stay Down Event
				#if UNITY_EDITOR
				Debug.LogWarning("【Call Check】Mouse Drag");
				#endif

				for (int i = 0; i < info.Count; i++) {
					if (info [i].TouchId == -1) {
						info [i].IsDrag = true;
						info [i].CurrentScreenPosition = Input.mousePosition;
						return true;
					}
				}

				#if UNITY_EDITOR
				Debug.LogWarning ("【Invalid Call】Target touchID isn't found");
				#endif
				return true;
			}

			return false;
		}
						
		/// <summary>
		/// Breaks the touch event.
		/// </summary>
		public void BreakTouchEvent (){
			gestureEvent.BreakFireEvent ();
		}

		/// <summary>
		/// Show Debug Flag
		/// </summary>
		public bool showDebug = false;

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI() {
			if (showDebug) {
				int x = 0;
				int y = 0;
				GUI.Label( new Rect(x,y,300,20), "TouchCount = " + gestureInfo.Count );
				y += 20;  
				for (int i = 0; i < gestureInfo.Count; i++) {
					GUI.Label( new Rect(x,y,300,20), "TouchPosition = " + gestureInfo[i].CurrentScreenPosition );
					y += 20;  
				}
			}
		}
	}
}