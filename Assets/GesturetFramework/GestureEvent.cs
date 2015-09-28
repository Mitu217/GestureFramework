using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureLibrary
{
	public class GestureEvent
	{
		/// <summary>
		/// FireEventのループをキャンセルするためのフラグ
		/// </summary>
		private bool cancel = false;

		/// <summary>
		/// イベントを発火する登録済みコントローラー
		/// </summary>
		List<IGestureController> controllers = new List<IGestureController>();

		/// <summary>
		/// コントローラの登録
		/// </summary>
		/// <param name="controller">Controller.</param>
		public void AddController (IGestureController controller) {
			var index = this.controllers.FindIndex (c => c.Order > controller.Order);
			if (index < 0) {
				index = controllers.Count;
			}
			controllers.Insert (index, controller);
		}

		/// <summary>
		/// コントローラの登録解除
		/// </summary>
		/// <param name="controller">Controller.</param>
		public void RemoveController (IGestureController controller) {
			controllers.Remove (controller);
		}
			
		/// <summary>
		/// イベント発火用
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public void FireEvent(List<GestureInfo> info) {
			for (int i = 0; i < info.Count; i++) {
				if (info [i].IsDown) {
					DoDown (info [i]);
				} else if (info [i].IsUp) {
					DoUp(info [i]);
				} else if (info [i].IsDrag) {
					DoDrag (info [i]);
				} else {
					Debug.LogWarning ("【Invalid Call】");
				}
			}
		}

		public void BreakFireEvent() {
			cancel = true;
		}

		void DoDown (GestureInfo info) {
			for (int i = 0; i < controllers.Count; i++) {
				if (cancel) {
					cancel = false;
					break;
				}

				if (controllers [i].IsGestureProcess) {
					controllers [i].OnGestureDown (info);
				}
			}
		}

		void DoUp (GestureInfo info) {
			for (int i = 0; i < controllers.Count; i++) {
				if (cancel) {
					cancel = false;
					break;
				}

				if (controllers [i].IsGestureProcess) {
					controllers [i].OnGestureUp (info);
				}
			}
		}

		void DoDrag (GestureInfo info) {
			for (int i = 0; i < controllers.Count; i++) {
				if (cancel) {
					cancel = false;
					break;
				}

				if (controllers [i].IsGestureProcess) {
					controllers [i].OnGestureDrag (info);
				}
			}
		}
	}
}