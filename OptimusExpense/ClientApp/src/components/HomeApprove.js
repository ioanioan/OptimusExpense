import React, { Component } from 'react';
import { Button } from 'primereact/button';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import authService from './api-authorization/AuthorizeService';
import { BaseComponent } from './base/BaseComponent';
import { Link } from 'react-router-dom';
import { Chart } from 'primereact/chart';

export class HomeApprove extends Component {
    constructor(props) {
        super(props);
        this.state = { list: [] };
    }

    componentDidMount() {
        this.incarcaDashboard();
    }

    async incarcaDashboard() {
        var list = await authService.fetchAuth('Expense/GetDashBoardExpenseReport', {});
        var listGraphic = await authService.fetchAuth('Expense/GetGraphicExpenses',);
      
        var lineStylesData = {
            labels: listGraphic.map(p => p.month > 9 ? p.month + '-' + p.year : '0' + p.month + '-' + p.year),
            datasets: [      
                {
                    label: 'Valoare cheltuiala',
                    data: listGraphic.map(p => p.sum),
                    fill: true,
                    borderColor: '#008CBA',
                    backgroundColor: '#60d2f7'
                }
            ]
        };

        this.setState({ list: list, lineStylesData: lineStylesData });
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card card-w-title">
                        <h3>Bine ati venit!</h3>
                        <div className="p-col-12">
                            <div className="p-grid">
                                <div className="p-col-3 pl-4 pr-4 pt-4">
                                </div>
                                <div className="p-col-6 pl-4 pr-4 pt-4">
                                    <div className="p-grid">
                                        {
                                            this.state.list.filter(p=>p.type==1).map((p) => {
                                                return (
                                                    <div className="p-col-4">
                                                        <div className="p-col-12" style={{ 'text-align': 'center' }}>
                                                            <i className={p.icon} style={{ 'fontSize': '3em', 'color': '#008CBA' }}></i>
                                                        </div>
                                                        <div className="p-col-12" style={{ 'fontSize': '2em', 'text-align': 'center', 'color': '#008CBA', 'font-weight': 'bold' }}>
                                                            { p.value}
                                                        </div>
                                                        <div className="p-col-12" style={{ 'font-style': 'italic', 'text-align': 'center', 'fontSize': '11px', 'color': 'grey' }}>
                                                            {p.text}
                                                    </div>
                                                    </div>
                                                )
                                            })
                                        }                               
                                    </div>
                                </div>
                                <div className="p-col-3 pl-4 pr-4 pt-4">
                                </div>
                            </div>
                        </div>
                        <div className="p-grid">
                            <div className="p-col 4 pl-4 pr-4 pt-4">
                                <div className="box p-shadow-5 pt-2 pb-2">
                                    <div className="p-grid p-col-12 pl-4">
                                        <div className="p-col-11">
                                            <p className="pt-2 pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>SUMAR DECONTURI LUNA CURENTA</p>
                                        </div>
                                        <div className="p-col-1">
                                            <Button icon="pi pi-refresh" className="p-button-rounded p-button-info p-button-text" onClick={this.incarcaDashboard.bind(this)} />
                                        </div>
                                    </div>
                                    {
                                        this.state.list.filter(p => p.type == 2).map((p) => {
                                            return (
                                                <div className="p-grid p-col-12 pl-4">
                                                    <div className="p-col-11">
                                                        {p.to != null &&
                                                            <Link to={{ pathname: p.to, statusName: p.parameter }} >  <p className="pr-1" >{p.text}</p></Link>                          
                                                        }
                                                        {p.to == null &&
                                                           <p className="pr-1" >{p.text}</p> 
                                                        }
                                                    </div>
                                                    <div className="p-col-1">
                                                        <p className="pl-2">{p.value}</p>
                                                    </div>
                                                </div>
                                            )
                                        })
                                    }                                     
                                    <div className="p-grid p-col-12 pl-4 pt-5">
                                    </div>
                                </div>
                            </div>
                            <div className="p-col 4 pr-4 pt-4">
                                <div className="box p-shadow-5 pt-2 pb-2">
                                    <div className="p-grid p-col-12 pl-4">
                                        <div className="p-col-11">
                                            <p className="pt-2 pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>EVOLUTIE CHELTUIELI</p>
                                        </div>
                                        <div className="p-col-1">
                                            <Button icon="pi pi-refresh" className="p-button-rounded p-button-info p-button-text" onClick={this.incarcaDashboard.bind(this)} />
                                        </div>
                                    </div>
                                    <div className="p-col-12">                                       
                                        <Chart type="line" data={this.state.lineStylesData}/>                                        
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

export default HomeApprove;