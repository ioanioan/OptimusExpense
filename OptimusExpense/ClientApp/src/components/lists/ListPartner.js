import React, { Component, createRef   } from 'react';
import { InputText } from 'primereact/inputtext';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import { Checkbox } from 'primereact/checkbox';
import { EditComponent } from '../base/EditComponent';
import { ListComponent } from '../base/ListComponent';
import { Dropdown } from 'primereact/dropdown';
import { ListPartnerPoint } from './ListPartnerPoint';

export class Partner extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.partner = {};
        this.methodSave = "Lists/SavePartner";
    }
    async save() {
        var isSave = this.state.entity.partner.name && this.state.entity.partner.code && this.state.entity.partner.statusId && this.state.entity.partner.partnerTypeId && this.state.entity.partner.companyId;
        await super.save(isSave);
    }
    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listStatus = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-4');
        var listPartnerType = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-7');
        var listCompany = await authService.fetchAuth('Lists/GetAllCompanies');
        
        this.setState({ listStatus: listStatus, listPartnerType: listPartnerType, listCompany: listCompany  });
    }

    componentDidUpdate(prevProps) {
        if (this.props.entity.partner == null) {
            this.props.entity.partner = {};
        }
        if (prevProps.entity.partner !== this.props.entity.partner) {
            if (this.state.listStatus != null && this.state.listStatus.length > 0 && this.props.entity.partner.statusId == null) {
                this.props.entity.partner.statusId = this.state.listStatus[0].dictionaryDetailId;
            }
            if (this.state.listPartnerType != null && this.state.listPartnerType.length > 0 && this.props.entity.partner.partnerTypeId == null) {
                this.props.entity.partner.partnerTypeId = this.state.listPartnerType[0].dictionaryDetailId;
            }
            if (this.state.listCompany != null && this.state.listCompany.length > 0 && this.props.entity.partner.companyId == null) {
                this.props.entity.partner.companyId = this.state.listCompany[0].partner.partnerId;
            }
        }

        super.componentDidUpdate(prevProps);
    }

    render() {
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '650px' }} header="Partener" modal className="p-fluid" onHide={this.hide}>
                <div className="p-grid">
                    <div className="p-col-12 p-md-6">
                        <span className="p-float-label mt-4">
                            <InputText id="name" value={this.state.entity.partner.name} onChange={(e) => this.onInputChange(e.target.value, 'partner.name')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.partner.name })} />
                            {this.state.submitted && !this.state.entity.partner.name && <small className="p-invalid">Numele este obligatoriu!</small>}
                            <label htmlFor="name">Nume*</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="code" value={this.state.entity.partner.code} onChange={(e) => this.onInputChange(e.target.value, 'partner.code')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.partner.code })} />
                            {this.state.submitted && !this.state.entity.partner.code && <small className="p-invalid">Codul este obligatoriu!</small>}
                            <label htmlFor="code">Cod*</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="fiscalCode" value={this.state.entity.partner.fiscalCode} onChange={(e) => this.onInputChange(e.target.value, 'partner.fiscalCode')} />
                            <label htmlFor="fiscalCode">Cod fiscal</label>
                        </span>
                    </div>
                    <div className="p-col-12 p-md-6">
                        <span className="p-float-label mt-4">
                            <InputText id="regComCode" value={this.state.entity.partner.regComCode} onChange={(e) => this.onInputChange(e.target.value, 'partner.regComCode')} />
                            <label htmlFor="regComCode">Nr. registrul comertului</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.partner.partnerTypeId} options={this.state.listPartnerType} onChange={(e) => this.onInputChange(e.value, 'partner.partnerTypeId', 'partnerType', e)} optionLabel="name" optionValue="dictionaryDetailId" />
                            {this.state.submitted && !this.state.entity.partner.partnerTypeId && <small className="p-invalid">Tipul este obligatoriu!</small>}
                            <label htmlFor="PartnerType">Tip partener</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.partner.companyId} options={this.state.listCompany} onChange={(e) => this.onInputChange(e.value, 'partner.companyId', 'company', e)} optionLabel="partner.name" optionValue="partner.partnerId" />
                            {this.state.submitted && !this.state.entity.partner.companyId && <small className="p-invalid">Compania este obligatorie!</small>}
                            <label htmlFor="CompanyId">Companie</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.partner.statusId} options={this.state.listStatus} onChange={(e) => this.onInputChange(e.value, 'partner.statusId', 'partnerStatus', e)} optionLabel="name" optionValue="dictionaryDetailId" />
                            {this.state.submitted && !this.state.entity.partner.statusId && <small className="p-invalid">Statusul este obligatoriu!</small>}
                            <label htmlFor="StatusId">Status</label>
                        </span>
                    </div>
                    {this.state.entity.partner.partnerId != null&&
                        <div className="p-col-12 p-md-12"  >
                            <ListPartnerPoint entityL={{ ...this.state.entity }} />
                        </div>
                    }
                </div>

            </Dialog>
        );
    }
}

export class ListPartner extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeletePartner";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetAllPartners');

        this.setState({ list: list });
    } 
    edit(rowData) {      
        super.edit(rowData);
    }
    save(entity) {
        var index = this.state.list.findIndex(p => p.partner.partnerId == entity.partner.partnerId);
        super.save(entity, index);  
    } 

    statusBodyTemplate = (rowData) => {
        return <center><span className={`status-badge status-${('' + rowData.partnerStatus).toLowerCase()}`}>{rowData.partnerStatus}</span></center>;
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Partener</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="partner.name" header="Nume" sortable filter ></Column>       
                            <Column field="partner.code" header="Cod" sortable filter ></Column>  
                            <Column field="partner.fiscalCode" header="Cod fiscal" sortable filter ></Column>  
                            <Column field="partner.regComCode" header="Nr. reg. comertului" sortable filter ></Column>
                            <Column field="partnerType" header="Tip partener" sortable filter ></Column>
                            <Column field="company" header="Companie" sortable filter ></Column>
                            <Column field="partnerStatus" header="Status" sortable filter body={this.statusBodyTemplate} headerStyle={{ width: '7rem' }}></Column>                       
                        </DataTable>
                        <Partner entity={this.state.obj} onSave={this.save}  />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                         <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.partner!=null&&this.state.rowData.partner.name}</b> ?</span>
                            </div>  
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListPartner;
