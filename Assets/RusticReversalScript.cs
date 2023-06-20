using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class RusticReversalScript : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMBombInfo BombInfo;
   public KMAudio Audio;
   public KMSelectable[] Buttons;
   public TextMesh[] DisplayTexts;
   public SpriteRenderer[] Sprites;
   public Sprite[] Actualsprites;

   private int correctButton = 0;
   private int towriteforlater;
   private int numberOfSolves;
   private int numberOfUnsolves;
   private bool activated;
   int numberToAdd = 0;
   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   private Color[] symbolColors = {
    new Color(1, 0, 0),        // Red symbol done
    new Color(0, 0, 1),       // Blue symboldone
    new Color(0, 1, 0),      // Green symbol done
    new Color(1, 0, 1),  // Magenta symbol
    new Color(1f, 0.92f, 0.016f),     // Yellow symbol done
    new Color(1, 0.5f, 0)  // Orange symbol
}; //symbol colors

private string[] colorWords = {
    "Red",
    "Blue",
    "Green",
    "Magenta",
    "Yellow",
    "Orange"
};

private List<string> colorsForNumber = new List<string>()
{
};

private List<string> spritesForNumber = new List<string>()
{
};

    private bool isCycling;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      Buttons[0].OnInteract += delegate () { buttonPress(1); return false; };
      Buttons[1].OnInteract += delegate () { buttonPress(2); return false; };
      Buttons[2].OnInteract += delegate () { buttonPress(3); return false; };
      Buttons[3].OnInteract += delegate () { buttonPress(4); return false; };
      Buttons[4].OnInteract += delegate () { buttonPress(5); return false; };
      Buttons[5].OnInteract += delegate () { buttonPress(6); return false; };
      Buttons[6].OnInteract += delegate () { buttonPress(7); return false; };
      Buttons[7].OnInteract += delegate () { buttonPress(8); return false; };
      //Audio.PlaySoundAtTransform("Gears", DisplayTexts[0].transform);
   }

   void buttonPress(int button)
   {
     if(!ModuleSolved)
     {
     if(!isCycling)
     {
     if(!activated)
     {
       LookButtonPressed();
       StartGlitchEffect();
       Audio.PlaySoundAtTransform("Switch", DisplayTexts[0].transform);
       Audio.PlaySoundAtTransform("Gears", DisplayTexts[0].transform);
       activated = true;
     }
     else
     {
       if(button == correctButton)
       {
         ModuleSolved = true;
         Audio.PlaySoundAtTransform("Switch", DisplayTexts[0].transform);
         StartGlitchEffect();
         Solve();
         Audio.PlaySoundAtTransform("Solve", DisplayTexts[0].transform);
         StartCoroutine(StartWinEffect());
         Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] Solved!", ModuleId);
       }
       else
       {
         Audio.PlaySoundAtTransform("Switch", DisplayTexts[0].transform);
         StartGlitchEffect();
         StartCoroutine(StrikeEffect());
         Strike();
         Audio.PlaySoundAtTransform("Strike", DisplayTexts[0].transform);
         Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] Strike! Defuser pressed button #" + button + " instead of button #" + correctButton + ".", ModuleId);
       }
     }
   }
 }
   else
   {
     //nothing. cycle button disables
   }
   }

private IEnumerator StartWinEffect()
{
  //
  yield return new WaitForSeconds(3.0f);
  DisplayTexts[0].text = "DEFUSED!";
  DisplayTexts[0].color = symbolColors[2];
  Sprites[0].material.color = Color.white;
  Sprites[1].material.color = Color.white;
  Sprites[2].material.color = Color.white;
  Sprites[0].sprite = Actualsprites[8];
  Sprites[1].sprite = Actualsprites[8];
  Sprites[2].sprite = Actualsprites[8];
}

   private Coroutine glitchCoroutine;
   private float durationInSeconds = 2f;
   private float cyclingStartTime; // Start time of the cycling
   private float glitchInterval = 0.005f;

   private void StartGlitchEffect()
   {
       if (glitchCoroutine != null)
           StopCoroutine(glitchCoroutine);

       glitchCoroutine = StartCoroutine(DoGlitchEffect());
   }

