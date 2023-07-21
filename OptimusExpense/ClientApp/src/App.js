import React, { Component, useState, useEffect, useRef, Suspense } from 'react';
import classNames from 'classnames';
import { Route } from 'react-router';
import { useLocation, Switch } from 'react-router-dom';
import { Button } from 'primereact/button';
import { ProgressSpinner } from 'primereact/progressspinner';
import { ScrollPanel } from 'primereact/scrollpanel';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import authService from './components/api-authorization/AuthorizeService';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import { CSSTransition } from 'react-transition-group';
import { Toast } from 'primereact/toast';
import { Dialog } from 'primereact/dialog';
import { AppTopbar } from './AppTopbar';
import { AppFooter } from './AppFooter';
import { AppMenu } from './AppMenu';
import { AppProfile } from './AppProfile';
import { AppConfig } from './AppConfig';
import { HomeApprove } from './components/HomeApprove';
import { HomeUser } from './components/HomeUser';
import { HomeAdmin } from './components/HomeAdmin';
import { HomeCusator } from './components/HomeCusator';
import { HomeCtc } from './components/HomeCtc';
import { HomeMaistru } from './components/HomeMaistru';
import { HomeManager } from './components/HomeManager';
 



import { ChangePassword } from './components/configuration/ChangePassword';
import PrimeReact from 'primereact/api';
import 'primereact/resources/themes/bootstrap4-light-blue/theme.css';
import 'primereact/resources/primereact.min.css';
import 'primeicons/primeicons.css';
import 'primeflex/primeflex.scss';

/*import 'primeflex/primeflex.min.css';*/
import './layout/flags/flags.css';
import './layout/layout.scss';
import './App.scss';


import './custom.css'
import { User } from 'oidc-client';

