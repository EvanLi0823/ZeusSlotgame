using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FitParticleLifeTime : MonoBehaviour
{

    public float StartLifeTime;

    public UnityEvent ParticleCallBack;

    [Header("Iphone_X")]
    public bool enableShapeRadius_X = false;
    public float ShapeRadius_X = 0f;
    public bool enableShapeAngle_X = false;
    public float ShapeAngle_X = 0f;
    public bool enableStartLifeTime_X = false;
    public float StartLifeTime_X = 0f;
    public bool enableStartSize_X = false;
    public float StartSize_X = 0f;

    [Header("Iphone_4_3")]
    public bool enableShapeRadius_4_3 = false;
    public float ShapeRadius_4_3 = 0f;
    public bool enableShapeAngle_4_3 = false;
    public float ShapeAngle_4_3 = 0f;
    public bool enableStartLifeTime_4_3 = false;
    public float StartLifeTime_4_3 = 0f;
    public bool enableStartSize_4_3= false;
    public float StartSize_4_3 = 0f;


    void Start () {

        ParticleSystem parSystem = GetComponent<ParticleSystem> ();

        if (parSystem != null && SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
            parSystem.Clear();
            FitParticleShapeModule_4_3(parSystem);
            parSystem.Play();
        }

        if (parSystem != null && IphoneXAdapter.IsIphoneX())
        {
            parSystem.Clear();
            FitParticleShapeModule_X(parSystem);
            parSystem.Play();
        }

        if(ParticleCallBack != null) ParticleCallBack.Invoke();

    }

    void FitParticleShapeModule_4_3(ParticleSystem ticleSystem)
    {
        if (ticleSystem == null)
        {
            return;
        }
        ParticleSystem.ShapeModule shape = ticleSystem.shape;
        ParticleSystem.MainModule module = ticleSystem.main;
        if (!shape.enabled || shape.shapeType != ParticleSystemShapeType.Cone)
        {
            return;
        }
        if (enableShapeRadius_4_3)
        {
            shape.radius = ShapeRadius_4_3;
        }
        
        if (enableShapeAngle_4_3)
        {
            shape.angle = ShapeAngle_4_3;
        }

        if (enableStartLifeTime_4_3)
        {
            module.startLifetime = StartLifeTime_4_3;
            StartLifeTime = StartLifeTime_4_3;
        }
        
        if (enableStartSize_4_3)
        {
            module.startSize = StartSize_4_3;
        }
    }

    void FitParticleShapeModule_X(ParticleSystem ticleSystem){
        if (ticleSystem == null)
        {
            return;
        }
        ParticleSystem.ShapeModule shape = ticleSystem.shape;
        ParticleSystem.MainModule module = ticleSystem.main;
        if (!shape.enabled || shape.shapeType != ParticleSystemShapeType.Cone)
        {
            return;
        }
        if (enableShapeRadius_X)
        {
            shape.radius = ShapeRadius_X;
        }

        if (enableShapeAngle_X)
        {
            shape.angle = ShapeAngle_X;
        }

        if (enableStartLifeTime_X)
        {
            module.startLifetime = StartLifeTime_X;
            StartLifeTime = StartLifeTime_X;
        }

        if (enableStartSize_X)
        {
            module.startSize = StartSize_X;
        }

    }

}
