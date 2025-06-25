
using Spine.Unity;

public class SpineContinueAnimation : SpineAnimation
{
    
    public void PlayAnimation(string first,string second)
    {
        SkeletonDataAsset skeletonDataAsset = Skeleton.skeletonDataAsset;
        System.Action callBack = () => {this.Play(skeletonDataAsset,second); };
        this.Play(skeletonDataAsset,first,false,0,1,null,callBack);
    }
    
}