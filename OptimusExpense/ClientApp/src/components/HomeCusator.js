import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class HomeCusator extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="p-col-12 p-md-4">
                        <div className="box p-shadow-5 pt-2 pb-2">
                            <div className="p-grid p-col-12 pl-4">
                                <div className="p-col-11">
                                    <p className="pr-1" style={{ 'fontSize': '24px', 'color': '#008CBA', 'font-weight': 'bold' }}>BINE ATI VENIT!</p>
                                </div>
                            </div>
                            <div className="p-grid p-col-12 pl-4 pt-4 pb-2">
                                <div className="p-col-12" style={{ 'border':'2px solid #008CBA', 'border-radius': '19px', 'textAlign':'center' }}>
                                    <i className="pi pi-fw pi-user-edit" style={{ 'fontSize': '26px', 'color': '#008CBA' }}></i>                 
                                    <Link to='secui/UserInfo'><p className="pr-2 pl-1" style={{ 'display': 'inline-block', 'fontSize': '24px', 'color': '#008CBA' }}>Date utilizator</p></Link>
                                </div>
                            </div>
                            <div className="p-grid p-col-12 pl-4 pt-4 pb-2">
                                <div className="p-col-12" style={{ 'border': '2px solid #008CBA', 'border-radius': '19px', 'textAlign': 'center' }}>
                                    <i className="pi pi-fw pi-calendar-plus" style={{ 'fontSize': '26px', 'color': '#008CBA' }}></i>
                                    <Link to='secui/DayActivity'><p className="pr-2 pl-1" style={{ 'display': 'inline-block', 'fontSize': '24px', 'color': '#008CBA' }}>Activitate zilnica</p></Link>
                                </div>
                            </div>
                            <div className="p-grid p-col-12 pl-4 pt-4 pb-2">
                                <div className="p-col-12" style={{ 'border': '2px solid #008CBA', 'border-radius': '19px', 'textAlign': 'center' }}>
                                    <i className="pi pi-fw pi-calendar-minus" style={{ 'fontSize': '26px', 'font-weight': 'bold', 'color': '#008CBA' }}></i>
                                    <Link to='secui/StornoActivity'><p className="pr-2 pl-1" style={{ 'display': 'inline-block', 'fontSize': '24px', 'color': '#008CBA' }}>Storno activitate</p></Link>
                                </div>
                            </div>
                            <div className="p-grid p-col-12 pl-4 pt-4 pb-3">
                                <div className="p-col-12" style={{ 'border': '2px solid #008CBA', 'border-radius': '19px', 'textAlign': 'center' }}>
                                    <i className="pi pi-fw pi-chart-line" style={{ 'fontSize': '26px', 'color': '#008CBA' }}></i>
                                    <Link to='reports/Report'><p className="pr-2 pl-1" style={{ 'display': 'inline-block', 'fontSize': '24px', 'color': '#008CBA' }}>Istoric activitate</p></Link>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
export default HomeCusator;