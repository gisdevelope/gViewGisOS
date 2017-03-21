//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:2.0.50727.3053
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Dieser Quellcode wurde automatisch generiert von wsdl, Version=2.0.50727.42.
// 
namespace gView.Interoperability.AGS.Proxy
{
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceCatalogBinding", Namespace="http://www.esri.com/schemas/ArcGIS/9.3")]
    public partial class Catalog : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetMessageVersionOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetMessageFormatsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTokenServiceURLOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetFoldersOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetServiceDescriptionsOperationCompleted;
        
        private System.Threading.SendOrPostCallback RequiresTokensOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetServiceDescriptionsExOperationCompleted;
        
        /// <remarks/>
        public Catalog(string url) {
            //this.Url = "http://localhost/ArcGIS/services";
            this.Url = url;
        }
        
        /// <remarks/>
        public event GetMessageVersionCompletedEventHandler GetMessageVersionCompleted;
        
        /// <remarks/>
        public event GetMessageFormatsCompletedEventHandler GetMessageFormatsCompleted;
        
        /// <remarks/>
        public event GetTokenServiceURLCompletedEventHandler GetTokenServiceURLCompleted;
        
        /// <remarks/>
        public event GetFoldersCompletedEventHandler GetFoldersCompleted;
        
        /// <remarks/>
        public event GetServiceDescriptionsCompletedEventHandler GetServiceDescriptionsCompleted;
        
        /// <remarks/>
        public event RequiresTokensCompletedEventHandler RequiresTokensCompleted;
        
