
public static class MiscFunctions{
   public static T RandomEnumValue<T> ()
    {
        System.Random rnd = new System.Random();
        var v = System.Enum.GetValues (typeof (T));
        return (T) v.GetValue (rnd.Next(v.Length));
    }

    public static int GetPercentage(int main,float percent){
        float q = (percent / 100f) * (float)main;
        return (int)q;
    }

    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}