using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosionPool : MonoBehaviour
{
    [SerializeField] GameObject poolObject;
    List<GameObject> poolObjectList;
    const int MAXCOUNT = 20;//20個のオブジェクトが最大

    void Start()
    {
        CreatePool();
    }

    //オブジェクトプールの作成
    public void CreatePool()
    {
        poolObjectList = new List<GameObject>();
        for (int i = 0; i < MAXCOUNT; i++)
        {
            GameObject newObject = CreateNewObject();
            newObject.SetActive(false);
            poolObjectList.Add(newObject);
        }
    }


    //リスト内のオブジェクトを返す、足りない場合は新しくオブジェクトをプールする
    public GameObject GetObject()
    {
        foreach (GameObject obj in poolObjectList)
        {
            if (obj.activeSelf == false)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObject = CreateNewObject();
        poolObjectList.Add(newObject);
        newObject.SetActive(true);
        return newObject;
    }

    //オブジェクトをプールする、名前を付けて管理
    public GameObject CreateNewObject()
    {
        Vector2 pos = new Vector3(0, 0, -50);
        GameObject newObject = Instantiate(poolObject, pos, Quaternion.identity);
        newObject.name = poolObject.name + (poolObjectList.Count + 1);
        newObject.transform.parent = GameObject.Find("BulletExplosionPool").transform;
        return newObject;
    }
}
