import React, { Component } from 'react';
import { InputText } from 'primereact/inputtext';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import { RadioButton } from 'primereact/radiobutton';
import { InputTextarea } from 'primereact/inputtextarea';
import { InputNumber } from 'primereact/inputnumber';
import { Dropdown } from 'primereact/dropdown';
import { Link, Redirect } from 'react-router-dom';
import { ListComponent } from '../base/ListComponent';
import { EditComponent } from '../base/EditComponent';

export class ExpenseAdvance extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.expenseAdvance = {};
        this.methodSave = "Expense/SaveExpenseAdvance";
    }
    async save() {
        var isSave = this.state.entity.expenseAdvance.description && this.state.entity.expenseAdvance.amount;
        await super.save(isSave);
    }

    componentDidMount() {
        this.populate();
    }

    componentDidUpdate(prevProps) {
        if (this.props.entity.expenseAdvance == null) {
            this.props.entity.expenseAdvance = {};
        }
        if (prevProps.entity !== this.props.entity) {
            if (this.state.listPayMethod != null && this.state.listPayMethod.length > 0 && this.props.entity.expenseAdvance.paymentMethodId == null) {
                this.props.entity.expenseAdvance.paymentMethodId = this.state.listPayMethod[0].dictionaryDetailId;
            }
        }
        super.componentDidUpdate(prevProps);
    }

    async populate() {
        var listExpProj = await authService.fetchAuth('Lists/GetExpenseProjects');
        var listPayMethod = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-15'); //PaymentMethod
        this.setState({ listExpProj: listExpProj, listPayMethod: listPayMethod });
    }

    render() {
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Avans" modal className="p-fluid" onHide={this.hide}>
                <span className="p-float-label mt-4">
                    <InputText id="name" value={this.state.entity.expenseAdvance.description} onChange={(e) => this.onInputChange(e.target.value, 'expenseAdvance.description')} autoFocus className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.expenseAdvance.description })} />
                    {this.state.submitted && !this.state.entity.expenseAdvance.description && <small className="p-invalid">Descrierea este obligatorie!</small>}
                    <label htmlFor="name">Descriere*</label>
                </span>
                <span className="p-float-label mt-4">
                    <InputNumber id="price" value={this.state.entity.expenseAdvance.amount} onChange={(e) => this.onInputChange(e.value, 'expenseAdvance.amount')} autoFocus mode="decimal" minFractionDigits={2} maxFractionDigits={4} />
                    {this.state.submitted && !this.state.entity.expenseAdvance.amount && <small className="p-invalid">Suma este obligatorie!</small>}
                    <label htmlFor="price">Suma*</label>
                </span>
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.expenseAdvance.expenseProjectId} options={this.state.listExpProj} onChange={(e) => this.onInputChange(e.value, 'expenseAdvance.expenseProjectId', 'expenseProject', e)} optionLabel="name" optionValue="expenseProjectId" />
                    <label htmlFor="expProject">Misiune/Serviciu</label>
                </span> 
                <div className="card mt-4">
                    <div className="p-grid">
                        <div className="p-col-4">
                            <div className="p-field-radiobutton">
                                Metoda plata:
                                </div>
                        </div>
                        {this.state.listPayMethod != null && this.state.listPayMethod.map((payMethod) => {
                            return (
                                <div className="p-col-3">
                                    <div key={payMethod.dictionaryDetailId} className="p-field-radiobutton">
                                        <RadioButton inputId={"" + payMethod.dictionaryDetailId} name='payMethod.dictionaryDetailId' value={payMethod.dictionaryDetailId} onChange={(e) => { this.onInputChange(e.value, 'expenseAdvance.paymentMethodId') }} checked={this.state.entity.expenseAdvance.paymentMethodId === payMethod.dictionaryDetailId} />
                                        <i style={{ 'font-size': '1.5rem' }} className={payMethod.dictionaryDetailId == -44 ? "pi pi-fw pi-dollar" : "pi pi-fw pi-credit-card"}></i>
                                    </div>
                                </div>
                            )
                        })
                        }
                    </div>
                </div>
                <span className="p-float-label">
                    <InputTextarea id="obs" autoResize rows="3" cols="30" value={this.state.entity.expenseAdvance.extraInfo} onChange={(e) => this.onInputChange(e.target.value, 'expenseAdvance.extraInfo')} />
                    <label htmlFor="obs">Observatii</label>
                </span>
            </Dialog>
        );
    }
}


export class ListExpenseAdvance extends ListComponent {
    constructor(props) {

        super(props);
        this.methodDelete = "Expense/DeleteExpenseAdvance";

    }
    componentDidMount() {
        this.populate();
        
    }
    async populate() {
        var list = await authService.fetchAuth('Expense/GetListExpenseAdvance', { NrRows: 0 });
        this.setState({ list: list });
        
        if (this.props.location.statusName != null && this.props.location.statusName!="")
            this.state.dt.current.filter(this.props.location.statusName, 'statusName', 'contains');
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

    async send(rowData) {
        rowData.approveType = "send";
        var r = await authService.fetchAuth("Expense/SaveExpenseAdvance", rowData);
        var index = this.findByIndex(r);
        super.save(r, index);
        this.hideApproveDialog();
    }

    async approveType(type) {
       
        this.state.obj.approveType = type;
        var r = await authService.fetchAuth("Expense/SaveExpenseAdvance", this.state.obj);
        var index = this.findByIndex(r);

        super.save(r, index);
        this.hideApproveDialog();
    }

    findByIndex(r) {
        var index = this.state.list.findIndex(p => p.expenseAdvance.expenseAdvanceId == r.expenseAdvance.expenseAdvanceId);
        return index;
    }

    save(entity) {
        var index = this.findByIndex(entity);// this.state.list.findIndex(p => p.expenseAdvance.expenseAdvanceId == entity.expenseAdvance.expenseAdvanceId);
        super.save(entity, index);
    } 

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>      
                <Button icon="pi pi-pencil" disabled={!rowData.enabled || rowData.enabledV} className="p-button-rounded p-button-success p-mr-2" onClick={() => { this.edit(rowData) }} />
                <Button icon="pi pi-trash" disabled={!rowData.enabled || rowData.enabledV} className="p-button-rounded p-button-danger p-mr-2" onClick={() => this.confirmDelete(rowData)} />               
                <Button icon="pi pi-send" disabled={!rowData.enabled ||rowData.enabledV} className="p-button-rounded p-button-warning p-mr-2" onClick={() => this.send(rowData)} />     
                {
                    rowData.enabledV && <Button icon="pi pi-check" className="p-button-rounded p-button-primary" onClick={() => this.showApprove(rowData)} />
                }
            </React.Fragment>
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

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Lista avansuri</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines" dataKey="expenseAdvance.expenseAdvanceId">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '12rem' }}></Column>
                            <Column field="document.number" header="Numar" sortable filter ></Column>
                            <Column field="expenseAdvance.description" header="Descriere" sortable filter ></Column>
                            <Column field="expenseAdvance.amount" header="Suma" sortable filter ></Column>
                            <Column field="expenseProject" header="Misiune/Serviciu" sortable filter f></Column>
                            <Column field="paymentMethod" header="Metoda plata" sortable filter f></Column>
                            <Column field="expenseAdvance.extraInfo" header="Observatii" sortable filter f></Column>
                            <Column field="statusName" header="Status" sortable filter f></Column>
                        </DataTable>
                        <ExpenseAdvance entity={this.state.obj} onSave={this.save} />
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

export default ListExpenseAdvance;