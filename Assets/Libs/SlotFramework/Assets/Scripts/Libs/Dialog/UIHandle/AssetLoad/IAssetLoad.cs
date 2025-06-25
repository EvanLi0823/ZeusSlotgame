using System;
using System.Collections;

namespace Libs
{
    public interface IAssetLoad
    {
        IEnumerator LoadRes<T>(OpenConfigParam<T> p, System.Action<T> sucCB, Action<string> failedCB)
            where T : UIBase;
    }
}