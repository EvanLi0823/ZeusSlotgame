using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//每一关卡自定义继承此接口，动态更改runData
public interface IRunDataChange {
	void ChangeRunData (ReelRunData _data);
}
