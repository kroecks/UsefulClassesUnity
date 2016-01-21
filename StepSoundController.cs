using UnityEngine;
using System.Collections;

public class StepSoundController : MonoBehaviour {

	public bool m_singleton = false;
	public static StepSoundController instance = null;

	// If we're flagged as the singleton step manager, register ourself to the static variable
	void Start () {
		if( m_singleton )
		{
			instance = this;
		}
	}

	// Keep track of how much time has passed since the last step
	public float m_lastStep = 0f;

	// An array of all of the different step sounds we can make
	public AudioClip[] m_steps = new AudioClip[0];
	
	// Movement speed is what drives how fast we make sounds. 1.0f is standard time
	public float m_movementSpeed = 0f;
	
	// How often we should play a step sound in seconds
	public float m_stepRate = 0.0f;
	
	private bool m_timeControlActive = false;
	
	public AudioSource m_audioSrc = null;

	// Update is called once per frame
	void Update () {
		
		if(m_movementSpeed <= 0f )
		{
			m_lastStep += Time.deltaTime;
			return;
		}
		
		// Scale the amount of time that's passed since the last step based on the relative speed
		m_lastStep += (Time.deltaTime * m_movementSpeed);

		if( m_timeControlActive && m_stepRate > 0f && m_stepRate < m_lastStep )
		{
			m_lastStep = 0f;
			Step();
		}
	}
	
	// Called into to activate repeating sounds on a timer
	// Does not reset the last step time, so can be called as often as necessary
	public void SetTimeControlledSoundActive( bool enabled, float stepRate, float moveSpeed )
	{
		m_timeControlActive = enabled;
		m_stepRate = stepRate;
		m_movementSpeed = moveSpeed;
	}

	// The function "Step" can either be controller from an animation event or by way of timed interval
	public void Step()
	{
		if( !m_audioSrc )
		{
			// Add an audio source component and reference it
			AddComponent<AudioSource>();
			m_audioSrc = GetComponent<AudioSource>();
		}
		
		if( m_audioSrc )
		{
			m_audioSrc.clip = m_steps[ Random.Range( 0, m_steps.Length ) ];
			m_audioSrc.PlayOneShot( m_audioSrc.clip );
		}
	}
}
