﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kandooz.KVR;

using Kandooz.KVR.Editors;

[UnityEditor.AssetImporters.ScriptedImporter(1,"KinterActionHandData")]
public class InputManagerImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        var managerIDs=AssetDatabase.FindAssets("t:VRInputManager");

        for (int i = 0; i < managerIDs.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(managerIDs[i]);
            var manager=AssetDatabase.LoadAssetAtPath<UnityAxisBasedVRInputManager>(path);
            VRInputManagerEditor.SeedInputs(manager);
        }
    }
}
