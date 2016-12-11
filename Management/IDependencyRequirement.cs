using System;
using System.Linq;

namespace LegoTuringMachine.Management
{
	/// <summary>
	/// Contract for special dependency requirement checker.
	/// </summary>
	public interface IDependencyRequirement
	{
		/// <summary>
		/// Check that dependency from constructor injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		void NotNullFromCI<TDependency>(TDependency dependency, string dependencyName = null);

		/// <summary>
		/// Check that dependency from constructor injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		void NotNullFromCI<TInjectionTarget, TDependency>(TDependency dependency, string dependencyName = null);

		/// <summary>
		/// Check that dependency from property injection is not null.
		/// </summary>
		/// <typeparam name="TInjectionTarget">The type of the injection target.</typeparam>
		/// <typeparam name="TDependency">The type of the dependency.</typeparam>
		/// <param name="dependency">The dependency.</param>
		/// <param name="dependencyName">Name of the dependency.</param>
		void NotNullFromPI<TInjectionTarget, TDependency>(TDependency dependency, string dependencyName = null);
	}
}
