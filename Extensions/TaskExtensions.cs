using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegoTuringMachine.Extensions
{
	public static class TaskExtensions
	{
		public static void Await(this Task task)
		{
			if (task != null)
			{
				task.Wait();
			}
		}

		public static T Await<T>(this Task<T> task)
		{
			T result = default(T);
			if (task != null)
			{
				task.Wait();
				result = task.Result;
			}
			return result;
		}

		public static void WaitAny<T>(this Task<T>[] tasks)
		{
			if (tasks != null)
			{
				Task.WaitAny(tasks);
			}
		}

		public static void WaitAll<T>(this Task<T>[] tasks)
		{
			if (tasks != null)
			{
				Task.WaitAll(tasks);
			}
		}
	}
}
