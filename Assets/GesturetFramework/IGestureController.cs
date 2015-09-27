using UnityEngine;
using System.Collections;

namespace GestureLibrary {
	public interface IGestureController {
	
		/// <summary>
		/// ジェスチャーの処理優先度
		/// </summary>
		/// <value>The order.</value>
		int Order {
			get;
		}

		/// <summary>
		/// ジェスチャーを処理する必要があるかどうか
		/// </summary>
		/// <value><c>true</c> if this instance is gesture process; otherwise, <c>false</c>.</value>
		bool IsGestureProcess {
			get;
		}

		/// <summary>
		/// Downイベント発生時に呼ばれる
		/// </summary>
		/// <param name="info">Info.</param>
		void OnGestureDown(GestureInfo info);

		/// <summary>
		/// Upイベント発生時に呼ばれる
		/// </summary>
		/// <param name="info">Info.</param>
		void OnGestureUp(GestureInfo info);
	
		/// <summary>
		/// Dragイベント発生時に呼ばれる
		/// </summary>
		/// <param name="info">Info.</param>
		void OnGestureDrag(GestureInfo info);
	}
}