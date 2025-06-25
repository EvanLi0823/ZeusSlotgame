using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace WildWest
{
    public class TaurusMove : MonoBehaviour
    {
        public bool stop {get; private set;}
    
        private Vector3 target;
        private float time;
    
        public void OnStartMove(Vector3 _target, float _time)
        {
            target = _target;
            time = _time;
            stop = false;
        }

        public void StartMove()
        {
            this.transform.DOMove(target, time).SetEase ((Ease)Ease.Linear).OnComplete(()=>
            {  
                stop = true;
                Destroy(this.gameObject);
            });
        }
    }
}


