using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LegoTuringMachine.Implements
{
	public class ThreadToken
	{
		private readonly object _syncObj = new object();

		public ThreadToken()
		{
			WaitHandle = new ManualResetEvent(true);
		}

		protected EventWaitHandle WaitHandle { get; private set; }

		public bool IsWaitingRequested { get; private set; }

		public bool IsCancellationRequested { get; private set; }

		public void Cancel()
		{
			lock (_syncObj)
			{
				IsCancellationRequested = true;
			}
		}

		public void Pause()
		{
			lock (_syncObj)
			{
				WaitHandle.Reset();
				IsWaitingRequested = true;
			}
		}

		public void Continue()
		{
			lock (_syncObj)
			{
				WaitHandle.Set();
				IsWaitingRequested = false;
			}
		}

		public void WaitIfWaitingRequested()
		{
			WaitHandle.WaitOne();
		}

		public void ThrowIfCancellationRequested()
		{
			lock (_syncObj)
			{
				if (IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}
			}
		}

		public void Reset()
		{
			lock (_syncObj)
			{
				WaitHandle.Set();
				IsWaitingRequested = false;
				IsCancellationRequested = false;
			}
		}
	}
}