private float delayBetweenCharacters = 0.1f;
private string incorrectText = "INCORRECT";
private IEnumerator StrikeEffect()
{
  Sprites[0].sprite = Actualsprites[6];
  Sprites[1].sprite = Actualsprites[6];
  Sprites[2].sprite = Actualsprites[6];
  Sprites[0].material.color = Color.white;
  Sprites[1].material.color = Color.white;
  Sprites[2].material.color = Color.white;
  DisplayTexts[0].text = "";
  yield return new WaitForSeconds(2.4f);
  Sprites[0].sprite = Actualsprites[7];
  Sprites[1].sprite = Actualsprites[7];
  Sprites[2].sprite = Actualsprites[7];
  DisplayTexts[0].text = "";
  DisplayTexts[0].fontSize = 95;
  for (int i = 0; i < incorrectText.Length; i++)
        {
            DisplayTexts[0].text += incorrectText[i];
            yield return new WaitForSeconds(delayBetweenCharacters);
        }
  yield return new WaitForSeconds(1f);
  int stage2 = Generate6DigitNumber();
  DisplayTexts[0].text = stage2.ToString();
  DisplayTexts[0].color = symbolColors[0];
  DisplayTexts[0].fontSize = 150;
  SetSprites();
  SetSpriteColors();
  LogSymbolInformation();
  DetermineFirstNumber();
  //Regoes through the list. Doesn't really need to do the first func.
}

   private IEnumerator DoGlitchEffect()
   {
       isCycling = true;
       int symbolIndex = 0;
       cyclingStartTime = Time.time;
       Audio.PlaySoundAtTransform("Flip", DisplayTexts[0].transform);
       while (isCycling)
       {
           string glitchedText = GetGlitchedText();
           DisplayTexts[0].fontSize = 95;
           DisplayTexts[0].text = glitchedText;
           DisplayTexts[0].color = symbolColors[symbolIndex];
           symbolIndex = (symbolIndex + 1) % symbolColors.Length;
           yield return new WaitForSeconds(glitchInterval);
       if (Time.time - cyclingStartTime >= durationInSeconds)
{
    isCycling = false; // Set cycling state to false when duration is over
}
}
int stage2 = Generate6DigitNumber();
DisplayTexts[0].text = stage2.ToString();
DisplayTexts[0].color = symbolColors[0];
DisplayTexts[0].fontSize = 150;
   }

   private string GetGlitchedText()
   {
       string glitchedText = "";
       string originalText = DisplayTexts[0].text;
       for (int i = 0; i < originalText.Length; i++)
       {
           char character = originalText[i];
           if (character != ' ')
           {
               character = GetRandomCharacter();
           }
           glitchedText += character;
       }

       return glitchedText;
   }

   private char GetRandomCharacter()
   {
       string characters = "1234567890!@#$%^&*(){}:";
       int index = UnityEngine.Random.Range(0, characters.Length);
       return characters[index];
   }

   public int Generate6DigitNumber()
{
    int min = 100000;
    int max = 999999;
    int randomNumber = UnityEngine.Random.Range(min, max + 1);
    return randomNumber;
}

  void SetRandomNumber(){
    int generatedNumber = Generate6DigitNumber();
    towriteforlater = generatedNumber + 0;
    DisplayTexts[0].text = generatedNumber.ToString();
  }

void DetermineFirstSymbolNumber(int digitSum) {
  Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] - The number made with the display is: #" + digitSum + ".", ModuleId);
    switch (colorsForNumber[0].ToUpper())
    {

      //RED
      case "RED":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;
        break;
        case "CYLINDER":
        digitSum = digitSum - 10;
        break;
        case "PRESSURE-GAUGE":
        digitSum = digitSum + 1337;
        break;
        case "GOOGLES":
        digitSum = digitSum - 1;
        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum * 5;
        break;
      }
      break;

      //BLUE
      case "BLUE":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 25;
        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            if (Bomb.GetSerialNumber().Any(ch => "AEIOU".Contains(ch)))
            {
                numberToAdd = 1;
            }
            else
            {
                numberToAdd = 0;
            }
            digitSum = digitSum + numberToAdd;
            break;
        case "GOOGLES":
        digitSum = digitSum - digitSum;
        break;
        case "BRUSH":
        digitSum = digitSum * digitSum;
        break;
        case "WELDER":
        digitSum = digitSum * 2;
        break;
      }
      break;

      //GREEEN
      case "GREEN":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetBatteryCount();;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "CYLINDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().Last();
        break;
        case "PRESSURE-GAUGE":
            FetchSolvedModuleCount();
            digitSum = digitSum + numberOfSolves;
            break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum * 12;
        break;
        case "WELDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().First();
        break;
      }
      break;

      //magenta
      case "MAGENTA":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 12;
        break;
        case "CYLINDER":
        digitSum = digitSum + 27;
        break;
        case "PRESSURE-GAUGE":
        int dayOfMonth = DateTime.Now.Day;
            digitSum = digitSum + dayOfMonth;
            break;
        case "GOOGLES":
        FetchUnsolvedModuleCount();
        digitSum = digitSum - numberOfUnsolves;
        break;
        case "BRUSH":
        digitSum = digitSum * 45;
        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 4;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //YELLOW
      case "YELLOW":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;
        break;
        case "CYLINDER":
        digitSum = digitSum * 7;
        break;
        case "PRESSURE-GAUGE":
        if(Bomb.IsPortPresent(Port.StereoRCA))
        {
          numberToAdd = 2;
        }
        else
        {
          numberToAdd = 0;
        }
            digitSum = digitSum - numberToAdd;
            break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum + 9;
        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 6;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //ORANGE
      case "ORANGE":
      switch(spritesForNumber[0].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(Bomb.IsPortPresent(Port.RJ45))
        {
          numberToAdd = 7;
        }
        else
        {
          numberToAdd = 0;
        }
        digitSum = digitSum + numberToAdd;

        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            digitSum = digitSum + 1337;
            break;
        case "GOOGLES":
        digitSum = digitSum * 99;
        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum + 333;
        break;
      }
      break;
    }
    Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] - The number made with the first icon is: #" + digitSum + ".", ModuleId);
    DetermineSecondSymbolNumber(digitSum);
}

