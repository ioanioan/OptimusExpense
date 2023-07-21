import React, { Component, createRef } from 'react';
import { BaseComponent } from './BaseComponent';
import { Button } from 'primereact/button';
import { Checkbox } from 'primereact/checkbox';
import authService from '../api-authorization/AuthorizeService';
import Moment from 'react-moment';
export class ListComponent extends BaseComponent {
    methodDelete = "";
    constructor(props) {
        super(props);
        this.state = {
            rowData: {},

            obj: {},
            dt: createRef(),
            list: [
            ]
        };
        this.add = this.add.bind(this);
        this.exportCSV = this.exportCSV.bind(this);
        this.save = this.save.bind(this); 

    }
    exportCSV() {
        this.state.dt.current.exportCSV();
    }

    add() {
        this.setState({
            obj: { edit: true },
        });
    }  

    async delete() {
        await authService.fetchAuth(this.methodDelete, this.state.rowData);
        this.state.list.splice(this.state.list.indexOf(this.state.rowData), 1);
        this.setState({ list: this.state.list, deleteDialog: false });
    }  

    edit(rowData) {
        this.state.obj = { ...rowData };
        this.state.obj.edit = true;
        this.setState({
            obj: this.state.obj,
        });
    }

    hide = () => {
        this.setState({ edit: false });
    }

    leftToolbarTemplate() {
        return (
            <React.Fragment>
                <Button label="Adauga" icon="pi pi-plus" className="p-button-success p-mr-2" onClick={this.add} />
            </React.Fragment>
        )
    }
    rightToolbarTemplate() {
        return (
            <React.Fragment>
                <Button label="Export" icon="pi pi-upload" className="p-button-help" onClick={this.exportCSV} />
            </React.Fragment>
        )
    }

    confirmDelete(rowData) {
        this.setState({ rowData: rowData, deleteDialog: true });
    }
    hideDeleteDialog() {
        this.setState({ deleteDialog: false });
    }

    save(entity, index) {
       /* if (ref != null) {
            if (index >= 0) {
                this.state.list[index][ref] = entity;
            }
            else {
                var obj = {};
                obj[ref] = entity;
                this.state.list.unshift(obj);
            }
        }
        else*/
        {
            if (index >= 0) {
                this.state.list[index] = entity;
            }
            else {
                this.state.list.unshift( entity );
            }
        }
        this.setState({ list: this.state.list });
    } 


    deleteDialogFooter =
        (
            <React.Fragment>
                <Button label="Nu" icon="pi pi-times" className="p-button-text" onClick={this.hideDeleteDialog.bind(this)} />
                <Button label="Da" icon="pi pi-check" className="p-button-text" onClick={this.delete.bind(this)} />
            </React.Fragment>
        );

    actionBodyTemplate(rowData) {
        return (
            <React.Fragment>
                <Button   icon="pi pi-pencil" className="p-button-rounded p-button-success p-mr-2" onClick={() => { this.edit(rowData) }} />
                <Button icon="pi pi-trash" className="p-button-rounded p-button-danger" onClick={() => this.confirmDelete(rowData)} />
            </React.Fragment>
        );
    }


    render() {
        return (
            <div>
            </div>
        );
    }

   
}
