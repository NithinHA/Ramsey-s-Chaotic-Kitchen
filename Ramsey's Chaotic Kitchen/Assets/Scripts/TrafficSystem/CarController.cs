using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float m_StoppingDist = .5f;
    [SerializeField] private float m_MovementSpeed = 5f;
    [SerializeField] private float m_RotationSpeed = 60f;

    private Vector3 _destination;
    public bool ReachedDestination;

    void Start()
    {
        
    }

    void Update()
    {
        if(transform.position != _destination)
        {
            Vector3 destinationDir = _destination - transform.position;
            destinationDir.y = 0;
            float destinationDist = destinationDir.magnitude;
            if(destinationDist >= m_StoppingDist)
            {
                ReachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * m_MovementSpeed * Time.deltaTime);
            }
            else
            {
                ReachedDestination = true;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this._destination = destination;
        ReachedDestination = false;
    }
}
