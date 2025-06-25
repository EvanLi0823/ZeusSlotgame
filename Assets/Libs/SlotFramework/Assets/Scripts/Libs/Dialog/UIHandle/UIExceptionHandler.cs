using System;
using Core;
using UnityEngine;
namespace Libs
{
    public class UIExceptionHandler : MonoBehaviour
    {
        private string KEY_PATH = "ApplicationConfig/UIExceptionSwitch";
        private void Awake()
        {
            Messenger.AddListener(GameConstants.OnGameConsoleStarted,SetSwitch);
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(GameConstants.OnGameConsoleStarted,SetSwitch);
        }

        private void SetSwitch()
        {
            bool enable = Plugins.Configuration.GetInstance().GetValueWithPath<bool>(KEY_PATH, true);
            enableExceptionMonitor = enable;
            if(!enable)  monitor = false;
        }
        private bool enableExceptionMonitor = true;
        private bool monitor = false;
        private DateTime start;
        public void StartMonitor()
        {
            if (!enableExceptionMonitor)  return;
            if (monitor) return;
            monitor = true;
            start = DateTime.Now;
        }

        public void StopMonitor()
        {
            if (!enableExceptionMonitor)  return;
            if (!monitor) return;
            monitor = false;
        }
        
        private void Update()
        {
            if (monitor)
            {
                DateTime now = DateTime.Now;
                if (Mathf.Abs((now - start).Seconds)>=1)
                {
                    start = now;
                    Messenger.Broadcast(GameConstants.CHECK_UP_UI_EXCEPTION_KEY);
                }
            }
        }
    }
}