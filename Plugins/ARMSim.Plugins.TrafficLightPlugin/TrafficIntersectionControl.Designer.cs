namespace ARMSim.Plugins.TrafficLightPlugin
{
    partial class TrafficIntersectionControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainWalkSignal = new ARMSim.Plugins.UIControls.WalkSignal();
            this.mainTrafficLight = new ARMSim.Plugins.UIControls.TrafficLight();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sideWalkSignal = new ARMSim.Plugins.UIControls.WalkSignal();
            this.sideTrafficLight = new ARMSim.Plugins.UIControls.TrafficLight();
            this.mainButton = new System.Windows.Forms.Button();
            this.sideButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainWalkSignal
            // 
            this.mainWalkSignal.Location = new System.Drawing.Point(139, 5);
            this.mainWalkSignal.Name = "mainWalkSignal";
            this.mainWalkSignal.Size = new System.Drawing.Size(82, 73);
            this.mainWalkSignal.State = ARMSim.Plugins.UIControls.WalkSignal.XWalkStates.DONTWALK;
            this.mainWalkSignal.TabIndex = 0;
            // 
            // mainTrafficLight
            // 
            this.mainTrafficLight.Location = new System.Drawing.Point(14, 33);
            this.mainTrafficLight.Name = "mainTrafficLight";
            this.mainTrafficLight.Size = new System.Drawing.Size(109, 110);
            this.mainTrafficLight.State = ARMSim.Plugins.UIControls.TrafficLight.TrafficLightStates.BLACK;
            this.mainTrafficLight.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Main Street";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(246, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Side Street";
            // 
            // sideWalkSignal
            // 
            this.sideWalkSignal.Location = new System.Drawing.Point(379, 5);
            this.sideWalkSignal.Name = "sideWalkSignal";
            this.sideWalkSignal.Size = new System.Drawing.Size(82, 73);
            this.sideWalkSignal.State = ARMSim.Plugins.UIControls.WalkSignal.XWalkStates.DONTWALK;
            this.sideWalkSignal.TabIndex = 0;
            // 
            // sideTrafficLight
            // 
            this.sideTrafficLight.Location = new System.Drawing.Point(251, 33);
            this.sideTrafficLight.Name = "sideTrafficLight";
            this.sideTrafficLight.Size = new System.Drawing.Size(109, 110);
            this.sideTrafficLight.State = ARMSim.Plugins.UIControls.TrafficLight.TrafficLightStates.BLACK;
            this.sideTrafficLight.TabIndex = 1;
            // 
            // mainButton
            // 
            this.mainButton.Location = new System.Drawing.Point(138, 109);
            this.mainButton.Name = "mainButton";
            this.mainButton.Size = new System.Drawing.Size(83, 34);
            this.mainButton.TabIndex = 4;
            this.mainButton.Text = "XWalk Button";
            this.mainButton.UseVisualStyleBackColor = true;
            this.mainButton.Click += new System.EventHandler(this.mainButton_Click);
            // 
            // sideButton
            // 
            this.sideButton.Location = new System.Drawing.Point(379, 109);
            this.sideButton.Name = "sideButton";
            this.sideButton.Size = new System.Drawing.Size(82, 34);
            this.sideButton.TabIndex = 4;
            this.sideButton.Text = "XWalk Button";
            this.sideButton.UseVisualStyleBackColor = true;
            this.sideButton.Click += new System.EventHandler(this.sideButton_Click);
            // 
            // TrafficIntersectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sideButton);
            this.Controls.Add(this.mainButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sideTrafficLight);
            this.Controls.Add(this.sideWalkSignal);
            this.Controls.Add(this.mainTrafficLight);
            this.Controls.Add(this.mainWalkSignal);
            this.Name = "TrafficIntersectionControl";
            this.Size = new System.Drawing.Size(477, 153);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ARMSim.Plugins.UIControls.WalkSignal mainWalkSignal;
        private ARMSim.Plugins.UIControls.TrafficLight mainTrafficLight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ARMSim.Plugins.UIControls.WalkSignal sideWalkSignal;
        private ARMSim.Plugins.UIControls.TrafficLight sideTrafficLight;
        private System.Windows.Forms.Button mainButton;
        private System.Windows.Forms.Button sideButton;


    }
}
