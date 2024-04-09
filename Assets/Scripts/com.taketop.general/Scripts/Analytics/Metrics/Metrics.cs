using System;


namespace Module.General.Analytics.Performance
{
	public class Metrics
	{
		#region Fields

		protected const int KEEP_DATA_SECONDS = 60;

		#endregion



		#region Properties

		public bool IsEnabled { get; private set; }

		public virtual bool IsValid => true;

		#endregion



		#region Ctor

		public Metrics()
		{
			SetEnabled(true);
		}

		#endregion



		#region Methods

		public virtual void SetEnabled(bool value) =>
			IsEnabled = value;


		public virtual void Sample() {}


		protected float NanosecondsToMilliseconds(float value) =>
			value / 1000000;

		#endregion



		#region NestedClasses

		public class AverageCounter
		{
			public float Average { get; private set; }
			private long count = 0;


			public static implicit operator float(AverageCounter value) =>
				value.Average;


			public void Increase(object value)
			{
				float v = Convert.ToSingle(value);
				Average += (v - Average) / ++count;
			}
		}

		#endregion
	}
}
