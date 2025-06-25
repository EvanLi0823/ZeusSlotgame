using UnityEngine;
using System.Collections;

public class FitParticleSize : MonoBehaviour {
	public float ipad_size = 6.2f;
    public bool enableShapeRadius = false;
    public float ipad_Radius = 4f;

    public bool enableShapeBox_X = false;
    public float ShapeBox_X = 0f;
    public bool enableShapeBox_Y = false;
    public float ShapeBox_Y = 0f;
	void Start () {
		ParticleSystem particleSystem = GetComponent<ParticleSystem> ();

		if (particleSystem !=null && SkySreenUtils.GetScreenSizeType () == SkySreenUtils.ScreenSizeType.Size_4_3) {
			particleSystem.Stop();
			particleSystem.Clear();
			var main = particleSystem.main;
			main.prewarm = false;
			particleSystem.startSize = ipad_size;
            FitParticleShapeModule(particleSystem);

            if (enableShapeRadius){
                ParticleSystem.ShapeModule tempShape = particleSystem.shape;
                tempShape.radius = ipad_Radius;
            }
           
			particleSystem.Play();
		}
	}

    void FitParticleShapeModule(ParticleSystem particleSystem){
        if (particleSystem==null)
        {
            return;
        }
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        if (!shape.enabled || shape.shapeType != ParticleSystemShapeType.Box)
        {
            return;
        }
        if (enableShapeBox_X)
        {
            shape.scale = new Vector3(ShapeBox_X, shape.scale.y, shape.scale.z);
        }
        if (enableShapeBox_Y)
        {
            shape.scale = new Vector3(shape.scale.x, ShapeBox_Y, shape.scale.z);
        }
    }

}
