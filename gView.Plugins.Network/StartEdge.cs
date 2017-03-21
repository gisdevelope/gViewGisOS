﻿using System;
using System.Collections.Generic;
using System.Text;
using gView.Framework.UI;
using gView.Framework.UI.Events;
using gView.Framework.Geometry;
using gView.Framework.Data;
using gView.Framework.Carto;
using gView.Plugins.Network.Graphic;
using gView.Framework.system;
using gView.Framework.Network;

namespace gView.Plugins.Network
{
    [RegisterPlugIn("D7640425-DEF2-4c57-A165-464CDAB7C56E")]
    public class StartEdge : ITool
    {
        private IMapDocument _doc = null;
        private Module _module = null;

        #region ITool Member

        public string Name
        {
            get { return "Start Edge"; }
        }

        public bool Enabled
        {
            get
            {
                if (_module == null || _module.SelectedNetworkFeatureClass == null)
                    return false;
                return true;
            }
        }

        public string ToolTip
        {
            get { return "Start Edge"; }
        }

        public ToolType toolType
        {
            get { return ToolType.click; }
        }

        public object Image
        {
            get
            {
                return global::gView.Plugins.Network.Properties.Resources.start_point;
            }
        }

        public void OnCreate(object hook)
        {
            if (hook is IMapDocument)
            {
                _doc = (IMapDocument)hook;
                _module = Module.GetModule(_doc);
            }
        }

        public void OnEvent(object MapEvent)
        {
            if (_module == null ||
                _module.SelectedNetworkFeatureClass == null ||
                !(MapEvent is MapEventClick))
                return;

            MapEventClick ev = (MapEventClick)MapEvent;
            Point p = new Point(ev.x, ev.y);
            double tol = 5 * ((IDisplay)ev.Map).mapScale / (96 / 0.0254);  // [m]

            _module.RemoveNetworkStartGraphicElement((IDisplay)ev.Map);

            IGraphEdge graphEdge = _module.SelectedNetworkFeatureClass.GetGraphEdge(p, tol);
            if (graphEdge != null)
            {
                _module.StartEdgeIndex = graphEdge.Eid;

                ((IDisplay)ev.Map).GraphicsContainer.Elements.Add(new GraphicStartPoint(p));
                _module.StartPoint = p;
            }
            ((MapEvent)MapEvent).drawPhase = DrawPhase.Graphics;
            ((MapEvent)MapEvent).refreshMap = true;
        }

        #endregion
    }
}
