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

export class ExpenseNature extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.expenseNature = {};
        this.methodSave = "Lists/SaveExpenseNature";
    }
    async save() {
        var isSave = this.state.entity.expenseNature.name;
        await super.save(isSave);
    }

    componentDidMount() {
        this.populate();
    }

    componentDidUpdate(prevProps) {
        if (this.props.entity.expenseNature == null) {
            this.props.entity.expenseNature = {};
        }
        if (prevProps.entity !== this.props.entity) {
            if (this.state.listTVA != null && this.state.listTVA.length > 0 && this.props.entity.expenseNature.vatId == null) {
                this.props.entity.expenseNature.vatId = this.state.listTVA[0].dictionaryDetailId;
            }
        }
        super.componentDidUpdate(prevProps);
    }

    async populate() {
        var listTVA = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-3'); //VAT
        this.setState({ listTVA: listTVA });
    }

    render() {     
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Natura cheltuiala" modal className="p-fluid" onHide={this.hide}>             
                <span className="p-float-label mt-4">
                    <InputText id="name" value={this.state.entity.expenseNature.name} onChange={(e) => this.onInputChange(e.target.value, 'expenseNature.name')} autoFocus className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.expenseNature.name })} />
                    {this.state.submitted && !this.state.entity.expenseNature.name && <small className="p-invalid">Numele este obligatoriu!</small>}
                    <label htmlFor="name">Nume*</label>
                </span> 
                <span className="p-float-label mt-4">
                    <InputText id="contC" value={this.state.entity.expenseNature.contContabil} onChange={(e) => this.onInputChange(e.target.value, 'expenseNature.contContabil')} />                   
                    <label htmlFor="contC">Cont contabil</label>
                </span> 
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.expenseNature.vatId} options={this.state.listTVA} onChange={(e) => this.onInputChange(e.value, 'expenseNature.vatId', 'vat', e)} optionLabel="name" optionValue="dictionaryDetailId" />
                    <label htmlFor="vat">TVA</label>
                </span>
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbActive" checked={this.state.entity.expenseNature.active} onChange={(e) => this.onInputChange(e.checked, 'expenseNature.active')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbActive" >Activ</label>                   
                </div>
            </Dialog>
        );
    }
} 

export class ListExpenseNature extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeleteExpenseNature";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetExpenseNature');

        this.setState({ list: list });
    } 
    save(entity) {
        var index = this.state.list.findIndex(p => p.expenseNature.expenseNatureId == entity.expenseNature.expenseNatureId);
        super.save(entity,index);  
    } 

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Natura cheltuiala</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="expenseNature.name" header="Nume" sortable filter ></Column> 
                            <Column field="expenseNature.contContabil" header="Cont contabil" sortable filter ></Column> 
                            <Column field="vat" header="TVA" sortable filter headerStyle={{ width: '7rem' }}></Column>
                            <Column field="expenseNature.active" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>                          
                        </DataTable>
                        <ExpenseNature entity={this.state.obj} onSave={this.save} />
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
export default ListExpenseNature;
