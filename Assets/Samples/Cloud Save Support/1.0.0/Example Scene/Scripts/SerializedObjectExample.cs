using Newtonsoft.Json;
using UnityEngine;
using VARLab.CloudSave;

[CloudSaved]
[JsonObject(MemberSerialization.OptIn)]
public class SerializedObjectExample : MonoBehaviour, ICloudSerialized, ICloudDeserialized
{
    [JsonProperty]
    private float scaleValue;

    [JsonProperty]
    public Vector3 rotation;

    [SerializeField] private Vector3 angularVelocity = new(0.2f, 0.3f, 0.1f);

    void OnMouseOver()
    {
        transform.Rotate(angularVelocity);
    }

    void OnMouseDown()
    {
        scaleValue += 0.2f;
        SetScale();
    }

    public void OnDeserialize()
    {
        SetScale();
        SetRotation();
    }

    public void OnSerialize()
    {
        rotation = transform.rotation.eulerAngles;
    }

    private void SetScale()
    {
        transform.localScale = Vector3.one * (1 + scaleValue);
    }

    private void SetRotation()
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
}
