import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService';
import { AutoComplete } from "primereact/autocomplete";
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column'; 
import { BaseComponent } from '../base/BaseComponent';
import { Dialog } from 'primereact/dialog';
import { Button } from 'primereact/button';
import { TabView, TabPanel } from 'primereact/tabview';
 
export class DayActivity extends BaseComponent {

    constructor(props) {
        super(props);
        this.state = {
            activeIndex:0,
            userInfo: {}, filteredItems: [], selectedItem: null,
            listTask: [],
            listCarts: [],
            selectedTasks: [],
            selectedCarts: [],
            _selectedCurrent: []
        };

    }

    componentDidMount() {

    }

    async setSelectedItem(value) {
        this.setState({ selectedItem: value, selectedTasks: [], selectedCarts:[] });
        this.loadTasksByOrder(value);
        this.loadCartsByOrder(value);
    }

    async loadTasksByOrder(selected) {
        var list = await authService.fetchAuth('Pck_OrderView/GetTasksByOrder', { Value: selected.orderNumber });
        this.setState({ listTask: list });
    }

    async loadCartsByOrder(selected) {
        var list = await authService.fetchAuth('Pck_OrderView/GetCartsByOrder', { Value: selected.orderNumber });
        this.setState({ listCarts: list });
    }
    async finalizeaza() {
        var result = await authService.fetchAuth('Pck_OrderView/SaveOrder', { ListCarts: this.state.selectedCarts, ListTasks: this.state.selectedTasks });
        window.toastE.current.show({ life: 20000, severity: 'success', summary: 'Comanda', detail: 'Operatiune finalizata cu succes!' });
        
        this.reset();
    }
    reset() {
        this.setState({
            selectedItem: null,
            listCarts: [],
            listTask: [],
            selectedTasks: [],
            selectedCarts: [],
            activeIndex:0
        });
    }

    selectectedTask(e) {
        if (e.value.length > 0 && e.value[e.value.length - 1].status != 0) {
        }
        else if (e.value.length > 1 && this.state.selectedTasks.length < e.value.length) {
            this.state._selectedCurrent = e.value;
            this.setState({ confirmDialog: true });
        }
        else {
            this.setState({ selectedTasks: e.value });
        }
    }
    selectedCart(e) {
        if (e.value.length > 0 && e.value[e.value.length - 1].status != 0) {
        }
        else
            this.setState({ selectedCarts: e.value });
    }

    searchItems = async (event) => {
        //in a real application, make a request to a remote url with the query and return filtered results, for demo purposes we filter at client side
        let query = event.query;

        var items = await authService.fetchAuth('Pck_OrderView/GetActiveOrders');
        let _filteredItems = [];

        for (let i = 0; i < items.length; i++) {
            let item = items[i];
            if (item.orderNumber.toLowerCase().indexOf(query.toLowerCase()) === 0) {
                _filteredItems.push(item);
            }
        }

        this.setState({ filteredItems: _filteredItems });
    }
    confirmSelected() {

        this.setState({ confirmDialog: false, selectedTasks: this.state._selectedCurrent });
    }

    hideConfirmDialog() {

        this.setState({ confirmDialog: false });
    }


    rowClass = (data) => {
        return {
            'bg-success' : data.status==1
        };
    };

    confirmDialogFooter = (
        <React.Fragment>
            <Button label="Nu" icon="pi pi-times" className="p-button-text" onClick={this.hideConfirmDialog.bind(this)} />
            <Button label="Da " icon="pi pi-check" className="p-button-text" onClick={this.confirmSelected.bind(this)} />
        </React.Fragment>
    );


    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">

                    <div className="card">
                        <div className="p-col-12 p-md-4">
                            <span className="p-float-label mt-12">

                                <AutoComplete value={this.state.selectedItem} suggestions={this.state.filteredItems} completeMethod={this.searchItems}
                                    virtualScrollerOptions={{ itemSize: 38 }} field="orderNumber" dropdown onChange={(e) => this.setSelectedItem(e.value)} />
                                <label htmlFor="">Numar comanda</label>
                            </span>
                        </div>

                        <div className="p-col-12 p-md-12">
                            <TabView activeIndex={this.state.activeIndex} onTabChange={(e) => this.setState({ activeIndex:e.index })  }>
                                <TabPanel header="Operatiuni">
                                    <DataTable dataKey="pck_TaskView.taskName" rowClassName={this.rowClass} removableSort resizableColumns reorderableColumns scrollable scrollHeight="450px" selection={this.state.selectedTasks} onSelectionChange={e => { this.selectectedTask(e); }} value={this.state.listTask} removableSort className="p-datatable-sm p-datatable-gridlines">
                                        <Column selectionMode="multiple" headerStyle={{ width: '3em' }}></Column>
                                        <Column field="pck_TaskView.taskName" header="Nume" sortable filter ></Column>
                                    </DataTable>
                                </TabPanel>
                                <TabPanel header="Carucioare">
                                    <DataTable dataKey="pck_CartView.carriageNumber" rowClassName={this.rowClass} removableSort resizableColumns reorderableColumns scrollable scrollHeight="450px" selection={this.state.selectedCarts} onSelectionChange={e => { this.selectedCart(e); }} value={this.state.listCarts} removableSort className="p-datatable-sm p-datatable-gridlines">
                                        <Column selectionMode="multiple" headerStyle={{ width: '3em' }}></Column>
                                        <Column field="pck_CartView.carriageNumber" header="Numar" sortable filter ></Column>
                                    </DataTable>
                                    <div className="p-col-12 p-md-4">
                                        <Button label="Finalizeaza" disabled={this.state.selectedTasks.length == 0 || this.state.selectedCarts.length == 0} className="p-button-rounded " onClick={this.finalizeaza.bind(this)} />
                                    </div>
                                </TabPanel>
                            </TabView>
                        </div>
                    </div>
                </div>
                <Dialog visible={this.state.confirmDialog} style={{ width: '450px' }} header="Confirm" modal footer={this.confirmDialogFooter} onHide={this.hideConfirmDialog.bind(this)}>
                    <div className="confirmation-content">
                        <i className="pi pi-exclamation-triangle p-mr-3" style={{ fontSize: '2rem' }} />
                        <span>Sigur doriti sa selectati mai multe operatiuni?  </span>
                    </div>
                </Dialog>
            </div>
        );
    }
}
export default DayActivity;