﻿namespace Subfuzion.Silverlight.UI.Charting
{
	using System;
	using System.Collections.ObjectModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Shapes;

	// http://blogs.msdn.com/b/delay/archive/2009/02/26/designerproperties-getisindesignmode-forrealz-how-to-reliably-detect-silverlight-design-mode-in-blend-and-visual-studio.aspx
	/// <summary>
	/// Provides a custom implementation of DesignerProperties.GetIsInDesignMode
	/// to work around an issue.
	/// </summary>
	internal static class DesignerProperties
	{
		/// <summary>
		/// Returns whether the control is in design mode (running under Blend
		/// or Visual Studio).
		/// </summary>
		/// <param name="element">The element from which the property value is
		/// read.</param>
		/// <returns>True if in design mode.</returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "element", Justification =
			"Matching declaration of System.ComponentModel.DesignerProperties.GetIsInDesignMode (which has a bug and is not reliable).")]
		public static bool GetIsInDesignMode(DependencyObject element)
		{
			if (!_isInDesignMode.HasValue)
			{
				_isInDesignMode =
					(null == Application.Current) ||
					Application.Current.GetType() == typeof(Application);
			}
			return _isInDesignMode.Value;
		}

		/// <summary>
		/// Stores the computed InDesignMode value.
		/// </summary>
		private static bool? _isInDesignMode;
	}

	[TemplatePart(Name = "PART_plotCanvas", Type = typeof (Canvas))]
	public class LinePlot : LinePlotRenderBase
	{
		private readonly Shape DefaultControlPoint = new Ellipse
		{Fill = new SolidColorBrush(Colors.Red), Width = 12, Height = 12};

		private bool isDragging;

		public LinePlot()
		{

			DefaultStyleKey = typeof (LinePlot);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (DesignerProperties.GetIsInDesignMode(this)) return;

			//SizeChanged += (sender, args) => OnSizeChanged(args.NewSize);

			PlotSurface = GetTemplateChild(PlotCanvasPart) as Canvas;
			if (PlotSurface != null)
			{
				PlotSurface.SizeChanged += OnPlotSurfaceSizeChanged;
				//PlotCanvas.SizeChanged += (sender, args) => OnSizeChanged(args.NewSize);
				ClipToBounds(ActualWidth, ActualHeight);

				//PlotCanvas.MouseEnter += (sender, args) => PlotCanvas.MouseMove += HandleOnMouseMove;
				//PlotCanvas.MouseLeave += (sender, args) => PlotCanvas.MouseMove -= HandleOnMouseMove;
				PlotSurface.MouseLeftButtonUp += (sender, args) =>
				{
					isDragging = false;
					ControlPointState = ControlPointState.Normal;
				};

				// add children...

				if (Polyline != null) PlotSurface.Children.Add(Polyline);

				if (ControlPoint != null)
				{
					PlotSurface.Children.Add(ControlPoint);
				}

				if (ControlPointHover != null)
				{
					PlotSurface.Children.Add(ControlPointHover);
				}

				if (ControlPointDrag != null)
				{
					PlotSurface.Children.Add(ControlPointDrag);
				}

				UpdatePlotDisplay();
				UpdateControlPointStateDisplay();

			}
		}

		#region PlotCanvas property

		#endregion

		#region PlotWidth property

		#endregion

		#region PlotHeight property

		#endregion

		#region IsControlPointVisible Property

		public static DependencyProperty IsControlPointVisibleProperty = DependencyProperty.Register(
			"IsControlPointVisible",
			typeof (bool),
			typeof (LinePlot),
			new PropertyMetadata(IsControlPointVisibleChangedHandler));

		public bool IsControlPointVisible
		{
			get { return (bool) GetValue(IsControlPointVisibleProperty); }
			set { SetValue(IsControlPointVisibleProperty, value); }
		}

		private static void IsControlPointVisibleChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnIsControlPointVisibleChanged((bool) args.NewValue, (bool) args.OldValue);
			}
		}

		protected virtual void OnIsControlPointVisibleChanged(bool newValue, bool oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnIsControlPointVisibleChanged(newValue);
		}

		protected virtual void OnIsControlPointVisibleChanged(bool newValue)
		{
			// add handler code
		}

		#endregion // IsControlPointVisible Property

		#region ControlPointPhysicalPosition Property

		#region ControlPointPosition Property

		public static DependencyProperty ControlPointPhysicalPositionProperty = DependencyProperty.Register(
			"ControlPointPosition",
			typeof (Point),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointPhysicalPositionChangedHandler));

		public Point ControlPointPhysicalPosition
		{
			get { return (Point) GetValue(ControlPointPhysicalPositionProperty); }
			set { SetValue(ControlPointPhysicalPositionProperty, value); }
		}

		private static void ControlPointPhysicalPositionChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointPhysicalPositionChanged((Point) args.NewValue, (Point) args.OldValue);
			}
		}

		protected virtual void OnControlPointPhysicalPositionChanged(Point newValue, Point oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnControlPointPhysicalPositionChanged(newValue);
		}

		protected virtual void OnControlPointPhysicalPositionChanged(Point newValue)
		{
			// add handler code
			UpdateControlPointStateDisplay();

			//Point logPoint = PhysicalToLogicalCoordinates(newValue);
			//ControlPointPlotX = logPoint.X;
			//ControlPointPlotY = logPoint.Y;
		}

		#endregion // ControlPointPhysicalPosition Property

		#region ControlPointPlotX Property

		public static DependencyProperty ControlPointPlotXProperty = DependencyProperty.Register(
			"ControlPointPlotX",
			typeof (double),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointPlotXChangedHandler));

		public double ControlPointPlotX
		{
			get { return (double) GetValue(ControlPointPlotXProperty); }
			set { SetValue(ControlPointPlotXProperty, value); }
		}

		private static void ControlPointPlotXChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointPlotXChanged((double) args.NewValue, (double) args.OldValue);
			}
		}

		protected virtual void OnControlPointPlotXChanged(double newValue, double oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnControlPointPlotXChanged(newValue);
		}

		protected virtual void OnControlPointPlotXChanged(double newValue)
		{
			// add handler code
			Point physPoint = LogicalToPhysicalCoordinates(new Point(newValue, ControlPointPlotY));
			if (Math.Abs(physPoint.X - ControlPointPhysicalPosition.X) > double.Epsilon)
				ControlPointPhysicalPosition = physPoint;
		}

		#endregion ControlPointPlotX Property

		#region ControlPointPlotY Property

		public static DependencyProperty ControlPointPlotYProperty = DependencyProperty.Register(
			"ControlPointPlotY",
			typeof (double),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointPlotYChangedHandler));

		public double ControlPointPlotY
		{
			get { return (double) GetValue(ControlPointPlotYProperty); }
			set { SetValue(ControlPointPlotYProperty, value); }
		}

		private static void ControlPointPlotYChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointPlotYChanged((double) args.NewValue, (double) args.OldValue);
			}
		}

		protected virtual void OnControlPointPlotYChanged(double newValue, double oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnControlPointPlotYChanged(newValue);
		}

		protected virtual void OnControlPointPlotYChanged(double newValue)
		{
			// add handler code
			Point physPoint = LogicalToPhysicalCoordinates(new Point(ControlPointPlotX, newValue));
			if (Math.Abs(physPoint.Y - ControlPointPhysicalPosition.Y) > double.Epsilon)
				ControlPointPhysicalPosition = physPoint;
		}

		#endregion ControlPointPlotY Property

		protected virtual void OnCoordinatesChanged(ObservableCollection<Point> newCoordinates)
		{
			base.OnCoordinatesChanged(newCoordinates);
			UpdateControlPointStateDisplay();
		}



		#region ControlPoint Property

		public static DependencyProperty ControlPointProperty = DependencyProperty.Register(
			"ControlPoint",
			typeof (Shape),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointChangedHandler));

		public Shape ControlPoint
		{
			get { return (Shape) GetValue(ControlPointProperty); }
			set { SetValue(ControlPointProperty, value); }
		}

		private static void ControlPointChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointChanged((Shape) args.NewValue, (Shape) args.OldValue);
			}
		}

		protected virtual void OnControlPointChanged(Shape newValue, Shape oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			if (PlotSurface != null && PlotSurface.Children.Contains(oldValue))
			{
				RemoveControlPointHandlers(oldValue);
				PlotSurface.Children.Remove(oldValue);
			}

			OnControlPointChanged(newValue);
		}

		protected virtual void OnControlPointChanged(Shape newValue)
		{
			// add handler code
			AddControlPointHandlers(newValue);
			newValue.Visibility = Visibility.Collapsed;
			if (PlotSurface != null) PlotSurface.Children.Add(newValue);
			UpdateControlPointStateDisplay();
		}

		#endregion // ControlPoint Property

		#region ControlPointHover Property

		public static DependencyProperty ControlPointHoverProperty = DependencyProperty.Register(
			"ControlPointHover",
			typeof (Shape),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointHoverChangedHandler));

		public Shape ControlPointHover
		{
			get { return (Shape) GetValue(ControlPointHoverProperty); }
			set { SetValue(ControlPointHoverProperty, value); }
		}

		private static void ControlPointHoverChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointHoverChanged((Shape) args.NewValue, (Shape) args.OldValue);
			}
		}

		protected virtual void OnControlPointHoverChanged(Shape newValue, Shape oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			if (PlotSurface != null && PlotSurface.Children.Contains(oldValue))
			{
				RemoveControlPointHandlers(oldValue);
				PlotSurface.Children.Remove(oldValue);
			}

			OnControlPointHoverChanged(newValue);
		}

		protected virtual void OnControlPointHoverChanged(Shape newValue)
		{
			// add handler code
			AddControlPointHandlers(newValue);
			newValue.Visibility = Visibility.Collapsed;
			if (PlotSurface != null) PlotSurface.Children.Add(newValue);
			UpdateControlPointStateDisplay();
		}

		#endregion ControlPointHover Property

		#region ControlPointDrag Property

		public static DependencyProperty ControlPointDragProperty = DependencyProperty.Register(
			"ControlPointDrag",
			typeof (Shape),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointDragChangedHandler));

		public Shape ControlPointDrag
		{
			get { return (Shape) GetValue(ControlPointDragProperty); }
			set { SetValue(ControlPointDragProperty, value); }
		}

		private static void ControlPointDragChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointDragChanged((Shape) args.NewValue, (Shape) args.OldValue);
			}
		}

		protected virtual void OnControlPointDragChanged(Shape newValue, Shape oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			if (PlotSurface != null && PlotSurface.Children.Contains(oldValue))
			{
				RemoveControlPointHandlers(oldValue);
				PlotSurface.Children.Remove(oldValue);
			}

			OnControlPointDragChanged(newValue);
		}

		protected virtual void OnControlPointDragChanged(Shape newValue)
		{
			// add handler code
			AddControlPointHandlers(newValue);
			newValue.Visibility = Visibility.Collapsed;
			if (PlotSurface != null) PlotSurface.Children.Add(newValue);
			UpdateControlPointStateDisplay();
		}

		#endregion ControlPointDrag Property

		#region ControlPointVisibility Property

		public static DependencyProperty ControlPointVisibilityProperty = DependencyProperty.Register(
			"ControlPointVisibility",
			typeof (Visibility),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointVisibilityChangedHandler));

		public Visibility ControlPointVisibility
		{
			get { return (Visibility) GetValue(ControlPointVisibilityProperty); }
			set { SetValue(ControlPointVisibilityProperty, value); }
		}

		private static void ControlPointVisibilityChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointVisibilityChanged((Visibility) args.NewValue, (Visibility) args.OldValue);
			}
		}

		protected virtual void OnControlPointVisibilityChanged(Visibility newValue, Visibility oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnControlPointVisibilityChanged(newValue);
		}

		protected virtual void OnControlPointVisibilityChanged(Visibility newValue)
		{
			// add handler code
			UpdateControlPointStateDisplay();
		}

		#endregion ControlPointVisibility Property

		#region ControlPointState Property

		public static DependencyProperty ControlPointStateProperty = DependencyProperty.Register(
			"ControlPointState",
			typeof (ControlPointState),
			typeof (LinePlot),
			new PropertyMetadata(ControlPointStateChangedHandler));

		public ControlPointState ControlPointState
		{
			get { return (ControlPointState) GetValue(ControlPointStateProperty); }
			set { SetValue(ControlPointStateProperty, value); }
		}

		private static void ControlPointStateChangedHandler(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs args)
		{
			var interactivePlot = dependencyObject as LinePlot;
			if (interactivePlot != null)
			{
				interactivePlot.OnControlPointStateChanged((ControlPointState) args.NewValue, (ControlPointState) args.OldValue);
			}
		}

		protected virtual void OnControlPointStateChanged(ControlPointState newValue, ControlPointState oldValue)
		{
			// handle property changed here if the old value is important; otherwise, just pass on new value
			OnControlPointStateChanged(newValue);
		}

		protected virtual void OnControlPointStateChanged(ControlPointState newValue)
		{
			// add handler code
			UpdateControlPointStateDisplay();
		}

		#endregion ControlPointState Property

		private Shape ActiveControlPoint
		{
			get
			{
				switch (ControlPointState)
				{
					case ControlPointState.Hover:
						return CurrentControlPointHover;

					case ControlPointState.Drag:
						return CurrentControlPointDrag;
				}

				return CurrentControlPoint;
			}
		}

		private Shape CurrentControlPoint
		{
			get
			{
				if (ControlPoint == null)
				{
					ControlPoint = DefaultControlPoint;
				}

				return ControlPoint;
			}
		}

		private Shape CurrentControlPointHover
		{
			get { return ControlPointHover ?? CurrentControlPoint; }
		}

		private Shape CurrentControlPointDrag
		{
			get { return ControlPointDrag ?? CurrentControlPointHover; }
		}

		private void UpdateControlPointStateDisplay()
		{
			switch (ControlPointState)
			{
				case ControlPointState.Normal:
					SetPosition(CurrentControlPoint, ControlPointPhysicalPosition);
					CurrentControlPoint.Visibility = ControlPointVisibility;

					if (ControlPointHover != null) ControlPointHover.Visibility = Visibility.Collapsed;
					if (ControlPointDrag != null) ControlPointDrag.Visibility = Visibility.Collapsed;
					break;

				case ControlPointState.Hover:
					if (ControlPointHover != null)
					{
						SetPosition(ControlPointHover, ControlPointPhysicalPosition);
						ControlPointHover.Visibility = ControlPointVisibility;

						if (ControlPoint != null) ControlPoint.Visibility = Visibility.Collapsed;
						if (ControlPointDrag != null) ControlPointDrag.Visibility = Visibility.Collapsed;
					}
					else
					{
						SetPosition(ControlPoint, ControlPointPhysicalPosition);
						ControlPoint.Visibility = ControlPointVisibility;
						if (ControlPointDrag != null) ControlPointDrag.Visibility = Visibility.Collapsed;
					}
					break;

				case ControlPointState.Drag:
					if (ControlPointDrag != null)
					{
						SetPosition(ControlPointDrag, ControlPointPhysicalPosition);
						ControlPointDrag.Visibility = ControlPointVisibility;

						if (ControlPoint != null) ControlPoint.Visibility = Visibility.Collapsed;
						if (ControlPointHover != null) ControlPointHover.Visibility = Visibility.Collapsed;
					}
					else
					{
						SetPosition(ControlPoint, ControlPointPhysicalPosition);
						ControlPoint.Visibility = ControlPointVisibility;
						if (ControlPointHover != null) ControlPointHover.Visibility = Visibility.Collapsed;
					}
					break;
			}
		}


		private void AddControlPointHandlers(Shape shape)
		{
			shape.MouseEnter += ControlPointOnMouseEnter;
			shape.MouseMove += HandleOnMouseMove;
			shape.MouseLeave += ControlPointOnMouseLeave;
			shape.MouseLeftButtonDown += ControlPointOnMouseLeftButtonDown;
			shape.MouseLeftButtonUp += ControlPointOnMouseLeftButtonUp;
		}

		private void RemoveControlPointHandlers(Shape shape)
		{
			shape.MouseEnter -= ControlPointOnMouseEnter;
			shape.MouseMove -= HandleOnMouseMove;
			shape.MouseLeave -= ControlPointOnMouseLeave;
			shape.MouseLeftButtonDown -= ControlPointOnMouseLeftButtonDown;
			shape.MouseLeftButtonUp -= ControlPointOnMouseLeftButtonUp;
		}

		private void ControlPointOnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			//if (ControlPointState != ControlPointState.Drag)
			if(!isDragging)
				ControlPointState = ControlPointState.Hover;
		}

		private void ControlPointOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
		{
			//if (ControlPointState != ControlPointState.Drag)
			if (!isDragging)
				ControlPointState = ControlPointState.Normal;
		}

		private void ControlPointOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			isDragging = true;
			ControlPointState = ControlPointState.Drag;
			ActiveControlPoint.CaptureMouse();
		}

		private void ControlPointOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			isDragging = false;
			ControlPointState = ControlPointState.Hover;
		}

		#endregion

		private void HandleOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
			if (isDragging)
			{
				Point p = mouseEventArgs.GetPosition(PlotSurface);

				if (p.X < 0) p.X = 0;
				if (p.X > ActualWidth - 1) p.X = ActualWidth - 1;
				if (p.Y < 0) p.Y = 0;
				if (p.Y > ActualHeight - 1) p.Y = ActualHeight - 1;
				ControlPointPhysicalPosition = p;
			}
		}

		//private void OnSizeChanged(Size size)
		//{
		//    //Width = size.Width;
		//    //Height = size.Height;
		//    ClipToBounds(size.Width, size.Height);
		//}

		#region MinimumLogicalCoordinate Property

		#endregion MinimumLogicalCoordinate Property

		#region MaximumLogicalCoordinate Property

		#endregion MaximumPlotCoordinate Property

		#region Line Plot

		#region Coordinates property

		#endregion

		#region Polyline property

		#endregion


		#endregion

	}
}