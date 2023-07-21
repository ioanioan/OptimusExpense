import React, { Component, createRef } from 'react';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Button } from 'primereact/button';
import { Steps } from 'primereact/steps';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Calendar } from 'primereact/calendar';
import { ListComponent } from '../base/ListComponent';
import Moment from 'react-moment';
import { Toolbar } from 'primereact/toolbar';
import moment from 'moment';
import authService from '../api-authorization/AuthorizeService';
import { BaseComponent } from '../base/BaseComponent';
import { Expense } from './Expense';
import { ExpenseProject } from '../lists/ListExpenseProject';
import { Redirect } from 'react-router-dom';
import { Toast } from 'primereact/toast';
import { Dialog } from 'primereact/dialog';

import BlockUi from 'react-block-ui';
import 'react-block-ui/style.css';
export class ListExpense extends ListComponent {

    constructor(props) {

        super(props);
        this.toast = createRef();
        this.state.entity = { document: { date: new Date() }, expenseReport: {}, selectedExpense: [] };
        this.state.objMisiune = {};
        this.methodDelete = "Expense/DeleteExpense";
    }

    componentDidMount() {
        this.populate();
    }
    activeIndex() {

        return this.state.entity.selectedExpense.length > 0 && this.state.entity.document.date != null && this.state.entity.expenseReport.description != null && ("" + this.state.entity.expenseReport.description) != "" ? 1 : 0;
    }
    async populate() {
        var list = await authService.fetchAuth('Expense/GetListExpense', {});
        this.setState({ list: list, entity: this.state.entity });
        if (this.props.location.statusName != null && this.props.location.statusName != "")
            this.state.dt.current.filter(this.props.location.statusName, 'statusDecont', 'contains');
    }

    save(entity) {
        var index = this.state.list.findIndex(p => p.expense.expenseId == entity.expense.expenseId);
        super.save(entity, index);
    }

    isAprobator() {
        return this.isEdit() && (this.state.entity.document.statusId == null || this.state.entity.document.statusId == -38 || this.state.entity.document.statusId == -57 || this.state.entity.document.statusId == -59);
    }
    isEdit() {
        return this.state.entity.enabled || this.state.entity.enabled == null;
    }

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>
                <Button icon="pi pi-pencil" className="p-button-rounded p-button-success p-mr-2" onClick={() => { this.edit(rowData) }} />
                <Button icon="pi pi-trash" disabled={!this.isAprobator()} className="p-button-rounded p-button-danger" onClick={() => this.confirmDelete(rowData)} />
            </React.Fragment>
        );
    }


    render() {
        if (this.state.redirect) {
            return <Redirect to={this.state.redirect} />
        }
        return (
            <div className="p-grid p-fluid">
                <Toast ref={this.toast} />
                <div className="p-col-12">
                    <div className="card">
                        <h3>Lista cheltuieli</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                      
                        <DataTable ref={this.state.dt} dataKey="expense.expenseId" removableSort resizableColumns reorderableColumns scrollable scrollHeight="180%" selection={this.state.entity.selectedExpense} onSelectionChange={e => { this.state.entity.selectedExpense = e.value; this.setState({ entity: this.state.entity }) }} value={this.state.list} removableSort className="p-datatable-sm p-datatable-gridlines">
                                    <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                                    <Column field="numeAngajat" header="Angajat" sortable filter ></Column>
                                    <Column field="expense.date" header="Data" sortable filter body={this.dateBodyTemplate} ></Column>
                                    <Column field="expense.description" header="Descriere" sortable filter ></Column>
                                    <Column field="expenseNature" header="Natura" sortable filter ></Column>
                                    <Column field="expenseDocumentType" header="Tip document" sortable filter ></Column>
                                    <Column field="expense.price" header="Suma" sortable filter ></Column>
                                    <Column field="currencyName" header="Moneda" sortable filter ></Column>
                                    <Column field="provider" header="Partener(CIF)" sortable filter ></Column>
                                    <Column field="expense.fiscalCode" header="Cod fiscal" sortable filter ></Column>
                                    <Column field="expenseProject" header="Misiune/serviciu" sortable filter ></Column>
                                    <Column field="dataDecont" header="Data decont" body={this.dateBodyTemplate} ></Column>
                                    <Column field="numarDecont" header="Nr. decont" sortable filter ></Column>
                                    <Column field="statusDecont" header="Status decont" sortable filter ></Column>

                                </DataTable>
                                <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                                    <div className="confirmation-content">
                                        <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                        <span>Sunteti sigur ca doriti sa stergeti?</span>
                                    </div>
                                </Dialog>
                                <Expense entity={this.state.obj} onSave={this.save} key="expenseKey" enabled={this.state.entity.enabled} />                                
                        
                    </div>
                </div>
            </div>
        );
    }
}

export default ListExpense;