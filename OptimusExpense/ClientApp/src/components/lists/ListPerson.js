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

export class Person extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.person = {};
        this.methodSave = "Lists/SavePerson";
    }
    async save() {
        var isSave = this.state.entity.person.firstName && this.state.entity.person.lastName && this.state.entity.person.partnerId;
        await super.save(isSave);
    }
    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listPosition = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-6');
        var listPartner = await authService.fetchAuth('Lists/GetAllPartners');

        this.setState({ listPartner: listPartner, listPosition: listPosition });
    }
    componentDidUpdate(prevProps) {
        if (this.props.entity.person == null) {
            this.props.entity.person = {};
        }
        if (prevProps.entity.person !== this.props.entity.person) {
            if (this.props.entity.person.issueDate != null)
                this.props.entity.person.issueDate = new Date(this.props.entity.person.issueDate);
            if (this.props.entity.person.birthDate != null)
                this.props.entity.person.birthDate = new Date(this.props.entity.person.birthDate);
        }
        super.componentDidUpdate(prevProps);
    }

    render() {
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '450px' }} header="Persoana" modal className="p-fluid" onHide={this.hide}>
                <div className="p-grid">
                    <div className="p-col-12 p-md-12">
                        <span className="p-float-label mt-4">
                            <InputText id="lastName" value={this.state.entity.person.lastName} onChange={(e) => this.onInputChange(e.target.value, 'person.lastName')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.person.lastName })} />
                            {this.state.submitted && !this.state.entity.person.lastName && <small className="p-invalid">Numele este obligatoriu!</small>}
                            <label htmlFor="lastName">Nume*</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="firstName" value={this.state.entity.person.firstName} onChange={(e) => this.onInputChange(e.target.value, 'person.firstName')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.person.firstName })} />
                            {this.state.submitted && !this.state.entity.person.firstName && <small className="p-invalid">Prenumele este obligatoriu!</small>}
                            <label htmlFor="firstName">Prenume*</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="phone" value={this.state.entity.person.phone} onChange={(e) => this.onInputChange(e.target.value, 'person.phone')} />
                            <label htmlFor="phone">Telefon</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="email" value={this.state.entity.person.email} onChange={(e) => this.onInputChange(e.target.value, 'person.email')} />
                            <label htmlFor="email">Email</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.person.partnerId} options={this.state.listPartner} onChange={(e) => this.onInputChange(e.value, 'person.partnerId', 'partnerName', e)} optionLabel="partner.name" optionValue="partner.partnerId" />
                            {this.state.submitted && !this.state.entity.person.partnerId && <small className="p-invalid">Partenerul este obligatoriu!</small>}
                            <label htmlFor="partnerId">Partener*</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.person.positionId} options={this.state.listPosition} onChange={(e) => this.onInputChange(e.value, 'person.positionId', 'positionName', e)} optionLabel="name" optionValue="dictionaryDetailId" />
                            <label htmlFor="positionId">Pozitie/Functie</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Calendar value={this.state.entity.person.birthDate} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'person.birthDate')} showIcon showButtonBar ></Calendar>
                            <label >Data nastere</label>
                        </span>
                    </div>
                    {/*<div className="p-col-12 p-md-6">
                        <span className="p-float-label mt-4">
                            <InputText id="personalNumericCode" value={this.state.entity.person.personalNumericCode} onChange={(e) => this.onInputChange(e.target.value, 'person.personalNumericCode')} autoFocus />
                    <label htmlFor="personalNumericCode">CNP</label>
                </span>  
                <span className="p-float-label mt-4">
                            <InputText id="identityCardSeries" value={this.state.entity.person.identityCardSeries} onChange={(e) => this.onInputChange(e.target.value, 'person.identityCardSeries')} autoFocus />
                    <label htmlFor="identityCardSeries">Serie CI</label>
                </span>  
                <span className="p-float-label mt-4">
                            <InputText id="identityCardNumber" value={this.state.entity.person.identityCardNumber} onChange={(e) => this.onInputChange(e.target.value, 'person.identityCardNumber')} autoFocus />
                    <label htmlFor="identityCardNumber">Numar CI</label>
                </span>  
                <span className="p-float-label mt-4">
                            <InputText id="issuedBy" value={this.state.entity.person.issuedBy} onChange={(e) => this.onInputChange(e.target.value, 'person.issuedBy')} autoFocus />
                    <label htmlFor="issuedBy">Eliberat de</label>
                </span>  
                <span className="p-float-label mt-4">
                            <Calendar value={this.state.entity.person.issueDate} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'person.issueDate')} showIcon showButtonBar ></Calendar>
                    <label >Data eliberare CI</label>
                </span>
                    </div>*/}
                </div>
            </Dialog>
        );
    }
} 

export class ListPerson extends ListComponent {
    constructor(props) {
      
        super(props);   
        this.methodDelete = "Lists/DeletePerson";
    } 
    componentDidMount() {
        this.populate();
    }
    async populate() {       
        var list = await authService.fetchAuth('Lists/GetAllPersons');

        this.setState({ list: list });
    } 
    edit(rowData) {
 
        super.edit(rowData);
    }
    save(entity) {
        var index = this.state.list.findIndex(p => p.person.personId == entity.person.personId);
        super.save(entity, index);  
    } 
    dateBodyTemplate(rowData, column) {     
        return (
            <React.Fragment>
                { rowData.person[column.field] != null &&
                    <Moment format="DD.MM.YYYY">{rowData.person[column.field]}</Moment>

                }
            </React.Fragment>
        );
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Persoana</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '7rem' }}></Column>
                            <Column field="person.firstName" header="Prenume" sortable filter ></Column>       
                            <Column field="person.lastName" header="Nume" sortable filter ></Column>  
                            <Column field="person.phone" header="Telefon" sortable filter ></Column>  
                            <Column field="person.email" header="Email" sortable filter ></Column>
                            <Column field="partnerName" header="Partener" sortable filter ></Column>
                            <Column field="positionName" header="Pozitie" sortable filter ></Column>
                            <Column field="birthDate" header="Data nastere" sortable filter body={this.dateBodyTemplate}></Column> 
                            {/*
                            <Column field="person.personalNumericCode" header="CNP" sortable filter ></Column>
                            <Column field="person.identityCardSeries" header="Serie CI" sortable filter ></Column>
                            <Column field="person.identityCardNumber" header="Numar CI" sortable filter ></Column>
                            <Column field="person.issuedBy" header="Eliberat de" sortable filter ></Column>
                            <Column field="issueDate" header="Data elib. CI" sortable filter body={this.dateBodyTemplate} ></Column>                            
                            \*/}                                                                            
                        </DataTable>
                        <Person entity={this.state.obj} onSave={this.save} />
                        <Dialog visible={this.state.deleteDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.deleteDialogFooter} onHide={this.hideDeleteDialog.bind(this)}>
                        <div className="confirmation-content">
                                <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                                <span>Sunteti sigur ca doriti sa stergeti <b>{this.state.rowData.fullName}</b> ?</span>
                        </div>    
                        </Dialog>
                    </div>
                </div>
            </div>
        );
    }
}
export default ListPerson;
