///////////////////////////////////////////////////////////////////////////////////////////////////
// Author      : LiNan
// Description : Entities
// Department  : XDTown Client / Gameplay-Entity
///////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;

namespace XDTGame.Core;

/// <summary>
/// 安全迭代器(单例)
/// 用于一个容器想要迭代，同时又修改的情况
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public readonly struct MapIterator<TKey,TValue> : IDisposable
{
    static bool Interating;
    static readonly Dictionary<TKey, TValue> s_map = new ();
    
    public MapIterator(Dictionary<TKey,TValue> dictionary)
    {
        if (Interating)
        {
            throw new InvalidOperationException($"MapIterator<{typeof(TKey).Name},{typeof(TValue).Name}> is interating !");
        }
        
        if(null == dictionary)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        
        Interating = true;
        foreach (var keyValue in dictionary)
            s_map.Add(keyValue.Key, keyValue.Value);
    }
    
    public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
    {
        return s_map.GetEnumerator();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
        if (Interating)
        {
            Interating = false;
            s_map.Clear();
        }
    }
}