using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Libs;
using Classic;


using DG.Tweening;
namespace Classic
{
    
    public class JackpotBonusWheelCollider : MonoBehaviour
    {

        public delegate void OnTriggerStartCallback(string name);

        public OnTriggerStartCallback triggerStartCallback = null;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(triggerStartCallback != null)
            {
                triggerStartCallback(collision.gameObject.name);
            }
        }

    }
}
