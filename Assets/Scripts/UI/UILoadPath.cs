using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UILoadPath",menuName = "UILoadPath")]
public class UILoadPath : ScriptableObject
{
    public UIType uiType;
    public string uiPath;
}
