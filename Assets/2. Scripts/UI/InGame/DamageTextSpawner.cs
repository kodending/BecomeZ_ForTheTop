using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public int poolSize = 30;

    private Queue<GameObject> pool = new();

    private void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, this.transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public void ShowDamage(Canvas worldCanvas, Vector3 worldPos, int damage, int atkResult)
    {
        GameObject obj = GetFromPool();
        obj.transform.SetParent(worldCanvas.transform);
        obj.transform.position = worldPos;

        DamageText dt = obj.GetComponent<DamageText>();
        dt.Init(damage, atkResult, ReturnToPool);
    }

    GameObject GetFromPool()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        // 부족할 경우 확장
        GameObject obj = Instantiate(damageTextPrefab, this.transform);
        obj.SetActive(false);
        return obj;
    }

    void ReturnToPool(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
