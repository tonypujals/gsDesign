﻿namespace gsDesign.Explorer.ViewModels.Design
{
	using System;
	using Models;
	using Subfuzion.Helpers;

	public class Design : NotifyPropertyChangedBase
	{
		private DesignParameters _designParameters;

		public Design(Func<string, bool> nameValidator)
		{
			NameValidator = nameValidator;
			_designScript = new DesignScript {Design = this};
		}

		internal DesignParameters Model
		{
			get { return _designParameters ?? (_designParameters = new DesignParameters()); }
		}

		#region Name property

		public string Name
		{
			get { return Model.Name; }

			set
			{
				if (Model.Name != value)
				{
					// test here instead of above because we do want
					// to fire the property changed event to force the
					// the UI to refresh regardless
					if (NameValidator == null || (NameValidator != null && NameValidator(value)))
					{
						Model.Name = value;
					}
					RaisePropertyChanged("Name");
				}
			}
		}

		public Func<string, bool> NameValidator { get; set; }

		#endregion // Name

		#region Description property

		public string Description
		{
			get { return Model.Description ?? string.Format("{0} description...", Name ?? "Design"); }

			set
			{
				if (Model.Description != value)
				{
					Model.Description = value;
					RaisePropertyChanged("Description");
				}
			}
		}

		#endregion // Description

		#region ErrorPowerTimingParameters property

		private Ept _ept;

		public Ept Ept
		{
			get
			{
				if (_ept == null)
				{
					_ept = new Ept(Model);
					RaisePropertyChanged("Ept");
				}

				return _ept;
			}

			set
			{
				if (_ept != value)
				{
					_ept = value;
					RaisePropertyChanged("Ept");
				}
			}
		}

		#endregion // ErrorPowerTimingParameters

		#region SampleSize property

		private SampleSize _sampleSize;

		public SampleSize SampleSize
		{
			get
			{
				if (_sampleSize == null)
				{
					_sampleSize = new SampleSize(Model);
					RaisePropertyChanged("SampleSize");
				}

				return _sampleSize;
			}

			set
			{
				if (_sampleSize != value)
				{
					_sampleSize = value;
					RaisePropertyChanged("SampleSize");
				}
			}
		}

		#endregion // SampleSize

		#region SpendingFunctions property

		private SpendingFunctions _spendingFunctions;

		public SpendingFunctions SpendingFunctions
		{
			get
			{
				if (_spendingFunctions == null)
				{
					_spendingFunctions = new SpendingFunctions(Model);
					RaisePropertyChanged("SpendingFunctions");
				}

				return _spendingFunctions;
			}

			set
			{
				if (_spendingFunctions != value)
				{
					_spendingFunctions = value;
					RaisePropertyChanged("SpendingFunctions");
				}
			}
		}

		#endregion // SpendingFunctions

		#region DesignScript property

		private DesignScript _designScript;

		public DesignScript DesignScript
		{
			get { return _designScript; }
		}

		#endregion // DesignScript

		#region General properties

		#region IsModified property

		private bool _isModified;

		public bool IsModified
		{
			get { return _isModified; }

			set
			{
				if (_isModified != value)
				{
					_isModified = value;
					RaisePropertyChanged("IsModified");
				}
			}
		}

		#endregion // IsModified

		#endregion

		#region Object overrides

		public override string ToString()
		{
			return Name ?? "(Design)";
		}

		#region Equality and HashCode

		public bool Equals(Design other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Model.Name, Model.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Design)) return false;
			return Equals((Design) obj);
		}

		public override int GetHashCode()
		{
			return (Model.Name != null ? Model.Name.GetHashCode() : 0);
		}

		public static bool operator ==(Design left, Design right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Design left, Design right)
		{
			return !Equals(left, right);
		}

		#endregion

		#endregion
	}
}
