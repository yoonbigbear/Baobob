using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Project
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			//DoSomething().Wait();

			Deadlock();

		}
		static async Task DoSomething()
		{
			int value = 13;//호출 스레드

			await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

			value *= 2; // 스레드풀 스레드

			await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

			Trace.WriteLine(value);
		}

		static async Task WaitAsync()
		{
			await Task.Delay(TimeSpan.FromSeconds(1));

		}

		static void Deadlock()
		{//싱글 스레드 컨텍스트라면 데드락 상태에 빠진다.
		 //데드락을 벗어나려면 ConfigureAwait(false)를 통해 context를 무기하던가 WaitAsync를 await로 기다리는 방법이 있다.
			Task task = WaitAsync();
			task.Wait();

		}

		static async Task<T> DelayResult<T>(T result, TimeSpan delay)
		{
			// 안에 취소 토큰을 넣으면 정해진 시간 뒤에 취소할 타임아웃 작업을 만들 수 있다.
			await Task.Delay(delay);
			return result;
		}

		async Task<string> DownloadStringWithRetries(HttpClient client, string uri)
		{
			TimeSpan nextDelay = TimeSpan.FromSeconds(1);
			for (int i = 0; i != 3; ++i)
			{
				try
				{
					return await client.GetStringAsync(uri);
				}
				catch
				{

				}
				await Task.Delay(nextDelay);
				nextDelay = nextDelay + nextDelay;
			}
			return await client.GetStringAsync(uri);
		}

		async Task<string> DownloadStringWithTimeout(HttpClient client, string uri)
		{// 3초 안에 응답이 없으면 null 반환
			using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
			{
				var downloadTask = client.GetStringAsync(uri);
				Task timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

				Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);
				if (completedTask == timeoutTask)
					return null;
				return await downloadTask;
			}
		}

		//비동기 시그니처를 사용해서 동기 메서드를 구현한다. 비동기 인터페이스나 비동기 클래스를 상속하고 있지만
		//동기적으로 구현하고 싶을 때 발생할 수 있음.
		// 비동기 코드를 단위 테스트 하면서 비동기 인터페이스에 사용할 간단한 스텁이나 목이 필요할 때 특히 유용
		interface IMyAsyncInterface
		{
			Task<int> GetValueAsync();
			Task DoSomethingAsync();
			Task<T> NotImplementedAsync<T>();
			Task<int> GetValueAsync(CancellationToken cancellation);
		}
		class MySynchronousImpl : IMyAsyncInterface
		{
			public Task<int> GetValueAsync()
			{
				return Task.FromResult(13);
			}

			public Task DoSomethingAsync()
			{
				try
				{
					//DoSomethingAsync();
					return Task.CompletedTask;
				}
				catch (Exception ex)
				{
					return Task.FromException(ex);
				}
			}

			Task<T> IMyAsyncInterface.NotImplementedAsync<T>()
			{
				return Task.FromException<T>(new NotImplementedException());
			}

			Task<int> IMyAsyncInterface.GetValueAsync(CancellationToken cancellationToken)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<int>(cancellationToken);
				}
				return Task.FromResult(13);
			}
		}

		// 만약 주기적으로 같은 값을 사용한다면 미리 결과값을 만들어놓고 gc로 사라지지 않도록 만들어 저장해둘 수 있다.
		private static readonly Task<int> zeroTask = Task.FromResult(0);
		Task<int> GetValueAsync()
		{
			return zeroTask;
		}


		async Task MyMethodAsync(IProgress<double> progress = null)
		{// IProgress<T>와 Progress<T>형식으로 진행률을 알 수 있다.
			bool done = false;
			double percentComplete = 0;
			while(!done)
			{
				//...
				progress?.Report(percentComplete);
			}
		}

		
		// UI 컨텍스트 같이 여러 비동기 메서드가 같은 컨텍스트에서 동작하지 않도록 하려면
		async Task ResumeOnContextAsync()
		{
			await Task.Delay(TimeSpan.FromSeconds(1));
		}
		async Task ResumeWithoutContextAsync()
		{
			await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
		}
		// UI 스레드에 많은 수의 연속 작업을 실행하면 성능 문제가 발생한다. 단순히 연속 작업때문이 아니라 어플리케이션이 복잡해질 수록 더 그렇다.
		// 알려지기로는 초당 백여 개 정도는 괜찮지만 천 개 정도는 너무 많다고 한다.
	}
}
