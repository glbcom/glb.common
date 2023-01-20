using System;

namespace Glb.Common.Utilities;
public static class Tools
{
    private static Random Random = new Random();
    public static string GenerateRandomString(int length, bool? includeCapitals = true, bool? includeSmall = true, bool? includeNumbers = true, bool? includeSpecial = true)
    {
        if (length <= 0)
        {
            return string.Empty;
        }
        string capitalCharacters = "BCDEFGHIJKLMNOPQRSTUVWXYZ";
        string smallCharacters = capitalCharacters.ToLower();
        string numbers = "0123456789";
        string specilCharacters = "-!#*";
        string defaultCharacters = string.Empty;
        if (includeSmall == true)
        {
            defaultCharacters = smallCharacters;
        }
        else if (includeCapitals == true)
        {
            defaultCharacters = capitalCharacters;
        }
        else if (includeNumbers == true)
        {
            defaultCharacters = numbers;
        }
        else if (includeSpecial == true)
        {
            defaultCharacters = specilCharacters;
        }

        var stringChars = new char[length];

        int j;

        for (int i = 0; i < stringChars.Length; i++)
        {
            j = (i + 1) % 5;
            if ((j == 0 || j == 1) && includeCapitals == true)
            {
                stringChars[i] = capitalCharacters[Random.Next(capitalCharacters.Length)];
            }
            else if (j == 2 && includeSmall == true)
            {
                stringChars[i] = smallCharacters[Random.Next(smallCharacters.Length)];
            }
            else if (j == 3 && includeNumbers == true)
            {
                stringChars[i] = numbers[Random.Next(numbers.Length)];
            }
            else if (j == 4 && includeSpecial == true)
            {
                stringChars[i] = specilCharacters[Random.Next(specilCharacters.Length)];
            }
            else
            {
                stringChars[i] = defaultCharacters[Random.Next(defaultCharacters.Length)];
            }
        }

        var finalString = new String(stringChars);
        return finalString;
    }
}