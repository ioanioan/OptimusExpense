import React, { Component, createRef } from 'react';
import { InputText } from 'primereact/inputtext';
import { Calendar } from 'primereact/calendar';
import { Button } from 'primereact/button';
import classNames from 'classnames';
import { DataTable } from 'primereact/datatable';
import authService from '../api-authorization/AuthorizeService';
import { Column } from 'primereact/column';
import { Toolbar } from 'primereact/toolbar';
import { Dialog } from 'primereact/dialog';
import { Checkbox } from 'primereact/checkbox';
import { EditComponent } from '../base/EditComponent';
import { ListComponent } from '../base/ListComponent';
import { Dropdown } from 'primereact/dropdown';
import Moment from 'react-moment';
import moment from 'moment';
import { RadioButton } from 'primereact/radiobutton';
import { InputTextarea } from 'primereact/inputtextarea';
import { InputNumber } from 'primereact/inputnumber';
import { FileUpload } from 'primereact/fileupload';
import { Link } from 'react-router-dom';
import BlockUi from 'react-block-ui';
import 'react-block-ui/style.css';
export class Expense extends EditComponent {
    constructor(props) {
        super(props);
        this.state.entity.expense = {};
        this.methodSave = "Expense/SaveExpense";
    }
    async save() {
        var isSave = this.state.entity.expense.date && this.state.entity.expense.price && this.state.entity.expense.filePath;
        if (this.state.entity.expense.date != null) {
            this.state.entity.expense.date = moment(this.state.entity.expense.date).format();
        }
       
        return await super.save(isSave);
    }
    componentDidMount() {
        this.populate();
        
    }

    componentDidUpdate(prevProps) {
        
        if (this.props.entity.expense == null) {
            this.props.entity.expense = {};
        }
        if (prevProps.entity !== this.props.entity) {
            if (this.state.listTVA != null && this.state.listTVA.length > 0 && this.props.entity.expense.vatId == null) {
                this.props.entity.expense.vatId = this.state.listTVA[0].dictionaryDetailId;
            }
            if (this.state.listCurrency != null && this.state.listCurrency.length > 0 && this.props.entity.expense.currencyId == null) {
                this.props.entity.expense.currencyId = this.state.listCurrency[0].currencyId;
            }
            if (this.state.listPayMethod != null && this.state.listPayMethod.length > 0 && this.props.entity.expense.paymentMethodId == null) {
                this.props.entity.expense.paymentMethodId = this.state.listPayMethod[0].dictionaryDetailId;
            }
            if (this.props.entity.expense.date != null) {
                
                this.props.entity.expense.date = new Date(this.props.entity.expense.date);
               
            }
        }
        super.componentDidUpdate(prevProps);
    }

    onUpload(e) {
        alert(e.xhr.response);

    };

    async populate() {
        var listTVA = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-3'); //VAT
        var listCurrency = await authService.fetchAuth('Lists/GetAllCurrencies');
        var listExpNature = await authService.fetchAuth('Lists/GetExpenseNatureActive');
        var listExpDocType = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-18'); //ExpDocumentType
        var listProvider = await authService.fetchAuth('Lists/GetAllActivePartners');
        var listExpProj = await authService.fetchAuth('Lists/GetExpenseProjects');
        var listPayMethod = await authService.fetchAuth('Lists/GetDictionaryDetailByDictionaryId/-15'); //PaymentMethod

        this.setState({ listTVA: listTVA, listCurrency: listCurrency, listExpNature: listExpNature, listExpDocType: listExpDocType, listProvider: listProvider, listExpProj: listExpProj, listPayMethod: listPayMethod });
    }

    setVatFromExpNature(expNatureId) {
        var obj = this.state.listExpNature.filter(p => p.expenseNature.expenseNatureId == expNatureId)[0];
        if (obj.expenseNature.vatId != null)
        {
            this.props.entity.expense.vatId = obj.expenseNature.vatId;
        }
    }
    async splitExpense() {
        var self = this;
        var r = await this.save();
      
        
        if (r != null) {




            var newR = { ...self.state.entity };
            newR.edit = true;
            newR.expense.date = moment(new Date(newR.expense.date));
             
            newR.expense.price = null;
            newR.expense.expenseId = 0;
            newR.expense.parentExpenseId = r.expense.expenseId;
            self.setState({
                submitted: false,
                entity: newR
            }, function () {
                 
            });




        }
    }
     

