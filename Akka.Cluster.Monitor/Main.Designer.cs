namespace Akka.Cluster.Monitor
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.secondsTB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.startSchedule = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.unSubscribeBtn = new System.Windows.Forms.Button();
            this.subscribeBtn = new System.Windows.Forms.Button();
            this.systemNameTB = new System.Windows.Forms.TextBox();
            this.portTB = new System.Windows.Forms.TextBox();
            this.clusterBtn = new System.Windows.Forms.Button();
            this.ipAddressTB = new System.Windows.Forms.TextBox();
            this.DownClusterButton = new System.Windows.Forms.Button();
            this.LeaveClusterButton = new System.Windows.Forms.Button();
            this.clusterListView = new System.Windows.Forms.ListView();
            this.loggerBox = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.unreachableListView = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.seenByListView = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.systemNameTB);
            this.panel1.Controls.Add(this.portTB);
            this.panel1.Controls.Add(this.clusterBtn);
            this.panel1.Controls.Add(this.ipAddressTB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 33);
            this.panel1.TabIndex = 23;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel7.Controls.Add(this.label7);
            this.panel7.Controls.Add(this.secondsTB);
            this.panel7.Controls.Add(this.label6);
            this.panel7.Controls.Add(this.startSchedule);
            this.panel7.Location = new System.Drawing.Point(609, -2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(237, 33);
            this.panel7.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(110, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "sec";
            // 
            // secondsTB
            // 
            this.secondsTB.Location = new System.Drawing.Point(81, 6);
            this.secondsTB.Name = "secondsTB";
            this.secondsTB.Size = new System.Drawing.Size(28, 20);
            this.secondsTB.TabIndex = 25;
            this.secondsTB.Text = "2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Ask Events";
            // 
            // startSchedule
            // 
            this.startSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startSchedule.Location = new System.Drawing.Point(136, 5);
            this.startSchedule.Name = "startSchedule";
            this.startSchedule.Size = new System.Drawing.Size(95, 23);
            this.startSchedule.TabIndex = 22;
            this.startSchedule.Text = "Start Schedule";
            this.startSchedule.UseVisualStyleBackColor = true;
            this.startSchedule.Click += new System.EventHandler(this.startSchedule_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel6.Controls.Add(this.label5);
            this.panel6.Controls.Add(this.unSubscribeBtn);
            this.panel6.Controls.Add(this.subscribeBtn);
            this.panel6.Location = new System.Drawing.Point(363, -2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(243, 33);
            this.panel6.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(0, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Push Events";
            // 
            // unSubscribeBtn
            // 
            this.unSubscribeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.unSubscribeBtn.Location = new System.Drawing.Point(156, 5);
            this.unSubscribeBtn.Name = "unSubscribeBtn";
            this.unSubscribeBtn.Size = new System.Drawing.Size(80, 23);
            this.unSubscribeBtn.TabIndex = 22;
            this.unSubscribeBtn.Text = "UnSubscribe";
            this.unSubscribeBtn.UseVisualStyleBackColor = true;
            this.unSubscribeBtn.Click += new System.EventHandler(this.unSubscribeBtn_Click);
            // 
            // subscribeBtn
            // 
            this.subscribeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.subscribeBtn.Location = new System.Drawing.Point(80, 5);
            this.subscribeBtn.Name = "subscribeBtn";
            this.subscribeBtn.Size = new System.Drawing.Size(75, 23);
            this.subscribeBtn.TabIndex = 21;
            this.subscribeBtn.Text = "Subscribe";
            this.subscribeBtn.UseVisualStyleBackColor = true;
            this.subscribeBtn.Click += new System.EventHandler(this.subscribeBtn_Click);
            // 
            // systemNameTB
            // 
            this.systemNameTB.Location = new System.Drawing.Point(3, 4);
            this.systemNameTB.Name = "systemNameTB";
            this.systemNameTB.Size = new System.Drawing.Size(128, 20);
            this.systemNameTB.TabIndex = 13;
            this.systemNameTB.Text = "myservice";
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(232, 4);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(48, 20);
            this.portTB.TabIndex = 15;
            this.portTB.Text = "4053";
            // 
            // clusterBtn
            // 
            this.clusterBtn.Location = new System.Drawing.Point(286, 3);
            this.clusterBtn.Name = "clusterBtn";
            this.clusterBtn.Size = new System.Drawing.Size(75, 23);
            this.clusterBtn.TabIndex = 11;
            this.clusterBtn.Text = "Start System";
            this.clusterBtn.UseVisualStyleBackColor = true;
            this.clusterBtn.Click += new System.EventHandler(this.clusterBtn_Click);
            // 
            // ipAddressTB
            // 
            this.ipAddressTB.Location = new System.Drawing.Point(137, 4);
            this.ipAddressTB.Name = "ipAddressTB";
            this.ipAddressTB.Size = new System.Drawing.Size(89, 20);
            this.ipAddressTB.TabIndex = 14;
            this.ipAddressTB.Text = "127.0.0.1";
            // 
            // DownClusterButton
            // 
            this.DownClusterButton.Location = new System.Drawing.Point(845, 1);
            this.DownClusterButton.Name = "DownClusterButton";
            this.DownClusterButton.Size = new System.Drawing.Size(75, 23);
            this.DownClusterButton.TabIndex = 20;
            this.DownClusterButton.Text = "Down";
            this.DownClusterButton.UseVisualStyleBackColor = true;
            this.DownClusterButton.Click += new System.EventHandler(this.DownClusterButton_Click);
            // 
            // LeaveClusterButton
            // 
            this.LeaveClusterButton.Location = new System.Drawing.Point(926, 1);
            this.LeaveClusterButton.Name = "LeaveClusterButton";
            this.LeaveClusterButton.Size = new System.Drawing.Size(75, 23);
            this.LeaveClusterButton.TabIndex = 19;
            this.LeaveClusterButton.Text = "Leave Cluster";
            this.LeaveClusterButton.UseVisualStyleBackColor = true;
            this.LeaveClusterButton.Click += new System.EventHandler(this.LeaveClusterButton_Click);
            // 
            // clusterListView
            // 
            this.clusterListView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.clusterListView.Location = new System.Drawing.Point(0, 24);
            this.clusterListView.Name = "clusterListView";
            this.clusterListView.Size = new System.Drawing.Size(1004, 194);
            this.clusterListView.TabIndex = 18;
            this.clusterListView.UseCompatibleStateImageBehavior = false;
            // 
            // loggerBox
            // 
            this.loggerBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.loggerBox.FormattingEnabled = true;
            this.loggerBox.Location = new System.Drawing.Point(0, 19);
            this.loggerBox.Name = "loggerBox";
            this.loggerBox.Size = new System.Drawing.Size(1004, 160);
            this.loggerBox.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SlateGray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.DownClusterButton);
            this.panel2.Controls.Add(this.LeaveClusterButton);
            this.panel2.Controls.Add(this.clusterListView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1008, 222);
            this.panel2.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Cluster Members";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.unreachableListView);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 255);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1008, 143);
            this.panel3.TabIndex = 25;
            // 
            // unreachableListView
            // 
            this.unreachableListView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.unreachableListView.Location = new System.Drawing.Point(0, 23);
            this.unreachableListView.Name = "unreachableListView";
            this.unreachableListView.Size = new System.Drawing.Size(1004, 116);
            this.unreachableListView.TabIndex = 25;
            this.unreachableListView.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "Unreachable Members";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.seenByListView);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 398);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1008, 166);
            this.panel4.TabIndex = 26;
            // 
            // seenByListView
            // 
            this.seenByListView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.seenByListView.Location = new System.Drawing.Point(0, 21);
            this.seenByListView.Name = "seenByListView";
            this.seenByListView.Size = new System.Drawing.Size(1004, 141);
            this.seenByListView.TabIndex = 25;
            this.seenByListView.UseCompatibleStateImageBehavior = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 17);
            this.label3.TabIndex = 24;
            this.label3.Text = "SeenBy Members";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.loggerBox);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 564);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1008, 183);
            this.panel5.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 24;
            this.label4.Text = "Logger";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox systemNameTB;
        private System.Windows.Forms.TextBox portTB;
        private System.Windows.Forms.Button clusterBtn;
        private System.Windows.Forms.TextBox ipAddressTB;
        private System.Windows.Forms.Button unSubscribeBtn;
        private System.Windows.Forms.Button subscribeBtn;
        private System.Windows.Forms.Button DownClusterButton;
        private System.Windows.Forms.Button LeaveClusterButton;
        private System.Windows.Forms.ListView clusterListView;
        private System.Windows.Forms.ListBox loggerBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView unreachableListView;
        private System.Windows.Forms.ListView seenByListView;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox secondsTB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button startSchedule;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label5;
    }
}

