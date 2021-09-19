using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
	public bool is_washing = false;
	Utensils utensils;

	[SerializeField] private GameObject countdown_display_prefab;

	[SerializeField] private GameObject tapwater_particles_prefab;

	void Start()
    {
		utensils = Utensils.instance;
		
    }
	
    void Update()
    {
        
    }

	public void washUtensils(Item utensil)				// called when user says "WASH utensil" where utensil can be "plates","bowls" or "cups" 
	{
		int utensil_index;
		utensils.utensil_index_dict.TryGetValue(utensil.name, out utensil_index);

		int dirty_utensils = utensils.utensil_count_arr[utensil_index] - utensils.clean_utensil_arr[utensil_index];		// compute number of dirty utensils
		float washing_time = utensil.time_to_prepare * dirty_utensils;									// compute time taken to wash dirty utensils
		Debug.Log(utensil.name + " to wash:" + dirty_utensils + "\ntime taken:" + washing_time);
		Test_script2.ts2.applyText(utensil.name + " to wash:" + dirty_utensils + "\ntime taken:" + washing_time);
		StartCoroutine(washing_utensils(washing_time, utensil_index));
	}

	IEnumerator washing_utensils(float washing_time, int utensil_index)
	{
		// displpay the countdown_timer and then delete it after the countdown is over
		GameObject countdown_display = Instantiate(countdown_display_prefab, transform.position + new Vector3(0, 1, .5f), Quaternion.Euler(new Vector3(45, 0, 0)));
		countdown_display.transform.SetParent(transform);
		countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(washing_time);
		Destroy(countdown_display, washing_time);

		// display tap water particles
		GameObject tapwater_particles = Instantiate(tapwater_particles_prefab, transform.position + new Vector3(0, .8f, 0), Quaternion.identity);
		tapwater_particles.transform.SetParent(transform);

		// play wash_basin sound
		GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(washing_time);
		utensils.clean_utensil_arr[utensil_index] = utensils.utensil_count_arr[utensil_index];            // cleans all utensil of that instance. ie.- cleans all plates or bowls or cups
		is_washing = false;

		Destroy(tapwater_particles);

		// stop wash_basin sound
		GetComponent<AudioSource>().Stop();

		utensils.on_utensil_changed_callback.Invoke();              // invoke delegate on cleaning utensils
	}
}
