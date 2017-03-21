using System;
using System.Collections.Generic;
using System.Text;
using gView.Framework.IO;
using gView.Framework.Carto;
using gView.Framework.UI;

namespace gView.MapServer.Lib
{
    class ServiceWrapper : IMetadataProvider, IPropertyPage
    {
        private IServiceMap _serviceMap;
        #region IMetadataProvider Member

        public bool ApplyTo(object Object)
        {
            if (Object is IServiceMap)
            {
                _serviceMap = (IServiceMap)Object; ;
                return true;
            }

            _serviceMap = null;
            return false;
        }

        public string Name
        {
            get { return "Service Wrapping"; }
        }

        #endregion

        #region IPersistable Member

        public void Load(IPersistStream stream)
        {
            
        }

        public void Save(IPersistStream stream)
        {
            
        }

        #endregion

        #region IPropertyPage Member

        public object PropertyPage(object initObject)
        {
            return null;
        }

        public object PropertyPageObject()
        {
            return null;
        }

        #endregion
    }
}
