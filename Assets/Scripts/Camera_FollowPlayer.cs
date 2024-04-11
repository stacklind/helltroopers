using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;

    private void Awake()
    {
        transform.parent = playerTransform;
        transform.localPosition = new Vector3 (xOffset, yOffset, zOffset);
    }
}
