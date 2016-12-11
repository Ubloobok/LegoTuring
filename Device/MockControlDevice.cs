using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegoTuringMachine.Device
{
	public class MockControlDevice : ControlDeviceBase, IControlDevice
	{
		private readonly int _operationTimeoutMs;

		public MockControlDevice()
			: this(1000)
		{
		}

		public MockControlDevice(int operationTimeoutMs)
		{
			_operationTimeoutMs = operationTimeoutMs;
			DefaultPositionState = DevicePositionState.ForReading;
			CurrentCellIndex = 0;
			Cells = new Dictionary<int, double>();
		}

		/// <summary>
		/// Gets current cell index.
		/// It's using for stubing readed values.
		/// </summary>
		public int CurrentCellIndex { get; private set; }

		/// <summary>
		/// Gets dictionary with cells values.
		/// </summary>
		public Dictionary<int, double> Cells { get; private set; }

		/// <summary>
		/// Connects to the device.
		/// </summary>
		public override void Connect()
		{
			ConnectionState = DeviceConnectionState.Connected;
		}

		/// <summary>
		/// Disconnects from the device.
		/// </summary>
		public override void Disconnect()
		{
			ConnectionState = DeviceConnectionState.Disconnected;
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		/// <param name="intervalsCount">Intervals count in the cell.</param>
		public override Task<ReadCompletedEventArgs> ReadIntervalAsync(int intervalsCount)
		{
			var t = Task<ReadCompletedEventArgs>.Factory.StartNew(() =>
			{
				Thread.Sleep(_operationTimeoutMs);

				double? translatedValue = null;
				double tempTranslatedValue;
				if (Cells.TryGetValue(CurrentCellIndex, out tempTranslatedValue))
				{
					translatedValue = tempTranslatedValue;
				}

				int? interval = null;
				if (translatedValue.HasValue)
				{
					double intervalStep = 1.0 / (double)intervalsCount;
					for (int i = 1; i < intervalsCount + 1; i++)
					{
						if (translatedValue.Value < i * intervalStep)
						{
							interval = i;
							break;
						}
					}
				}

				var e = new ReadCompletedEventArgs(interval.HasValue ? interval.Value : 1, translatedValue);
				return e;
			});
			return t;
		}

		/// <summary>
		/// Writes the new interval value into the current cell asynchronously.
		/// </summary>
		/// <param name="newInterval">The new
		/// interval value, must be in the [1, *] range.</param>
		/// <param name="oldInterval">The old inteval value, must be in the [1, *] range or null.</param>
		/// <param name="intervalsCount">The total intervals count, must be in the [1, *] range.</param>
		public override Task<WriteCompletedEventArgs> WriteIntervalAsync(int newInterval, int oldInterval, int intervalsCount)
		{
			var t = Task<WriteCompletedEventArgs>.Factory.StartNew(() =>
			{
				Thread.Sleep(_operationTimeoutMs);

				double intervalStep = 1.0 / (double)intervalsCount;
				double translatedValue = (((newInterval - 1) * intervalStep) + (newInterval * intervalStep)) / 2;
				if (Cells.ContainsKey(CurrentCellIndex))
				{
					Cells[CurrentCellIndex] = translatedValue;
				}
				else
				{
					Cells.Add(CurrentCellIndex, translatedValue);
				}

				var e = new WriteCompletedEventArgs();
				return e;
			});
			return t;
		}

		/// <summary>
		/// Moves the device asynchronously by the concrete cells count.
		/// </summary>
		/// <param name="direction">The moving direction.</param>
		/// <param name="distance">The distance calculated in cells count.</param>
		public override Task<MoveCompletedEventArgs> MoveAsync(Direction direction, int distance)
		{
			var t = Task<MoveCompletedEventArgs>.Factory.StartNew(() =>
			{
				Thread.Sleep(_operationTimeoutMs);
				int directionSign =
					direction == Direction.Left ? -1 :
					direction == Direction.Right ? +1 :
					0;;
				int newCurrentCellIndex = CurrentCellIndex + (directionSign * distance);
				if (newCurrentCellIndex < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				CurrentCellIndex = newCurrentCellIndex;
				var e = new MoveCompletedEventArgs();
				return e;
			});
			return t;
		}

		/// <summary>
		/// Stop engine motors, with reseting status and state.
		/// </summary>
		public override void StopMoveAsync()
		{
			//base.StopMoveAsync();
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		public override void StopWriteAsync()
		{
			//base.StopWriteAsync();
		}

		/// <summary>
		/// Rotates asynchronously manipulator by concrete degree.
		/// </summary>
		public override void RotateManipulatorByDegreeAsync(Direction direction, int degree)
		{
			//base.RotateManipulatorByDegreeAsync(direction, degree);
		}
	}
}
