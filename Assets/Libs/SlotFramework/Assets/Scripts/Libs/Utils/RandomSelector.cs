using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Libs;

namespace Libs
{
	public class RandomSelector
	{
		public static List<T> Select<T>(List<T> selectPool, List<int> weight = null, int selectNumber = 1, bool canSelectTheSame = false)
		{
			if (weight != null && weight.Count != selectPool.Count) {
				return null;
			}
			if (!canSelectTheSame && selectPool.Count < selectNumber) {
				return null;
			}

			List<T> ret = new List<T>();
			List<T> selectPool2 = new List<T>(selectPool);
			List<int> weight2 = null;
			if (weight != null) {
				weight2 = new List<int> (weight);
			}
			for (int i = 0; i < selectNumber; i++) {
				int index = 0;
				if (weight2 == null) {
					index = Random.Range(0, selectPool2.Count);
				}
				else {
					int totalWeight = 0;
					for (int j = 0; j < weight2.Count; j++) {
						totalWeight += weight2[j];
					}
					int randomInt = Random.Range(0, totalWeight);
					totalWeight = 0;
					for (int j = 0; j < weight2.Count; j++) {
						totalWeight += weight2[j];
						if (totalWeight > randomInt) {
							index = j;
							break;
						}
					}
				}

				ret.Add(selectPool2[index]);
				if (!canSelectTheSame) {
					selectPool2.RemoveAt(index);
					if (weight2 != null) {
						weight2.RemoveAt(index);
					}
				}
			}
			return ret;
		}

		public static List<int> SelectIndex(int length, List<int> weight = null, int selectNumber = 1, bool canSelectTheSame = false)
		{
			if (weight != null && weight.Count != length) {
				return null;
			}
			if (!canSelectTheSame && length < selectNumber) {
				return null;
			}

			if (weight == null && selectNumber == 1) {
				List<int> tempList = new List<int>();
				tempList.Add(Random.Range(0, length));
				return tempList;
			}

			List<bool> selected = new List<bool>();
			for (int i = 0; i < length; i++) {
				selected.Add(false);
			}
			if (weight == null) {
				weight = new List<int>();
				for (int j = 0; j < length; j++) {
					weight.Add(1);
				}
			}

			List<int> ret = new List<int>();
			for (int i = 0; i < selectNumber; i++) {
				int index = 0;

				int totalWeight = 0;
				for (int j = 0; j < weight.Count; j++) {
					if (!selected[j]) {
						totalWeight += weight[j];
					}
				}
				int randomInt = Random.Range(0, totalWeight);
				totalWeight = 0;
				for (int j = 0; j < weight.Count; j++) {
					if (!selected[j]) {
						totalWeight += weight[j];
						if (totalWeight > randomInt) {
							index = j;
							break;
						}
					}
				}

				ret.Add(index);
				if (!canSelectTheSame) {
					selected[index] = true;
				}
			}
			return ret;
		}


		//对一个数组进行随机排序
		/// <summary>
		/// 对一个数组进行随机排序
		/// </summary>
		/// <typeparam name="T">数组的类型</typeparam>
		/// <param name="arr">需要随机排序的数组</param>
		public static void GetRandomArray<T>(T[] arr)
		{
			int count = arr.Length;

			//开始交换
			for (int i = 0; i < count - 1; i++)
			{
				//生成两个随机数位置
				int randomNum = Random.Range(i, arr.Length);

				//定义临时变量
				T temp;

				//交换两个随机数位置的值
				temp = arr[randomNum];
				arr[randomNum] = arr[i];
				arr[i] = temp;
			}
		}

		public static List<T> GetRandomList<T>(List<T> inputList)
		{
			List<T> ret = new List<T>(inputList);

			//开始交换
			for (int i = 0; i < ret.Count - 1; i++) {
				//生成两个随机数位置
				int randomNum = Random.Range(i, ret.Count);

				//定义临时变量
				T temp;

				//交换两个随机数位置的值
				temp = ret[randomNum];
				ret[randomNum] = ret[i];
				ret[i] = temp;
			}

			return ret;
		}
	}
}
