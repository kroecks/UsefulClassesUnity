public class ScriptableObjectWithId : ScriptableObject
{
    public string SerializedGuid = null;

    [NonSerialized]
    private Guid _cachedGuid = Guid.Empty;
    
    public Guid definitionId => GetDefinitionId();
    
    public void ForceNewGuid()
    {
        SerializedGuid = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
    }
    
    public Guid GetDefinitionId()
    {
        if (_cachedGuid == Guid.Empty)
        {
            try
            {
                _cachedGuid = Guid.Parse(SerializedGuid);
            }
            catch (Exception e)
            {
                Debug.LogError($"Guid {SerializedGuid} for {name} is not ready! " + e);
                ForceNewGuid();
            }

        }
            
        return _cachedGuid;
    }
    
}

public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
    public class ScriptableObjectIdDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            if (string.IsNullOrEmpty(property.stringValue)) {
                Debug.LogError($"ScriptableObjectIdDrawer - Property Value {property.name} is Empty! Assigning new GUID!");
                property.stringValue = Guid.NewGuid().ToString();
                AssetDatabase.SaveAssets();
            }
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif