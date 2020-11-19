using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PA6Menu
{
    [MenuItem("PA6/Randomize Rotations")]
    public static void RandomizeRotation() {
        foreach(GameObject obj in Selection.gameObjects) {
            obj.transform.rotation = Random.rotationUniform;
        }
    }

    [MenuItem("PA6/Randomize Positions")]
    public static void RandomizePositions() {
        float radius = 1000f;
        foreach (GameObject obj in Selection.gameObjects) {
            Vector3 pos = Random.insideUnitSphere * radius;
            pos.y = 0;
            obj.transform.position = pos;
        }
    }
}
