import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Link, Redirect, Route, Switch, useLocation} from "react-router-dom";
import {Provider} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppSelector} from "./utils/hooks/reduxHooks";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";
import {modalContents} from "./components/utils/Modal/Modal";
import {StartPage} from "./components/StartPage/StartPage";
import {Messages} from "./components/utils/Messages/Messages";
import {Navbar} from "./components/Navbar/Navbar";
import Loading from "./components/utils/Loading/Loading";


const App: FC = () => {
    const {profile} = useAppSelector((state) => state);
    const {messages} = profile;
    const location = useLocation();
    console.log(location);
    console.log(new URLSearchParams(location.search).get('token'));
    console.log(new URLSearchParams(location.search).get('refreshToken'));
    return (<div className="App app">
        {!!messages.length && <Messages messages={messages} className={"app__messages"}/>}
        <Switch>
            <Route path={"/login"} component={StartPage}/>
            <Route>
                <Content1 />
            </Route>
        </Switch>
    </div>)
}

const Content1: FC = () => {
    const {filesReducer, profile} = useAppSelector((state) => state);
    const {loading, modalConfirm} = filesReducer;
    const {isOpen, id, content} = modalConfirm;
    const Content = modalContents[content || 0];
    return (<>
        <Navbar className={"app__navbar"}/>
        {loading && <Loading/>}
        <div className={"app__content"}>
            <header className="header"/>
            <div style={{flex: "1 1 auto", display: "flex", flexDirection: "column"}}>
                <Switch>
                    <Route path={"/files"} exact component={FilesMain}/>
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
