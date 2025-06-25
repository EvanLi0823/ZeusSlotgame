using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//主要修改maskSprite的时候material和自己的material匹配
public class MaskStencilSet : MonoBehaviour
{
    public MeshRenderer m_FromMeshRender;
    public Material m_ToMaterial;
    public Text m_Text;


    private void Start()
    {
        if(m_FromMeshRender!=null)
        { 
            float v = m_FromMeshRender.materials[0].GetFloat("_Stencil");

            Material m = new Material(m_ToMaterial);

            m.SetFloat("_Stencil", v);
            m_Text.material = m;
        }
    }
}
