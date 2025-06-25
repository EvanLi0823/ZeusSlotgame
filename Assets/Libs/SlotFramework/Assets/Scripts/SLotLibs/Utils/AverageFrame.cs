using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Classic
{
	public class AverageFrame
	{
		List<float>  times = new List<float> ();
		int totalNumber ;
		int index = 0;
	
		public float averageTime {
			get;
			private set;
		}
	
		public AverageFrame (int totalNumber)
		{
			this.totalNumber = totalNumber;
			for (int i=0; i<this.totalNumber; i++) {
				this.times.Add (1 / 30f);
			}
			index = 0;
			averageTime = 1 / 30f;
		}
	
		public void ChangeFrame (float currentTime)
		{
			if (currentTime < 1 / 60f)
				currentTime = 1 / 60f;
			averageTime = (currentTime - times [index] + averageTime * totalNumber) / totalNumber;
			times [index] = currentTime;
			index ++;
			if (index >= totalNumber) {
				index = 0;
			}
		}
	}
}