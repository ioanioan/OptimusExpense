import React, { Component } from 'react';
import { InputText } from 'primereact/inputtext';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import { InputTextarea } from 'primereact/inputtextarea';
import { Link, Redirect } from 'react-router-dom';
import { ListComponent } from '../base/ListComponent';

import { param } from 'jquery';
export class ListExpenseReport extends ListComponent {
    constructor(props) {

        super(props);
        this.methodDelete = "Expense/DeleteExpenseReport";

    }
    componentDidMount() {
        this.populate();
        
    }
    async populate() {
        var list = await authService.fetchAuth('Expense/GetListExpenseReport', { NrRows: 0 });
        this.setState({ list: list });
        const query = new URLSearchParams(this.props.location.search);
        if (query.get("id") != null && query.get("id") != "") {
            this.state.dt.current.filter(query.get("id"), 'document.number', 'contains');
        }
        
        if (this.props.location.statusName != null && this.props.location.statusName!="")
            this.state.dt.current.filter(this.props.location.statusName, 'statusName', 'contains');
    }

    async print(rowData) {
        await authService.downloadAuth("Report/GetExpenseReport/" + rowData.expenseReport.expenseReportId);
    }

    showApprove(rowData) {
        rowData.approveDescription = "";
        this.setState({ approveDialog: true, obj: { ...rowData } });
    }
    approve() {
        this.approveType('approve');
    }
    canceled() {
        this.approveType('canceled');
    }

    async approveType(type) {
       
        this.state.obj.approveType = type;
        var r = await authService.fetchAuth("Expense/SaveExpenseReport", this.state.obj);
        var index = this.state.list.findIndex(p => p.expenseReport.expenseReportId == r.expenseReport.expenseReportId);

        super.save(r, index);
        this.hideApproveDialog();
    }

    actionBodyTemplateDetail(rowData) {
        return (
            <React.Fragment>
                {rowData.expense.filePath != null && <Link to={"/" + rowData.expense.filePath} target="_blank"  ><Button icon="pi pi-download" className="p-button-rounded p-button-info" /></Link>}
            </React.Fragment>
        );
    }

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>
                <Link   to={{ pathname: "ExpenseReport", expenseReport: rowData  }}> <Button   icon="pi pi-pencil" className="p-button-rounded p-button-success p-mr-2" /></Link>
                <Button icon="pi pi-trash" disabled={!rowData.enabled} className="p-button-rounded p-button-danger p-mr-2" onClick={() => this.confirmDelete(rowData)} />
                <Button icon="pi pi-print" className="p-button-rounded p-button-help p-mr-2" onClick={() => this.print(rowData)} />

                {rowData.enabledV && <Button icon="pi pi-check" className="p-button-rounded p-button-primary" onClick={() => this.showApprove(rowData)} />
                }
            </React.Fragment>
        );
    }
     
    rowExpansionTemplate = (data) => {

        return (
            <div className="orders-subtable">

                <DataTable header="Lista cheltuieli" value={data.selectedExpense} >
                    <Column body={this.actionBodyTemplateDetail.bind(this)} headerStyle={{ width: '4rem' }}></Column>
                    <Column field="expense.date" header="Data" body={this.dateBodyTemplate} ></Column>
                    <Column field="expense.description" header="Descriere" ></Column>
                    <Column field="expense.price" header="Suma" ></Column>
                </DataTable>
            </div>
        );
    }


    approveDialogFooter = (
        <React.Fragment>
            <Button label="Respinge" icon="pi pi-times" className="p-button-danger" onClick={this.canceled.bind(this)} />
            <Button label="Aproba " icon="pi pi-check" className="p-button-primary" onClick={this.approve.bind(this)} />
        </React.Fragment>
    );

    hideApproveDialog() {
        this.setState({ approveDialog: false });
    }
    leftToolbarTemplate() {
        return (
            <React.Fragment>
                <Link to="ExpenseReport"> <Button label="Adauga" icon="pi pi-plus" className="p-button-success p-mr-2" /></Link>
            </React.Fragment>
        )
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Deconturi</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            rowExpansionTemplate={this.rowExpansionTemplate}
                            expandedRows={this.state.expandedRows}
                            onRowToggle={(e) => this.setState({ expandedRows: e.data })}
                            removableSort className="p-datatable-sm p-datatable-gridlines" dataKey="expenseReport.expenseReportId">
                            <Column expander style={{ width: '3em' }} />
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '12rem' }}></Column>

                            <Column field="document.date" header="Data" body={this.monthBodyTemplate} sortable filter ></Column>
                            <Column field="document.number" header="Numar" sortable filter></Column>
                            <Column field="expenseReport.description" header="Descriere" sortable filter></Column>
                            <Column field="sumaDecont" header="Suma" sortable filter ></Column>
                            <Column field="numeAngajat" header="Angajat" sortable filter ></Column>
                            <Column field="plAngajat" header="Punct lucru" sortable filter ></Column>
                            <Column field="statusName" header="Status" sortable filter f></Column>
                            {/* <Column field="obsStatus" header="Observatii" sortable filter f></Column> */}
                        </DataTable>
                        <Dialog className="p-fluid" visible={this.state.approveDialog} style={{ width: '450px' }} header="Aproba/Respinge decont" modal  footer={this.approveDialogFooter} onHide={this.hideApproveDialog.bind(this)}>                 
                            <span className="p-float-label mt-4">
                                <InputTextarea id="approveDescription" value={this.state.obj.approveDescription}  onChange={(e) => {  this.onInputChange(e.target.value, 'approveDescription') }}  />
                                <label htmlFor="approveDescription">Observatii</label>
                            </span>
                        </Dialog>
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                        <div className="confirmation-content">
                            <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                            <span>Sunteti sigur ca doriti sa stergeti?</span>
                            </div>
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}

export default ListExpenseReport;