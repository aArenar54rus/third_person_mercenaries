using System;


namespace Module.General.Analytics.Performance
{
	public sealed class InfoEntry
	{
		public string Title { get; set; }

		public object Value
		{
			get
			{
				try
				{
					return valueGetter();
				}
				catch (Exception e)
				{
					return "Error ({0})".Format(e.GetType().Name);
				}
			}
		}

		public bool IsPrivate { get; private set; }

		private Func<object> valueGetter;


		/// <summary>
		/// Create an <see cref="InfoEntry"/> instance with a getter function for the value.
		/// </summary>
		/// <param name="name">Name to display to the user.</param>
		/// <param name="getter">Getter method to acquire the latest value.</param>
		/// <param name="isPrivate">If true, will be excluded from the bug reporter system.</param>
		/// <returns>The created <see cref="InfoEntry"/> object.</returns>
		public static InfoEntry Create(string name, Func<object> getter, bool isPrivate = false)
		{
			return new InfoEntry
			{
				Title = name,
				valueGetter = getter,
				IsPrivate = isPrivate
			};
		}


		/// <summary>
		/// Create an <see cref="InfoEntry"/> instance with a fixed value.
		/// </summary>
		/// <param name="name">Name to display to the user.</param>
		/// <param name="value">The value of the entry.</param>
		/// <param name="isPrivate">If true, will be excluded from the bug reporter system.</param>
		/// <returns>The created <see cref="InfoEntry"/> object.</returns>
		public static InfoEntry Create(string name, object value, bool isPrivate = false)
		{
			return new InfoEntry
			{
				Title = name,
				valueGetter = () => value,
				IsPrivate = isPrivate
			};
		}
	}
}
