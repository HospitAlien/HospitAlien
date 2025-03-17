public class Status
{
    public bool needsInjection;
    public bool needsExtinguishing;
    public bool hasShrapnel;
    public int shrapnelCount;
    public int curSize;
    public int numberOfBadEyes;


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
            response += "I need an injection.\n";
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

}