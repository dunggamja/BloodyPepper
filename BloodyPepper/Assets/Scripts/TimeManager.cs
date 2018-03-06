using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    //public double TotalPlayTime { get; private set; }

    //private void Awake()
    //{
    //    //TotalPlayTime = 0f;
    //}

    //// Update is called once per frame
    //public void Update(float aDeltaTime)
    //{
    //    TotalPlayTime += aDeltaTime;
    //}

    private static System.Text.StringBuilder commonSB = new System.Text.StringBuilder(32);
    public static string TimeToString(double aTime, int decimalPoint = 0)
    {
        // 00:00:00 (시:분:초 로 표시를 한다.)   
        // 소수점 자리를 표시하려면 decimalPoint의 자리수를 늘린다.
        commonSB.Length = 0;

        Int64 iTime = (Int64)aTime;

        Int64 iHour = iTime / DEFINE.SECOND_PER_HOUR;
        Int64 iMinute = (iTime / DEFINE.SECOND_PER_MINUTE) % 60;
        Int64 iSecond = iTime % DEFINE.SECOND_PER_MINUTE;

        commonSB.AppendFormat("{0:D2}:", iHour);
        commonSB.AppendFormat("{0:D2}:", iMinute);
        commonSB.AppendFormat("{0:D2}", iSecond);

        if (0 < decimalPoint)
        {
            string strFormat = string.Format("D{0}", decimalPoint);
            int iPow = (int)Math.Pow(10, decimalPoint);
            int iDecimal = ((int)(aTime * iPow)) % iPow;

            commonSB.AppendFormat(".{0}", iDecimal.ToString(strFormat));
        }

        return commonSB.ToString();
    }
}
