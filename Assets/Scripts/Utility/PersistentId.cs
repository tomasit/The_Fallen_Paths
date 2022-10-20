using System.Linq;
using UnityEngine;
using System;

public class PersistentId : MonoBehaviour
{
    [HideInInspector][SerializeField] private UniqueID _id;

    public string ID
    {
        get { return _id.Value; }
    }

    [ContextMenu("Force reset ID")]
    private void ResetId()
    {
        _id.Value = Guid.NewGuid().ToString();
        Debug.Log("Setting new ID on object: " + gameObject.name, gameObject);
    }

    //Need to check for duplicates when copying a gameobject/component
    public static bool IsUnique(string ID)
    {
        return Resources.FindObjectsOfTypeAll<PersistentId>().Count(x => x.ID == ID) == 1;
    }

    protected void OnValidate()
    {
        if (!gameObject.scene.IsValid())
        {
            _id.Value = string.Empty;
            return;
        }

        if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
        {
            ResetId();
        }
    }

    [Serializable]
    private struct UniqueID
    {
        public string Value;
    }
}