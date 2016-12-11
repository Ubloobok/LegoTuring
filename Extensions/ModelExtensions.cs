using AForge.Robotics.Lego;
using LegoTuringMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Extensions
{
	public static class ModelExtensions
	{
		public static bool IsTouched(this NXTBrick.SensorValues values)
		{
			return values.Raw < 500;
		}

		public static NXTBrick.Motor ToAForge(this DeviceMotorPort port)
		{
			NXTBrick.Motor resultMotor;
			switch (port)
			{
				case DeviceMotorPort.A:
					resultMotor = NXTBrick.Motor.A;
					break;
				case DeviceMotorPort.B:
					resultMotor = NXTBrick.Motor.B;
					break;
				case DeviceMotorPort.C:
					resultMotor = NXTBrick.Motor.C;
					break;
				default:
					resultMotor = (NXTBrick.Motor)(-1);
					break;
			}
			return resultMotor;
		}

		public static NXTBrick.Sensor ToAForge(this DeviceSensorPort port)
		{
			NXTBrick.Sensor nxtSensor = (NXTBrick.Sensor)((int)port - 1);
			return nxtSensor;
		}

		public static bool IsEquals(this DeviceSensorPort port1, NXTBrick.Sensor port2)
		{
			bool isEquals = port1.ToAForge() == port2;
			return isEquals;
		}
	}
}
