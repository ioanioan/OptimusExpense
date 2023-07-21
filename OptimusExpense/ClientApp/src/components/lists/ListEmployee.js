import React, { Component, createRef   } from 'react';
import { InputText } from 'primereact/inputtext';
import { Calendar } from 'primereact/calendar';
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
import Moment from 'react-moment';

export class Employee extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.employee = {};
        this.methodSave = "Lists/SaveEmployee";
    }
    async save() {
        var isSave = this.state.entity.employee.employeeId && this.state.entity.employee.partnerPointId;
        await super.save(isSave);
    }
    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listPerson = await authService.fetchAuth('Lists/GetAllPersons');                  
        var listStatus = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-4');
        var listCostCenter = await authService.fetchAuth('Lists/GetAllExpenseCostCenter');

        this.setState({ listPerson: listPerson, listStatus: listStatus, listCostCenter: listCostCenter });
    }

    componentDidUpdate(prevProps) {
        if (this.props.entity.employee == null) {
            this.props.entity.employee = {};
        }
        if (prevProps.entity.employee !== this.props.entity.employee) {
            if (this.props.entity.employee.hireDate != null)
                this.props.entity.employee.hireDate = new Date(this.props.entity.employee.hireDate);
            if (this.props.entity.employee.employeeId > 0)
                this.populatePartnerPoint(this.props.entity.employee.employeeId);
            if (this.state.listStatus != null && this.state.listStatus.length > 0 && this.props.entity.employee.statusId == null) {
                this.props.entity.employee.statusId = this.state.listStatus[0].dictionaryDetailId;
            }
        }
     
        super.componentDidUpdate(prevProps);
    }

    async populatePartnerPoint(id) {
        var listPartnerPoint = await authService.fetchAuth('Lists/GetPartnerPointsByPersonId/' + id);
        this.setState({ listPartnerPoint: listPartnerPoint });
    }
     
    render() {     
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Angajat" modal className="p-fluid" onHide={this.hide}>             
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.employeeId} options={this.state.listPerson} onChange={(e) => { this.onInputChange(e.value, 'employee.employeeId', 'employeeName',e); this.populatePartnerPoint(e.value); }} optionLabel="fullName" optionValue="person.personId" />
                    {this.state.submitted && !this.state.entity.employee.employeeId && <small className="p-invalid">Persoana este obligatorie!</small>}
                    <label htmlFor="employeeId">Persoana*</label>
                </span>
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.partnerPointId} options={this.state.listPartnerPoint} onChange={(e) => this.onInputChange(e.value, 'employee.partnerPointId', 'partnerPointName',e)} optionLabel="name" optionValue="partnerPointId" />
                    {this.state.submitted && !this.state.entity.employee.partnerPointId && <small className="p-invalid">Punctul de lucru este obligatoriu!</small>}
                    <label htmlFor="partnerPointId">Punct de lucru*</label>
                </span>   
                <div className="p-grid">
                    <div className="p-col-5">
                <span className="p-float-label mt-4">                   
                    <InputText id="phone" value={this.state.entity.employee.phone} onChange={(e) => this.onInputChange(e.target.value, 'employee.phone')} />                   
                    <label htmlFor="phone">Telefon</label>
                </span> </div> 
                    <div className="p-col-7">
                <span className="p-float-label mt-4">
                    <InputText id="email" value={this.state.entity.employee.email} onChange={(e) => this.onInputChange(e.target.value, 'employee.email')} />                   
                    <label htmlFor="email">Email</label>
                        </span>
                    </div>
                </div>
                <span className="p-float-label mt-4">
                    <Calendar value={this.state.entity.employee.hireDate} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'employee.hireDate')} showIcon showButtonBar ></Calendar>
                    <label >Data angajare</label>
                </span>
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.superiorEmployeeId} options={this.state.listPerson} onChange={(e) => this.onInputChange(e.value, 'employee.superiorEmployeeId', 'SuperiorName',e)} optionLabel="fullName" optionValue="person.personId" />
                    <label htmlFor="superiorEmployeeId">Superior</label>
                </span> 
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.accountingEmployeeId} options={this.state.listPerson} onChange={(e) => this.onInputChange(e.value, 'employee.accountingEmployeeId', 'AccountingName',e)} optionLabel="fullName" optionValue="person.personId" />
                    <label htmlFor="accountingEmployeeId">Contabil</label>
                </span> 
                <span className="p-float-label mt-4">
                    <InputText id="marca" value={this.state.entity.employee.marca} onChange={(e) => this.onInputChange(e.target.value, 'employee.marca')} />
                    <label htmlFor="marca">Marca</label>
                </span> 
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.costCenterId} options={this.state.listCostCenter} onChange={(e) => this.onInputChange(e.value, 'employee.costCenterId', 'CostCenterName', e)} optionLabel="cboxName" optionValue="expenseCostCenter.costCenterId" />
                    <label htmlFor="costCenterId">Centru de cost</label>
                </span> 
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.employee.statusId} options={this.state.listStatus} onChange={(e) => this.onInputChange(e.value, 'employee.statusId', 'StatusName',e)} optionLabel="name" optionValue="dictionaryDetailId" />
                    <label htmlFor="statusId">Status</label>
                </span>                              
            </Dialog>
        );
    }
} 

export class ListEmployee extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeleteEmployee";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetAllEmployees');

        this.setState({ list: list });
    } 
    edit(rowData) {      
        super.edit(rowData);
    }
    save(entity) {
        var index = this.state.list.findIndex(p => p.employee.employeeId == entity.employee.employeeId);
        super.save(entity, index);  
    } 
    statusBodyTemplate = (rowData) => {
        return <center><span className={`status-badge status-${('' + rowData.statusName).toLowerCase()}`}>{rowData.statusName}</span></center>;
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Angajat</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="employeeName" header="Angajat" sortable filter ></Column>       
                            <Column field="partnerPointName" header="Punct lucru" sortable filter ></Column>  
                            <Column field="employee.phone" header="Telefon" sortable filter ></Column>  
                            <Column field="employee.email" header="Email" sortable filter ></Column>
                            <Column field="employee.hireDate" header="Data angajare" sortable filter body={this.dateBodyTemplate} ></Column>
                            <Column field="superiorName" header="Superior" sortable filter ></Column>
                            <Column field="accountingName" header="Contabil" sortable filter ></Column>
                            <Column field="employee.marca" header="Marca" sortable filter ></Column>
                            <Column field="costCenterName" header="Centru de cost" sortable filter ></Column>
                            <Column field="statusName" header="Status" sortable filter body={this.statusBodyTemplate} headerStyle={{ width: '7rem' }}></Column>
                        </DataTable>
                        <Employee entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                        <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.employeeName}</b> ?</span>
                        </div>    
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListEmployee;
