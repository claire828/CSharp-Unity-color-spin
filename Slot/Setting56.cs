using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting56 : BaseSlotSetting
{
    public override int ID { get { return 56; } }
    public override bool IsHaveLogo { get { return false; } }
    public override bool IsHaveLanguageLogoName { get { return true; } }
    public override bool IsHaveFreeSpin { get { return true; } }
    public override bool IsHaveBonusGame { get { return true; } }
    public override bool IsHaveSlotBg { get { return true; } }
    public override bool IsHaveFreeSpinBg { get { return false; } }
    public override bool Is3D { get { return base.Is3D; } }

}
