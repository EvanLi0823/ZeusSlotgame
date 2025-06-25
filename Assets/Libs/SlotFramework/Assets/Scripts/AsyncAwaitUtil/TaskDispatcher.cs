using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Task dispatcher.
/// task中如果有thread的话，用来在thread中访问unity的方法
/// </summary>
public class TaskDispatcher : MonoBehaviour
{
    private List<Action> pending = new List<Action>();

    public static TaskDispatcher Instance { get; private set; }

    public void Invoke(Action fn)
    {
        lock (this.pending)
        {
            this.pending.Add(fn);
        }
    }

    private void InvokePending()
    {
        lock (this.pending)
        {
            foreach (Action action in this.pending)
            {
                action();
            }

            this.pending.Clear();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        this.InvokePending();
    }
}