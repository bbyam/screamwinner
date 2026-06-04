using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ScreamControl
{
    internal class ConfettiBurst : IAnimation
    {
        private readonly Random _random = new();
        private readonly List<Particle> _particles = new();

        private const int ParticleCount = 300;

        private readonly Canvas _canvas;
        private readonly int _locX;
        private readonly int _locY;
        private readonly double _scale;
        private readonly int _particleBottom;

        Color[] golds = [
            Color.FromRgb(255, 225, 120),
            Color.FromRgb(255, 235, 130),
            Color.FromRgb(230, 200, 90),
            Color.FromRgb(255, 200, 110)
        ];


        public ConfettiBurst(Canvas canvas, double locationXPercent, double locationYPercent, double scale)
        {
            _canvas = canvas;
            _locX = (int)(canvas.ActualWidth * locationXPercent);
            _locY = (int)(canvas.ActualHeight * locationYPercent);

            _scale = scale;
            _particleBottom = (int)(_canvas.ActualHeight + (20 * scale));

            CreateBurst(ParticleCount);
        }

        private void CreateBurst(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Rectangle rect = new Rectangle
                {
                    Width = _random.Next(12, 30) * _scale,
                    Height = _random.Next(8, 16) * _scale,
                    RadiusX = 1,
                    RadiusY = 1,
                    Fill = CreateGoldBrush(),
                    Opacity = 1
                };

                _canvas.Children.Add(rect);

                double angle = _random.NextDouble() * Math.PI * 2;
                double speed = 3 + _random.NextDouble() * 8;

                var particle = new Particle
                {
                    Shape = rect,
                    X = _locX,
                    Y = _locY,

                    VelocityX = Math.Cos(angle) * speed * 0.4,
                    VelocityY = (Math.Sin(angle) * speed - 11) * .7,

                    Rotation = _random.NextDouble() * 360,
                    RotationSpeed = -10 + _random.NextDouble() * 20,

                    ShimmerOffset = _random.NextDouble() * Math.PI * 2,

                    Alive = true
                };

                _particles.Add(particle);
            }
        }

        public void ReceiveEvent(ScreamEvents screamEvent)
        {
        }

        private Brush CreateGoldBrush()
        {
            return new SolidColorBrush(
                golds[_random.Next(golds.Length)]);
        }


        public void OnAnimate()
        {
            var deadCount = 0;
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                Particle p = _particles[i];

                if (!p.Alive)
                {
                    ++deadCount;
                    continue;
                }

                // Gravity
                p.VelocityY += 0.26;

                // Air drag
                p.VelocityX *= 0.995;
                p.VelocityY *= 0.995;

                // Flutter
                p.VelocityX += Math.Sin(p.Rotation * 0.02) * 0.03;

                // Move
                p.X += p.VelocityX;
                p.Y += p.VelocityY;

                // Rotate
                p.Rotation += p.RotationSpeed;

                double shimmer = Math.Abs(Math.Sin(p.Rotation * 0.05 + p.ShimmerOffset));
                double scaleY = 0.2 + shimmer;

                // Apply transforms
                p.Shape.RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(
                            1,
                            scaleY,
                            p.Shape.Width / 2,
                            p.Shape.Height / 2),

                        new RotateTransform(
                            p.Rotation,
                            p.Shape.Width / 2,
                            p.Shape.Height / 2)
                    }
                };
                Canvas.SetLeft(p.Shape, p.X);
                Canvas.SetTop(p.Shape, p.Y);

                // Remove dead particles
                if (p.Y > _particleBottom)
                {
                    p.Alive = false;
                    p.Shape.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            if (deadCount >= _particles.Count)
            {
                foreach (var particle in _particles)
                {
                    _canvas.Children.Remove(particle.Shape);
                }
                _particles.Clear();
            }
        }

        public bool IsDead()
        {
            return _particles.Count == 0;
        }

        private class Particle
        {
            public Rectangle Shape = null!;

            public double X;
            public double Y;

            public double VelocityX;
            public double VelocityY;

            public double Rotation;
            public double RotationSpeed;

            public double ShimmerOffset;

            public bool Alive;
        }
    }
}
