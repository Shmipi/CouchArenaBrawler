using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScript : MonoBehaviour
{
    public Rigidbody rb;
    public Collider body;

    private Transform holdingTransform;
    [SerializeField] private LayerMask playerLayer;

    private bool isHeld;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        body = GetComponent<Collider>();

        body.enabled = true;

        isHeld = false;
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            transform.position = holdingTransform.position;
            transform.rotation = holdingTransform.rotation;
        }
    }

    public void PickUp(Transform transform)
    {
        isHeld = true;
        isGrounded = false;
        holdingTransform = transform;
        body.enabled = false;
    }

    public void ThrowWeapon(Transform tempTransform)
    {
        isHeld = false;
        holdingTransform = null;
        body.enabled = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        rb.excludeLayers = playerLayer;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        Vector3 hurl = 10 * 10 * transform.forward;
        rb.AddForce(hurl, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wall" || other.tag == "floor")
        {
            isGrounded = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.excludeLayers = playerLayer;
        }
    }
}
