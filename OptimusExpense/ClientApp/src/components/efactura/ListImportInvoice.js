import React, { Component } from 'react';
import { InputText } from 'primereact/inputtext';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import { Calendar } from 'primereact/calendar';
import { InputTextarea } from 'primereact/inputtextarea';
import { Link, Redirect } from 'react-router-dom';
import { ListComponent } from '../base/ListComponent';
import { FileUpload } from 'primereact/fileupload';
import BlockUi from 'react-block-ui';
import 'react-block-ui/style.css';
import { Tooltip } from 'primereact/tooltip';

import { param } from 'jquery';
export class ListExpenseReport extends ListComponent {
    constructor(props) {

        super(props);
        var today = new Date();
        var start = new Date();
        start.setDate(today.getDate() - 1);
        this.state = { list: [], obj: { dateStart: start, dateEnd: today }, selected: [] };
        this.setState(this.state);
        
    }
    componentDidMount() {
        this.populate();
        
    }
    async populate() {
        this.setLoading(true);
        var list = await authService.fetchAuth('EFacturaUpload/GetEF_Raportare', { date: this.state.obj.dateStart, dateEnd: this.state.obj.dateEnd } );
        this.setState({ list: list,   selected: [] });
        this.setLoading(false);
    }

 
    showSend(rowData) {
        
        this.setState({ sendDialog: true, factura: { ...rowData } });
    }
    async send() {
        this.setLoading(true);
        var r = await authService.fetchAuth("EFacturaUpload/SendEFactura", this.state.factura).catch(rr => this.alertError(rr));
        this.setLoading(false);
        this.populate();
        this.hideSendDialog();
    }

    async sendFacturi() {
        this.setLoading(true);
        var r = await authService.fetchAuth("EFacturaUpload/SendEFacturi", this.state. selected.map((item, i) => {
            return item.eF_Raportare.idEFRaportare;
        }) ).catch(rr => this.alertError(rr));
        this.setLoading(false);
        this.populate();
    }

    async refresh(rowData) {
        var r = await authService.fetchAuth("EFacturaUpload/GetStareEFactura", rowData).catch(rr => this.alertError(rr));
        this.populate();
    }

    async remove(rowData) {
        var r = await authService.fetchAuth("EFacturaUpload/RemoveEFactura", rowData).catch(rr => this.alertError(rr));
        this.populate();
    }

    async downloadXml(rowData) {
        await authService.downloadAuth("EFacturaUpload/DownloadXml/" + rowData.eF_Raportare.idEFRaportare).catch(rr => this.alertError(rr));
    }


