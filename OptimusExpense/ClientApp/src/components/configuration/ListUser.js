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
import { Password } from 'primereact/password';


export class EditUser extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.aspNetUsers = {};
        
        this.methodSave = "Configuration/SaveUser";
        this.hideH = this.hideH.bind(this);
    }
    async save() {
        var isSave = this.state.entity.aspNetUsers.userName && this.state.entity.aspNetUsers.email && this.state.entity.roleId;
        await super.save(isSave);
    }

    hideH() {
        this.setState({ edit1: false });
    }
    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listEmployee = await authService.fetchAuth('Lists/GetAllEmployees');
        var listRoles = await authService.fetchAuth('Configuration/GetAspNetRoles');
        
        this.setState({ listEmployee: listEmployee, listRoles: listRoles });
    }
    render() {
        return (     
           
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '600px' }} header="Utilizator" modal className="p-fluid" onHide={this.hide}>

                <span className="p-float-label mt-4">
                    <InputText id="userName" value={this.state.entity.aspNetUsers.userName} onChange={(e) => this.onInputChange(e.target.value, 'aspNetUsers.userName')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.aspNetUsers.userName })} />
                    {this.state.submitted && !this.state.entity.aspNetUsers.userName && <small className="p-invalid">Utilizator este obligatoriu!</small>}
                    <label htmlFor="userName">Utilizator</label>
                </span>
                <span className="p-float-label mt-4">
                    <InputText id="email" value={this.state.entity.aspNetUsers.email} onChange={(e) => this.onInputChange(e.target.value, 'aspNetUsers.email')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.aspNetUsers.email })} />
                    {this.state.submitted && !this.state.entity.aspNetUsers.email && <small className="p-invalid">Email este obligatoriu!</small>}
                    <label htmlFor="email">Email</label>
                </span>
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.aspNetUsers.employeeId}
                        options={this.state.listEmployee}
                        onChange={(e, target) =>
                            this.onInputChange(e.value, 'aspNetUsers.employeeId', 'employee', e)
                        }
                        optionLabel="employeeName"
                        optionValue="employee.employeeId" />
                    {this.state.submitted && !this.state.entity.aspNetUsers.employeeId && <small className="p-invalid">Angajatul este obligatoriu!</small>}
                    <label htmlFor="employeeId">Angajat</label>
                </span>
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.entity.roleId}
                        options={this.state.listRoles}
                        onChange={(e, target) =>
                            this.onInputChange(e.value, 'roleId', 'roleName', e)
                        }
                        optionLabel="name"
                        optionValue="id" />
                    {this.state.submitted && !this.state.entity.roleId && <small className="p-invalid">Rolul este obligatoriu!</small>}
                    <label htmlFor="roleId">Rol</label>
                </span>
                 
                <span className="p-float-label mt-4">
                    <Password  id="password" value={this.state.entity.password} onChange={(e) => this.onInputChange(e.target.value, 'password')} />
                    <label htmlFor="password">Parola</label>
                </span>
                <span className="p-float-label mt-4">
                    <Password  id="confirmPassword" value={this.state.entity.confirmPassword} onChange={(e) => this.onInputChange(e.target.value, 'confirmPassword')} />
                    <label htmlFor="confirmPassword">Confirma parola</label>
                </span>
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbActive" checked={this.state.entity.aspNetUsers.active} onChange={(e) => this.onInputChange(e.checked, 'aspNetUsers.active')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbActive" >Activ</label>
                </div>
            </Dialog>
        );
    }
}

export class ListUser extends ListComponent {
    constructor(props) {

        super(props);
        this.methodDelete = "Lists/DeleteDictionaryDetail";
    }
    componentDidMount() {
       
        this.populate();
        
    }
    async populate() {
        var list = await authService.fetchAuth('Configuration/GetUsers');

        this.setState({ list: list });
    }
  
    save(entity) {
        var index = this.state.list.findIndex(p => p.aspNetUsers.id == entity.aspNetUsers.id);
 
        super.save(entity, index);
    }

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>
                <Button icon="pi pi-pencil" className="p-button-rounded p-button-success p-mr-2" onClick={() => { this.edit(rowData) }} />
            </React.Fragment>
        );
    }
    

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Utilizatori</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '3.5rem' }}></Column>
                            <Column field="aspNetUsers.userName" header="Utilizator" sortable filter ></Column>
                            <Column field="aspNetUsers.email" header="Email" sortable filter ></Column>
                            <Column field="roleName" header="Rol" sortable filter ></Column>
                            <Column field="employee" header="Angajat" sortable filter ></Column>
                            <Column field="aspNetUsers.createdDate" header="Data creare" sortable filter body={this.dateBodyTemplate} ></Column>
                            <Column field="aspNetUsers.lastLogin" header="Ultima autentificare" sortable filter body={this.dateBodyTemplate} ></Column>
                            <Column field="aspNetUsers.active" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>

                        </DataTable>
                        <EditUser entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                            <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.dictionaryDetail != null && this.state.rowData.dictionaryDetail.name}</b> ?</span>
                            </div>
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListUser;
