using UnityEngine;
using System.Collections;
using UI;
using App;
using Core;
using System.Collections.Generic;
using System;
using Utils;

namespace UI.Performance
{
    public class ConfigDefGPUConsumer : MonoBehaviour, IGPUConsumer
    {
        #region LifeCycle

        void Start()
        {
            SetupWithConfig();
            PerformanceManager.Instance.Register(this);
            DoApply();
        }

        #endregion


        #region Public API

        public virtual float RequiredGPUScore()
        {
            return expectedGPUScore;
        }

        public virtual bool ShouldDeactiveGameObjectOnLowGPU()
        {
            return isDeactiveGameObject;
        }

        public virtual bool ShouldDisableComponentOnLowGPU()
        {
            return isDisableComponent;
        }

        public virtual bool DoApply()
        {
            bool hasApplied = false;

            if (PerformanceManager.Instance.GPUScore() < RequiredGPUScore())
            {
                hasApplied = DoApply(false);
            }

            return hasApplied;
        }

        public virtual bool Restore()
        {
            bool hasRestored = false;
            if (false)
            {
                hasRestored = DoApply(true);
            }

            return hasRestored;
        }

        #endregion


        private bool DoApply(bool isEnabled)
        {
            bool hasApplied = false;

            if (ShouldDeactiveGameObjectOnLowGPU())
            {
                gameObject.SetActive(isEnabled);
                hasApplied = true;
            }
            else if (ShouldDisableComponentOnLowGPU() && !string.IsNullOrEmpty(disabledComponentType))
            {
                try
                {
                    Type t = Type.GetType(disabledComponentType);
                    MonoBehaviour component = gameObject.GetComponentInChildren(t) as MonoBehaviour;
                    if (component == null)
                    {
                        component = gameObject.GetComponentInChildren(t) as MonoBehaviour;
                    }

                    if (component != null)
                    {
                        component.enabled = isEnabled;
                        hasApplied = true;
                    }
                }
                catch (Exception e)
                {
                    logger.LogErrorF("DoApply({0}) throws an expection {1}", isEnabled, e);
                }
            }

            return hasApplied;
        }


        private void SetupWithConfig()
        {
            if (BaseGameConsole.ActiveGameConsole().IsInSlotMachine())
            {
                Dictionary<string, object> properties =
                    BaseSlotMachineController.Instance.slotMachineConfig.Properties();

                string name = gameObject.name;
                if (CSharpUtil.GetValueWithPath<Dictionary<string, object>>(properties,
                        string.Format("/Performance/{0}", name), null) == null)
                {
                    if (name.EndsWith("(Clone)"))
                    {
                        name = name.Substring(0, name.Length - "(Clone)".Length);
                    }
                }

                if (CSharpUtil.GetValueWithPath<Dictionary<string, object>>(properties,
                        string.Format("/Performance/{0}", name), null) != null)
                {
                    expectedGPUScore = CSharpUtil.GetValueWithPath<float>(properties,
                        string.Format("/Performance/{0}/RequiredGPUScore", name), 0f);
                    isDeactiveGameObject = CSharpUtil.GetValueWithPath<bool>(properties,
                        string.Format("/Performance/{0}/IsDeactiveGameObject", name), false);
                    isDisableComponent = CSharpUtil.GetValueWithPath<bool>(properties,
                        string.Format("/Performance/{0}/IsDisableComponent", name), false);
                    disabledComponentType = CSharpUtil.GetValueWithPath<string>(properties,
                        string.Format("/Performance/{0}/DisabledComponentType", name), null);
                }
            }
        }

        private float expectedGPUScore;
        private bool isDeactiveGameObject;
        private bool isDisableComponent;
        private string disabledComponentType;


        private static global::Utils.Logger logger =
            global::Utils.Logger.GetUnityDebugLogger(typeof(ConfigDefGPUConsumer), false);
    }
}