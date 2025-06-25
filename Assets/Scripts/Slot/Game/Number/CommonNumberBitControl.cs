using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonNumberBitControl : MonoBehaviour
{
    public UIText firstText;
    public UIText lastText;

    protected List<int> numberList = new List<int>();
    public int GetNumberListCount()
    {
        return numberList.Count;
    }

    public int currentIndex = 0;

    public bool finished = false;

    public string prePausePosition = "";//LQ 再次恢复使用时需要重新执行一遍Animation Event
    public float speed = -1;
    public int state = -1;
    public void InitNumberBitData(List<int> numberData, bool run = true)
    {
        numberList.Clear();
        currentIndex = 0;
        finished = false;
        numberList.AddRange(numberData);
        if (run)
        {
            ChangeTextContext(prePausePosition);//再次恢复使用时需要重新执行一遍Animation Event
            SetAnimatorSpeed(1);
        }
        else
        {
            SetAnimatorSpeed(0);
        }

    }
    //LQ AnimationEvent Use
    public void ChangeTextContext(string toggleFlag)
    {
        switch (toggleFlag)
        {
            case "First":
                {
                    if (CheckNumberBitRollupFinish())
                    {
                        prePausePosition = "First";
                        SetAnimatorSpeed(0);
                        return;
                    }
                    else
                    {
                        SetAnimatorSpeed(1);
                    }

                    firstText.SetText(numberList[currentIndex++].ToString());//LQ最后一帧设定第一个old text
                }
                break;
            case "Last":
                {
                    if (CheckNumberBitRollupFinish())
                    {
                        prePausePosition = "Last";
                        SetAnimatorSpeed(0);
                        return;
                    }
                    else
                    {
                        SetAnimatorSpeed(1);
                    }

                    lastText.SetText(numberList[currentIndex++].ToString());//LQ 第一帧  设定第二个new text
                }
                break;
            default:
                break;
        }
    }

    public void SetUnNumberText(char unDigital)
    {
        firstText.SetText(unDigital.ToString());
        lastText.SetText(unDigital.ToString());
        SetAnimatorSpeed(0);

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            if (Mathf.Approximately(speed, 0f)&&animator.GetInteger("state")==1)
            {
                //Debug.Log("SetUnNumberText:"+ unDigital);
                //AnimatorStateInfo asinfo = animator.GetCurrentAnimatorStateInfo(-1);
                //if (asinfo.length>0&&asinfo.length<1)
                {
                    animator.Play("CollectNumberUp", -1, 0);
                    SetAnimatorSpeed(0);
                }

                //animator.Update(0);
            }
        }
    }
        protected bool CheckNumberBitRollupFinish()
    {
        finished = false;
        if (currentIndex >= numberList.Count)
        {
            finished = true;
        }
        return finished;
    }

    protected void SetAnimatorSpeed(float speed)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = speed;

            this.speed = speed;
           
            this.state = animator.GetInteger("state");
        }
    }
}
