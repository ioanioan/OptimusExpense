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
export class ExpenseReport extends ListComponent {

    constructor(props) {

        super(props);
        this.toast = createRef();
        if (props.location.expenseReport != null) {

            props.location.expenseReport.document.date = new Date(props.location.expenseReport.document.date);

            this.state.entity = props.location.expenseReport;
        }
        else {
            this.state.entity = { document: { date: new Date() }, expenseReport: {}, selectedExpense: [] };
        }
        this.state.objMisiune = {};
        this.state.wizardItems = [
            { label: 'Creare' },
            { label: 'Depunere' }
        ];
        this.schimbarePerioada = this.schimbarePerioada.bind(this);
        this.methodDelete = "Expense/DeleteExpense";
    }

    componentDidMount() {
        this.populate();
    }
    activeIndex() {

        return this.state.entity.selectedExpense.length > 0 && this.state.entity.document.date != null && this.state.entity.expenseReport.description != null && ("" + this.state.entity.expenseReport.description) != "" ? 1 : 0;
    }
    async populate() {
        var list = await authService.fetchAuth('Expense/GetListExpenseDraft', { Date: moment(this.state.entity.document.date).format(), Id: this.state.entity.document.documentId });
       
        this.setState({ list: list, entity: this.state.entity });
    }

    adaugaMisiune() {
        this.setState({
            objMisiune: { edit: true },
        });
    }

    saveMisiune() {

    }

    save(entity) {

        var dt = new Date(entity.expense.date);
        var dtReport = new Date(this.state.entity.document.date);

        if (dtReport.getMonth() != dt.getMonth() || dtReport.getYear() != dt.getYear())
            this.toast.current.show({ life: 20000, severity: 'warn', summary: 'Adaugare cheltuiala', detail: 'Data cheltuielii nu e in aceeasi luna cu data decontului. Selectati perioada din luna cheltuielii pentru a se incarca cheltuiala adaugata!' });

        var index = this.state.list.findIndex(p => p.expense.expenseId == entity.expense.expenseId);
        super.save(entity, index);
    }

    schimbarePerioada(data) {
        var dtReport = new Date(this.state.entity.document.date);
        if (data.getMonth() != dtReport.getMonth() || data.getYear() != dtReport.getYear()) {

            this.state.entity.document.date = data;
             //  this.state.entity.selectedExpense = [];
            this.populate();
        }
    }


    saveReportSend() {
        this.state.entity.approveType = "send";
      
        this.saveReport();
    }

    async saveReport() {
        try {
            this.state.entity.document.date = moment(this.state.entity.document.date).format();
            var expenseReport = await authService.fetchAuth('Expense/SaveExpenseReport', this.state.entity);
            this.setState({ redirect: "/expense/ListExpenseReport" });
        }
        catch (ex) {
            this.state.entity.approveType = null;
            this.alertError(ex);
        }
    }

    isAprobator() {
        return this.isEdit() && (this.state.entity.document.statusId == null || this.state.entity.document.statusId == -38 || this.state.entity.document.statusId ==-57|| this.state.entity.document.statusId==-59);
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
                    <div className="card card-w-title">
                        <h3>Creati o noua cerere de decont</h3>
                        <Steps model={this.state.wizardItems} readOnly={true} activeIndex={this.activeIndex()} />
                        <div className="p-grid">
                            <div className="p-col-12 p-md-8">
                               
                                <span className="p-float-label mt-4">
                                    <Calendar id="perioada" disabled={true} value={this.state.entity.document.date} view="month" yearNavigator yearRange="2015:2040" dateFormat="mm.yy" onChange={(e) => { this.schimbarePerioada(e.value);this.onInputChange(e.value, 'document.date');}} showIcon showButtonBar ></Calendar>

                                    <label htmlFor="perioada">Perioada*</label>
                                </span>
                                
                                <span className="p-float-label mt-4">
                                    <InputTextarea id="descriere" autoResize rows="3" onChange={(e) => this.onInputChange(e.target.value, 'expenseReport.description')} cols="30" value={this.state.entity.expenseReport.description} />
                                    <label htmlFor="descriere">Descriere*</label>
                                </span>
                            </div>
                            <div className="p-col-12 p-md-4">
                                {/*<Button label="Adaugati o noua misiune/serviciu" className="p-button-rounded p-button-secondary" onClick={this.adaugaMisiune.bind(this)}/>
                                 */}

                                {(this.isAprobator()) && <Button label="Adaugati o cheltuiala noua" className="p-button-rounded " onClick={this.add} />}
                                <div className="p-col-12 p-md-8"></div>
                                {(this.isEdit()) &&
                                    <Button label="Salveaza cererea decont" className="p-button-rounded p-button-info" disabled={this.activeIndex() < 1} onClick={this.saveReport.bind(this)} />
                                }
                                    <div className="p-col-12 p-md-8"></div>
                                
                                {(this.isAprobator() ) &&< Button label="Depune cererea decont" className="p-button-rounded p-button-success" disabled={this.activeIndex() < 1} onClick={this.saveReportSend.bind(this)} />}
                            </div>                            
                            <div className="p-col-12 p-md-12">

                                <DataTable header="Lista cheltuieli" dataKey="expense.expenseId" removableSort resizableColumns reorderableColumns   scrollable scrollHeight="250px" selection={this.state.entity.selectedExpense} onSelectionChange={e => { this.state.entity.selectedExpense = e.value; this.setState({ entity: this.state.entity }) }} value={this.state.list} removableSort className="p-datatable-sm p-datatable-gridlines">
                                     
                                    <Column selectionMode="multiple" headerStyle={{ width: '3em' }}></Column>
                                    <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                                    <Column field="expense.date" header="Data" sortable filter body={this.dateBodyTemplate} ></Column>
                                    <Column field="expense.description" header="Descriere" sortable filter ></Column>
                                    <Column field="expenseNature" header="Natura" sortable filter ></Column>
                                    <Column field="expenseDocumentType" header="Tip document" sortable filter ></Column>
                                    <Column field="expense.price" header="Suma" sortable filter ></Column>
                                    <Column field="provider" header="Partener(CIF)" sortable filter ></Column>
                                    <Column field="expense.fiscalCode" header="Cod fiscal" sortable filter ></Column>
                                    <Column field="expenseProject" header="Misiune/serviciu" sortable filter ></Column>
                                </DataTable>
                                <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                                <div className="confirmation-content">
                                        <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                        <span>Sunteti sigur ca doriti sa stergeti?</span>
                                    </div>
                                </Dialog>
                                <Expense entity={this.state.obj} onSave={this.save} key="expenseKey" enabled={this.state.entity.enabled} />
                                <ExpenseProject entity={this.state.objMisiune} onSave={this.saveMisiune} key="expenseProjectKey" />                             
                            </div>
                        </div>
                    </div>
                </div>
                </div>
          
        );
    }
}

export default ExpenseReport;