//SECOND
void DetermineSecondSymbolNumber(int digitSum) {
    switch (colorsForNumber[1].ToUpper())
    {

      //RED
      case "RED":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;

        break;
        case "CYLINDER":
        digitSum = digitSum - 10;

        break;
        case "PRESSURE-GAUGE":
        digitSum = digitSum + 1337;

        break;
        case "GOOGLES":
        digitSum = digitSum - 1;

        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum * 5;

        break;
      }
      break;

      //BLUE
      case "BLUE":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 25;

        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            if (Bomb.GetSerialNumber().Any(ch => "AEIOU".Contains(ch)))
            {
               numberToAdd = 1;
            }
            else
            {
                numberToAdd = 0;
            }
            digitSum = digitSum + numberToAdd;

            break;
        case "GOOGLES":
        digitSum = digitSum - digitSum;

        break;
        case "BRUSH":
        digitSum = digitSum * digitSum;

        break;
        case "WELDER":
        digitSum = digitSum * 2;

        break;
      }
      break;

      //GREEEN
      case "GREEN":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetBatteryCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "CYLINDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().Last();

        break;
        case "PRESSURE-GAUGE":
        FetchSolvedModuleCount();
        digitSum = digitSum + numberOfSolves;
          break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum * 12;

        break;
        case "WELDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().First();

        break;
      }
      break;

      //magenta
      case "MAGENTA":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 12;

        break;
        case "CYLINDER":
        digitSum = digitSum + 27;

        break;
        case "PRESSURE-GAUGE":
        int dayOfMonth = DateTime.Now.Day;
            digitSum = digitSum + dayOfMonth;

            break;
        case "GOOGLES":
        FetchUnsolvedModuleCount();
        digitSum = digitSum - numberOfUnsolves;

        break;
        case "BRUSH":
        digitSum = digitSum * 45;

        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 4;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //YELLOW
      case "YELLOW":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;

        break;
        case "CYLINDER":
        digitSum = digitSum * 7;

        break;
        case "PRESSURE-GAUGE":
        if(Bomb.IsPortPresent(Port.StereoRCA))
        {
          numberToAdd = 2;
        }
        else
        {
          numberToAdd = 0;
        }
            digitSum = digitSum - numberToAdd;

            break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum + 9;

        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 6;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //ORANGE
      case "ORANGE":
      switch(spritesForNumber[1].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(Bomb.IsPortPresent(Port.RJ45))
        {
          numberToAdd = 7;
        }
        else
        {
          numberToAdd = 0;
        }
        digitSum = digitSum + numberToAdd;

        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            digitSum = digitSum + 1337;

            break;
        case "GOOGLES":
        digitSum = digitSum * 99;

        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum + 333;
        break;
      }
      break;
    }
    Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] - The number made with the second icon is: #" + digitSum + ".", ModuleId);
    DetermineThirdSymbolNumber(digitSum);
}

