using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class Cooker : MonoBehaviour
{
	public bool is_cooking = false;
	public bool is_cooked = false;

	Item food_item;				// this is the item cooked rice/noodles that is to be added to inventory
	
	public float time_to_spoil = 15;
	float cur_time;

	private Coroutine cooking_delay_coroutine;
	private Coroutine spoiling_delay_coroutine;

	[SerializeField] private GameObject countdown_display_prefab;
	[SerializeField] private Transform m_CountdownDisplayTransform;
	private GameObject countdown_display;

	[Header("Particles:")]
	[SerializeField] private GameObject flame_particles_prefab;
	private GameObject flame_particles;
	[SerializeField] private GameObject smoke_particles_prefab;
	private GameObject smoke_particles;
	[SerializeField] private GameObject overcooked_particles_prefab;
	private GameObject overcooked_particles;

	[Header("Audio Clips:")]
	[SerializeField] private AudioSource stove_ignition_audio;
	[SerializeField] private AudioSource cooker_flame_audio;
	[SerializeField] private AudioSource overcooked_audio;

	//[Header("Materials:")]
	//[SerializeField] Material cooking_mat;
	//[SerializeField] Material cooked_mat;
	//Material default_mat;
	//[SerializeField] Renderer stove;

	void Start()
	{
		cur_time = time_to_spoil;
		
		//default_mat = stove.material;
	}


	void Update()
	{

	}

	public void cook(Item food_item)		// called when player says, "COOK food_item AT __"
	{
		if (!is_cooking)            // this overcomes the scenario in which one cheff goes to get food_item to boil and at the same time, you instruct other cheff to turn off cooker.
			is_cooking = true;      // eg.- "alice boil rice at a" followed by "bob turn off cooker a" immediately. Without this check, it will result in "Overcooked".
		string food_item_name = food_item.name.Split()[1];
		Debug.Log("cooking " + food_item_name);
		InstructionPanel.Instance.DisplayInstruction("cooking " + food_item_name);
		this.food_item = food_item;
		//stove.material = cooking_mat;			// indicates that stove is lit and food_item is being cooked
		cooking_delay_coroutine = StartCoroutine(cooking_delay(food_item_name));
	}

	IEnumerator cooking_delay(string food_item_name)
	{
		// displpay the countdown_timer and then delete it after the countdown is over
		countdown_display = Instantiate(countdown_display_prefab, m_CountdownDisplayTransform.position, Quaternion.Euler(new Vector3(45, 0, 0)));
		countdown_display.transform.SetParent(transform);
		countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(food_item.time_to_prepare);
		Destroy(countdown_display, food_item.time_to_prepare);

		// display particle effects for cooking action
		flame_particles = Instantiate(flame_particles_prefab, transform.position + new Vector3(0, -1f, 0), Quaternion.identity);
		flame_particles.transform.SetParent(transform);

		// play stove_ignition and cooker_flame sound
		stove_ignition_audio.Play();
		cooker_flame_audio.Play();

		yield return new WaitForSeconds(food_item.time_to_prepare);
		Debug.Log(food_item_name + " cooked");
		InstructionPanel.Instance.DisplayInstruction(food_item_name + " cooked");
		is_cooked = true;						// at this time, both is_cooking and is_cooked  will be true and system waits for user to turn off cooker
		//stove.material = cooked_mat;			// indicates that food_item is cooked and you may turn off the stove

		Debug.Log("indicate turn off cooker!");
		InstructionPanel.Instance.DisplayInstruction("indicate turn off cooker");
		spoiling_delay_coroutine = StartCoroutine(spoiling_delay());
	}

	IEnumerator spoiling_delay()
	{
		// displpay the countdown_timer and then delete it after the countdown is over
		countdown_display = Instantiate(countdown_display_prefab, m_CountdownDisplayTransform.position, Quaternion.Euler(new Vector3(45, 0, 0)));
		countdown_display.transform.SetParent(transform);
		countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(time_to_spoil);
		Destroy(countdown_display, time_to_spoil);

		// display smoke particle indicating that food is being spoiled
		smoke_particles = Instantiate(smoke_particles_prefab, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);

		yield return new WaitForSeconds(time_to_spoil);
		//instantiate smoke particle effects at cooker position that self destroy after 1s indicating food has spoiled
		is_cooking = false;
		is_cooked = false;
		Debug.Log("Overcooked!");
		InstructionPanel.Instance.DisplayInstruction("Overcooked");

		// turn off flame automatically
		if(flame_particles != null)
		{
			Destroy(flame_particles);
		}
		// destroy smoke particles as well
		if(smoke_particles != null)
		{
			Destroy(smoke_particles);
		}

		// stop cooker_flame sound
		cooker_flame_audio.Stop();

		// release lot of smoke indicating food has been overcooked
		overcooked_particles = Instantiate(overcooked_particles_prefab, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
		Destroy(overcooked_particles, 5);

		// play overcooked sound
		overcooked_audio.Play();
		//stove.material = default_mat;
	}

	public void turn_off_cooker()       // called when player says, "TURN OFF COOKER __"
	{
		if (cooking_delay_coroutine != null)
		{
			StopCoroutine(cooking_delay_coroutine);
		}
		if (spoiling_delay_coroutine != null)
		{
			StopCoroutine(spoiling_delay_coroutine);
		}

		if (is_cooked)
		{
			// add cooked rice or cooked noodles (food_item) to inventory... DONE
			bool has_added = Inventory.Instance.addItem(food_item);
			if (has_added) {
				Debug.Log(food_item.name + " added to inventory");
				InstructionPanel.Instance.DisplayInstruction(food_item.name + " added to inventory");
			} else {
				Debug.Log("can not add " + food_item.name + " to inventory");
				InstructionPanel.Instance.DisplayInstruction("can not add " + food_item.name + " to inventory");
			}
		}
		is_cooking = false;
		is_cooked = false;

		if(countdown_display != null)
		{
			Destroy(countdown_display);			// destroy countdown timer
		}
		if(flame_particles != null)
		{
			Destroy(flame_particles);			// turn off flame
		}
		if(smoke_particles != null)
		{
			Destroy(smoke_particles);			// destroy smoke effect
		}

		// stop cooker_flame sound
		cooker_flame_audio.Stop();

		Debug.Log("cooking stops");
		InstructionPanel.Instance.DisplayInstruction("cooking stops");

		//stove.material = default_mat;		// indicates cooking is over and stove is turned off
	}
}
