using System;
using System.ComponentModel;
using System.Windows.Forms;
using iCodeGenerator.GenericDataAccess;
using iCodeGenerator.DatabaseStructure;
using System.Collections.Generic;
//using TD.SandBar;

namespace iCodeGenerator.DatabaseNavigator
{
	public class NavigatorControl : UserControl
	{
		private enum NavigatorIcon : int
		{
			ServerOff,
			ServerOn,
			DatabaseOff,
			DatabaseOn,
			Table,
			Column
		}

		#region Attributes

        private TreeViewMultiSelect uiNavigatorTreeView;
		private IContainer components;

        //private ShortcutListener _shortcuts = null;
        //private MenuBar _menuBar = null;
        //private ContextMenuBarItem _contextMenu = null;
        private ContextMenuStrip _contextMenuStrip = null;
		private ImageList uiNavigatorImageList;

		private TreeNode _rootNode;

		#endregion

		[Browsable(true),Category("Navigator")]
		public string ConnectionString
		{
			set{ Server.ConnectionString = value; }
			get{ return Server.ConnectionString; }
		}

		[Browsable(true),Category("Navigator")]
		public DataProviderType ProviderType
		{
			set{ Server.ProviderType = value; }
			get{ return Server.ProviderType; }
		}

		public NavigatorControl()
		{
			InitializeComponent();
			InitializeMenu();
			InitializeTree();
		}

		private void InitializeTree()
		{
			_rootNode = new TreeNode("Server");
			_rootNode.Tag = new Server();			
			_rootNode.ImageIndex = (int)NavigatorIcon.ServerOff;
			_rootNode.SelectedImageIndex = (int)NavigatorIcon.ServerOff;			
			uiNavigatorTreeView.Nodes.Add(_rootNode);			
		}

		private void InitializeMenu()
		{
            //_shortcuts = new ShortcutListener();
            //_menuBar = new MenuBar();
			
			SetDefaultMenu();
            this.uiNavigatorTreeView.ContextMenuStrip = _contextMenuStrip;

            //_shortcuts.UpdateAcceleratorTable(new TopLevelMenuItemBase[] { _contextMenu });
	
            //_menuBar.Buttons.Add(_contextMenu);
            //_menuBar.SetSandBarMenu(this, _contextMenu);
		}

		// Server Activate
		private void SetDefaultMenu()
		{
            //if (_contextMenu == null)
            //{
            //    _contextMenu = new ContextMenuBarItem();
            //}
            if (_contextMenuStrip == null)
            {
                _contextMenuStrip = new ContextMenuStrip();
            }
            //_contextMenu.MenuItems.Clear();
            _contextMenuStrip.Items.Clear();
            //MenuButtonItem connect = new MenuButtonItem("Connect");
            //connect.Activate += new EventHandler(connect_Activate);
            ToolStripItem connect = new ToolStripMenuItem("Connect");
            connect.Click += new EventHandler(connect_Activate);

            //MenuButtonItem disconnect = new MenuButtonItem("Disconnect");
            //disconnect.Activate += new EventHandler(disconnect_Activate);
            ToolStripItem disconnect = new ToolStripMenuItem("Disconnect");
            disconnect.Click += new EventHandler(disconnect_Activate);

            //MenuButtonItem edit = new MenuButtonItem("Edit");
            //edit.Activate += new EventHandler(serverEdit_Activate);
            ToolStripItem edit = new ToolStripMenuItem("Edit");
            edit.Click += new EventHandler(serverEdit_Activate);
			

            //_contextMenu.MenuItems.AddRange(new MenuButtonItem[] { connect, edit, disconnect });
            _contextMenuStrip.Items.AddRange(new ToolStripItem[] { connect, edit, disconnect });

		}

		private void connect_Activate(object sender, EventArgs e)
		{
			Connect();
		}