const App = (props) => {

    const [layoutMode, setLayoutMode] = useState('static');
    const [layoutColorMode, setLayoutColorMode] = useState('light')
    const [staticMenuInactive, setStaticMenuInactive] = useState(false);
    const [overlayMenuActive, setOverlayMenuActive] = useState(false);
    const [mobileMenuActive, setMobileMenuActive] = useState(false);
    const [pages, setPages] = useState([]);
    const [homePage, setHomePage] = useState({ "homeAdmin": HomeAdmin });
    const [menu, setMenu] = useState([]);
    const [inputStyle, setInputStyle] = useState('outlined');
    const [ripple, setRipple] = useState(true);
    const [loading, setLoading] = useState(false);
    const [sidebarActive, setSidebarActive] = useState(true);
    const sidebar = useRef();
    window.toastE = useRef();
    const [displayPositionAlert, setDisplayPositionAlert] = useState(false);
    let componentsHome = { "HomeApprove": HomeApprove, "HomeAdmin": HomeAdmin, "HomeUser": HomeUser, "HomeCusator": HomeCusator, "HomeCtc": HomeCtc, "HomeMaistru": HomeMaistru, "HomeManager": HomeManager };
    let menuClick = false;
     

    useEffect(() => {
        PrimeReact.ripple = ripple;
        if (sidebarActive) {
            addClass(document.body, 'body-overflow-hidden');
        }
        else {
            removeClass(document.body, 'body-overflow-hidden');
        }

        setMenuView();
    }, [sidebarActive]);

    const isSidebarVisible = () => {
        return sidebarActive;
    };

    const onInputStyleChange = (inputStyle) => {
        setInputStyle(inputStyle);
    }

    const setItemMenu = (item1) => {
        var m1 = { label: item1.name };


        if (item1.action != null) {
            if (item1.action != null)
                m1.to = item1.action;
            var pag = pages;
            pag.push(item1);
            setPages(pag);
        }

        if (item1.icon != null) {
            m1.icon = item1.icon;
        }
        if (item1.action == "/" && item1.component != null) {
            //alert(componentsHome[item1.component] + " " + item1.component);
            setHomePage({ "homeAdmin": componentsHome[item1.component] });


        }
        return m1;
    };
    const setMenuView = async () => {
        try {
            const dataMenu = await authService.fetchAuth("Configuration/GetMenus");
            var result = [];
            if (dataMenu != null) {
                dataMenu.map((item, i) => {

                    var m = setItemMenu(item.menu);
                    if (item.subMenus != null) {
                        m.items = [];
                        item.subMenus.map((item1, i1) => {
                            var m1 = setItemMenu(item1);
                            m.items.push(m1);
                        });
                    }
                    result.push(m);
                });
                setMenu(result);
            }
        }
        catch {

        }

    }

    const onRipple = (e) => {
        PrimeReact.ripple = e.value;
        setRipple(e.value)
    }

    const onLayoutModeChange = (mode) => {
        setLayoutMode(mode)
    }

    const onColorModeChange = (mode) => {
        setLayoutColorMode(mode)
    }

    const onWrapperClick = (event) => {
        if (!menuClick && layoutMode === "overlay") {
            setSidebarActive(false);
        }
        menuClick = false;
    }

    const onToggleMenu = (event) => {

        menuClick = true;

        setSidebarActive((prevState) => !prevState);
        event.preventDefault();
    }

    const onSidebarClick = () => {
        menuClick = true;
    }

    const onMenuItemClick = (event) => {
        if (!event.item.items && !isDesktop()) {
            setSidebarActive(false);
        }
    }



    const addClass = (element, className) => {
        if (element.classList)
            element.classList.add(className);
        else
            element.className += ' ' + className;
    }

    const removeClass = (element, className) => {
        if (element.classList)
            element.classList.remove(className);
        else
            element.className = element.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
    }

    const isDesktop = () => {
        return window.innerWidth > 1024;
    }

    //const isSidebarVisible = () => {
    //    if (isDesktop()) {
    //        if (layoutMode === 'static')
    //            return !staticMenuInactive;
    //        else if (layoutMode === 'overlay')
    //            return overlayMenuActive;
    //        else
    //            return true;
    //    }

    //    return true;
    //}

    //const logo = layoutColorMode === 'dark' ? 'assets/layout/images/logo-white.svg' : 'assets/layout/images/logo.svg';
    const logo = layoutColorMode === 'dark' ? 'assets/layout/images/logoOptimus.png' : 'assets/layout/images/logoOptimus.png';

    const wrapperClass = classNames('layout-wrapper', {
        'layout-overlay': layoutMode === 'overlay',
        'layout-static': layoutMode === 'static',
        'layout-static-sidebar-inactive': staticMenuInactive && layoutMode === 'static',
        'layout-overlay-sidebar-active': overlayMenuActive && layoutMode === 'overlay',
        'layout-mobile-sidebar-active': mobileMenuActive,
        'p-input-filled': inputStyle === 'filled',
        'p-ripple-disabled': ripple === false,
        'layout-active': sidebarActive,
    });

    const sidebarClassName = classNames('layout-sidebar', {
        'layout-sidebar-dark': layoutColorMode === 'dark',
        'layout-sidebar-light': layoutColorMode === 'light'
    });

    return (

        <div className={wrapperClass} onClick={onWrapperClick}>

            <AppTopbar onToggleMenu={onToggleMenu} />
            <CSSTransition classNames="layout-sidebar" timeout={{ enter: 200, exit: 200 }} in={isSidebarVisible()} unmountOnExit>
                <div ref={sidebar} className={sidebarClassName} onClick={onSidebarClick}>
                    <div className="layout-logo">
                        <a href="http:\\www.optimus.ro" target="_blank"><img alt="Logo" src={logo} width="180" height="70" /></a>
                    </div>
                    <AppProfile />
                    <AppMenu model={menu} onMenuItemClick={onMenuItemClick} />
                </div>
            </CSSTransition>
            <AppConfig rippleEffect={ripple} onRippleEffect={onRipple} inputStyle={inputStyle} onInputStyleChange={onInputStyleChange}
                layoutMode={layoutMode} onLayoutModeChange={onLayoutModeChange} layoutColorMode={layoutColorMode} onColorModeChange={onColorModeChange} />

            <div className="layout-main">


                <Toast ref={window.toastE} />



                <AuthorizeRoute exact path='/' component={homePage["homeAdmin"]} />

                <Suspense fallback={<div>Page is Loading...</div>}>
                    <Switch>
                        {
                            pages.filter(p => p.action != "/").map((it1, k) => {

                                const comp = React.lazy(() => import('./components' + it1.action + (it1.component != null ? it1.component : '')));
                                return (

                                    <AuthorizeRoute path={it1.action} component={comp} key={it1.userActionId} />
                                )
                            })
                        }
                    </Switch>
                </Suspense>

                <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />


            </div>
            <AppFooter />

        </div>

    );

}


export default App;
