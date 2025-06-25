using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PayLineCustomConfig : MonoBehaviour {

    public Sprite sprite;
    public int payLinesPaddingLeft =0;
    public int payLinesPaddingRight= 0;
    public int payLinesPaddingTop= 0;
    public int payLinesPaddingBottom = 0;
    public Vector2 payLinesCellSize = new Vector2();
    public Vector2 payLinesSpacing = new Vector2();
    public int payLinesConstraintCount = 3;

    public int lineSymbolPaddingLeft = 0;
    public int lineSymbolPaddingRight = 0;
    public int lineSymbolPaddingTop = 0;
    public int lineSymbolPaddingBottom = 0;
    public Vector2 lineSymbolCellSize = new Vector2();
    public Vector2 lineSymbolSpacing = new Vector2();
    public int lineSymbolConstraintCount = 3;


    public Vector3 lineLocalPosition = new Vector3();
    public Vector2 lineSizeDelta = new Vector2();

    public Vector3 numberLessTenLocalPosition = new Vector3();
    public Vector2 numberLessTenSizeDelta = new Vector2();

    public Vector3 numberNotLessTenLocalPosition = new Vector3();
    public Vector2 numberNotLessTenSizeDelta = new Vector2();
}
