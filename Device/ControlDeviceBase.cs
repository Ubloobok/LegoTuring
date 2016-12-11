using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.Model;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace LegoTuringMachine.Device
{
	public abstract class ControlDeviceBase : IControlDevice
	{
		private DevicePositionState _positionState;
		private DeviceConnectionState _connectionState;

		/// <summary>
		/// Gets default position state for the device.
		/// </summary>
		public DevicePositionState DefaultPositionState
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets current position of the device.
		/// </summary>
		public DevicePositionState PositionState
		{
			get { return _positionState; }
			set
			{
				var oldValue = _positionState;
				_positionState = value;
				OnPositionChanged(oldValue, value);
				if (PositionStateChanged != null)
				{
					PositionStateChanged(this, new EventArgs());
				}
			}
		}

		/// <summary>
		/// Occurs when the current position state has been changed.
		/// </summary>
		public event EventHandler PositionStateChanged;

		/// <summary>
		/// Called when the current position has been changd.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnPositionChanged(DevicePositionState oldValue, DevicePositionState newValue)
		{
		}

		/// <summary>
		/// Gets current connection state of the device.
		/// </summary>
		public DeviceConnectionState ConnectionState
		{
			get { return _connectionState; }
			protected set
			{
				_connectionState = value;
				if (ConnectionStateChanged != null)
				{
					ConnectionStateChanged(this, new EventArgs());
				}
			}
		}

		/// <summary>
		/// Occurs when the current connection state has been changed.
		/// </summary>
		public event EventHandler ConnectionStateChanged;

		/// <summary>
		/// Gets the current device settings.
		/// </summary>
		protected DeviceSettings Settings { get; private set; }
		/// <summary>
		/// Applies the settings.
		/// </summary>
		/// <param name="deviceSettings">The device settings.</param>
		public virtual void ApplySettings(DeviceSettings deviceSettings)
		{
			OnSettingsApplyingOverride(Settings, deviceSettings);
			var oldValue = Settings;
			Settings = deviceSettings;
			OnSettingsAppliedOverride(oldValue, deviceSettings);
		}

		/// <summary>
		/// Called when the device settings are applying.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnSettingsApplyingOverride(DeviceSettings oldValue, DeviceSettings newValue)
		{
		}

		/// <summary>
		/// Called when the device settings has been applyed.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnSettingsAppliedOverride(DeviceSettings oldValue, DeviceSettings newValue)
		{
		}

		/// <summary>
		/// Connects to the device.
		/// </summary>
		public abstract void Connect();

		/// <summary>
		/// Disconnects from the device.
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		/// Moves the device asynchronously by the concrete cells count.
		/// </summary>
		/// <param name="direction">The moving direction.</param>
		/// <param name="distance">The distance calculated in cells count.</param>
		public abstract Task<MoveCompletedEventArgs> MoveAsync(Direction direction, int distance);

		/// <summary>
		/// Occurs when the moving has been completed.
		/// </summary>
		public event EventHandler<MoveCompletedEventArgs> MoveCompleted;

		/// <summary>
		/// Raises the move completed event.
		/// </summary>
		/// <param name="e">The <see cref="LegoTuringMachine.Device.MoveCompletedEventArgs"/> instance containing the event data.</param>
		protected void RaiseMoveCompleted(MoveCompletedEventArgs e)
		{
			if (MoveCompleted != null)
			{
				MoveCompleted(this, e);
			}
		}

		/// <summary>
		/// Stop engine motors, with reseting status and state.
		/// </summary>
		public virtual void StopMoveAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		/// <param name="intervalsCount">Intervals count in the cell.</param>
		public virtual Task<ReadCompletedEventArgs> ReadIntervalAsync(int intervalsCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		public virtual Task<ReadCompletedEventArgs> ReadTranslatedValueAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Occurs when the reading has been completed.
		/// </summary>
		public event EventHandler<ReadCompletedEventArgs> ReadCompleted;

		/// <summary>
		/// Raises the read completed.
		/// </summary>
		/// <param name="e">The <see cref="LegoTuringMachine.Device.ReadCompletedEventArgs"/> instance containing the event data.</param>
		protected void RaiseReadCompleted(ReadCompletedEventArgs e)
		{
			if (ReadCompleted != null)
			{
				ReadCompleted(this, e);
			}
		}

		/// <summary>
		/// Rotates asynchronously manipulator by concrete degree.
		/// </summary>
		public virtual void RotateManipulatorByDegreeAsync(Direction direction, int degree)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Writes the new interval value into the current cell asynchronously.
		/// </summary>
		/// <param name="newInterval">The new interval value, must be in the [1, *] range.</param>
		/// <param name="oldInterval">The old inteval value, must be in the [1, *] range or null.</param>
		/// <param name="intervalsCount">The total intervals count, must be in the [1, *] range.</param>
		public virtual Task<WriteCompletedEventArgs> WriteIntervalAsync(int newInterval, int oldInterval, int intervalsCount)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Occurs when the writing has been completed.
		/// </summary>
		public event EventHandler<WriteCompletedEventArgs> WriteCompleted;

		/// <summary>
		/// Stop manipulator motor, with reseting status and state.
		/// </summary>
		public virtual void StopWriteAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Raises the write completed.
		/// </summary>
		/// <param name="e">The <see cref="LegoTuringMachine.Device.WriteCompletedEventArgs"/> instance containing the event data.</param>
		protected void RaiseWriteCompleted(WriteCompletedEventArgs e)
		{
			if (WriteCompleted != null)
			{
				WriteCompleted(this, e);
			}
		}

		protected void DoAsync(Action action)
		{
			ThreadPool.QueueUserWorkItem(state =>
			{
				action();
			});
		}
	}
}