    async descarcaEFactura(rowData) {
        await authService.downloadAuth("EFacturaUpload/DescarcaEFactura/" + rowData.eF_Raportare.idEFRaportare).catch(rr => this.alertError(rr));

    }

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>
                <Button icon="pi pi-download" tooltip="Descarca XML" tooltipOptions={{ position: 'bottom' }} className="p-button-rounded p-button-info p-mr-2" onClick={() => this.downloadXml(rowData)} />
                <Button icon="pi pi-send" tooltip="Trimite" tooltipOptions={{ position: 'bottom' }} disabled={rowData.eF_Raportare.codRaspuns != null} className="p-button-rounded p-button-success p-mr-2" onClick={() => this.showSend(rowData)} />
                <Button icon="pi pi-refresh" tooltip="Refresh" tooltipOptions={{ position: 'bottom' }} disabled={rowData.eF_Raportare.codRaspuns == null} className="p-button-rounded p-button-help p-mr-2" onClick={() => this.refresh(rowData)} />
                <Button icon="pi pi-trash" tooltip="Sterge" tooltipOptions={{ position: 'bottom' }} disabled={rowData.eF_Raportare.mesajRaspuns == "ok"} className="p-button-rounded p-button-danger p-mr-2" onClick={() => this.remove(rowData)} />
                <Button icon="pi pi-download" tooltip="Descarca raspuns" tooltipOptions={{ position: 'bottom' }} disabled={rowData.eF_Raportare.idTranzactieRaspuns == null} className="p-button-rounded p-button-primary" onClick={() => this.descarcaEFactura(rowData)} />
            </React.Fragment>
        );
    }
     
    


    sendDialogFooter = (
        <React.Fragment>
            <Button label="Nu" icon="pi pi-times" className="p-button-danger" onClick={this.hideSendDialog.bind(this)} />
            <Button label="Da " icon="pi pi-check" className="p-button-primary" onClick={this.send.bind(this)} />
        </React.Fragment>
    );

    hideSendDialog() {
        this.setState({ sendDialog: false });
    }
    async uploadEFactura(path) {
        
        var result = await authService.fetchAuth('EFacturaUpload/UploadEFactura', { value: path });
        this.populate();
    }


    async uploadFacturi() {
        this.setLoading(true);
        var result = await authService.fetchAuth('EFacturaUpload/UploadNewFacturi', { date: this.state.obj.dateStart, dateEnd: this.state.obj.dateEnd, value: this.state.obj.nrFactura }).catch(e => this.setLoading(false));
        this.populate();
        this.setLoading(false);
    }

    async getStariEFacturi() {
        this.setLoading(true);
        var result = await authService.fetchAuth('EFacturaUpload/GetStariEFacturi', { date: this.state.obj.dateStart, dateEnd: this.state.obj.dateEnd  }).catch(e => this.setLoading(false));
        this.populate();
        this.setLoading(false);
    }

    setSelection(value) {
        var selected = [];
        value.map((item, i) => {
            if (item.eF_Raportare.codRaspuns==null)
                selected.push(item);
        });
        this.state.selected = selected; this.setState({ selected: selected });
    }

    

    render() {
        return (           
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Facturi importate</h3>
                        {/*<Toolbar className="p-mb-12" left={this.leftToolbarTemplate()} right={this.rightToolbarTemplate()}></Toolbar>*/}
                        <div className="p-grid">
                            <div className="p-col-12 p-md-11">
                                <React.Fragment>
                                    <BlockUi tag="div" blocking={this.state.loading} >
                                        <div className="p-grid">
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <Button icon="pi pi-refresh" className="p-button-help" label="Incarca facturi" onClick={this.uploadFacturi.bind(this)} />
                                                </span>
                                            </div>
                                            
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <Calendar value={this.state.obj.dateStart} dateFormat="dd.mm.yy" onChange={(e) => { this.onInputChange(e.value, 'dateStart'); this.populate(); }} showIcon showButtonBar ></Calendar>
                                                    <label >Data inceput</label>
                                                </span>
                                            </div>
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <Calendar value={this.state.obj.dateEnd} dateFormat="dd.mm.yy" onChange={(e) => { this.onInputChange(e.value, 'dateEnd'); this.populate(); }} showIcon showButtonBar ></Calendar>
                                                    <label >Data sfarsit</label>
                                                </span>
                                            </div>
                                            <div className="p-col-2">
                                                <span className="p-float-label ">
                                                    <InputText id="nrFactura" value={this.state.obj.nrFactura} onChange={(e) => this.onInputChange(e.target.value, 'nrFactura')}   />
                                                    <label htmlFor="nrFactura">Nr. factura</label>
                                                </span>
                                            </div>
                                        </div>

                                        <div className="p-grid">
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <Button icon="pi pi-send" disabled={this.state .selected?.length==0 } className="p-button-success" label="Trimite facturi" onClick={this.sendFacturi.bind(this)} />
                                                </span>
                                            </div>
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <Button icon="pi pi-refresh" className="p-button-help" label="Preluare stari" onClick={this.getStariEFacturi.bind(this)} />
                                                </span>
                                            </div>
                                            <div className="p-col-2">
                                                <span className="p-float-label">
                                                    <FileUpload name="file" className="p-button-info" mode="basic" url="Base/Upload" auto chooseLabel="Incarca o factura" onProgress={(e) => { }} onError={(e) => { alert('A avut loc o eroare la incarcare!'); }} onUpload={(e) => { this.uploadEFactura(e.xhr.response); }} />
                                                </span>
                                            </div>
                                        </div>
                                    </BlockUi>
                                </React.Fragment>
                            </div>
                            <div className="p-col-12 p-md-1">
                                <React.Fragment>
                                    <div className="p-grid">
                                        <div className="p-col-10">
                                            <span className="p-float-label">
                                                <Button label="Export" icon="pi pi-upload" className="p-button-help" onClick={this.exportCSV} />
                                            </span>
                                        </div>
                                    </div>
                                </React.Fragment>
                            </div>
                        </div>

                        <DataTable value={this.state.list} ref={this.state.dt} resizableColumns reorderableColumns responsiveLayout="scroll"  
                            paginator   rows={10}

                            selection={this.state .selected} onSelectionChange={e => { this.setSelection(e.value); }}
                            removableSort className="p-datatable-sm p-datatable-gridlines" dataKey="eF_Raportare.idEFRaportare">
                            <Column selectionMode="multiple" disabled="True" headerStyle={{ width: '3em' }}></Column>
                            <Column body={this.actionBodyTemplate.bind(this)} headerStyle={{ width: '15rem' }}></Column>
                            <Column field="eF_Raportare.dataFactura" header="Data factura" body={this.dateBodyTemplate} sortable filter ></Column>

                            <Column field="eF_Raportare.importedTime" header="Data import" body={this.dateTimeBodyTemplate} sortable filter ></Column>
                            <Column field="eF_Raportare.idEFRaportare" header="Numar" sortable filter></Column>
                            <Column field="eF_Raportare.numarFactura" header="Numar factura" sortable filter></Column>
                            <Column field="user" header="Utilizator" sortable filter></Column>
                            <Column field="eF_Raportare.codRaspuns" header="Id incarcare" sortable filter></Column>
                            <Column field="eF_Raportare.idTranzactieRaspuns" header="Id descarcare" sortable filter></Column>
                            <Column field="eF_Raportare.mesajRaspuns" header="Mesaj raspuns" sortable filter></Column>
                            <Column field="eF_Raportare.dataRaportare" header="Data raportare" body={this.dateTimeBodyTemplate} sortable filter></Column>
                            <Column field="eF_Raportare.dataRaspuns" header="Data raspuns" body={this.dateTimeBodyTemplate} sortable filter></Column>

                        </DataTable>
                          <Dialog visible={this.state.sendDialog} style={{ width: '450px' }} header="Trimite" modal footer={this.sendDialogFooter} onHide={this.hideSendDialog.bind(this)}>
                            <BlockUi tag="div" blocking={this.state.loading} >
                        <div className="confirmation-content">
                            <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                            <span>Sunteti sigur ca doriti sa trimiteti raportarea?</span>
                                </div>
                            </BlockUi>
                            </Dialog>
                         
                    </div>
                </div>
                </div>
           
        );
    }
}

export default ListExpenseReport;