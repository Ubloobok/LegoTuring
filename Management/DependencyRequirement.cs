using System;
using System.Linq;

namespace LegoTuringMachine.Management
{
	/// <summary>
	/// Common dependency requirement checker for application.
	/// </summary>
	public class DependencyRequirement : IDependencyRequirement
	{
		private static IDependencyRequirement DefaultRequirement = new DependencyRequirement();
		private static IDependencyRequirement CurrentRequirement;

		/// <summary>
		/// Initializes the <see cref="DependencyRequirement"/> class and singleton instance.
		/// </summary>
		static DependencyRequirement()
		{
			Current = DefaultRequirement;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyRequirement"/> class.
		/// </summary>
		public DependencyRequirement()
		{
		}

		/// <summary>
		/// Gets the current dependency requirement checker associated with application.
		/// </summary>
		public static IDependencyRequirement Current
		{
			get { return CurrentRequirement; }
			private set { CurrentRequirement = value ?? DefaultRequirement; }
		}

		/// <summary>
		/// Check that dependency from constructor injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		public void NotNullFromCI<TDependency>(TDependency dependency, string dependencyName = null)
		{
			if (dependency == null)
			{
				string message = string.IsNullOrEmpty(dependencyName) ? typeof(TDependency).Name : dependencyName;
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		/// Check that dependency from constructor injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		public void NotNullFromCI<TInjectionTarget, TDependency>(TDependency dependency, string dependencyName = null)
		{
			if (dependency == null)
			{
				string message = string.IsNullOrEmpty(dependencyName) ? typeof(TDependency).Name : dependencyName;
				throw new ArgumentException(message);
			}
		}

		/// <summary>
		/// Check that dependency from property injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		public void NotNullFromPI<TInjectionTarget, TDependency>(TDependency dependency, string dependencyName)
		{
			if (dependency == null)
			{
				string message = string.IsNullOrEmpty(dependencyName) ? typeof(TDependency).Name : dependencyName;
				throw new ArgumentException(message);
			}
		}
	}
}
