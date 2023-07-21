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

export class ExpenseCostCenter extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.expenseCostCenter = {};
        this.methodSave = "Lists/SaveExpenseCostCenter";
    }
    async save() {
        var isSave = this.state.entity.expenseCostCenter.name;
        await super.save(isSave);
    }

    componentDidMount() {
        this.populate();
    }

    componentDidUpdate(prevProps) {
        if (this.props.entity.expenseCostCenter == null) {
            this.props.entity.expenseCostCenter = {};
        }
        if (prevProps.entity !== this.props.entity) {
        }
        super.componentDidUpdate(prevProps);
    }

    async populate() {
    }

    render() {     
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Centru de cost" modal className="p-fluid" onHide={this.hide}>             
                <span className="p-float-label mt-4">
                    <InputText id="name" value={this.state.entity.expenseCostCenter.name} onChange={(e) => this.onInputChange(e.target.value, 'expenseCostCenter.name')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.expenseCostCenter.name })} />
                    {this.state.submitted && !this.state.entity.expenseCostCenter.name && <small className="p-invalid">Numele este obligatoriu!</small>}
                    <label htmlFor="name">Nume*</label>
                </span> 
                <span className="p-float-label mt-4">
                    <InputText id="code" value={this.state.entity.expenseCostCenter.code} onChange={(e) => this.onInputChange(e.target.value, 'expenseCostCenter.code')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.expenseCostCenter.code })} />
                    {this.state.submitted && !this.state.entity.expenseCostCenter.code && <small className="p-invalid">Codul este obligatoriu!</small>}
                    <label htmlFor="code">Cod*</label>
                </span> 
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbActive" checked={this.state.entity.expenseCostCenter.active} onChange={(e) => this.onInputChange(e.checked, 'expenseCostCenter.active')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbActive" >Activ</label>                   
                </div>
            </Dialog>
        );
    }
} 

export class ListExpenseCostCenter extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeleteExpenseCostCenter";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetAllExpenseCostCenter');

        this.setState({ list: list });
    } 
    save(entity) {
        var index = this.state.list.findIndex(p => p.expenseCostCenter.costCenterId == entity.expenseCostCenter.costCenterId);
        super.save(entity,index);  
    } 

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Centru de cost</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="expenseCostCenter.name" header="Nume" sortable filter ></Column> 
                            <Column field="expenseCostCenter.code" header="Cod" sortable filter ></Column> 
                            <Column field="expenseCostCenter.active" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>                          
                        </DataTable>
                        <ExpenseCostCenter entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                         <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.name}</b> ?</span>
                            </div>
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListExpenseCostCenter;
