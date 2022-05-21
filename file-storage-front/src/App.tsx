import React, {FC, useEffect} from 'react';
import './App.css';
import './App.scss';
import {BrowserRouter, Route, Switch} from "react-router-dom";
import {Provider, useDispatch} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFileContainer} from "./components/FilesMain/File/OpenFileContainer";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppDispatch, useAppSelector} from "./utils/hooks/reduxHooks";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";
import {modalContents} from "./components/utils/Modal/Modal";
import {Navbar} from "./components/Navbar/Navbar";
import Loading from "./components/utils/Loading/Loading";
import {fetchIsAuth, fetchLogout} from "./redux/thunks/profileThunks";
import {ReactComponent as Logout} from "./assets/logout.svg";
import {RightsManagerPanel} from "./components/RightsManagerPanel/RightsManagerPanel";
import {fetchRightsCurrentUser, fetchRightsDescription, fetchUserCurrent} from "./redux/thunks/rightsThunks";
import {Rights} from "./models/File";
import {Profile} from "./components/Profile/Profile";
import {fetchFilesTypes} from "./redux/thunks/mainThunks";
import DocsClasses from "./components/DocsClasses/DocsClasses";
import {StartPage} from "./components/StartPage/StartPage";
import {Messages} from "./components/utils/Messages/Messages";
import FilesMainContainer from "./components/FilesMain/FilesMainCotainer";

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


const App: FC = () => {
    const dispatch = useDispatch();
    const messages = useAppSelector((state) => state.profile.messages);
    const loading = useAppSelector((state) => state.profile.loading);
    const isAuth = useAppSelector((state) => state.profile.isAuth);
    useEffect(() => {
        dispatch(fetchIsAuth());
    }, []);

    return (<div className="App app">
        {!!messages.length && <Messages messages={messages} className={"app__messages"}/>}
        {isAuth ? <Main/> : loading ? "Загрузка..." : <StartPage/>}
    </div>)
}


const Main: FC = () => {
    const dispatch = useAppDispatch();
    useEffect(() => {
        dispatch(fetchUserCurrent());
        dispatch(fetchFilesTypes());
        dispatch(fetchRightsDescription());
        dispatch(fetchRightsCurrentUser());
    }, [])
    localStorage.setItem("flag", "false");

    const rights = useAppSelector((state) => state.profile.rights);
    const hasTelegram = useAppSelector((state) => state.profile.hasTelegram);
    const loading = useAppSelector((state) => state.filesReducer.loading);

    return (<>
        <Navbar className={"app__navbar"}/>
        {loading && <Loading/>}
        <div className={"app__content"}>
            <header className="header">
                <button className={"header__logout"} onClick={() => dispatch(fetchLogout())}><span>Выйти</span>
                    <Logout/></button>
            </header>
            <div className={"app__content-components"}
                 style={{flex: "1 1 auto", display: "flex", flexDirection: "column"}}>
                <Switch>
                    <Route path={"/Profile"} component={Profile}/>
                    <Route path={"/docs-сlasses"} component={DocsClasses}/>
                    {hasTelegram && <>
                        <Route  path={"/files"} component={FilesMainContainer}/>
                        <Route path={"/file/:id"} component={FilesMainContainer}/>
                        {rights?.includes(Rights["Редактировать права пользователей"]) &&
                        <Route path={"/admin"} component={RightsManagerPanel}/>}
                        {rights?.includes(Rights["Загружать файлы"]) &&
                        <Route path={"/load/"} component={LoadFileMain}/>}
                    </>}
                </Switch>
            </div>

        </div>
    </>)
}


export default FileStorageApp;
