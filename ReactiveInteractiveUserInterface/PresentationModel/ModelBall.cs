//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;
using System.Windows.Media;
using System;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            TopBackingField = top;
            LeftBackingField = left;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        #region IBall

        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                if (TopBackingField == value)
                    return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                if (LeftBackingField == value)
                    return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter { get; init; } = 20;
        public Brush Filling { get; set; } = Brushes.Red;

        private static readonly Random _random = new();

        private Color currentColor = Colors.Red;
        private int colorPhase = 0;
        public void SetRandomColor()
        {
            if (!(Filling is SolidColorBrush solidColorBrush))
            {
                Filling = new SolidColorBrush(Colors.Red);
                currentColor = Colors.Red; 
                colorPhase = 0;
            }
            else
            {
                currentColor = solidColorBrush.Color;
            }

            byte r = currentColor.R;
            byte g = currentColor.G;
            byte b = currentColor.B;

            // Logika płynnego przechodzenia przez kolory
            switch (colorPhase)
            {
                case 0: // Czerwony -> Żółty (zwiększ G)
                    if (g < 255) g++; else colorPhase = 1;
                    break;
                case 1: // Żółty -> Zielony (zmniejsz R)
                    if (r > 0) r--; else colorPhase = 2;
                    break;
                case 2: // Zielony -> Cyjan (zwiększ B)
                    if (b < 255) b++; else colorPhase = 3;
                    break;
                case 3: // Cyjan -> Niebieski (zmniejsz G)
                    if (g > 0) g--; else colorPhase = 4;
                    break;
                case 4: // Niebieski -> Magenta (zwiększ R)
                    if (r < 255) r++; else colorPhase = 5;
                    break;
                case 5: // Magenta -> Czerwony (zmniejsz B)
                    if (b > 0) b--; else colorPhase = 0;
                    break;
            }

            Filling = new SolidColorBrush(Color.FromRgb(r, g, b));
            RaisePropertyChanged(nameof(Filling));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double TopBackingField;
        private double LeftBackingField;

        private void NewPositionNotification(object sender, IPosition e)
        {
            Top = e.y - Diameter / 2; Left = e.x - Diameter / 2;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion private

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }

        #endregion testing instrumentation
    }
}