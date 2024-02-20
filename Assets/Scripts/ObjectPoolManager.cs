using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ObjectPool
{
    public ObjectPoolTag tag;
    public int quantity;
    public PoolableObject prefab;
}
public enum ObjectPoolTag{BATTLENUMBER,CORPSE}
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public List<ObjectPool> pools = new List<ObjectPool>();
    public GenericDictionary<ObjectPoolTag,List<PoolableObject>> dict = new GenericDictionary<ObjectPoolTag,List<PoolableObject>>();
    public GenericDictionary<ObjectPoolTag,Queue<PoolableObject>> Qdict = new GenericDictionary<ObjectPoolTag,Queue<PoolableObject>>();
    public void Start()
    {
        foreach (var item in pools)
        {
            dict.Add(item.tag,new List<PoolableObject>());
            Qdict.Add(item.tag,new Queue<PoolableObject>());
        }

        foreach (var item in pools)
        {
            for (int i = 0; i < item.quantity; i++)
            {
                PoolableObject go = Instantiate(item.prefab);
                dict[item.tag].Add(go);
                Qdict[item.tag].Enqueue(go);
                go.gameObject.SetActive(false);
            }
        }
    }

    public T Get<T>(ObjectPoolTag tag)
    {
        if(Qdict.ContainsKey(tag))
        {
            if(Qdict[tag].Count == 0)
            {
                Reset(tag);
                return Get<T>(tag);
            }
            else
            {
                
                return  (T)Convert.ChangeType(Qdict[tag].Dequeue(), typeof(T));
            }
        }
        else
        {
            Debug.LogAssertion(tag + " NOT FOUND!!!");
            return default(T);
        }
    }

    void Reset(ObjectPoolTag tag)
    {
        Qdict[tag].Clear();
        foreach (var item in dict[tag])
        {
            item.Reset();
            //item.Reposition(Vector3.zero,Quaternion.identity);
            Qdict[tag].Enqueue(item);
        }
    }

}