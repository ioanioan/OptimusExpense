import React, { Component, createRef   } from 'react';
import { InputText } from 'primereact/inputtext';
import { Calendar } from 'primereact/calendar';

import { InputTextarea } from 'primereact/inputtextarea';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import Moment from 'react-moment';
import { Checkbox } from 'primereact/checkbox';
import { ListComponent } from "../base/ListComponent";
import { EditComponent } from "../base/EditComponent";

export class ExpenseProject extends EditComponent {
    constructor(props) {
        super(props);
        this.methodSave = "Lists/SaveProject";       
    }  
    async save() {
        var isSave = this.state.entity.name;
        await super.save(isSave);
    }
    componentDidUpdate(prevProps) {
        if (prevProps.entity !== this.props.entity) {
            if (this.props.entity.valabilityStartDate != null)
                this.props.entity.valabilityStartDate = new Date(this.props.entity.valabilityStartDate);
            if (this.props.entity.valabilityEndDate != null)
                this.props.entity.valabilityEndDate = new Date(this.props.entity.valabilityEndDate);
        
        }
        super.componentDidUpdate(prevProps);
    }
    render() {    
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Misiune/serviciu" modal className="p-fluid" onHide={this.hide.bind(this)}>
                <span className="p-float-label mt-4">                    
                    <InputText id="name" value={this.state.entity.name} onChange={(e) => this.onInputChange(e.target.value, 'name')}   autoFocus className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.name })} />
                    {this.state.submitted && !this.state.entity.name && <small className="p-invalid">Numele este obligatoriu.</small>}
                    <label htmlFor="name">Nume*</label>
                </span>
                <span className="p-float-label mt-4">                     
                    <Calendar value={this.state.entity.valabilityStartDate} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'valabilityStartDate')} showIcon showButtonBar ></Calendar>
                    <label >Data inceput</label>
                </span>
                <span className="p-float-label mt-4">                     
                    <Calendar value={this.state.entity.valabilityEndDate} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'valabilityEndDate')} showIcon showButtonBar ></Calendar>
                    <label >Data sfarsit</label>
                </span>
                <div className="p-field-checkbox mt-4">
                    <Checkbox inputId="cbActive" checked={this.state.entity.active} onChange={(e) => this.onInputChange(e.checked, 'active')}></Checkbox>
                    <label className="p-checkbox-label" htmlFor="cbActive" >Activ</label>
                   
                </div>
            </Dialog>
        );
    }
} 

export class ListExpenseProject extends ListComponent {
    constructor(props) {
        super(props);
        this.methodDelete = "Lists/DeleteProject";
    }
  
    componentDidMount() {
        this.populateProjects();
    }
    async populateProjects() {       
        var listExpenseProjects = await authService.fetchAuth('Lists/GetExpenseProjects');    
        this.setState({ list: listExpenseProjects });
    }

   
    save(entity) {
        var index = this.state.list.findIndex(p => p.expenseProjectId == entity.expenseProjectId);
      
        super.save(entity, index);
    } 

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Misiuni/Servicii</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">

                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="name" header="Nume" sortable filter ></Column>
                            <Column field="valabilityStartDate" header="Data inceput" sortable filter body={this.dateBodyTemplate } ></Column>
                            <Column field="valabilityEndDate" header="Data sfarsit" sortable filter body={this.dateBodyTemplate}></Column>                           
                            <Column field="active" header="Activ" sortable filter body={this.checkBodyTemplate} headerStyle={{ width: '7rem' }} ></Column>
                           
                        </DataTable>
                        <ExpenseProject entity={this.state.obj} onSave={this.save} />
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
export default ListExpenseProject;

 

