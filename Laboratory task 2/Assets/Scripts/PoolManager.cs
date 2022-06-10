using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    class Pool
    {
        private List<GameObject> inactive = new List<GameObject>();
        private GameObject prefab;
        public Pool(GameObject prefab) { this.prefab = prefab; }

        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            GameObject obj;
            if(inactive.Count == 0)
            {
                obj = Instantiate(prefab, pos, rot);
                obj.name = prefab.name;
                obj.transform.SetParent(Instance.transform);
            }
            else
            {
                obj = inactive[inactive.Count - 1];
                inactive.RemoveAt(inactive.Count - 1);
            }
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);
            return obj;
        }

        public void Despawn(GameObject obj)
        {
            obj.SetActive(false);
            inactive.Add(obj);
        }
    }

    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    void Init(GameObject prefab)
    {
        if(prefab != null && pools.ContainsKey(prefab.name)==false)
        {
            pools[prefab.name] = new Pool(prefab);
        }
    }
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        Init(prefab);
        return pools[prefab.name].Spawn(pos, rot);
    }
    public void Despawn(GameObject obj)
    {
        if (pools.ContainsKey(obj.name))
        {
            pools[obj.name].Despawn(obj);
        }
        else
            Destroy(obj);
    }
    
}
