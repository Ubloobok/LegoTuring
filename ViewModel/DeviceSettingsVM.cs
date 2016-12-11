using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.ViewModel.Item;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Model;
using LegoTuringMachine.Device;
using LegoTuringMachine.Management;
using LegoTuringMachine.Extensions;

namespace LegoTuringMachine.ViewModel
{
	public class DeviceSettingsVM : ViewModelBase, IDeviceSettingsVM
	{
		private IControlDevice _controlDevice;
		private ISettingsFacade _settingsFacade;

		private string _deviceComPort;
		private int _engineStepPower;
		private int _engineStepSonarPartAngle;
		private int _engineStepAngle;
		private int _manipulatorPower;
		private int _manipulatorMaxAngle;
		private double _maxBlockDistance;
		private double _minBlockDistance;
		private double _blockWidth;
		private double _blockWidthAngle;
		private DeviceSensorPort _leftTouchPort;
		private DeviceSensorPort _rightTouchPort;
		private double _leftManipulatorDirectionKoef;
		private double _rightManipulatorDirectionKoef;

		public DeviceSettingsVM(IControlDevice controlDevice, ISettingsFacade settingsFacade)
		{
			DependencyRequirement.Current.NotNullFromCI(controlDevice);
			DependencyRequirement.Current.NotNullFromCI(settingsFacade);
			_controlDevice = controlDevice;
			_settingsFacade = settingsFacade;
			EnginePortsSelector = new SelectableCollection<SelectorItemVM>(
				new SelectorItemVM("A,B", DeviceMotorPort.A | DeviceMotorPort.B),
				new SelectorItemVM("A,C", DeviceMotorPort.A | DeviceMotorPort.C),
				new SelectorItemVM("B,C", DeviceMotorPort.B | DeviceMotorPort.C));
			EnginePortsSelector.SelectedItem = EnginePortsSelector[0];
			ManipulatorPortSelector = new SelectableCollection<SelectorItemVM>(
				new SelectorItemVM("A", DeviceMotorPort.A),
				new SelectorItemVM("B", DeviceMotorPort.B),
				new SelectorItemVM("C", DeviceMotorPort.C));
			ManipulatorPortSelector.SelectedItem = ManipulatorPortSelector[2];
			SonarPortSelector = new SelectableCollection<SelectorItemVM>(
				new SelectorItemVM("1-й", DeviceSensorPort.First),
				new SelectorItemVM("2-й", DeviceSensorPort.Second),
				new SelectorItemVM("3-й", DeviceSensorPort.Third),
				new SelectorItemVM("4-й", DeviceSensorPort.Fourth));
			SonarPortSelector.SelectedItem = SonarPortSelector[0];

			_controlDevice.ConnectionStateChanged -= OnControlDeviceConnectionStateChanged;
			_controlDevice.ConnectionStateChanged += OnControlDeviceConnectionStateChanged;
			InitializeCommands();
		}

		public bool IsConnected
		{
			get
			{
				return _controlDevice.ConnectionState == DeviceConnectionState.Connected;
			}
		}

		public string DeviceComPort
		{
			get { return _deviceComPort; }
			set
			{
				if (value != _deviceComPort)
				{
					RaisePropertyChanging<string>(() => DeviceComPort);
					_deviceComPort = value;
					RaisePropertyChanged<string>(() => DeviceComPort);
				}
			}
		}

		public SelectableCollection<SelectorItemVM> EnginePortsSelector { get; private set; }

		public int EngineStepPower
		{
			get { return _engineStepPower; }
			set
			{
				if (value != _engineStepPower)
				{
					RaisePropertyChanging<int>(() => EngineStepPower);
					_engineStepPower = value;
					RaisePropertyChanged<int>(() => EngineStepPower);
				}
			}
		}

		public int EngineStepSonarPartAngle
		{
			get { return _engineStepSonarPartAngle; }
			set
			{
				if (value != _engineStepSonarPartAngle)
				{
					RaisePropertyChanging<int>(() => EngineStepSonarPartAngle);
					_engineStepSonarPartAngle = value;
					RaisePropertyChanged<int>(() => EngineStepSonarPartAngle);
				}
			}
		}

