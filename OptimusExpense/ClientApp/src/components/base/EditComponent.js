import React, { Component, createRef } from 'react';
import { BaseComponent } from './BaseComponent';
import { Button } from 'primereact/button';
import authService from '../api-authorization/AuthorizeService';
import { confirmDialog } from 'primereact/confirmdialog';
import { ScrollPanel } from 'primereact/scrollpanel';
export class EditComponent extends BaseComponent {
   // required=[];
    methodSave = "";
    constructor(props) {
        super(props);
        this.state = {
            submitted: false,
            entity: props.entity
        };
        this.toastE = createRef();
        this.hide = this.hide.bind(this);
    }

    async save(isSave) {
        var result = null;
        if (isSave) {
            this.setLoading(true);
            try {
                var r = await authService.fetchAuth(this.methodSave, this.state.entity);
                if (this.props.onSave!=null)
                    this.props.onSave(r);
                result = r;
            }
            catch (ex) {
                this.alertError(ex);

                return result;
            }
            this.setLoading(false);
            this.state.entity.edit = false;          
        }
        this.state.submitted = true;
        this.setState({ submitted: this.state.submitted, entity: this.state.entity });
        return result;
    }  

    hide() {       
        this.state.entity.edit = false;
        this.setState({ entity: this.state.entity });    
    }
  

    componentDidUpdate(prevProps) {
        if (prevProps.entity !== this.props.entity) {
            if (this.state.entity != null) {
                for (var a in this.state.entity) {
                    if (this.state.entity[a] != null && this.state.entity[a] instanceof Object && this.state.entity[a].constructor === Object&& this.props.entity[a] == null) {
                        this.props.entity[a] = {};
                    }
                }
            }
            this.setState({ entity: this.props.entity, submitted: false });
        }
    }

    dialogFooter = (
        <React.Fragment>
            <Button label="Renunta" icon="pi pi-times" className="p-button-text" onClick={this.hide.bind(this)} />
            <Button label="Salveaza " icon="pi pi-check" className="p-button-text" onClick={this.save.bind(this)} />
        </React.Fragment>
    );

    render() {
        return (
            <div></div>
        );
    }
}
