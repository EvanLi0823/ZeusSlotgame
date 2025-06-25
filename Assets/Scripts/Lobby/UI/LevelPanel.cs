using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Plugins;
using TMPro;
using UnityEngine;

namespace Classic
{
    public class LevelPanel : MonoBehaviour
    {
        public TextMeshProUGUI LevelTest;
        public const string LevelKey = "Level.";

        public void SetLevel(int level)
        {
            LevelTest.text = LevelKey + level;
        }
    }
}

