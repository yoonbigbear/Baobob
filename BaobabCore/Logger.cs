namespace BaobobCore
{
	using System;
	using System.IO;
	using System.Threading.Tasks;

	public static class Logger
	{
		private static TaskFactory taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);

		public static bool Assert(bool condition)
		{
			System.Diagnostics.Debug.Assert(condition);
			return condition;
		}

		public static bool Assert(bool condition, string msg)
		{
			System.Diagnostics.Debug.Assert(condition, msg);
			return condition;
		}

		public static void Trace(string str)
		{
#if DEBUG
			taskFactory.StartNew(() =>
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine($"[trace] {DateTimeOffset.Now.DateTime} {str}");
				Console.ForegroundColor = ConsoleColor.Gray;
			});
		}

#endif

		public static void Debug(string str)
		{
#if DEBUG
			taskFactory.StartNew(() =>
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"[debug] {DateTimeOffset.Now.DateTime} {str}");
				Console.ForegroundColor = ConsoleColor.Gray;
			});
#endif
		}

		public static void Warn(string str)
		{
#if DEBUG
			taskFactory.StartNew(() =>
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"[warn] {DateTimeOffset.Now.DateTime} {str}");
				Console.ForegroundColor = ConsoleColor.Gray;
			});
#endif
		}

		public static void Error(string str)
		{
#if DEBUG
			taskFactory.StartNew(() =>
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[error] {DateTimeOffset.Now.DateTime} {str}");
				Console.ForegroundColor = ConsoleColor.Gray;
			});
#endif
		}

		public static void Success(string str)
		{
#if DEBUG
			taskFactory.StartNew(() =>
			{
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine($"[Success] {DateTimeOffset.Now.DateTime} {str}");
				Console.ForegroundColor = ConsoleColor.Gray;
			});
#endif
		}

		//file i/o
		private static void FileIO(string str)
		{
			string path = Directory.GetCurrentDirectory() + "\\log";
			string filename = $"\\log_{DateTimeOffset.Now.Date.ToShortDateString()}.log";

			taskFactory.StartNew(async () =>
			{
				if (!Directory.Exists($"{path}"))
				{
					Directory.CreateDirectory($"{path}");
				}
				using (var sw = File.AppendText($"{path}{filename}"))
				{
					await sw.WriteLineAsync(str).ConfigureAwait(false);
				}
			});
		}

		public static void LogInfo(string str) => FileIO($"[Info] {DateTimeOffset.Now.DateTime} {str}");

		public static void LogWarning(string str) => FileIO($"[Warning] {DateTimeOffset.Now.DateTime} {str}");

		public static void LogError(string str) => FileIO($"[Error] {DateTimeOffset.Now.DateTime} {str}");
	}

	public class GameLog
	{
		private static TaskFactory taskFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);

		//file i/o
		private static void FileIO(string str)
		{
			string path = Directory.GetCurrentDirectory() + "\\game_log";
			string filename = $"\\log_{DateTimeOffset.Now.Date.ToShortDateString()}.log";

			taskFactory.StartNew(async () =>
			{
				if (!Directory.Exists($"{path}"))
				{
					Directory.CreateDirectory($"{path}");
				}

				using (var sw = File.AppendText($"{path}{filename}"))
				{
					await sw.WriteLineAsync(str).ConfigureAwait(false);
				}
			});
		}

		public static void Write<T>(T type, object[] param) where T : System.Enum
		{
#if NET5_0_OR_GREATER
			var json = System.Text.Json.JsonSerializer.Serialize(param);

			System.Text.StringBuilder log = new();
			log.Append("{");
			log.Append($"\"{type.ToString()}\":");
			log.Append(json);
			log.Append("}");

#if DEBUG
			Logger.Trace($"{DateTimeOffset.Now.DateTime} {log}");
#else
		FileIO($"[Trace] {DateTimeOffset.Now.DateTime} {log}");
#endif
#else
#endif
			throw new NotImplementedException();
		}

		public static void Write<T>(T type, object param) where T : System.Enum =>
			Write(type, new object[] { param });
	}
}