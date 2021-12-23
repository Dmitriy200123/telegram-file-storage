import React, {FC, useEffect} from 'react';
import './App.css';
import './App.scss';
import {BrowserRouter, Link, Redirect, Route, Switch, useLocation} from "react-router-dom";
import {Provider, useDispatch} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppDispatch, useAppSelector} from "./utils/hooks/reduxHooks";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";
import {modalContents} from "./components/utils/Modal/Modal";
import {StartPage} from "./components/StartPage/StartPage";
import {Messages} from "./components/utils/Messages/Messages";
import {Navbar} from "./components/Navbar/Navbar";
import Loading from "./components/utils/Loading/Loading";
import {fetchIsAuth, fetchLogout} from "./redux/thunks/profileThunks";
import {ReactComponent as Logout} from "./assets/logout.svg";
import {fetchData, myHeaders} from "./redux/api/api";
import {RightsManagerPanel} from "./components/RightsManagerPanel/RightsManagerPanel";
import {fetchAllUsers, fetchUserCurrent} from "./redux/thunks/rightsThunks";

const App: FC = () => {
    const dispatch = useDispatch();
    const {profile} = useAppSelector((state) => state);
    const {messages, loading} = profile;
    useEffect(() => {
        dispatch(fetchIsAuth());
    }, [])
    return (<div className="App app">
        {!!messages.length && <Messages messages={messages} className={"app__messages"}/>}
        {profile.isAuth ? <Main/> : loading ? "Загрузка..." : <StartPage/>}
        {/*<Main/>*/}
    </div>)
}

const Main: FC = () => {
    const dispatch = useAppDispatch();
    useEffect(() => {
        dispatch(fetchUserCurrent());
    },[])
    localStorage.setItem("flag", "false");
    const {filesReducer} = useAppSelector((state) => state);
    const {loading, modalConfirm} = filesReducer;
    const {isOpen, id, content} = modalConfirm;
    const Content = modalContents[content || 0];
    return (<>
        <Navbar className={"app__navbar"}/>
        {loading && <Loading/>}
        <div className={"app__content"}>
            <header className="header">
                <button className={"header__logout"} onClick={() => dispatch(fetchLogout())}><span>Выйти</span> <Logout/></button>
            </header>
            <div className={"app__content-components"} style={{flex: "1 1 auto", display: "flex", flexDirection: "column"}}>
                <Switch>
                    <Route path={"/files"} exact component={FilesMain}/>
                    <Route path={"/admin"} component={RightsManagerPanel}/>
                    <Route path={"/file/:id"} component={OpenedFile}/>
                    <Route path={"/load/"} component={LoadFileMain}/>
                    <Route path={"/login"} component={StartPage}/>
                    <Redirect to={"/files"}/>
                </Switch>
            </div>
            {isOpen && id && <Content id={id}/>}
        </div>
    </>)
}


const store = setupStore();

function FileStorageApp() {
    return (
        <BrowserRouter>
            <Provider store={store}>
                <App/>
            </Provider>
        </BrowserRouter>
    );
}

export default FileStorageApp;
