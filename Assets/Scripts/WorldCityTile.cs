using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCityTile : WorldTile
{
    public SoundData zoom;
  
    public override void Click()
    {
        base.Click();
        AudioManager.inst.GetSoundEffect().Play(zoom);

        
    }

}
