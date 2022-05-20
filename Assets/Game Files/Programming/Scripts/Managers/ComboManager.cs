/** Keeps track of combo count, letter, and UI.
 *  NOTE: LIKELY WILL BE REPLACED. Not sold to anything and likely will make this better and integrated in with more appropriate scripts 
 *  when I understand all the pieces better - Andrew
 *  
 *  TO DO
 *  - Transition letter steps to animation curve
 *  - Likely have rate of decrease increase at higher letters
 *  - Display the increase and decrease on the letter/combo counter
 *  - Should we display combo count as well, only 'true' combos?
 *  - Fun Letter descriptions
 *  - Attacks should add different combo values (assuming we go dmc route)
 *  - Grass should count towards combo, but only very small amount
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboManager : Singleton<ComboManager>
{
    public float currentCombo = 0;

    [SerializeField] TMP_Text comboLetter;
    [SerializeField] float decreaseRate = 1;
    [SerializeField] float comboCap = 50;
    //[SerializeField] AnimationCurve letterCurve;
    [SerializeField] float[] letterSteps;
    [SerializeField] string[] letters;

    private int currentLetterIndex = 0;

    private void Update()
    {
        if (currentCombo > 0) //Add a proper clamp
        {
            currentCombo -= Time.deltaTime * decreaseRate;
        }
        UpdateComboLetter();
    }

    public void AddCombo(float amount)
    {
        if (currentCombo < comboCap) //Make a proper clamp
        {
            currentCombo += amount;
        }
        UpdateComboLetter();
    }

    private void UpdateComboLetter()
    {
        if(currentLetterIndex < letterSteps.Length - 1 && currentCombo >= letterSteps[currentLetterIndex + 1])
        {
            currentLetterIndex++;
            comboLetter.text = letters[currentLetterIndex];
        }
        else if(currentLetterIndex > 0 && currentCombo < letterSteps[currentLetterIndex])
        {
            currentLetterIndex--;
            comboLetter.text = letters[currentLetterIndex];
        }
    }
}
