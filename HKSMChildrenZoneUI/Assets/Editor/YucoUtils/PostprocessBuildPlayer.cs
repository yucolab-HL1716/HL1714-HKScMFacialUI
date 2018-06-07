/* Copyright (C) YU & CO. (LAB) LIMITED - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Eric Koo <eric.koo@yucolab.com>, May 2013
 */

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;

public class PostprocessBuildPlayer : MonoBehaviour
{
	[PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string executablePath) {
		/*string directoryPath = Path.GetDirectoryName(executablePath);

        var directories = Directory.GetDirectories(Application.dataPath + "/Resources/");
        foreach(string src_dir in directories)
        {
            string dirName = Path.GetFileName(src_dir);
            // Copy INI file
            string dest_dir = directoryPath + "/" + Application.productName + "_Data/Resources/" + dirName;
            Debug.Log("OnPostprocessBuild: Copying directory " + src_dir + " to " + dest_dir);
            try
            {
                FileUtil.CopyFileOrDirectory(src_dir, dest_dir);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
            }
            Debug.Log("OnPostprocessBuild: Copied folder " + src_dir + " to " + dest_dir);
        }*/
    }
	
}
