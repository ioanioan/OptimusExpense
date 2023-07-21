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

export class DictionaryDetail extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.dictionaryDetail = {};
        this.methodSave = "Lists/SaveDictionaryDetail";
    }
    async save() {
        var isSave = this.state.entity.dictionaryDetail.name && this.state.entity.dictionaryDetail.code && this.state.entity.dictionaryDetail.dictionaryId;
        await super.save(isSave);
    }
    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listDictonary = await authService.fetchAuth('Lists/GetDictionary');        
        this.setState({ listDictonary: listDictonary });
    }
    render() {     
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Dictionar detaliu" modal className="p-fluid" onHide={this.hide}>             
                <span className="p-float-label mt-4">
                    <InputText id="name" value={this.state.entity.dictionaryDetail.name} onChange={(e) => this.onInputChange(e.target.value, 'dictionaryDetail.name')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.dictionaryDetail.name })} />
                    {this.state.submitted && !this.state.entity.dictionaryDetail.name && <small className="p-invalid">Numele este obligatoriu!</small>}
                    <label htmlFor="name">Nume*</label>
                </span>           
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.dictionaryDetail.dictionaryId}                     
                        options={this.state.listDictonary}
                        onChange={(e,target) =>
                            this.onInputChange(e.value, 'dictionaryDetail.dictionaryId', 'numeDictionar',e)
                        }
                    optionLabel="name"
                    optionValue="dictionaryId" />
                    {this.state.submitted && !this.state.entity.dictionaryDetail.dictionaryId && <small className="p-invalid">Dictionarul este obligatoriu!</small>}
                    <label htmlFor="dictionaryId">Dictionar*</label>
                </span>                 
                <span className="p-float-label mt-4">
                    <InputText id="code" value={this.state.entity.dictionaryDetail.code} onChange={(e) => this.onInputChange(e.target.value, 'dictionaryDetail.code')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.dictionaryDetail.code })} />
                    {this.state.submitted && !this.state.entity.dictionaryDetail.code && <small className="p-invalid">Codul este obligatoriu!</small>}
                    <label htmlFor="code">Cod*</label>
                </span>                
                <span className="p-float-label mt-4">                   
                    <InputText id="displayOrder" value={this.state.entity.dictionaryDetail.displayOrder} onChange={(e) => this.onInputChange(e.target.value, 'dictionaryDetail.displayOrder')} />                   
                    <label htmlFor="displayOrder">Ordine</label>
                </span>                
                <span className="p-float-label mt-4">
                    <InputText id="value" value={this.state.entity.dictionaryDetail.value} onChange={(e) => this.onInputChange(e.target.value, 'dictionaryDetail.value')} />                   
                    <label htmlFor="value">Valoare</label>
                </span>              
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbActive" checked={this.state.entity.dictionaryDetail.active} onChange={(e) => this.onInputChange(e.checked, 'dictionaryDetail.active')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbActive" >Activ</label>                   
                </div>
            </Dialog>
        );
    }
} 

export class ListDictionaryDetail extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeleteDictionaryDetail";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetDictionaryDetail');

        this.setState({ list: list });
    } 
    edit(rowData) {      
        super.edit(rowData);
    }
    save(entity) {
        var index = this.state.list.findIndex(p => p.dictionaryDetail.dictionaryDetailId == entity.dictionaryDetail.dictionaryDetailId);
        super.save(entity, index);  
    } 
    actionBodyTemplate(rowData) {
        if (rowData.dictionaryDetail == null) {
            var a = "";
        }
        return (
            <React.Fragment>
                <Button disabled={rowData.dictionaryDetail.dictionaryDetailId<0} icon="pi pi-pencil" className="p-button-rounded p-button-success p-mr-2" onClick={() => { this.edit(rowData) }} />
                <Button disabled={rowData.dictionaryDetail.dictionaryDetailId < 0} icon="pi pi-trash" className="p-button-rounded p-button-danger" onClick={() => this.confirmDelete(rowData)} />
            </React.Fragment>
        );
    }
    checkBodyTemplate(rowData, column) {
        return super.checkBodyTemplate(rowData.dictionaryDetail != null ? rowData.dictionaryDetail : {}, column);
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Dictionar detaliu</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="dictionaryDetail.name" header="Nume" sortable filter ></Column>     
                            <Column field="numeDictionar" header="Dictionar" sortable filter ></Column>  
                            <Column field="dictionaryDetail.code" header="Cod" sortable filter ></Column>  
                            <Column field="dictionaryDetail.displayOrder" header="Ordine afisare" sortable filter ></Column>  
                            <Column field="dictionaryDetail.value" header="Valoare" sortable filter ></Column>  
                            <Column field="active" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>                          
                        </DataTable>
                        <DictionaryDetail entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                         <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.dictionaryDetail!=null&&this.state.rowData.dictionaryDetail.name}</b> ?</span>
                            </div>
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListDictionaryDetail;
