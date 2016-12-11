using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using AForge.Robotics.Lego;
using LegoTuringMachine.Model;
using LegoTuringMachine.Extensions;

namespace LegoTuringMachine.Device
{
	public class AForgeControlDevice : ControlDeviceBase
	{
		private NXTBrick Brick { get; set; }

		public AForgeControlDevice()
		{
			Brick = new NXTBrick();
		}

		/// <summary>
		/// Called when the device settings are applying.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected override void OnSettingsApplyingOverride(DeviceSettings oldValue, DeviceSettings newValue)
		{
			//if (Brick.IsConnected)
			//{
			//    throw new ApplicationException("Brick.IsConnected = true");
			//}
		}

		/// <summary>
		/// Called when the device settings has been applyed.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected override void OnSettingsAppliedOverride(DeviceSettings oldValue, DeviceSettings newValue)
		{
		}

		/// <summary>
		/// Connects to the device.
		/// </summary>
		public override void Connect()
		{
			if (Brick.IsConnected)
			{
				throw new ApplicationException("Brick.IsConnected = true");
			}
			bool isBrickConnected = Brick.Connect(Settings.DeviceComPort);
			if (!isBrickConnected)
			{
				throw new ApplicationException("Не удалось подключиться к устройству.");
			}
			Brick.SetSensorMode(Settings.SonarPort.ToAForge(), NXTBrick.SensorType.Lowspeed9V, NXTBrick.SensorMode.Raw);
			Brick.SetSensorMode(Settings.LeftTouchPort.ToAForge(), NXTBrick.SensorType.Switch, NXTBrick.SensorMode.Boolean);
			Brick.SetSensorMode(Settings.RightTouchPort.ToAForge(), NXTBrick.SensorType.Switch, NXTBrick.SensorMode.Boolean);
			ConnectionState = DeviceConnectionState.Connected;
		}

		/// <summary>
		/// Disconnects from the device.
		/// </summary>
		public override void Disconnect()
		{
			if (!Brick.IsConnected)
			{
				throw new ApplicationException("Brick.IsConnected != true");
			}
			Brick.Disconnect();
			ConnectionState = DeviceConnectionState.Disconnected;
		}

		/// <summary>
		/// Moves the device asynchronously by the concrete cells count.
		/// </summary>
		/// <param name="direction">The moving direction.</param>
		/// <param name="distance">The distance calculated in cells count.</param>
		public override Task<MoveCompletedEventArgs> MoveAsync(Direction direction, int distance)
		{
			int tachoCount = Settings.EngineStepAngle * distance;
			var t = RotateMotorsAsync(direction, tachoCount);
			return t;
		}

