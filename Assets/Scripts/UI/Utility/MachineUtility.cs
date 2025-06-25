using System.Collections.Generic;
//using RealYou.Slot.UI;
//using RealYou.SlotLib;
using Classic;
using Libs;
using UnityEngine;

namespace RealYou.Core.UI
{
    public partial class MachineUtility
    {
        public static MachineUtility Instance { get { return Classic.Singleton<MachineUtility>.Instance; } }

        private MachineUtility() { }
        
        public bool IsValid()
        {
            return BaseSlotMachineController.Instance != null;
        }
        
        /// <summary>
        /// 返回当前机器名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrentMachineName()
        {
            if (BaseSlotMachineController.Instance == null || BaseSlotMachineController.Instance.slotMachineConfig == null)
                return string.Empty;

            return BaseSlotMachineController.Instance.slotMachineConfig.Name();
        }
        
        /// <summary>
        /// 返回当前机器是否处于 Freespin 状态
        /// </summary>
        /// <returns></returns>
        public bool IsFreespinBonus()
        {
            if (BaseSlotMachineController.Instance == null)
                return false;

            return BaseSlotMachineController.Instance.isFreeRun;
        }

        /*public bool IsOldMachine()
        {
            if (BaseSlotMachineController.Instance == null)
                return false;
            return BaseSlotMachineController.Instance.IsOldMachine;
        }*/
        /// <summary>
        /// 返回机器是否包含 blank symbol
        /// </summary>
        /// <returns></returns>
        public bool HasBlank(int panelId=0)
        {
            if (BaseSlotMachineController.Instance == null)
                return false;
            
            if (BaseSlotMachineController.Instance.reelManager != null)
                return BaseSlotMachineController.Instance.reelManager.gameConfigs.hasBlank;
            
            /*SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                return spinGame.HasBlank(panelId);
            }*/

            return false;
        }

        /// <summary>
        /// 返回当前机器棋盘数量
        /// </summary>
        /// <returns></returns>
        public int GetPanelCount()
        {
            if (BaseSlotMachineController.Instance == null)
                return 0;

            if (BaseSlotMachineController.Instance.reelManager != null)
                return 1;
            
            /*SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                return spinGame.PanelCount;
            }*/

            return 0;
        }
        
        /// <summary>
        /// 返回指定棋盘的中心位置
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public Vector3 GetPanelPosition(int panelId = 0)
        {
            if (BaseSlotMachineController.Instance == null)
                return Vector3.zero;
            
            /*if (BaseSlotMachineController.Instance.reelManager != null)
                return BaseSlotMachineController.Instance.reelManager.GetMiddlePosition();
            
            SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                return spinGame.GetMiddlePosition(panelId);
            }*/
            
            return Vector3.zero;
        }
        
        /// <summary>
        /// 返回指定棋盘的行数
        /// ReelCoun
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public int GetReelCols(int panelId = 0)
        {
            if (BaseSlotMachineController.Instance == null)
                return 0;
            
            if (BaseSlotMachineController.Instance.reelManager != null)
                return BaseSlotMachineController.Instance.reelManager.GetReelCount();

            /*SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                return spinGame.GetReelCols(panelId);
            }*/

            return 0;
        }

        /// <summary>
        /// 返回指定棋盘，指定行的列数
        /// ElementCount
        /// </summary>
        /// <param name="reelIndex"></param>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public int GetReelRows(int reelIndex, int panelId = 0)
        {
            if (BaseSlotMachineController.Instance == null)
                return 0;
            
            if (BaseSlotMachineController.Instance.reelManager != null)
                return BaseSlotMachineController.Instance.reelManager.Reels[reelIndex].Elements.Count;

            /*SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                return spinGame.GetReelRows(reelIndex,panelId);
            }*/

            return 0;
        }

        /// <summary>
        /// 返回指定位置的positio
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public Vector3 GetSymbolPosition(int col, int row, int panelId = 0)
        {
            if (BaseSlotMachineController.Instance == null)
                return Vector3.zero;

            if (BaseSlotMachineController.Instance.reelManager != null)
            {
                var symbol = BaseSlotMachineController.Instance.reelManager.GetSymbolRender(col,row);
                if (symbol != null)
                    return symbol.transform.position;
            }
            
            /*SpinGame spinGame = ViewFacade.TryGetView<SpinGame>(PanelView.ViewName);
            
            if(spinGame != null)
            {
                var symbol = spinGame.GetSymbolRender(col,row,panelId);
                if (symbol != null)
                    return symbol.transform.position;
            }*/
            
            return Vector3.zero;

        }
        
