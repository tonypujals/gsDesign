namespace gsDesign.Explorer.ViewModels.Design.ErrorPowerTiming
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel.DataAnnotations;
	using Subfuzion.Helpers;
	using gsDesign.Design;
	using gsDesign.Design.ErrorPowerTiming;

	public class ErrorPowerTiming : NotifyPropertyChangedBase
	{
		private DesignParameters _designParameters;

		public ErrorPowerTiming(DesignParameters designParameters)
		{
			_designParameters = designParameters;
		}

		private ErrorPowerTimingParameters Model
		{
			get { return _designParameters.ErrorPowerTimingParameters; }
		}

		#region Error property

		[Display(Name = "Type I Error (1-sided \u03B1 x 100)",
			Description = "0 \u2264 Type I Error \u003C Power \u2264 100")]
		public double Error
		{
			get { return Math.Round(Model.Alpha * 100, 4); }

			set
			{
				if (Math.Abs(Error - value) >= double.Epsilon)
				{
					if (!ValidateErrorValue(value))
						throw new ArgumentException("Error must be a value between 0.0 and 100.0, inclusive");

					Model.Alpha = value / 100;
					NotifyPropertyChanged("Error");

					UpdatePower();
					NotifyPropertyChanged("MinimumValidPower");
					NotifyPropertyChanged("MinimumPowerDisplayText");
				}
			}
		}

		public double ErrorIncrement
		{
			get { return 0.1; }
		}

		public int ErrorPrecision
		{
			get { return 4; }
		}

		#endregion // Error

		#region MinimumValidError property

		public double MinimumValidError
		{
			get { return 0.0001; }
		}

		#endregion // MinimumValidError

		#region MaximumValidError property

		public double MaximumValidError
		{
			get { return 99.9999; }
		}

		#endregion // MaximumValidError

		#region Power property

		[Display(Name = "Power (100 x [1-\u03B2])",
			Description = "0 \u2264 Type I Error \u003C Power \u2264 100")]
		public double Power
		{
			get { return Math.Round(100.0 * (1.0 - Model.Beta), 1); }

			set
			{
				var power = ComputeValidPower(value, Error);
				if (Math.Abs(Power - power) >= 0.09)
				{
					Model.Beta = 1.0 - (power / 100.0);
				}

				// notifications out here inside of previous block
				// to force the slider to re-update if attempting to
				// set value too low.
				NotifyPropertyChanged("Power");
				NotifyPropertyChanged("MinimumValidPower");
				NotifyPropertyChanged("MinimumPowerDisplayText");
			}
		}

		#endregion // Power

		#region MinimumValidPower property

		public double MinimumValidPower
		{
			get { return Math.Min(Math.Round(Error + 0.1, 1), MaximumValidPower); }
		}

		#endregion // MinimumValidPower

		#region MaximumValidPower property

		public double MaximumValidPower
		{
			get { return 100.0; }
		}

		#endregion // MaximumValidPower

		#region MinimumPowerDisplayText

		public string MinimumPowerDisplayText
		{
			get { return string.Format("min={0}", MinimumValidPower); }
		}

		#endregion

		#region IntervalCount property

		[Display(Name = "Number of interim analyses",
			Description = "Number \u003E 0")]
		public int IntervalCount
		{
			get { return Model.K - 1; }

			set
			{
				if (Model.K != value + 1 && value >= MinimumIntervalCount && value <= MaximumIntervalCount)
				{
					Model.K = value + 1;
					NotifyPropertyChanged("IntervalCount");
					UpdateTimingTable();
				}
			}
		}

		#endregion // IntervalCount

		#region MinimumIntervalCount property

		private int _minimumIntervalCount = 1;

		public int MinimumIntervalCount
		{
			get { return _minimumIntervalCount; }

			set
			{
				if (_minimumIntervalCount != value)
				{
					_minimumIntervalCount = value;
					NotifyPropertyChanged("MinimumIntervalCount");
				}
			}
		}

		#endregion // MinimumIntervalCount

		#region MaximumIntervalCount property

		private int _maximumIntervalCount = 20;

		public int MaximumIntervalCount
		{
			get { return _maximumIntervalCount; }

			set
			{
				if (_maximumIntervalCount != value)
				{
					_maximumIntervalCount = value;
					NotifyPropertyChanged("MaximumIntervalCount");
				}
			}
		}

		#endregion // MaximumIntervalCount

		#region Spacing property

		private IntervalSpacing _spacing;

		public IEnumerable<string> SpacingValues
		{
			get { return EnumHelper.GetNames<IntervalSpacing>(); }
		}

		[Display(Name = "Spacing",
			Description = "Interim analyses timing: equal intervals or unequal (set timings manually)")]
		public IntervalSpacing Spacing
		{
			get { return _spacing; }

			set
			{
				if (_spacing != value)
				{
					_spacing = value;
					NotifyPropertyChanged("Spacing");
					NotifyPropertyChanged("IsTimingTableEnabled");
					UpdateTimingTable();
				}
			}
		}

		#endregion // Spacing

		#region TimingTable property

		private ObservableCollection<IntervalTiming> _timingTable = new ObservableCollection<IntervalTiming>
		    {
		        new IntervalTiming {Index = 1, Value = 0.3333},
		        new IntervalTiming {Index = 2, Value = 0.6667},
		    };

		public ObservableCollection<IntervalTiming> TimingTable
		{
			get { return _timingTable; }

			set
			{
				if (_timingTable != value)
				{
					_timingTable = value;
					NotifyPropertyChanged("TimingTable");
				}
			}
		}

		#endregion // TimingTable

		#region IsTimingTableEnabled property

		public bool IsTimingTableEnabled
		{
			get { return Spacing == IntervalSpacing.Unequal; }
		}

		#endregion // IsTimingTableEnabled

		#region Implementation

		private bool ValidateErrorValue(double value)
		{
			return !(MinimumValidError - value > double.Epsilon || value - MaximumValidError > double.Epsilon);
		}

		private double ComputeValidPower(double power, double error)
		{
			if (power - error < 0.1)
			{
				power = error + 0.1;
			}

			if (power > MaximumValidPower)
			{
				power = MaximumValidPower;
			}

			return Math.Round(power, 1);
		}

		// triggers notification if needs updating
		private void UpdatePower()
		{
			Power = ComputeValidPower(Power, Error);
		}

		private void UpdateTimingTable()
		{
			if (TimingTable.Count < IntervalCount)
			{
				var baseValue = TimingTable[TimingTable.Count - 1].Value;
				var increment = (1.0 - baseValue) / (IntervalCount - TimingTable.Count + 1);

				for (int i = TimingTable.Count; i < IntervalCount; i++, baseValue += increment)
				{
					TimingTable.Add(new IntervalTiming {Index = i + 1, Value = baseValue + increment});
				}
			}
			else if (TimingTable.Count > IntervalCount)
			{
				for (int i = IntervalCount, count = TimingTable.Count - IntervalCount; count > 0; count--)
				{
					TimingTable.RemoveAt(i);
				}
			}

			if (Spacing == IntervalSpacing.Equal)
			{
				var timing = 1.0 / (IntervalCount + 1);

				for (int i = 0; i < TimingTable.Count; i++)
				{
					TimingTable[i].Value = Math.Round(timing * (i + 1), 4);
				}
			}

			Model.Timing.Clear();
			foreach (var timing in TimingTable)
			{
				Model.Timing.Add(timing.Value);
			}

			NotifyPropertyChanged("TimingTable");
		}

		#endregion
	}
}
