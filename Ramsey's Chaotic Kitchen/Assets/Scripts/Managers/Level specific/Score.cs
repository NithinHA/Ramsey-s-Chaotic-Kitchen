using SingletonBase;
using System;
using UnityEngine;

public class Score : Singleton<Score>
{
	public static Action<int, bool> OnScoreUpdate;	// pass score and can_animate
	public int score = 0;
	public int TargetScore = 500;

	private KeywordsData keywords_data;

    protected override void Start()
    {
		base.Start();
		keywords_data = KeywordsData.Instance;
		OnScoreUpdate?.Invoke(score, false);
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			score += UnityEngine.Random.Range(40, 90);
			OnScoreUpdate?.Invoke(score, true);
		}
	}
#endif

	public void PayBill(Item dish, int tips)
	{
		int baseDishCost = keywords_data.dish_cost[dish];
		score += baseDishCost + tips;

		OnScoreUpdate?.Invoke(score, true);
		AudioManager.Instance?.PlaySound(Constants.Audio.CoinsEarn);
	}
}