		private Task<MoveCompletedEventArgs> RotateMotorsAsync(Direction direction, int tachoCount)
		{
			var t = Task<MoveCompletedEventArgs>.Factory.StartNew(() =>
			{
				int power = Settings.EngineStepPower;
				if (direction == Direction.Left)
				{
					power = power * -1;
				}
				int translatedTachoCount = tachoCount;
				List<Task> tasks = new List<Task>();
				if (direction == Direction.Left)
				{
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.C))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.C, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.B))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.B, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.A))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.A, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
				}
				else
				{
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.A))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.A, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.B))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.B, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
					if (Settings.EnginePorts.HasFlag(DeviceMotorPort.C))
					{
						var task = RotateMotorWithChecking(NXTBrick.Motor.C, power, translatedTachoCount, 10);
						tasks.Add(task);
					}
				}
				
				Task.WaitAll(tasks.ToArray());
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
			var state = new NXTBrick.MotorState(0, 100, NXTBrick.MotorMode.None, NXTBrick.MotorRegulationMode.Idle, NXTBrick.MotorRunState.Idle, 0);
			if (Settings.EnginePorts.HasFlag(DeviceMotorPort.A))
			{
				Brick.SetMotorState(NXTBrick.Motor.A, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.A, false, true);
			}
			if (Settings.EnginePorts.HasFlag(DeviceMotorPort.B))
			{
				Brick.SetMotorState(NXTBrick.Motor.B, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.B, true, true);
			}
			if (Settings.EnginePorts.HasFlag(DeviceMotorPort.C))
			{
				Brick.SetMotorState(NXTBrick.Motor.C, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.C, true, true);
			}
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		/// <param name="intervalsCount">Intervals count in the cell.</param>
		public override Task<ReadCompletedEventArgs> ReadIntervalAsync(int intervalsCount)
		{
			var t = Task<ReadCompletedEventArgs>.Factory.StartNew(() =>
			{
				Brick.SetSensorMode(Settings.SonarPort.ToAForge(), NXTBrick.SensorType.Lowspeed9V, NXTBrick.SensorMode.Raw, true);
				int value;
				Brick.GetUltrasonicSensorsValue(Settings.SonarPort.ToAForge(), out value);

				double readedValue =
					((double)value - Settings.MinBlockDistance + Settings.BlockWidth / 2)
					/ Math.Max((Settings.MaxBlockDistance - Settings.MinBlockDistance + Settings.BlockWidth), 1);
				double translatedValue =
					readedValue * 0.8;
				double intervalStep =
					1.0 / (double)intervalsCount;

				int? interval = null;
				for (int i = 1; i < intervalsCount + 1; i++)
				{
					if (translatedValue < i * intervalStep)
					{
						interval = i;
						break;
					}
				}

				var e = new ReadCompletedEventArgs(interval, translatedValue);
				return e;
			});
			return t;
		}

		/// <summary>
		/// Reads the value of the current cell asynchronously.
		/// </summary>
		public override Task<ReadCompletedEventArgs> ReadTranslatedValueAsync()
		{
			var t = Task<ReadCompletedEventArgs>.Factory.StartNew(() =>
			{
				int value = GetUltrasonicSensorsValue(3);

				double readedValue =
					((double)value - Settings.MinBlockDistance + Settings.BlockWidth / 2)
					/ Math.Max((Settings.MaxBlockDistance - Settings.MinBlockDistance + Settings.BlockWidth), 1);
				double translatedValue =
					readedValue * 0.8;

				var e = new ReadCompletedEventArgs(null, translatedValue);
				return e;
			});
			return t;
		}

		private int GetUltrasonicSensorsValue(int attempts)
		{
			int value = -1;
			for (int i = 1; i <= attempts; i++)
			{
				try
				{
					Brick.SetSensorMode(Settings.SonarPort.ToAForge(), NXTBrick.SensorType.Lowspeed9V, NXTBrick.SensorMode.Raw);
					Brick.GetUltrasonicSensorsValue(Settings.SonarPort.ToAForge(), out value);
				}
				catch
				{
					if (i == attempts)
					{
						throw;
					}
				}
			}
			return value;
		}

		/// <summary>
		/// Rotates asynchronously manipulator by concrete degree.
		/// </summary>
		public override void RotateManipulatorByDegreeAsync(Direction direction, int degree)
		{
			RotateManipulatorWithChecking(direction, degree);
		}

		private Task RotateMotorWithChecking(NXTBrick.Motor motor, int power, int tachoLimit, int tachoError)
		{
			var t = Task.Factory.StartNew(() =>
			{
				NXTBrick.MotorState initialMotorState;
				Brick.GetMotorState(motor, out initialMotorState);

				int directionSign = power / Math.Abs(power);
				NXTBrick.MotorState currentMotorState;
				Brick.GetMotorState(motor, out currentMotorState);
				int currentTachoCount = currentMotorState.TachoCount;
				int targetTachoCount = currentTachoCount + directionSign * tachoLimit;
				var runState = new NXTBrick.MotorState(0, 100, NXTBrick.MotorMode.On | NXTBrick.MotorMode.Regulated, NXTBrick.MotorRegulationMode.Speed, NXTBrick.MotorRunState.Running, 0);
				runState.Power = power;
				runState.TachoLimit = Math.Abs(targetTachoCount - currentTachoCount);
				Debug.WriteLine("Start: " + motor.ToString() + ", " + DateTime.Now.Millisecond.ToString());
				Brick.SetMotorState(motor, runState, true);
				Debug.WriteLine("End: " + motor.ToString() + ", " + DateTime.Now.Millisecond.ToString());

				Thread.Sleep(Math.Max(1200, (int)(((double)tachoLimit / 180) * 1200)));

				Brick.GetMotorState(motor, out currentMotorState);
				currentTachoCount = currentMotorState.TachoCount;

				Debug.WriteLine(
					"Motor '{0}': Rotation Completed. 'Before' TC = {1}, 'After' TC = {2}, 'Target' = {3}, Error = {4}.",
					motor.ToString(),
					initialMotorState.TachoCount.ToString(), currentMotorState.TachoCount.ToString(),
					targetTachoCount.ToString(),
					(targetTachoCount - currentTachoCount).ToString());
			});
			return t;
		}

		private Task<RotationCompletedEventArgs> RotateManipulatorWithChecking(
			Direction direction, int tachoLimit,
			int? tachoLimitFirstStep = null, bool useFirstStepAnyway = false)
		{
			const int TachoStep = 10;
			var t = Task<RotationCompletedEventArgs>.Factory.StartNew(() =>
			{
				var args = new RotationCompletedEventArgs();

				int directionSign = direction == Direction.Left ? -1 : +1;
				int power = directionSign * Settings.ManipulatorPower;
				var motor = Settings.ManipulatorPort.ToAForge();

				NXTBrick.MotorState initialMotorState;
				Brick.GetMotorState(motor, out initialMotorState);
				args.InitialTachoCount = initialMotorState.TachoCount;

				NXTBrick.MotorState currentMotorState;
				Brick.GetMotorState(motor, out currentMotorState);

				int currentTachoCount = initialMotorState.TachoCount;
				int targetTachoCount = currentTachoCount + directionSign * tachoLimit;

				var runState = new NXTBrick.MotorState(power, 100, NXTBrick.MotorMode.On | NXTBrick.MotorMode.Regulated, NXTBrick.MotorRegulationMode.Speed, NXTBrick.MotorRunState.Running, TachoStep);

				currentTachoCount = initialMotorState.TachoCount;
				int step = 0;

				while (true)
				{
					NXTBrick.SensorValues touchValuesLeft;
					Brick.GetSensorValue(Settings.LeftTouchPort.ToAForge(), out touchValuesLeft);
					NXTBrick.SensorValues touchValuesRight;
					Brick.GetSensorValue(Settings.RightTouchPort.ToAForge(), out touchValuesRight);

					if ((direction == Direction.Left && touchValuesLeft.IsTouched())
						|| (direction == Direction.Right && touchValuesRight.IsTouched()))
					{
						args.IsFinalPosition = true;
						break;
					}

					if ((step == 0)
						&& tachoLimitFirstStep.HasValue
						&& (useFirstStepAnyway 
							|| (direction == Direction.Left && touchValuesRight.IsTouched())
							|| (direction == Direction.Right && touchValuesLeft.IsTouched())))
					{
						runState.TachoLimit = tachoLimitFirstStep.Value;
						Brick.SetMotorState(motor, runState, true);
						Thread.Sleep(1000);
					}
					else
					{
						runState.TachoLimit = TachoStep;
						Brick.SetMotorState(motor, runState, true);
						Thread.Sleep(250);
					}

					Brick.GetMotorState(motor, out currentMotorState);
					currentTachoCount = currentMotorState.TachoCount;

					if (((direction == Direction.Left) && ((targetTachoCount + TachoStep / 2) > currentTachoCount))
						|| ((direction == Direction.Right) && ((targetTachoCount - TachoStep / 2) < currentTachoCount)))
					{
						break;
					}
					step++;
				}

				args.FinalTachoCount = currentTachoCount;
				args.ErrorTachoCount = targetTachoCount - currentTachoCount;

				Debug.WriteLine(
					"Motor '{0}': Rotation Comleted. 'Before' TC = {1}, 'After' TC = {2}, 'Target' = {3}, Error = {4}.",
					motor.ToString(),
					initialMotorState.TachoCount.ToString(), currentMotorState.TachoCount.ToString(),
					targetTachoCount.ToString(),
					(targetTachoCount - currentTachoCount).ToString());

				return args;
			});
			return t;
		}

		/// <summary>
		/// Writes the new interval value into the current cell asynchronously.
		/// </summary>
		/// <param name="newInterval">The new interval value, must be in the [1, *] range.</param>
		/// <param name="oldInterval">The old inteval value, must be in the [1, *] range or null.</param>
		/// <param name="intervalsCount">The total intervals count, must be in the [1, *] range.</param>
		public override Task<WriteCompletedEventArgs> WriteIntervalAsync(int newInterval, int oldInterval, int intervalsCount)
		{
			var t = Task<WriteCompletedEventArgs>.Factory.StartNew(() =>
			{
				if (oldInterval == newInterval)
				{
					return null;
				}
				else if (newInterval > oldInterval)
				{
					RotateManipulatorWithChecking(Direction.Right, Settings.ManipulatorMaxAngle * 2, (int)(Settings.ManipulatorMaxAngle * 0.8)).Await();
					RotateMotorsAsync(Direction.Left, Settings.EngineStepSonarPartAngle).Await();
					//int tachoCount = (int)(translatedValue * Settings.ManipulatorMaxAngle - Settings.BlockWidthAngle / 2);
					int tachoCount = GetIntervalTachoCount(newInterval, oldInterval, intervalsCount);
					int backTachoCount = (int)(tachoCount * 0.8);
					RotateManipulatorWithChecking(Direction.Left, tachoCount, tachoCount).Await();
					RotateManipulatorWithChecking(Direction.Right, Settings.ManipulatorMaxAngle * 2, backTachoCount, true).Await();
					RotateMotorsAsync(Direction.Right, Settings.EngineStepSonarPartAngle).Await();
				}
				else
				{
					RotateManipulatorWithChecking(Direction.Left, Settings.ManipulatorMaxAngle * 2, (int)(Settings.ManipulatorMaxAngle * 0.8)).Await();
					RotateMotorsAsync(Direction.Left, Settings.EngineStepSonarPartAngle).Await();
					//int tachoCount = (int)((1 - translatedValue) * Settings.ManipulatorMaxAngle - Settings.BlockWidthAngle / 2);
					int tachoCount = GetIntervalTachoCount(newInterval, oldInterval, intervalsCount);
					int backTachoCount = (int)(tachoCount * 0.8);
					RotateManipulatorWithChecking(Direction.Right, tachoCount, tachoCount).Await();
					RotateManipulatorWithChecking(Direction.Left, Settings.ManipulatorMaxAngle, backTachoCount, true).Await();
					RotateMotorsAsync(Direction.Right, Settings.EngineStepSonarPartAngle).Await();
				}
				return new WriteCompletedEventArgs();
			});
			return t;
		}

		private int GetIntervalTachoCount(int newInterval, int oldInterval, int intervalsCount)
		{
			double mean = (intervalsCount + 1) / 2;
			double interval = newInterval > oldInterval ? newInterval : intervalsCount - newInterval + 1;
			if (intervalsCount > 3)
			{
				throw new Exception("intervalsCount > 3");
			}
			double intervalStep = 1.0 / (double)intervalsCount;
			double koef = newInterval < oldInterval ? Settings.RightManipulatorDirectionKoef : Settings.LeftManipulatorDirectionKoef;
			double translatedValue = koef * (((interval - 1) * intervalStep) + (interval * intervalStep)) / 2;
			int tachoCount = (int)(translatedValue * Settings.ManipulatorMaxAngle - Settings.BlockWidthAngle / 2);
			return tachoCount;
		}

		/// <summary>
		/// Stop manipulator motor, with reseting status and state.
		/// </summary>
		public override void StopWriteAsync()
		{
			var state = new NXTBrick.MotorState(0, 100, NXTBrick.MotorMode.None, NXTBrick.MotorRegulationMode.Idle, NXTBrick.MotorRunState.Idle, 0);
			if (Settings.ManipulatorPort == DeviceMotorPort.A)
			{
				Brick.SetMotorState(NXTBrick.Motor.A, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.A, false, true);
			}
			if (Settings.ManipulatorPort == DeviceMotorPort.B)
			{
				Brick.SetMotorState(NXTBrick.Motor.B, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.B, false, true);
			}
			if (Settings.ManipulatorPort == DeviceMotorPort.C)
			{
				Brick.SetMotorState(NXTBrick.Motor.C, state, true);
				Brick.ResetMotorPosition(NXTBrick.Motor.C, false, true);
			}
		}
	}
}