//THIRD
void DetermineThirdSymbolNumber(int digitSum) {
    switch (colorsForNumber[2].ToUpper())
    {
      //RED
      case "RED":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;

        break;
        case "CYLINDER":
        digitSum = digitSum - 10;

        break;
        case "PRESSURE-GAUGE":
        digitSum = digitSum + 1337;

        break;
        case "GOOGLES":
        digitSum = digitSum - 1;

        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum * 5;

        break;
      }
      break;

      //BLUE
      case "BLUE":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 25;

        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            if (Bomb.GetSerialNumber().Any(ch => "AEIOU".Contains(ch)))
            {
               numberToAdd = 1;
            }
            else
            {
               numberToAdd = 0;
            }
            digitSum = digitSum + numberToAdd;

            break;
        case "GOOGLES":
        digitSum = digitSum - digitSum;

        break;
        case "BRUSH":
        digitSum = digitSum * digitSum;

        break;
        case "WELDER":
        digitSum = digitSum * 2;

        break;
      }
      break;

      //GREEEN
      case "GREEN":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetBatteryCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "CYLINDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().Last();

        break;
        case "PRESSURE-GAUGE":
        FetchSolvedModuleCount();
        digitSum = digitSum + numberOfSolves;
            break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum * 12;

        break;
        case "WELDER":
        digitSum = digitSum + Bomb.GetSerialNumberNumbers().First();

        break;
      }
      break;

      //magenta
      case "MAGENTA":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum - 12;

        break;
        case "CYLINDER":
        digitSum = digitSum + 27;

        break;
        case "PRESSURE-GAUGE":
        int dayOfMonth = DateTime.Now.Day;
            digitSum = digitSum + dayOfMonth;

            break;
        case "GOOGLES":
        FetchUnsolvedModuleCount();
        digitSum = digitSum - numberOfUnsolves;

        break;
        case "BRUSH":
        digitSum = digitSum * 45;

        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 4;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //YELLOW
      case "YELLOW":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        digitSum = digitSum + 20;

        break;
        case "CYLINDER":
        digitSum = digitSum * 7;

        break;
        case "PRESSURE-GAUGE":
        if(Bomb.IsPortPresent(Port.StereoRCA))
        {
          numberToAdd = 2;
        }
        else
        {
          numberToAdd = 0;
        }
            digitSum = digitSum - numberToAdd;

            break;
        case "GOOGLES":
        if(digitSum > 0)
        {
        digitSum = digitSum / Bomb.GetPortPlateCount();
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "BRUSH":
        digitSum = digitSum + 9;

        break;
        case "WELDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 6;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
      }
      break;

      //ORANGE
      case "ORANGE":
      switch(spritesForNumber[2].ToString().ToUpper())
      {
        case "WELDING-MASK":
        if(Bomb.IsPortPresent(Port.RJ45))
        {
          numberToAdd = 7;
        }
        else
        {
          numberToAdd = 0;
        }
        digitSum = digitSum + numberToAdd;

        break;
        case "CYLINDER":
        if(digitSum > 0)
        {
        digitSum = digitSum / 2;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "PRESSURE-GAUGE":
            digitSum = digitSum + 1337;

            break;
        case "GOOGLES":
        digitSum = digitSum * 99;

        break;
        case "BRUSH":
        if(digitSum > 0)
        {
        digitSum = digitSum / 3;
        }
        else
        {
          digitSum = digitSum;
        }
        break;
        case "WELDER":
        digitSum = digitSum + 333;

        break;
      }
      break;
    }
    int finalnumTry3 = digitSum;
    if(finalnumTry3 < 0)
    {
      finalnumTry3 = 1;
    }
    else
    {
      finalnumTry3 = Mathf.CeilToInt(finalnumTry3);
    }
    correctButton = GetNumberWithinRange(finalnumTry3);
    Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] The button to press is: #" + correctButton + ".", ModuleId);
}

