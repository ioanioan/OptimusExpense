import React, { Component } from 'react';
import Moment from 'react-moment';
import { Checkbox } from 'primereact/checkbox';
import moment from 'moment';
import { confirmDialog } from 'primereact/confirmdialog';
import { ScrollPanel } from 'primereact/scrollpanel';
export class BaseComponent extends Component {
    
    constructor(props) {
        super(props);
        this.onInputChange = this.onInputChange.bind(this);
        this.setLoading = this.setLoading.bind(this);
    }
    alertError(ex) {
        var mes = <ScrollPanel style={{ width: '100%', height: '60px' }}>{ex}</ScrollPanel>;
        confirmDialog({
            message: mes,
            header: 'Avertizare',
            acceptLabel: "OK",
            acceptClassName: "p-button-danger",
            rejectLabel: " ",
            style: { width: '400px' }
        });
    }
    onInputChange(evt, name, name2, e) {
         
        if (evt instanceof Date) {
            evt = moment(evt).format();
        }
        this.setProperty(name, evt);
        if (name2 != null)
            this.setProperty(name2, e.originalEvent.currentTarget.innerText);
        if (this.state.entity != null) {
            this.setState({ entity: this.state.entity });
        }
        else if (this.state.obj != null) {
            this.setState({ obj: this.state.obj });
        }
        
    }
    setLoading = (show) => {

        this.setState({ loading: show });
   

    }
   static getProp(obj, column) {
        var nameSp = (""+column).split('.');
        for (var i = 0; i < nameSp.length; i++) {
            obj = obj[nameSp[i]];
            if (obj == null) {
                break;
            }
        }
        return obj;
    }
    dateBodyTemplate(rowData, column) {

        var r = BaseComponent. getProp(rowData, column.field);
        return (
            <React.Fragment>
                { r != null &&
                    <Moment format="DD.MM.YYYY">{r}</Moment>
                }
            </React.Fragment>
        );
    }
    dateBodyTemplate(rowData, column) {

        var r = BaseComponent.getProp(rowData, column.field);
        return (
            <React.Fragment>
                {r != null &&
                    <Moment format="DD.MM.YYYY">{r}</Moment>
                }
            </React.Fragment>
        );
    }
    dateTimeBodyTemplate(rowData, column) {

        var r = BaseComponent.getProp(rowData, column.field);
        return (
            <React.Fragment>
                {r != null &&
                    <Moment format="DD.MM.YYYY HH:mm:ss">{r}</Moment>
                }
            </React.Fragment>
        );
    }


    monthBodyTemplate(rowData, column) {

        var r = BaseComponent.getProp(rowData, column.field);
        return (
            <React.Fragment>
                { r != null &&
                    <Moment format="MM.YYYY">{r}</Moment>
                }
            </React.Fragment>
        );
    }
    checkBodyTemplate(rowData, column) {
        var r = BaseComponent.getProp(rowData, column.field);
        return (
            <React.Fragment>
                <center>
                    <Checkbox checked={r != null ? r : false}></Checkbox>
                </center>
            </React.Fragment>
        );
    }
    setProperty(name, value) {
        var nameSp = name.split('.');
        var i = 0;
        var ent = this.getEntity();
        for (i = 0; i < nameSp.length - 1; i++) {
            ent = ent[nameSp[i]];
        }
        ent[nameSp[i]] = value;
    }

    getEntity() {
        return this.state.entity != null ? this.state.entity : (this.state.obj != null ? this.state.obj: this.state);
    }
  
    render() {
        return (
            <div>
            </div>
        );
    }
}
