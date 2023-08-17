using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSnakeBoss : MonoBehaviour
{
    [SerializeField] private Transform circleMiddlePoint;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int bodyLength;
    [SerializeField] private float gapDistanceMultiplier;
    [SerializeField] private float smoothSpeed;

    [SerializeField] private float circleMovementSpeed;
    [SerializeField] private Vector3 circleSize;
    [SerializeField] private Vector3 timeMultiplayer;

    private List<Transform> bodyParts = new();
    private Vector3[] segmentV;
    private Vector3[] positions;

    private float circleTimeCounter;

    private Rigidbody rb;
    private SphereCollider sphereCol;
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        CircleMovement();
        //MainPartMovement();
        BodyPartsMovement();
    }

    private void Initialize()
    {
        sphereCol = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        bodyParts.Add(this.transform);
        SpawnBodyParts(bodyLength - 1);
        segmentV = new Vector3[bodyLength];
        positions = new Vector3[bodyLength];
    }


    private void CircleMovement()
    {
        circleTimeCounter += Time.deltaTime * circleMovementSpeed;
        float x = Mathf.Cos(circleTimeCounter * timeMultiplayer.x) * circleSize.x;
        float y = Mathf.Sin(circleTimeCounter * timeMultiplayer.y) * circleSize.y;
        float z = Mathf.Sin(circleTimeCounter * timeMultiplayer.z) * circleSize.z;
        y = Mathf.Clamp(y, 0, circleSize.y);
        transform.position = new Vector3(circleMiddlePoint.position.x + x, circleMiddlePoint.position.y + y, circleMiddlePoint.position.z + z);
    }

    private void SpawnBodyParts(int _partNum)
    {
        for (int i = 0; i < _partNum; i++)
        {
            GameObject go = Instantiate(transform.gameObject);
            float newScale = transform.localScale.x / bodyLength;
            newScale = (bodyLength - i) * newScale;
            go.transform.localScale = new Vector3(newScale, newScale, newScale);
            go.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            Destroy(go.GetComponent<StoneSnakeBoss>());
            bodyParts.Add(go.transform);
        }
    }

    private void MainPartMovement()
    {
        transform.Rotate(new Vector3(0,1,0), Time.deltaTime * movementSpeed * 1.3f);
        rb.AddForce(GetSlopeDirection().normalized * movementSpeed, ForceMode.Force);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (velocity.magnitude > movementSpeed)
        {
            Vector3 limit = velocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }

        if (!IsGrounded() && rb.velocity.y < 0) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 1.1f, rb.velocity.z);

        if (rb.velocity.y < -50) rb.velocity = new Vector3(rb.velocity.x, -50, rb.velocity.z);
    }

    private void BodyPartsMovement()
    {
        positions[0] = transform.position;

        for (int i = 1; i < positions.Length; i++)
        {
            float targetDistance = (bodyParts[i - 1].gameObject.GetComponent<SphereCollider>().bounds.size.x + bodyParts[i].gameObject.GetComponent<SphereCollider>().bounds.size.x) / 2 / gapDistanceMultiplier;
            Vector3 targetPos = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * targetDistance;
            positions[i] = Vector3.SmoothDamp(positions[i], targetPos, ref segmentV[i], smoothSpeed);
            bodyParts[i].transform.position = positions[i - 1];
        }
    }

    private bool IsGrounded()
    {
        float sphereSize = sphereCol.radius * transform.localScale.y - 0.02f;
        float capsuleRadius = sphereCol.radius * transform.localScale.y;
        Vector3 spherePosition = sphereCol.bounds.min + new Vector3(capsuleRadius, 0, capsuleRadius) + new Vector3(0, capsuleRadius - 0.15f, 0);

        bool _isGrounded = Physics.CheckSphere(spherePosition, sphereSize, whatIsGround);

        return _isGrounded;
    }

    private Vector3 GetSlopeDirection()
    {
        Vector3 direction = rb.transform.forward;
        Vector3 raycastPoint = sphereCol.bounds.center;
        float raySize = 20;

        if (Physics.Raycast(raycastPoint, -transform.up, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            Vector3 forward = rb.transform.forward;
            Vector3 up = transform.up;
            Vector3 right = Vector3.Cross(up.normalized, forward.normalized);
            direction = Vector3.Cross(right, hitinfo.normal).normalized;
        }
        
        return direction;
    }
}
