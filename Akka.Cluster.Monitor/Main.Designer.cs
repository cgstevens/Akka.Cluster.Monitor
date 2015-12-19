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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.seenByListView = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.unreachableListView = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.DownClusterButton = new System.Windows.Forms.Button();
            this.LeaveClusterButton = new System.Windows.Forms.Button();
            this.clusterListView = new System.Windows.Forms.ListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.clusterListBox = new System.Windows.Forms.ListBox();
            this.tabPage3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.seenByListView);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.unreachableListView);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.DownClusterButton);
            this.tabPage3.Controls.Add(this.LeaveClusterButton);
            this.tabPage3.Controls.Add(this.clusterListView);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(816, 507);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "MyCluster";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 378);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "SeenBy Members";
            // 
            // seenByListView
            // 
            this.seenByListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.seenByListView.Location = new System.Drawing.Point(28, 394);
            this.seenByListView.Name = "seenByListView";
            this.seenByListView.Size = new System.Drawing.Size(786, 110);
            this.seenByListView.TabIndex = 18;
            this.seenByListView.UseCompatibleStateImageBehavior = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Unreachable Members";
            // 
            // unreachableListView
            // 
            this.unreachableListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.unreachableListView.Location = new System.Drawing.Point(28, 251);
            this.unreachableListView.Name = "unreachableListView";
            this.unreachableListView.Size = new System.Drawing.Size(785, 111);
            this.unreachableListView.TabIndex = 16;
            this.unreachableListView.UseCompatibleStateImageBehavior = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Cluster Members";
            // 
            // DownClusterButton
            // 
            this.DownClusterButton.Location = new System.Drawing.Point(652, 10);
            this.DownClusterButton.Name = "DownClusterButton";
            this.DownClusterButton.Size = new System.Drawing.Size(75, 23);
            this.DownClusterButton.TabIndex = 8;
            this.DownClusterButton.Text = "Down";
            this.DownClusterButton.UseVisualStyleBackColor = true;
            this.DownClusterButton.Click += new System.EventHandler(this.DownClusterButton_Click);
            // 
            // LeaveClusterButton
            // 
            this.LeaveClusterButton.Location = new System.Drawing.Point(733, 10);
            this.LeaveClusterButton.Name = "LeaveClusterButton";
            this.LeaveClusterButton.Size = new System.Drawing.Size(75, 23);
            this.LeaveClusterButton.TabIndex = 7;
            this.LeaveClusterButton.Text = "Leave Cluster";
            this.LeaveClusterButton.UseVisualStyleBackColor = true;
            this.LeaveClusterButton.Click += new System.EventHandler(this.LeaveClusterButton_Click);
            // 
            // clusterListView
            // 
            this.clusterListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clusterListView.Location = new System.Drawing.Point(28, 39);
            this.clusterListView.Name = "clusterListView";
            this.clusterListView.Size = new System.Drawing.Size(785, 183);
            this.clusterListView.TabIndex = 5;
            this.clusterListView.UseCompatibleStateImageBehavior = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(824, 533);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.clusterListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(816, 507);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Log Messages";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // clusterListBox
            // 
            this.clusterListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterListBox.FormattingEnabled = true;
            this.clusterListBox.Location = new System.Drawing.Point(3, 3);
            this.clusterListBox.Name = "clusterListBox";
            this.clusterListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.clusterListBox.Size = new System.Drawing.Size(810, 501);
            this.clusterListBox.TabIndex = 4;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 533);
            this.Controls.Add(this.tabControl1);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button DownClusterButton;
        private System.Windows.Forms.Button LeaveClusterButton;
        private System.Windows.Forms.ListView clusterListView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView seenByListView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView unreachableListView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox clusterListBox;
    }
}