		public void Connect()
		{
			try
			{
				_rootNode.Nodes.Clear();
				Server server = new Server();
                _rootNode.Text = ProviderType.ToString() + " " + ConnectionString.Split(new char[] { ';' } )[0]; //do not show the whole connection string, because password is included
				foreach(Database database in server.Databases)
				{
					TreeNode databaseNode = new TreeNode(database.Name);
					databaseNode.Tag = database;
					databaseNode.ImageIndex = (int)NavigatorIcon.DatabaseOff;
					databaseNode.SelectedImageIndex = (int)NavigatorIcon.DatabaseOff;
					_rootNode.Nodes.Add(databaseNode);
				}
				_rootNode.SelectedImageIndex = (int)NavigatorIcon.ServerOn;
				_rootNode.ImageIndex = (int)NavigatorIcon.ServerOn;
			}
			catch (Exception)
			{
				ShowEditConnectionStringDialog();
			}
		}

		private void disconnect_Activate(object sender, EventArgs e)
		{
			
			Disconnect();
		}

		public void Disconnect()
		{
			_rootNode.Nodes.Clear();
			_rootNode.Text = "Server";
			_rootNode.SelectedImageIndex = (int)NavigatorIcon.ServerOff;
			_rootNode.ImageIndex = (int)NavigatorIcon.ServerOff;
		}

		private void serverEdit_Activate(object sender, EventArgs e)
		{
			ShowEditConnectionStringDialog();
		}

		public void ShowEditConnectionStringDialog()
		{
			ServerSettingsForm editForm = new ServerSettingsForm();
			if(editForm.ShowDialog(this) == DialogResult.OK)
			{
				Connect();	
			}
		}

		// Database Activate
		private void SetDatabaseMenu()
		{
            //_contextMenu.MenuItems.Clear();
            _contextMenuStrip.Items.Clear();
            //MenuButtonItem open = new MenuButtonItem("Open");
            //open.Activate += new EventHandler(databaseOpen_Activate);
            ToolStripItem open = new ToolStripMenuItem("Open");
            open.Click += new EventHandler(databaseOpen_Activate);
            //_contextMenu.MenuItems.AddRange(new MenuButtonItem[] { open });
            _contextMenuStrip.Items.AddRange(new ToolStripItem[] { open });
		}

		private void databaseOpen_Activate(object sender, EventArgs e)
		{
			OpenSelectedDatabase();
		}

		private void OpenSelectedDatabase()
		{
			TreeNode databaseNode = uiNavigatorTreeView.SelectedNode;
			databaseNode.Nodes.Clear();

			/* Changed by Ferhat */
			// Fill tree with tables
			foreach(Table table in ((Database)databaseNode.Tag).Tables)
			{
				TreeNode tableNode = new TreeNode(table.Name);				
				tableNode.Tag = table;
				tableNode.ImageIndex = (int)NavigatorIcon.Table;
				tableNode.SelectedImageIndex = (int)NavigatorIcon.Table;
				databaseNode.Nodes.Add(tableNode);
			}

			// Fill tree with views
			databaseNode = uiNavigatorTreeView.SelectedNode;
			foreach (Table table in ((Database)databaseNode.Tag).Views)
			{
				TreeNode tableNode = new TreeNode(table.Name);
				tableNode.Tag = table;
				tableNode.ImageIndex = (int)NavigatorIcon.Table;
				tableNode.SelectedImageIndex = (int)NavigatorIcon.Table;
				databaseNode.Nodes.Add(tableNode);
			}

			databaseNode.ImageIndex = (int)NavigatorIcon.DatabaseOn;
			databaseNode.SelectedImageIndex = (int)NavigatorIcon.DatabaseOn;
		}

		// Table Activate
		private void SetTableMenu()
		{
            //_contextMenu.MenuItems.Clear();
            _contextMenuStrip.Items.Clear();
            //MenuButtonItem open = new MenuButtonItem("Open");
            //open.Activate += new EventHandler(tableOpen_Activate);
            ToolStripItem open = new ToolStripMenuItem("Open");
            open.Click += new EventHandler(tableOpen_Activate);
            //_contextMenu.MenuItems.AddRange( new MenuButtonItem[] { open } );
            _contextMenuStrip.Items.AddRange(new ToolStripItem[] { open });
		}

