using System;
using System.Collections.Generic;
using System.IO;
using CagrsLib.LibCore;
using Newtonsoft.Json;
using UnityEngine;

namespace CagrsLib.Archive
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CagrsLib/Archive/ArchiveManager")]
    public class ArchiveManager : MonoBehaviour
    {
        public static ArchiveManager GetInstance()
        {
            return FindObjectOfType<ArchiveManager>();
        }
        
        public PathType dataPosition = PathType.PersistentData;

        public string relativePath;
        
        private JsonSerializerSettings _jsonSerializerSetting = new()
        {
            TypeNameHandling = TypeNameHandling.All
            // ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        private void Reset()
        {
            dataPosition = PathType.Data;
            
            relativePath = "/MyGameArchive/data_slot_1/";
        }

        public void WriteAll()
        {
            LibUtil.Log(this, "Writing ......");
            ArchiveData[] archiveDatas = FindObjectsOfType<ArchiveData>();
            
            foreach (var data in archiveDatas)
            {
                int packCount = 0;
                List<ArchivePack> archivePacks = new List<ArchivePack>();
                foreach (var archivePack in data.WriteArchive())
                {
                    packCount++;
                    archivePacks.Add(archivePack);
                }
                LibUtil.WriteFile(GetFile(data.dataGuid), Serialize(archivePacks));
                LibUtil.Log(this, $"Written {packCount} packs to {GetFile(data.dataGuid).FullName}");
            }
            LibUtil.Log(this, "[ WRITTEN ]");
        }
        
        public void LoadAll()
        {
            LibUtil.Log(this, "Loading ......");
            ArchiveData[] archiveDatas = FindObjectsOfType<ArchiveData>();
            Dictionary<string, List<ArchivePack>> dictionary = new Dictionary<string, List<ArchivePack>>();

            int fileCount = 0;
            foreach (var fileInfo in GetFolder().GetFiles())
            {
                List<ArchivePack> pack = (List<ArchivePack>)Deserialize(
                    LibUtil.ReadFile(fileInfo));
                
                dictionary.Add(fileInfo.Name, pack);

                fileCount ++;
            }
            LibUtil.Log(this, $"Found {fileCount} files from {GetFolder().FullName}");

            foreach (var data in archiveDatas)
            {
                if (! dictionary.ContainsKey(data.dataGuid)) continue;
                
                List<ArchivePack> part = dictionary[data.dataGuid];
                Dictionary<string, ArchivePack> packDictionary = new();

                foreach (var item in part)
                {
                    packDictionary.Add(item.packName, item);
                }
                
                data.LoadArchive(packDictionary);
            }
            LibUtil.Log(this, "[ LOADED ]");
        }

        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSetting);
        }
        
        private object Deserialize(string str)
        {
            return JsonConvert.DeserializeObject(str, _jsonSerializerSetting);
        }

        private string GetPrefix()
        {
            string prefix = "";

            switch (dataPosition)
            {
                case PathType.Data:
                    prefix = Application.dataPath;
                    break;
                case PathType.Cache:
                    prefix = Application.temporaryCachePath;
                    break;
                case PathType.PersistentData:
                    prefix = Application.persistentDataPath;
                    break;
                case PathType.StreamingAssets:
                    prefix = Application.streamingAssetsPath;
                    break;
            }

            return prefix;
        }

        public DirectoryInfo GetFolder()
        {
            DirectoryInfo path = new DirectoryInfo(GetPrefix() + "/" + relativePath);
            
            if (! path.Exists)
            {
                path.Create();
            }

            return path;
        }

        private FileInfo GetFile(string fileName)
        {
            return new FileInfo(GetPrefix() + "/" + relativePath + "/" + fileName);
        }

        [Serializable]
        public enum PathType
        {
            Data, StreamingAssets, PersistentData, Cache 
        }
    }
}
