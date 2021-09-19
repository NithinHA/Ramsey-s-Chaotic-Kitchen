using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
	#region Singleton
	public static Score instance;       // singleton
	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one score instance found in the scene");
			return;
		}
		instance = this;
	}
	#endregion

	public static int score = 0;
	Text score_text;

	KeywordsData keywords_data;

    void Start()
    {
		keywords_data = KeywordsData.instance;
		score_text = GetComponentInChildren<Text>();

        score = 0;      // doing so avoids the score being carried when you restart the level
		score_text.text = "Score: ₹" + score.ToString();
    }
	
    void Update()
    {
        
    }

	public void payBill(Item dish, int tips)
	{
		int bill_amount = keywords_data.dish_cost[dish];
		score += bill_amount + tips;
		score_text.text = "Score: ₹" + score.ToString();

		// play coins sound
		GetComponent<AudioSource>().Play();
	}
}
