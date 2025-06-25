using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PayLinesCreator))]
public class PayTableObjectBuilderEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PayLinesCreator payLinesCreator = (PayLinesCreator)target;
        if (GUILayout.Button("Gen PayTable"))
        {
            payLinesCreator.GeneratePayTable();
        }
    }
}
