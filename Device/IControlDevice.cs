using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.Model;
using System.Threading.Tasks;

namespace LegoTuringMachine.Device
{
	public enum DeviceConnectionState
	{
		Disconnected,
		Connecting,
		Connected,
		Disconnecting,
	}

	public enum DevicePositionState
	{
		Unknown,
		ForWriting,
		ForReading,
	}

	public interface IControlDevice
	{
		/// <summary>
		/// Gets default position state for the device.
		/// </summary>
		DevicePositionState DefaultPositionState { get; }

		/// <summary>
		/// Gets current position of the device.
		/// </summary>
		DevicePositionState PositionState { get; }

		/// <summary>
		/// Occurs when the current position state has been changed.
		/// </summary>
		event EventHandler PositionStateChanged;

		/// <summary>
		/// Gets current connection state of the device.
		/// </summary>
		DeviceConnectionState ConnectionState { get; }

		/// <summary>
		/// Occurs when the current connection state has been changed.
		/// </summary>
		event EventHandler ConnectionStateChanged;

		/// <summary>
		/// Applies the settings.
		/// </summary>
		/// <param name="deviceSettings">The device settings.</param>
		void ApplySettings(DeviceSettings deviceSettings);

		/// <summary>
		/// Connects to the device.
		/// </summary>
		void Connect();

		/// <summary>
		/// Disconnects from the device.
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Moves the device asynchronously by the concrete cells count.
		/// </summary>
		/// <param name="direction">The moving direction.</param>
		/// <param name="distance">The distance calculated in cells count.</param>
		Task<MoveCompletedEventArgs> MoveAsync(Direction direction, int distance);

		/// <summary>
		/// Stop engine motors, with reseting status and state.
		/// </summary>
		void StopMoveAsync();

		/// <summary>
		/// Occurs when the moving has been completed.
		/// </summary>
		event EventHandler<MoveCompletedEventArgs> MoveCompleted;

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		/// <param name="intervalsCount">Intervals count in the cell.</param>
		Task<ReadCompletedEventArgs> ReadIntervalAsync(int intervalsCount);

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		Task<ReadCompletedEventArgs> ReadTranslatedValueAsync();

		/// <summary>
		/// Occurs when the reading has been completed.
		/// </summary>
		event EventHandler<ReadCompletedEventArgs> ReadCompleted;

		/// <summary>
		/// Rotates asynchronously manipulator by concrete degree.
		/// </summary>
		void RotateManipulatorByDegreeAsync(Direction direction, int degree);

		/// <summary>
		/// Writes the new interval value into the current cell asynchronously.
		/// </summary>
		/// <param name="newInterval">The new interval value, must be in the [1, *] range.</param>
		/// <param name="oldInterval">The old inteval value, must be in the [1, *] range or null.</param>
		/// <param name="intervalsCount">The total intervals count, must be in the [1, *] range.</param>
		Task<WriteCompletedEventArgs> WriteIntervalAsync(int newInterval, int oldInterval, int intervalsCount);

		/// <summary>
		/// Stop manipulator motor, with reseting status and state.
		/// </summary>
		void StopWriteAsync();

		/// <summary>
		/// Occurs when the writing has been completed.
		/// </summary>
		event EventHandler<WriteCompletedEventArgs> WriteCompleted;
	}
}
