using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class NameArrayObject : SerializedScriptableObject
{
    [SerializeField]
    public string[] Names;
}