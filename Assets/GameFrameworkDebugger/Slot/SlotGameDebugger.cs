using System.IO;
using RealYou.Tool;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameFrameworkDebugger.Runtime
{
    public class SlotGameDebugger : ISlotGameDebugger
    {
        private string _segment = string.Empty;
        public SlotGameDebugger()
        {
            Messenger.AddListener<string>(GameConstants.SEGMENT_INIT_DONE, OnSegmentInitDone);
        }

        ~SlotGameDebugger()
        {
            Messenger.RemoveListener<string>(GameConstants.SEGMENT_INIT_DONE, OnSegmentInitDone);
        }

        private void OnSegmentInitDone(string segment)
        {
            _segment = segment;
        }

        public string GetUserSegment()
        {
            return _segment;
        }

        public void DeletePersistentData()
        {
            string path = Application.persistentDataPath;
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Delete(true);
                Application.Quit();
            }
        }

        public void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll ();
            Application.Quit ();
        }

        public bool GetUsingScaledScreenResolution()
        {
            var useScaledResolution = PlayerPrefs.GetString(SkySreenUtils.UseScaledResolution, string.Empty);
            if (string.IsNullOrEmpty(useScaledResolution))
            {
                return !IsHighPerformanceDevice();
            }
            bool scaleResolution = false;
            if (useScaledResolution.Equals(true.ToString()))
            {
                scaleResolution = true;
            }else if (useScaledResolution.Equals(false.ToString()))
            {
                scaleResolution = false;
            }

            return scaleResolution;
        }
        

        public bool IsHighPerformanceDevice()
        {
            
            return SkySreenUtils.IsHighPerformanceDevice();
        }

        public void SetUseScaledScreenResolution()
        {
            PlayerPrefs.SetString(SkySreenUtils.UseScaledResolution,true.ToString());
            Application.Quit();
        }

        public void SetDoNotUseScaledScreenResolution()
        {
            PlayerPrefs.SetString(SkySreenUtils.UseScaledResolution,false.ToString());
            Application.Quit();
        }
        
        public void SendGMCmd(string cmd)
        {
            if(string.IsNullOrEmpty(cmd))
                return;
            GMCommandUtil.SendGM2Server(cmd);
        }

        public void DebugToReload()
        {
            BaseGameConsole.singletonInstance.Restart();
        }
    }
}