private int GetNumberWithinRange(int inputNumber)
{
    int num = inputNumber;
    while (num > 8)
    {
        num -= 2;
    }
    if (num == 0)
    {
        num += 1;
    }
    return num;
}

  void DetermineFirstNumber() {
    int displayedNumber = towriteforlater;
    int reversedNumber = ReverseNumber(displayedNumber);
    int totalSum = reversedNumber;
    int digitSum = 0;
    string totalSumString = totalSum.ToString();
    for (int i = 0; i < totalSumString.Length; i++) {
      int digit = int.Parse(totalSumString[i].ToString());
      digitSum += digit;
    }
    //Go to symbol solving!!
    DetermineFirstSymbolNumber(digitSum);
  }

  private int ReverseNumber(int number)
  {
      int reversedNumber = 0;
      while (number > 0)
      {
          reversedNumber = (reversedNumber * 10) + (number % 10);
          number /= 10;
      }
      return reversedNumber;
  }

  void SetSpriteColors(){
    int randomIndex1 = UnityEngine.Random.Range(0, symbolColors.Length);
    int randomIndex2 = UnityEngine.Random.Range(0, symbolColors.Length);
    int randomIndex3 = UnityEngine.Random.Range(0, symbolColors.Length);

    Sprites[0].material.color = symbolColors[randomIndex1];
    Sprites[1].material.color = symbolColors[randomIndex2];
    Sprites[2].material.color = symbolColors[randomIndex3];

  }

  void SetSprites()
  {
      int randomIndex = UnityEngine.Random.Range(0, Actualsprites.Length - 3);
      int randomIndex1 = UnityEngine.Random.Range(0, Actualsprites.Length - 3);
      int randomIndex2 = UnityEngine.Random.Range(0, Actualsprites.Length - 3);
      Sprites[0].sprite = Actualsprites[randomIndex];
      Sprites[1].sprite = Actualsprites[randomIndex1];
      Sprites[2].sprite = Actualsprites[randomIndex2];
  }

  string GetColorWord(Color color)
  {
      for (int i = 0; i < symbolColors.Length; i++)
      {
          if (symbolColors[i] == color)
          {
              return colorWords[i];
          }
      }
      return "Unknown";
  }

void LogSymbolInformation(){
  for (int i = 0; i < Sprites.Length; i++)
        {
            Renderer renderer = Sprites[i];
            Color color = renderer.material.color;
            string colorWord = GetColorWord(color);
            SpriteRenderer spriteRenderer = Sprites[i];
            Sprite sprite = spriteRenderer.sprite;
            colorsForNumber.Add(colorWord);
            Debug.LogFormat("[Rustic Reversal " + "#" + ModuleId + "] Symbol " + (i + 1) + " - Color: " + colorWord + ", Sprite: " + sprite.name, ModuleId);
            spritesForNumber.Add(sprite.name);
        }
    }


   void OnDestroy () { //Shit you need to do when the bomb ends

   }

   void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

   }

   void FetchSolvedModuleCount()
   {
     int numberOfSolves = BombInfo.GetSolvedModuleNames().Count;
     numberOfSolves = numberOfSolves;
   }

   void FetchUnsolvedModuleCount()
   {
     int numberOfUnsolves = BombInfo.GetSolvableModuleNames().Count;
     numberOfUnsolves = numberOfUnsolves;
   }

   void Start () { //Shit to start with
     //Activate later
     activated = false;
     DisplayTexts[0].text = "";
     Sprites[0].sprite = Actualsprites[6];
     Sprites[1].sprite = Actualsprites[6];
     Sprites[2].sprite = Actualsprites[6];
   }

   void LookButtonPressed(){
     SetRandomNumber();
     SetSpriteColors();
     SetSprites();
     LogSymbolInformation();
     DetermineFirstNumber();
   }

   void Update () { //Shit that happens at any point after initialization

   }

   void Solve () {
      GetComponent<KMBombModule>().HandlePass();
   }

   void Strike () {
      GetComponent<KMBombModule>().HandleStrike();
   }

   #pragma warning disable 414
   private
   readonly string TwitchHelpMessage =
       @"Use !{0} press [1-8] to press that respective button in reading order.";
   #pragma warning restore 414

   IEnumerator ProcessTwitchCommand(string Command) {
     Command = Command.ToUpper();
     yield return null;

     switch (Command) {
       case "PRESS 1":
         Buttons[0].OnInteract();
         break;

       case "PRESS 2":
         Buttons[1].OnInteract();
         break;

       case "PRESS 3":
         Buttons[2].OnInteract();
         break;

       case "PRESS 4":
         Buttons[3].OnInteract();
         break;

       case "PRESS 5":
         Buttons[4].OnInteract();
         break;

       case "PRESS 6":
         Buttons[5].OnInteract();
         break;

       case "PRESS 7":
         Buttons[6].OnInteract();
         break;

       case "PRESS 8":
         Buttons[7].OnInteract();
         break;
     }
   }

   IEnumerator TwitchHandleForcedSolve() {
     int twitchButton = correctButton;
     switch (twitchButton) {
       case 1:
         Buttons[0].OnInteract();
         break;

       case 2:
         Buttons[1].OnInteract();
         break;

       case 3:
         Buttons[2].OnInteract();
         break;

       case 4:
         Buttons[3].OnInteract();
         break;

       case 5:
         Buttons[4].OnInteract();
         break;

       case 6:
         Buttons[5].OnInteract();
         break;

       case 7:
         Buttons[6].OnInteract();
         break;

       case 8:
         Buttons[7].OnInteract();
         break;
     }
     yield return null;
   }
   }
