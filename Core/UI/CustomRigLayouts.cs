#if !UNITY_EDITOR
using EFT.UI.DragAndDrop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace PackNStrap.Core.UI
{
    // Thank you SSH for finally figuring this shit out :)

    internal abstract class CustomRigLayouts
    {
        public static void LoadRigLayouts()
        {
            string rigLayoutsDirectory = Path.Combine(PackNStrap.PluginPath, "WTT-PackNStrap", "Layouts");


            if (!Directory.Exists(rigLayoutsDirectory))
            {
                Console.WriteLine("Rig layouts directory not found.");
                return;
            }

            var rigLayoutBundles = Directory.GetFiles(rigLayoutsDirectory, "*.bundle");

            foreach (var rigLayoutBundleFile in rigLayoutBundles)
            {
                string bundleName = Path.GetFileNameWithoutExtension(rigLayoutBundleFile);

                AssetBundle rigLayoutBundle = AssetBundle.LoadFromFile(rigLayoutBundleFile);

                if (rigLayoutBundle == null)
                {
                    Console.WriteLine($"Failed to load rig layout bundle: {bundleName}");
                    continue;
                }


                GameObject[] prefabs = rigLayoutBundle.LoadAllAssets<GameObject>();

                foreach (var prefab in prefabs)
                {


                    if (prefab == null)
                    {
                        Console.WriteLine($"Failed to load rig layout prefab from bundle: {prefab.name}");
                        continue;
                    }


                    ContainedGridsView gridView = prefab.GetComponent<ContainedGridsView>();

                    if (gridView == null)
                    {
                        Console.WriteLine($"Rig layout prefab {prefab} is missing ContainedGridsView component.");
                        continue;
                    }

                    AddEntryToDictionary($"UI/Rig Layouts/{prefab.name}", gridView);
                }

                rigLayoutBundle.Unload(false);
            }
        }

        private static void AddEntryToDictionary(string key, object value)
        {
            CacheResourcesPopAbstractClass.dictionary_0.Add(key, value);
#if DEBUG
            Console.WriteLine($"Successfully added new rig layout {key} to resources dictionary!");
#endif
        }
    }
}

#endif