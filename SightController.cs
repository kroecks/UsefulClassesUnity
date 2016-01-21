using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SightObject
{
    // The actual object we're tracking
    public GameObject m_object;
    // Whether or not they're currently visible
    public bool m_visible;
    // The last position we saw them at
    public Vector3 m_lastKnownPosition;
    // The amount of time that it's been since we last saw them
    public float m_timeSinceSight;
}

public class SightController : MonoBehaviour
{
    // Ideally you can use a unique value as the key, but we're not developing a guid system for this
    public Dictionary<GameObject, SightObject> m_sightObjects = new Dictionary<GameObject, SightObject>();

    // Number of degrees, centred on forward. \-60|+60/ = 120
    public float m_fieldOfViewAngle = 110f;

    public float m_memoryLifespan = 10f;

    public Transform m_visionOrigin = null;   

    void OnTriggerEnter(Collider other)
    {
        if( m_sightObjects.ContainsKey(other.gameObject))
        {
            // we already know of this object, nothing more to do
            return;
        }

        SightObject newSighting = new SightObject();
        newSighting.m_object = other.gameObject;
        newSighting.m_timeSinceSight = 0.0f;
        newSighting.m_lastKnownPosition = Vector3.zero;
        newSighting.m_visible = IsObjectVisible(other.gameObject, ref newSighting.m_lastKnownPosition);

        if( newSighting.m_visible )
        {
            newSighting.m_object.SendMessage("SightObjectFound");
        }

        m_sightObjects.Add(other.gameObject, newSighting);
    }


    void Update()
    {
        UpdateKnownObjects();
    }

    void UpdateKnownObjects()
    {
        List<GameObject> removeObjects = new List<GameObject>();
        foreach( KeyValuePair<GameObject,SightObject> sightPair in m_sightObjects )
        {
            SightObject vision = sightPair.Value;
            bool newVisible = IsObjectVisible(vision.m_object, ref vision.m_lastKnownPosition);
            if( newVisible != vision.m_visible )
            {
                vision.m_object.SendMessage( newVisible ? "SightObjectFound" : "SightObjectLost");
                vision.m_visible = newVisible;
            }

            if( !vision.m_visible )
            {
                vision.m_timeSinceSight = Time.deltaTime;
                if(vision.m_timeSinceSight > m_memoryLifespan )
                {
                    removeObjects.Add(vision.m_object);
                }
            }
            else
            {
                vision.m_timeSinceSight = 0f;
            }
        }

        foreach( GameObject lostObject in removeObjects )
        {
            // Notify the object that we've forgotten about it
            lostObject.SendMessage("SightObjectForgotten");

            // Remove it from our list of known objects
            m_sightObjects.Remove(lostObject);
        }
    }

    public bool IsObjectVisible( GameObject otherObj, ref Vector3 visiblePosition )
    {
        // Null check
        if( !otherObj )
        {
            return false;
        }

        // If we haven't set our vision up, default to our transform
        if( m_visionOrigin == null )
        {
            m_visionOrigin = transform;
        }

        // Start with the most basic direction
        Vector3 direction = (otherObj.transform.position - m_visionOrigin.position);

        // Now attempt to get one based on the center of mass of the actor, since we're not looking at feet
        // It's possible that the renderer component isn't on the root object, so find it in children
        Renderer renderComp = otherObj.GetComponentInChildren<Renderer>();
        if (renderComp)
        {
            direction = (renderComp.bounds.center - m_visionOrigin.position);
        }

        // Now find the angle out of our forward direction they are
        float angle = Vector3.Angle(direction, m_visionOrigin.forward);
        // Check that they're within half of our field of view (-60|60) = 120
        if( angle > (0.5f * m_fieldOfViewAngle))
        {
            // we can early out, since they're not in sight.
            return false;
        }

        // Now make a physics query to see if anything is blocking them
        Ray testRay = new Ray();
        testRay.origin = m_visionOrigin.position;
        testRay.direction = direction;

        RaycastHit hitInfo = new RaycastHit();

        // Raycast to the point specified and make sure the thing we hit is our object
        if (Physics.Raycast(testRay, out hitInfo) && hitInfo.collider.gameObject == otherObj)
        {
            // Since the visible position is the point they're actually standing at, we want to assign the point to their position
            visiblePosition = otherObj.transform.position;
            return true;
        }

        return false;
    }
}