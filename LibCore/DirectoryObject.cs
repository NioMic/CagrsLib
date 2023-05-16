using System;
using System.Collections.Generic;

namespace CagrsLib.LibCore
{
    [Serializable]
    public class DirectoryObject<T>
    {
        public Dictionary<string, T> Objects;
        public Dictionary<string, DirectoryObject<T>> Directories;

        private char split;

        public static DirectoryObject<T> Create(
            char split = '/'
            )
        {
            return new DirectoryObject<T>
            {
                Objects = new Dictionary<string, T>(),
                Directories = new Dictionary<string, DirectoryObject<T>>(),
                split = split
            };
        }

        public DirectoryObject<T> QuickPut(string path, string objName, T obj)
        {
            DirectoryObject<T> putIn = this;

            foreach (var directory in path.Trim().Split('/'))
            {
                var dir = directory.Trim();
                
                if (dir == "")
                {
                    putIn.AddObject(objName, obj);
                    return this;
                }

                if (putIn.Directories == null) continue;
                if (putIn.Directories.ContainsKey(dir))
                {
                    putIn = putIn.Directories[dir];
                }
                else
                {
                    putIn = putIn.CreateDirectory(dir);
                }
            }
            
            putIn.AddObject(objName, obj);
            
            return this;
        }

        public DirectoryObject<T> CreateDirectory(string name)
        {
            DirectoryObject<T> directoryObject = Create();
            Directories.Add(ParseString(name), directoryObject);
            return directoryObject;
        }
        
        public void AddObject(string objName, T obj)
        {
            Objects.Add(ParseString(objName), obj);
        }
        
        public void AddObjectToDirectory(string dicName, string objName, T obj)
        {
            Directories[dicName].AddObject(ParseString(objName), obj);
        }

        private string ParseString(string content)
        {
            return content
                .Replace("\n", "")
                .Replace("\t", "")
                .Replace("\b", "")
                .Replace("\r", "")
                .Replace("\\", "")
                .Replace(split.ToString(), "")
                .Trim();
        }

         public string[] ListTree(bool ignoreDir = false,
             bool ignoreObj = false,
             bool objectFirst = false,
             TreeOrder order = TreeOrder.Descending,
             TreeSearchMode searchMode = TreeSearchMode.DFS
             , IDirectoryTreeEvent<T> IdirectoryTreeEvent = null)
         {
             List<string> result = new List<string>();
             
             if (searchMode == TreeSearchMode.DFS)
             {
                 // 深度优先递归
                 RecursionTreeDFS(result, "", 0, this, objectFirst, ignoreDir, ignoreObj, order, IdirectoryTreeEvent);
             }
             else if (searchMode == TreeSearchMode.BFS)
             {
                 // 广度优先递归
                 // TODO :: 还没写
             }
             
             return result.ToArray();
         }

         private void RecursionTreeDFS(List<string> list,
             string prefix,
             int depth,
             DirectoryObject<T> entry,
             bool objectFirst,
             bool ignoreDir,
             bool ignoreObj,
             TreeOrder order,
             IDirectoryTreeEvent<T> idirectoryTreeEvent)
         {
             if (idirectoryTreeEvent == null) idirectoryTreeEvent = new SimpleDirectoryTreeEvent();

             if (objectFirst) TraverseAllObjects(list, prefix, depth, entry, ignoreObj, order, idirectoryTreeEvent);
             
             foreach (var dir in entry.Directories)
             {
                 if (!ignoreDir)
                 {
                     AddToList(list, order, idirectoryTreeEvent.OnDirectory(depth, dir.Key, prefix, split));
                 }
                 
                 RecursionTreeDFS(list, prefix + dir.Key + split, depth + 1, dir.Value, objectFirst, ignoreDir, ignoreObj, order, idirectoryTreeEvent);
             }
             
             if (!objectFirst) TraverseAllObjects(list, prefix, depth, entry, ignoreObj, order, idirectoryTreeEvent);
         }

         private void TraverseAllObjects(List<string> list,
             string prefix,
             int depth,
             DirectoryObject<T> entry,
             bool ignoreObj,
             TreeOrder order,
             IDirectoryTreeEvent<T> idirectoryTreeEvent)
         {
             foreach (var obj in entry.Objects)
             {
                 if (!ignoreObj)
                 {
                     AddToList(list, order, idirectoryTreeEvent.OnObject(depth, obj.Key, prefix, split, obj.Value));
                 }
             }
         }

        private void AddToList(List<string> list, TreeOrder order, string item)
        {
            // 降序
            if (order == TreeOrder.Descending)
            {
                list.Add(item);
            }
            // 升序
            else if (order == TreeOrder.Ascending)
            {
                list.Insert(0, item);
            }
        }

        public enum TreeOrder
        {
            Ascending, 
            Descending 
        }

        public enum TreeSearchMode
        {
            DFS, BFS
        }
        
        public class SimpleDirectoryTreeEvent : IDirectoryTreeEvent<T>
        {
            public string OnDirectory(int depth, string dicName, string fromDic, char split)
            {
                return fromDic + dicName;
            }

            public string OnObject(int depth, string objName, string fromDic, char split, T obj)
            {
                return fromDic + objName;
            }
        }
    }
    
    public interface IDirectoryTreeEvent<in T>
    {
        string OnDirectory(int depth, string dicName, string fromDic, char split);

        string OnObject(int depth, string objName, string fromDic, char split, T obj);
    }
}