using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

using LegoTuringMachine.Management;
using System.IO;

namespace LegoTuringMachine
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			GlobalDispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			GlobalDispatcher.BeginInvoke(new Action<Exception>(error =>
			{
				MessageBox.Show(error.Message, "Ошибка");
			}), e.Exception);
            Log(e.Exception.Message, e.Exception.ToString());
		}

        protected void Log(string text, string details)
        {
            try
            {
                using (var f = new StreamWriter("Log.txt", true))
                {
                    f.WriteLine($"Date: {DateTime.Now.ToString()}, Message: {text}");
                    if (!string.IsNullOrEmpty(details))
                    {
                        f.WriteLine(details);
                    }
                    f.WriteLine("----------");
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets current threads dispatcher for application.
        /// </summary>
        public static Dispatcher GlobalDispatcher
		{
			get { return App.Current.Dispatcher; }
		}

		protected override void OnStartup(StartupEventArgs e)
		{
		}
	}
}
