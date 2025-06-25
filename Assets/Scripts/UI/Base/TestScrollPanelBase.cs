using UnityEngine;
using UnityEngine.UI;
using Libs;

namespace Classic
{
public class TestScrollPanelBase : UIDialog 
{
		[HideInInspector]
    public UnityEngine.UI.HorizontalSrollRect HorizontalScrollView;

    protected override void Awake()
    {
        base.Awake();
        this.HorizontalScrollView = Util.FindObject<UnityEngine.UI.HorizontalSrollRect>(transform,"HorizontalScrollView/");
    }
}}
