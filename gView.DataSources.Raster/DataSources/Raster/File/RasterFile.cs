using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using gView.Framework.Geometry;
using gView.Framework.Data;
using gView.Framework.IO;
using gView.Framework.Db;
using gView.Framework.system;
using gView.Framework.Carto;
using gView.MapServer;
using System.Runtime.InteropServices;

namespace gView.DataSources.Raster.File
{
    [gView.Framework.system.RegisterPlugIn("D4812641-3F53-48eb-A66C-FC0203980C79")]
    public class RasterFileDataset : DatasetMetadata, gView.Framework.Data.IRasterFileDataset, gView.Framework.IO.IPersistable
    {
        private List<IDatasetElement> _layers = new List<IDatasetElement>();
        private string _directory = "";
        private DatasetState _state = DatasetState.opened;

        public RasterFileDataset() { }

        #region IDataset Members

        public void Dispose()
        {

        }

        public string ConnectionString
        {
            get
            {
                return _directory;
            }
            set
            {
                _directory = value;
            }
        }

        public string DatasetGroupName
        {
            get { return "Raster"; }
        }

        public string DatasetName
        {
            get { return "Raster File"; }
        }

        public string ProviderName
        {
            get { return "Raster File"; }
        }

        public DatasetState State
        {
            get { return _state; }
        }

        public bool Open()
        {
            return false;
        }

        public string lastErrorMsg
        {
            get { return ""; }
        }

        public int order
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public gView.Framework.Data.IDatasetEnum DatasetEnum
        {
            get { return null; }
        }

        public List<IDatasetElement> Elements
        {
            get
            {
                List<IDatasetElement> ret = new List<IDatasetElement>();
                foreach (gView.Framework.Data.IRasterLayer layer in _layers)
                {
                    ret.Add(layer);
                }
                return ret;
            }
        }

        public string Query_FieldPrefix
        {
            get { return ""; }
        }

        public string Query_FieldPostfix
        {
            get { return ""; }
        }

        public gView.Framework.FDB.IDatabase Database
        {
            get { return null; }
        }

        public IDatasetElement this[string title]
        {
            get
            {
                foreach (IDatasetElement element in _layers)
                {
                    if (element == null) continue;
                    if (element.Title == title) return element;
                }

                try
                {
                    if (_directory != "")
                    {
                        FileInfo fi = new FileInfo(_directory + @"\" + title);
                        if (fi.Exists)
                        {
                            return AddRasterFile(fi.FullName);
                        }
                    }
                }
                catch { }
                return null;
            }
        }

        public void RefreshClasses()
        {
        }
        #endregion

        #region IRasterFileDataset Member

        public IRasterLayer AddRasterFile(string filename)
        {
            return AddRasterFile(filename, null);
        }
        public IRasterLayer AddRasterFile(string filename, IPolygon polygon)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (_directory == "")
                {
                    _directory = fi.Directory.FullName;
                }
                else if (_directory.ToLower() != fi.Directory.FullName.ToLower())
                {
                    return null;
                }
                if (filename.IndexOf("jpg.mdb") != -1 ||
                    filename.IndexOf("png.mdb") != -1 ||
                    filename.IndexOf("tif.mdb") != -1)
                {
                    PyramidFileClass rasterClass = (polygon == null) ? new PyramidFileClass(this, filename) : new PyramidFileClass(this, filename, polygon);
                    RasterLayer layer = new RasterLayer(rasterClass);
                    if (rasterClass.isValid)
                    {
                        _layers.Add(layer);
                    }
                    return layer;
                }
                else if (fi.Extension.ToLower() == ".pyc")
                {
                    PyramidFile rasterClass = new PyramidFile(this, fi.FullName);
                    RasterLayer layer = new RasterLayer(rasterClass);
                    if (rasterClass.isValid)
                    {
                        _layers.Add(layer);
                    }
                    return layer;
                }
                else if (fi.Extension.ToLower() == ".mdb")
                {
                    //ImageCatalogLayer layer = new ImageCatalogLayer(this, filename);
                    //if (layer.isValid) _layers.Add(layer);
                    //return layer;
                    return null;
                }
                else if (fi.Extension.ToLower() == ".sid" || fi.Extension.ToLower() == ".jp2")
                {
                    MrSidFileClass rasterClass = (polygon == null) ? new MrSidFileClass(this, filename) : new MrSidFileClass(this, filename, polygon);
                    RasterLayer layer = new RasterLayer(rasterClass);
                    if (rasterClass.isValid)
                    {
                        _layers.Add(layer);
                    }
                    return layer;
                }
                else
                {
                    RasterFileClass rasterClass = (polygon == null) ? new RasterFileClass(this, filename) : new RasterFileClass(this, filename, polygon);
                    RasterLayer layer = new RasterLayer(rasterClass);
                    if (rasterClass.isValid)
                    {
                        _layers.Add(layer);
                    }
                    return layer;
                }
            }
            catch { }
            return null;
        }

        public string SupportedFileFilter
        {
            get
            {
                if (PlugInManager.Create(new Guid("43DFABF1-3D19-438c-84DA-F8BA0B266592")) is IRasterFileDataset)
                {
                    // GDAL is installed!!
                    return "*.sid|*.jp2";
                }
                return "*.sid|*.jp2|*.tif|*.tiff|*.png|*.jpg|*.jpeg";
            }
        }

