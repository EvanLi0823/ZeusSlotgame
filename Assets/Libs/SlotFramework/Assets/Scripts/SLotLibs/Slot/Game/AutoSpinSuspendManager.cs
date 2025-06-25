using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoSpinSuspendManager
{
    private static bool _autoSpinState;

    private static int _refCount;

    //主要使用的操作是添加和删除，使用链表
    private static readonly LinkedList<AutoSpinSuspendHandler> SuspendHandlers =
        new LinkedList<AutoSpinSuspendHandler>();

    //挂起，返回AutoSpinSuspendHandler（用于Resume）
    public static AutoSpinSuspendHandler Suspend(string source)
    {
        bool isSuspendValid;
        if (_autoSpinState && IsReelManagerValid())
        {
            BaseSlotMachineController.Instance.reelManager.SetAutoRun(false);
            isSuspendValid = true;
            _refCount++;
        }
        else
        {
            isSuspendValid = false;
        }

        var handler = new AutoSpinSuspendHandler(source, isSuspendValid);
        SuspendHandlers.AddLast(new LinkedListNode<AutoSpinSuspendHandler>(handler));
        return handler;
    }

    //重置(开启或者关闭autospin时重置)
    public static void Reset(bool autoSpinState)
    {
        _autoSpinState = autoSpinState;
        _refCount = 0;
        foreach (var handler in SuspendHandlers)
        {
            handler.SetInValid();
        }

        SuspendHandlers.Clear();
    }

    //打印调试信息
    public static void PrintSuspends()
    {
        if (SuspendHandlers.Count == 0) Debug.Log("No AutoSpin Suspend!");
        foreach (var handler in SuspendHandlers)
        {
            Debug.Log(handler.ToString());
        }
    }

    public static bool IsSuspending()
    {
        return _refCount > 0;
    }

    public static bool IsReelManagerValid()
    {
        if (BaseSlotMachineController.Instance == null) return false;
        if (BaseSlotMachineController.Instance.reelManager == null) return false;
        return true;
    }

    public static void DecreaseRefCount()
    {
        _refCount--;
    }

    public static void RemoveHandler(AutoSpinSuspendHandler handler)
    {
        SuspendHandlers.Remove(handler);
    }
}

public class AutoSpinSuspendHandler
{
    public void Resume()
    {
        RemoveFromAutoSpinManager();
        if (!_isValid) return;
        _isValid = false;
        AutoSpinSuspendManager.DecreaseRefCount();
        if (AutoSpinSuspendManager.IsSuspending()) return;
        if (!AutoSpinSuspendManager.IsReelManagerValid()) return;
        BaseSlotMachineController.Instance.reelManager.SetAutoRun(true);
        BaseSlotMachineController.Instance.DoSpin();
    }

    private void RemoveFromAutoSpinManager()
    {
        AutoSpinSuspendManager.RemoveHandler(this);
    }

    private bool _isValid;
    private readonly string _source;

    public AutoSpinSuspendHandler(string source, bool isSuspendValid)
    {
        _isValid = isSuspendValid;
        _source = source;
    }

    public void SetInValid()
    {
        _isValid = false;
    }

    public override string ToString()
    {
        var toString = $"AutoSpinSuspend Source : {_source} , IsValid : {_isValid}";
        return toString;
    }
}