		public int EngineStepAngle
		{
			get { return _engineStepAngle; }
			set
			{
				if (value != _engineStepAngle)
				{
					RaisePropertyChanging<int>(() => EngineStepAngle);
					_engineStepAngle = value;
					RaisePropertyChanged<int>(() => EngineStepAngle);
				}
			}
		}

		public SelectableCollection<SelectorItemVM> ManipulatorPortSelector { get; private set; }

		public int ManipulatorPower
		{
			get { return _manipulatorPower; }
			set
			{
				if (value != _manipulatorPower)
				{
					RaisePropertyChanging<int>(() => ManipulatorPower);
					_manipulatorPower = value;
					RaisePropertyChanged<int>(() => ManipulatorPower);
				}
			}
		}

		public int ManipulatorMaxAngle
		{
			get { return _manipulatorMaxAngle; }
			set
			{
				if (value != _manipulatorMaxAngle)
				{
					RaisePropertyChanging<int>(() => ManipulatorMaxAngle);
					_manipulatorMaxAngle = value;
					RaisePropertyChanged<int>(() => ManipulatorMaxAngle);
				}
			}
		}

		public SelectableCollection<SelectorItemVM> SonarPortSelector { get; private set; }

		private double _sonarTestValue;

		public double SonarTestValue
		{
			get { return _sonarTestValue; }
			set
			{
				if (value != _sonarTestValue)
				{
					RaisePropertyChanging<double>(() => SonarTestValue);
					_sonarTestValue = value;
					RaisePropertyChanged<double>(() => SonarTestValue);
				}
			}
		}

		public ActionCommand DeviceConnectCommand { get; set; }
		public ActionCommand DeviceDisconnectCommand { get; set; }
		public ActionCommand SonarGetValueCommand { get; set; }
		public ActionCommand EngineStepForwardFullCommand { get; set; }
		public ActionCommand EngineStepForwardPartCommand { get; set; }
		public ActionCommand EngineStepBackwardFullCommand { get; set; }
		public ActionCommand EngineStepBackwardPartCommand { get; set; }
		public ActionCommand EngineStopFullCommand { get; private set; }
		public DelegateCommand<int> ManipulatorStepLeftCommand { get; set; }
		public DelegateCommand<int> ManipulatorStepRightCommand { get; set; }
		public ActionCommand ManipulatorStopCommand { get; set; }

		public ActionCommand DoSaveCommand { get; set; }

		/// <summary>
		/// Initializes settings values in view model, loads values from facade.
		/// </summary>
		public void InitializeSettings()
		{
			var deviceSettings = _settingsFacade.LoadDeviceSettings();
			DeviceComPort = deviceSettings.DeviceComPort;
			EnginePortsSelector.SelectedItem = EnginePortsSelector.FirstOrDefault(i => (DeviceMotorPort)i.Value == deviceSettings.EnginePorts);
			EngineStepPower = deviceSettings.EngineStepPower;
			EngineStepSonarPartAngle = deviceSettings.EngineStepSonarPartAngle;
			EngineStepAngle = deviceSettings.EngineStepAngle;
			ManipulatorPortSelector.SelectedItem = ManipulatorPortSelector.FirstOrDefault(i => (DeviceMotorPort)i.Value == deviceSettings.ManipulatorPort);
			ManipulatorPower = deviceSettings.ManipulatorPower;
			ManipulatorMaxAngle = deviceSettings.ManipulatorMaxAngle;
			SonarPortSelector.SelectedItem = SonarPortSelector.FirstOrDefault(i => (DeviceSensorPort)i.Value == deviceSettings.SonarPort);
			_maxBlockDistance = deviceSettings.MaxBlockDistance;
			_minBlockDistance = deviceSettings.MinBlockDistance;
			_blockWidth = deviceSettings.BlockWidth;
			_blockWidthAngle = deviceSettings.BlockWidthAngle;
			_leftTouchPort = deviceSettings.LeftTouchPort;
			_rightTouchPort = deviceSettings.RightTouchPort;
			_leftManipulatorDirectionKoef = deviceSettings.LeftManipulatorDirectionKoef;
			_rightManipulatorDirectionKoef = deviceSettings.RightManipulatorDirectionKoef;
		}

