
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
}