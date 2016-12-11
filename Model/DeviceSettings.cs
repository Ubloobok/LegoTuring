using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Model
{
	[Flags]
	public enum DeviceMotorPort
	{
		Unknown = 0,
		A = 1,
		B = 2,
		C = 4,
	}

	public enum DeviceSensorPort
	{
		Unknown = 0,
		First = 1,
		Second = 2,
		Third = 3,
		Fourth = 4,
	}

	public class DeviceSettings
	{
		public string DeviceComPort { get; set; }

		public DeviceMotorPort EnginePorts { get; set; }
		public int EngineStepPower { get; set; }
		public int EngineStepSonarPartAngle { get; set; }
		public int EngineStepAngle { get; set; }

		public DeviceMotorPort ManipulatorPort { get; set; }
		public int ManipulatorPower { get; set; }
		public int ManipulatorMaxAngle { get; set; }

		public DeviceSensorPort SonarPort { get; set; }
		public double MaxBlockDistance { get; set; }
		public double MinBlockDistance { get; set; }
		public double BlockWidth { get; set; }
		public double BlockWidthAngle { get; set; }

		public DeviceSensorPort LeftTouchPort { get; set; }
		public DeviceSensorPort RightTouchPort { get; set; }

		public double LeftManipulatorDirectionKoef { get; set; }
		public double RightManipulatorDirectionKoef { get; set; }
	}
}
