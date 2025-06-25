using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumbertSlideIncrease : MonoBehaviour
{
    public SingleNumber singleNumber;
    public AlignmentType alignmentType = AlignmentType.Middle;

    private List<float> numberPosX = new List<float>();
    private List<SingleNumber> singleNumberTable = new List<SingleNumber>();

    private float initSingleTextWidth = 0;

    private bool initText = true;
    private float startPosX;
    private float textWidth;
    private float singleTextWidth;
    private Vector2 singleTextScale = Vector2.one;
    
    private bool isSlided = false;
    private long curNumber = 0;
    private long initCurNumber = 0;
    private long increaseNumber = 0;
    private Coroutine slideCoroutine;
    private WaitForSecondsRealtime waitSeconds = new WaitForSecondsRealtime (0.2f);

    void Awake()
    {
        this.InitTextUIData();
    }

    public void InitTextUIData()
    {
        initText = true;
        textWidth = this.GetComponent<RectTransform>().rect.width;
        initSingleTextWidth = singleTextWidth = singleNumber.GetComponent<RectTransform>().rect.width;
    }

    public void SetText(double number)
    {
        this.StopNumberSlide();

        if (!Classic.UserManager.GetInstance().UserProfile().isOlderUser && Core.ApplicationConfig.GetInstance ().ShowCoinsMultiplier != 0) 
        {
            number *= Core.ApplicationConfig.GetInstance ().ShowCoinsMultiplier;
		}

        initCurNumber = (long)number;

        if(initText)
        {
            this.InitTextUIData();
            this.curNumber = (long)number;
            this.InitNumberText();
        }else
        {
            this.SetIncreaseNumber((long)number - this.curNumber);
            slideCoroutine = StartCoroutine(IncreaseTo((long)number));
        }
    }

    public void ChangeBetRefresh(double number)
    {
        this.SetCurNumber(number);
        if(isSlided && slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }
        this.RefreshNumberItem(this.ThousandSeparator(this.curNumber).ToCharArray(), false);
    }

    public void ReSetRefresh(double number, bool state = false)
    {
        this.SetCurNumber(number);
        if(isSlided && slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }
        this.RefreshNumberItem(this.ThousandSeparator(this.curNumber).ToCharArray(), state);
    }

    public void StopNumberSlide()
    {
        if(isSlided && slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            this.RefreshNumberItem(this.ThousandSeparator(this.initCurNumber).ToCharArray(), false);
        }
    }

    public void SetCurNumber(double number)
    {
        if (!Classic.UserManager.GetInstance().UserProfile().isOlderUser) 
        {
            this.curNumber = (long)number * Core.ApplicationConfig.GetInstance ().ShowCoinsMultiplier;
		}
    }    
    
    public void SetIncreaseNumber(long number)
    {
        increaseNumber = number / 10;
    }

    private IEnumerator IncreaseTo(long number)
    {
        isSlided = true;
        for (int i = 0; i < 10; i++)
        {
            this.curNumber += increaseNumber;
            this.RefreshNumberItem(this.ThousandSeparator(this.curNumber).ToCharArray(), true);
            if(increaseNumber != 0)
            {
                yield return waitSeconds;
            }            
        }
        isSlided = false;
    }

    public void RefreshNumberItem(char[] numberStr, bool slide)
    {
        int numberStrCount = numberStr.Length;
        int singleNumberTableCount = singleNumberTable.Count;
        if(numberStrCount == singleNumberTableCount)
        {
            for (int i = 0; i < singleNumberTable.Count; i++)
            {
                if(slide)
                {
                    singleNumberTable[i].SetSingleText(numberStr[i].ToString());
                }else
                {
                    singleNumberTable[i].SetText(numberStr[i].ToString());
                }
            }

        }else if(numberStrCount > singleNumberTableCount)
        {

            for (int i = 0; i < numberStrCount - singleNumberTableCount; i++)
            {
                GameObject numberObject = GameObject.Instantiate(singleNumber.gameObject, this.transform);
                SingleNumber single = numberObject.GetComponent<SingleNumber>();
                single.transform.SetAsFirstSibling();
                singleNumberTable.Insert(0, single);
            }
            
            this.SetTextUIData(numberStr);

            for (int i = 0; i < singleNumberTable.Count; i++)
            {
                singleNumberTable[i].transform.localPosition = new Vector2(this.GetNumberInterval(numberStr, i), 0);
                singleNumberTable[i].SetSingleScale(singleTextScale);
                singleNumberTable[i].GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetInterval(numberStr[i]), singleNumberTable[i].GetComponent<RectTransform>().rect.height);
                if(slide)
                {
                    singleNumberTable[i].SetSingleText(numberStr[i].ToString());
                }else
                {
                    singleNumberTable[i].SetText(numberStr[i].ToString());
                }
            }
        }else if(numberStrCount < singleNumberTableCount)
        {
            for (int i = 0; i < singleNumberTableCount - numberStrCount; i++)
            {
                Destroy(singleNumberTable[i].gameObject);
                singleNumberTable.RemoveAt(i);
            }

            this.SetTextUIData(numberStr);

            for (int i = 0; i < singleNumberTable.Count; i++)
            {
                singleNumberTable[i].transform.localPosition = new Vector2(this.GetNumberInterval(numberStr, i), 0);
                singleNumberTable[i].SetSingleScale(singleTextScale);
                singleNumberTable[i].GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetInterval(numberStr[i]), singleNumberTable[i].GetComponent<RectTransform>().rect.height);
                if(slide)
                {
                    singleNumberTable[i].SetSingleText(numberStr[i].ToString());
                }else
                {
                    singleNumberTable[i].SetText(numberStr[i].ToString());
                }
            }
        }
    }

    public string ThousandSeparator (long number)
    {
        if(number > 99) 
        {
            return StaticString.DefaultStaticStr.SetThousand (number);
		}else 
        {
            return number.ToString ();
		}
    }

    public void InitNumberText()
    {
        initText = false;
        char[] numberStr = this.ThousandSeparator(this.curNumber).ToString().ToCharArray();
        this.SetTextUIData(numberStr);
        for (int i = 0; i < numberStr.Length; i++)
        {
            GameObject numberObject = GameObject.Instantiate(singleNumber.gameObject, this.transform);
            SingleNumber single = numberObject.GetComponent<SingleNumber>();
            single.transform.localPosition = new Vector2(this.GetNumberInterval(numberStr, i), 0);
            single.SetSingleScale(singleTextScale);
            single.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetInterval(numberStr[i]), single.GetComponent<RectTransform>().rect.height);
            single.SetText(numberStr[i].ToString());
            singleNumberTable.Add(single);
        }
    }

    private float GetNumberInterval(char[] numberStr, int index)
    {
        float totalnterval = startPosX;


        if(index == 0)
        {
            return totalnterval;
        }

        for (int i = 0; i <= index; i++)
        {
            if(i == 0)
            {
                totalnterval = startPosX + this.GetInterval(numberStr[0])/2;
            }else if(i == index)
            {
                totalnterval += this.GetInterval(numberStr[index])/2;
            }else
            {
                totalnterval += this.GetInterval(numberStr[i]);
            }
        }
        return totalnterval;
    }

    public void SetTextUIData(char[] numberStr)
    {
        float totalWidth = 0;

        foreach (var str in numberStr)
        {
            totalWidth += this.GetInitInterval(str);
        }
        
        float scale = textWidth/totalWidth;

        if(scale < 1)
        {
            singleTextWidth = initSingleTextWidth * scale;
            singleTextScale = new Vector2(scale, scale);

            totalWidth = 0;
            foreach (var str in numberStr)
            {
                totalWidth += this.GetInterval(str);
            }
        }else
        {
            singleTextWidth = initSingleTextWidth;
            singleTextScale = Vector2.one;
        }
        switch(alignmentType)
        {
            case AlignmentType.Left :
                startPosX = -textWidth/2 + singleTextWidth/2;
            break;
            case AlignmentType.Middle :
                startPosX = -totalWidth/2 + singleTextWidth/2;
            break;
            case AlignmentType.Right :
                startPosX = textWidth/2 - totalWidth + singleTextWidth/2;
            break;
        }
    }

    public float GetInterval(char item)
    {
        if(item == '.' || item == ',')
        {
            return singleTextWidth/2;
        }
        return singleTextWidth;
    }

    public float GetInitInterval(char item)
    {
        if(item == '.' || item == ',')
        {
            return initSingleTextWidth/2;
        }
        return initSingleTextWidth;
    }

    public enum AlignmentType
    {
        Left,
        Middle,
        Right
    }
}
