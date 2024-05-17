using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public static class MiscFunctions{
   public static T RandomEnumValue<T> ()
    {
        System.Random rnd = new System.Random();
        var v = System.Enum.GetValues (typeof (T));
        return (T) v.GetValue (rnd.Next(v.Length));
    }

    public static int GetPercentage(int main,float percent){
        float q = (percent / 100f) * (float)main;
        return (int)q;
    }

    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    // public static List<T> ShuffleList<T>(List<T> l) broke
    // {
    //     List<T> X = new List<T>();
    //     System.Random rng = new System.Random();
    //     return X.OrderBy(_ => rng.Next()).ToList();
    // }

    public static bool FiftyFifty(){
        return Random.Range(0,2) == 1;
    }

    public static void RandomYRotation(Transform t){
        t.rotation = Quaternion.Euler(t.transform.rotation.eulerAngles.x,Random.Range(0,360),t.transform.rotation.eulerAngles.z);
    }

    public static Vector3 FindCenterOfTransforms(List<Transform> transforms)
    {
        var bound = new Bounds(transforms[0].position, Vector3.zero);
        for(int i = 1; i < transforms.Count; i++)
        {
            bound.Encapsulate(transforms[i].position);
        }
        return bound.center;
    }

    public static (GameObject,GameObject) FindInList(List<GameObject> GameObjectList)
    {
        float FurthestDistance = 0;
        GameObject FurthestObjectOne = null;
        GameObject FurthestObjectTwo = null;
        foreach(GameObject Object in GameObjectList)
        {
            for (int i = 0; i < GameObjectList.Count; i++)
            {
                float ObjectDistance = Vector3.Distance(GameObjectList[i].transform.position, Object.transform.position);
                if (ObjectDistance > FurthestDistance)
                {
                    FurthestObjectOne = Object;
                    FurthestObjectTwo = GameObjectList[i];
                    FurthestDistance = ObjectDistance;
                }
            }
        }
        return (FurthestObjectOne,FurthestObjectTwo);
    }
}