    dialogFooter = (
        <React.Fragment>
            <Button label="Renunta"  icon="pi pi-times" className="p-button-text" onClick={this.hide.bind(this)} />

            <Button label="Salveaza " disabled={this.props.enabled==false} icon="pi pi-check" className="p-button-text" onClick={this.save.bind(this)} />
        </React.Fragment>
    );


    render() {
        return (
            <Dialog footer={this.dialogFooter} visible={this.state.entity.edit} style={{ width: '750px' }} header="Cheltuiala" modal className="p-fluid" onHide={this.hide}>
                <BlockUi tag="div" blocking={this.state.loading} >
                <div className="p-grid">
                    <div className="p-col-12 p-md-6">
                        <span className="p-float-label mt-4">
                            <Calendar value={this.state.entity.expense.date} dateFormat="dd.mm.yy" onChange={(e) => this.onInputChange(e.value, 'expense.date')} showIcon showButtonBar ></Calendar>
                            {this.state.submitted && !this.state.entity.expense.date && <small className="p-invalid">Data este obligatorie!</small>}
                            <label >Data*</label>
                        </span>
                        <div className="p-grid">
                            <div className="p-col-5">
                                <span className="p-float-label mt-4">
                                    <InputNumber id="price" value={this.state.entity.expense.price} onChange={(e) => this.onInputChange(e.value, 'expense.price')} autoFocus mode="decimal" minFractionDigits={2} maxFractionDigits={4} />
                                    {this.state.submitted && !this.state.entity.expense.price && <small className="p-invalid">Suma este obligatorie!</small>}
                                    <label htmlFor="price">Suma*</label>
                                </span>
                            </div>
                            <div className="p-col-4">
                                <span className="p-float-label mt-4">
                                    <Dropdown value={this.state.entity.expense.currencyId} options={this.state.listCurrency} onChange={(e) => this.onInputChange(e.value, 'expense.currencyId', 'currencyName', e)} optionLabel="code" optionValue="currencyId" />
                                    {this.state.submitted && !this.state.entity.expense.currencyId && <small className="p-invalid">Moneda este obligatorie!</small>}
                                    <label htmlFor="curr">Moneda</label>
                                </span>
                            </div>
                            <div className="p-col-3">
                                <span className="p-float-label mt-4">
                                    <Dropdown value={this.state.entity.expense.vatId} options={this.state.listTVA} onChange={(e) => this.onInputChange(e.value, 'expense.vatId', 'vat', e)} optionLabel="code" optionValue="dictionaryDetailId" />
                                    <label htmlFor="vat">TVA</label>
                                </span>
                            </div>
                        </div>
                        <span className="p-float-label p-mt-3">
                            <InputText id="descriere" value={this.state.entity.expense.description} onChange={(e) => this.onInputChange(e.target.value, 'expense.description')} />
                            <label htmlFor="descriere">Descriere</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.expense.expenseDocumentTypeId} options={this.state.listExpDocType} onChange={(e) => this.onInputChange(e.value, 'expense.expenseDocumentTypeId', 'expenseDocumentType', e)} optionLabel="name" optionValue="dictionaryDetailId" />
                            <label htmlFor="docType">Tip document</label>
                        </span>
                        <div className="card mt-4">
                            <div className="p-grid">
                                <div className="p-col-4">
                                    <div className="p-field-radiobutton">
                                        Metoda plata:
                                </div>
                                </div>
                                {this.state.listPayMethod != null && this.state.listPayMethod.map((payMethod) => {
                                    return (
                                        <div className="p-col-3">
                                            <div key={payMethod.dictionaryDetailId} className="p-field-radiobutton">
                                                <RadioButton inputId={""+payMethod.dictionaryDetailId} name='payMethod.dictionaryDetailId' value={payMethod.dictionaryDetailId} onChange={(e) => { this.onInputChange(e.value, 'expense.paymentMethodId') }} checked={this.state.entity.expense.paymentMethodId === payMethod.dictionaryDetailId} />
                                                <i style={{ 'font-size': '1.5rem' }} className={payMethod.dictionaryDetailId == -44 ? "pi pi-fw pi-dollar" : "pi pi-fw pi-credit-card"}></i>
                                            </div>
                                        </div>
                                    )
                                })
                                }
                            </div>
                        </div>
                    </div>
                    <div className="p-col-12 p-md-6">                        
                        <span className="p-float-label mt-4">
                            <Dropdown value={this.state.entity.expense.providerId} filter options={this.state.listProvider} onChange={(e) => this.onInputChange(e.value, 'expense.providerId', 'provider', e)} optionLabel="partnerNameCUI" optionValue="partner.partnerId" />
                            <label htmlFor="provider">Partener</label>
                        </span>
                        <span className="p-float-label mt-4">
                            <InputText id="fCode" value={this.state.entity.expense.fiscalCode} onChange={(e) => this.onInputChange(e.target.value, 'expense.fiscalCode')} />
                            <label htmlFor="fCode">Cod Fiscal</label>
                        </span>
                        <span className="p-float-label mt-4">
                                <Dropdown value={this.state.entity.expense.expenseNatureId} filter options={this.state.listExpNature} onChange={(e) => { this.onInputChange(e.value, 'expense.expenseNatureId', 'expenseNature', e); this.setVatFromExpNature(e.value); }} optionLabel="cboxName" optionValue="expenseNature.expenseNatureId" />
                            <label htmlFor="expNature">Natura cheltuiala</label>
                        </span>  
                        <span className="p-float-label mt-4">
                                <Dropdown value={this.state.entity.expense.expenseProjectId} filter options={this.state.listExpProj} onChange={(e) => this.onInputChange(e.value, 'expense.expenseProjectId', 'expenseProject', e)} optionLabel="name" optionValue="expenseProjectId" />
                            <label htmlFor="expProject">Misiune/Serviciu</label>
                        </span>                                              
                        <span className="p-float-label mt-4">
                            <InputText id="docNr" value={this.state.entity.expense.documentNumber} onChange={(e) => this.onInputChange(e.target.value, 'expense.documentNumber')} />
                            <label htmlFor="docNr">Nr. Document</label>
                        </span>
                        <div className="p-grid mt-4">
                            <div className="p-col-4">
                                <Button label="Fractionare cheltuiala" className="p-button-info" onClick={this.splitExpense.bind(this)} />
                            </div>
                            <div className="p-col-4">
                                <FileUpload name="file" className="p-button-info" mode="basic" url="Base/Upload" auto  chooseLabel="Incarca factura" onProgress={(e) => {  }} onError={(e) => { alert('A avut loc o eroare la incarcare!'); }} onUpload={this.onUpload.bind(this)} onUpload={(e) => this.onInputChange(e.xhr.response, 'expense.filePath')} />                                             
                            </div>
                            <div className="p-col-4">
                                {this.state.entity.expense.filePath != null && <Link to={"/" + this.state.entity.expense.filePath} target="_blank"  ><Button icon="pi pi-download" label="Descarca factura" className="p-button p-button-primary" /></Link>}
                                {this.state.submitted && !this.state.entity.expense.filePath  && <small className="p-invalid">Trebuie sa incarcati un document!</small>}
                            </div>                            
                        </div>
                    </div>
                    <div className="p-col-12 p-md-12">
                        <span className="p-float-label">
                            <InputTextarea id="obs" autoResize rows="3" cols="30" value={this.state.entity.expense.extraInfo} onChange={(e) => this.onInputChange(e.target.value, 'expense.extraInfo')} />
                            <label htmlFor="obs">Observatii</label>
                        </span>
                    </div>                   
                    </div>
                </BlockUi>
            </Dialog >
        );
    }
}

export default Expense;