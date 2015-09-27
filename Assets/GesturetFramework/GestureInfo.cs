using UnityEngine;
using System.Collections;

namespace GestureLibrary
{
	public class GestureInfo {
		/// <summary>
		/// Finger ID
		/// </summary>
		/// <value>The finger ID.</value>
		public int TouchId{ get; set; }
		/// <summary>
		/// 最新のタッチ・クリック座標
		/// </summary>
		/// <value>The current screen position.</value>
		public Vector3 CurrentScreenPosition{ get; set; }
		/// <summary>
		/// 前フレームからの移動距離
		/// </summary>
		/// <value>The delta drag distance.</value>
		public Vector3 DeltaDragDistance{ get; set;	}
		/// <summary>
		/// 前フレームからの経過時間
		/// </summary>
		/// <value>The delta duration time.</value>
		public float DeltaDurationTime{ get; set; }
		/// <summary>
		/// DownEvent中かどうか
		/// </summary>
		/// <value><c>true</c> if this instance is down; otherwise, <c>false</c>.</value>
		public bool IsDown { get; set; }
		/// <summary>
		/// UpEvent中かどうか
		/// </summary>
		/// <value><c>true</c> if this instance is up; otherwise, <c>false</c>.</value>
		public bool IsUp { get; set; }
		/// <summary>
		/// DragEvent中かどうか
		/// </summary>
		/// <value><c>true</c> if this instance is drag; otherwise, <c>false</c>.</value>
		public bool IsDrag { get; set; }
	}
}