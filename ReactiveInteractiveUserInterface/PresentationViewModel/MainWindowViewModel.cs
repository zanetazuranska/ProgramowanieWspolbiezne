//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;
using TP.ConcurrentProgramming.Data.WindowData;
using System.Windows.Input;
using System.ComponentModel;
using System.Timers;
using System.Windows;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        {
            StartSimulation = new RelayCommand(ExecuteStartSimulation);
        }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
            StartSimulation = new RelayCommand(ExecuteStartSimulation);
        }

        #endregion ctor

        #region public API

        public void Start(double diameter, WindowData windowData)
        {
            this.Diameter = diameter;
            this.WindowData = windowData;
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        public ICommand StartSimulation { get; }

        private System.Timers.Timer timer;
        public void ExecuteStartSimulation()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(ballAmount, Diameter, WindowData);
            Observer.Dispose();

            timer = new System.Timers.Timer(25);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += OnTimerElapsed;


            IsStartSimulationEnabled = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in Balls)
                {
                    item.SetRandomColor();
                }
            });
        }

        public int BallAmount
        {
            get => ballAmount;
            set
            {
                if (ballAmount != value)
                {
                    ballAmount = value;
                    RaisePropertyChanged(nameof(BallAmount));
                }
            }
        }

        public bool IsStartSimulationEnabled
        {
            get => isStartSimulationEnabled;
            set
            {
                if (isStartSimulationEnabled != value)
                {
                    isStartSimulationEnabled = value;
                    RaisePropertyChanged(nameof(IsStartSimulationEnabled));
                }
            }
        }
        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();

                    timer.Stop();
                    timer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        private bool isStartSimulationEnabled = true;
        private int ballAmount = 1;
        public double Diameter { get; private set; } = 1;
        public WindowData WindowData { get; private set; }

        #endregion private
    }
}