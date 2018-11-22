using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletonJoints {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        KinectSensor _sensor;
        public MainWindow() {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            _sensor = KinectSensor.KinectSensors.FirstOrDefault();
            //KinectSensor.KinectSensors.Where(x => x.Status == KinectStatus.Connected).FirstOrDefault(); 
            if (_sensor != null) {
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                _sensor.SkeletonFrameReady += KinectSkeletonFrameReady;
                _sensor.SkeletonStream.Enable();
                _sensor.Start();
            }
            else {
                MessageBox.Show("No Kinect Connected!");
                Close();
            }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            if (_sensor != null) {
                _sensor.Stop();
            }
        }
        void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            using (var frame = e.OpenSkeletonFrame()) {
                if (frame != null) {
                    canvas.Children.Clear();
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    var skeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                    if (skeleton != null) {
                        // Draw skeleton joints.
                        foreach (JointType joint in Enum.GetValues(typeof(JointType))) {
                            DrawJoint(skeleton.Joints[joint].ScaleTo(640, 480));
                        }
                    }
                }
            }
        }
        private void DrawJoint(Joint joint) {
            Ellipse ellipse = new Ellipse {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.LightCoral)
            };
            Canvas.SetLeft(ellipse, joint.Position.X);
            Canvas.SetTop(ellipse, joint.Position.Y);
            canvas.Children.Add(ellipse);
        }
    }
}
