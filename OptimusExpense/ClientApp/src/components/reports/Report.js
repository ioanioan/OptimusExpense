import React, { Component, createRef } from 'react';

import { Calendar } from 'primereact/calendar';

import { Button } from 'primereact/button';

import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';

import Moment from 'react-moment';

import { ListComponent } from "../base/ListComponent";

import { Dropdown } from 'primereact/dropdown';
import moment from 'moment';
export class Report extends ListComponent {
    constructor(props) {
        super(props);
        this.state.listDetails = [];
        this.state.reportParameters = {};
    }

    componentDidMount() {
        this.populate();
    }
    async populate() {
        var listReports = await authService.fetchAuth('Report/GetReports');
        this.setState({ listReports: listReports });
    }
    async populateDetails(entity) {
        this.state.reportParameters = {};
        var listDetails = [];
        try {
            listDetails = await authService.fetchAuth('Report/GetReportDetails', entity);          
        }
        catch { }
        this.setState({ listDetails: listDetails, list: [], reportParameters: this.state.reportParameters });
    }

    setParameter(param,value) {
        this.state.reportParameters[param] = value;
        this.setState({ reportParameters: this.state.reportParameters });
    }
    async run() {
        var result = [];
         
        this.setState({ submitted: true });
        if (Object.keys(this.state.reportParameters).length != this.state.listDetails.length) {
            return;
        }
        try {
             result = await authService.fetchAuth("Report/RunReport", { report: this.state.report, parameters: this.state.reportParameters });
        } catch { }
        this.setState({ list: result, submitted: false });
    }

    leftToolbarTemplate() {
        return (
          
                <div className="p-grid">
                  
                <span className="p-float-label mt-4">
                    <Dropdown value={this.state.report} options={this.state.listReports} onChange={(e) => { this.setState({ report: e.value }); this.populateDetails(e.value); }} optionLabel="name"  />                        
                        <label htmlFor="curr">Raport</label>
                </span>      
                {
                    this.state.listDetails.map((item) => {                  
                        return (
                            <span className="p-float-label mt-4 ml-4">
                                {item.reportDetail.type == 'date' &&
                                    <Calendar value={this.state.reportParameters[item.reportDetail.parameterName]} dateFormat="dd.mm.yy" onChange={(e) => { var val = moment(e.value).format(); this.setParameter(item.reportDetail.parameterName, val); }} showIcon showButtonBar ></Calendar>
                                }
                                {item.reportDetail.type == 'combo' &&
                                    <Dropdown value={this.state.reportParameters[item.reportDetail.parameterName]} options={item.reportDetailComboResult} onChange={(e) => { this.setParameter(item.reportDetail.parameterName, e.value) }} optionLabel="displayMember" optionValue="valueMember" />
                                }
                                {this.state.submitted && !this.state.reportParameters[item.reportDetail.parameterName] && <small className="p-invalid">Campul este obligatoriu!</small>}

                                <label >{item.reportDetail.name}</label>
                            </span>      
                        )
                    })
                }
                {this.state.report!=null&&
                    <span className="p-float-label mt-4 ml-4">
                        <Button label="Ruleaza" icon="pi pi-refresh" className="p-button-success p-mr-2" onClick={this.run.bind(this)} />
                    </span>
                }
                </div> 
        )
    }
    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Rapoarte</h3>
                        <Toolbar className="p-mb-4" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>
                        <DataTable value={this.state.list} ref={this.state.dt} removableSort resizableColumns reorderableColumns scrollable scrollHeight="200%"
                            removableSort className="p-datatable-sm p-datatable-gridlines">
                            {this.state.list != null && this.state.list.length > 0 && Object.keys(this.state.list[0]).map((col) => {
                                return <Column key={col} field={col} header={col} sortable filter / >;
                            })
                            }
                        </DataTable>
                                            
                    </div>
                </div>
            </div>
        );
    }
}
export default Report;



