using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBoxCollider : MonoBehaviour
{
    public LayerMask m_LayerMask;
    public bool isHit;
    private Collider _collider;
    private Rigidbody _rigidBody;
    public List<GameObject> excludeGO;
    // Start is called before the first frame update
    private void Awake()
    {
        excludeGO = new List<GameObject>();
        _collider = this.gameObject.GetComponent<BoxCollider>();
        if (_collider == null)
        {
            _collider = this.gameObject.AddComponent<BoxCollider>();
            _collider.isTrigger = true;
        }
        _rigidBody = this.gameObject.GetComponent<Rigidbody>();
        if (_rigidBody == null)
        {
            _rigidBody = this.gameObject.AddComponent<Rigidbody>();
            _rigidBody.useGravity = false;
        }
        isHit = false;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if ((m_LayerMask.value & (1 << other.gameObject.layer)) > 0)
            if (!excludeGO.Contains(other.gameObject))
            {
                isHit = true;
            }
    }

    private void OnDestroy()
    {
        DestroyImmediate(_collider);
    }
}
