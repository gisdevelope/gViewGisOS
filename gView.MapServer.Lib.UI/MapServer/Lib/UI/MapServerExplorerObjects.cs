using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using gView.Framework.UI;
using gView.Framework.IO;
using gView.Framework.Data;
using gView.Framework.UI.Dialogs;
using gView.Framework.Carto;
using gView.Framework.system;
using gView.Framework.system.UI;
using System.Windows.Forms;
using gView.MapServer.Connector;
using gView.Framework.Globalisation;
using System.Resources;
using gView.MapServer;
using gView.Framework.UI.Controls.Filter;
using gView.Framework.Web;

namespace gView.MapServer.Lib.UI
{
    [gView.Framework.system.RegisterPlugIn("2763F3A3-AECE-42cc-A788-CD3020E79434")]
    public class MapServerExplorerObjects : ExplorerParentObject, IExplorerGroupObject
    {
        IExplorerIcon _icon = new MapServerIcon();

        public MapServerExplorerObjects()
            : base(null, null)
        {
        }

        #region IExplorerObject Member

        public string Name
        {
            get { return "gView Map Server"; }
        }

        public string FullName
        {
            get { return "gView.MapServer"; }
        }

        public string Type
        {
            get { return "gView.MapServer Connections"; }
        }

        public IExplorerIcon Icon
        {
            get { return _icon; }
        }

        public new object Object
        {
            get { return null; }
        }

        public IExplorerObject CreateInstanceByFullName(string FullName)
        {
            return null;
        }

        #endregion

        #region IExplorerParentObject Member

        public override void Refresh()
        {
            base.Refresh();
            base.AddChildObject(new MapServerNewConnectionExplorerObject(this));

            ConfigTextStream stream = new ConfigTextStream("gviewmapserver_connections");
            string connStr, id;
            while ((connStr = stream.Read(out id)) != null)
            {
                base.AddChildObject(new MapServerConnectionExplorerObject(this, id, connStr));
            }
            stream.Close();
        }

        #endregion

        #region ISerializableExplorerObject Member

        public IExplorerObject CreateInstanceByFullName(string FullName, ISerializableExplorerObjectCache cache)
        {
            if (cache.Contains(FullName)) return cache[FullName];

            if (FullName == this.FullName)
            {
                MapServerExplorerObjects obj = new MapServerExplorerObjects();
                cache.Append(obj);
                return obj;
            }

            return null;
        }

        #endregion
    }

    [gView.Framework.system.RegisterPlugIn("C91C6B78-047C-446B-85D9-0D57A3DD4FA7")]
    internal class MapServerNewConnectionExplorerObject : ExplorerObjectCls, IExplorerSimpleObject, IExplorerObjectDoubleClick, IExplorerObjectCreatable
    {
        IExplorerIcon _icon = new MapServerNewConnectionIcon();

        public MapServerNewConnectionExplorerObject()
            : base(null, null)
        {
        }

        public MapServerNewConnectionExplorerObject(IExplorerObject parent)
            : base(null, null)
        {
        }

        #region IExplorerObject Member

        public string Name
        {
            get { return LocalizedResources.GetResString("String.NewConnection", "New MapServer Connection..."); }
        }

        public string FullName
        {
            get { return ""; }
        }

        public string Type
        {
            get { return ""; }
        }

        public IExplorerIcon Icon
        {
            get { return _icon; }
        }

        public void Dispose()
        {

        }

        public new object Object
        {
            get { return null; }
        }

        public IExplorerObject CreateInstanceByFullName(string FullName)
        {
            return null;
        }

        #endregion

        #region IExplorerObjectDoubleClick Member

        public void ExplorerObjectDoubleClick(ExplorerObjectEventArgs e)
        {
            FormNewConnection dlg = new FormNewConnection();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string connStr = dlg.ConnectionString;
                ConfigTextStream stream = new ConfigTextStream("gviewmapserver_connections", true, true);
                string id = ConfigTextStream.ExtractValue(connStr, "server");
                if (id.IndexOf(":") != -1)
                {
                    id = id.Replace(":", " (Port=") + ")";
                }
                stream.Write(connStr, ref id);
                stream.Close();

                e.NewExplorerObject = new MapServerConnectionExplorerObject(this.ParentExplorerObject, id, dlg.ConnectionString);
            }
        }

