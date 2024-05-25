using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SingletonBase;

public class Score : Singleton<Score>
{
	public static int score = 0;
	//Text score_text;
	[SerializeField] TextMeshProUGUI scoreText;

	KeywordsData keywords_data;

    protected override void Start()
    {
		base.Start();

		keywords_data = KeywordsData.Instance;
		//score_text = GetComponentInChildren<Text>();

        score = 0;      // doing so avoids the score being carried when you restart the level
		//score_text.text = "Score: ₹" + score.ToString();
		scoreText.text = "Score: $" + score.ToString();
	}
	
    void Update()
    {
        
    }

	public void payBill(Item dish, int tips)
	{
		int bill_amount = keywords_data.dish_cost[dish];
		score += bill_amount + tips;
		scoreText.text = "Score: $" + score.ToString();

		AudioManager.Instance.PlaySound(Constants.Audio.CoinsEarn);
	}
}
