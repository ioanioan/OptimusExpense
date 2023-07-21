import React, { Component, createRef } from 'react';
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

export class PartnerPoint extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.partnerPoint = {};
        this.methodSave = "Lists/SavePartnerPoint";
    }
    async save() {
        var isSave = this.state.entity.partnerPoint.name;
        await super.save(isSave);
    }
    componentDidMount() {
      //  this.populate();
    }
   
    render() {
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Punct lucru" modal className="p-fluid" onHide={this.hide}>
                <span className="p-float-label mt-4">
                    <InputText id="name" value={this.state.entity.partnerPoint.name} onChange={(e) => this.onInputChange(e.target.value, 'partnerPoint.name')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.partnerPoint.name })} />
                    {this.state.submitted && !this.state.entity.partnerPoint.name && <small className="p-invalid">Numele este obligatoriu!</small>}
                    <label htmlFor="name">Nume*</label>
                </span>
                
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbCommercialPoint" checked={this.state.entity.partnerPoint.commercialPoint} onChange={(e) => this.onInputChange(e.checked, 'partnerPoint.commercialPoint')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbCommercialPoint" >Punct comercial</label>
                </div>
            </Dialog>
        );
    }
}

export class ListPartnerPoint extends ListComponent {
    constructor(props) {

        super(props);
        this.state.entityL = props.entityL;
        
        this.methodDelete = "Lists/DeletePartnerPoint";
    }
    componentDidMount() {
        this.populate();
    }
    
    async populate() {
        var list = await authService.fetchAuth('Lists/GetPartnerPointsByPartnerId/' + this.getPartnerId());

        this.setState({ list: list });
    }

    getPartnerId() {
        return this.state.entityL != null && this.state.entityL.partner != null && this.state.entityL.partner.partnerId != null ? this.state.entityL.partner.partnerId : 0;
    }
     
    save(entity) {
        var index = this.state.list.findIndex(p => p.partnerPoint.partnerPointId == entity.partnerPoint.partnerPointId);
        super.save(entity, index);
    }

    add() {
        this.setState({
            obj: {
                edit: true, partnerPoint: { partnerId: this.getPartnerId() }
            },
        });
    }  

  

    render() {
        return (
            <div className="p-grid p-fluid" >
                <div className="p-col-12">
                    <div className="card">
                        <h3>Puncte lucru</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="partnerPoint.name" header="Nume" sortable filter ></Column>
                           
                            <Column field="partnerPoint.commercialPoint" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>
                        </DataTable>
                        <PartnerPoint entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                            <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.partnerPoint != null && this.state.rowData.partnerPoint.name}</b> ?</span>
                            </div>
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListPartnerPoint;
