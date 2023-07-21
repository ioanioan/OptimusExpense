import React, { useState, useEffect } from 'react';
import classNames from 'classnames';
import { CSSTransition } from 'react-transition-group';
import authService from './components/api-authorization/AuthorizeService';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import { Link, useHistory  } from 'react-router-dom';
import { NavItem, NavLink } from 'reactstrap';
export const AppProfile = () => {

    const [expanded, setExpanded] = useState(false);
    let isAuthenticated = useState(false);
    const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
    const [user, setUser] = useState('');
    const [firma, setFirma] = useState('');
    const onClick = (event) => {
        setExpanded(prevState => !prevState);
        event.preventDefault();
    }

    const populateState = async () => {
        
        const [isAuthen, userN] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        var result = (await authService.fetchAuth('Lists/GetCompanyByUserId'));
        if(result!=null)
            setFirma(result.partner.name);
        isAuthenticated = isAuthen;
    
       
        setUser(userN && userN.name);
        
        if (isAuthenticated && ("" + window.location).indexOf("configuration/ChangePassword") < 0 &&  result.lastLogin==null) {
            window.location = '/configuration/ChangePassword';
        }
   
    }

    useEffect(() => {

       // authService.subscribe(() => populateState());
        populateState();
    });

    return (
        <div className="layout-profile">
            <div>
                <img src="assets/layout/images/profile.png" alt="Profile" width="56" height="56"/>
            </div>
            <button className="p-link layout-profile-link" onClick={onClick}>
                <span className="username">{ user }</span>
                <i className="pi pi-fw pi-cog" />               
            </button>
            <img src={'assets/layout/images/' + firma + '.png'} width="200" />
            <CSSTransition classNames="p-toggleable-content" timeout={{ enter: 1000, exit: 450 }} in={expanded} unmountOnExit>
                <ul className={classNames({ 'layout-profile-expanded': expanded })}>
                    <li><button type="button" className="p-link"><i className="pi pi-fw pi-user" /><span>Account</span></button></li>
                    {/*<li><button type="button" className="p-link"><i className="pi pi-fw pi-inbox" /><span>Notifications</span><span className="menuitem-badge">2</span></button></li>*/}


                    <li>
                        <NavLink tag={Link} className="p-link" to={logoutPath}><button type="button" to className="p-link"><i className="pi pi-fw pi-power-off" /><span>Logout</span></button></NavLink>
                    </li>
                </ul>
            </CSSTransition>
        </div>
    );

}