        public int SupportsFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case ".sid":
                case ".jp2":
                case ".tif":
                case ".tiff":
                case ".png":
                case ".jpg":
                case ".jpeg":
                    return 100;
            }
            return -1;
        }

        #endregion

        #region IRasterDataset Members

        public IEnvelope Envelope
        {
            get
            {
                Envelope env = null;

                foreach (gView.Framework.Data.IRasterLayer layer in _layers)
                {
                    if (layer.RasterClass==null || layer.RasterClass.Polygon == null) continue;

                    if (env == null)
                        env = new Envelope(layer.RasterClass.Polygon.Envelope);
                    else
                        env.Union(layer.RasterClass.Polygon.Envelope);
                }
                return env;
            }
        }

        public ISpatialReference SpatialReference
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        #endregion

        #region IPersistable Members

        public string PersistID
        {
            get { return ""; }
        }

        public void Load(gView.Framework.IO.IPersistStream stream)
        {
            _directory=(string)stream.Load("Directory");
            if (_directory == null) _directory = "";
        }

        public void Save(gView.Framework.IO.IPersistStream stream)
        {
            stream.Save("Directory", _directory);
        }

        #endregion
    }

    public class RasterFileClass : IRasterClass,IBitmap,IRasterFile
    {
        private string _filename,_title;
        private bool _valid = true;
        private int _iWidth = 0, _iHeight = 0;
        internal TFWFile _tfw;
        private IPolygon _polygon;
        private ISpatialReference _sRef = null;
        private IRasterDataset _dataset;

        public RasterFileClass()
        {
        }

        public RasterFileClass(IRasterDataset dataset, string filename)
            : this(dataset, filename, null)
        {
        }
        public RasterFileClass(IRasterDataset dataset,string filename, IPolygon polygon)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                _title = fi.Name;
                _filename = filename;
                _dataset = dataset;

                string tfwfilename = filename.Substring(0, filename.Length - fi.Extension.Length);
                if (fi.Extension.ToLower() == ".jpg")
                    tfwfilename += ".jgw";
                else
                    tfwfilename += ".tfw";

                _tfw = new TFWFile(tfwfilename);
                //if (!_tfw.isValid)
                //{
                //    _valid = false;
                //    return;
                //}

                FileInfo fiPrj = new FileInfo(fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".prj");
                if (fiPrj.Exists)
                {
                    StreamReader sr = new StreamReader(fiPrj.FullName);
                    string wkt = sr.ReadToEnd();
                    sr.Close();

                    _sRef = gView.Framework.Geometry.SpatialReference.FromWKT(wkt);
                }

                if (polygon != null)
                {
                    _polygon = polygon;
                }
                else
                {
                    calcPolygon();
                }
            }
            catch { _valid = false; }
        }

        public bool isValid { get { return _valid; } }

        private void calcPolygon()
        {
            try
            {
                if (_tfw == null)
                {
                    FileInfo fi = new FileInfo(_filename);

                    string tfwfilename = _filename.Substring(0, _filename.Length - fi.Extension.Length);
                    if (fi.Extension.ToLower() == ".jpg")
                        tfwfilename += ".jgw";
                    else
                        tfwfilename += ".tfw";

                    _tfw = new TFWFile(tfwfilename);
                }
                Bitmap image = null;
                if (_iWidth == 0 || _iHeight == 0)
                {
                    image = (Bitmap)ImageFast.FromFile(_filename);
                }
                setBounds(image);
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
            }
            catch
            {
                _polygon = null;
            }
        }
        private void setBounds(Image image)
        {
            if (image!=null)
            {
                _iWidth = image.Width;
                _iHeight = image.Height;
            }
            _polygon = new Polygon();
            Ring ring = new Ring();
            gView.Framework.Geometry.Point p1 = new gView.Framework.Geometry.Point(
                _tfw.X - _tfw.dx_X / 2.0 - _tfw.dy_X / 2.0,
                _tfw.Y - _tfw.dx_Y / 2.0 - _tfw.dy_Y / 2.0);

            ring.AddPoint(p1);
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + _tfw.dx_X * _iWidth, p1.Y + _tfw.dx_Y * _iWidth));
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + _tfw.dx_X * _iWidth + _tfw.dy_X * _iHeight, p1.Y + _tfw.dx_Y * _iWidth + _tfw.dy_Y * _iHeight));
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + _tfw.dy_X * _iHeight, p1.Y + _tfw.dy_Y * _iHeight));
            _polygon.AddRing(ring);
        }

        #region IRasterClass
        public IPolygon Polygon
        {
            get { return _polygon; }
        }

        Bitmap _bm;
        BitmapData _bmData;

        public void BeginPaint(gView.Framework.Carto.IDisplay display, ICancelTracker cancelTracker)
        {
            if (_bm != null)
            {
                _bm.Dispose();
                _bm = null;
            }
            
            _bm = (Bitmap)ImageFast.FromFile(_filename);
           
            /*
            _bmData = _bm.LockBits(new Rectangle(0, 0, _bm.Width, _bm.Height),
                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            */
        }
        public void EndPaint(ICancelTracker cancelTracker)
        {
            //return;
            if (_bm != null)
            {
                if (_bmData != null)
                {
                    /*
                    _bm.UnlockBits(_bmData);
                    _bmData = null;
                     */
                }
                _bm.Dispose();
                _bm = null;
            }
        }
        public Bitmap Bitmap
        {
            get { return _bm; }
        }

        public Color GetPixel(double X, double Y)
        {
            //return Color.Beige;
            if (_bm == null) return Color.Transparent;
            int x, y;
            _tfw.World2Image(X, Y, out x, out y);
            if (x < 0 || y < 0 || x >= _iWidth || y >= _iHeight) return Color.Transparent;

            if (_bmData != null)
            {
                unsafe
                {
                    int* p = (int*)(void*)_bmData.Scan0;
                    p += y * _iWidth + x;
                    return Color.FromArgb(*p);
                }
            }
            else
            {
                return _bm.GetPixel(x, y);
            }
        }

        public double oX { get { return _tfw.X; } }
        public double oY { get { return _tfw.Y; } }
        public double dx1 { get { return _tfw.dx_X; } }
        public double dx2 { get { return _tfw.dx_Y; } }
        public double dy1 { get { return _tfw.dy_X; } }
        public double dy2 { get { return _tfw.dy_Y; } }

        public ISpatialReference SpatialReference
        {
            get { return _sRef; }
            set { _sRef = value; }
        }

        public IRasterDataset Dataset
        {
            get { return _dataset; }
        }

        #endregion

        #region IBitmap Members

        public Bitmap LoadBitmap()
        {
            try
            {
                if (_filename == "" || _filename == null) return null;
                return (System.Drawing.Bitmap)Bitmap.FromFile(_filename);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region IRasterFile Members

        public string Filename
        {
            get { return _filename; }
        }

        public IRasterWorldFile WorldFile
        {
            get { return _tfw; }
        }

        #endregion

        #region IClass Member

        public string Name
        {
            get { return _title; }
        }

        public string Aliasname
        {
            get { return _title; }
        }

        IDataset IClass.Dataset
        {
            get { return _dataset; }
        }

        #endregion
    }

    internal class PyramidImageClass : IRasterClass,IRasterFile
    {
        private string _filename;
        private int _ID;
        private IPolygon _polygon;
        private ISpatialReference _sRef=null;
        private IRasterDataset _dataset;

        public IRasterClass RasterClass { get { return null; } }

        public PyramidImageClass(IRasterDataset dataset, string filename, int ID, IPolygon polygon)
        {
            _filename = filename;
            _ID = ID;
            _polygon = polygon;
            _dataset = dataset;
        }

        private double _X, _Y;
        private double _dx_X, _dx_Y, _dy_X, _dy_Y;
        private double _detA;
        int _iWidth, _iHeight;

        private void World2Image(double X, double Y, out int x, out int y)
        {
            if (_dx_Y == 0.0 && _dy_X == 0.0)
            {
                x = (int)(((X - _X) / _dx_X)+0.5);  // 0.5 f�rs runden
                y = (int)(((Y - _Y) / _dy_Y)+0.5);
            }
            else
            {
                double dX = X - _X;
                double dY = Y - _Y;

                // | dX   dx_y |
                // | dY   dy_y |
                double detX = dX * _dy_Y - _dx_Y * dY;
                // | dx_x  dX |
                // | dy_x  dY |
                double detY = _dx_X * dY - dX * _dy_X;
                x = (int)((detX / _detA)+0.5);
                y = (int)((detY / _detA)+0.5);
            }
        }

        #region IRasterClass Members

        public IPolygon Polygon
        {
            get { return _polygon; }
        }

        Bitmap _bm = null;
        BitmapData _bmData = null;
        public void BeginPaint(gView.Framework.Carto.IDisplay display, ICancelTracker cancelTracker)
        {
            if (_bm != null || _bmData != null)
            {
                EndPaint(cancelTracker);
            }
            CommonDbConnection conn = new CommonDbConnection();
            conn.ConnectionString2 = _filename;

            DataTable tab = conn.Select("*", "PYRAMID_IMG", "ID="+_ID);
            if (tab == null) return;
            if (tab.Rows.Count == 0) return;

            byte[] obj = (byte[])tab.Rows[0]["IMG"];
            //BinaryReader r = new BinaryReader(new MemoryStream());
            //r.BaseStream.Write((byte[])obj, 0, ((byte[])obj).Length);
            //r.BaseStream.Position = 0;
            //_bm = (Bitmap)Image.FromStream(r.BaseStream);
            _bm = (Bitmap)ImageFast.FromStream(obj);
            //r.Close();

            /*
            _bmData = _bm.LockBits(new Rectangle(0, 0, _bm.Width, _bm.Height),
                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            */

            _X = (double)tab.Rows[0]["X"];
            _Y = (double)tab.Rows[0]["Y"];
            _dx_X = (double)tab.Rows[0]["dx1"];
            _dx_Y = (double)tab.Rows[0]["dx2"];
            _dy_X = (double)tab.Rows[0]["dy1"];
            _dy_Y = (double)tab.Rows[0]["dy2"];
            //_iWidth = (int)tab.Rows[0]["iWidth"];
            //_iHeight = (int)tab.Rows[0]["iHeight"];
            _iWidth = _bm.Width;
            _iHeight = _bm.Height;


            // | dx_x   dx_y |
            // | dy_x   dy_y |
            _detA = _dx_X * _dy_Y - _dx_Y * _dy_X;  
        }

        public void EndPaint(ICancelTracker cancelTracker)
        {
            if (_bmData != null && _bm != null)
            {
                _bm.UnlockBits(_bmData);
                _bmData = null;
            }
            if (_bm != null)
            {
                _bm.Dispose();
                _bm = null;
            }
        }

        public Color GetPixel(double X, double Y)
        {
            if (_bm == null) return Color.Transparent;
            int x, y;
            this.World2Image(X, Y, out x, out y);
            
            if (x < 0 || y < 0 || x >= _iWidth || y >= _iHeight) return Color.Transparent;

            if (_bmData != null)
            {
                unsafe
                {
                    int* p = (int*)(void*)_bmData.Scan0;
                    p += y * _iWidth + x;
                    return Color.FromArgb(*p);
                }
            }
            else
            {
                return _bm.GetPixel(x, y);
            }
        }

        public Bitmap Bitmap
        {
            get { return _bm; }
        }

        public double oX { get { return _X; } }
        public double oY { get { return _Y; } }
        public double dx1 { get { return _dx_X; } }
        public double dx2 { get { return _dx_Y; } }
        public double dy1 { get { return _dy_X; } }
        public double dy2 { get { return _dy_Y; } }

        public ISpatialReference SpatialReference
        {
            get { return _sRef; }
            set { _sRef = value; }
        }

        private InterpolationMethod _interpolMethod = InterpolationMethod.Fast;
        public InterpolationMethod InterpolationMethod
        {
            get { return _interpolMethod; }
            set { _interpolMethod = value; }
        }

        public IRasterDataset Dataset
        {
            get { return _dataset; }
        }
        #endregion

        #region IRasterFile Members

        public string Filename
        {
            get { return _filename; }
        }

        public IRasterWorldFile WorldFile
        {
            get
            {
                return new TFWFile(oX, oY, dx1, dx2, dy1, dy2);
            }
        }
        #endregion

        #region IClass Member

        public string Name
        {
            get { return "image"; }
        }

        public string Aliasname
        {
            get { return "image"; }
        }

        IDataset IClass.Dataset
        {
            get { return _dataset; }
        }

        #endregion
    }

    public class PyramidFileClass : IRasterClass, gView.Framework.Data.IParentRasterLayer
    {
        private string _title = "";
        private string _filename;
        private bool _valid = true;
        private IPolygon _polygon;
        private ISpatialReference _sRef = null;
        private IRasterDataset _dataset = null;

        public IRasterClass RasterClass { get { return null; } }

        public PyramidFileClass() { }
        public PyramidFileClass(IRasterDataset dataset, string filename) : this(dataset, filename,null)
        {
        }
        public PyramidFileClass(IRasterDataset dataset, string filename, IPolygon polygon)
        {

            FileInfo fi = new FileInfo(filename);
            if (!fi.Exists)
            {
                _valid = false;
                return;
            }
            _title = fi.Name;
            _filename = filename;
            _dataset = dataset;

            if (polygon != null)
            {
                _polygon = polygon;
            }
            else
            {
                init();
            }
        }

        private void init()
        {
            try
            {
                CommonDbConnection conn = new CommonDbConnection();
                conn.ConnectionString2 = _filename;

                DataTable tab = conn.Select("*", "PYRAMID_SHAPE");
                if (tab == null)
                {
                    _valid = false;
                    return;
                }
                DataRow row = tab.Rows[0];
                byte[] obj = (byte[])row["SHAPE"];

                BinaryReader r = new BinaryReader(new MemoryStream());
                r.BaseStream.Write((byte[])obj, 0, ((byte[])obj).Length);
                r.BaseStream.Position = 0;

                _polygon = new Polygon();
                _polygon.Deserialize(r, new GeometryDef(geometryType.Polygon, null, true));
                r.Close();

                conn.Dispose();
            }
            catch
            {
                _valid = false;
            }
        }

        public bool isValid { get { return _valid; } }

        private double _X, _Y;
        private double _dx_X, _dx_Y, _dy_X, _dy_Y;
        private double _detA;
        int _iWidth, _iHeight;

        private void World2Image(double X, double Y, out int x, out int y)
        {
            if (_dx_Y == 0.0 && _dy_X == 0.0)
            {
                x = (int)((X - _X) / _dx_X);
                y = (int)((Y - _Y) / _dy_Y);
            }
            else
            {
                double dX = X - _X;
                double dY = Y - _Y;

                // | dX   dx_y |
                // | dY   dy_y |
                double detX = dX * _dy_Y - _dx_Y * dY;
                // | dx_x  dX |
                // | dy_x  dY |
                double detY = _dx_X * dY - dX * _dy_X;
                x = (int)(detX / _detA);
                y = (int)(detY / _detA);
            }
        }

        #region IRasterLayer Members

        public IPolygon Polygon
        {
            get { return _polygon; }
        }

        Bitmap _bm = null;
        BitmapData _bmData = null;
        public void BeginPaint(gView.Framework.Carto.IDisplay display, ICancelTracker cancelTracker)
        {
            if (_bm != null || _bmData!=null)
            {
                EndPaint(cancelTracker);
            }
            CommonDbConnection conn = new CommonDbConnection();
            conn.ConnectionString2 = _filename;

            DataTable tab = conn.Select("*", "PYRAMID_IMG", "ID=85");
            if (tab == null) return;
            if (tab.Rows.Count == 0) return;

            byte[] obj = (byte[])tab.Rows[0]["IMG"];
            BinaryReader r = new BinaryReader(new MemoryStream());
            r.BaseStream.Write((byte[])obj, 0, ((byte[])obj).Length);
            r.BaseStream.Position = 0;
            _bm = (Bitmap)Image.FromStream(r.BaseStream);
            r.Close();

            /*
            _bmData = _bm.LockBits(new Rectangle(0, 0, _bm.Width, _bm.Height),
                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            */

            _X = (double)tab.Rows[0]["X"];
            _Y = (double)tab.Rows[0]["Y"];
            _dx_X = (double)tab.Rows[0]["dx1"];
            _dx_Y = (double)tab.Rows[0]["dx2"];
            _dy_X = (double)tab.Rows[0]["dy1"];
            _dy_Y = (double)tab.Rows[0]["dy2"];
            _iWidth = (int)tab.Rows[0]["iWidth"];
            _iHeight = (int)tab.Rows[0]["iHeight"];

            // | dx_x   dx_y |
            // | dy_x   dy_y |
            _detA = _dx_X * _dy_Y - _dx_Y * _dy_X;
        }

        public void EndPaint(ICancelTracker cancelTracker)
        {
            if (_bmData != null && _bm!=null)
            {
                /*
                _bm.UnlockBits(_bmData);
                _bmData = null;
                */
            }
            if (_bm != null)
            {
                _bm.Dispose();
                _bm = null;
            }
        }

        public Color GetPixel(double X, double Y)
        {
            if (_bm == null) return Color.Transparent;
            int x, y;
            this.World2Image(X, Y, out x, out y);
            if (x < 0 || y < 0 || x >= _iWidth || y >= _iHeight) return Color.Transparent;

            if (_bmData != null)
            {
                unsafe
                {
                    int* p = (int*)(void*)_bmData.Scan0;
                    p += y * _iWidth + x;
                    return Color.FromArgb(*p);
                }
            }
            else
            {
                return _bm.GetPixel(x, y);
            }
        }

        public Bitmap Bitmap
        {
            get { return _bm; }
        }

        public double oX { get { return _X; } }
        public double oY { get { return _Y; } }
        public double dx1 { get { return _dx_X; } }
        public double dx2 { get { return _dx_Y; } }
        public double dy1 { get { return _dy_X; } }
        public double dy2 { get { return _dy_Y; } }

        public ISpatialReference SpatialReference
        {
            get { return _sRef; }
            set { _sRef = value; }
        }

        private InterpolationMethod _interpolMethod = InterpolationMethod.Fast;
        public InterpolationMethod InterpolationMethod
        {
            get { return _interpolMethod; }
            set { _interpolMethod = value; }
        }

        public IRasterDataset Dataset
        {
            get { return _dataset; }
        }

        #endregion

        #region IParentClass Members

        public IRasterLayerCursor ChildLayers(gView.Framework.Carto.IDisplay display, string filterClause)
        {
            List<IRasterLayer> layers = new List<IRasterLayer>();

            double dpm = Math.Max(display.GraphicsContext.DpiX, display.GraphicsContext.DpiY) / 0.0254;
            double pix = display.mapScale / dpm;/*display.dpm;*/  // [m]

            //System.Windows.Forms.MessageBox.Show(pix.ToString());

            CommonDbConnection conn = new CommonDbConnection();
            conn.ConnectionString2 = _filename;

            int level = 1;
            string sql = "SELECT LEV, Max(cellX) AS MaxCellX, Max(cellY) AS MaxCellY FROM PYRAMID_IMG GROUP BY LEV HAVING (((Max(cellX))<="+pix.ToString().Replace(",",".")+") AND ((Max(cellY))<="+pix.ToString().Replace(",",".")+")) ORDER BY Max(cellX) DESC , Max(cellY) DESC";
            DataSet ds = new DataSet();
            if (conn.SQLQuery(ref ds, sql, "LEVEL"))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    level = (int)ds.Tables[0].Rows[0]["LEV"];
                }
            }
            //System.Windows.Forms.MessageBox.Show(level.ToString());

            DataTable tab = conn.Select("[SHAPE],[ID]", "PYRAMID_IMG", "LEV=" + level);

            IEnvelope dispEnvelope = display.Envelope;
            if (display.GeometricTransformer != null)
            {
                dispEnvelope = (IEnvelope)((IGeometry)display.GeometricTransformer.InvTransform2D(dispEnvelope)).Envelope;
            }
            IGeometryDef geomDef = new GeometryDef(geometryType.Polygon, null, true);

            if (tab != null)
            {
                foreach (DataRow row in tab.Rows)
                {
                    byte[] obj = (byte[])row["SHAPE"];

                    BinaryReader r = new BinaryReader(new MemoryStream());
                    r.BaseStream.Write((byte[])obj, 0, ((byte[])obj).Length);
                    r.BaseStream.Position = 0;

                    Polygon polygon = new Polygon();
                    polygon.Deserialize(r,new GeometryDef(geometryType.Polygon,null,true));
                    r.Close();

                    if (gView.Framework.SpatialAlgorithms.Algorithm.IntersectBox(polygon, dispEnvelope))
                    {
                        PyramidImageClass pClass = new PyramidImageClass(_dataset, _filename, (int)row["ID"], polygon);
                        RasterLayer rLayer = new RasterLayer(pClass);
                        rLayer.InterpolationMethod = this.InterpolationMethod;
                        if (pClass.SpatialReference == null) pClass.SpatialReference = _sRef;
                        layers.Add(rLayer);
                    }
                }
            }
            return new SimpleRasterlayerCursor(layers);
        }

        #endregion

        #region IClass Member

        public string Name
        {
            get { return _title; }
        }

        public string Aliasname
        {
            get { return _title; }
        }

        IDataset IClass.Dataset
        {
            get { return _dataset; }
        }

        #endregion
    }

    public class MrSidFileClass : IRasterClass2,IBitmap,IRasterFile, IDisposable
    {
        private enum RasterType { sid, jp2,unknown }

        private IRasterDataset _dataset = null;
        private string _filename = "";
        private IPolygon _polygon = null;
        private IntPtr _reader = (IntPtr)0;
        private MrSidGeoCoord _geoCoord = new MrSidGeoCoord();
        private ISpatialReference _sRef = null;
        private Bitmap _bm = null;
        private RasterType _type;
        private bool _isValid = false;

        public MrSidFileClass()
        {
        }

        public MrSidFileClass(IRasterDataset dataset, string filename)
            : this(dataset, filename, null)
        {
        }

        public MrSidFileClass(IRasterDataset dataset, string filename, IPolygon polygon)
        {
            _dataset = dataset;
            _filename = filename;

            FileInfo fi = new FileInfo(filename);

            switch (fi.Extension.ToLower())
            {
                case ".sid":
                    _type = RasterType.sid;
                    //_reader = MrSidWrapper.LoadMrSIDReader(filename, ref _geoCoord);
                    break;
                case ".jp2":
                    _type = RasterType.jp2;
                    //_reader = MrSidWrapper.LoadJP2Reader(filename, ref _geoCoord);
                    break;
                default:
                    _type = RasterType.unknown;
                    break;
            }

            FileInfo fiPrj = new FileInfo(fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".prj");
            if (fiPrj.Exists)
            {
                StreamReader sr = new StreamReader(fiPrj.FullName);
                string wkt = sr.ReadToEnd();
                sr.Close();

                _sRef = gView.Framework.Geometry.SpatialReference.FromWKT(wkt);
            }

            if (polygon == null)
            {
                if (!calcPolygon()) return;
            }
            else
            {
                _polygon = polygon;
            }
            _isValid = true;
        }

        ~MrSidFileClass()
        {
            CleanUp();
        }

        
        //private IntPtr memBuffer;
        private bool InitReader()
        {
            if(_reader!=(IntPtr)0) ReleaseReader();
            switch (_type)
            {
                case RasterType.sid:
                    _reader = MrSidWrapper.LoadMrSIDReader(_filename, ref  _geoCoord);
                    break;
                case RasterType.jp2:
                    _reader = MrSidWrapper.LoadJP2Reader(_filename, ref _geoCoord);
                    //FileInfo finfo = new FileInfo(_filename);

                    //unsafe
                    //{
                    //    byte[] buffer = new byte[finfo.Length];
                    //    StreamReader s = new StreamReader(_filename);
                    //    s.BaseStream.Read(buffer, 0, (int)finfo.Length);
                    //    s.Close();

                    //    IntPtr memBuffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(buffer.Length);
                    //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, memBuffer, buffer.Length);
                    //    _reader = MrSidWrapper.LoadJP2MemReader(memBuffer, (int)finfo.Length, ref _geoCoord);

                    //}
                    break;
            }

            string wf = _filename.Substring(0, _filename.Length - 4) + ((_type == RasterType.jp2) ? ".j2w" : ".sdw");
            FileInfo fi = new FileInfo(wf);
            if (fi.Exists)
            {
                //_geoCoord.X -= _geoCoord.xRes / 2.0 + _geoCoord.xRot / 2.0;
                //_geoCoord.Y -= _geoCoord.yRes / 2.0 + _geoCoord.yRot / 2.0;
            }
            return (_reader!=(IntPtr)0);
        }

        private void ReleaseReader()
        {
            try
            {
                if (_reader != (IntPtr)0)
                {
                    MrSidWrapper.FreeReader(_reader);
                    _reader = (IntPtr)0;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void CleanUp()
        {
            ReleaseReader();
            EndPaint(null);
        }

        internal bool isValid
        {
            get { return _isValid; }
        }

        private bool calcPolygon()
        {
            if(!InitReader()) return false;

            TFWFile tfw = this.GeoCoord as TFWFile;
            if (tfw == null) return false;

            int iWidth = _geoCoord.iWidth;
            int iHeight = _geoCoord.iHeight;
            
            _polygon = new Polygon();
            Ring ring = new Ring();
            gView.Framework.Geometry.Point p1 = new gView.Framework.Geometry.Point(
                tfw.X - tfw.dx_X / 2.0 - tfw.dy_X / 2.0,
                tfw.Y - tfw.dx_Y / 2.0 - tfw.dy_Y / 2.0);

            ring.AddPoint(p1);
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + tfw.dx_X * iWidth, p1.Y + tfw.dx_Y * iWidth));
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + tfw.dx_X * iWidth + tfw.dy_X * iHeight, p1.Y + tfw.dx_Y * iWidth + tfw.dy_Y * iHeight));
            ring.AddPoint(new gView.Framework.Geometry.Point(p1.X + tfw.dy_X * iHeight, p1.Y + tfw.dy_Y * iHeight));
            _polygon.AddRing(ring);

            return true;
        }

        #region IRasterClass Member

        public IPolygon Polygon
        {
            get { return _polygon; }
        }

        public Bitmap Bitmap
        {
            get { return _bm; }
        }

        public double oX
        {
            get { return _geoCoord.X; }
        }

        public double oY
        {
            get { return _geoCoord.Y; }
        }

        public double dx1
        {
            get { return _geoCoord.xRes; }
        }

        public double dx2
        {
            get { return _geoCoord.xRot; }
        }

        public double dy1
        {
            get { return _geoCoord.yRot; }
        }

        public double dy2
        {
            get { return _geoCoord.yRes; }
        }

        public ISpatialReference SpatialReference
        {
            get
            {
                return _sRef;
            }
            set
            {
                _sRef = value;
            }
        }

        public void BeginPaint2(gView.Framework.Carto.IDisplay display, ICancelTracker cancelTracker)
        {
            IntPtr hbmp = (IntPtr)0;
            //IntPtr bufferData = (IntPtr)0;
            double mag=1f; // mag immer als float, l�uft stabiler!!!

            int x = 0;
            int y = 0;
            int iWidth = 0;
            int iHeight = 0;

            try
            {
                if (_reader == (IntPtr)0)
                {
                    if (!InitReader()) return;
                }

                if (!(_polygon is ITopologicalOperation) || _reader == (IntPtr)0) return;

                TFWFile tfw = this.GeoCoord as TFWFile;
                if (tfw == null) return;

                IEnvelope dispEnvelope = display.Envelope;
                if (display.GeometricTransformer != null)
                {
                    dispEnvelope = (IEnvelope)((IGeometry)display.GeometricTransformer.InvTransform2D(dispEnvelope)).Envelope;
                }

                IGeometry clipped;
                ((ITopologicalOperation)_polygon).Clip(dispEnvelope, out clipped);
                if (!(clipped is IPolygon)) return;

                IPolygon cPolygon = (IPolygon)clipped;

                // geclipptes Polygon transformieren -> Bild
                vector2[] vecs = new vector2[cPolygon[0].PointCount];
                for (int i = 0; i < cPolygon[0].PointCount; i++)
                {
                    vecs[i] = new vector2(cPolygon[0][i].X, cPolygon[0][i].Y);
                }
                if (!tfw.ProjectInv(vecs)) return;
                IEnvelope picEnv = vector2.IntegerEnvelope(vecs);
                picEnv.minx = Math.Max(0, picEnv.minx);
                picEnv.miny = Math.Max(0, picEnv.miny);
                picEnv.maxx = Math.Min(picEnv.maxx, _geoCoord.iWidth);
                picEnv.maxy = Math.Min(picEnv.maxy, _geoCoord.iHeight);

                // Ecken zur�cktransformieren -> Welt
                vecs = new vector2[3];
                vecs[0] = new vector2(picEnv.minx, picEnv.maxy);
                vecs[1] = new vector2(picEnv.maxx, picEnv.maxy);
                vecs[2] = new vector2(picEnv.minx, picEnv.miny);
                tfw.Project(vecs);
                _p1 = new gView.Framework.Geometry.Point(vecs[0].x, vecs[0].y);
                _p2 = new gView.Framework.Geometry.Point(vecs[1].x, vecs[1].y);
                _p3 = new gView.Framework.Geometry.Point(vecs[2].x, vecs[2].y);

                double pix = display.mapScale / (display.dpi / 0.0254);  // [m]
                double c1 = Math.Sqrt(_geoCoord.xRes * _geoCoord.xRes + _geoCoord.xRot * _geoCoord.xRot);
                double c2 = Math.Sqrt(_geoCoord.yRes * _geoCoord.yRes + _geoCoord.yRot * _geoCoord.yRot);
                mag = Math.Round((Math.Min(c1, c2) / pix), 8);

                // Immer in auf float runden! L�uft stabiler!!!
                //mag = (float)mag; //1.03;
                if (mag > 1f) mag = 1f;
                if (mag < _geoCoord.MinMagnification)
                    mag = (float)_geoCoord.MinMagnification;

                //int x = (int)Math.Max(0, (picEnv.minx * mag));
                //int y = (int)Math.Max(0, (picEnv.miny * mag));
                //int iWidth = (int)((Math.Min(_geoCoord.iWidth, picEnv.Width) - 1) * mag);
                //int iHeight = (int)((Math.Min(_geoCoord.iHeight, picEnv.Height) - 1) * mag);

                x = (int)(picEnv.minx * mag);
                y = (int)(picEnv.miny * mag);
                iWidth = (int)((picEnv.Width - 1) * mag);
                iHeight = (int)((picEnv.Height - 1) * mag);

                hbmp = MrSidWrapper.ReadHBitmap(_reader, x, y, iWidth, iHeight, (double)mag);
                if (hbmp == (IntPtr)0) return;

                _bm = Bitmap.FromHbitmap(hbmp);
                //_bm.Save(@"C:\temp\pic\" + Guid.NewGuid() + ".jpg", ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                //string errMsg = ex.Message;
                EndPaint(cancelTracker);

                if (display is IServiceMap && ((IServiceMap)display).MapServer != null)
                {
                    IMapServer mapServer = ((IServiceMap)display).MapServer;
                    mapServer.Log(
                    "RenderRasterLayerThread", loggingMethod.error,
                        ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" +
                        "filename=" + _filename + "\n" +
                        "x=" + x.ToString() + "\n" +
                        "y=" + y.ToString() + "\n" +
                        "iWidth=" + iWidth.ToString() + "\n" +
                        "iHeight=" + iHeight.ToString() + "\n" +
                        "mag=" + mag.ToString() + "\n");        
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                MrSidWrapper.ReleaseHBitmap(hbmp);
                ReleaseReader();
            }
        }

        public void BeginPaint(gView.Framework.Carto.IDisplay display, ICancelTracker cancelTracker)
        {
            

            IntPtr bufferData = (IntPtr)0;
            BitmapData bitmapData = null;
            double mag = 1f; // mag immer als float, l�uft stabiler!!!

            int x = 0;
            int y = 0;
            int iWidth = 0;
            int iHeight = 0;

            try
            {
                if (_reader == (IntPtr)0)
                {
                    if (!InitReader()) return;
                }

                if (!(_polygon is ITopologicalOperation) || _reader == (IntPtr)0) return;

                TFWFile tfw = this.GeoCoord as TFWFile;
                if (tfw == null) return;

                IEnvelope dispEnvelope = display.DisplayTransformation.TransformedBounds(display); //display.Envelope;
                if (display.GeometricTransformer != null)
                {
                    dispEnvelope = (IEnvelope)((IGeometry)display.GeometricTransformer.InvTransform2D(dispEnvelope)).Envelope;
                }

                IGeometry clipped;
                ((ITopologicalOperation)_polygon).Clip(dispEnvelope, out clipped);
                if (!(clipped is IPolygon)) return;

                IPolygon cPolygon = (IPolygon)clipped;

                // geclipptes Polygon transformieren -> Bild
                vector2[] vecs = new vector2[cPolygon[0].PointCount];
                for (int i = 0; i < cPolygon[0].PointCount; i++)
                {
                    vecs[i] = new vector2(cPolygon[0][i].X, cPolygon[0][i].Y);
                }
                if (!tfw.ProjectInv(vecs)) return;
                IEnvelope picEnv = vector2.IntegerEnvelope(vecs);
                picEnv.minx = Math.Max(0, picEnv.minx);
                picEnv.miny = Math.Max(0, picEnv.miny);
                picEnv.maxx = Math.Min(picEnv.maxx, _geoCoord.iWidth);
                picEnv.maxy = Math.Min(picEnv.maxy, _geoCoord.iHeight);

                // Ecken zur�cktransformieren -> Welt
                vecs = new vector2[3];
                vecs[0] = new vector2(picEnv.minx, picEnv.miny);
                vecs[1] = new vector2(picEnv.maxx, picEnv.miny);
                vecs[2] = new vector2(picEnv.minx, picEnv.maxy);
                tfw.Project(vecs);
                _p1 = new gView.Framework.Geometry.Point(vecs[0].x, vecs[0].y);
                _p2 = new gView.Framework.Geometry.Point(vecs[1].x, vecs[1].y);
                _p3 = new gView.Framework.Geometry.Point(vecs[2].x, vecs[2].y);

                double pix = display.mapScale / (display.dpi / 0.0254);  // [m]
                double c1 = Math.Sqrt(_geoCoord.xRes * _geoCoord.xRes + _geoCoord.xRot * _geoCoord.xRot);
                double c2 = Math.Sqrt(_geoCoord.yRes * _geoCoord.yRes + _geoCoord.yRot * _geoCoord.yRot);
                mag = Math.Round((Math.Min(c1, c2) / pix), 8);

                // Immer in auf float runden! L�uft stabiler!!!
                //mag = (float)mag; //1.03;
                if (mag > 1f) mag = 1f;
                if (mag < _geoCoord.MinMagnification)
                    mag = (float)_geoCoord.MinMagnification;

                x = (int)(picEnv.minx * mag);
                y = (int)(picEnv.miny * mag);
                iWidth = (int)((picEnv.Width - 1) * mag);
                iHeight = (int)((picEnv.Height - 1) * mag);

                bufferData = MrSidWrapper.Read(_reader, x, y, iWidth, iHeight, (double)mag);
                if (bufferData == (IntPtr)0) return;

                int totalWidth = MrSidWrapper.GetTotalCols(bufferData);
                int totalHeight = MrSidWrapper.GetTotalRows(bufferData);

                if (_bm != null)
                    _bm.Dispose();
                _bm = new Bitmap(totalWidth, totalHeight, PixelFormat.Format24bppRgb);
                bitmapData = _bm.LockBits(new Rectangle(0, 0, totalWidth, totalHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                MrSidWrapper.ReadBandData(bufferData, bitmapData.Scan0, (uint)3, (uint)bitmapData.Stride);
                
                //_bm.Save(@"C:\temp\pic\" + Guid.NewGuid() + ".jpg", ImageFormat.Jpeg);

            }
            catch (Exception ex)
            {
                //string errMsg = ex.Message;
                EndPaint(cancelTracker);

                if (display is IServiceMap && ((IServiceMap)display).MapServer != null)
                {
                    IMapServer mapServer = ((IServiceMap)display).MapServer;
                    mapServer.Log(
                    "RenderRasterLayerThread", loggingMethod.error,
                        ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" +
                        "filename=" + _filename + "\n" +
                        "x=" + x.ToString() + "\n" +
                        "y=" + y.ToString() + "\n" +
                        "iWidth=" + iWidth.ToString() + "\n" +
                        "iHeight=" + iHeight.ToString() + "\n" +
                        "mag=" + mag.ToString() + "\n");
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                if (bitmapData != null)
                    _bm.UnlockBits(bitmapData);
                MrSidWrapper.ReleaseBandData(bufferData);
                ReleaseReader();
            }
        }

        public void EndPaint(ICancelTracker cancelTracker)
        {
            if (_bm != null)
            {
                _bm.Dispose();
                _bm = null;
            }
        }

        #endregion

        #region IClass Member

        public string Name
        {
            get
            {
                try
                {
                    FileInfo fi = new FileInfo(_filename);
                    return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                }
                catch
                {
                    return "???";
                }
            }
        }

        public string Aliasname
        {
            get { return Name; }
        }

        public IDataset Dataset
        {
            get { return _dataset; }
        }

        #endregion

        #region IBitmap Member

        public Bitmap LoadBitmap()
        {
            return null;
        }

        #endregion

        #region IRasterFile Member

        public string Filename
        {
            get { return _filename; }
        }

        public IRasterWorldFile WorldFile
        {
            get
            {

                TFWFile tfw = new TFWFile(
                    _geoCoord.X,
                    _geoCoord.Y,
                    _geoCoord.xRes,
                    _geoCoord.xRot,
                    _geoCoord.yRot,
                    _geoCoord.yRes);

                string wf = _filename.Substring(0, _filename.Length - 4) + ((_type == RasterType.jp2) ? ".j2w" : ".sdw");
                FileInfo fi = new FileInfo(wf);
                if (fi.Exists)
                {
                    tfw.Filename = wf;
                }
                else if (_geoCoord.X != 0.0 && _geoCoord.Y != 0.0 &&
                    Math.Abs(_geoCoord.xRes)!=1.0 && Math.Abs(_geoCoord.yRes)!=1.0)
                {
                    // valid
                }
                else
                {
                    tfw.isValid = false;
                }
                return tfw;

            }
        }

        #endregion

        public IRasterWorldFile GeoCoord
        {
            get
            {
                return new TFWFile(
                        _geoCoord.X,
                        _geoCoord.Y,
                        _geoCoord.xRes,
                        _geoCoord.xRot,
                        _geoCoord.yRot,
                        _geoCoord.yRes);
            }
        }

        #region IRasterClass2 Member

        private IPoint _p1, _p2, _p3;
        public IPoint PicPoint1
        {
            get { return _p1; }
        }

        public IPoint PicPoint2
        {
            get { return _p2; }
        }

        public IPoint PicPoint3
        {
            get { return _p3; }
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CleanUp();
        }

        #endregion
    }
}