        #endregion

        #region ISerializableExplorerObject Member

        public IExplorerObject CreateInstanceByFullName(string FullName, ISerializableExplorerObjectCache cache)
        {
            if (cache.Contains(FullName)) return cache[FullName];

            return null;
        }

        #endregion

        #region IExplorerObjectCreatable Member

        public bool CanCreate(IExplorerObject parentExObject)
        {
            return (parentExObject is MapServerExplorerObjects);
        }

        public IExplorerObject CreateExplorerObject(IExplorerObject parentExObject)
        {
            ExplorerObjectEventArgs e = new ExplorerObjectEventArgs();
            ExplorerObjectDoubleClick(e);
            return e.NewExplorerObject;
        }

        #endregion
    }

    internal class MapServerConnectionExplorerObject : ExplorerParentObject, IExplorerSimpleObject, IExplorerObjectContentDragDropEvents, IExplorerObjectDeletable, IExplorerObjectRenamable, IExplorerObjectContextMenu2, IRefreshedEventHandler
    {
        private string _name = "", _connectionString = "";
        private IExplorerIcon _icon = new MapServerConnectionIcon();
        private ToolStripItem[] _contextItems;

        public MapServerConnectionExplorerObject(IExplorerObject parent)
            : base(parent, null)
        {
            List<ToolStripItem> contextItems = new List<ToolStripItem>();

            ToolStripMenuItem importItem = new ToolStripMenuItem(LocalizedResources.GetResString("Menu.Import", "Import"));
            contextItems.Add(importItem);
            ToolStripMenuItem newService = new ToolStripMenuItem(LocalizedResources.GetResString("Menu.NewServiceFromMap", "New Service From gView Map..."));
            newService.Click += new EventHandler(newService_Click);
            newService.Image = (new MapServiceIcon()).Image;
            importItem.DropDownItems.Add(newService);

            //ToolStripMenuItem newServiceColl = new ToolStripMenuItem("New Map Service Collection ...");
            //newServiceColl.Click += new EventHandler(newServiceColl_Click);
            //importItem.DropDownItems.Add(newServiceColl);

            PlugInManager compMan = new PlugInManager();
            foreach (XmlNode serviceableDS in compMan.GetPluginNodes(Plugins.Type.IServiceableDataset))
            {
                IServiceableDataset ds = compMan.CreateInstance(serviceableDS) as IServiceableDataset;
                if (ds == null) return;

                if (importItem.DropDownItems.Count == 1)
                    importItem.DropDownItems.Add(new ToolStripSeparator());

                ToolStripItem item = new ServiceableDatasetItem(this, ds);
                item.Image = (new MapServiceIcon2()).Image;
                importItem.DropDownItems.Add(item);
            }

            _contextItems = contextItems.ToArray();
        }

        void newService_Click(object sender, EventArgs e)
        {
            List<ExplorerDialogFilter> filters = new List<ExplorerDialogFilter>();
            filters.Add(new OpenMapFilter());

            ExplorerDialog exDialog = new ExplorerDialog("Select Map", filters, true);

            if (exDialog.ShowDialog() == DialogResult.OK)
            {
                List<IMap> maps = new List<IMap>();
                foreach (IExplorerObject exObject in exDialog.ExplorerObjects)
                {
                    if (exObject.Object is IMap)
                    {
                        maps.Add((IMap)exObject.Object);
                    }
                }

                FormAddServices services = new FormAddServices(maps);
                if (services.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AddMap2Connection(services.ServiceNames, services.Maps);
                }

                if (_refreshDelegate != null) _refreshDelegate();
            }
        }

        void newServiceColl_Click(object sender, EventArgs e)
        {
            FormAddServiceCollection dlg = new FormAddServiceCollection(_connectionString);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Services != null)
            {
                AddServiceCollection2Connection(dlg.CollectionName, dlg.Services);
            }
        }

        internal MapServerConnectionExplorerObject(IExplorerObject parent, string name, string connectionString)
            : this(parent)
        {
            _name = name;
            _connectionString = connectionString;
        }

        #region IExplorerObject Member

        public string Name
        {
            get { return _name; }
        }

        public string FullName
        {
            get { return @"gView.MapServer\" + _name; }
        }

        public string Type
        {
            get { return "gView.MapServer Connection"; }
        }

        public IExplorerIcon Icon
        {
            get { return _icon; }
        }

        public new object Object
        {
            get { return null; }
        }

        public IExplorerObject CreateInstanceByFullName(string FullName)
        {
            return null;
        }
        #endregion

        #region IExplorerParentObject Member

        public override void Refresh()
        {
            base.Refresh();

            try
            {
                MapServerConnection service = new MapServerConnection(ConfigTextStream.ExtractValue(_connectionString, "server"));
                //string axl = service.ServiceRequest("catalog", "<GETCLIENTSERVICES/>", "BB294D9C-A184-4129-9555-398AA70284BC",
                //    ConfigTextStream.ExtractValue(_connectionString, "user"),
                //    Identity.HashPassword(ConfigTextStream.ExtractValue(_connectionString, "pwd")));
                string axl = WebFunctions.HttpSendRequest("http://" + ConfigTextStream.ExtractValue(_connectionString, "server") + "/catalog?format=xml", "GET", null,
                      ConfigTextStream.ExtractValue(_connectionString, "user"),
                      ConfigTextStream.ExtractValue(_connectionString, "pwd"));

                if (String.IsNullOrEmpty(axl)) return;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(axl);
                foreach (XmlNode mapService in doc.SelectNodes("//SERVICE[@name]"))
                {
                    MapServiceType type = MapServiceType.MXL;
                    if (mapService.Attributes["servicetype"] != null)
                    {
                        switch (mapService.Attributes["servicetype"].Value.ToLower())
                        {
                            case "mxl":
                                type = MapServiceType.MXL;
                                break;
                            case "svc":
                                type = MapServiceType.SVC;
                                break;
                            case "gdi":
                                type = MapServiceType.GDI;
                                break;
                        }
                    }

                    base.AddChildObject(new MapServerServiceExplorerObject(this, mapService.Attributes["name"].Value, _connectionString, type));
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region IExplorerObjectContentDragDropEvents Member

        public void Content_DragDrop(System.Windows.Forms.DragEventArgs e)
        {
            foreach (string format in e.Data.GetFormats())
            {
                object ob = e.Data.GetData(format);
                if (ob is List<IExplorerObjectSerialization>)
                {
                    ExplorerObjectManager exObjectManager = new ExplorerObjectManager();

                    List<IExplorerObject> exObjects = exObjectManager.DeserializeExplorerObject((List<IExplorerObjectSerialization>)ob);
                    if (exObjects == null) return;

                    foreach (IExplorerObject exObject in exObjects)
                    {
                        if (exObject is IExplorerParentObject)
                        {
                            ((IExplorerParentObject)exObject).Refresh();
                        }
                        if (exObject.Object is IFeatureDataset)
                        {
                            IDataset dataset = (IDataset)exObject.Object;

                            FormDatasetProperties dlg = new FormDatasetProperties(dataset);
                            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Map map = new Map();
                                map.AddDataset(dataset, 0);
                                map.Name = dataset.DatasetName;

                                map.Limit = map.Envelope = ((IFeatureDataset)dataset).Envelope;
                                FormAddServices services = new FormAddServices(map);
                                if (services.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    AddMap2Connection(services.ServiceNames, services.Maps);
                                }
                            }
                        }
                        else if (exObject.Object is IMap)
                        {
                            FormAddServices services = new FormAddServices((IMap)exObject.Object);
                            if (services.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                AddMap2Connection(services.ServiceNames, services.Maps);
                            }
                        }
                        else if (exObject.Object is IMapDocument)
                        {
                            FormAddServices services = new FormAddServices(((IMapDocument)exObject.Object).Maps);
                            if (services.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                AddMap2Connection(services.ServiceNames, services.Maps);
                            }
                        }
                    }
                }
            }
        }

        public void Content_DragEnter(System.Windows.Forms.DragEventArgs e)
        {
            foreach (string format in e.Data.GetFormats())
            {
                object ob = e.Data.GetData(format);
                if (ob is List<IExplorerObjectSerialization>)
                {
                    foreach (IExplorerObjectSerialization ser in (List<IExplorerObjectSerialization>)ob)
                    {
                        if (ser.ObjectTypes.Contains(typeof(IFeatureDataset)) ||
                            ser.ObjectTypes.Contains(typeof(IMap)) ||
                            ser.ObjectTypes.Contains(typeof(IMapDocument)))
                        {
                            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
                            return;
                        }
                    }
                }
            }
        }

        public void Content_DragLeave(EventArgs e)
        {

        }

        public void Content_DragOver(System.Windows.Forms.DragEventArgs e)
        {

        }

        public void Content_GiveFeedback(System.Windows.Forms.GiveFeedbackEventArgs e)
        {

        }

        public void Content_QueryContinueDrag(System.Windows.Forms.QueryContinueDragEventArgs e)
        {

        }

        #endregion

        private void AddMap2Connection(List<string> serviceNames, List<IMap> maps)
        {
            try
            {
                if (serviceNames.Count != maps.Count) return;

                for (int i = 0; i < serviceNames.Count; i++)
                {
                    XmlStream stream = new XmlStream("MapDocument");
                    stream.Save("IMap", maps[i]);
                    stream.ReduceDocument("//IMap");

                    StringBuilder sb = new StringBuilder();
                    StringWriter sw = new StringWriter(sb);
                    stream.WriteStream(sw);

                    //XmlDocument doc = new XmlDocument();
                    //doc.LoadXml(sb.ToString());

                    MapServerConnection service = new MapServerConnection(ConfigTextStream.ExtractValue(_connectionString, "server"));
                    if (!service.AddMap(serviceNames[i], sb.ToString(),
                        ConfigTextStream.ExtractValue(_connectionString, "user"),
                        ConfigTextStream.ExtractValue(_connectionString, "pwd")))
                    {
                        System.Windows.Forms.MessageBox.Show("Can't add map", "ERROR");
                    }
                }

                Refresh();
                if (Refreshed != null) Refreshed(this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void AddService2Connection(XmlStream stream)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                stream.WriteStream(sw);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sb.ToString());

                FormAddServiceableDataset dlg = new FormAddServiceableDataset();
                if (dlg.ShowDialog() != DialogResult.OK) return;

                MapServerConnection service = new MapServerConnection(ConfigTextStream.ExtractValue(_connectionString, "server"));
                if (!service.AddMap(dlg.ServiceName, doc.SelectSingleNode("//IServiceableDataset").OuterXml,
                    ConfigTextStream.ExtractValue(_connectionString, "user"),
                        ConfigTextStream.ExtractValue(_connectionString, "pwd")))
                {
                    System.Windows.Forms.MessageBox.Show("Can't add map", "ERROR");
                }
                //Refresh();
                Refresh();
                if (Refreshed != null) Refreshed(this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void AddServiceCollection2Connection(string collectionName, string[] services)
        {
            XmlStream stream = new XmlStream("ServiceCollection");
            stream.Save("Services", new XmlStreamStringArray(services));

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            stream.WriteStream(sw);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            MapServerConnection service = new MapServerConnection(ConfigTextStream.ExtractValue(_connectionString, "server"));
            if (!service.AddMap(collectionName, doc.SelectSingleNode("//ServiceCollection").OuterXml,
                        ConfigTextStream.ExtractValue(_connectionString, "user"),
                        ConfigTextStream.ExtractValue(_connectionString, "pwd")))
            {
                System.Windows.Forms.MessageBox.Show("Can't add map", "ERROR");
            }
        }

        #region ISerializableExplorerObject Member

        public IExplorerObject CreateInstanceByFullName(string FullName, ISerializableExplorerObjectCache cache)
        {
            if (cache.Contains(FullName)) return cache[FullName];

            MapServerExplorerObjects obj = new MapServerExplorerObjects();
            if (obj.ChildObjects == null) return null;

            foreach (IExplorerObject exObject in obj.ChildObjects)
            {
                if (exObject.FullName == FullName)
                {
                    cache.Append(exObject);
                    return exObject;
                }
            }
            return null;
        }

        #endregion

        #region IExplorerObjectDeletable Member

        public event ExplorerObjectDeletedEvent ExplorerObjectDeleted = null;

        public bool DeleteExplorerObject(ExplorerObjectEventArgs e)
        {
            ConfigTextStream stream = new ConfigTextStream("gviewmapserver_connections", true, true);
            stream.Remove(this.Name, _connectionString);
            stream.Close();
            if (ExplorerObjectDeleted != null) ExplorerObjectDeleted(this);
            return true;
        }

        #endregion

        #region IExplorerObjectRenamable Member

        public event ExplorerObjectRenamedEvent ExplorerObjectRenamed = null;

        public bool RenameExplorerObject(string newName)
        {
            bool ret = false;
            ConfigTextStream stream = new ConfigTextStream("gviewmapserver_connections", true, true);
            ret = stream.ReplaceHoleLine(ConfigTextStream.BuildLine(_name, _connectionString), ConfigTextStream.BuildLine(newName, _connectionString));
            stream.Close();

            if (ret == true)
            {
                _name = newName;
                if (ExplorerObjectRenamed != null) ExplorerObjectRenamed(this);
            }
            return ret;
        }

        #endregion

        #region IExplorerObjectContextMenu2 Member
        private RefreshContextDelegate _refreshDelegate = null;
        ToolStripItem[] IExplorerObjectContextMenu2.ContextMenuItems(RefreshContextDelegate callback)
        {
            _refreshDelegate = callback;
            return _contextItems;
        }

        #endregion

        private class ServiceableDatasetItem : ToolStripMenuItem
        {
            private IServiceableDataset _sds;
            private MapServerConnectionExplorerObject _exObject;

            public ServiceableDatasetItem(MapServerConnectionExplorerObject exObject, IServiceableDataset sds)
            {
                _exObject = exObject;
                _sds = sds;

                if (sds == null) return;
                base.Text = sds.Name;
                base.Click += new EventHandler(ServiceableDatasetItem_Click);
            }

            void ServiceableDatasetItem_Click(object sender, EventArgs e)
            {
                if (_sds == null) return;

                if (_sds.GenerateNew())
                {
                    XmlStream stream = new XmlStream("Service");
                    stream.Save("IServiceableDataset", _sds);

                    _exObject.AddService2Connection(stream);

                    if (_exObject != null && _exObject._refreshDelegate != null)
                    {
                        _exObject._refreshDelegate();
                    }
                    //MessageBox.Show("All right!!!");
                }
            }
        }

        #region IRefreshedEventHandler Member

        public event RefreshedEventHandler Refreshed = null;

        #endregion
    }

    [gView.Framework.system.RegisterPlugIn("14CA7600-550E-4102-A7E4-651BA2F09B20")]
    internal class MapServerServiceExplorerObject : ExplorerObjectCls, IExplorerSimpleObject, IExplorerObjectDeletable, IMetadata, IExplorerObjectContextMenu
    {
        private IExplorerIcon _icon;
        private string _name = "", _connectionString = "";
        private MapServerClass _class = null;
        private MapServerConnectionExplorerObject _parent = null;
        private MapServiceType _type;
        private ToolStripItem[] _menuitems;
        public MapServerServiceExplorerObject()
            : base(null, typeof(MapServerClass))
        {
            _menuitems = new ToolStripItem[]{
                   new ToolStripMenuItem("Pre-Render Tiles...",null,this.MapServerServiceExplorerObject_PreRenderTiles)
            };
        }

        internal MapServerServiceExplorerObject(MapServerConnectionExplorerObject parent, string name, string connectionString, MapServiceType type)
            : base(parent, typeof(MapServerClass))
        {
            _menuitems = new ToolStripItem[]{
                   new ToolStripMenuItem("Pre-Render Tiles...",null,this.MapServerServiceExplorerObject_PreRenderTiles)
            };

            switch (_type = type)
            {
                case MapServiceType.MXL:
                    _icon = new MapServiceIcon();
                    break;
                case MapServiceType.SVC:
                    _icon = new MapServiceIcon2();
                    break;
                case MapServiceType.GDI:
                    _icon = new MapServiceIcon3();
                    break;
            }

            _name = name;
            _connectionString = connectionString;
            _parent = parent;
        }

        #region IExplorerObject Member

        public string Name
        {
            get { return _name; }
        }

        public string FullName
        {
            get
            {
                if (_parent == null) return "";
                return _parent.FullName + @"\" + _name;
            }
        }

        public string Type
        {
            get
            {
                switch (_type)
                {
                    case MapServiceType.MXL:
                        return "gView.MapServer Map Service";
                    case MapServiceType.SVC:
                        return "gView.MapServer Service Connection";
                    case MapServiceType.GDI:
                        return "gView.MapServer GDI Service";
                }
                return String.Empty;
            }
        }

        public IExplorerIcon Icon
        {
            get { return _icon; }
        }

        public void Dispose()
        {

        }

        public new object Object
        {
            get
            {
                if (_class == null)
                {
                    MapServerDataset dataset = new MapServerDataset(_connectionString, _name);
                    // dataset.Open();  // kein open, weil sonst GET_SERVICE_INFO gemacht wird...
                    if (dataset.Elements.Count == 0)
                    {
                        dataset.Dispose();
                        return null;
                    }

                    _class = dataset.Elements[0].Class as MapServerClass;
                }
                return _class;
            }
        }

        public IExplorerObject CreateInstanceByFullName(string FullName)
        {
            return null;
        }

        #endregion

        #region ISerializableExplorerObject Member

        public IExplorerObject CreateInstanceByFullName(string FullName, ISerializableExplorerObjectCache cache)
        {
            if (cache.Contains(FullName)) return cache[FullName];

            FullName = FullName.Replace("/", @"\");
            int lastIndex = FullName.LastIndexOf(@"\");
            if (lastIndex == -1) return null;

            string cnName = FullName.Substring(0, lastIndex);
            string svName = FullName.Substring(lastIndex + 1, FullName.Length - lastIndex - 1);

            MapServerConnectionExplorerObject pObj = new MapServerConnectionExplorerObject(null);
            pObj = pObj.CreateInstanceByFullName(cnName) as MapServerConnectionExplorerObject;
            if (pObj == null || pObj.ChildObjects == null) return null;

            foreach (IExplorerObject exObject in pObj.ChildObjects)
            {
                if (exObject.Name == svName)
                {
                    cache.Append(exObject);
                    return exObject;
                }
            }
            return null;
        }

        #endregion

        #region IExplorerObjectDeletable Member

        public event ExplorerObjectDeletedEvent ExplorerObjectDeleted = null;

        public bool DeleteExplorerObject(ExplorerObjectEventArgs e)
        {
            try
            {
                MapServerConnection service = new MapServerConnection(ConfigTextStream.ExtractValue(_connectionString, "server"));
                if (!service.RemoveMap(_name,
                         ConfigTextStream.ExtractValue(_connectionString, "user"),
                         ConfigTextStream.ExtractValue(_connectionString, "pwd")))
                {
                    System.Windows.Forms.MessageBox.Show("Can't remove map...", "ERROR");
                    return false;
                }
                _parent.Refresh();
                if (ExplorerObjectDeleted != null) ExplorerObjectDeleted(this);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ERROR");
                return false;
            }
        }

        #endregion

        #region IMetadata Member

        public void ReadMetadata(IPersistStream stream)
        {
            if (!(stream is XmlStream)) return;

            string server = ConfigTextStream.ExtractValue(_connectionString, "server");
            string user = ConfigTextStream.ExtractValue(_connectionString, "user");
            string pwd = ConfigTextStream.ExtractValue(_connectionString, "pwd");

            MapServerConnection conn = new MapServerConnection("http://" + server);
            string meta = conn.QueryMetadata(_name, user, pwd);
            if (meta == String.Empty) return;

            StringReader sr = new StringReader(meta);
            ((XmlStream)stream).ReadStream(sr);
        }

        public void WriteMetadata(IPersistStream stream)
        {
            if (!(stream is XmlStream)) return;

            string server = ConfigTextStream.ExtractValue(_connectionString, "server");
            string user = ConfigTextStream.ExtractValue(_connectionString, "user");
            string pwd = ConfigTextStream.ExtractValue(_connectionString, "pwd");

            MapServerConnection conn = new MapServerConnection("http://" + server);

            //StringBuilder sb = new StringBuilder();
            //StringWriter sw = new StringWriter(sb);
            MemoryStream ms = new MemoryStream();
            ((XmlStream)stream).WriteStream(ms);

            string metadata = Encoding.Unicode.GetString(ms.GetBuffer()).Trim(new char[] { ' ', '\0' });
            while (metadata[0] != '<' && metadata.Length > 0)
            {
                metadata = metadata.Substring(1, metadata.Length - 1);
            }

            if (!conn.UploadMetadata(_name, metadata, user, pwd))
            {
                MessageBox.Show("Error...");
            }
        }

        public IMetadataProvider MetadataProvider(Guid guid)
        {
            return null;
        }

        public List<IMetadataProvider> Providers
        {
            get { return new List<IMetadataProvider>(); }
        }

        #endregion

        #region IExplorerObjectContextMenu Member

        public ToolStripItem[] ContextMenuItems
        {
            get { return _menuitems; }
        }

        #endregion

        private void MapServerServiceExplorerObject_PreRenderTiles(object sender, EventArgs e)
        {
            XmlStream stream = new XmlStream("metadata");
            this.ReadMetadata(stream);
            gView.Framework.Data.Metadata metadata = new gView.Framework.Data.Metadata();
            metadata.ReadMetadata(stream);

            gView.Framework.Metadata.TileServiceMetadata tileServiceMetadata = metadata.MetadataProvider(new Guid("D33D3DD2-DD63-4a47-9F84-F840FE0D01C0")) as gView.Framework.Metadata.TileServiceMetadata;
            if (_class == null || tileServiceMetadata == null ||
                tileServiceMetadata.Use == false || tileServiceMetadata.UpperLeftCacheTiles == false)
            {
                MessageBox.Show("Service do not provide tiling properties.\nUse the Service-Metadata to define tiling properties!");
                return;
            }

            FormPreRenderTiles dlg = new FormPreRenderTiles(tileServiceMetadata, _class);
            dlg.ShowDialog();
        }
    }

    internal class MapServiceIcon : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("8B552F56-4EB5-4b70-BB64-38DEB524FB06"); }
        }

        public System.Drawing.Image Image
        {
            get { return gView.MapServer.Lib.UI.Properties.Resources.i_connection; }
        }

        #endregion
    }
    internal class MapServiceIcon2 : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("523A596C-701F-4858-8A2C-B77A44B653B3"); }
        }

        public System.Drawing.Image Image
        {
            get { return gView.MapServer.Lib.UI.Properties.Resources.i_connection_server; }
        }

        #endregion
    }
    internal class MapServiceIcon3 : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("3DE0D698-0716-4593-A9E8-4D1F520F393D"); }
        }

        public System.Drawing.Image Image
        {
            get { return gView.MapServer.Lib.UI.Properties.Resources.i_connection_collection; }
        }

        #endregion
    }

    internal class MapServerIcon : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("2763F3A3-AECE-42cc-A788-CD3020E79434"); }
        }

        public System.Drawing.Image Image
        {
            get { return gView.MapServer.Lib.UI.Properties.Resources.computer; }
        }

        #endregion
    }
    internal class MapServerNewConnectionIcon : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("0EA16B95-3F98-4b7c-A8D6-2807DBF5F9DF"); }
        }

        public System.Drawing.Image Image
        {
            get { return (new Icons()).imageList1.Images[1]; }
        }

        #endregion
    }
    internal class MapServerConnectionIcon : IExplorerIcon
    {
        #region IExplorerIcon Member

        public Guid GUID
        {
            get { return new Guid("C553594F-4A9A-45f4-99A6-2003F06F9E48"); }
        }

        public System.Drawing.Image Image
        {
            get { return gView.MapServer.Lib.UI.Properties.Resources.computer_go; }
        }

        #endregion
    }
}
