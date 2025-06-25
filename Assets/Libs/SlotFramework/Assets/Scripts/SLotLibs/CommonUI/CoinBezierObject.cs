using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CoinBezierObject : MonoBehaviour {

	public SkyBezierCurve skyBezierCurve;
	public delegate void MCallBack ();
	
	public MCallBack MOnAnimationStart;
	public MCallBack MOnAnimationCompleted;

	// 这两个List<Vector2>使用了折线来描述物体运动位置与物体大小随时间的变化
	// x轴坐标取值范围为[0,1]，表示自物体开始运动到现在经过时间占运动总时间的比例
	// y轴取值表示为物体位移占总位移的比例/物体的缩放大小
	// 折线本身通过这显得各个端点描述，要求端点x坐标按升序排列
	// 对于给定的x坐标值，会通过左右离它最近的点连接而成的线段进行线性插值取得对应的y坐标值
	public List<Vector2> sizeScaleChangePolyline;
	public List<Vector2> timeScaleChangePolyline;

	public Vector3 rotateSpeed = Vector3.zero;

	// 经过time秒后开始播放动画
	public void startAnimation (float time)
	{
		// 设置起始点
		skyBezierCurve.startPoint = transform.localPosition;
		// 根据之前的参数（起点、终点、中间点）生成贝塞尔曲线
		skyBezierCurve.CreateCurve ();
		// 经过time秒后依曲线轨迹移动
		StartCoroutine (Tweening (time));
	}

	// 更新
	public virtual void UpdateAnimation (float time)
	{
//		transform.localScale = new Vector3 (((1 - time / skyBezierCurve.timeDuration) * 0.5f + 0.73f), ((1 - time / skyBezierCurve.timeDuration)) * 0.5f + 0.73f, 0);
		if (sizeScaleChangePolyline != null)
		{
			transform.localScale = new Vector3 (ChangeSizeScale(time / skyBezierCurve.timeDuration), ChangeSizeScale(time / skyBezierCurve.timeDuration), 0);
		}
//		transform.localPosition = new Vector3 (skyBezierCurve.animX.Evaluate (time / skyBezierCurve.timeDuration), skyBezierCurve.animY.Evaluate (time / skyBezierCurve.timeDuration), 0);
		if (timeScaleChangePolyline != null)
		{
			transform.localPosition = new Vector3 (skyBezierCurve.animX.Evaluate (ChangeTimeScale(time / skyBezierCurve.timeDuration)), skyBezierCurve.animY.Evaluate (ChangeTimeScale(time / skyBezierCurve.timeDuration)), 0);
		}
		transform.rotation = Quaternion.Euler(rotateSpeed * time);
	}
	
	public void SetScaleZero(){
		Image mImage = transform.GetComponent<Image> ();
		mImage.color = new Color(1,1,1,0);
	}

	protected virtual float ChangeTimeScale(float timeRatio) {
		return ChangeScale(timeScaleChangePolyline, timeRatio);
	}

	protected virtual float ChangeSizeScale(float timeRatio) {
		return ChangeScale(sizeScaleChangePolyline, timeRatio);
	}

	protected virtual float ChangeScale(List<Vector2> polyline, float x) {
		if (polyline == null || polyline.Count < 2) {
			Debug.LogError("Not enough points to discribe polyline! Add more points.");
			return 0;
		}

		if (x > 1.0f) {
			x = 1.0f;
		}
		if (x < 0.0f) {
			x = 0.0f;
		}

		for (int i = 1; i < polyline.Count - 1; i++) {
			if (x < polyline[i].x) {
				return LinearInterpolation(polyline[i], polyline[i - 1], x);
			}
		}

		return LinearInterpolation(polyline[polyline.Count - 1], polyline[polyline.Count - 2], x);
	}

	protected float LinearInterpolation(Vector2 p1, Vector2 p2, float x) {
		if (p1.x == p2.x) {
			Debug.LogError("p1.x == p2.x, cannot decide return value.");
		}
		return (x * (p1.y - p2.y) + (p1.x * p2.y - p1.y * p2.x)) / (p1.x - p2.x);
	}

	/// <summary>
	/// 启动协程，在经过time秒后播放动画（更新位置、大小）
	/// </summary>
	/// <param name="time">开始之前的等待时间（秒）</param>
	IEnumerator Tweening (float time)
	{
		yield return new WaitForSecondsRealtime (time);
		Animator anim = gameObject.GetComponent<Animator>();
		if (anim != null) {
			anim.updateMode = AnimatorUpdateMode.UnscaledTime;
			anim.enabled = true;
		}
		if (MOnAnimationStart != null)
			MOnAnimationStart ();
		float t = Time.realtimeSinceStartup;
		while (2 * (Time.realtimeSinceStartup - t) < skyBezierCurve.timeDuration) {
			yield return 0;
			UpdateAnimation (2f * (Time.realtimeSinceStartup - t));
		}
		if (MOnAnimationCompleted != null)
			MOnAnimationCompleted ();
	}
}
