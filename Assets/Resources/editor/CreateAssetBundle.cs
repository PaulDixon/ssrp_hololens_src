using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEditor;

using UnityEditorInternal;
//using UnityEngine.Windows;
using System.IO;
using UnityStandardAssets;



public class CreateAssetBundles
{
    /*
    [MenuItem("Assets/Build SSRP AssetBundles ")]
    static void BuildSSRPAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.none, BuildTarget.StandaloneWindows);
    }
    */

    [MenuItem("Assets/Build ALL AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}