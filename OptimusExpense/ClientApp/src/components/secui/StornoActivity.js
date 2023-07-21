import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService';

export class StornoActivity extends Component {
    constructor(props) {
        super(props);
        this.state = { userInfo: {} };
    }

    componentDidMount() {
        this.incarcaDateUtilizator();
    }

    async incarcaDateUtilizator() {
        var userInfo = await authService.fetchAuth('Configuration/GetDateUtilizator', {});

        this.setState({ userInfo: userInfo });
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">                   
                    <div className="card">                        
                        <div className="p-col-12 p-md-4">
                            <div className="box p-shadow-5 pt-2 pb-2">
                                <div className="p-grid p-col-12 pl-4">
                                    <div className="p-col-11">
                                        <p className="pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>INFORMATII UTILIZATOR</p>
                                    </div>
                                </div>
                                <div className="p-grid p-col-12 pl-4 pt-2 pb-0">
                                    <div className="p-col-11">
                                        <i className="pi pi-fw pi-user" style={{ 'fontSize': '16px', 'font-weight': 'bold', 'color': '#008CBA' }}></i>
                                        <p className="pr-1 pl-2" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold', 'color': '#008CBA' }}>Nume angajat:</p>
                                        <p className="pr-1" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold' }}>{this.state.userInfo.employeeName}</p>
                                    </div>
                                </div>
                                <div className="p-grid p-col-12 pl-4 pt-0 pb-0">
                                    <div className="p-col-11">
                                        <i className="pi pi-fw pi-shopping-cart" style={{ 'fontSize': '16px', 'font-weight': 'bold', 'color': '#008CBA' }}></i>
                                        <p className="pr-1 pl-2" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold', 'color': '#008CBA' }}>Sectie:</p>
                                        <p className="pr-1" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold' }}>{this.state.userInfo.sectionName}</p>
                                    </div>
                                </div>
                                <div className="p-grid p-col-12 pl-4 pt-0 pb-0">
                                    <div className="p-col-11">
                                        <i className="pi pi-fw pi-user-plus" style={{ 'fontSize': '16px', 'font-weight': 'bold', 'color': '#008CBA' }}></i>
                                        <p className="pr-1 pl-2" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold', 'color': '#008CBA' }}>Manager:</p>
                                        <p className="pr-1" style={{ 'display': 'inline-block', 'fontSize': '14px', 'font-weight': 'bold' }}>{this.state.userInfo.superiorName}</p>
                                    </div>
                                </div>                              
                            </div>                           
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
export default StornoActivity;