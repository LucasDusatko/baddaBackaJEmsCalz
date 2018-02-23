// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoleApp1
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;

	public class Program
	{
		public static void Main(string[] args)
		{
			var timeStamps = generateSimpleClockStamps(HoursDay: 8, NumberOfDays: 5);
			var timeSheet = CalculateHours(timeStamps); 
			var timeSheet = CheckForAdditionalClockInfo(timeSheet);
			Console.Write(PrintPrettyHours(timeSheet));


			var timeStamps2 = generateClockStampsWithLunches(HoursDay: 24, NumberOfDays: 2);
			var timeSheet2 = CalculateHours(timeStamps2);
			var timeSheet2 = CheckForAdditionalClockInfo(timeSheet2);
			Console.Write(PrintPrettyHours(timeSheet2));
		}

		// Todo take into account lunches and new days and missed clocks.
		// This required that it be sent an array of timestamps for 1 weeks pay
		private static TimeSheet CalculateHours(List<ClockStamp> timeStamps)
		{
			if (timeStamps.Count < 0)
				return new TimeSheet { };

			var timeSheet = new TimeSheet() { EmployeeId = timeStamps.First().EmployeeId };
			var calcDate = DateTime.MinValue;
			var currentDate = DateTime.MinValue;
			var unAddedHours = 0m;
			foreach (var timeStamp in timeStamps)
			{
				// Need to do this but by day instead of by clock out
				if (timeStamp.In)
				{
					if (timeStamp.TimeStamp.Date > currentDate)
					{
						timeSheet = CalculateWorkDay(timeSheet, unAddedHours);
						unAddedHours = 0;
					}

					calcDate = timeStamp.TimeStamp;
					currentDate = timeStamp.TimeStamp.Date;
					continue;
				}

				var hoursForDay = RoundTimeStamp(timeSpan: timeStamp.TimeStamp - calcDate);
				unAddedHours += hoursForDay;
			}

			if (unAddedHours > 0)
			{
				timeSheet = CalculateWorkDay(timeSheet, unAddedHours);
			}

			return timeSheet;
		}

		private static TimeSheet CalculateWorkDay(TimeSheet timeSheet, decimal hoursForDay)
		{
			var WORK_WEEK = 40;
			var WORK_DAY = 8;
			var DOUBLETIME = 12;

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
			return timeSheet;
		}

		// Round time stamp to nearest quarter hour
		private static decimal RoundTimeStamp(TimeSpan timeSpan)
		{
			return (decimal)Math.Round(timeSpan.TotalHours * 4, MidpointRounding.ToEven) / 4;
		}

		private static List<ClockStamp> generateSimpleClockStamps(double HoursDay, int NumberOfDays)
		{
			// The list were adding too
			var list = new List<ClockStamp>();
			// The clock Id
			var id = 0;
			// The Date Time we will use to manipulate
			var dateTime = DateTime.Now.AddDays(-7);
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

		private static List<ClockStamp> generateClockStampsWithLunches(double HoursDay, int NumberOfDays)
		{
			// The list were adding too
			var list = new List<ClockStamp>();
			// The clock Id
			var id = 100;
			// The Date Time we will use to manipulate
			var dateTime = DateTime.Now.AddDays(-7);
			// The Id of the empoyee's clock in's
			var employeeId = 2;
			// The person who logged the timestamp
			var createdBy = "Bob";
			// Loops according to number of days
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
						                 TimeStamp = dateTime.AddHours(-HoursDay / 2),
						                 In = false
					                 };
				list.Add(timestamp2);
				var timestamp3 = new ClockStamp()
					                 {
						                 Id = id++,
						                 EmployeeId = employeeId,
						                 CreatedBy = createdBy,
						                 TimeStamp = dateTime.AddHours(-HoursDay / 2).AddMinutes(30),
						                 In = true
					                 };
				list.Add(timestamp3);
				var timestamp4 = new ClockStamp()
					                 {
						                 Id = id++,
						                 EmployeeId = employeeId,
						                 CreatedBy = createdBy,
						                 TimeStamp = dateTime.AddMinutes(30),
						                 In = false
					                 };
				list.Add(timestamp4);
				dateTime = dateTime.AddDays(1);
			}
			return list;
		}

		private static string PrintPrettyHours(TimeSheet timeSheet)
		{
			return
				$"Regular Hours = {timeSheet.RegularTime}\r\nOvertime = {timeSheet.OverTime}\r\nDoubletime = {timeSheet.DoubleTime}\r\n \r\n";
		}
	}

	// The clock in's and outs of an employee
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

	// Additional Information related to employee's work record
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

	// An Employee's Timecard 
	public class TimeSheet
	{
		public decimal RegularTime { get; set; } = 0;
		public decimal OverTime { get; set; } = 0;
		public decimal DoubleTime { get; set; } = 0;
		public int EmployeeId { get; set; }
		public int WorkDay { get; set; } 
		public int WorkWeek { get; set; } 
	}

}
