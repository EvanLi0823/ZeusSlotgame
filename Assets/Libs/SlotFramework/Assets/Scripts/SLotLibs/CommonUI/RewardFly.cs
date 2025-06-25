
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Classic;
using DG.Tweening;
using UnityEngine.UI;
using Beebyte.Obfuscator;

namespace Libs
{
	[Skip]
	public class RewardFly : MonoBehaviour
	{
		private  string RewardItemPanelPath = "Prefab/Shared/RewardItemPanel";
		private static RewardFly _instance;

		// 单例模式
		public static RewardFly Instance {
			get {
				if (_instance == null) {
					// 尝试获取场景中已存在的，active的同类对象
					_instance = GameObject.FindObjectOfType<RewardFly> ();
					if (_instance == null) {
						// 初始化
						_instance = Instantiate(Libs.ResourceLoadManager.Instance.LoadResource<GameObject> ("Prefab/Shared/RewardItemPanel")).GetComponent<RewardFly> ();
						// 向场景中导入指定prefab，并从中获取对象
						// 设置transform并移动至最上层
						_instance.transform.SetParent (Libs.UIManager.Instance.Root.transform.parent);
						(_instance.transform as RectTransform).localPosition = new Vector3(0,0,0);
						(_instance.transform as RectTransform).localScale = Vector3.one;
						
						_instance.transform.SetAsLastSibling ();
					
					}
					Canvas canvas = _instance.GetComponent<Canvas> ();
					if (canvas != null) {
						canvas.overrideSorting = true;
					}
				}
				return _instance;
			}
		}

		public void PlayShopItemFly(List<GameObject> list,List<Vector3> startPos,Transform target)
		{
			if(target==null || list==null)return;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].transform.SetParent(transform);
				list[i].transform.localScale = Vector3.one;
				list[i].transform.position = startPos[i];
				Sequence mySequence = DOTween.Sequence();
				mySequence.Append(list[i].transform.DOScale(new Vector3(2, 2, 2), 0.2f))
					.Append(list[i].transform.DOMove(target.position, 0.5f))
					.Insert(0.2f, list[i].transform.DOScale(new Vector3(0, 0, 0), 0.5f))
				.OnComplete(() =>
					{
//						mySequence.Kill();
//						list[i].SetActive(false);
//						Destroy(list[i]);
					});

			}
		}
	}
}
