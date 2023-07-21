import React from 'react';
import { InputText } from 'primereact/inputtext';
import { Link } from 'react-router-dom';
import { NavItem, NavLink } from 'reactstrap';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

export const AppTopbar = (props) => {

    const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
    return (
        <div className="layout-topbar clearfix">
            <button type="button" className="p-link layout-menu-button" onClick={props.onToggleMenu}>
                <span className="pi pi-bars" />
            </button>
            <div className="layout-topbar-icons">
                {/*<span className="layout-topbar-search">
                    <InputText type="text" placeholder="Search" />
                    <span className="layout-topbar-search-icon pi pi-search" />
                </span>*/}
                <button type="button" className="p-link">
                    <span className="layout-topbar-item-text">Events</span>
                    <span className="layout-topbar-icon pi pi-calendar" />
                    {/*<span className="layout-topbar-badge">5</span>*/}
                </button>
                <button type="button" className="p-link">
                    <span className="layout-topbar-item-text">Settings</span>
                    <span className="layout-topbar-icon pi pi-cog" />
                </button>
                <Link tag={Link} className="p-link" to={logoutPath} >
                    <button type="button" className="p-link">
                        <span className="layout-topbar-item-text">Logout</span>
                        <span className="layout-topbar-icon pi pi-power-off" />
                    </button>
                </Link>              
            </div>
        </div>
    );
}
