using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NKH.MindSqualls;
using LegoTuringMachine.Model;
using System.Threading.Tasks;

namespace LegoTuringMachine.Device
{
	public class MindSquallsControlDevice : ControlDeviceBase
	{
		private NxtBrick Brick { get; set; }

		public MindSquallsControlDevice()
		{
		}

		protected override void OnSettingsAppliedOverride(DeviceSettings oldValue, DeviceSettings newValue)
		{
			if (ConnectionState == DeviceConnectionState.Connected)
			{
				//Disconnect();
			}
			else
			{
				Brick = new NxtBrick(newValue.DeviceComPort);
			}
		}

		NxtMotorSync Engine;

		public override void Connect()
		{
			var motorA = new NxtMotor();
			motorA.OnPolled += motorA_OnPolled;
			motorA.PollInterval = 100;
			var motorB = new NxtMotor();
			var motorC = new NxtMotor();
			Brick.MotorA = motorA;
			Brick.MotorB = motorB;
			Brick.MotorC = motorC;
			Engine = new NxtMotorSync(Brick.MotorA, Brick.MotorB);
			Brick.Sensor1 = new NxtUltrasonicSensor();
			Brick.InitSensors();
			Brick.Connect();
			ConnectionState = DeviceConnectionState.Connected;
		}

		void motorA_OnPolled(NxtPollable polledItem)
		{

		}

		public override void Disconnect()
		{
			Brick.Disconnect();
			ConnectionState = DeviceConnectionState.Disconnected;
		}

		public override Task<MoveCompletedEventArgs> MoveAsync(Direction direction, int distance)
		{
			if (direction == Direction.Right)
			{

				//Brick.MotorA.Run((sbyte)Settings.EngineStepPower, (ushort)Settings.EngineStepAngle);
				Engine.Run((sbyte)Settings.EngineStepPower, (ushort)Settings.EngineStepAngle, 0);
			}
			else
			{
				//Engine.Coast();
				Engine.ResetMotorPosition(true);
				//Engine.Brake();
				Engine.Idle();
			}
			//sync.Run(20, 20, 20);
			//sync.Run(20, 20, 20);
			//sync.Run(20, 20, 20);
			//sync.Brake();
			//sync.Idle();
			return null;
		}
	}
}
