using System;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class SpineResourceAsset
{
    public int animationId = 1;
    public string animationName = "";
    public string animationSkin = "default";
    public bool animationClip = false;

    public List<SkeletonDataAsset> skeletonDataAsset = new List<SkeletonDataAsset>();

    public SpineResourceAsset(int id = 1, string name = "", string skin = "default", bool clip = false)
    {
        this.animationId = id;
        this.animationName = name;
        this.animationSkin = skin;
        this.animationClip = clip;
    }

    public void AddSkeletonDataAsset(SkeletonDataAsset dataAsset)
    {
        skeletonDataAsset.Add(dataAsset);
    }
}
