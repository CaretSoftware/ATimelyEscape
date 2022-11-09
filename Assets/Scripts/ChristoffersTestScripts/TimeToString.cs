
using System.Text;
using UnityEngine;

public class TimeToString
{
	public static readonly int Minute = 60;
	public static readonly int Hour = 60 * Minute;
	public static readonly int Day = 24 * Hour;
	public static readonly int Week = 7 * Day;
	public static readonly int Month = 30 * Day;
	public static readonly int Year = 365 * Day;
	//private const float StartTime = Year + Month + Week + Day + Hour + Minute + 59;
	//private float time;

/*	private void Update()
	{
		time = StartTime - Time.time;
		// Debug.Log(time);
		TimeAsString(time, );
	}*/

	public static string TimeAsString(float time)
	{
		StringBuilder sb = new StringBuilder();

		int years = (int)(time / Year);
		time %= Year;
		int months = (int)(time / Month);
		time %= Month;
		int weeks = (int)(time / Week);
		time %= Week;
		int days = (int)(time / Day);
		time %= Day;
		int hours = (int)(time / Hour);
		time %= Hour;
		int minutes = (int)(time / Minute);
		time %= Minute;
		int seconds = (int)time;

		sb.AppendFormat("TIME UNTIL DONE \n YEARS: {0} \n MONTHS: {1} \n WEEKS: {2} \n DAYS: {3} \n HOURS: {4} \n MIN: {5} \n SEC: {6}",
			years, months, weeks, days, hours, minutes, seconds);
		// Debug.Log(sb.ToString());
		return sb.ToString();
	}
}
