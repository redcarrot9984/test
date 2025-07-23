using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    // このリストはprivateに戻してもOKですが、他の場所で使っているならpublicのままで大丈夫です
    public List<GameObject> placedGameObjects = new();

    // ★★戻り値の型を「int」から「GameObject」に変更★★
    // 戻り値の型を「GameObject」から「int」に戻します
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // プレハブをシーンに生成
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;

        // Constructableなどの初期化処理
        Constructable constructable = newObject.GetComponent<Constructable>();
        if (constructable != null)
        {
            constructable.ConstructableWasPlaced();
        }

        // 生成したオブジェクトをリストに追加
        placedGameObjects.Add(newObject);

        // ★★ リストの最後の番号（＝今追加したオブジェクトの管理番号）を返す ★★
        return placedGameObjects.Count - 1;
    }
    public void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex 
           || placedGameObjects[gameObjectIndex] == null)
            return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
    // ... RemoveObjectAtメソッドは変更なし ...
}