		private void tableOpen_Activate(object sender, EventArgs e)
		{
			OpenSelectedTable();
		}

		private void OpenSelectedTable()
		{
			TreeNode tableNode = uiNavigatorTreeView.SelectedNode;
			tableNode.Nodes.Clear();
			foreach(Column column in ((Table)tableNode.Tag).Columns)
			{
				TreeNode columnNode = new TreeNode(column.Name);
				columnNode.Tag = column;
				columnNode.ImageIndex = (int)NavigatorIcon.Column;
				columnNode.SelectedImageIndex = (int)NavigatorIcon.Column;
				tableNode.Nodes.Add(columnNode);
			}			
		}

		// Column Activate
		private void SetColumnMenu()
		{

            //_contextMenu.MenuItems.Clear();
            //MenuButtonItem remove = new MenuButtonItem("Remove");
            //remove.Activate += new EventHandler(columnRemove_Activate);
            //MenuButtonItem showProperties = new MenuButtonItem("Properties");
            //showProperties.Activate += new EventHandler(columnShowProperties);

            _contextMenuStrip.Items.Clear();
            ToolStripItem remove = new ToolStripMenuItem("Remove");
            remove.Click += new EventHandler(columnRemove_Activate);
            ToolStripItem showProperties = new ToolStripMenuItem("Properties");
            showProperties.Click += new EventHandler(columnShowProperties);
            _contextMenuStrip.Items.AddRange(new ToolStripItem[] { remove, showProperties });
		}

		private void columnShowProperties(object sender, EventArgs e)
		{
			Column column = (Column)uiNavigatorTreeView.SelectedNode.Tag;
			OnColumnShowProperties(new ColumnEventArgs(column));
		}

		private void columnRemove_Activate(object sender, EventArgs e)
		{
			TreeNode node = uiNavigatorTreeView.SelectedNode;
			Column column = node.Tag as Column;
			column.ParentTable.Columns.Remove(column);
			node.Remove();
		}


		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
            //if(_shortcuts.ShortcutActivated(keyData))
            //    return true;