        /// <remarks/>
        public event GetServiceDescriptionsExCompletedEventHandler GetServiceDescriptionsExCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("MessageVersion", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public esriArcGISVersion GetMessageVersion() {
            object[] results = this.Invoke("GetMessageVersion", new object[0]);
            return ((esriArcGISVersion)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetMessageVersion(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetMessageVersion", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public esriArcGISVersion EndGetMessageVersion(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((esriArcGISVersion)(results[0]));
        }
        
        /// <remarks/>
        public void GetMessageVersionAsync() {
            this.GetMessageVersionAsync(null);
        }
        
        /// <remarks/>
        public void GetMessageVersionAsync(object userState) {
            if ((this.GetMessageVersionOperationCompleted == null)) {
                this.GetMessageVersionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMessageVersionOperationCompleted);
            }
            this.InvokeAsync("GetMessageVersion", new object[0], this.GetMessageVersionOperationCompleted, userState);
        }
        
        private void OnGetMessageVersionOperationCompleted(object arg) {
            if ((this.GetMessageVersionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMessageVersionCompleted(this, new GetMessageVersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("MessageFormats", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public esriServiceCatalogMessageFormat GetMessageFormats() {
            object[] results = this.Invoke("GetMessageFormats", new object[0]);
            return ((esriServiceCatalogMessageFormat)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetMessageFormats(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetMessageFormats", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public esriServiceCatalogMessageFormat EndGetMessageFormats(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((esriServiceCatalogMessageFormat)(results[0]));
        }
        
        /// <remarks/>
        public void GetMessageFormatsAsync() {
            this.GetMessageFormatsAsync(null);
        }
        
        /// <remarks/>
        public void GetMessageFormatsAsync(object userState) {
            if ((this.GetMessageFormatsOperationCompleted == null)) {
                this.GetMessageFormatsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMessageFormatsOperationCompleted);
            }
            this.InvokeAsync("GetMessageFormats", new object[0], this.GetMessageFormatsOperationCompleted, userState);
        }
        
        private void OnGetMessageFormatsOperationCompleted(object arg) {
            if ((this.GetMessageFormatsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMessageFormatsCompleted(this, new GetMessageFormatsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("TokenServiceURL", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GetTokenServiceURL() {
            object[] results = this.Invoke("GetTokenServiceURL", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetTokenServiceURL(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetTokenServiceURL", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public string EndGetTokenServiceURL(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetTokenServiceURLAsync() {
            this.GetTokenServiceURLAsync(null);
        }
        
        /// <remarks/>
        public void GetTokenServiceURLAsync(object userState) {
            if ((this.GetTokenServiceURLOperationCompleted == null)) {
                this.GetTokenServiceURLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTokenServiceURLOperationCompleted);
            }
            this.InvokeAsync("GetTokenServiceURL", new object[0], this.GetTokenServiceURLOperationCompleted, userState);
        }
        
        private void OnGetTokenServiceURLOperationCompleted(object arg) {
            if ((this.GetTokenServiceURLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTokenServiceURLCompleted(this, new GetTokenServiceURLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("FolderNames", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("String", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public string[] GetFolders() {
            object[] results = this.Invoke("GetFolders", new object[0]);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetFolders(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetFolders", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndGetFolders(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetFoldersAsync() {
            this.GetFoldersAsync(null);
        }
        
        /// <remarks/>
        public void GetFoldersAsync(object userState) {
            if ((this.GetFoldersOperationCompleted == null)) {
                this.GetFoldersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetFoldersOperationCompleted);
            }
            this.InvokeAsync("GetFolders", new object[0], this.GetFoldersOperationCompleted, userState);
        }
        
        private void OnGetFoldersOperationCompleted(object arg) {
            if ((this.GetFoldersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetFoldersCompleted(this, new GetFoldersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("ServiceDescriptions", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ServiceDescription[] GetServiceDescriptions() {
            object[] results = this.Invoke("GetServiceDescriptions", new object[0]);
            return ((ServiceDescription[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetServiceDescriptions(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetServiceDescriptions", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public ServiceDescription[] EndGetServiceDescriptions(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServiceDescription[])(results[0]));
        }
        
        /// <remarks/>
        public void GetServiceDescriptionsAsync() {
            this.GetServiceDescriptionsAsync(null);
        }
        
        /// <remarks/>
        public void GetServiceDescriptionsAsync(object userState) {
            if ((this.GetServiceDescriptionsOperationCompleted == null)) {
                this.GetServiceDescriptionsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetServiceDescriptionsOperationCompleted);
            }
            this.InvokeAsync("GetServiceDescriptions", new object[0], this.GetServiceDescriptionsOperationCompleted, userState);
        }
        
        private void OnGetServiceDescriptionsOperationCompleted(object arg) {
            if ((this.GetServiceDescriptionsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetServiceDescriptionsCompleted(this, new GetServiceDescriptionsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("Result", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool RequiresTokens() {
            object[] results = this.Invoke("RequiresTokens", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRequiresTokens(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("RequiresTokens", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndRequiresTokens(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void RequiresTokensAsync() {
            this.RequiresTokensAsync(null);
        }
        
        /// <remarks/>
        public void RequiresTokensAsync(object userState) {
            if ((this.RequiresTokensOperationCompleted == null)) {
                this.RequiresTokensOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRequiresTokensOperationCompleted);
            }
            this.InvokeAsync("RequiresTokens", new object[0], this.RequiresTokensOperationCompleted, userState);
        }
        
        private void OnRequiresTokensOperationCompleted(object arg) {
            if ((this.RequiresTokensCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RequiresTokensCompleted(this, new RequiresTokensCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.esri.com/schemas/ArcGIS/9.3", ResponseNamespace="http://www.esri.com/schemas/ArcGIS/9.3", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("ServiceDescriptions", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ServiceDescription[] GetServiceDescriptionsEx([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string FolderName) {
            object[] results = this.Invoke("GetServiceDescriptionsEx", new object[] {
                        FolderName});
            return ((ServiceDescription[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetServiceDescriptionsEx(string FolderName, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetServiceDescriptionsEx", new object[] {
                        FolderName}, callback, asyncState);
        }
        
        /// <remarks/>
        public ServiceDescription[] EndGetServiceDescriptionsEx(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServiceDescription[])(results[0]));
        }
        
        /// <remarks/>
        public void GetServiceDescriptionsExAsync(string FolderName) {
            this.GetServiceDescriptionsExAsync(FolderName, null);
        }
        
        /// <remarks/>
        public void GetServiceDescriptionsExAsync(string FolderName, object userState) {
            if ((this.GetServiceDescriptionsExOperationCompleted == null)) {
                this.GetServiceDescriptionsExOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetServiceDescriptionsExOperationCompleted);
            }
            this.InvokeAsync("GetServiceDescriptionsEx", new object[] {
                        FolderName}, this.GetServiceDescriptionsExOperationCompleted, userState);
        }
        
        private void OnGetServiceDescriptionsExOperationCompleted(object arg) {
            if ((this.GetServiceDescriptionsExCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetServiceDescriptionsExCompleted(this, new GetServiceDescriptionsExCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.esri.com/schemas/ArcGIS/9.3")]
    public enum esriArcGISVersion {
        
        /// <remarks/>
        esriArcGISVersion83,
        
        /// <remarks/>
        esriArcGISVersion90,
        
        /// <remarks/>
        esriArcGISVersion92,
        
        /// <remarks/>
        esriArcGISVersion93,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.esri.com/schemas/ArcGIS/9.3")]
    public enum esriServiceCatalogMessageFormat {
        
        /// <remarks/>
        esriServiceCatalogMessageFormatSoap,
        
        /// <remarks/>
        esriServiceCatalogMessageFormatBin,
        
        /// <remarks/>
        esriServiceCatalogMessageFormatSoapOrBin,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.esri.com/schemas/ArcGIS/9.3")]
    public partial class ServiceDescription {
        
        private string nameField;
        
        private string typeField;
        
        private string urlField;
        
        private string parentTypeField;
        
        private string capabilitiesField;
        
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Url {
            get {
                return this.urlField;
            }
            set {
                this.urlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ParentType {
            get {
                return this.parentTypeField;
            }
            set {
                this.parentTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Capabilities {
            get {
                return this.capabilitiesField;
            }
            set {
                this.capabilitiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetMessageVersionCompletedEventHandler(object sender, GetMessageVersionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMessageVersionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetMessageVersionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public esriArcGISVersion Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((esriArcGISVersion)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetMessageFormatsCompletedEventHandler(object sender, GetMessageFormatsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMessageFormatsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetMessageFormatsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public esriServiceCatalogMessageFormat Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((esriServiceCatalogMessageFormat)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetTokenServiceURLCompletedEventHandler(object sender, GetTokenServiceURLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTokenServiceURLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTokenServiceURLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetFoldersCompletedEventHandler(object sender, GetFoldersCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetFoldersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetFoldersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetServiceDescriptionsCompletedEventHandler(object sender, GetServiceDescriptionsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetServiceDescriptionsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetServiceDescriptionsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ServiceDescription[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ServiceDescription[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void RequiresTokensCompletedEventHandler(object sender, RequiresTokensCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RequiresTokensCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RequiresTokensCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void GetServiceDescriptionsExCompletedEventHandler(object sender, GetServiceDescriptionsExCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetServiceDescriptionsExCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetServiceDescriptionsExCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ServiceDescription[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ServiceDescription[])(this.results[0]));
            }
        }
    }
}
