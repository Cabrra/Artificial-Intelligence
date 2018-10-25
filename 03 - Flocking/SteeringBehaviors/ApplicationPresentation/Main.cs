using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FullSailAFI.SteeringBehaviors.Core;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace FullSailAFI.SteeringBehaviors.ApplicationPresentation
{
    public partial class Main : Form
    {
        #region Non designer fields

        int frameCount;
        int shipCount;
        float deltaTimeAccumulator;
        float oneSecondAccumulator;
        const float UpdateTimeStep = 0.04f;
        const int sliderPrecision = 100;
        Random rng;
        List<ITaskForce> taskForces;

        #region Managed DirectX stuff

        bool deviceLost;
        Device device;
        PresentParameters presentationParameters;
        Sprite sprite;
        Texture backgroundTexture;
        Texture shipTexture;

        #endregion

        #endregion

        public Main()
        {
            InitializeComponent();
            InitializeDirectX();
            deltaTimeAccumulator = 0;
            taskForces = new List<ITaskForce>(5);
            rng = new Random();
            // nothing selected, disable controls
            numShipsSelector.Enabled = false;
            flockRadiusSelector.Enabled = false;
            colorButton.Enabled = false;
            alignmentWeightSelector.Enabled = false;
            cohesionWeightSelector.Enabled = false;
            separationWeightSelector.Enabled = false;
        }

        private void InitializeDirectX()
        {
            // TODO: init directx stuff
            presentationParameters = new PresentParameters();
            presentationParameters.Windowed = true;
            presentationParameters.BackBufferWidth = renderControl.Width;
            presentationParameters.BackBufferHeight = renderControl.Height;
            presentationParameters.BackBufferFormat = Format.A8R8G8B8;
            presentationParameters.SwapEffect = SwapEffect.Discard;
            presentationParameters.PresentationInterval = PresentInterval.One;

            deviceLost = false;
            device = new Device(0, DeviceType.Hardware, renderControl, 
                CreateFlags.HardwareVertexProcessing, presentationParameters);
            device.DeviceLost += new EventHandler(device_DeviceLost);
            sprite = new Sprite(device);
            backgroundTexture = TextureLoader.FromFile(device, "ApplicationPresentation/spacefield.png");
            shipTexture = TextureLoader.FromFile(device, "ApplicationPresentation/ship.png");
        }

        private void RecoverDevice()
        {
            try
            {
                device.TestCooperativeLevel();
            }
            catch (DeviceLostException)
            {
                // Failed big time, so just wait a bit before
                // trying again
            }
            catch (DeviceNotResetException)
            {
                try
                {
                    device.Reset(presentationParameters);
                    deviceLost = false;
                }
                catch (DeviceLostException)
                {
                    // Failed big time, so just wait a bit before
                    // trying again
                }
            }
        }

        private void device_DeviceLost(object sender, EventArgs e)
        {
            //if (null != backgroundTexture)
            //    backgroundTexture.Dispose();
            //if (null != shipTexture)
            //    shipTexture.Dispose();
        }

        public void Render()
        {
            // If the device is empty or lost, don't bother rendering
            if (device == null || sprite == null)
                return;
            if (deviceLost)
                RecoverDevice();
            if (deviceLost)
                return;

            try
            {
                lock (this)
                {
                    device.BeginScene();
                    //device.Clear(ClearFlags.Target, Color.CornflowerBlue, 0, 0);
                    sprite.Begin(SpriteFlags.AlphaBlend);

                    sprite.Transform = Matrix.Identity;

                    sprite.Draw(backgroundTexture, Vector3.Empty,
                        Vector3.Empty, ColorValue.FromColor(Color.White).ToArgb());

                    foreach (ITaskForce tf in taskForces)
                    {
                        foreach (MovingObject ship in tf.Boids)
                        {
                            // set up matricies to determine where to render
                            Matrix transform = new Matrix();
                            transform = Matrix.Identity;

                            //// compute scaling in x and y axis
                            //float x = Convert.ToSingle(bobject.Dimensions.Width) / Convert.ToSingle(bobject.ImageDimensions.Width) * GlobalScale,
                            //    y = Convert.ToSingle(bobject.Dimensions.Height) / Convert.ToSingle(bobject.ImageDimensions.Height) * GlobalScale;

                            //// setup scaling matrix and
                            //// apply scaling to transform
                            //transform.Multiply(Matrix.Scaling(new Vector3(x, y, 1)));

                            // setup rotation matrix and
                            // apply rotation to transform
                            transform.Multiply(Matrix.RotationZ(ship.Heading));

                            // setup translate matrix and
                            // apply translation to transform
                            transform.Multiply(Matrix.Translation(ship.Position));

                            // apply transform to sprite object
                            sprite.Transform = transform;

                            sprite.Draw(shipTexture, new Vector3(ship.CollisionRadius, ship.CollisionRadius, 0),
                                Vector3.Empty, tf.Color.ToArgb());
                        }
                    }

                    sprite.End();
                    device.EndScene();
                    device.Present();
                }
            }
            catch (DeviceLostException)
            {
                deviceLost = true;
            }

            // increment the frame counter for FPS display
            ++frameCount;
        }

        private void UpdateTaskForces()
        {
            // Let's not try to update if Main hasn't finished initializing
            if (null == taskForces)
                return;

            shipCount = 0;

            // update all objects
            foreach (ITaskForce tf in taskForces)
            {
                lock (this)
                {
                    shipCount += tf.Boids.Count;

                    // perform steering updates (flocking)
                    tf.Update(UpdateTimeStep);

                    // bounds check
                    foreach (MovingObject ship in tf.Boids)
                    {
                        Vector3 position = ship.Position;
                        if (position.X + ship.CollisionRadius < 0)
                            position.X = renderControl.Width + ship.CollisionRadius;
                        if (position.X - ship.CollisionRadius > renderControl.Width)
                            position.X = -ship.CollisionRadius;
                        if (position.Y + ship.CollisionRadius < 0)
                            position.Y = renderControl.Height + ship.CollisionRadius;
                        if (position.Y - ship.CollisionRadius > renderControl.Height)
                            position.Y = -ship.CollisionRadius;
                        ship.Position = position;
                    }

                    totalShipsInfo.Text = "Total Ships: " + shipCount.ToString();
                }
            }
        }

        private void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (false == updateTimer.Enabled)
                return;
            float deltaTime = Convert.ToSingle(updateTimer.Interval) / 1000.0f; // Update calls "should" always be 100Hz
            deltaTimeAccumulator += deltaTime;
            oneSecondAccumulator += deltaTime;
            if (deltaTimeAccumulator > UpdateTimeStep)
            {
                // update accumulator and prevent multiple update calls
                deltaTimeAccumulator -= UpdateTimeStep;
                if (deltaTimeAccumulator > UpdateTimeStep)
                    deltaTimeAccumulator = 0;

                UpdateTaskForces();

            }
            if (oneSecondAccumulator >= 1.0f)
            {
                oneSecondAccumulator = 0;
                lock (this)
                {
                    FPSInfo.Text = "FPS: " + frameCount.ToString();
                    frameCount = 0;
                }
            }
        }

        private void addTaskForceButton_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf;
                if (fullSailImplementationButton.Checked)
                    tf = new ExampleTaskForce();
                else
                    tf = new TaskForce();
                tf.AveragePosition = new Vector3(Convert.ToSingle(
                    rng.NextDouble() * renderControl.Width), Convert.ToSingle(
                    rng.NextDouble() * renderControl.Height), 0);
                tf.SetShipAmount(20);
                taskForces.Add(tf);
                taskForceSelector.Items.Add(tf.ToString());
                taskForceSelector.SelectedIndex = taskForceSelector.Items.Count - 1;
            }
        }

        private void removeTaskForceButton_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                int index = taskForceSelector.SelectedIndex;
                if (index >= 0)
                {
                    taskForceSelector.Items.RemoveAt(index);
                    taskForces[index].RemoveAllShips();
                    taskForces.RemoveAt(index);
                    if (0 != taskForces.Count)
                    {
                        if (index > taskForces.Count - 1)
                            index = taskForces.Count - 1;
                        taskForceSelector.SelectedIndex = index;
                    }
                    else taskForceSelector.SelectedIndex = -1;
                }
            }
        }

        private void numShipsSelector_ValueChanged(object sender, EventArgs e)
        {
            ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
            lock (this)
            {
                tf.SetShipAmount(Convert.ToInt32(numShipsSelector.Value));
                taskForceSelector.Items[taskForceSelector.SelectedIndex] = tf.ToString();
            }
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
            ColorDialog cd = new ColorDialog();
            lock (this)
            {
                renderControl.Enabled = false;
                cd.ShowDialog(this);
                tf.Color = ColorValue.FromColor(cd.Color);
                renderControl.Enabled = true;
            }
            taskForceSelector.Items[taskForceSelector.SelectedIndex] = tf.ToString();
            colorButton.BackColor = cd.Color;
            colorButton.ForeColor = GetReadableForeColor(colorButton.BackColor);
        }

        private void alignmentWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.AlignmentStrength = Convert.ToSingle(alignmentWeightSelector.Value)
                    / sliderPrecision;
                alignmentWeightInfo.Text = tf.AlignmentStrength.ToString("F");
            }
        }

        private void cohesionWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.CohesionStrength = Convert.ToSingle(cohesionWeightSelector.Value)
                    / sliderPrecision;
                cohesionWeightInfo.Text = tf.CohesionStrength.ToString("F");
            }
        }

        private void separationWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.SeparationStrength = Convert.ToSingle(separationWeightSelector.Value)
                    / sliderPrecision;
                separationWeightInfo.Text = tf.SeparationStrength.ToString("F");
            }
        }

        private void flockRadiusSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.FlockRadius = Convert.ToSingle(flockRadiusSelector.Value);
            }
        }

        private void fullSailImplementationButton_CheckedChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (fullSailImplementationButton.Checked)
                {
                    List<ITaskForce> copy = new List<ITaskForce>(taskForces.Count);
                    for(int i = 0; i < taskForces.Count; ++i)
                    {
                        ITaskForce tf = new ExampleTaskForce();
                        tf.CopyFrom(taskForces[i]);
                        copy.Add(tf);
                    }
                    taskForces = copy;
                }
                else
                {
                    List<ITaskForce> copy = new List<ITaskForce>(taskForces.Count);
                    for (int i = 0; i < taskForces.Count; ++i)
                    {
                        ITaskForce tf = new TaskForce();
                        tf.CopyFrom(taskForces[i]);
                        copy.Add(tf);
                    }
                    taskForces = copy;
                }
            }
        }

        private void enableVSyncButton_CheckedChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (null != device)
                {
                    if (enableVSyncButton.Checked)
                        presentationParameters.PresentationInterval = PresentInterval.One;
                    else
                        presentationParameters.PresentationInterval = PresentInterval.Immediate;
                    device.Reset(presentationParameters);
                }
            }
        }

        private Color GetReadableForeColor(Color backColor)
        {
            if (backColor.GetBrightness() > 0.5f)
                return Color.Black;
            return Color.White;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer.Enabled = false;
        }

        private void taskForceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 == taskForceSelector.SelectedIndex)
            {
                // nothing selected, disable controls
                numShipsSelector.Enabled = false;
                colorButton.Enabled = false;
                alignmentWeightSelector.Enabled = false;
                cohesionWeightSelector.Enabled = false;
                separationWeightSelector.Enabled = false;
                flockRadiusSelector.Enabled = false;
            }
            else
            {
                lock (this)
                {
                    // reset controls to selected taskforce's information
                    ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                    numShipsSelector.Enabled = true;
                    numShipsSelector.Value = tf.Boids.Count;
                    colorButton.Enabled = true;
                    colorButton.BackColor = Color.FromArgb(tf.Color.ToArgb());
                    colorButton.ForeColor = GetReadableForeColor(colorButton.BackColor);
                    alignmentWeightSelector.Enabled = true;
                    alignmentWeightSelector.Value = Clamp(Convert.ToInt32(tf.AlignmentStrength * sliderPrecision), alignmentWeightSelector.Minimum, alignmentWeightSelector.Maximum);
                    alignmentWeightInfo.Text = tf.AlignmentStrength.ToString("F");
                    cohesionWeightSelector.Enabled = true;
                    cohesionWeightSelector.Value = Clamp(Convert.ToInt32(tf.CohesionStrength * sliderPrecision), cohesionWeightSelector.Minimum, cohesionWeightSelector.Maximum);
                    cohesionWeightInfo.Text = tf.CohesionStrength.ToString("F");
                    separationWeightSelector.Enabled = true;
                    separationWeightSelector.Value = Clamp(Convert.ToInt32(tf.SeparationStrength * sliderPrecision), separationWeightSelector.Minimum, separationWeightSelector.Maximum);
                    separationWeightInfo.Text = tf.SeparationStrength.ToString("F");
                    flockRadiusSelector.Enabled = true;
                    flockRadiusSelector.Value = Clamp(Convert.ToDecimal(tf.FlockRadius), flockRadiusSelector.Minimum, flockRadiusSelector.Maximum);
                }
            }
        }

        private T Clamp<T>(T value, T min, T max) where T : System.IComparable<T>
        {
            return (value.CompareTo(min) < 0) ? min : (value.CompareTo(max) > 0) ? max : value;
        }

    }
}
