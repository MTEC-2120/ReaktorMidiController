using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SgLib
{
    public class EditorTools
    {
        [MenuItem("Tools/Reset PlayerPrefs", false)]
        public static void ResetPlayerPref()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("*** PlayerPrefs was reset! ***");
        }
    }
}