		/// <summary>
		/// Initializes all control commands.
		/// </summary>
		private void InitializeCommands()
		{
			DeviceConnectCommand = new ActionCommand(() =>
			{
				ApplySettings();
				_controlDevice.Connect();
			}, () =>
			{
				return _controlDevice.ConnectionState == DeviceConnectionState.Disconnected && !string.IsNullOrEmpty(DeviceComPort);
			});
			DeviceDisconnectCommand = new ActionCommand(() =>
			{
				_controlDevice.Disconnect();
			}, () =>
			{
				return _controlDevice.ConnectionState == DeviceConnectionState.Connected;
			});
			SonarGetValueCommand = new ActionCommand(() =>
			{
				ApplySettings();
				var r = _controlDevice.ReadTranslatedValueAsync().Await();
				SonarTestValue = r.TranslatedValue.GetValueOrDefault();
			}, () =>
			{
				return IsConnected && SonarPortSelector.SelectedItem != null;
			});
			EngineStepForwardFullCommand = new ActionCommand(() =>
			{
				ApplySettings();
				_controlDevice.MoveAsync(Direction.Right, 1);
			});
			EngineStepBackwardFullCommand = new ActionCommand(() =>
			{
				ApplySettings();
				_controlDevice.MoveAsync(Direction.Left, 1);
			});
			EngineStopFullCommand = new ActionCommand(() =>
			{
				ApplySettings();
				_controlDevice.StopMoveAsync();
			});
			ManipulatorStepLeftCommand = new DelegateCommand<int>(i =>
			{
				ApplySettings();
				_controlDevice.RotateManipulatorByDegreeAsync(Direction.Left, i);
			});
			ManipulatorStepRightCommand = new DelegateCommand<int>(i =>
			{
				ApplySettings();
				_controlDevice.RotateManipulatorByDegreeAsync(Direction.Right, i);
			});
			ManipulatorStopCommand = new ActionCommand(() =>
			{
				ApplySettings();
				_controlDevice.StopWriteAsync();
			});
			DoSaveCommand = new ActionCommand(() =>
			{
				ApplySettings();
			});
		}

		private void OnControlDeviceConnectionStateChanged(object sender, EventArgs e)
		{
			DeviceConnectCommand.RaiseCanExecuteChanged();
			DeviceDisconnectCommand.RaiseCanExecuteChanged();
			SonarGetValueCommand.RaiseCanExecuteChanged();
			EngineStepForwardFullCommand.RaiseCanExecuteChanged();
			//EngineStepForwardPartCommand.RaiseCanExecuteChanged();
			EngineStepBackwardFullCommand.RaiseCanExecuteChanged();
			//EngineStepBackwardPartCommand.RaiseCanExecuteChanged();
			ManipulatorStepLeftCommand.RaiseCanExecuteChanged();
			ManipulatorStepRightCommand.RaiseCanExecuteChanged();
			RaisePropertyChanged<bool>(() => IsConnected);
		}

		private void ApplySettings()
		{
			var newDeviceSettings = new DeviceSettings()
			{
				DeviceComPort = DeviceComPort,
				EnginePorts = EnginePortsSelector.SelectedItem == null ? DeviceMotorPort.Unknown : (DeviceMotorPort)EnginePortsSelector.SelectedItem.Value,
				EngineStepPower = EngineStepPower,
				EngineStepSonarPartAngle = EngineStepSonarPartAngle,
				EngineStepAngle = EngineStepAngle,
				ManipulatorPort = ManipulatorPortSelector.SelectedItem == null ? DeviceMotorPort.Unknown : (DeviceMotorPort)ManipulatorPortSelector.SelectedItem.Value,
				ManipulatorPower = ManipulatorPower,
				ManipulatorMaxAngle = ManipulatorMaxAngle,
				SonarPort = SonarPortSelector.SelectedItem == null ? DeviceSensorPort.Unknown : (DeviceSensorPort)SonarPortSelector.SelectedItem.Value,
				MaxBlockDistance = _maxBlockDistance,
				MinBlockDistance = _minBlockDistance,
				BlockWidth = _blockWidth,
				BlockWidthAngle = _blockWidthAngle,
				LeftTouchPort = _leftTouchPort,
				RightTouchPort = _rightTouchPort,
				LeftManipulatorDirectionKoef = _leftManipulatorDirectionKoef,
				RightManipulatorDirectionKoef = _rightManipulatorDirectionKoef,
			};
			_controlDevice.ApplySettings(newDeviceSettings);
			_settingsFacade.SaveDeviceSettings(newDeviceSettings);
		}
	}
}
