using System;
using System.Collections.Generic;
using UnityEngine;
using XD.Unity.Game;
using XDFramework.Core;
using XDTGame.Framework;
using Object = UnityEngine.Object;

namespace XDTGame.Core;

internal class GameObjectPool
{
    class GameObjectPoolElement
    {
        readonly GameObject original = null;

        readonly Transform root = null;

        public GameObjectPoolElement(string type, GameObject original = null)
        {
            if (type == "")
                root = new GameObject("Default_Pool").transform;
            else
                root = new GameObject(type + "_Pool").transform;

            root.gameObject.SetActive(false);

            root.transform.parent = GameApp.Inst.transform;

            if (original)
            {
                this.original = Object.Instantiate(original);
                this.original.transform.parent = root;
            }
        }

        Stack<Transform> poolData = new Stack<Transform>();

        public Transform Fetch()
        {
            if (!M<IObjectPoolManager>.Inst.PoolOpen)
            {
                GameObject go = null;

                if (original == false)
                    go = new GameObject();
                else
                    go = Object.Instantiate(original);

                return go.transform;
            }


            if (poolData.Count == 0)
            {
                for (int i = 0; i < 20; ++i)
                {
                    GameObject go = null;
                    if (original == false)
                        go = new GameObject();
                    else
                        go = Object.Instantiate(original);

                    go.transform.parent = root;
                    poolData.Push(go.transform);
                }
            }

            int count = poolData.Count;
            var v = poolData.Pop();
            
            // make sure Fetch element correctly.
            if (v == false)
            {
                if (count > 0)
                {
                    DebugSystem.LogError( LogCategory.Framework, $"GameObjectPool_{root.name} maybe has some invalid object.");
                }
                
                GameObject go = null;
                if (original == false)
                    go = new GameObject();
                else
                    go = Object.Instantiate(original);

                return go.transform;
            }

            v.parent = null;
            return v;
        }

        public void Release(Transform go)
        {
            if (go == true)
            {
                if (!M<IObjectPoolManager>.Inst.PoolOpen)
                {
                    Object.Destroy(go.gameObject);
                    return;
                }

                go.transform.parent = root;
                poolData.Push(go);
            }
        }

        public void Dispose()
        {
            Object.Destroy(root.gameObject);
            if (original)
                Object.Destroy(original.gameObject);

            foreach (Transform t in poolData)
            {
                if (t)
                    Object.Destroy(t.gameObject);
            }

            poolData.Clear();
        }

        public void Clear()
        {
            //Object.Destroy(root.gameObject);
            foreach (Transform t in poolData)
            {
                if (t)
                    Object.Destroy(t.gameObject);
            }

            poolData.Clear();
        }
    }

    readonly Dictionary<string, GameObjectPoolElement> gameObjectPools =
        new Dictionary<string, GameObjectPoolElement>();


    GameObjectPoolElement defaultGameObjectPool = new GameObjectPoolElement("");

    public GameObjectPool()
    {
    }

    public void Dispose()
    {
        foreach (var item in gameObjectPools)
        {
            item.Value.Dispose();
        }

        gameObjectPools.Clear();

        defaultGameObjectPool.Dispose();
    }

    public void Purge()
    {
        foreach (var item in gameObjectPools)
        {
            string name = item.Key;
            item.Value.Clear();
        }

        defaultGameObjectPool.Clear();
    }

    public void Prepare(string type, GameObject original)
    {
        if (gameObjectPools.ContainsKey(type))
            return;

        gameObjectPools.Add(type, new GameObjectPoolElement(type, original));
    }

    public Transform Fetch(string type, string name)
    {
        if (gameObjectPools.TryGetValue(type, out GameObjectPoolElement result))
        {
            var value = result.Fetch();
            value.name = name;
            return value;
        }

        return null;
    }

    public Transform Fetch(string name)
    {
        var value = defaultGameObjectPool.Fetch();
        if ("" != name)
            value.name = name;
        return value;
    }

    public void Release(Transform go, string type)
    {
        if (!go)
        {
            throw new Exception(  "Release invalid object. check your code");
        }
        
        if (gameObjectPools.TryGetValue(type, out GameObjectPoolElement result))
        {
            result.Release(go);
        }
        else
        {
            Object.Destroy(go.gameObject);
        }
    }

    public void Release(Transform go)
    {
        if (!go)
        {
            throw new Exception(  "Release invalid object. check your code");
        }
        
        defaultGameObjectPool.Release(go);
    }
}
