using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			var timeStamps = GenerateClockStamps(HoursDay: 8.41, NumberOfDays: 5);
			var paidHours = CalculateHours(timeStamps);
			Console.Write($"Regular Hours = {paidHours.RegularTime}\r\nOvertime = {paidHours.OverTime}\r\nDoubletime = {paidHours.DoubleTime}\r\n \r\n");
		}

		// Todo replace all numbers with variables
		// Todo take into account lunches and new days and missed clocks.
		// This required that it be sent an array of timestamps for 1 weeks pay
		public static TimeSheet CalculateHours(List<ClockStamp> timeStamps)
		{
			var WORK_WEEK = 40;
			var WORK_DAY = 10;
			var DOUBLETIME = 12;

			if (timeStamps.Count < 0)
				return new TimeSheet { };

			var timeSheet = new TimeSheet() { EmployeeId = timeStamps.First().EmployeeId };
			var calcDate = DateTime.MinValue;
			foreach (var timeStamp in timeStamps)
			{
				if (timeStamp.In)
				{
					calcDate = timeStamp.TimeStamp;
					continue;
				}
				var hoursForDay = RoundTimeStamp(timeSpan: timeStamp.TimeStamp - calcDate);
				if (hoursForDay > DOUBLETIME)
				{
					if (timeSheet.RegularTime + WORK_DAY > WORK_WEEK)
					{
						timeSheet.OverTime += timeSheet.RegularTime + WORK_DAY - WORK_WEEK;
						timeSheet.RegularTime = WORK_WEEK;
					}
					else
						timeSheet.RegularTime += WORK_DAY;

					timeSheet.OverTime += DOUBLETIME - WORK_DAY;
					timeSheet.DoubleTime += hoursForDay - DOUBLETIME;
				}
				else if (hoursForDay > WORK_DAY)
				{
					if (timeSheet.RegularTime + hoursForDay > WORK_WEEK)
					{
						timeSheet.OverTime += timeSheet.RegularTime + WORK_DAY - WORK_WEEK;
						timeSheet.RegularTime = WORK_WEEK;
					}
					else
						timeSheet.RegularTime += WORK_DAY;

					timeSheet.OverTime += hoursForDay - WORK_DAY;
				}
				else
				{
					if (timeSheet.RegularTime + hoursForDay > WORK_WEEK)
					{
						timeSheet.OverTime += timeSheet.RegularTime + hoursForDay - WORK_WEEK;
						timeSheet.RegularTime = WORK_WEEK;
					}
					else
						timeSheet.RegularTime += hoursForDay;
				}

			}
			return timeSheet;
		}

		// Round time stamp to nearest quarter hour
		public static decimal RoundTimeStamp(TimeSpan timeSpan)
		{
			return (decimal) Math.Round(timeSpan.TotalHours * 4, MidpointRounding.ToEven) / 4;
		}

		public static List<ClockStamp> GenerateClockStamps(double HoursDay, int NumberOfDays)
		{
			// The list were adding too
			var list = new List<ClockStamp>();
			// The clock Id
			var id = 0;
			// The Date Time we will use to manipulate
			var dateTime = DateTime.Now.AddDays(-HoursDay);
			// The Id of the empoyee's clock in's
			var employeeId = 1;
			// The person who logged the timestamp
			var createdBy = "Lucas";
			// Loop 5 times created 5 days worth of stamps, Should come out to 40 hours
			for (int i = 0; i < NumberOfDays; i++)
			{
				var timestamp1 = new ClockStamp()
				{
					Id = id++,
					EmployeeId = employeeId,
					CreatedBy = createdBy,
					TimeStamp = dateTime.AddHours(-HoursDay),
					In = true
				};
				list.Add(timestamp1);
				var timestamp2 = new ClockStamp()
				{
					Id = id++,
					EmployeeId = employeeId,
					CreatedBy = createdBy,
					TimeStamp = dateTime,
					In = false
				};
				list.Add(timestamp2);
				dateTime = dateTime.AddDays(NumberOfDays);
			}

			return list;
		}
	}

	public class ClockStamp
	{
		public int Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public int EmployeeId { get; set; }
		public string CreatedBy { get; set; }
		public bool Archived { get; set; } = false;
		public bool In { get; set; } = false;
		public string Comments { get; set; }
		public string EditedBy { get; set; }
		public DateTime? EditedDate { get; set; }
	}

	public class AdditionalTimeClockInfo
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public decimal Hours { get; set; }
		public DateTime Date { get; set; }
		public string CreatedBy { get; set; }
		public bool Approved { get; set; }
		public string EditedBy { get; set; }
		public DateTime? DatedEdited { get; set; }

	}

	public class TimeSheet
	{
		public decimal RegularTime { get; set; } = 0;
		public decimal OverTime { get; set; } = 0;
		public decimal DoubleTime { get; set; } = 0;
		public int EmployeeId { get; set; }
	}

}
