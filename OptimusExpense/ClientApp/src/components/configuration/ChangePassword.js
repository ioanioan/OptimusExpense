import React, { Component, createRef } from 'react';
import { Button } from 'primereact/button';
import classNames from 'classnames';

import { EditComponent } from '../base/EditComponent';
import { Password } from 'primereact/password';
export class ChangePassword extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity = {};
        this.methodSave = "Configuration/ChangePasswordS";
    }
    async save() {
        var isSave = this.state.entity.password && this.state.entity.oldPassword && this.state.entity.confirmPassword;
        var r = await super.save(isSave);
        if (r != null) {
            window.toastE.current.show({ life: 20000, severity: 'success', summary: 'Schimbare parola', detail: 'Parola a fost schimbata cu succes!' });
            this.state.entity.oldPassword = '';
            this.state.entity.password = '';
            this.state.entity.confirmPassword = '';
            this.setState({ entity: this.state.entity, submitted: false });
        }
    }

    render() {
        return (
            <div className="p-grid p-fluid">
                <div className="p-col-12">
                    <div className="card">
                        <h3>Administrare</h3>
                        <div className="p-col-12 p-md-4">
                            <div className="box p-shadow-5 pt-2 pb-2">
                                <div className="p-grid p-col-12 pl-4">
                                    <div className="p-col-11">
                                        <p className="pr-1" style={{ 'color': '#008CBA', 'font-weight': 'bold' }}>SCHIMBARE PAROLA</p>
                                    </div>
                                </div>
                                <span className="p-float-label mt-4 pl-4 pr-4">
                                    <Password id="password" value={this.state.entity.oldPassword} onChange={(e) => this.onInputChange(e.target.value, 'oldPassword')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.oldPassword })} />
                                    {this.state.submitted && !this.state.entity.oldPassword && <small className="p-invalid">Campul este obligatoriu!</small>}
                                    <label className="pl-4" htmlFor="oldPassword">Parola veche*</label>
                                </span>
                                <span className="p-float-label mt-4 pl-4 pr-4">
                                    <Password id="password" value={this.state.entity.password} onChange={(e) => this.onInputChange(e.target.value, 'password')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.password })} />
                                    {this.state.submitted && !this.state.entity.password && <small className="p-invalid">Campul este obligatoriu!</small>}
                                    <label className="pl-4" htmlFor="password">Parola noua*</label>
                                </span>
                                <span className="p-float-label mt-4 pl-4 pr-4">
                                    <Password id="confirmPassword" value={this.state.entity.confirmPassword} onChange={(e) => this.onInputChange(e.target.value, 'confirmPassword')} className={classNames({ 'p-invalid': this.state.submitted && !this.state.entity.confirmPassword })} />
                                    {this.state.submitted && !this.state.entity.confirmPassword && <small className="p-invalid">Campul este obligatoriu!</small>}
                                    <label className="pl-4" htmlFor="confirmPassword">Confirma parola*</label>
                                </span>
                                <span className="p-float-label mt-4 pl-4 pr-4">
                                    <Button label="Schimbare parola" className="p-button-rounded p-button-info" onClick={this.save.bind(this)} />
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
export default ChangePassword;