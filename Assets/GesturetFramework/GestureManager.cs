using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GestureLibrary
{
	public class GestureManager : MonoBehaviour 
	{
		/// <summary>
		/// タッチ情報（キャッシュ）
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
				Debug.LogWarning ("【Invalid Call】");
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
			if (Input.GetKeyDown (KeyCode.RightAlt) || Input.GetKeyDown (KeyCode.LeftAlt)) {
				Debug.Log ("Alt key Down");
			} else if (Input.GetKeyUp (KeyCode.RightAlt) || Input.GetKeyUp (KeyCode.LeftAlt)) {
				Debug.Log ("Alt key Up");
			}
				
			if (Input.GetMouseButtonDown (0)) { // Mouse Click Down Eent
				for (int i = 0; i < info.Count; i++) {
					// Error Check
					if (info [i].TouchId == -1) {
						Debug.LogWarning ("【Invalid Call】");
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

				#if UNITY_EDITOR
				if (Input.GetKeyDown (KeyCode.RightAlt) 
					|| Input.GetKeyDown (KeyCode.LeftAlt) 
					|| Input.GetKey(KeyCode.RightAlt) 
					|| Input.GetKey(KeyCode.LeftAlt)) {
					GestureInfo demoGi = new GestureInfo ();
					demoGi.TouchId = -2;
					Vector3 demoPos 
						= new Vector2(Screen.width-Input.mousePosition.x,  Screen.height-Input.mousePosition.y);
					demoGi.CurrentScreenPosition = demoPos;
					demoGi.IsDown = true;
					demoGi.IsUp = false;
					demoGi.IsDrag = false;
					info.Add(demoGi);
				} 
				#endif

				return true;
			} else if (Input.GetMouseButtonUp (0)) { // Mouse Click Up Event
				for (int i = 0; i < info.Count; i++) {
					if (info [i].TouchId == -1) {
						info [i].IsUp = true;
						info [i].CurrentScreenPosition = Input.mousePosition;

						#if UNITY_EDITOR
						if (Input.GetKeyUp (KeyCode.RightAlt) 
							|| Input.GetKeyUp (KeyCode.LeftAlt) 
							|| Input.GetKey(KeyCode.RightAlt) 
							|| Input.GetKey(KeyCode.LeftAlt)) {
							for (int j = 0; j < info.Count; j++) {
								if (info [j].TouchId == -2) {
									info [j].IsUp = true;
									Vector3 demoPos 
										= new Vector2(Screen.width-Input.mousePosition.x,  Screen.height-Input.mousePosition.y);
									info [j].CurrentScreenPosition = demoPos;
								}
							}
						} 
						#endif

						return true;
					}
				}

				return true;
			} else if (Input.GetMouseButton (0)) { // Mouse Click Stay Down Event
				for (int i = 0; i < info.Count; i++) {
					if (info [i].TouchId == -1) {
						info [i].IsDrag = true;
						info [i].CurrentScreenPosition = Input.mousePosition;

						#if UNITY_EDITOR
						if (Input.GetKeyUp (KeyCode.RightAlt) || Input.GetKeyUp (KeyCode.LeftAlt)){
							for (int j = 0; j < info.Count; j++) {
								if (info [j].TouchId == -2) {
									info [j].IsUp = true;
									Vector3 demoPos 
										= new Vector2(Screen.width-Input.mousePosition.x,  Screen.height-Input.mousePosition.y);
									info [j].CurrentScreenPosition = demoPos;
								}
							}
							return true;
						}
						if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)) {
							if (Input.GetKeyDown (KeyCode.RightAlt) 
								|| Input.GetKeyDown (KeyCode.LeftAlt)){
								GestureInfo demoGi = new GestureInfo ();
								demoGi.TouchId = -2;
								Vector3 demoPos 
									= new Vector2(Screen.width-Input.mousePosition.x,  Screen.height-Input.mousePosition.y);
								demoGi.CurrentScreenPosition = demoPos;
								demoGi.IsDown = true;
								demoGi.IsUp = false;
								demoGi.IsDrag = false;
								info.Add(demoGi);
								return true;
							}
							for (int j = 0; j < info.Count; j++) {
								if (info [j].TouchId == -2) {
									info [j].IsDrag = true;
									Vector3 demoPos 
										= new Vector2(Screen.width-Input.mousePosition.x,  Screen.height-Input.mousePosition.y);
									info [j].CurrentScreenPosition = demoPos;
								}
							}
							return true;
						} 
						#endif

						return true;
					}
				}

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
		/// デバッグ用
		/// </summary>
		public bool showDebug = false;
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