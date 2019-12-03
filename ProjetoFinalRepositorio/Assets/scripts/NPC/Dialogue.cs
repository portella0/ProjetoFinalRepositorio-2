﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    public Sprite image;

    [TextArea(3, 10)]
    public string[] sentences;
}
