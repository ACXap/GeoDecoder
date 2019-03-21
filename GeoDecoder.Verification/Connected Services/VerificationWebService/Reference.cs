﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeoDecoder.VerificationService.VerificationWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.informatica.com/dis/ws/ws_", ConfigurationName="VerificationWebService.wsSearchAddrElByFullNamePortType")]
    public interface wsSearchAddrElByFullNamePortType {
        
        // CODEGEN: Контракт генерации сообщений с операцией SearchAddressElementByFullName не является ни RPC, ни упакованным документом.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="", Name="ErrorField")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse SearchAddressElementByFullName(GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse> SearchAddressElementByFullNameAsync(GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.informatica.com/dis/ws/ws_")]
    public partial class AddressElementNameData : object, System.ComponentModel.INotifyPropertyChanged {
        
        private AddressElementNameDataAddressElementFullNameGroup[] addressElementFullNameListField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("AddressElementFullNameGroup", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public AddressElementNameDataAddressElementFullNameGroup[] AddressElementFullNameList {
            get {
                return this.addressElementFullNameListField;
            }
            set {
                this.addressElementFullNameListField = value;
                this.RaisePropertyChanged("AddressElementFullNameList");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.informatica.com/dis/ws/ws_")]
    public partial class AddressElementNameDataAddressElementFullNameGroup : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string fullAddressField;
        
        private string districtField;
        
        private string cityField;
        
        private string localityField;
        
        private string streetField;
        
        private string houseField;
        
        private string maxResultField;
        
        private string systemCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string FullAddress {
            get {
                return this.fullAddressField;
            }
            set {
                this.fullAddressField = value;
                this.RaisePropertyChanged("FullAddress");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string District {
            get {
                return this.districtField;
            }
            set {
                this.districtField = value;
                this.RaisePropertyChanged("District");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
                this.RaisePropertyChanged("City");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string Locality {
            get {
                return this.localityField;
            }
            set {
                this.localityField = value;
                this.RaisePropertyChanged("Locality");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string Street {
            get {
                return this.streetField;
            }
            set {
                this.streetField = value;
                this.RaisePropertyChanged("Street");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string House {
            get {
                return this.houseField;
            }
            set {
                this.houseField = value;
                this.RaisePropertyChanged("House");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer", Order=6)]
        public string MaxResult {
            get {
                return this.maxResultField;
            }
            set {
                this.maxResultField = value;
                this.RaisePropertyChanged("MaxResult");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string SystemCode {
            get {
                return this.systemCodeField;
            }
            set {
                this.systemCodeField = value;
                this.RaisePropertyChanged("SystemCode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.informatica.com/dis/ws/ws_")]
    public partial class AddressElementNameResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private AddressElementNameResponseAddressElementNameGroup[] addressElementResponseListField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("AddressElementNameGroup", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public AddressElementNameResponseAddressElementNameGroup[] AddressElementResponseList {
            get {
                return this.addressElementResponseListField;
            }
            set {
                this.addressElementResponseListField = value;
                this.RaisePropertyChanged("AddressElementResponseList");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.informatica.com/dis/ws/ws_")]
    public partial class AddressElementNameResponseAddressElementNameGroup : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string qualityCodeField;
        
        private string checkStatusField;
        
        private string parsingLevelCodeField;
        
        private string systemCodeField;
        
        private string globalIDField;
        
        private string kLADRLocalityIdField;
        
        private string fIASLocalityIdField;
        
        private string kLADRStreetIdField;
        
        private string fIASStreetIdField;
        
        private string streetField;
        
        private string streetKindField;
        
        private string houseField;
        
        private string houseLiteraField;
        
        private string cornerHouseField;
        
        private string buildingBlockField;
        
        private string buildingBlockLiteraField;
        
        private string buildingField;
        
        private string buildingLiteraField;
        
        private string ownershipField;
        
        private string ownershipLiteraField;
        
        private string fIASHouseIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string QualityCode {
            get {
                return this.qualityCodeField;
            }
            set {
                this.qualityCodeField = value;
                this.RaisePropertyChanged("QualityCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string CheckStatus {
            get {
                return this.checkStatusField;
            }
            set {
                this.checkStatusField = value;
                this.RaisePropertyChanged("CheckStatus");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string ParsingLevelCode {
            get {
                return this.parsingLevelCodeField;
            }
            set {
                this.parsingLevelCodeField = value;
                this.RaisePropertyChanged("ParsingLevelCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string SystemCode {
            get {
                return this.systemCodeField;
            }
            set {
                this.systemCodeField = value;
                this.RaisePropertyChanged("SystemCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string GlobalID {
            get {
                return this.globalIDField;
            }
            set {
                this.globalIDField = value;
                this.RaisePropertyChanged("GlobalID");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string KLADRLocalityId {
            get {
                return this.kLADRLocalityIdField;
            }
            set {
                this.kLADRLocalityIdField = value;
                this.RaisePropertyChanged("KLADRLocalityId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string FIASLocalityId {
            get {
                return this.fIASLocalityIdField;
            }
            set {
                this.fIASLocalityIdField = value;
                this.RaisePropertyChanged("FIASLocalityId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string KLADRStreetId {
            get {
                return this.kLADRStreetIdField;
            }
            set {
                this.kLADRStreetIdField = value;
                this.RaisePropertyChanged("KLADRStreetId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string FIASStreetId {
            get {
                return this.fIASStreetIdField;
            }
            set {
                this.fIASStreetIdField = value;
                this.RaisePropertyChanged("FIASStreetId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public string Street {
            get {
                return this.streetField;
            }
            set {
                this.streetField = value;
                this.RaisePropertyChanged("Street");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string StreetKind {
            get {
                return this.streetKindField;
            }
            set {
                this.streetKindField = value;
                this.RaisePropertyChanged("StreetKind");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string House {
            get {
                return this.houseField;
            }
            set {
                this.houseField = value;
                this.RaisePropertyChanged("House");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string HouseLitera {
            get {
                return this.houseLiteraField;
            }
            set {
                this.houseLiteraField = value;
                this.RaisePropertyChanged("HouseLitera");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string CornerHouse {
            get {
                return this.cornerHouseField;
            }
            set {
                this.cornerHouseField = value;
                this.RaisePropertyChanged("CornerHouse");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public string BuildingBlock {
            get {
                return this.buildingBlockField;
            }
            set {
                this.buildingBlockField = value;
                this.RaisePropertyChanged("BuildingBlock");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public string BuildingBlockLitera {
            get {
                return this.buildingBlockLiteraField;
            }
            set {
                this.buildingBlockLiteraField = value;
                this.RaisePropertyChanged("BuildingBlockLitera");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=16)]
        public string Building {
            get {
                return this.buildingField;
            }
            set {
                this.buildingField = value;
                this.RaisePropertyChanged("Building");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=17)]
        public string BuildingLitera {
            get {
                return this.buildingLiteraField;
            }
            set {
                this.buildingLiteraField = value;
                this.RaisePropertyChanged("BuildingLitera");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=18)]
        public string Ownership {
            get {
                return this.ownershipField;
            }
            set {
                this.ownershipField = value;
                this.RaisePropertyChanged("Ownership");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=19)]
        public string OwnershipLitera {
            get {
                return this.ownershipLiteraField;
            }
            set {
                this.ownershipLiteraField = value;
                this.RaisePropertyChanged("OwnershipLitera");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=20)]
        public string FIASHouseId {
            get {
                return this.fIASHouseIdField;
            }
            set {
                this.fIASHouseIdField = value;
                this.RaisePropertyChanged("FIASHouseId");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SearchAddressElementByFullName {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.informatica.com/dis/ws/ws_", Order=0)]
        public GeoDecoder.VerificationService.VerificationWebService.AddressElementNameData AddressElementNameData;
        
        public SearchAddressElementByFullName() {
        }
        
        public SearchAddressElementByFullName(GeoDecoder.VerificationService.VerificationWebService.AddressElementNameData AddressElementNameData) {
            this.AddressElementNameData = AddressElementNameData;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SearchAddressElementByFullNameResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.informatica.com/dis/ws/ws_", Order=0)]
        public GeoDecoder.VerificationService.VerificationWebService.AddressElementNameResponse AddressElementNameResponse;
        
        public SearchAddressElementByFullNameResponse() {
        }
        
        public SearchAddressElementByFullNameResponse(GeoDecoder.VerificationService.VerificationWebService.AddressElementNameResponse AddressElementNameResponse) {
            this.AddressElementNameResponse = AddressElementNameResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsSearchAddrElByFullNamePortTypeChannel : GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsSearchAddrElByFullNamePortTypeClient : System.ServiceModel.ClientBase<GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType>, GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType {
        
        public wsSearchAddrElByFullNamePortTypeClient() {
        }
        
        public wsSearchAddrElByFullNamePortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsSearchAddrElByFullNamePortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsSearchAddrElByFullNamePortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsSearchAddrElByFullNamePortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType.SearchAddressElementByFullName(GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName request) {
            return base.Channel.SearchAddressElementByFullName(request);
        }
        
        public GeoDecoder.VerificationService.VerificationWebService.AddressElementNameResponse SearchAddressElementByFullName(GeoDecoder.VerificationService.VerificationWebService.AddressElementNameData AddressElementNameData) {
            GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName inValue = new GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName();
            inValue.AddressElementNameData = AddressElementNameData;
            GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse retVal = ((GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType)(this)).SearchAddressElementByFullName(inValue);
            return retVal.AddressElementNameResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse> GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType.SearchAddressElementByFullNameAsync(GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName request) {
            return base.Channel.SearchAddressElementByFullNameAsync(request);
        }
        
        public System.Threading.Tasks.Task<GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullNameResponse> SearchAddressElementByFullNameAsync(GeoDecoder.VerificationService.VerificationWebService.AddressElementNameData AddressElementNameData) {
            GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName inValue = new GeoDecoder.VerificationService.VerificationWebService.SearchAddressElementByFullName();
            inValue.AddressElementNameData = AddressElementNameData;
            return ((GeoDecoder.VerificationService.VerificationWebService.wsSearchAddrElByFullNamePortType)(this)).SearchAddressElementByFullNameAsync(inValue);
        }
    }
}
