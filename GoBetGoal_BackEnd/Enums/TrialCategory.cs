using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Enums
{
    [Flags]
    public enum TrialCategory
    {
        運動 = 1,
        飲食 = 2,
        作息 = 4,
    }
}