			return base.ProcessCmdKey (ref msg, keyData);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
                //_shortcuts.Dispose();
                //_menuBar.SetSandBarMenu(this,null);
                //_menuBar.Dispose();
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigatorControl));
            this.uiNavigatorTreeView = new iCodeGenerator.DatabaseNavigator.TreeViewMultiSelect();
            this.uiNavigatorImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // uiNavigatorTreeView
            // 
            this.uiNavigatorTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiNavigatorTreeView.ImageIndex = 0;
            this.uiNavigatorTreeView.ImageList = this.uiNavigatorImageList;
            this.uiNavigatorTreeView.Location = new System.Drawing.Point(0, 0);
            this.uiNavigatorTreeView.Name = "uiNavigatorTreeView";
            this.uiNavigatorTreeView.SelectedImageIndex = 0;
            this.uiNavigatorTreeView.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("uiNavigatorTreeView.SelectedNodes")));
            this.uiNavigatorTreeView.Size = new System.Drawing.Size(150, 150);
            this.uiNavigatorTreeView.TabIndex = 0;
            this.uiNavigatorTreeView.DoubleClick += new System.EventHandler(this.uiNavigatorTreeView_DoubleClick);
            this.uiNavigatorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.uiNavigatorTreeView_AfterSelect);
            this.uiNavigatorTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uiNavigatorTreeView_KeyUp);
            // 
            // uiNavigatorImageList
            // 
            this.uiNavigatorImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("uiNavigatorImageList.ImageStream")));
            this.uiNavigatorImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.uiNavigatorImageList.Images.SetKeyName(0, "");
            this.uiNavigatorImageList.Images.SetKeyName(1, "");
            this.uiNavigatorImageList.Images.SetKeyName(2, "");
            this.uiNavigatorImageList.Images.SetKeyName(3, "");
            this.uiNavigatorImageList.Images.SetKeyName(4, "");
            this.uiNavigatorImageList.Images.SetKeyName(5, "");
            // 
            // NavigatorControl
            // 
            this.Controls.Add(this.uiNavigatorTreeView);
            this.Name = "NavigatorControl";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.uiNavigatorTreeView_KeyUp);
            this.ResumeLayout(false);

		}
		#endregion


		private void uiNavigatorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			object obj = e.Node.Tag;
			if(obj == null) return;
			if(obj.GetType() == typeof(Server))
			{
				SetDefaultMenu();
			}
			else if(obj.GetType() == typeof(Database))
			{
				SetDatabaseMenu();
				OnDatabaseSelect(new DatabaseEventArgs((Database) uiNavigatorTreeView.SelectedNode.Tag));
			}
			else if(obj.GetType() == typeof(Table) )
			{
				SetTableMenu();
				OnTableSelect(new TableEventArgs((Table) uiNavigatorTreeView.SelectedNode.Tag));

                List<Table> tables = new List<Table>();
                foreach (TreeNode node in uiNavigatorTreeView.SelectedNodes)
                {
                    if (node.Tag.GetType() == typeof(Table))
                    {
                        tables.Add((Table)node.Tag);
                    }
                }
                OnTablesSelect(new TablesEventArgs(tables.ToArray()));
			}
			else if( obj.GetType() == typeof(Column) )
			{
				SetColumnMenu();
				OnColumnSelect(new ColumnEventArgs((Column)uiNavigatorTreeView.SelectedNode.Tag));
			}
		}

		private void uiNavigatorTreeView_DoubleClick(object sender, EventArgs e)
		{
			OpenSelectedItem();
		}
		
		private void uiNavigatorTreeView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				OpenSelectedItem();
			}
		}

		private void OpenSelectedItem()
		{
			object obj = uiNavigatorTreeView.SelectedNode.Tag;
			if(obj.GetType() == typeof(Database))
			{
				OpenSelectedDatabase();
			}
			else if(obj.GetType() == typeof(Table) )
			{
				OpenSelectedTable();
			}
		}

		
		#region Events & Delegates

// Column Events & Delegates

		public delegate void ColumnEventHandler(object sender, ColumnEventArgs args);

		[Browsable(true),Category("Navigator")]
		public event ColumnEventHandler ColumnSelect;
		protected virtual void OnColumnSelect(ColumnEventArgs args)
		{
			if(ColumnSelect != null)
			{
				ColumnSelect(this,args);
			}
		}
		
		[Browsable(true),Category("Navigator")]
		public event ColumnEventHandler ColumnShowProperties;
		protected virtual void OnColumnShowProperties(ColumnEventArgs args)
		{
			if(ColumnShowProperties != null)
			{
				ColumnShowProperties(this,args);
			}
		}
	
		// Table Events & Delegates

		public delegate void TableEventHandler(object sender, TableEventArgs args);

		[Browsable(true),Category("Navigator")]
		public event TableEventHandler TableSelect;
		protected virtual void OnTableSelect(TableEventArgs args)
		{
			if(TableSelect != null)
			{
				TableSelect(this,args);
			}
		}

        public delegate void TablesEventHandler(object sender, TablesEventArgs args);

        [Browsable(true), Category("Navigator")]
        public event TablesEventHandler TablesSelect;
        protected virtual void OnTablesSelect(TablesEventArgs args)
        {
            if (TablesSelect != null)
            {
                TablesSelect(this, args);
            }
        }
		
		
		// Database Events & Delegates

		public delegate void DatabaseEventHandler(object sender, DatabaseEventArgs args);

		[Browsable(true),Category("Navigator")]
		public event DatabaseEventHandler DatabaseSelect;
		protected virtual void OnDatabaseSelect(DatabaseEventArgs args)
		{
			if(DatabaseSelect != null)
			{
				DatabaseSelect(this,args);
			}
		}

		#endregion



	}
}