        /// <summary>
        /// 返回当前机器的 SymbolMap
        /// 老机器返回正确值，新机器返回 null
        /// </summary>
        /// <returns></returns>
        public SymbolMap GetCurSymbolMap()
        {
            if (BaseSlotMachineController.Instance == null)
                return null;

            if (BaseSlotMachineController.Instance.reelManager == null)
                return null;
            
            return BaseSlotMachineController.Instance.reelManager.symbolMap;
        }
        
        /// <summary>
        /// 返回当前机器的 ResultContent.
        /// 老机器返回正确值，新机器返回 null.
        /// </summary>
        /// <returns></returns>
        public ResultContent GetCurResultContent()
        {
            if (BaseSlotMachineController.Instance == null)
                return null;

            if (BaseSlotMachineController.Instance.reelManager == null)
                return null;
            
            return BaseSlotMachineController.Instance.reelManager.resultContent;
        }
        
        /// <summary>
        /// 通过 symbol type 返回当前牌面改 symbol 的数量
        /// 老机器返回正确值；新机器不支持此接口，故返回0.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetSymbolCountByType(string type)
        {
            if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null)
            {
                return BaseSlotMachineController.Instance.reelManager.GetSpecialCount(type);
            }
            
            return 0;
        }

        /// <summary>
        /// 通过 symbol name 返回当前牌面改 symbol 的数量.
        /// 老机器返回正确值；新机器不支持此接口，故返回0.
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public int GetSymbolCountByName(string symbolName)
        {
            if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null)
            {
                SymbolMap symbolMap = GetCurSymbolMap();
                if (symbolMap == null) return 0;
                int index = symbolMap.getSymbolIndex(symbolName);
                if (index == -1) return 0;
                List<BaseElementPanel> elementList = BaseSlotMachineController.Instance.reelManager.GetElementsInReelsBySymbolIndex(index, false);
                return (elementList == null || elementList.Count == 0) ? 0 : elementList.Count;
            }
            
            return 0;
        }

        /// <summary>
        /// 返回当前机器是否播放背景音乐.
        /// </summary>
        /// <returns></returns>
        public bool HasPlayBackMusic()
        {
            if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null)
            {
                return BaseSlotMachineController.Instance.reelManager.EnableInitBackgroundAudio;
            }
            
            return true;
        }
     
        /// <summary>
        /// 播放当前机器背景音乐
        /// </summary>
        /// <returns></returns>
        public void PlayBackMusic()
        {
            if (BaseSlotMachineController.Instance != null && BaseSlotMachineController.Instance.reelManager != null)
            {
                BaseSlotMachineController.Instance.reelManager.RestoreMusicAfterAnimation();
            }
            AudioManager.Instance.ResumeSound();
            
        }

        public void StopMusic()
        {
            AudioEntity.Instance.StopAllMusic ();
            
            AudioManager.Instance.PauseSound();
        }

        public void SetAutoRun(bool isStop)
        {
            /*if (BaseSlotMachineController.Instance != null)
            {
                BaseSlotMachineController.Instance.SetAutoRun(isStop);
            }*/
        }
        
        public void SetCostCoinsState(bool isCost)
        {
            /*if (BaseSlotMachineController.Instance != null)
            {
                BaseSlotMachineController.Instance.IsSpinCostCoins = isCost;
            }*/
        }

        /// <summary>
        /// 暂停或者恢复机器当中的所有操作.
        /// </summary>
        /// <param name="pause">true:暂停；false：恢复</param>
        public void Pause(bool pause)
        {
            /*if(MachineEventManager.instance == null)
                return;
            
            if (pause)
            {
                MachineEventManager.instance.PauseAllAnimations();
            }
            else
            {
                MachineEventManager.instance.ResumeAllAnimations();
            }*/
        }

        /// <summary>
        /// 检查机器是正在spin中.
        /// </summary>
        /// <returns></returns>
        public bool IsSpining()
        {
            if (BaseSlotMachineController.Instance == null)
                return false;
            
            return BaseSlotMachineController.Instance.GetIsSpining();
        }
        
        public bool CanChangeBet()
        {
            var machine = BaseSlotMachineController.Instance;
            if (machine == null)
                return true;

            return !machine.isFreeRun;
        }
    }
}

