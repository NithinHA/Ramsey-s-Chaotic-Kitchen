using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SingletonBase;

public class InstructionPanel : Singleton<InstructionPanel>
{
	[SerializeField] private TextMeshProUGUI displayText;

	public void DisplayInstruction(string text)
	{
		displayText.text = text;
	}
}
