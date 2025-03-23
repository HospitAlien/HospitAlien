using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Status
{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool hasShrapnel;
    public int shrapnelCount;
    public int curSize;
    public int numberOfBadEyes;
    public string bloodType;

    public Status(){
        System.Random random = new System.Random();
        double result = random.NextDouble();
        if (result < (1.0/3.0))
        {
            bloodType = "Green";
        }else if((1.0/3.0) <= result && result < (2.0/3.0)){
            bloodType = "Blue";
        }else{
            bloodType = "Red";
        }

        Debug.Log(result);
        Debug.Log(bloodType);
    }

    public bool isHealthy()
    {
        return (!needsInjection && !needsExtinguishing && !hasShrapnel && curSize == 0 && numberOfBadEyes == 0);
    }

    public string getIllness()
    {
        string response = string.Empty;
        if (needsExtinguishing)
        {
            response += "I'm burning, grab the fire extinguisher and put me out.\n";
        }
        else if (needsInjection)
        {
            response += "I need blood, inject me with the correct type please.\n";
        }
        else if (hasShrapnel)
        {
            response += "There is shrapnel inside of me, grab it and pull it out.\n";
        }
        else if (curSize == -1)
        {
            response += "I am really small get a growth pill.\n";
        }
        else if (curSize == 1)
        {
            response += "I am massive get me a shrink pill.\n";
        }
        else if(numberOfBadEyes != 0)
        {
            response += "I have a really itchy eye, get me an eyedrop.\n";
        }

        return response;
    }

    public void shrapnelRemoved()
    {
        shrapnelCount = shrapnelCount - 1;
        if (shrapnelCount == 0)
        {
            hasShrapnel = false;
        }
    }

    public void shrapnelInserted()
    {
        shrapnelCount = shrapnelCount + 1;
        hasShrapnel = true;
    }

    public void applyEyeDrop(){
        numberOfBadEyes = numberOfBadEyes -1;
    }


    public void initiateStatus(PurpleAlien alien, int gameStage){
        System.Random random = new System.Random();

        int numberOfIllnesses;
        if(gameStage == 0){ //first stage
            numberOfIllnesses = random.Next(1, 3);
        }else if(gameStage == 1){
            numberOfIllnesses = random.Next(2,5);
        }else{
            numberOfIllnesses = random.Next(3,6);
        }


        List<Action> illnesses = new List<Action>();
        illnesses.Add(() => {
            needsInjection = true;
            alien.sweatParticles.Play();
            alien.reward += 100;
        });

        illnesses.Add(() => {
            needsExtinguishing = true;
            alien.fireParticles.Play();
            alien.reward += 50;
        });

        illnesses.Add(() => {
            shrapnelCount = 4;
            alien.reward += 150;
            hasShrapnel = true;
            alien.initiateShrapnel();
        });

        illnesses.Add(() => {
            alien.reward += 100;
            if (random.NextDouble() < 0.5) { // shrink
                curSize = -1;
                alien.transform.localScale /= 2f;
            }
            else { // enlargement
                curSize = 1;
                alien.transform.localScale *= 1.4f;
            }
        });

        illnesses.Add(() => {
            alien.reward += 75;
            numberOfBadEyes = 1;
            int eyeNumber = random.Next(1, 4);
            EyeScript eye = alien.transform.Find($"eye{eyeNumber}").gameObject.GetComponent<EyeScript>();
            eye.activate(alien);
        });

        List<Action> shuffledIllnesses = illnesses.OrderBy(x => random.Next()).ToList();
        for (int i = 0; i < numberOfIllnesses; i++) {
            shuffledIllnesses[i]();
        }
    }


    public void initiateStatus(GreenAlien alien, int gameStage){
        System.Random random = new System.Random();

        int numberOfIllnesses;
        if(gameStage == 0){ //first stage
            numberOfIllnesses = random.Next(1, 3);
        }else if(gameStage == 1){
            numberOfIllnesses = random.Next(2,5);
        }else{
            numberOfIllnesses = random.Next(3,5);
        }


        List<Action> illnesses = new List<Action>();
        illnesses.Add(() => {
            needsInjection = true;
            alien.sweatParticles.Play();
            alien.reward += 100;
        });

        illnesses.Add(() => {
            needsExtinguishing = true;
            alien.fireParticles.Play();
            alien.reward += 50;
        });

        illnesses.Add(() => {
            shrapnelCount = 4;
            alien.reward += 150;
            hasShrapnel = true;
            alien.initiateShrapnel();
        });

        illnesses.Add(() => {
            alien.reward += 100;
            if (random.NextDouble() < 0.5) { // shrink
                curSize = -1;
                alien.transform.localScale /= 2f;
            }
            else { // enlargement
                curSize = 1;
                alien.transform.localScale *= 1.4f;
            }
        });

        List<Action> shuffledIllnesses = illnesses.OrderBy(x => random.Next()).ToList();
        for (int i = 0; i < numberOfIllnesses; i++) {
            shuffledIllnesses[i]();
        }
    }

}