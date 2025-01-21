using UnityEngine;

public class ListManager : MonoBehaviour
{
    public GameObject[] listOfObjects;

    private void Start()
    {
        listOfObjects = new GameObject[transform.childCount];

        for (var x = 0; x < transform.childCount; x++) listOfObjects[x] = transform.GetChild(x).gameObject;
    }

    public GameObject GetObjectByName(string name)
    {
        foreach (var obj in listOfObjects)
            if (obj.GetComponent<WeaponController>().weaponName == name)
                return obj;
        return null;
    }
}