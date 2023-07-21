import React, { Component } from 'react';
import { Button } from 'primereact/button';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import authService from './api-authorization/AuthorizeService';
import {  BaseComponent } from './base/BaseComponent';
import { Link } from 'react-router-dom';
import { Row } from 'reactstrap';

export class HomeUser extends BaseComponent {
    nrRows = 5;
    constructor(props) {
        
        super(props);
        this.state = {
            nrD: this.nrRows,
            nrT: this.nrRows
        };

        this.incarcaDeconturi = this.incarcaDeconturi.bind(this);
        this.incarcaTranzactii = this.incarcaTranzactii.bind(this);
    }

    componentDidMount() {
        this.incarcaDeconturi();
        this.incarcaTranzactii();
    }

    async incarcaDeconturi() {
        
        var listDeconturi = await authService.fetchAuth('Expense/GetListExpenseReport', { NrRows: this.state.nrD });
        this.setState({ listDeconturi: listDeconturi });
    }

    async incarcaTranzactii() {
        var listTranzactii = await authService.fetchAuth('Expense/GetListLastTransactions', { NrRows: this.state.nrT });
        this.setState({ listTranzactii: listTranzactii });
    }


    linkDescriere(rowData) {
        return (
            <React.Fragment>

                <Link to={{ pathname: "Expense/ExpenseReport", expenseReport: rowData }} >{rowData.document.number}</Link>
            </React.Fragment>
        );
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card card-w-title">
                        <h3>Bine ati venit!</h3>
                        <div className="p-grid">
                            <div className="p-col 4 pl-4 pr-4 pt-4">
                                <div className="box p-shadow-5 pt-2 pb-2">
                                    <div className="p-grid p-col-12 pl-4">
                                        <div className="p-col-11">
                                            <p className="pt-2 pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>ULTIMELE DECONTURI</p>
                                        </div>
                                        <div className="p-col-1">
                                            <Button icon="pi pi-refresh" className="p-button-rounded p-button-info p-button-text" onClick={this.incarcaDeconturi}/>
                                        </div>
                                    </div>
                                    <div className="p-col-12">
                                        <DataTable value={this.state.listDeconturi} className="p-datatable-sm p-datatable px-4">
                                            <Column field="document.date" header="Data" body={this.dateBodyTemplate} ></Column>
                                            <Column field="document.number" header="Numar" body={this.linkDescriere} ></Column>
                                            <Column field="expenseReport.description" header="Descriere"   ></Column>
                                            <Column field="sumaDecont" header="Suma" ></Column>
                                            <Column field="statusName" header="Status"  ></Column>
                                        </DataTable>
                                    </div>
                                    <div className="p-grid mt-4">
                                        <div className="p-col-6"></div>
                                        <div className="p-col-3">
                                            <Link to="/expense/ExpenseReport" ><Button label="Decont nou" className="p-button-rounded p-button-info" /></Link>                                        
                                        </div>
                                        <div className="p-col-3 pr-4">
                                            <Button label={this.state.nrD == 0 ? "Vezi putine" : "Vezi toate"} className="p-button-rounded p-button-outlined p-button-info" onClick={(e) => { this.state.nrD = this.state.nrD == 0 ? this.nrRows : 0; this.setState({ nrD: this.state.nrD }); this.incarcaDeconturi(); }} />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="p-col 4 pr-4 pt-4">
                                <div className="box p-shadow-5 pt-2 pb-2">
                                    <div className="p-grid p-col-12 pl-4">
                                        <div className="p-col-11">
                                            <p className="pt-2 pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>ULTIMELE CHELTUIELI NEDECONTATE</p>
                                        </div>
                                        <div className="p-col-1">
                                            <Button icon="pi pi-refresh" className="p-button-rounded p-button-info p-button-text" onClick={this.incarcaDeconturi} />
                                        </div>
                                    </div>
                                    <div className="p-col-12">
                                        <DataTable value={this.state.listTranzactii} className="p-datatable-sm p-datatable px-4">
                                            <Column field="expense.date" header="Data" body={this.dateBodyTemplate} ></Column>
                                            <Column field="expense.description" header="Descriere" ></Column>
                                            <Column field="expense.price" header="Suma" ></Column>
                                            <Column field="currencyName" header="Moneda" ></Column>
                                        </DataTable>
                                    </div>
                                    <div className="p-grid mt-4">
                                        <div className="p-col-9"></div>
                                        <div className="p-col-3 pr-4">
                                            <Button label={this.state.nrT == 0 ? "Vezi putine" : "Vezi toate"} className="p-button-rounded p-button-outlined p-button-info" onClick={(e) => { this.state.nrT = this.state.nrT == 0 ? this.nrRows:0; this.setState({ nrT: this.state.nrT }); this.incarcaTranzactii(); }} />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}

export default HomeUser;